using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy.Contributors;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy.Generators.Emitters;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace PhillipScottGivens.StaticProxy.Infrastructure
{
    public class PartCompositeTypeContributor : ITypeContributor
    {
        private readonly IEnumerable<Type> parts;
        private readonly Type genericInterface;
        private static readonly MethodAttributes propertyAttributes
            = MethodAttributes.Public | MethodAttributes.SpecialName |
            MethodAttributes.HideBySig | MethodAttributes.ReuseSlot |
            MethodAttributes.Virtual | MethodAttributes.Final;

        private readonly ConstructorContributor constructorContributor;

        public PartCompositeTypeContributor(ConstructorContributor constructorContributor, IEnumerable<Type> parts)
        {
            this.parts = parts;
            genericInterface = typeof(IHasPart<>);
            this.constructorContributor = constructorContributor;
        }

        public void CollectElementsToProxy(IProxyGenerationHook hook, MetaType model)
        {
        }

        public void Generate(ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope)
        {
            var baseClass = @class.BaseType;

            foreach (var partType in parts)
            {
                var specificInterface = genericInterface.MakeGenericType(partType);
                var typeBuilder = @class.TypeBuilder;
                typeBuilder.AddInterfaceImplementation(specificInterface);
                var field = @class.CreateField(string.Format("_part<{0}>", partType.Name), partType); ;
                var partProperty = specificInterface.GetMethod("get_Part", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                constructorContributor.AppendConstructorStatement(
                    new AssignStatement(field,
                    new NewInstanceExpression(partType, new Type[0])));
                
                var getMethodBuilder = typeBuilder.DefineMethod(
                    string.Format("get_Part<{0}>", partType.Name),
                    propertyAttributes, partType, null);

                var getILGenerator = getMethodBuilder.GetILGenerator();

                getILGenerator.Emit(OpCodes.Ldarg_0);
                getILGenerator.Emit(OpCodes.Ldfld, field.Reference);

                getILGenerator.Emit(OpCodes.Ret);

                typeBuilder.DefineMethodOverride(getMethodBuilder, partProperty);
            }

        }
    }
}
