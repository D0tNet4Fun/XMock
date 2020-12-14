using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XMock.Discovery
{
    internal class TestFrameworkDiscoverer : XunitTestFrameworkDiscoverer
    {
        public TestFrameworkDiscoverer(IAssemblyInfo assemblyInfo, ISourceInformationProvider sourceProvider, IMessageSink diagnosticMessageSink, IXunitTestCollectionFactory collectionFactory = null)
            : base(assemblyInfo, sourceProvider, diagnosticMessageSink, collectionFactory)
        {
        }

        protected override bool FindTestsForMethod(ITestMethod testMethod, bool includeSourceInformation, IMessageBus messageBus, ITestFrameworkDiscoveryOptions discoveryOptions)
        {
            // copy-paste from base class...

            var factAttributes = testMethod.Method.GetCustomAttributes(typeof(FactAttribute)).ToList();
            if (factAttributes.Count > 1)
            {
                var message = $"Test method '{testMethod.TestClass.Class.Name}.{testMethod.Method.Name}' has multiple [Fact]-derived attributes";
                var testCase = new ExecutionErrorTestCase(DiagnosticMessageSink, TestMethodDisplay.ClassAndMethod, testMethod, message);
                return ReportDiscoveredTestCase(testCase, includeSourceInformation, messageBus);
            }

            var factAttribute = factAttributes.FirstOrDefault();
            if (factAttribute == null)
                return true;

            var testCaseDiscovererAttribute = factAttribute.GetCustomAttributes(typeof(XunitTestCaseDiscovererAttribute)).FirstOrDefault();
            if (testCaseDiscovererAttribute == null)
                return true;

            var args = testCaseDiscovererAttribute.GetConstructorArguments().Cast<string>().ToList();
            var discovererType = ReflectionUtils.SerializationHelper.GetType(args[1], args[0]);
            if (discovererType == null)
                return true;

            var discoverer = GetDiscoverer(discovererType);
            if (discoverer == null)
                return true;

            foreach (var testCase in discoverer.Discover(discoveryOptions, testMethod, factAttribute))
            {
                UpdateTraits(testCase);
                if (!ReportDiscoveredTestCase(testCase, includeSourceInformation, messageBus))
                    return false;
            }

            return true;
        }

        private static void UpdateTraits(ITestCase testCase)
        {
            // if this test case is [Isolated], or if the test class is [Isolated], then add a trait to specify the isolation level

            var designMode = testCase.TestMethod.Method.GetMethodIsolatedDesignMode() ?? testCase.TestMethod.TestClass.Class.GetClassIsolatedDesignMode();
            if(designMode == null) return;

            if(!testCase.Traits.TryGetValue("Typemock", out var list))
            {
                list = new List<string>();
                testCase.Traits["Typemock"] = list;
            }
            list.Add(designMode.ToString());
        }
    }
}