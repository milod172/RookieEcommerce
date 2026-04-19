using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NovaFashion.API.Entities;
using NovaFashion.API.Features.Products;
using NovaFashion.API.Shared.Extensions;
using NSubstitute;
using Xunit;

namespace MyProject.Tests.Features.Products;

public class CreateProductTests
{
    private readonly IProductRepository _repoMock;
    private readonly CreateProduct _endpoint;

    public CreateProductTests()
    {
        
        // NSubstitute uses Substitute.For<T>() instead of new Mock<T>()
        _repoMock = Substitute.For<IProductRepository>();

        // Factory.Create handles the internal FastEndpoints plumbing
        _endpoint = Factory.Create<CreateProduct>(_repoMock);
    }

    #region Endpoint Tests

    
    [Fact]
    public async Task HandleAsync_ValidRequest_ShouldReturnCreatedAndCallRepository()
    {
        // ARRANGE
        var request = new CreateProductRequest
        {
            ProductName = "Mechanical Keyboard",
            Description = "RGB Backlit",
            UnitPrice = 120.50m,
            TotalQuantity = 10,
            CategoryId = Guid.NewGuid()
        };

        // ACT
        await _endpoint.HandleAsync(request, default);
        var response = _endpoint.Response;

        // ASSERT
        _endpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status201Created);

        response.Should().NotBeNull();
        response.ProductName.Should().Be(request.ProductName);

        // NSubstitute verification: check if AddAsync was called once with any Product
        await _repoMock.Received(1).AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Validator Tests

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validator_WhenNameIsEmpty_ShouldHaveError(string? invalidName)
    {
        // ARRANGE
        var validator = new CreateProductValidator();
        var request = new CreateProductRequest { ProductName = invalidName! };

        // ACT
        var result = await validator.ValidateAsync(request);

        // ASSERT
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(request.ProductName))
                     .Which.ErrorMessage.Should().Be("Tên sản phẩm không được để trống");
    }

    [Fact]
    public async Task Validator_WhenPriceIsZero_ShouldHaveError()
    {
        // ARRANGE
        var validator = new CreateProductValidator();
        var request = new CreateProductRequest { ProductName = "Valid", UnitPrice = 0 };

        // ACT
        var result = await validator.ValidateAsync(request);

        // ASSERT
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == "Giá phải lớn hơn 0");
    }

    #endregion

    #region Mapper Tests

    [Fact]
    public void Mapper_ToEntity_ShouldMapAllFields()
    {
        // ARRANGE
        var mapper = new CreateProductMapper();
        var categoryId = Guid.NewGuid();
        var request = new CreateProductRequest
        {
            ProductName = "Monitor",
            Description = "4K OLED",
            UnitPrice = 800m,
            Details = "HDMI 2.1",
            TotalQuantity = 5,
            CategoryId = categoryId
        };

        // ACT
        var entity = mapper.ToEntity(request);

        // ASSERT
        entity.Should().BeEquivalentTo(request, options => options
            .ExcludingMissingMembers()); // Checks that matching names map correctly
    }

    [Fact]
    public void Mapper_FromEntity_ShouldHandleNullNavigationProperties()
    {
        // ARRANGE
        var mapper = new CreateProductMapper();
        var entity = new Product
        {
            Id = Guid.NewGuid(),
            ProductName = "No Category Laptop",
            Category = null // Simulate a case where Category is not loaded/included
        };

        // ACT
        var dto = mapper.FromEntity(entity);

        // ASSERT
        dto.CategoryName.Should().BeEmpty(); 
        dto.CategoryId.Should().Be(Guid.Empty);
    }

    #endregion

    #region Sku Generation Tests

    [Theory]
    [InlineData("Bàn Phím Cơ", "BAN-PHIM-CO")]
    [InlineData("Gaming Mouse", "GAMING-MOUSE")]
    public void SkuGenerator_ShouldHandleVariousNames(string inputName, string expectedSku)
    {
        // Arrange
        var product = new Product { ProductName = inputName };

        // Act
        var result = product.GenerateSku();

        // Assert
        result.Should().Be(expectedSku);
    }
     #endregion
}