using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XMock.Runners
{
    public class TestCollectionRunner : XunitTestCollectionRunner
    {
        private readonly IMessageSink _diagnosticMessageSink;
        private readonly SharedContext _sharedContext;

        public TestCollectionRunner(ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource,
            SharedContext sharedContext)
            : base(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
            _sharedContext = sharedContext;
        }

        public Guid CollectionId => TestCollection.UniqueID;

        protected override void CreateCollectionFixture(Type fixtureType)
        {
            lock (_sharedContext)
            {
                var instance = _sharedContext.GetCollectionFixture(CollectionId, fixtureType);
                if (instance == null)
                {
                    // use base to create the collection fixture and then cache it
                    base.CreateCollectionFixture(fixtureType);
                    instance = CollectionFixtureMappings[fixtureType];
                    _sharedContext.CacheCollectionFixture(CollectionId, fixtureType, instance);
                }
                else
                {
                    // pass the cached instance to collection fixtures
                    CollectionFixtureMappings.Add(fixtureType, instance);
                }
                _sharedContext.AddCollectionFixtureUsage(CollectionId, fixtureType);
            }
        }

        protected override async Task BeforeTestCollectionFinishedAsync()
        {
            lock (_sharedContext)
            {
                var count = _sharedContext.GetCollectionFixturesUsagesLeft(CollectionId);
                if (count != 0)
                {
                    // remove the collection fixtures to prevent their disposal
                    CollectionFixtureMappings.Clear();
                }
                else
                {
                    // remove the collection fixtures from the shared context and let the base class dispose them
                    _sharedContext.ClearCollectionFixtures(CollectionId);
                }

                // remove all references of the collection from the shared context if it is not in use anymore
                count = _sharedContext.RemoveCollectionReference(CollectionId);
                if (count == 0)
                {
                    _sharedContext.RemoveCollectionReferences(CollectionId);
                }
            }
            await base.BeforeTestCollectionFinishedAsync();
        }

        protected override Task<RunSummary> RunTestClassAsync(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases)
            => new TestClassRunner(testClass, @class, testCases, _diagnosticMessageSink, MessageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), CancellationTokenSource, CollectionFixtureMappings, _sharedContext).RunAsync();
    }
}