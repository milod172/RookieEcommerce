using FluentValidation.TestHelper;
using NovaFashion.API.Features.Products;
using Xunit;

namespace NovaFashion.Tests.Features.Products
{
    public class CreateProductValidatorTests
    {
        private readonly CreateProductValidator _validator = new();

        #region Helpers

        private static CreateProductRequest CreateValidRequest() => new()
        {
            ProductName = "Áo Thun Basic",
            Description = "Mô tả sản phẩm hợp lệ",
            Details = null,
            TotalQuantity = 1,
            UnitPrice = 1m
        };

        #endregion

        // -------------------------------------------------------------------------
        // Valid request
        // -------------------------------------------------------------------------

        [Fact]
        public void Validate_ValidRequest_ShouldPassAllRules()
        {
            // Arrange
            var request = CreateValidRequest();

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        // -------------------------------------------------------------------------
        // ProductName
        // -------------------------------------------------------------------------

        [Fact]
        public void ProductName_WhenEmpty_ShouldHaveError()
        {
            // Arrange
            var request = CreateValidRequest();
            request.ProductName = string.Empty;

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.ProductName)
                .WithErrorMessage(CreateProductValidator.ProductNameRequired);
        }

        [Fact]
        public void ProductName_WhenNull_ShouldHaveError()
        {
            // Arrange
            var request = CreateValidRequest();
            request.ProductName = null!;

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.ProductName)
                .WithErrorMessage(CreateProductValidator.ProductNameRequired);
        }

        [Fact]
        public void ProductName_WhenWhitespace_ShouldHaveError()
        {
            // Arrange
            var request = CreateValidRequest();
            request.ProductName = "   ";

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.ProductName)
                .WithErrorMessage(CreateProductValidator.ProductNameRequired);
        }

        [Fact]
        public void ProductName_WhenProvided_ShouldNotHaveError()
        {
            // Arrange
            var request = CreateValidRequest();
            request.ProductName = "Áo Thun";

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.ProductName);
        }

        // -------------------------------------------------------------------------
        // Description
        // -------------------------------------------------------------------------

        [Fact]
        public void Description_WhenEmpty_ShouldHaveRequiredError()
        {
            // Arrange
            var request = CreateValidRequest();
            request.Description = string.Empty;

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage(CreateProductValidator.DescriptionRequired);
        }

        [Fact]
        public void Description_WhenNull_ShouldHaveRequiredError()
        {
            // Arrange
            var request = CreateValidRequest();
            request.Description = null!;

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage(CreateProductValidator.DescriptionRequired);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(499)]
        [InlineData(500)]
        public void Description_WhenLengthWithinLimit_ShouldNotHaveError(int length)
        {
            // Arrange
            var request = CreateValidRequest();
            request.Description = new string('a', length);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [InlineData(501)]
        [InlineData(1000)]
        public void Description_WhenExceedsMaxLength_ShouldHaveTooLongError(int length)
        {
            // Arrange
            var request = CreateValidRequest();
            request.Description = new string('a', length);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage(CreateProductValidator.DescriptionTooLong);
        }

        // -------------------------------------------------------------------------
        // Details (optional — chỉ giới hạn độ dài)
        // -------------------------------------------------------------------------

        [Fact]
        public void Details_WhenNull_ShouldNotHaveError()
        {
            // Arrange
            var request = CreateValidRequest();
            request.Details = null;

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Details);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(999)]
        [InlineData(1000)]
        public void Details_WhenLengthWithinLimit_ShouldNotHaveError(int length)
        {
            // Arrange
            var request = CreateValidRequest();
            request.Details = length == 0 ? string.Empty : new string('x', length);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Details);
        }

        [Theory]
        [InlineData(1001)]
        [InlineData(2000)]
        public void Details_WhenExceedsMaxLength_ShouldHaveTooLongError(int length)
        {
            // Arrange
            var request = CreateValidRequest();
            request.Details = new string('x', length);

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.Details)
                .WithErrorMessage(CreateProductValidator.DetailsTooLong);
        }

        // -------------------------------------------------------------------------
        // TotalQuantity
        // -------------------------------------------------------------------------

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-9999)]
        public void TotalQuantity_WhenZeroOrNegative_ShouldHaveGreaterThanZeroError(int quantity)
        {
            // Arrange
            var request = CreateValidRequest();
            request.TotalQuantity = quantity;

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.TotalQuantity)
                .WithErrorMessage(CreateProductValidator.TotalQuantityMustBeGreaterThanZero);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5000)]
        [InlineData(9999)]
        public void TotalQuantity_WhenWithinValidRange_ShouldNotHaveError(int quantity)
        {
            // Arrange
            var request = CreateValidRequest();
            request.TotalQuantity = quantity;

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TotalQuantity);
        }

        [Theory]
        [InlineData(10000)]
        [InlineData(99999)]
        public void TotalQuantity_WhenExceedsMaxLimit_ShouldHaveTooLargeError(int quantity)
        {
            // Arrange
            var request = CreateValidRequest();
            request.TotalQuantity = quantity;

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.TotalQuantity)
                .WithErrorMessage(CreateProductValidator.TotalQuantityTooLarge);
        }

        // -------------------------------------------------------------------------
        // UnitPrice
        // -------------------------------------------------------------------------

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-1_000_000)]
        public void UnitPrice_WhenZeroOrNegative_ShouldHaveGreaterThanZeroError(double price)
        {
            // Arrange
            var request = CreateValidRequest();
            request.UnitPrice = (decimal)price;

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.UnitPrice)
                .WithErrorMessage(CreateProductValidator.UnitPriceMustBeGreaterThanZero);
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(1)]
        [InlineData(500_000_000)]
        [InlineData(1_000_000_000)]
        public void UnitPrice_WhenWithinValidRange_ShouldNotHaveError(double price)
        {
            // Arrange
            var request = CreateValidRequest();
            request.UnitPrice = (decimal)price;

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.UnitPrice);
        }

        [Theory]
        [InlineData(1_000_000_001)]
        [InlineData(9_999_999_999)]
        public void UnitPrice_WhenExceedsMaxLimit_ShouldHaveTooLargeError(long price)
        {
            // Arrange
            var request = CreateValidRequest();
            request.UnitPrice = price;

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result
                .ShouldHaveValidationErrorFor(x => x.UnitPrice)
                .WithErrorMessage(CreateProductValidator.UnitPriceTooLarge);
        }

        // -------------------------------------------------------------------------
        // Multiple errors cùng lúc
        // -------------------------------------------------------------------------

        [Fact]
        public void Validate_WhenMultipleFieldsInvalid_ShouldReturnAllErrors()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                ProductName = string.Empty,
                Description = string.Empty,
                Details = null,
                TotalQuantity = 0,
                UnitPrice = 0m
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductName);
            result.ShouldHaveValidationErrorFor(x => x.Description);
            result.ShouldHaveValidationErrorFor(x => x.TotalQuantity);
            result.ShouldHaveValidationErrorFor(x => x.UnitPrice);
        }
    }
}
