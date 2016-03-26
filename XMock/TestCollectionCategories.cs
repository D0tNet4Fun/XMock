using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XMock
{
    internal class TestCollectionCategories
    {
        private readonly SharedContext _sharedContext;

        public TestCollectionCategories(SharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public List<TestCollection> TypemockPragmaticCollections { get; } = new List<TestCollection>();
        public List<TestCollection> TypemockInterfaceOnlyCollections { get; } = new List<TestCollection>();
        public List<TestCollection> OtherCollections { get; } = new List<TestCollection>();

        public void AddTypemockPragmaticTestCases(ICollection<IXunitTestCase> testCases, ITestCollection testCollection)
        {
            AddTestCases(testCases, testCollection, TypemockPragmaticCollections);
        }

        public void AddTypemockInterfaceOnlyTestCases(ICollection<IXunitTestCase> testCases, ITestCollection testCollection)
        {
            AddTestCases(testCases, testCollection, TypemockInterfaceOnlyCollections);
        }

        public void AddOtherTestCases(ICollection<IXunitTestCase> testCases, ITestCollection testCollection)
        {
            AddTestCases(testCases, testCollection, OtherCollections);
        }

        private void AddTestCases(ICollection<IXunitTestCase> testCases, ITestCollection testCollection, ICollection<TestCollection> targetCollections)
        {
            if (testCases.Count == 0)
                return;

            var collection = targetCollections.SingleOrDefault(c => c.Unwrap().UniqueID == testCollection.UniqueID);
            if (collection == null)
            {
                collection = new TestCollection(testCollection);
                targetCollections.Add(collection);
                // add usage on the shared context for the collection
                _sharedContext.AddGlobalUsage(testCollection.UniqueID);
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