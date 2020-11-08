using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using jsonbase.Hubs;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace jsonbase.Controllers
{
    [ApiController]
    [Route("{**slug}")]
    public class JsonController : ControllerBase
    {
        private readonly ILogger<JsonController> _logger;
        private readonly IHubContext<ApiHub, IApiHub> _hub;

        public JsonController(ILogger<JsonController> logger, IHubContext<ApiHub, IApiHub> hub)
        {
            _logger = logger;
            _hub = hub;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var path = RequestPathToFilePath(Request.Path);
                if (!System.IO.File.Exists(path))
                {
                    return NotFound();
                }

                var data = Load(path);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] dynamic data)
        {
            try
            {
                var path = RequestPathToFilePath(Request.Path);
                Save(path, data);
                await _hub.Clients.Groups(Request.Path).SendUpdated(Request.Path);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] dynamic data)
        {
            try
            {
                var path = RequestPathToFilePath(Request.Path);
                Update(path, data);
                await _hub.Clients.Groups(Request.Path).SendUpdated(Request.Path);
                data = Load(path);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private dynamic Load(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var json = System.IO.File.ReadAllText(path);
                var data = System.Text.Json.JsonSerializer.Deserialize<dynamic>(json);
                return data;
            }
            else
            {
                return null;
            }
        }

        private void Save(string path, dynamic data)
        {
            System.IO.File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(data));
        }

        private void Update(string path, dynamic data)
        {
            if (System.IO.File.Exists(path))
            {
                var jsonExisting = System.IO.File.ReadAllText(path);
                var jsonNew = System.Text.Json.JsonSerializer.Serialize(data);

                try
                {
                    var o1 = JObject.Parse(jsonExisting);
                    var o2 = JObject.Parse(jsonNew);
                    o1.Merge(o2, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                
                    data = System.Text.Json.JsonSerializer.Deserialize<dynamic>(o1.ToString());                
                }
                catch
                {
                    var a1 = JArray.Parse(jsonExisting);
                    var a2 = JArray.Parse(jsonNew);
                    a1.Merge(a2, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });

                    data = System.Text.Json.JsonSerializer.Deserialize<dynamic>(a1.ToString());                
                }

                //                System.IO.File.WriteAllText(path, o1.ToString());
                //data = System.Text.Json.JsonSerializer.Deserialize<dynamic>(o1.ToString());
                Save(path, data);
            }
            else
            {
                Save(path, data);
            }
        }

        private string RequestPathToFilePath(string requestPath)
        {
            var rootPath = Environment.GetEnvironmentVariable("ROOT_PATH");
            if (rootPath == null)
            {
                var msg = "Environment variable ROOT_PATH is not set. Set it to the folder where you want all data to be stored.";
                _logger.LogError(msg);
                throw new Exception(msg);
            }

            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            var items = requestPath.Trim().Split('/');
            var dirs = new List<string>();
            dirs.Add(rootPath);
            dirs.AddRange(items);
            var path = System.IO.Path.Combine(dirs.ToArray());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var file = Path.Combine(path, "data.json");
            return file;
        }
    }
}
