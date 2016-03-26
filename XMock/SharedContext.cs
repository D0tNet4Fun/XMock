using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace XMock
{
    public class SharedContext
    {
        private readonly Dictionary<Guid, CollectionReference> _references;

        public SharedContext()
        {
            _references = new Dictionary<Guid, CollectionReference>();
        }

        public void AddGlobalUsage(Guid collectionId)
        {
            CollectionReference collectionReference;
            if (!_references.TryGetValue(collectionId, out collectionReference))
            {
                collectionReference = new CollectionReference();
                _references.Add(collectionId, collectionReference);
            }
            var value = ++collectionReference.Usages;
            if (value <= 0)
                throw new InvalidOperationException($"Collection {collectionId} is not in use anymore");
        }

        public void CacheClassFixture(Guid collectionId, Type fixtureType, object instance)
        {
            var collectionReference = _references[collectionId];
            collectionReference.ClassFixtures.Add(fixtureType, new CollectionReference.Fixture(collectionReference) { Value = instance });
        }

        public object GetClassFixture(Guid collectionId, Type fixtureType)
        {
            CollectionReference.Fixture fixture;
            return _references[collectionId].ClassFixtures.TryGetValue(fixtureType, out fixture) ? fixture.Value : null;
        }

        public void AddClassFixtureUsage(Guid collectionId, Type fixtureType)
        {
            _references[collectionId].ClassFixtures[fixtureType].Usages++;
        }

        public int GetClassFixturesUsagesLeft(Guid collectionId)
        {
            var collectionReference = _references[collectionId];
            var result = collectionReference.Usages * collectionReference.ClassFixtures.Count - collectionReference.ClassFixtures.Values.Sum(f => f.Usages);
            if (result < 0)
                throw new InvalidOperationException($"Collection {collectionId} is used more than expected");
            return result;
        }

        public void ClearClassFixtures(Guid collectionId)
        {
            _references[collectionId].ClassFixtures.Clear();
        }

        [DebuggerDisplay("{Usages} usages, {CollectionFixtures.Count} collection fixtures, {ClassFixtures.Count} class fixtures")]
        private class CollectionReference
        {
            public int Usages { get; set; }

            public Dictionary<Type, Fixture> ClassFixtures { get; } = new Dictionary<Type, Fixture>();

            public class Fixture
            {
                private readonly CollectionReference _owner;
                private int _usages;

                public Fixture(CollectionReference owner)
                {
                    _owner = owner;
                }

                public object Value { get; set; }

                public int Usages
                {
                    get { return _usages; }
                    set
                    {
                        if (_owner.Usages < value)
                            throw new ArgumentOutOfRangeException(nameof(value), "Cannot have more usages than owner");
                        _usages = value;
                    }
                }
            }
        }
    }
}