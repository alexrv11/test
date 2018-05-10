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
        private readonly IConfiguration _configuration;
        private readonly IClientServices _clientServices;
        private readonly ILogger _logger;
        private readonly IAfipServices _afipServices;

        public ClientController(IClientServices clientServices, IAfipServices afipServices,
            ILogger<ClientController> logger, IConfiguration configuration) : base(logger, configuration)
        {
            _configuration = configuration;
            _clientServices = clientServices;
            _logger = logger;
            _afipServices = afipServices;

            var trace = new Models.N.Core.Trace.TraceEventHandler(delegate (object sender, Models.N.Core.Trace.TraceEventArgs e)
            {
                base.Communicator_TraceHandler(sender, e);
            });

            _clientServices.TraceHandler += trace;
            _afipServices.TraceHandler += trace;
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
                var dataPadron = await _afipServices.GetClient(cuix);
                _logger.LogInformation("Afip services OK.");

                if (dataPadron == null)
                    return NotFound();


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

                try
                {
                    dataPadron.HostId = await _clientServices.GetClientNV(dataPadron);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    _logger.LogInformation("Error getting client from NV.");
                }

                return new ObjectResult(dataPadron);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "Error al consultar los datos padron.");
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody]MinimumClientData client)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                await _clientServices.AddClient(client);
                _logger.LogInformation("add client.");

                var idHost = await _clientServices.GetClientNV(client);

                return new ObjectResult(idHost);
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

        [HttpGet("email")]
        public async Task<IActionResult> SendEmail(string email, string attachmentName)
        {
            var body = (await System.IO.File.ReadAllTextAsync(_configuration["WelcomeEmail:Body"]));
            var attachment = new Attachment(new MemoryStream(await System.IO.File.ReadAllBytesAsync(_configuration[$"WelcomeEmail:AttachmentPath"])), attachmentName, MediaTypeNames.Application.Pdf);


            var msg = CrearMensaje(_configuration["WelcomeEmail:Sender"], email, _configuration["WelcomeEmail:Subject"], body, new[] { attachment }, true);

            EnviarMail(msg);

            return Ok();
        }

        private MailMessage CrearMensaje(string origen, string destino, string asunto, string body, ICollection<Attachment> attachments, bool isBodyHtml = false)
        {
            MailMessage message = new MailMessage();

            message.From = new MailAddress(origen);
            message.To.Add(new MailAddress(destino));
            message.Subject = asunto;
            message.Body = body;
            message.IsBodyHtml = isBodyHtml;

            if (attachments != null)
            {
                foreach (var item in attachments)
                {
                    message.Attachments.Add(item);
                }
            }

            return message;
        }

        private MailMessage CrearMensaje(string sender, string origen, string displayName, ICollection<string> destino, ICollection<string> cc, string asunto, string body, ICollection<Attachment> attachments)
        {
            MailMessage message = new MailMessage
            {
                From = new MailAddress(sender, System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(displayName))
            };
            message.ReplyToList.Add(new MailAddress(origen, System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(displayName)));

            foreach (var item in destino)
                message.To.Add(new MailAddress(item));

            message.Subject = asunto;
            message.Body = body;

            if (cc != null)
                foreach (var item in cc)
                    message.CC.Add(item);

            if (attachments != null)
                foreach (var item in attachments)
                    message.Attachments.Add(item);

            return message;
        }

        private void EnviarMail(MailMessage mensaje)
        {
            SmtpClient smtpClient;
            string host;
            bool enabledSSl;

            host = _configuration["Smtp:Url"];
            enabledSSl = bool.Parse(_configuration["Smtp:EnabledSSL"]);
            smtpClient = new SmtpClient(host)
            {
                EnableSsl = enabledSSl
            };

            smtpClient.Send(mensaje);
        }

    }
}