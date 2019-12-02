using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using Newtonsoft.Json;

namespace WebApplicationVideoStore.Controllers
{
    /*
    [Route("api/[controller]")]
    [ApiController]
    public class HealthcheckController : ControllerBase
    {
    }
    */
    [Produces("application/json")]
    [Route("api/Healthcheck")]
    public class HealthcheckController : Controller
    {
        [HttpGet(Name = "GetAppStatusCode")]
        public IActionResult Get()
        {
            return Ok();
        }
        [HttpGet("{key}", Name = "GetSpecificAppStatusCode")]
        public IActionResult Get(string key)
        {
            dynamic result = new ExpandoObject();
            switch (key.ToLower())
            {
                case "env":
                    result.Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown";
                    break;
                default:
                    result.Add(new KeyValuePair<string, object>(key, "undefined"));
                    break;
            }
            return Ok(JsonConvert.SerializeObject(result));
        }
    }
}