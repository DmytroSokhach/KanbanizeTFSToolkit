using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kanban.Kanbanize;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace KanbanizeTool.Common
{
    public static class TfsToKanbanizeAdapter
    {
        public static KanbanizeTask ToKanbanizeTask(this WorkItem wi, string tfsUri)
        {
            return ToKanbanizeTask(wi, tfsUri, null);
        }

        public static KanbanizeTask ToKanbanizeTask(this WorkItem wi, string tfsUri,  IEnumerable<WorkItem> relatedWorkItems)
        {
            var assignee = ResolveAssignedTo((string) wi["Assigned To"]);
            var resultTask = new KanbanizeTask
            {
                Title = CompileTitleFromWorkItem(wi),
                Description = wi.Description,
                ExternalLink = String.Format(Properties.Settings.Default.TfsTaskLinkTemplate, tfsUri, wi.Id),
                Size = (double)wi["Est Man Days"],
                Color = Properties.Settings.Default.ColorTask,
                Assignee = assignee,
                Priority = SetPriorityByTfsSeverity(wi),
                Type = "Task"
            };

            #region Add subtasks
            // ATTENTION: for now Kanbanize API doesn't allow include subtasks when creating task
            /*resultTask.SubtaskDetails = relatedWorkItems
                 .Select(x =>
                     new KanbanizeSubtaskDetails()
                     {
                         Title = CompileTitleFromWorkItem(x),
                         Assignee = assignee
                     }
             ).ToArray();//*/
            #endregion

            return resultTask;
        }

        private static string CompileTitleFromWorkItem(WorkItem wi)
        {
            return string.Join(Kanban.Properties.Settings.Default.KanbanizeTitleIdSeparator, new[] { wi.Id.ToString(), wi.Title });
        }

        public static void UpdateByWorkItem(this KanbanizeTask kanbanizeTask, WorkItem wi)
        {
            if (kanbanizeTask.TfsId != wi.Id)
                throw new InvalidOperationException("This operation cannot be performed cause ids not match.");

            kanbanizeTask.Title = string.Join(Kanban.Properties.Settings.Default.KanbanizeTitleIdSeparator, new[] { wi.Id.ToString(), wi.Title });
            kanbanizeTask.Description = wi.Description;
            kanbanizeTask.Size = (double)wi["Est Man Days"];
            kanbanizeTask.ColorString = "00FF00";
            kanbanizeTask.Assignee = ResolveAssignedTo((string)wi["Assigned To"]);
        }

        private static string ResolveAssignedTo(string tfsAccount)
        {
            int index = Properties.Settings.Default.TfsToKanbanizeAssigneeMapping.IndexOf(tfsAccount) + 1;
            string kbAccount;
            try
            {
                kbAccount = Properties.Settings.Default.TfsToKanbanizeAssigneeMapping[index] ?? "";
            }
            finally
            {
                
            }
            return kbAccount;
        }

        static KanbanizePriority? SetPriorityByTfsSeverity(WorkItem wi)
        {
            switch ((string)(wi["Ipreo Severity"]))
            {
                case "1-Critical":
                case "2-High":
                    return KanbanizePriority.High;
                case "3-Medium":
                    return KanbanizePriority.Average;
                case "4-Low":
                    return KanbanizePriority.Low;
                default:
                    return null;
            }
        }
    }
}
