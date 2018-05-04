using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.N.Core.Microservices;
using MS.N.Log.Models;

namespace MS.N.Log.Controllers
{
    public class AuditController : MicroserviceController
    {
        public AuditController(ILogger<MicroserviceController> logger, IConfiguration configuration) : base(logger, configuration)
        {
        }

        [HttpPost("audit")]
        public IActionResult Audit(Audit data)
        {


            return null;

        }

        [HttpGet("audit")]
        public IActionResult GetAudit(Guid id)
        {
            return null;

        }
    }
}