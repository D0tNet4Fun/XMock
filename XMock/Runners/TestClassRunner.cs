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

        protected override async Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, object[] constructorArguments)
        {
            var runSummary = await base.RunTestMethodAsync(testMethod, method, testCases, constructorArguments);
            if (Aggregator.HasExceptions)
            {
                var ex = Aggregator.ToException();
                if (ex is TestClassException && TestClass.TestCollection.CollectionDefinition == null)
                {
                    DiagnosticMessageSink.OnMessage(new DiagnosticMessage(
                        $"Error running test method \"{testMethod.Method.Name}\" in class \"{TestClass.Class.Name}\". " +
                        "The class instance could not be created because the constructor arguments could not be resolved. " +
                        "This happens when the XMock configuration specifies a Typemock collection definition, the class is not part of a user-defined collection and it contains both Typemock tests and other tests. " +
                        "To workaround this, either move the other tests to a different class or do not use constructor arguments."));
                }
            }
            return runSummary;
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