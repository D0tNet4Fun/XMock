using TypeMock.ArrangeActAssert;
using Xunit;

namespace XMock.Tests.Simple
{
    [Isolated(DesignMode.InterfaceOnly)]
    public class OnlyTypemockInterfaceOnlyTests1
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

    [Isolated(DesignMode.InterfaceOnly)]
    public class OnlyTypemockInterfaceOnlyTests2
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