using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BGBA.Services.N.ATReference;
using BGBA.Services.N.Location;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.N.Core.Tests.Attributes;
using Moq;
using MS.N.Location.Controllers;
using Xunit;

namespace MS.N.Location.Tests
{
    public class LocationControllerShould
    {
        [Theory]
        [JsonFileData("PredictiveListData.json")]
        public async Task ReturnPredictiveList(BGBA.Models.N.Location.MapOptions value, BGBA.Models.N.Location.PredictionsResult expected)
        {
            var serviceTableMock = new Mock<ITableServices>();
            var serviceMapMock = new Mock<IMapServices>();
            serviceMapMock.Setup(x => x.GetPrediction(value))
                .Returns(() => new TaskFactory<BGBA.Models.N.Location.PredictionsResult>().StartNew(() => expected));

            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<LocationController>>();

            var controller = new LocationController(serviceMapMock.Object, loggerMock.Object, serviceTableMock.Object, mapperMock.Object);

            var result = await controller.Predictive(value);

            var client = result.Should().BeOfType(typeof(ObjectResult))
                .And.Should().BeAssignableTo<List<BGBA.Models.N.Location.PredictionsResult>>().Subject;

            client.Should().BeEquivalentTo(expected);
        }
    }
}
