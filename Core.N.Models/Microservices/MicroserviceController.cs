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

        public MicroserviceController(ILogger<MicroserviceController> logger)
        {
            _logger = logger;
        }
        
        protected void Communicator_TraceHandler(object sender, TraceEventArgs ea)
        {
            this.Communicator_TraceHandler(sender, ea, "");
        }

        protected void Communicator_TraceHandler(object sender, TraceEventArgs ea, string sessionId)
        {
            var serviceTrace = JsonConvert.SerializeObject(ea);
            _logger.LogInformation($"{DateTime.Now.ToString("yyyyMMdd hh:mm:ss")}|{serviceTrace}|sessionId:{sessionId}");
        }
    }
}
