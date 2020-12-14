using System;
using System.Reflection;
using Xunit.Sdk;

namespace XMock.Discovery
{
    internal static class ReflectionUtils
    {
        public static SerializationHelper SerializationHelper { get; } = new SerializationHelper();
    }

    internal class SerializationHelper
    {
        private static readonly Type _type;
        private static readonly MethodInfo _method;

        static SerializationHelper()
        {
            _type = typeof(XunitTestFrameworkDiscoverer).Assembly.GetType("Xunit.Sdk.SerializationHelper");
            _method = _type.GetMethod("GetType", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(string), typeof(string) }, null);
        }

        public Type GetType(string assemblyName, string typeName)
        {
            return (Type)_method.Invoke(null, new object[] { assemblyName, typeName });
        }
    }
}