using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using PhillipScottGivens.StaticProxy.Infrastructure;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy;

namespace PhillipScottGivens.StaticProxy.Templates
{
    public class ProxyTemplateRegistrar<TProxyTemplate> : IProxyTemplateRegistrar
    {
        private Func<MethodInfo, Expression<Action<TProxyTemplate>>> selector;
        private List<Statement> constructorStatements = new List<Statement>();
        private Action<ClassEmitter, ProxyGenerationOptions> pregenerateMethod;
        private Action<ClassEmitter, ProxyGenerationOptions> postgenerateMethod;
        private List<Type> typesToProxy = new List<Type>();

        public void RegisterSelector(Func<MethodInfo, Expression<Action<TProxyTemplate>>> selector)
        {
            if (this.selector != null)
                throw new ProxyGenerationException("Selector was already registered for template: "
                    + typeof(TProxyTemplate));

            this.selector = selector;
        }
        public void RegisterPregenerate(Action<ClassEmitter, ProxyGenerationOptions> generateMethod)
        {
            if (this.pregenerateMethod != null)
                throw new ProxyGenerationException("Pre-generate method was already registered for template: "
                    + typeof(TProxyTemplate));

            this.pregenerateMethod = generateMethod;
        }
        public void RegisterPostgenerate(Action<ClassEmitter, ProxyGenerationOptions> generateMethod)
        {
            if (this.postgenerateMethod != null)
                throw new ProxyGenerationException("Post-generate method was already registered for template: "
                    + typeof(TProxyTemplate));

            this.postgenerateMethod = generateMethod;
        }
        public void AppendConstructorStatement(Statement statement)
        {
            constructorStatements.Add(statement);
        }
        public void AddTypeToProxy<TType>()
        {
            typesToProxy.Add(typeof(TType));
        }
        public void AddTypesToProxy(params Type[] types)
        {
            typesToProxy.AddRange(types);
        }

        // http://stackoverflow.com/questions/8044496/using-a-strongly-typed-method-as-an-argument-without-specifying-parameters
        MethodInfo IProxyTemplateRegistrar.GetTemplate(MethodInfo method)
        {
            Expression<Action<TProxyTemplate>> expression = selector(method);

            var methodExpression = expression.Body as MethodCallExpression;
            if (methodExpression == null)
                throw new ProxyGenerationException("Expression created in RegisterSelector must be a MethodCallExpression.");

            return methodExpression.Method;
        }
        IEnumerable<Statement> IProxyTemplateRegistrar.ConstructorStatements
        {
            get
            {
                return constructorStatements;
            }
        }


        void IProxyTemplateRegistrar.Pregenerate(ClassEmitter @class, ProxyGenerationOptions options)
        {
            if (pregenerateMethod != null)
                pregenerateMethod(@class, options);
        }

        void IProxyTemplateRegistrar.Postgenerate(ClassEmitter @class, ProxyGenerationOptions options)
        {
            if (postgenerateMethod != null)
                postgenerateMethod(@class, options);
        }
    }

    public interface IProxyTemplateRegistrar
    {
        MethodInfo GetTemplate(MethodInfo method);
        IEnumerable<Statement> ConstructorStatements { get; }
        void Pregenerate(ClassEmitter @class, ProxyGenerationOptions options);
        void Postgenerate(ClassEmitter @class, ProxyGenerationOptions options);
    }
}
