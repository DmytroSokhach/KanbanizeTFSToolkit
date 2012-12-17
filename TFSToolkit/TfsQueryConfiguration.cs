using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TFSToolkit
{
    public class TfsQueryConfiguration
    {
        public string ProjectName { get; set; }
        public string QueryName { get; set; }
        public string FolderName { get; set; }

        private Dictionary<string, string> replacements;
        public Dictionary<string, string> Replcements
        {
            get
            {
                if (replacements == null)
                    replacements = new Dictionary<string, string>();
                return replacements;
            }
        }
    }
}
