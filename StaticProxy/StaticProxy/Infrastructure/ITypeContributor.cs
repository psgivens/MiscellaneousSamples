#region Assembly Castle.Core.dll, v3.1.0.0
// E:\HgSC\psgivens\sandbox\StaticProxy\Castle.Core.3.1.0\net40-client\Castle.Core.dll
#endregion

using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy.Generators.Emitters;
using System;

namespace PhillipScottGivens.StaticProxy.Infrastructure
{
    // Summary:
    //     Interface describing elements composing generated type
    public interface ITypeContributor
    {
        void CollectElementsToProxy(IProxyGenerationHook hook, MetaType model);
        void Generate(ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope);
    }
}
