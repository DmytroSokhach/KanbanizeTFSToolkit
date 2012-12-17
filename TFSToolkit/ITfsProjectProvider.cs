using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TFSToolkit
{
    public interface ITfsProjectProvider
    {
        void Initialize(TfsTeamProjectCollection tfs, string projectName);
        Query RunQuery(TfsQueryConfiguration condig);
        Project TfsProject { get; }
    }
}
