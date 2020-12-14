using System;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;

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

        public static TypemockDesignMode? GetClassIsolatedDesignMode(this ITypeInfo testClassInfo)
        {
            var isolatedAttribute = testClassInfo.GetCustomAttributes(TypemockHelper.IsolatedAttributeType).SingleOrDefault();
            return isolatedAttribute?.GetNamedArgument<TypemockDesignMode>("Design");
        }

        public static TypemockDesignMode? GetMethodIsolatedDesignMode(this IMethodInfo testMethodInfo)
        {
            var isolatedAttribute = testMethodInfo.GetCustomAttributes(TypemockHelper.IsolatedAttributeType).SingleOrDefault();
            return isolatedAttribute?.GetNamedArgument<TypemockDesignMode>("Design");
        }
    }

    internal enum TypemockDesignMode
    {
        Pragmatic,
        InterfaceOnly,
    }
}