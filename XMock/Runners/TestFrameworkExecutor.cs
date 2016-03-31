using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XMock.Runners
{
    public class TestFrameworkExecutor : XunitTestFrameworkExecutor
    {
        public TestFrameworkExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
        {
        }

        protected override async void RunTestCases(IEnumerable<IXunitTestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
        {
            var testAssembly = new TestAssembly(AssemblyInfo, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            using (var assemblyRunner = new TestAssemblyRunner(testAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions, new SharedContext()))
            {
                try
                {
                    await assemblyRunner.RunAsync();
                }
                catch (Exception e)
                {
                    DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Assembly runner threw unhandled exception: {e}"));
                    throw;
                }
            }
        }
    }
}