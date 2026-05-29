using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using HelpMe.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HelpMe.Tests.Controllers;

[TestFixture]
public class CategoriesControllerTests
{
    private Mock<ICategoryService> _categoryServiceMock = null!;
    private CategoriesController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _categoryServiceMock = new Mock<ICategoryService>();
        _controller = new CategoriesController(_categoryServiceMock.Object);
    }

    // --- GetAll ---

    [Test]
    public async Task GetAll_ReturnsOkWithList()
    {
        var categories = new List<CategoryDto>
        {
            new() { Id = 1, Name = "ВиК", IsActive = true },
            new() { Id = 2, Name = "Електро", IsActive = true }
        };

        _categoryServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(categories);

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(categories));
    }

    [Test]
    public async Task GetAll_WhenEmpty_ReturnsOkWithEmptyList()
    {
        _categoryServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<CategoryDto>());

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var list = ok!.Value as List<CategoryDto>;
        Assert.That(list, Is.Empty);
    }

    // --- GetById ---

    [Test]
    public async Task GetById_WhenFound_ReturnsOkWithCategory()
    {
        var category = new CategoryDto { Id = 1, Name = "ВиК", IsActive = true };

        _categoryServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(category);

        var result = await _controller.GetById(1);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(category));
    }

    [Test]
    public async Task GetById_WhenNotFound_ReturnsNotFound()
    {
        _categoryServiceMock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((CategoryDto?)null);

        var result = await _controller.GetById(999);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    // --- Create ---

    [Test]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var dto = new CreateCategoryDto { Name = "Нова категория" };
        var created = new CategoryDto { Id = 5, Name = "Нова категория", IsActive = true };

        _categoryServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

        var result = await _controller.Create(dto);

        var createdAt = result as CreatedAtActionResult;
        Assert.That(createdAt, Is.Not.Null);
        Assert.That(createdAt!.Value, Is.EqualTo(created));
        Assert.That(createdAt.RouteValues!["id"], Is.EqualTo(created.Id));
    }

    // --- Update ---

    [Test]
    public async Task Update_WhenSuccess_ReturnsNoContent()
    {
        var dto = new UpdateCategoryDto { Name = "Ново Електро" };

        _categoryServiceMock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(true);

        var result = await _controller.Update(1, dto);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Update_WhenNotFound_ReturnsNotFound()
    {
        var dto = new UpdateCategoryDto { Name = "X" };

        _categoryServiceMock.Setup(s => s.UpdateAsync(999, dto)).ReturnsAsync(false);

        var result = await _controller.Update(999, dto);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    // --- Deactivate ---

    [Test]
    public async Task Deactivate_WhenSuccess_ReturnsNoContent()
    {
        _categoryServiceMock.Setup(s => s.DeactivateAsync(1)).ReturnsAsync(true);

        var result = await _controller.Deactivate(1);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Deactivate_WhenNotFound_ReturnsNotFound()
    {
        _categoryServiceMock.Setup(s => s.DeactivateAsync(999)).ReturnsAsync(false);

        var result = await _controller.Deactivate(999);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
