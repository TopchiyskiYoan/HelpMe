using HelpMe.Application.Services;
using HelpMe.Domain.Entities;
using HelpMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Tests.Services;

[TestFixture]
public class RegionServiceTests
{
    private ApplicationDbContext _context = null!;
    private RegionService _regionService = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _regionService = new RegionService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetAllAsync_ReturnsOnlyActiveRegionsAndCities()
    {
        _context.Regions.AddRange(
            new Region
            {
                Name = "София",
                IsActive = true,
                Cities =
                [
                    new City { Name = "Столична", IsActive = true },
                    new City { Name = "Стара община", IsActive = false }
                ]
            },
            new Region { Name = "Архив", IsActive = false }
        );
        await _context.SaveChangesAsync();

        var result = await _regionService.GetAllAsync();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("София"));
        Assert.That(result[0].Cities, Has.Count.EqualTo(1));
        Assert.That(result[0].Cities[0].Name, Is.EqualTo("Столична"));
    }

    [Test]
    public async Task GetByIdAsync_WhenActive_ReturnsRegion()
    {
        var region = new Region
        {
            Name = "Пловдив",
            IsActive = true,
            Cities = [new City { Name = "Пловдив град", IsActive = true }]
        };
        _context.Regions.Add(region);
        await _context.SaveChangesAsync();

        var result = await _regionService.GetByIdAsync(region.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Пловдив"));
        Assert.That(result.Cities, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetByIdAsync_WhenInactive_ReturnsNull()
    {
        var region = new Region { Name = "Архив", IsActive = false };
        _context.Regions.Add(region);
        await _context.SaveChangesAsync();

        var result = await _regionService.GetByIdAsync(region.Id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        var result = await _regionService.GetByIdAsync(999);

        Assert.That(result, Is.Null);
    }
}
