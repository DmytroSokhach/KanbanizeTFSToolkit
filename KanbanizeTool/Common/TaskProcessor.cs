using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Kanban.Kanbanize;
using KanbanizeTool.Config;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Newtonsoft.Json;
using TFSToolkit;

namespace KanbanizeTool.Common
{
    public class TaskProcessor
	{
		private readonly KanbanizeRepository _kanbanizeRepository;
		private readonly Lazy<TfsRepository> _tfsRepositoryLazy;
        private TfsRepository tfsRepository { get { return _tfsRepositoryLazy.Value; } }
		private readonly SynchronizationModuleConfiguration _configuration;

        public TaskProcessor(SynchronizationModuleConfiguration config)
		{
			_configuration = config;

			_kanbanizeRepository = new KanbanizeRepository(_configuration.KanbanizeUri, _configuration.ApiKey, _configuration.BoardId);
            var tfs = new TfsTeamProjectCollection(_configuration.TfsUri, new NetworkCredential(
                Properties.Settings.Default.TfsUserName,
                Properties.Settings.Default.TfsPassword,
                Properties.Settings.Default.TfsDomainName));
			_tfsRepositoryLazy = new Lazy<TfsRepository>(() => new TfsRepository(tfs, new ArborTfsProjectProvider(), _configuration.ProjectName));
		}

        public void Add(Options options)
        {
            int[] taskIds = options.TfsIds;
            var workItems = tfsRepository.GetWorkItems(taskIds);
			var kanbanizeTasks = _kanbanizeRepository.GetAllTasks();

			foreach (var workItem in workItems.Where(x => kanbanizeTasks.All(y => y.TfsId != x.Id)))
			{
                string operationReslt = _kanbanizeRepository.CreateNewTask(workItem.ToKanbanizeTask(_configuration.TfsUri.ToString()));
                //string operationReslt = _kanbanizeRepository.CreateNewTask(workItem.ToKanbanizeTask(_configuration.TfsUri.ToString(), GetRelatedTaskTitles(workItem)));
			}
		}

        public void Update(Options options)
        {
			var kanbanizeTasks = _kanbanizeRepository.GetAllTasks();
            IEnumerable<KanbanizeTask> qualifiedTasks = kanbanizeTasks;
            if (options.KanbanizeIdStrings != null && options.KanbanizeIdStrings.Count > 0)
            {
                qualifiedTasks = qualifiedTasks.Where(x => options.KanbanizeIds.Contains(x.Id));
            }
            if (!String.IsNullOrWhiteSpace(options.SelectType))
            {
                qualifiedTasks = qualifiedTasks.Where(x => x.Type == options.SelectType);
            }
            if (!String.IsNullOrWhiteSpace(options.SelectNoTfsId))
            {
                qualifiedTasks = qualifiedTasks.Where(x => x.TfsId == null);
            }

            foreach (var kanbanizeTask in qualifiedTasks)
            {
                bool needUpdate = false;
                var updateTask = new KanbanizeTask {Id = kanbanizeTask.Id};
                if (!needUpdate && !String.IsNullOrWhiteSpace(options.UpdateColor) && kanbanizeTask.ColorString != ( "#" + options.UpdateColor))
                {
                    needUpdate = true;
                    updateTask.ColorString = options.UpdateColor;
                }

                if (!needUpdate && !String.IsNullOrWhiteSpace(options.UpdateType) && kanbanizeTask.Type != options.UpdateType)
                {
                    needUpdate = true;
                    updateTask.Type = options.UpdateType;
                }

                if (needUpdate)
                {
                    _kanbanizeRepository.UpdateTask(updateTask);
                }
            }

            /*
            int[] taskIds = options.TfsIds;
            var workItems = tfsRepository.GetWorkItems(taskIds);
			foreach (var workItem in workItems)
			{
				var kanbanizeTask = enumerable.FirstOrDefault(x => x.TfsId == workItem.Id);
				if (kanbanizeTask != null)
				{
					//kanbanizeTask.UpdateByWorkItem(workItem);
					_kanbanizeRepository.UpdateTask(kanbanizeTask);
				}
			}
             * */
		}

        public void Delete(Options options)
        {
            int[] taskIds = options.TfsIds;

			var kanbanizeTasks = _kanbanizeRepository.GetAllTasks();

            foreach (var task in kanbanizeTasks.Where(x => taskIds.Contains(x.TfsId ?? 0)))
			{
				_kanbanizeRepository.DeleteTask(task.Id);
			}
		}

		public void Clear()
		{
			var kanbanizeTasks = _kanbanizeRepository.GetAllTasks();
			foreach (var task in kanbanizeTasks)
			{
				_kanbanizeRepository.DeleteTask(task.Id);
			}
		}

		public void ListAll()
		{
			var kanbanizeTasks = _kanbanizeRepository.GetAllTasks();
            StringBuilder sb = new StringBuilder();		    
			foreach (var task in kanbanizeTasks)
			{
                sb.AppendLine(JsonConvert.SerializeObject(task));
			}
            Console.WriteLine(sb.ToString());
		}

        public void GetDetailsbyKanbanizeId(int kbId)
        {
            var kanbanizeTask = _kanbanizeRepository.GetTaskDetails(kbId);
            Console.WriteLine(JsonConvert.SerializeObject(kanbanizeTask));
            Console.Write(kanbanizeTask.Color);
        }

        public IEnumerable<WorkItem> GetRelatedTaskTitles(WorkItem wi)
        {
            IEnumerable<WorkItem> relatedWorkItems = null;
            if (wi.RelatedLinkCount > 0)
            {
                var relatedLinkIds = wi.Links.OfType<RelatedLink>().Select(x => x.RelatedWorkItemId).ToList();
                relatedWorkItems = tfsRepository.GetWorkItems(relatedLinkIds);
            }
            return relatedWorkItems;
        }
	}
}
