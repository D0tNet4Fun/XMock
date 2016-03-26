using TypeMock.ArrangeActAssert;
using Xunit;

namespace XMock.Tests.Simple
{
    [Isolated(DesignMode.Pragmatic)]
    public class OnlyTypemockPragmaticTests1
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

    [Isolated(DesignMode.Pragmatic)]
    public class OnlyTypemockPragmaticTests2
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