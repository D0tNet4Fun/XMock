using TypeMock.ArrangeActAssert;
using Xunit;

namespace XMock.Tests.Simple
{
    public class MixedTests1
    {
        [Fact]
        [Isolated(DesignMode.Pragmatic)]
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

    public class MixedTests2
    {
        [Fact]
        [Isolated(DesignMode.Pragmatic)]
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