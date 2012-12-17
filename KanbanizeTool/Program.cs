using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using KanbanizeTool.Common;
using KanbanizeTool.Config;

namespace KanbanizeTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Utils.CheckSettings();

            try
            {
                Options options = Options.GetOptions(args);

                var taskProcessor = InitTaskProcessor();

                switch (options.Operation)
                {
                    case null:
                    case "add":
                    case "create":
                        taskProcessor.Add(options);
                        break;
                    case "update":
                    case "upd":
                        taskProcessor.Update(options);
                        break;
                    case "del":
                    case "delete":
                        taskProcessor.Delete(options);
                        break;
                    case "clear":
                        taskProcessor.Clear();
                        break;
                    case "list":
                        taskProcessor.ListAll();
                        break;
                    case "details":
                        taskProcessor.GetDetailsbyKanbanizeId(77); // some test value
                        break;
                }//*/

            }
            catch (Exception e)
            {
                WriteErrorToConsole(e.Message);
            }
        }

        private static TaskProcessor InitTaskProcessor()
        {
            Uri tfsServerUri = null;
            Uri kanbanizeUri = null;
            try
            {
                tfsServerUri = new Uri(Properties.Settings.Default.TfsServerUri);
                kanbanizeUri = new Uri(Properties.Settings.Default.KanbanizeUri);
            }
            catch (UriFormatException)
            {

                throw new ApplicationException("TfsServerUri or KanbanizeUri have wrong URI format." +
                                    "(Should be like (quotes for clarity): 'http://<some address>' )" +
                                               string.Format("\nPlease edit (or delete) 'user.config' somewhere at: \n{0} \nwith folder prefix like: {1}",
                                               Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + 
                                               Path.DirectorySeparatorChar +
                                               System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                                               AppDomain.CurrentDomain.FriendlyName
                                               ));
            }
            TaskProcessor taskProcessor = new TaskProcessor(new SynchronizationModuleConfiguration()
                                                                {
                                                                    ApiKey = Properties.Settings.Default.KanbanizeApiKey,
                                                                    BoardId = int.Parse(Properties.Settings.Default.KanbanizeBoardId),
                                                                    KanbanizeUri = kanbanizeUri,
                                                                    ProjectName = Properties.Settings.Default.TfsProjectName,
                                                                    TfsUri = tfsServerUri
                                                                });
            return taskProcessor;
        }

        private static void WriteErrorToConsole(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ReadKey();
            Console.ResetColor();
        }
    }
}
