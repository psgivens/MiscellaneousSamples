using PhillipScottGivens.StaticProxy;
using Xunit;
using PhillipScottGivens.StaticProxy.UnitTests.Samples;


namespace PhillipScottGivens.StaticProxy.UnitTests
{
    public class TemplateProxy_Tests
    {
        [Fact]
        public void Proxy_alter_outcome()
        {
            var thing = TemplateProxyGenerator.GenerateType<ClassToProxy>();
            int outcome = thing.ReturnThirteen();
            Assert.Equal(outcome, 19);
        }

        [Fact]
        public void Proxy_catch_exception()
        {
            var thing = TemplateProxyGenerator.GenerateType<ClassToProxy>();
            int outcome = thing.ThrowException();
            Assert.Equal(outcome, 42);
        }
          //var container = new IoCContainer();
          //  Type thingType = TemplateProxyGenerator.GenerateType<ClassToProxy>();
          //  container.Register<thingType>().As<ClassToProxy>();
          //  var thing = container.Resolve<ClassToProxy>();
          //  int outcome = thing.ThrowException();
          
    }
}
