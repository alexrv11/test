﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BGBA.Models.N.Core.Microservices;
using MongoDB.Bson;
using BGBA.MS.N.Log.DAO;
using BGBA.MS.N.Log.Models;
using System.Threading.Tasks;

namespace BGBA.MS.N.Log.Controllers
{
    [Route("api/audit")]
    public class AuditController : MicroserviceController
    {
        private readonly MongoRepository _repository;

        public AuditController(ILogger<AuditController> logger, IConfiguration configuration, DAO.MongoRepository repository) : base(logger, configuration)
        {
            _repository = repository;
        }

        [HttpPost()]
        public async Task<IActionResult> Audit([FromBody]Audit data)
        {
            await _repository.Add<Audit>(data); 
            return Ok(data.Id.ToString());
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
    }
}