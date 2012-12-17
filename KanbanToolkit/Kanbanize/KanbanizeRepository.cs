using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kanban.Kanbanize
{
    public class KanbanizeRepository
    {
        private readonly Uri _uri;
        private readonly string _apiKey;
        private readonly int _boardId;

        public KanbanizeRepository(Uri uri, string apiKey, int boardId)
        {
            _uri = uri;
            _apiKey = apiKey;
            _boardId = boardId;
        }

        public string CreateNewTask(KanbanizeTask newTask)
        {
            //newTask.Template = "Task";
            return QueryTask(newTask, KanbanizeCommand.create_new_task);
        }

        public IEnumerable<KanbanizeTask> GetAllTasks(bool getWithSubtasks = true)
        {
            var optionalParams = getWithSubtasks ? new[] { "subtasks", "yes" } : null;
            var responseJson = QueryTask(new KanbanizeTask(), KanbanizeCommand.get_all_tasks, optionalParams);
            File.WriteAllText(
                "all_tasks_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".js",
                responseJson);
            return JsonConvert.DeserializeObject<List<KanbanizeTask>>(responseJson);
        }

        public bool DeleteTask(int taskId)
        {
            var responseJson = QueryTask(new KanbanizeTask { Id = taskId }, KanbanizeCommand.delete_task);
            return responseJson == "1";
        }

        public bool UpdateTask(KanbanizeTask kanbanizeTask)
        {
            var responseJson = QueryTask(kanbanizeTask, KanbanizeCommand.edit_task);
            return responseJson == "1";
        }

        public KanbanizeTask GetTaskDetails(int taskId)
        {
            var responseJson = QueryTask(new KanbanizeTask { Id = taskId }, KanbanizeCommand.get_task_details);
            return JsonConvert.DeserializeObject<KanbanizeTask>(responseJson);
        }

        private string QueryTask(KanbanizeTask taskToQuery, KanbanizeCommand command, string[] optionalParams = null)
        {
            taskToQuery.BoardId = _boardId;
            var uriBuilder = new UriBuilder(_uri);
            uriBuilder.Path += string.Format(@"/{0}", command.ToString("G"));
            if (optionalParams != null)
            {
                uriBuilder.Path += "/" + string.Join("/", optionalParams);
            }
            uriBuilder.Path += "/format/json";
            var request = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
            request.Headers.Add("apikey", _apiKey);
            request.Method = "POST";
            request.ContentType = "application/json";

            UTF8Encoding encoding = new UTF8Encoding();
            var taskJson = JsonConvert.SerializeObject(taskToQuery,
                                                       new JsonSerializerSettings
                                                       {
                                                           Converters =
                                                               new[]
                                                                       {
                                                                           new IsoDateTimeConverter
                                                                               {DateTimeFormat = "yyyy-mm-dd"}
                                                                       },
                                                           NullValueHandling = NullValueHandling.Ignore
                                                       });
            var taskBytes = encoding.GetBytes(taskJson);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(taskBytes, 0, taskBytes.Length);
            }

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = GetResponseContent(encoding, response);
                return responseString;
            }
            catch (WebException wex)
            {
                var httpResponse = wex.Response as HttpWebResponse;
                if (httpResponse != null)
                {   
                    if (httpResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        throw new ApplicationException("API key is forbidden");
                    }

                    var responseString = GetResponseContent(encoding, httpResponse);
                    if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        throw  new ApplicationException(string.Format("Bad request returned error:\n{0}", responseString));
                    }

                    throw new ApplicationException(string.Format(
                        "Remote server call {0} {1} resulted in a http error {2} {3}.",
                        "POST",
                        uriBuilder.Uri,
                        httpResponse.StatusCode,
                        httpResponse.StatusDescription), wex);
                }
                else
                {
                    throw new ApplicationException(string.Format(
                        "Remote server call {0} {1} resulted in an error.",
                        "POST",
                        uriBuilder.Uri), wex);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static string GetResponseContent(UTF8Encoding encoding, HttpWebResponse httpResponse)
        {
            string responseString;
            using (var memoryStream = new MemoryStream())
            {
                using (var webResponseStream = httpResponse.GetResponseStream())
                {
                    webResponseStream.CopyTo(memoryStream);
                }
                responseString = encoding.GetString(memoryStream.ToArray());
            }
            return responseString;
        }
    }
}
