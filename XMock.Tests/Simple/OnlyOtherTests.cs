using Xunit;

namespace XMock.Tests.Simple
{
    public class OnlyOtherTests1
    {
        [Fact]
        public void A1()
        {
            TestUtils.Sleep();
        }

        [Fact]
        public void A2()
        {
            TestUtils.Sleep();
        }
    }

    public class OnlyOtherTests2
    {
        [Fact]
        public void A1()
        {
            TestUtils.Sleep();
        }

        [Fact]
        public void A2()
        {
            TestUtils.Sleep();
        }
    }

    public class OnlyOtherTests3
    {
        [Fact]
        public void A1()
        {
            TestUtils.Sleep();
        }

        [Fact]
        public void A2()
        {
            TestUtils.Sleep();
        }
    }
}