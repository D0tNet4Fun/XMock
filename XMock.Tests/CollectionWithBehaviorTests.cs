using System;
using System.Reflection;
using TypeMock.ArrangeActAssert;
using XMock;
using XMock.Tests;
using Xunit;
using Xunit.Sdk;

[assembly: XMockConfiguration(TypemockCollectionDefinition = typeof(TestCollectionWithBehavior))]

namespace XMock.Tests
{
    public class MyBeforeAfterTestAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
        }

        public override void After(MethodInfo methodUnderTest)
        {
            base.After(methodUnderTest);
        }
    }

    [CollectionDefinition("MyCollectionWithBehavior")]
    [MyBeforeAfterTest]
    public class TestCollectionWithBehavior : ICollectionFixture<CollectionFixture>
    {

    }

    public class ClassWithCollectionWithBehavior
    {
        private readonly CollectionFixture _fixture;

        public ClassWithCollectionWithBehavior(CollectionFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Isolated]
        public void A1()
        {
            TestUtils.Sleep();
        }

        [Fact]
        [Isolated(DesignMode.InterfaceOnly)]
        public void A2()
        {
            TestUtils.Sleep();
        }

        [Fact]
        public void A3()
        {
            TestUtils.Sleep();
        }
    }

    [Collection("MyCollectionWithBehavior")]
    public class ClassWithCollectionWithUserDefinedBehavior
    {
        private readonly CollectionFixture _fixture;

        public ClassWithCollectionWithUserDefinedBehavior(CollectionFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Isolated]
        public void A1()
        {
            TestUtils.Sleep();
        }

        [Fact]
        [Isolated(DesignMode.InterfaceOnly)]
        public void A2()
        {
            TestUtils.Sleep();
        }

        [Fact]
        public void A3()
        {
            TestUtils.Sleep();
        }
    }
}