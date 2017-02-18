using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Contributors;
using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;


namespace PhillipScottGivens.StaticProxy.Infrastructure
{
    public class ExtensibleClassProxyGenerator : BaseProxyGenerator
    {
        #region Fields
        private readonly ITypeContributor[] contributors;
        private readonly string proxyNamespace;
        #endregion

        public ExtensibleClassProxyGenerator(ModuleScope scope, string proxyNamespace, Type targetType, params  ITypeContributor[] contributors)
            : base(scope, targetType)
        {
            this.contributors = contributors;
            this.proxyNamespace = proxyNamespace;
        }

        public Type GenerateCode(Type[] interfaces, ProxyGenerationOptions options)
        {
            // make sure ProxyGenerationOptions is initialized
            options.Initialize();

            interfaces = TypeUtil.GetAllInterfaces(interfaces).ToArray();
            CheckNotGenericTypeDefinitions(interfaces, "interfaces");
            ProxyGenerationOptions = options;
            var cacheKey = new CacheKey(targetType, interfaces, options);
            return ObtainProxyType(cacheKey, (n, s) => GenerateType(n, interfaces, s));
        }

        protected virtual Type GenerateType(string name, Type[] interfaces, INamingScope namingScope)
        {
            //var implementedInterfaces = GetTypeImplementerMapping(interfaces, out contributors, namingScope);
            var implementedInterfaces = Enumerable.Empty<Type>();

            var model = new MetaType();
            // Collect methods
            foreach (var contributor in contributors)
            {
                contributor.CollectElementsToProxy(ProxyGenerationOptions.Hook, model);
            }
            ProxyGenerationOptions.Hook.MethodsInspected();

            var emitter = BuildClassEmitter(name, targetType, implementedInterfaces);

            CreateOptionsField(emitter);

            // Constructor
            var cctor = GenerateStaticConstructor(emitter);

            var constructorArguments = new List<FieldReference>();
            foreach (var contributor in contributors)
            {
                contributor.Generate(emitter, ProxyGenerationOptions, namingScope);
            }

            //// The following code was temporarily removed.
            //System.Diagnostics.Debugger.Break();
            //// TODO: Add this call back in and handle the field setters for 
            ////          the composite type.
            //GenerateConstructors(emitter, targetType, constructorArguments.ToArray());

            // Complete type initializer code body
            CompleteInitCacheMethod(cctor.CodeBuilder);

            // Crosses fingers and build type
            var proxyType = emitter.BuildType();
            InitializeStaticFields(proxyType);
            return proxyType;
        }

        //protected virtual IEnumerable<Type> GetTypeImplementerMapping(
        //    Type[] interfaces, out IEnumerable<ITypeContributor> contributors, INamingScope namingScope)
        //{
        //    List<ITypeContributor> list = new List<ITypeContributor>();
        //    List<Type> ret = new List<Type>();
        //    foreach (var contributorFactory in contributorFactories)
        //        list.Add(contributorFactory(targetType, namingScope));
        //    contributors = list;
        //    return ret;
        //}

        protected Type ObtainProxyType(CacheKey cacheKey, Func<string, INamingScope, Type> factory)
        {
            using (var locker = Scope.Lock.ForReadingUpgradeable())
            {
                var cacheType = GetFromCache(cacheKey);
                if (cacheType != null)
                {
                    Logger.DebugFormat("Found cached proxy type {0} for target type {1}.", cacheType.FullName, targetType.FullName);
                    return cacheType;
                }

                // Upgrade the lock to a write lock, then read again. This is to avoid generating duplicate types
                // under heavy multithreaded load.
                locker.Upgrade();

                cacheType = GetFromCache(cacheKey);
                if (cacheType != null)
                {
                    Logger.DebugFormat("Found cached proxy type {0} for target type {1}.", cacheType.FullName, targetType.FullName);
                    return cacheType;
                }

                // Log details about the cache miss
                Logger.DebugFormat("No cached proxy type was found for target type {0}.", targetType.FullName);
                EnsureOptionsOverrideEqualsAndGetHashCode(ProxyGenerationOptions);

                var name = Scope.NamingScope.GetUniqueName(
                    string.Format("{0}.{1}Proxy", proxyNamespace, targetType.Name));
                var proxyType = factory.Invoke(name, Scope.NamingScope.SafeSubScope());

                AddToCache(cacheKey, proxyType);
                return proxyType;
            }
        }
    }
}
