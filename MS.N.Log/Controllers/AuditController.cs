using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BGBA.Models.N.Core.Microservices;
using MongoDB.Bson;
using BGBA.MS.N.Log.DAO;
using BGBA.MS.N.Log.Models;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace BGBA.MS.N.Log.Controllers
{
    [Route("api/audit")]
    public class AuditController : MicroserviceController
    {
        private readonly MongoRepository _repository;

        public AuditController(ILogger<AuditController> logger, IConfiguration configuration, DAO.MongoRepository repository) : base(logger)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Audit([FromBody]Audit data)
        {
            await _repository.Add<Audit>(data);
            return Ok(data.Id.ToString());
        }

        [HttpGet]
        public IActionResult GetAudit()
        {
            return new ObjectResult(_repository.All<Audit>());
        }

        [HttpGet("{id}")]
        public IActionResult GetAudit(string id)
        {

            var objId = new ObjectId(id);

            var result = _repository.Single<Audit>(a => a.Id == objId);

            if (result == null)
                return NotFound();

            return new ObjectResult(result);
        }

        [HttpGet("du/{du}")]
        public async Task<IActionResult> GetAuditByDU(string du)
        {
            var builder = Builders<Audit>.Filter;
            var query = builder.Eq("data.subscription_info.msg_dni.value", du);

            var result = await _repository.Find(query);

            if (result == null)
                return NotFound();

            return new ObjectResult(result);
        }
    }
}