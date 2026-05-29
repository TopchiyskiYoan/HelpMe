using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using HelpMe.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HelpMe.Tests.Controllers;

[TestFixture]
public class RegionsControllerTests
{
    private Mock<IRegionService> _regionServiceMock = null!;
    private RegionsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _regionServiceMock = new Mock<IRegionService>();
        _controller = new RegionsController(_regionServiceMock.Object);
    }

    // --- GetAll ---

    [Test]
    public async Task GetAll_ReturnsOkWithList()
    {
        var regions = new List<RegionDto>
        {
            new() { Id = 1, Name = "София (град)", Cities = [new CityDto { Id = 1, Name = "Столична" }] },
            new() { Id = 2, Name = "Пловдив", Cities = [new CityDto { Id = 2, Name = "Пловдив" }] }
        };

        _regionServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(regions);

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(regions));
    }

    [Test]
    public async Task GetAll_WhenEmpty_ReturnsOkWithEmptyList()
    {
        _regionServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<RegionDto>());

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var list = ok!.Value as List<RegionDto>;
        Assert.That(list, Is.Empty);
    }

    // --- GetById ---

    [Test]
    public async Task GetById_WhenFound_ReturnsOkWithRegion()
    {
        var region = new RegionDto
        {
            Id = 1,
            Name = "София (град)",
            Cities = [new CityDto { Id = 1, Name = "Столична" }]
        };

        _regionServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(region);

        var result = await _controller.GetById(1);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(region));
    }

    [Test]
    public async Task GetById_WhenNotFound_ReturnsNotFound()
    {
        _regionServiceMock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((RegionDto?)null);

        var result = await _controller.GetById(999);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
