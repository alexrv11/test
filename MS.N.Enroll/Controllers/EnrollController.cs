using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BGBA.Services.N.Enrollment;
using Models.N.Core.Exceptions;
using BGBA.Models.N.Client.Enrollment;
using System.Linq;

namespace BGBA.MS.N.Adhesion.Controllers
{
    [Route("api/enroll")]
    public class EnrollController : BGBA.Models.N.Core.Microservices.MicroserviceController
    {
        public const string ErrorPrefix = "MS_Adherir";

        private readonly ILogger<EnrollController> _logger;
        private readonly IEnrollmentServices _enrollServices;

        public EnrollController(IConfiguration configuration, ILogger<EnrollController> logger, IEnrollmentServices enrollmentServices)
            : base(logger, configuration)
        {
            _logger = logger;
            _enrollServices = enrollmentServices;
        }

        [HttpPost("client")]
        public async Task<IActionResult> Cliente([FromBody]EnrollmentData data, [FromHeader]string sessionId = "")
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _enrollServices.TraceHandler += new BGBA.Models.N.Core.Trace.TraceEventHandler(delegate (object sender, BGBA.Models.N.Core.Trace.TraceEventArgs e)
                {
                    base.Communicator_TraceHandler(sender, e, sessionId);
                });

                var result = new Models.EnrollResult();

                if (string.IsNullOrEmpty(data.EnrollNumber))
                {
                    data.EncryptedPin = await _enrollServices.GetSCSCipherPasswordAsync(data.IdHost, data.Pin);
                    _logger.LogInformation($"Pin encrypted|sessionId:{sessionId}");

                    try
                    {
                        data.EnrollNumber = result.EnrollNumber = await _enrollServices.EnrollClientAsync(data);
                        result.EnrollState = Models.EnrollState.OK;
                        _logger.LogInformation($"Enroll client|sessionId:{sessionId}");
                    }
                    catch (TechnicalException e)
                    {
                        if (e.TechnicalCode == _enrollServices.ERROR_ALREDY_REGISTERED)
                            result.EnrollState = Models.EnrollState.ALREADY_ENROLL;
                        else if (e.TechnicalCode == _enrollServices.ERROR_PIN_SCS)
                            result.EnrollState = Models.EnrollState.INVALID_PIN;
                        else
                            throw;

                        return new ObjectResult(result);
                    }
                }
                else
                    result.EnrollNumber = data.EnrollNumber;

                try
                {
                    await _enrollServices.EnrollAlphanumericAsync(data);
                    result.AlphanumericState = Models.AlphanumericState.OK;
                    _logger.LogInformation($"Enroll alphanumeric|sessionId:{sessionId}");
                }
                catch (TechnicalException e)
                {
                    if (e.TechnicalCode == _enrollServices.NOT_INFORMED)
                        result.AlphanumericState = Models.AlphanumericState.NOT_INFORMED;
                    else if (e.TechnicalCode == _enrollServices.CONSECUTIVE_CHARACTERS)
                        result.AlphanumericState = Models.AlphanumericState.CONSECUTIVE_CHARACTERS;
                    else if (e.TechnicalCode == _enrollServices.INCORRECT_CHARACTERS)
                        result.AlphanumericState = Models.AlphanumericState.INCORRECT_CHARACTERS;
                    else
                        throw;
                }

                return new ObjectResult(result);
            }
            catch (System.Exception e)
            {
                _logger.LogError($"{e.ToString()}|sessionId:{sessionId}");
                return new ObjectResult("Error al adherir cliente.") { StatusCode = 500 };
            }
        }

        [HttpGet("client/{hostId}/{du}")]
        public async Task<IActionResult> Cliente(string hostId, string du, [FromHeader]string sessionId)
        {
            _enrollServices.TraceHandler += new BGBA.Models.N.Core.Trace.TraceEventHandler(delegate (object sender, BGBA.Models.N.Core.Trace.TraceEventArgs e)
            {
                base.Communicator_TraceHandler(sender, e, sessionId);
            });

            var result = await _enrollServices.GetEnrolledClientsAsync(du);

            return new ObjectResult(result.FirstOrDefault(r => r.HostId.PadLeft(10, '0') == hostId.PadLeft(10,'0')));
        }

        [HttpGet("client/{du}")]
        public async Task<IActionResult> Cliente(string du, [FromHeader]string sessionId)
        {
            _enrollServices.TraceHandler += new BGBA.Models.N.Core.Trace.TraceEventHandler(delegate (object sender, BGBA.Models.N.Core.Trace.TraceEventArgs e)
            {
                base.Communicator_TraceHandler(sender, e, sessionId);
            });

            var result = await _enrollServices.GetEnrolledClientsAsync(du);

            return new ObjectResult(result);
        }
    }
}
