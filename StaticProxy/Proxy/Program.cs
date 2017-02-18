using PhillipScottGivens.SharedCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Proxy
{
    class Program
    {
        private const string AssemblyName = "assembly name";
        private const string NewAssemblyname = "new assembly name";
        private const string TargetAssembly = "target-assembly";
        private const string IgnoreAttributes = "ignore-attributes";
        private const string Template = "template";
        private const string Namespace = "namespace";

        static void Main(string[] args)
        {
            var arguments = new CommandLineArguments(Assembly.GetEntryAssembly(), args,
                new ExpectedArguments(
                    new ExpectedArgument(AssemblyName, "The name of the assembly with proxy generation logic."),
                    new ExpectedArgument(NewAssemblyname, "The name of the assembly to generate.")),
                new CommandLineFlag("ign-att", IgnoreAttributes, "Do not look for the AspectTempateAttribute."),
                new CommandLineFlag("targ", TargetAssembly, "target assembly", "Target a secondary assembly to run against templates.", true),
                new CommandLineFlag("temp", Template, "template", "Specify the fully qualified name of the template to use.", true),
                new CommandLineFlag("ns", Namespace, "namespace of proxies", "The namespace to apply to proxies generated.", false));

            string assemblyName = arguments[0];
            var templateAssembly = Assembly.Load(assemblyName);
            Type templateProxyGeneraotorType = FindTemplateProxyGeneratorType(templateAssembly);
            MethodInfo generateAssembly = templateProxyGeneraotorType.GetMethod("GenerateAssembly",
                BindingFlags.Public | BindingFlags.Static, null,
                new Type[] { typeof(Assembly), typeof(string), typeof(bool), typeof(List<string>), typeof(List<string>), typeof(string) },
                null);

            generateAssembly.Invoke(null, new object[]{
                templateAssembly, arguments[1],
                arguments.Found(IgnoreAttributes),
                arguments[TargetAssembly].Values, arguments[Template].Values,
                arguments[Namespace].Values.FirstOrDefault()});
        }

        private static Type FindTemplateProxyGeneratorType(Assembly assembly)
        {
            var scannedAssemblies = new List<AssemblyName>();

            var assembliesToScan = new Queue<Assembly>();
            assembliesToScan.Enqueue(assembly);
            while (assembliesToScan.Count > 0)
            {
                var current = assembliesToScan.Dequeue();
                foreach (var refAssembly in current.GetReferencedAssemblies())
                {
                    if (scannedAssemblies.Contains(refAssembly))
                        continue;

                    var scan = Assembly.Load(refAssembly);
                    Type generatorType = scan.GetType("PhillipScottGivens.StaticProxy.TemplateProxyGenerator");
                    if (generatorType != null)
                        return generatorType;
                }
            }
            return null;
        }
    }

}
