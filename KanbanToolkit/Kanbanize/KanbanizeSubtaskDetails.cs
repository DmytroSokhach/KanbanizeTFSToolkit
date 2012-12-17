using System;
using Newtonsoft.Json;

namespace Kanban.Kanbanize
{
    [JsonObject(MemberSerialization.OptIn)]
    public class KanbanizeSubtaskDetails
    {
        [JsonProperty(PropertyName = "taskid")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "assignee")]
        public string Assignee { get; set; }

        [JsonProperty(PropertyName = "completiondate")]
        public DateTime? CompletionDate { get; set; }

        [JsonProperty(PropertyName = "assignedtome")]
        public int? AssignedToMe { get; set; }
    }

    #region For reference
    /*
     * "subtaskdetails" : [{
				"taskid" : "177",
				"title" : "101136 - Arbor - Trader\/Bidder Market Activity tab: Permissioning changes",
				"assignee" : "Ievgenii A",
				"completiondate" : null,
				"assignedtome" : 2
			}*/
    #endregion
}
