using System.Reflection;
using XMock.Runners;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XMock
{
    /// <summary>
    /// Implements a custom test framework executor which prioritizes Typemock tests and runs them synchronously.
    /// </summary>
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
