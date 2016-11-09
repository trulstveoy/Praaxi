using Core;
using Web.Contracts.Hello;
using Xunit;

namespace Tests
{
    public class Class1
    {
        [Fact]
        public void PassingTest()
        {
           var p = new ControllerProxy("");
            p.DoShit();
        }
    }
}
