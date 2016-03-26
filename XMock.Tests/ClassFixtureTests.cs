using System;
using System.Collections.Generic;
using TypeMock.ArrangeActAssert;
using Xunit;

namespace XMock.Tests
{

    public class ClassFixture : IDisposable
    {
        public ClassFixture()
        {
            InstanceCount++;
        }

        public static int InstanceCount { get; private set; }
        public void Dispose()
        {

        }
    }

    public class ClassFixture2 : IDisposable
    {
        public ClassFixture2()
        {
            InstanceCount++;
        }

        public static int InstanceCount { get; private set; }
        public void Dispose()
        {

        }
    }

    public class ClassWithOneFixture1 : IClassFixture<ClassFixture>
    {
        private readonly ClassFixture _fixture;

        public ClassWithOneFixture1(ClassFixture fixture)
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

    public class ClassWithOneFixture2 : IClassFixture<ClassFixture>
    {
        private readonly ClassFixture _fixture;

        public ClassWithOneFixture2(ClassFixture fixture)
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

    public class ClassWithManyFixtures1 : IClassFixture<ClassFixture>, IClassFixture<ClassFixture2>
    {
        private readonly ClassFixture _fixture;

        public ClassWithManyFixtures1(ClassFixture fixture)
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

    public class ClassWithManyFixtures2 : IClassFixture<ClassFixture>, IClassFixture<ClassFixture2>
    {
        private readonly ClassFixture _fixture;

        public ClassWithManyFixtures2(ClassFixture fixture)
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