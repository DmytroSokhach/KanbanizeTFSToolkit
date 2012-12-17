using System.Collections.Generic;
using System.Drawing;
using System.Text;
using CommandLine;
using System.Linq;
using System;
using CommandLine.Text;

namespace KanbanizeTool.Config
{
    public class Options
    {
        private string _operation;
        [Option("o", "operation", DefaultValue = "", HelpText = "add | del | delete | update | clear | update | list | details")]
        public string Operation
        {
            get { return _operation; }
            set { _operation = value.ToLowerInvariant(); }
        }

        [OptionList("t", "tfs-ids", Separator = ',', HelpText = "TFS work items id(s) separated by comma")]
        public IList<string> TfsIdStrings { get; set; }
        public int[] TfsIds
        {
            get { return TfsIdStrings.Select(n => Convert.ToInt32(n)).ToArray(); }
        }

        [OptionList("k", "kb-ids", Separator = ',', HelpText = "Kanbanize work items id(s) separated by comma")]
        public IList<string> KanbanizeIdStrings { get; set; }
        public int[] KanbanizeIds
        {
            get { return KanbanizeIdStrings.Select(n => Convert.ToInt32(n)).ToArray(); }
        }

        [Option(null, "select-type")]
        public string SelectType{get; set;}

        [Option(null, "select-notfsid")]
        public string SelectNoTfsId { get; set; }

        [Option(null, "update-type")]
        public string UpdateType { get; set; }

        private Color _updateColor;
        [Option(null, "update-color")]
        public string UpdateColor
        {
            get { return ColorTranslator.ToHtml(_updateColor).Replace("#", "").ToLower(); }
            set { _updateColor = ColorTranslator.FromHtml(value.Insert(0, "#")); }
        }


        [HelpOption(HelpText = "Dispaly this help screen.")]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            var briefUsage = @"KanbanizeTool.exe [[-o | --operation] [add] | del | delete | update | clear | update] -ids <coma-separated-tfs-id-list>";
            var examples = @"EXAMPLES:
Add tfs task: -o add -t 113872
Update Kanbanize task with id 176 to new color: -o update -k 176 --update-color=ffdf60 
Update Kanbanize task with id 176 to new color: -o update -t100000,100111,100222 --update-color=ffdf60 
or -o update --tfs-ids=100000,100111,100222 --update-color=ffdf60 
Update Kanbanize tasks with NO TFS id in their title or external link and update their color: -o update --select-notfsid --update-color=ffdf60 
Update Kanbanize tasks with type 'Task' to new color: -o update --select-type=Task --update-color=ffdf60 
Update Kanbanize tasks with type 'None' to task type 'Task': -o update --select-type=None --update-type=Task 
";
            usage.AppendLine(briefUsage);
            usage.AppendLine(examples);
            /*Example thoughts
tool.exe -o update <what> <how>
tool.exe -o update select-type="Task" upd-Color=00CC00
tool.exe -o update select-ids=1,2,3 upd-Color=00CC00
tool.exe -o update select-tfs-ids=100171,101194 upd-Color=00CC00
{KanbanizeTool.exe [-o add] -ids 101193,101194} equals {KanbanizeTool.exe -op add -ids 101193,101194} :add 101193 and 101194 TFS work items to Kanbanize
             */
            return usage.ToString();
        }

        public static Options GetOptions(string[] args)
        {
            var options = new Options();
            ICommandLineParser parser = new CommandLineParser();
            try
            {
                if (!parser.ParseArguments(args, options))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: was unable to parse some arguments");
                    Console.ResetColor();
                    Console.WriteLine(options.GetUsage());
                    Environment.Exit(-1);
                }
            }
            catch (ArgumentException ae)
            {
                if (ae.Message == "shortName")
                {
                    Console.Error.WriteLine("Short name attribute can't be NULL or Length > 1");
                }
            }

            if (string.IsNullOrWhiteSpace(options.Operation))
            {
                Console.WriteLine("Error parsing arguments: No <operation> option specified");
                Console.WriteLine(options.GetUsage());
                Environment.Exit(1);
            }
            return options;
        }
    }
}
