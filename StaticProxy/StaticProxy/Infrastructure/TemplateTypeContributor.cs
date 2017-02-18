using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy.Contributors;
using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using System.Reflection;
using PhillipScottGivens.StaticProxy.Templates;

namespace PhillipScottGivens.StaticProxy.Infrastructure
{
    public class TemplateTypeContributor : CompositeTypeContributor
    {
        #region Fields
        private readonly Type targetType;
        private readonly Func<Type, MembersCollector> membersCollectorFactory;
        private readonly MethodInfo templateMethod;
        private readonly IProxyTemplateRegistrar registrar;
        private readonly ConstructorContributor constructorContributor;
        #endregion

        #region Constructor
        public TemplateTypeContributor(Type targetType, 
                Func<Type, MembersCollector> membersCollectorFactory)
        {
            this.targetType = targetType;
            this.membersCollectorFactory = membersCollectorFactory;
        }

        public TemplateTypeContributor(Type targetType, 
                Func<Type, MembersCollector> membersCollectorFactory,
                Action actionAsTemplate)
            : this(targetType, membersCollectorFactory, actionAsTemplate.Method)
        { }

        public TemplateTypeContributor(Type targetType, 
                Func<Type, MembersCollector> membersCollectorFactory,
                MethodInfo templateMethod)
            : this(targetType, membersCollectorFactory)
        {
            this.targetType = targetType;
            this.membersCollectorFactory = membersCollectorFactory;
            this.templateMethod = templateMethod;
        }

        public TemplateTypeContributor(Type targetType, 
                Func<Type, MembersCollector> membersCollectorFactory,
                IProxyTemplateRegistrar registrar,
                ConstructorContributor constructorContributor)
            : this(targetType, membersCollectorFactory)
        {
            this.registrar = registrar;
            this.constructorContributor = constructorContributor;
        }
        #endregion

        protected override IEnumerable<MembersCollector> CollectElementsToProxyInternal(IProxyGenerationHook hook)
        {         
            var targetItem = (membersCollectorFactory != null)
                ? membersCollectorFactory(targetType)
                : new ClassMembersCollector(targetType) { Logger = Logger };
            targetItem.CollectMembersToProxy(hook);
            yield return targetItem;
        }

        public override void Generate(ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope)
        {
            if (constructorContributor != null && registrar != null)
            {
                foreach (var statement in registrar.ConstructorStatements)
                {
                    constructorContributor.AppendConstructorStatement(statement);
                }
            }
            base.Generate(@class, options, namingScope);
        }

        protected override MethodGenerator GetMethodGenerator(
            MetaMethod method,
            ClassEmitter @class,
            ProxyGenerationOptions options,
            OverrideMethodDelegate overrideMethod)
        {
            if (templateMethod != null)
                return new TemplateMethodGenerator(method, overrideMethod, templateMethod);

            if (registrar != null)
                return new TemplateMethodGenerator(method, overrideMethod, registrar.GetTemplate(method.Method));

            return new TemplateMethodGenerator(method, overrideMethod);
        }
    }
}
