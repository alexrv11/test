using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using BGBA.Models.N.Core.Microservices;
using BGBA.Models.N.Location;
using BGBA.Services.N.Client;
using BGBA.Services.N.Afip;
using BGBA.Models.N.Client;
using BGBA.MS.N.Client.ViewModels;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;

namespace BGBA.MS.N.Client.Controllers
{
    [Route("api/client")]
    public class ClientController : MicroserviceController
    {
        private readonly IClientServices _clientServices;
        private readonly ILogger _logger;

        public ClientController(IClientServices clientServices,
            ILogger<ClientController> logger) : base(logger)
        {
            _clientServices = clientServices;
            _logger = logger;

            var trace = new Models.N.Core.Trace.TraceEventHandler(delegate (object sender, Models.N.Core.Trace.TraceEventArgs e)
            {
                base.Communicator_TraceHandler(sender, e);
            });

            _clientServices.TraceHandler += trace;
        }

        [HttpPost("{du}/{sex}")]
        public async Task<IActionResult> GetClient(string du, Sex sex, [FromBody]MapOptions mapOptions)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var cuix = String.Empty;

            try
            {
                cuix = await _clientServices.GetCuix(du, sex.ToString());
                _logger.LogInformation("Cuix OK.");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error getting CUIX");
            }

            try
            {
                var dataPadron = await _clientServices.GetClientAfip(cuix);
                _logger.LogInformation("Afip services OK.");

                if (dataPadron == null)
                    return NotFound();


                await _clientServices.GetClientNV(dataPadron);

                var addressNV = await _clientServices.GetAddressNV(dataPadron.HostId);

                dataPadron.Addresses.Add(addressNV);

                try
                {
                    var normalizedAddresses = new List<Address>();

                    foreach (var item in dataPadron.Addresses)
                    {
                        mapOptions.Address = item;

                        var result = await _clientServices.NormalizeAddress(mapOptions);

                        if (result != null)
                        {
                            normalizedAddresses.Add(result);
                            _logger.LogInformation("Normalize Address OK.");
                        }
                        else
                        {
                            _logger.LogInformation("Address not found.");
                        }
                    }

                    if (normalizedAddresses.Count > 0)
                        dataPadron.Addresses = normalizedAddresses;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    _logger.LogTrace("Error normalizing address.");
                }

                return new ObjectResult(dataPadron);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error al consultar los datos padron.");
            }

        }

        [HttpPost("NV")]
        public async Task<IActionResult> GetClientNV([FromBody]ClientData clientData)
        {
            try
            {
                await _clientServices.GetClientNV(clientData);

                return new ObjectResult(clientData.HostId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error getting client from NV.");

            }
        }

        [HttpPost("NV/Address/{idHost}")]
        public async Task<IActionResult> GetAddresNV(string idHost)
        {
            try
            {
                var address = await _clientServices.GetAddressNV(idHost);

                var result = await _clientServices.NormalizeAddress(new MapOptions { Address = address});


                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error getting address from NV.");

            }
        }

        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody]ClientData client)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _clientServices.AddClientNV(client);
                _logger.LogInformation("add client.");

                await _clientServices.GetClientNV(client);

                return new ObjectResult(client);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "error adding client.");
            }

        }

        [HttpPatch("{idHost}")]
        public async Task<IActionResult> UpdateClient(string idHost, [FromBody]UpdateClientVM updateClient)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                await _clientServices.UpdateClientNV(idHost, updateClient.Address, updateClient.Email, updateClient.Phone);
                _logger.LogInformation("update client.");

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "error updating the client address.");
            }
        }

        [HttpPost("email")]
        public async Task<IActionResult> SendEmail([FromBody]SendEmailVM httpParams)
        {
            await _clientServices.SendEmail(httpParams.Data, httpParams.Email, httpParams.AttachmentNameWithExtension);

            return Ok();
        }
    }
}