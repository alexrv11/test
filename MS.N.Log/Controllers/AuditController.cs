using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BGBA.Models.N.Core.Microservices;
using MongoDB.Bson;
using BGBA.MS.N.Log.DAO;
using BGBA.MS.N.Log.Models;

namespace BGBA.MS.N.Log.Controllers
{
    public class AuditController : MicroserviceController
    {
        private readonly MongoRepository _repository;

        public AuditController(ILogger<AuditController> logger, IConfiguration configuration, DAO.MongoRepository repository) : base(logger, configuration)
        {
            _repository = repository;
        }

        [HttpPost("audit")]
        public IActionResult Audit(Audit data)
        {
            _repository.Add<Audit>(data);

            return Ok(data.Id);
        }

        [HttpGet("audit")]
        public IActionResult GetAudit(ObjectId id)
        {

            var result = _repository.Single<Audit>(a => a.Id == id);

            if (result == null)
                return NotFound();

            return new ObjectResult(result);
        }
    }
}