using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy.Contributors;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using System.Reflection;
using System.Reflection.Emit;
using ClrTest.Reflection;
using PhillipScottGivens.StaticProxy.Templates;

namespace PhillipScottGivens.StaticProxy.Infrastructure
{
    public class TemplateMethodGenerator : MethodGenerator
    {
        #region Fields
        private readonly MethodInfo templateMethod;
        #endregion

        #region Initialize and Teardown
        public TemplateMethodGenerator(MetaMethod method, OverrideMethodDelegate overrideMethod)
            : this(method, overrideMethod, GetTemplateMethod(method.Method.ReflectedType))
        {
        }

        private static MethodInfo GetTemplateMethod(Type type)
        {
            return (from method 
                        in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    where method.IsDefined(typeof(AspectAttribute), true)
                    select method).First();
        }

        public TemplateMethodGenerator(MetaMethod method, OverrideMethodDelegate overrideMethod, Action actionAsTemplate)
            : this(method, overrideMethod, actionAsTemplate.Method)
        {
        }
        public TemplateMethodGenerator(MetaMethod method, OverrideMethodDelegate overrideMethod, MethodInfo templateMethod)
            : base(method, overrideMethod)
        {
            this.templateMethod = templateMethod;
        }
        #endregion

        protected override MethodEmitter BuildProxiedMethodBody(MethodEmitter emitter, ClassEmitter @class,
                                                                ProxyGenerationOptions options, INamingScope namingScope)
        {
            if (emitter.ReturnType == typeof(int))
            {
                MethodBuilder methodBuilder = emitter.MethodBuilder;
                ILGenerator ilGenerator = methodBuilder.GetILGenerator();
                ParameterInfo[] parameterInfos = MethodToOverride.GetParameters();

                var v = new ILTemplateEmitter(ilGenerator, templateMethod, MethodToOverride, methodBuilder);
                var reader = new ILReader(templateMethod);
                reader.Accept(v);

                return emitter;
            }

            InitOutParameters(emitter, MethodToOverride.GetParameters());

            if (emitter.ReturnType == typeof(void))
            {
                emitter.CodeBuilder.AddStatement(new ReturnStatement());
            }
            else
            {
                emitter.CodeBuilder.AddStatement(new ReturnStatement(new DefaultValueExpression(emitter.ReturnType)));
            }

            return emitter;
        }

        private void InitOutParameters(MethodEmitter emitter, ParameterInfo[] parameters)
        {
            for (var index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                if (parameter.IsOut)
                {
                    emitter.CodeBuilder.AddStatement(
                        new AssignArgumentStatement(new ArgumentReference(parameter.ParameterType, index + 1),
                                                    new DefaultValueExpression(parameter.ParameterType)));
                }
            }
        }

        
    }
}
