using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XMock.Runners
{
    public class TestClassRunner : XunitTestClassRunner
    {
        private readonly SharedContext _sharedContext;

        public TestClassRunner(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, IDictionary<Type, object> collectionFixtureMappings,
            SharedContext sharedContext)
            : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource, collectionFixtureMappings)
        {
            _sharedContext = sharedContext;
        }

        public Guid CollectionId => TestClass.TestCollection.UniqueID;

        protected override void CreateClassFixture(Type fixtureType)
        {
            lock (_sharedContext)
            {
                var instance = _sharedContext.GetClassFixture(CollectionId, fixtureType);
                if (instance == null)
                {
                    // use base to create the class fixture and then cache it
                    base.CreateClassFixture(fixtureType);
                    instance = ClassFixtureMappings[fixtureType];
                    _sharedContext.CacheClassFixture(CollectionId, fixtureType, instance);
                }
                else
                {
                    // pass the cached instance to class fixtures
                    ClassFixtureMappings.Add(fixtureType, instance);
                }
                _sharedContext.AddClassFixtureUsage(CollectionId, fixtureType);
            }
        }

        protected override async Task BeforeTestClassFinishedAsync()
        {
            lock (_sharedContext)
            {
                var count = _sharedContext.GetClassFixturesUsagesLeft(CollectionId);
                if (count != 0)
                {
                    // remove the class fixtures to prevent their disposal
                    ClassFixtureMappings.Clear();
                }
                else
                {
                    // remove the class fixtures from the shared context and let the base class dispose them
                    _sharedContext.ClearClassFixtures(CollectionId);
                }
            }
            await base.BeforeTestClassFinishedAsync();
        }
    }
}