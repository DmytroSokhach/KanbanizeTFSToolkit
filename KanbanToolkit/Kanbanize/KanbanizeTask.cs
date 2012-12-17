using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Drawing;

namespace Kanban.Kanbanize
{
    [JsonObject(MemberSerialization.OptIn)]
    public class KanbanizeTask
    {
        [JsonProperty(PropertyName = "taskid")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "position")]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "assignee")]
        public string Assignee { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "subtasks")]
        public int? Subtasks { get; set; }

        [JsonProperty(PropertyName = "subtaskscomplete")]
        public int? SubtasksComplete { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string ColorString { get; set; }

        public Color Color
        {
            get { return ColorTranslator.FromHtml("#" + ColorString); }
            set { ColorString = ColorTranslator.ToHtml(value).Replace("#", "").ToLower(); }
        }

        [JsonProperty(PropertyName = "priority")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KanbanizePriority? Priority { get; set; }

        [JsonProperty(PropertyName = "size")]
        public double? Size { get; set; }

        [JsonProperty(PropertyName = "deadline")]
        public DateTime? Deadline { get; set; }

        [JsonProperty(PropertyName = "extlink")]
        public string ExternalLink { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public string Tags { get; set; }

        [JsonProperty(PropertyName = "blocked")]
        public string BlockedString { get; set; }

        public bool IsBlocked
        {
            get { return BlockedString == "1"; }
            set { BlockedString = value ? "1" : "0"; }
        }

        [JsonProperty(PropertyName = "blockedreason")]
        public string BlockedReason { get; set; }

        [JsonProperty(PropertyName = "subtaskdetails")]
        public KanbanizeSubtaskDetails[] SubtaskDetails { get; set; }

        /*
		"columnid" : "progress_2",
		"laneid" : "4",
		"leadtime" : 4,
		"subtaskdetails" : [],
		"columnname" : "Analyze",
		"lanename" : "Regular Task",
		"columnpath" : "Analyze"
         * */
        
        // TODO: dsokhach: no such property for task - just a parameter to query?
        [JsonProperty(PropertyName = "boardid")]
        public int BoardId { get; set; }

        [JsonProperty(PropertyName = "template")]
        public string Template { get; set; }

        public int? TfsId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Title) ||
                    !Title.Contains(Properties.Settings.Default.KanbanizeTitleIdSeparator))
                    return null;
                else
                {
                    var idString = Title
                        .Split(new [] { Properties.Settings.Default.KanbanizeTitleIdSeparator }, StringSplitOptions.RemoveEmptyEntries)
                        .First();

                    /*if (string.IsNullOrWhiteSpace(idString))
                        idString = ExternalLink.Substring(ExternalLink.LastIndexOf("id=", StringComparison.OrdinalIgnoreCase) + 3);*/

                    return Int32.Parse(idString);
                }
            }
        }
    }

    public enum KanbanizePriority
    {
        Low,
        Average,
        High
    }
}
