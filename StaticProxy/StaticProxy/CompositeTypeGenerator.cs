using System;
using Castle.DynamicProxy;
using CastleContributors = Castle.DynamicProxy.Contributors;
using CastleGenerators = Castle.DynamicProxy.Generators;
using CastleProxy = Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using System.Collections.Generic;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using PhillipScottGivens.StaticProxy.Infrastructure;

namespace PhillipScottGivens.StaticProxy
{
    public class CompositeTypeGenerator
    {
        public static ModuleScope CreateModuleScope(string weakAssemblyName, string weakModulePath)
        {
            var savePhysicalAssembly = true;
            var disableSignedAssembly = true;
            var strongAssemblyName = ModuleScope.DEFAULT_ASSEMBLY_NAME;
            var strongModulePath = ModuleScope.DEFAULT_FILE_NAME;

            var scope = new ModuleScope(
                savePhysicalAssembly,
                disableSignedAssembly,
                strongAssemblyName,
                strongModulePath,
                weakAssemblyName,
                weakModulePath);

            return scope;
        }

        public static TClass GenerateType<TClass>(params Type[] parts)
        {
            var weakAssemblyName = "_Composite.Foo.Bar.Proxies";
            var weakModulePath = "_Composite.Proxies.dll";
            var scope = CreateModuleScope(weakAssemblyName, weakModulePath);
            var value = GenerateType<TClass>(scope, parts);

            scope.SaveAssembly();
            return value;
        }

        public static TClass GenerateType<TClass>(ModuleScope scope, params Type[] parts)
        {
            var sampleType = typeof(TClass);

            var constructorContributor = new ConstructorContributor();

            ITypeContributor compositeContributor =
                new PartCompositeTypeContributor(constructorContributor, parts);

            // Create proxy generator
            var proxyGenerator = new ExtensibleClassProxyGenerator(
                scope, "PhillipScottGivens.CompositeProxies",
                sampleType, compositeContributor, constructorContributor);

            // Generate type
            Type proxiedType = proxyGenerator.GenerateCode(new Type[] { },
                new CastleProxy.ProxyGenerationOptions());

            return (TClass)Activator.CreateInstance(proxiedType);
        }



    }
}

