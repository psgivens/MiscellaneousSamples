using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy.Contributors;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Internal;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;


namespace PhillipScottGivens.StaticProxy.Infrastructure
{
    public class ConstructorContributor : ITypeContributor
    {
        private readonly List<Statement> appendedStatements
            = new List<Statement>();

        public void AppendConstructorStatement(Statement statement)
        {
            appendedStatements.Add(statement);
        }

        protected void GenerateConstructor(ClassEmitter emitter, ConstructorInfo baseConstructor,
                                           params FieldReference[] fields)
        {
            ArgumentReference[] args;
            ParameterInfo[] baseConstructorParams = null;

            if (baseConstructor != null)
            {
                baseConstructorParams = baseConstructor.GetParameters();
            }

            if (baseConstructorParams != null && baseConstructorParams.Length != 0)
            {
                args = new ArgumentReference[fields.Length + baseConstructorParams.Length];

                var offset = fields.Length;
                for (var i = offset; i < offset + baseConstructorParams.Length; i++)
                {
                    var paramInfo = baseConstructorParams[i - offset];
                    args[i] = new ArgumentReference(paramInfo.ParameterType);
                }
            }
            else
            {
                args = new ArgumentReference[fields.Length];
            }

            for (var i = 0; i < fields.Length; i++)
            {
                args[i] = new ArgumentReference(fields[i].Reference.FieldType);
            }

            var constructor = emitter.CreateConstructor(args);
            if (baseConstructorParams != null && baseConstructorParams.Length != 0)
            {
                var last = baseConstructorParams.Last();
                if (last.ParameterType.IsArray && last.IsDefined(typeof(ParamArrayAttribute), true))
                {
                    var parameter = constructor.ConstructorBuilder.DefineParameter(args.Length, ParameterAttributes.None, last.Name);
                    var builder = AttributeUtil.CreateBuilder<ParamArrayAttribute>();
                    parameter.SetCustomAttribute(builder);
                }
            }

            for (var i = 0; i < fields.Length; i++)
            {
                constructor.CodeBuilder.AddStatement(new AssignStatement(fields[i], args[i].ToExpression()));
            }

            foreach (var statement in appendedStatements)
                constructor.CodeBuilder.AddStatement(statement);
            
            // Invoke base constructor

            if (baseConstructor != null)
            {
                Debug.Assert(baseConstructorParams != null);

                var slice = new ArgumentReference[baseConstructorParams.Length];
                Array.Copy(args, fields.Length, slice, 0, baseConstructorParams.Length);

                constructor.CodeBuilder.InvokeBaseConstructor(baseConstructor, slice);
            }
            else
            {
                constructor.CodeBuilder.InvokeBaseConstructor();
            }

            constructor.CodeBuilder.AddStatement(new ReturnStatement());
        }


        protected void GenerateConstructors(ClassEmitter emitter, Type baseType, params FieldReference[] fields)
        {
            var constructors =
                baseType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var constructor in constructors)
            {
                if (!IsConstructorVisible(constructor))
                {
                    continue;
                }

                GenerateConstructor(emitter, constructor, fields);
            }
        }


        private bool IsConstructorVisible(ConstructorInfo constructor)
        {
            return constructor.IsPublic
                   || constructor.IsFamily
                   || constructor.IsFamilyOrAssembly
#if !Silverlight
 || (constructor.IsAssembly && InternalsUtil.IsInternalToDynamicProxy(constructor.DeclaringType.Assembly));
#else
            ;
#endif
        }

        public void CollectElementsToProxy(Castle.DynamicProxy.IProxyGenerationHook hook, Castle.DynamicProxy.Generators.MetaType model)
        {
        }

        public void Generate(ClassEmitter @class, 
            ProxyGenerationOptions options,
            INamingScope namingScope)
        {
            GenerateConstructors(@class, @class.BaseType);
        }
    }
}
