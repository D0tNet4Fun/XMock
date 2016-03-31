using System;
using System.Reflection;

namespace XMock
{
    internal static class TypemockHelper
    {
        private static Type _isolatedAttributeType, _isolateType;
        private static MethodInfo _isolateCleanupMethod;

        public static Type IsolatedAttributeType =>
            _isolatedAttributeType ?? (_isolatedAttributeType = LoadTypemockType("TypeMock.ArrangeActAssert.IsolatedAttribute"));

        public static Type IsolateType =>
            _isolateType ?? (_isolateType = LoadTypemockType("TypeMock.ArrangeActAssert.Isolate"));

        public static MethodInfo IsolateCleanupMethodInfo =>
            _isolateCleanupMethod ?? (_isolateCleanupMethod = IsolateType.GetMethod("CleanUp", BindingFlags.Public | BindingFlags.Static));

        public static void InvokeIsolateCleanup() =>
            IsolateCleanupMethodInfo.Invoke(null, null);

        private static Type LoadTypemockType(string typeFullName) =>
            Type.GetType($"{typeFullName}, Typemock.ArrangeActAssert");
    }

    internal enum TypemockDesignMode
    {
        Pragmatic,
        InterfaceOnly,
    }
}