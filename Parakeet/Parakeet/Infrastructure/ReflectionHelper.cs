using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace Parakeet.Infrastructure
{
    public class ReflectionHelper
    {
        // TODO: Move to a shared-core
        public static PropertyInfo GetPropertyInfo<TValue, TItem>(
                TValue source,
                Expression<Func<TValue, TItem>> propertyLambda)
        {
            Type type = typeof(TValue);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }


        // TODO: Move to a shared-core
        public static MethodInfo GetMethodInfo<TValue>(
                Expression<Func<TValue, Action>> methodLambda)
        {
            Type type = typeof(TValue);

            MemberExpression member = methodLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    methodLambda.ToString()));

            MethodInfo methodInfo = member.Member as MethodInfo;
            if (methodInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    methodLambda.ToString()));


            if (type != methodInfo.ReflectedType &&
                !type.IsSubclassOf(methodInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    methodLambda.ToString(),
                    type));

            return methodInfo;
        }

    }
}
