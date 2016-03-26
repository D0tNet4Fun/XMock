using System;
using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XMock
{
    public class TestCollection
    {
        private readonly ITestCollection _testCollection;

        public TestCollection(ITestCollection testCollection)
        {
            if (testCollection == null)
            {
                throw new ArgumentNullException(nameof(testCollection));
            }
            _testCollection = testCollection;
        }

        public List<IXunitTestCase> TestCases { get; } = new List<IXunitTestCase>();

        public ITestCollection Unwrap()
        {
            return _testCollection;
        }
    }
}