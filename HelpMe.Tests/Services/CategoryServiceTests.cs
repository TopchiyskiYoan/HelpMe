using HelpMe.Application.DTOs;
using HelpMe.Application.Services;
using HelpMe.Domain.Entities;
using HelpMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Tests.Services;

[TestFixture]
public class CategoryServiceTests
{
    private ApplicationDbContext _context = null!;
    private CategoryService _categoryService = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _categoryService = new CategoryService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    // --- GetAllAsync ---

    [Test]
    public async Task GetAllAsync_ReturnsOnlyActiveCategories()
    {
        _context.ServiceCategories.AddRange(
            new ServiceCategory { Name = "Електро", IsActive = true },
            new ServiceCategory { Name = "Стара", IsActive = false }
        );
        await _context.SaveChangesAsync();

        var result = await _categoryService.GetAllAsync();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Електро"));
    }

    [Test]
    public async Task GetAllAsync_IncludesActiveSubCategories()
    {
        var category = new ServiceCategory
        {
            Name = "ВиК",
            IsActive = true,
            SubCategories =
            [
                new ServiceSubCategory { Name = "Течове", Description = "Отстраняване на течове", IsActive = true },
                new ServiceSubCategory { Name = "Стара", Description = "Неактивна", IsActive = false }
            ]
        };
        _context.ServiceCategories.Add(category);
        await _context.SaveChangesAsync();

        var result = await _categoryService.GetAllAsync();

        Assert.That(result[0].SubCategories, Has.Count.EqualTo(1));
        Assert.That(result[0].SubCategories[0].Name, Is.EqualTo("Течове"));
    }

    // --- GetByIdAsync ---

    [Test]
    public async Task GetByIdAsync_WhenExists_ReturnsCategory()
    {
        var category = new ServiceCategory { Name = "Електро", IsActive = true };
        _context.ServiceCategories.Add(category);
        await _context.SaveChangesAsync();

        var result = await _categoryService.GetByIdAsync(category.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Електро"));
    }

    [Test]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        var result = await _categoryService.GetByIdAsync(999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_WhenInactive_ReturnsNull()
    {
        var category = new ServiceCategory { Name = "Архив", IsActive = false };
        _context.ServiceCategories.Add(category);
        await _context.SaveChangesAsync();

        var result = await _categoryService.GetByIdAsync(category.Id);

        Assert.That(result, Is.Null);
    }

    // --- CreateAsync ---

    [Test]
    public async Task CreateAsync_SavesCategoryAndReturnsDto()
    {
        var dto = new CreateCategoryDto { Name = "Нова категория" };

        var result = await _categoryService.CreateAsync(dto);

        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.Name, Is.EqualTo("Нова категория"));
        Assert.That(result.IsActive, Is.True);

        var inDb = await _context.ServiceCategories.FindAsync(result.Id);
        Assert.That(inDb, Is.Not.Null);
    }

    // --- UpdateAsync ---

    [Test]
    public async Task UpdateAsync_WhenExists_UpdatesNameAndReturnsTrue()
    {
        var category = new ServiceCategory { Name = "Стара", IsActive = true };
        _context.ServiceCategories.Add(category);
        await _context.SaveChangesAsync();

        var success = await _categoryService.UpdateAsync(category.Id, new UpdateCategoryDto { Name = "Нова" });

        Assert.That(success, Is.True);
        var updated = await _context.ServiceCategories.FindAsync(category.Id);
        Assert.That(updated!.Name, Is.EqualTo("Нова"));
    }

    [Test]
    public async Task UpdateAsync_WhenNotFound_ReturnsFalse()
    {
        var success = await _categoryService.UpdateAsync(999, new UpdateCategoryDto { Name = "X" });

        Assert.That(success, Is.False);
    }

    // --- DeactivateAsync ---

    [Test]
    public async Task DeactivateAsync_WhenExists_SetsIsActiveFalseAndReturnsTrue()
    {
        var category = new ServiceCategory { Name = "Електро", IsActive = true };
        _context.ServiceCategories.Add(category);
        await _context.SaveChangesAsync();

        var success = await _categoryService.DeactivateAsync(category.Id);

        Assert.That(success, Is.True);
        var deactivated = await _context.ServiceCategories.FindAsync(category.Id);
        Assert.That(deactivated!.IsActive, Is.False);
    }

    [Test]
    public async Task DeactivateAsync_WhenNotFound_ReturnsFalse()
    {
        var success = await _categoryService.DeactivateAsync(999);

        Assert.That(success, Is.False);
    }
}
