using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BGBA.Models.N.Core.Trace;
using Newtonsoft.Json;

namespace BGBA.Models.N.Core.Microservices
{
    public class MicroserviceController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public MicroserviceController(ILogger<MicroserviceController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected void Communicator_TraceHandler(object sender, TraceEventArgs ea)
        {
            var serviceTrace = JsonConvert.SerializeObject(ea);
            _logger.LogInformation($"{DateTime.Now.ToString("yyyyMMdd hh:mm:ss")}|{serviceTrace}");
        }

        [HttpGet("config")]
        public IActionResult Config()
        {
            try
            {
                return new ObjectResult(_configuration.AsEnumerable());
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
