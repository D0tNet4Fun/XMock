using System;
using System.Collections.Generic;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XMock.Runners
{
    public class TestClassRunner : XunitTestClassRunner
    {
        public TestClassRunner(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, IDictionary<Type, object> collectionFixtureMappings)
            : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource, collectionFixtureMappings)
        {
        }
    }
}