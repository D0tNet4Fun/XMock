using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XMock
{
    public class TestCollection
    {
        private readonly ITestCollection _testCollection;

        public TestCollection(ITestCollection testCollection)
            : this(testCollection, Enumerable.Empty<IXunitTestCase>())
        {
        }

        public TestCollection(ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases)
        {
            if (testCollection == null)
            {
                throw new ArgumentNullException(nameof(testCollection));
            }
            _testCollection = testCollection;
            TestCases = new List<IXunitTestCase>(testCases);
        }

        public List<IXunitTestCase> TestCases { get; }

        public ITestCollection Unwrap()
        {
            return _testCollection;
        }

        public void ChangeCollectionDefinition(Type collectionDefinition)
        {
            ChangeCollectionDefinition(_testCollection, collectionDefinition);
            foreach (var testCase in TestCases)
            {
                ChangeCollectionDefinition(testCase.TestMethod.TestClass.TestCollection, collectionDefinition);
            }
        }

        private static void ChangeCollectionDefinition(ITestCollection testCollection, Type collectionDefinition)
        {
            var xunitTestCollection = testCollection as Xunit.Sdk.TestCollection;
            if (xunitTestCollection != null)
            {
                xunitTestCollection.CollectionDefinition = collectionDefinition != null ? new ReflectionTypeInfo(collectionDefinition) : null;
            }
        }
    }
}