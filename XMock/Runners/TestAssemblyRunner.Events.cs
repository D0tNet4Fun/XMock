namespace XMock.Runners
{
    public partial class TestAssemblyRunner
    {
        protected virtual void OnTypemockPragmaticTestsStarting()
        {
        }

        protected virtual void OnTypemockPragmaticTestsFinished()
        {
        }

        protected virtual void OnTypemockInterfaceOnlyTestsStarting()
        {
        }

        protected virtual void OnTypemockInterfaceOnlyTestsFinished()
        {
        }

        protected virtual void OnAllTypemockTestsFinished()
        {
            // clean up after Typemock
            TypemockHelper.InvokeIsolateCleanup();
        }

        protected virtual void OnOtherTestsStarting()
        {
        }

        protected virtual void OnOtherTestsFinished()
        {
        }
    }
}