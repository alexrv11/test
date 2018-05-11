using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BGBA.MS.N.Log;
using BGBA.MS.N.Log.Models;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json;
using Xunit;

namespace MS.N.Log.Tests
{
    public class AuditControllerShould
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public AuditControllerShould()
        {

            //WebHost.CreateDefaultBuilder()
            //    .UseStartup<Startup>()
            //    .Build();

            _server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task GetAuditOkResult()
        {
            // Act
            var response = await _client.GetAsync("/api/audit");
            response.EnsureSuccessStatusCode();

            response.Should().BeOfType<OkResult>();

            var responseString = await response.Content.ReadAsStringAsync();

            responseString.Should().BeAssignableTo<ObjectId>();
        }

        [Fact]
        public async Task PostAuditObjectResult()
        {

            var content = "{\"data\":\"this is a test\"}";
            
            // Act
            var response = await _client.PostAsync("/api/audit",new StringContent(content));
            response.EnsureSuccessStatusCode();
            response.Should().BeOfType<ObjectResult>();
            response.Should().BeAssignableTo<ObjectId>();
        }



        [Fact]
        public void Save_Object_1()
        {
            var serviceProvider = new ServiceCollection()
                                .AddLogging()
                                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();
            var logger = factory.CreateLogger<BGBA.MS.N.Log.Controllers.AuditController>();
            var config = serviceProvider.GetService<IConfiguration>();

            var controller = new BGBA.MS.N.Log.Controllers.AuditController(logger, config, new BGBA.MS.N.Log.DAO.MongoRepository(config));

            var data = new Audit
            {
                Data = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText("Save_Object_1.json"))
            };

            var add = controller.Audit(data);

            add.Should().NotBeNull();

            var vmAdd = add.Should().BeOfType<ObjectResult>().Subject;

            var idAdd = vmAdd.Should().BeAssignableTo<string>().Subject;

            var obj = controller.GetAudit(idAdd);

            var vmGet = obj.Should().BeOfType<ObjectResult>().Subject;
            vmGet.Should().BeEquivalentTo(data);
        }
    }
}
