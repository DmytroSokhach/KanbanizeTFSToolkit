using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TFSToolkit
{
    public class TfsRepository
    {
        private readonly TfsTeamProjectCollection _tfs;
        private readonly ITfsProjectProvider _projectProvider;

        public TfsRepository(TfsTeamProjectCollection tfs, ITfsProjectProvider projectProvider, string projectName)
        {
            _tfs = tfs;
            _tfs.EnsureAuthenticated();

            _projectProvider = projectProvider;
            _projectProvider.Initialize(_tfs, projectName);
        }

        public IEnumerable<WorkItem> GetWorkItems(IEnumerable<int> workItemIds)
        {
            return workItemIds.Select(x => _projectProvider.TfsProject.Store.GetWorkItem(x)).ToList();
        }
    }
}
