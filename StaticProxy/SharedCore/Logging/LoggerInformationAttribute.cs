using System;
using System.Reflection;

namespace PhillipScottGivens.SharedCore.Logging
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LoggerInformationAttribute : Attribute
    {
        #region Properties
        public Type LoggerFactoryType { get; private set; }
        public Type CategoryType { get; private set; }
        public Type PriorityType { get; private set; }
        #endregion

        #region Initialize and Teardown
        public LoggerInformationAttribute(Type loggerFactoryType)
        {
            Type[] loggerInfo = loggerFactoryType.IsGenericType
                ? loggerFactoryType.GetGenericArguments()
                : loggerFactoryType.BaseType.GetGenericArguments();

            Type categoryType = loggerInfo[0];
            Type priorityType = loggerInfo[1];

            if (!categoryType.IsEnum)
                throw new ArgumentException("Type must be an enum", "catagoryType");
            if (!priorityType.IsEnum)
                throw new ArgumentException("Type must be an enum", "priorityType");

            CategoryType = categoryType;
            PriorityType = priorityType;
            LoggerFactoryType = loggerFactoryType;
        }
        #endregion

        #region Descovery
        public static LoggerInformationAttribute FindAttribute(Assembly assembly)
        {
            var attributes = assembly.GetCustomAttributes(typeof(LoggerInformationAttribute), true);
            if (attributes.Length == 0)
                return null;

            return (LoggerInformationAttribute)attributes[0];
        }
        #endregion
    }
}
