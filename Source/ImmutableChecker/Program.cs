using System;
using System.ComponentModel;
using System.Reflection;
using CommandLine;

namespace ImmutableChecker
{
    class Program
    {
        static int Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options) == false)
            {
                return 1;
            }

            //var immutableAttributeTypeFullName = "TargetAssembly.ImmutableAttribute";
            var immutableAttributeTypeFullName = options.ImmutableAttributeTypeFullName;
            // var assembly = Assembly.LoadFrom("TargetAssembly.dll");
            var assembly = Assembly.LoadFrom(options.Assembly);
            var allTypes = assembly.GetTypes();

            var engine = new ImmutableCheckEngine(immutableAttributeTypeFullName);

            var result = CheckResult.AllOK;
            foreach (var type in allTypes)
            {
                if (engine.IsCheckTargetType(type))
                {
                    result += engine.CheckImmutable(type);
                }
            }

            foreach (var error in result.ErrorLog)
            {
                Console.Error.WriteLine(error);
            }
            
            return result.Result ? 0 : 1;
        }
    }
}
