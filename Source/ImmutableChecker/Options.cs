using CommandLine;
using CommandLine.Text;

namespace ImmutableChecker
{
    public class Options
    {
        [Option('a', "Attribute", Required = true, HelpText = "不変性を表す属性型のFullNameを指定します")]
        public string ImmutableAttributeTypeFullName { get; set; }
        [Option('i', "input", Required = true, HelpText = "チェック対象のアセンブリのファイルパスを指定します")]
        public string Assembly { get; set; }


        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText { AddDashesToOption = true };

            help.AddPreOptionsLine("Usage: ImmutableChecker.exe <options> ");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("options:");
            help.AddOptions(this);

            return help;
        }
    }
}