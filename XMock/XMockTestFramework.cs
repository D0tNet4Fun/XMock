using System.Reflection;
using XMock.Runners;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XMock
{
    public class XMockTestFramework : XunitTestFramework
    {
        public XMockTestFramework(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
        }

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            return new TestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
        }
    }
}
