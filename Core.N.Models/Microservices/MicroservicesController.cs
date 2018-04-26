using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.N.Core.Trace;
using Newtonsoft.Json;

namespace Models.N.Core.Microservices
{
    public class MicroservicesController : Controller
    {
        private readonly ILogger _logger;

        public MicroservicesController(ILogger logger)
        {
            _logger = logger;
        }

        protected void Communicator_TraceHandler(object sender, TraceEventArgs ea, string description)
        {
            var serviceTrace = JsonConvert.SerializeObject(ea);
            _logger.LogError($"{description}|{DateTime.Now.ToString("yyyyMMdd hh:mm:ss")}|{serviceTrace}");
        }
    }
}
