using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.N.Models;
using Core.N.Utils.ObjectFactory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MS.N.Autenticacion.Controllers
{
    [Route("api/autenticacion")]
    public class AutenticacionController : Controller
    {

        protected const string ErrorPrefix = "MS_Autenticacion";

        private IConfiguration _configuration;
        private IHostingEnvironment _enviroment;
        private IObjectFactory _objectFactory;
        private ILogger _logger;

        public AutenticacionController(IConfiguration configuration, IHostingEnvironment env, IObjectFactory objectFactory, ILogger logger)
        {
            _configuration = configuration;
            _enviroment = env;
            _objectFactory = objectFactory;
            _logger = logger;
        }

        [HttpGet("cipher")]
        public async Task<IActionResult> Cipher(string userid, string password)
        {
            var service = new Services.N.Autenticacion.AutenticacionServices(_configuration, _objectFactory);
            var result = new MicroserviceModel<string>
            {
                Header = new Header
                {
                    Status = Status.Ok
                }
            };


            try
            {
                result.Model = await service.GetSCSCipherPassword(userid, password);
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());

                result.Header.Status = Status.Error;
                result.Header.Description = e.ToString();
                result.Header.ErrorCode = $"{ErrorPrefix}_Chiper";

                return new ObjectResult(result) { StatusCode = 500 };
            }
        }

    }
}
