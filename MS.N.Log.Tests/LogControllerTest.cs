using System;
using System.IO;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MS.N.Log.Models;
using Newtonsoft.Json;
using Xunit;

namespace MS.N.Log.Tests
{
    public class AuditControllerTest
    {
        [Fact]
        public void Save_Object_1()
        {
            var serviceProvider = new ServiceCollection()
                                .AddLogging()
                                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();
            var logger = factory.CreateLogger<Controllers.AuditController>();
            var config = serviceProvider.GetService<IConfiguration>();

            var controller = new Controllers.AuditController(logger,config);
            
            var data = new Audit
            {
                Id = Guid.NewGuid(),
                Data = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText("Save_Object_1.json"))
            };

            var add = controller.Audit(data);

            add.Should().NotBeNull();

            var vmAdd = add.Should().BeOfType<ObjectResult>().Subject;

            var idAdd = vmAdd.Should().BeAssignableTo<Guid>().Subject;

            var obj = controller.GetAudit(idAdd);

            var vmGet = obj.Should().BeOfType<ObjectResult>().Subject;
            vmGet.Should().BeEquivalentTo(data);
        }
    }
}
