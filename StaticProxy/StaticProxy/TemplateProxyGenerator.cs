using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CastleProxy = Castle.DynamicProxy;
using CastleGenerators = Castle.DynamicProxy.Generators;
using CastleContributors = Castle.DynamicProxy.Contributors;
using System.Reflection;
using Castle.DynamicProxy;
using PhillipScottGivens.StaticProxy.Infrastructure;
using PhillipScottGivens.StaticProxy.Templates;

namespace PhillipScottGivens.StaticProxy
{
    public class TemplateProxyGenerator
    {
        // Warning: Do not modify, this is called through reflection
        public static void GenerateAssembly(
            Assembly proxyAssembly,
            string newAssemblyName,
            bool ignoreAttributes,
            List<string> targetAssemblies,
            List<string> templates,
            string @namespace)
        {
            newAssemblyName += ".dll";
            ModuleScope scope = CreateModuleScope(newAssemblyName, newAssemblyName);
            foreach (Type type in proxyAssembly.GetTypes())
                GenerateType(scope, @namespace, type);

            scope.SaveAssembly();
        }

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

        public static TClass GenerateType<TClass>()
        {
            var weakAssemblyName = "Composite.Foo.Bar.Proxies";
            var weakModulePath = "Composite.Proxies.dll";
            var scope = CreateModuleScope(weakAssemblyName, weakModulePath);
            var value = GenerateType<TClass>(scope);

            scope.SaveAssembly();
            return value;
        }
        
        public static TClass GenerateType<TClass>(ModuleScope scope)
        {
            var sampleType = typeof(TClass);
            Type proxiedType = GenerateType(scope, "PhillipScottGivens.TemplateProxies", sampleType);
            return (TClass)Activator.CreateInstance(proxiedType);
        }

        public static Type GenerateType(ModuleScope scope, string proxyNamespace, Type sampleType)
        {
            var attributes = sampleType.GetCustomAttributes(typeof(AspectTemplateAttribute), true);
            if (attributes.Length < 1)
                return null;
            //    throw new ProxyGenerationException("Proxies must be decorated with an AspectTemplateAttribute");

            var aspectAttribute = (AspectTemplateAttribute)attributes[0];

            Type registrarGenericType = typeof(ProxyTemplateRegistrar<>);
            Type registrarType = registrarGenericType.MakeGenericType(aspectAttribute.TemplateType);
            var registrar = Activator.CreateInstance(registrarType);
            Activator.CreateInstance(aspectAttribute.TemplateType, registrar);

            var constructorContributor = new ConstructorContributor();

            ITypeContributor templateContributorFactory
                = new TemplateTypeContributor(sampleType, innerType
                    => new CastleContributors.ClassMembersCollector(innerType),
                     (IProxyTemplateRegistrar)registrar, constructorContributor);

            // Create proxy generator
            var proxyGenerator = new ExtensibleClassProxyGenerator(
                scope, proxyNamespace,
                sampleType, templateContributorFactory, constructorContributor);

            // Generate type
            return proxyGenerator.GenerateCode(new Type[] { }, new CastleProxy.ProxyGenerationOptions());

        }
    }
}
