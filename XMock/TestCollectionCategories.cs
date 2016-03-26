using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XMock
{
    internal class TestCollectionCategories
    {
        public List<TestCollection> TypemockPragmaticCollections { get; } = new List<TestCollection>();
        public List<TestCollection> TypemockInterfaceOnlyCollections { get; } = new List<TestCollection>();
        public List<TestCollection> OtherCollections { get; } = new List<TestCollection>();

        public void AddTypemockPragmaticTestCases(IEnumerable<IXunitTestCase> testCases, ITestCollection testCollection)
        {
            AddTestCases(testCases, testCollection, TypemockPragmaticCollections);
        }

        public void AddTypemockInterfaceOnlyTestCases(IEnumerable<IXunitTestCase> testCases, ITestCollection testCollection)
        {
            AddTestCases(testCases, testCollection, TypemockInterfaceOnlyCollections);
        }

        public void AddOtherTestCases(IEnumerable<IXunitTestCase> testCases, ITestCollection testCollection)
        {
            AddTestCases(testCases, testCollection, OtherCollections);
        }

        private void AddTestCases(IEnumerable<IXunitTestCase> testCases, ITestCollection testCollection, ICollection<TestCollection> targetCollections)
        {
            var collection = targetCollections.SingleOrDefault(c => c.Unwrap().UniqueID == testCollection.UniqueID);
            if (collection == null)
            {
                collection = new TestCollection(testCollection);
                targetCollections.Add(collection);
            }
            collection.TestCases.AddRange(testCases);
        }

        public void OrderCollections(ITestCollectionOrderer testCollectionOrderer)
        {
            var orderer = new TestCollectionOrderer(testCollectionOrderer);
            foreach (var collection in new[] { TypemockPragmaticCollections, TypemockInterfaceOnlyCollections, OtherCollections })
            {
                orderer.Order(collection);
            }
        }
    }
}