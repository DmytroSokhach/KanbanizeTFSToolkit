using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanbanizeTool.Config
{
    public class SynchronizationModuleConfiguration
    {
        public int BoardId { get; set; }
        public Uri TfsUri { get; set; }
        public Uri KanbanizeUri { get; set; }
        public string ApiKey { get; set; }
        public string ProjectName { get; set; }
    }
}
