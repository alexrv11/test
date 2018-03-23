﻿using System;
using System.Threading.Tasks;
using Core.N.Utils.ObjectFactory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.N.Adhesion;

namespace MS.N.Generar.Pin.Controllers
{
    [Route("api/generar")]
    public class GenerarPinController : Controller
    {
        protected const string ErrorPrefix = "MS_Generar_Pin";

        private IConfiguration _configuration;
        private IObjectFactory _objectFactory;
        private ILogger _logger;

        public GenerarPinController(IConfiguration configuration, IObjectFactory objectFactory, ILogger logger)
        {
            _configuration = configuration;
            _objectFactory = objectFactory;
            _logger = logger;
        }

        [Route("pin")]
        public async Task<IActionResult> Pin([FromBody]DatosAdhesion datos)
        {
            try
            {
                var services = new Services.N.Adhesion.AdhesionServices(_configuration, _objectFactory);
                var model = new Core.N.Models.MicroserviceModel<string>
                {
                    Header = new Core.N.Models.Header
                    {
                        Status = Core.N.Models.Status.Ok
                    },
                    Model = await services.AdherirUsuario(datos)
                };

                return new ObjectResult(model);

            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());

                var model = new Core.N.Models.MicroserviceModel<string>
                {
                    Header = new Core.N.Models.Header
                    {
                        Status = Core.N.Models.Status.Error,
                        Description = e.ToString(),
                        ErrorCode = ErrorPrefix
                    }
                };

                return new ObjectResult(model) { StatusCode = 500 };
            }

        }
    }
}