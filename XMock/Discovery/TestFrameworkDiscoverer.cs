using System.Collections.Generic;
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

        public override string Serialize(ITestCase testCase)
        {
            UpdateTraits(testCase);
            return base.Serialize(testCase);
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