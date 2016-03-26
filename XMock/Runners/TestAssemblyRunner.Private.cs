using System.Reflection;
using System.Threading;
using Xunit.Sdk;

namespace XMock.Runners
{
    public partial class TestAssemblyRunner
    {
        private static readonly FieldInfo OriginalSyncContextField = typeof(XunitTestAssemblyRunner).GetField("originalSyncContext", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo DisableParallelizationField = typeof(XunitTestAssemblyRunner).GetField("disableParallelization", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo MaxParallelThreadsField = typeof(XunitTestAssemblyRunner).GetField("maxParallelThreads", BindingFlags.Instance | BindingFlags.NonPublic);

        private SynchronizationContext originalSyncContext
        {
            set { OriginalSyncContextField.SetValue(this, value); }
        }

        private bool disableParallelization
        {
            get { return (bool)DisableParallelizationField.GetValue(this); }
        }

        private int maxParallelThreads
        {
            get { return (int)MaxParallelThreadsField.GetValue(this); }
        }
    }
}