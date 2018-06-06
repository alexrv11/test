using System;
using System.Threading.Tasks;
using BGBA.MS.N.Client.Controllers;
using BGBA.Services.N.Client;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.N.Core.Tests.Attributes;
using Moq;
using Xunit;

namespace BGBA.MS.N.Client.Tests
{
    public class ClientControllerShould
    {

        public ClientControllerShould()
        {


        }

        [Theory]
        [JsonFileData("")]
        public async Task GetClient(GetClientRequest request, BGBA.Models.N.Client.ClientData expected, Type type)
        {
            var serviceMock = new Mock<IClientServices>();
            serviceMock.Setup(x => x.GetClientAfip(""))
                .Returns(() => new TaskFactory<BGBA.Models.N.Client.ClientData>().StartNew(() => expected));

            var loggerMock = new Mock<ILogger<ClientController>>();

            var controller = new ClientController(serviceMock.Object, loggerMock.Object);

            var result = await controller.GetClient(request.DocumentNumber, (BGBA.MS.N.Client.ViewModels.Sex)Enum.Parse(typeof(BGBA.MS.N.Client.ViewModels.Sex), request.Sex), null);

            var client = result.Should().BeOfType(type)
                .And.Should().BeAssignableTo<BGBA.Models.N.Client.ClientData>().Subject;

            client.Should().BeEquivalentTo(expected);
        }

    }
}
