using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using PhillipScottGivens.StaticProxy;
using PhillipScottGivens.StaticProxy.UnitTests.Samples;

namespace PhillipScottGivens.StaticProxy.UnitTests
{
    public class Composite_Tests
    {
        [Fact]
        public void Proxy_alter_outcome()
        {
            var thing = CompositeTypeGenerator.GenerateType<ClassToProxy>(typeof(ClassA), typeof(ClassB), typeof(ClassC));
            int outcome = thing.ReturnThirteen();
            Assert.True(thing is IHasPart<ClassA>);
            Assert.NotNull(((IHasPart<ClassA>)thing).Part);
        }
    }
}
