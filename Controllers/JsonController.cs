using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace jsonbase.Controllers
{
    [ApiController]
    [Route("{**slug}")]
    public class JsonController : ControllerBase
    {
        private readonly ILogger<JsonController> _logger;

        public JsonController(ILogger<JsonController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var path = RequestPathToFilePath(Request.Path);
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            var data = Load(path);
            return Ok(data);
        }

        [HttpPut]
        public IActionResult Put([FromBody] dynamic data)
        {
            var path = RequestPathToFilePath(Request.Path);
            Save(path, data);
            return Ok(data);
            // TODO: return Ok(new
            // {
            //     Version = 1,
            //     Data = data,
            //     Path = Request.Path.ToString()
            // });
        }

        private dynamic Load(string path)
        {
            var json = System.IO.File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<dynamic>(json);
            return data;
        }

        private void Save(string path, dynamic data)
        {
            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(data));
        }

        private string RequestPathToFilePath(string requestPath)
        {
            var rootPath = Environment.GetEnvironmentVariable("ROOT_PATH");
            if (rootPath == null)
            {
                var msg = "Environment variable ROOT_PATH is not set. Set it to the folder where you want all data to be store.";
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
