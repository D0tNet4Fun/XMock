using System;
using TypeMock.ArrangeActAssert;
using Xunit;

namespace XMock.Tests
{

    public class CollectionFixture : IDisposable
    {
        private bool _disposed;

        public CollectionFixture()
        {
            InstanceCount++;
        }

        public static int InstanceCount { get; private set; }

        public int Get()
        {
            if (_disposed) throw new ObjectDisposedException("disposed");
            return 0;
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }

    public class CollectionFixture2 : IDisposable
    {
        private bool _disposed;

        public CollectionFixture2()
        {
            InstanceCount++;
        }

        public static int InstanceCount { get; private set; }

        public int Get()
        {
            if (_disposed) throw new ObjectDisposedException("disposed 2");
            return 0;
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }

    [CollectionDefinition("MyCollection")]
    public class TestCollection : ICollectionFixture<CollectionFixture>, ICollectionFixture<CollectionFixture2>
    {

    }

    [Collection("MyCollection")]
    public class ClassWithCollectionFixture1
    {
        private readonly CollectionFixture _fixture;

        public ClassWithCollectionFixture1(CollectionFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Isolated]
        public void A1()
        {
            TestUtils.Sleep();
            _fixture.Get();
        }

        [Fact]
        [Isolated(DesignMode.InterfaceOnly)]
        public void A2()
        {
            TestUtils.Sleep();
            _fixture.Get();
        }

        [Fact]
        public void A3()
        {
            TestUtils.Sleep();
            _fixture.Get();
        }
    }

    [Collection("MyCollection")]
    public class ClassWithCollectionFixture2
    {
        private readonly CollectionFixture _fixture;

        public ClassWithCollectionFixture2(CollectionFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Isolated]
        public void A1()
        {
            TestUtils.Sleep();
            _fixture.Get();
        }

        [Fact]
        [Isolated(DesignMode.InterfaceOnly)]
        public void A2()
        {
            TestUtils.Sleep();
            _fixture.Get();
        }

        [Fact]
        public void A3()
        {
            TestUtils.Sleep();
            _fixture.Get();
        }
    }
}