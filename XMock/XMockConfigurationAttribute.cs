using System;

namespace XMock
{
    /// <summary>
    /// Used to specify configuration options for a test assembly which uses <see cref="XMockTestFramework"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class XMockConfigurationAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the Xunit collection definition to be used when running Typemock test cases which are not part of a user-defined collection.
        /// </summary>
        public Type TypemockCollectionDefinition { get; set; }
    }
}