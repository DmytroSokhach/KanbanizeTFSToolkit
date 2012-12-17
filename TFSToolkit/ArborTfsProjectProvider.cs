using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TFSToolkit
{
    public class ArborTfsProjectProvider : ITfsProjectProvider
    {
        private bool _isInitialized;
        private WorkItemStore _workItemStore;
        private Project _project;

        public Project TfsProject
        {
            get
            {
                if (_isInitialized == false)
                    throw new InvalidOperationException("ArborTfsProjectProvider should be initialized");
                return _project;
            }
        }

        public void Initialize(TfsTeamProjectCollection tfs, string projectName)
        {
            _isInitialized = true;
            _workItemStore = tfs.GetService<WorkItemStore>();
            _project = _workItemStore.Projects
                    .Cast<Project>()
                    .SingleOrDefault(p => p.Name.Equals(projectName));
        }

        public Query RunQuery(TfsQueryConfiguration configuration)
        {
            var myQueries = _project.QueryHierarchy.SingleOrDefault(x => x.Name.Equals(configuration.FolderName));

            var myArborQuery = ((QueryFolder)myQueries).SingleOrDefault(qi => qi.Name == configuration.QueryName);

            var queryText = _project.Store.GetQueryDefinition(myArborQuery.Id).QueryText;

            queryText = configuration.Replcements
                .Aggregate(queryText, (current, replacement) => current.Replace(replacement.Key, replacement.Value));

            var query = new Query(_workItemStore, queryText);

            return query;
        }




    }
}
