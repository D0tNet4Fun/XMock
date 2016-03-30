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

        public void AddCollectionReference(Guid collectionId)
        {
            CollectionReference collectionReference;
            if (!_references.TryGetValue(collectionId, out collectionReference))
            {
                collectionReference = new CollectionReference();
                _references.Add(collectionId, collectionReference);
            }
            collectionReference.Usages++;
        }

        public int RemoveCollectionReference(Guid collectionId)
        {
            var usages = --_references[collectionId].Usages;
            if (usages < 0)
                throw new InvalidOperationException($"Collection {collectionId} was already not referenced anymore.");
            return usages;
        }

        public bool RemoveCollectionReferences(Guid collectionId)
        {
            var collectionReference = _references[collectionId];
            if (collectionReference.ClassFixtures.Count != 0 || collectionReference.CollectionFixtures.Count != 0)
                throw new InvalidOperationException($"Collection {collectionId} cannot be removed because it has fixtures");
            return _references.Remove(collectionId);
        }

        #region Class fixtures
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
        #endregion

        #region Collection fixtures
        public void CacheCollectionFixture(Guid collectionId, Type fixtureType, object instance)
        {
            var collectionReference = _references[collectionId];
            collectionReference.CollectionFixtures.Add(fixtureType, new CollectionReference.Fixture(collectionReference) { Value = instance });
        }

        public object GetCollectionFixture(Guid collectionId, Type fixtureType)
        {
            CollectionReference.Fixture fixture;
            return _references[collectionId].CollectionFixtures.TryGetValue(fixtureType, out fixture) ? fixture.Value : null;
        }

        public void AddCollectionFixtureUsage(Guid collectionId, Type fixtureType)
        {
            _references[collectionId].CollectionFixtures[fixtureType].Usages++;
        }

        public int GetCollectionFixturesUsagesLeft(Guid collectionId)
        {
            var collectionReference = _references[collectionId];
            var result = collectionReference.Usages * collectionReference.CollectionFixtures.Count - collectionReference.CollectionFixtures.Values.Sum(f => f.Usages);
            if (result < 0)
                throw new InvalidOperationException($"Collection {collectionId} has been used more than expected");
            return result;
        }

        public void ClearCollectionFixtures(Guid collectionId)
        {
            _references[collectionId].CollectionFixtures.Clear();
        }

        #endregion

        [DebuggerDisplay("{Usages} usages, {CollectionFixtures.Count} collection fixtures, {ClassFixtures.Count} class fixtures")]
        private class CollectionReference
        {
            public int Usages { get; set; }

            public Dictionary<Type, Fixture> ClassFixtures { get; } = new Dictionary<Type, Fixture>();
            public Dictionary<Type, Fixture> CollectionFixtures { get; } = new Dictionary<Type, Fixture>();

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