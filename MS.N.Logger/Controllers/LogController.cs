﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace MS.N.Logger.Controllers
{
    [Route("api/log")]
    public class LogController : Controller
    {
        private ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("info")]
        public IActionResult Info([FromBody]string descripcion)
        {
            if (descripcion == null)
                return BadRequest();

            _logger.LogInformation(descripcion);
            return NoContent();
        }

        [HttpPost]
        [Route("warning")]
        public IActionResult Warning([FromBody]string descripcion)
        {
            if (descripcion == null)
                return BadRequest();

            _logger.LogWarning(descripcion);
            return NoContent();
        }

        [HttpPost]
        [Route("critical")]
        public IActionResult Critical([FromBody]string descripcion)
        {
            if (descripcion == null)
                return BadRequest();

            _logger.LogCritical(descripcion);
            return NoContent();
        }

        [HttpPost]
        [Route("debug")]
        public IActionResult Debug([FromBody]string descripcion)
        {
            if (descripcion == null)
                return BadRequest();

            _logger.LogDebug(descripcion);
            return NoContent();
        }

        [HttpPost]
        [Route("error")]
        public IActionResult Error([FromBody]string descripcion)
        {
            if (descripcion == null)
                return BadRequest();

            _logger.LogError(descripcion);
            return NoContent();
        }

        [HttpPost]
        [Route("trace")]
        public IActionResult Trace([FromBody]string descripcion)
        {
            if (descripcion == null)
                return BadRequest();

            _logger.LogTrace(descripcion);
            return NoContent();
        }

        //[HttpPost("audit")]
        //public IActionResult Audit(string id, dynamic data)
        //{




        //}
    }
}