using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XMock
{
    public class TestCollectionOrderer
    {
        private readonly ITestCollectionOrderer _testCollectionOrderer;

        public TestCollectionOrderer(ITestCollectionOrderer testCollectionOrderer)
        {
            _testCollectionOrderer = testCollectionOrderer;
        }

        public void Order(List<TestCollection> collections)
        {
            // use the test collection orderer to order the unwrapped test collections, 
            // then sort the wrapped test collections using the positions in the list
            var orderedTestCollections = _testCollectionOrderer.OrderTestCollections(collections.Select(c => c.Unwrap())).ToList();

            collections.Sort((x, y) =>
            {
                var collectionX = x.Unwrap();
                var collectionY = y.Unwrap();
                var indexOfX = orderedTestCollections.IndexOf(collectionX);
                var indexOfY = orderedTestCollections.IndexOf(collectionY);
                if (indexOfX == -1)
                    throw new ArgumentException($"Test collection {collectionX.DisplayName} was not found in the list of ordered collections.");
                if (indexOfY == -1)
                    throw new ArgumentException($"Test collection {collectionY.DisplayName} was not found in the list of ordered collections.");
                return indexOfX.CompareTo(indexOfY);
            });
        }
    }
}