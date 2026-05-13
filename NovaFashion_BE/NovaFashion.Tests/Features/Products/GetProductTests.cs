using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using FluentValidation.Results;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Features.Products;
using NovaFashion.API.Shared.Pagination;
using Shouldly;
using Xunit;

namespace NovaFashion.Tests.Features.Products
{
   

    // ─────────────────────────────────────────────────────────────────────────────
    // 1. MAPPER TESTS
    // ─────────────────────────────────────────────────────────────────────────────

    public class GetProductMapper_Tests
    {
        private readonly GetProductMapper _mapper = new();

        // ── MapToDto ─────────────────────────────────────────────────────────────

        [Fact]
        public void MapToDto_Should_Use_BaseUnitPrice_When_Product_Has_No_Variants()
        {
            // Arrange
            var product = ProductFakes.SimpleProduct(unitPrice: 150_000m);

            // Act
            var dto = _mapper.MapToDto(product);

            // Assert
            dto.UnitPrice.ShouldBe(150_000m);
        }

        [Fact]
        public void MapToDto_Should_Use_MinVariantPrice_When_Product_Has_Variants()
        {
            // Arrange
            var product = ProductFakes.ProductWithVariants(prices: new[] { 200_000m, 180_000m, 220_000m });

            // Act
            var dto = _mapper.MapToDto(product);

            // Assert
            dto.UnitPrice.ShouldBe(180_000m);
        }

        [Fact]
        public void MapToDto_Should_Map_Images_In_SortOrder_Ascending()
        {
            // Arrange
            var product = ProductFakes.ProductWithImages(sortOrders: new[] { 3, 1, 2 });

            // Act
            var dto = _mapper.MapToDto(product);

            // Assert
            dto.Images.Select(i => i.SortOrder).ShouldBe(new[] { 1, 2, 3 });
        }

        [Fact]
        public void MapToDto_Should_Map_All_Scalar_Fields_Correctly()
        {
            // Arrange
            var product = ProductFakes.SimpleProduct();

            // Act
            var dto = _mapper.MapToDto(product);

            // Assert
            dto.Id.ShouldBe(product.Id);
            dto.ProductName.ShouldBe(product.ProductName);
            dto.Description.ShouldBe(product.Description);
            dto.Sku.ShouldBe(product.Sku);
            dto.TotalQuantity.ShouldBe(product.TotalQuantity);
            dto.TotalSell.ShouldBe(product.TotalSell);
            dto.IsDeleted.ShouldBe(product.IsDeleted);
        }

        [Fact]
        public void MapToDto_Should_Replace_Null_AltText_With_EmptyString()
        {
            // Arrange
            var product = ProductFakes.ProductWithImages(altText: null);

            // Act
            var dto = _mapper.MapToDto(product);

            // Assert
            dto.Images.ShouldAllBe(i => i.AltText == string.Empty);
        }

        // ── FromEntity ────────────────────────────────────────────────────────────

        [Fact]
        public void FromEntity_Should_Preserve_Pagination_Metadata()
        {
            // Arrange
            var products = new List<Product> { ProductFakes.SimpleProduct(), ProductFakes.SimpleProduct() };
            var paginationList = new PaginationList<Product>(products, count: 50, pageNumber: 3, pageSize: 2);

            // Act
            var result = _mapper.FromEntity(paginationList);

            // Assert
            result.TotalCount.ShouldBe(50);
            result.PageNumber.ShouldBe(3);
            result.PageSize.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // 2. QUERY EXTENSION / BUSINESS LOGIC TESTS
    // ─────────────────────────────────────────────────────────────────────────────

    public class GetProductQueryExtensions_StatusFilter_Tests
    {
        [Fact]
        public void ApplyStatusFilter_Should_Return_Only_Active_Products_When_Status_Is_Active()
        {
            // Arrange
            var products = new[]
            {
            ProductFakes.SimpleProduct(isDeleted: false),
            ProductFakes.SimpleProduct(isDeleted: true),
            ProductFakes.SimpleProduct(isDeleted: false),
        }.AsQueryable();

            // Act
            var result = products.ApplyStatusFilter(FilterStatus.Active).ToList();

            // Assert
            result.ShouldAllBe(p => !p.IsDeleted);
            result.Count.ShouldBe(2);
        }

        [Fact]
        public void ApplyStatusFilter_Should_Return_Only_Inactive_Products_When_Status_Is_Inactive()
        {
            // Arrange
            var products = new[]
            {
            ProductFakes.SimpleProduct(isDeleted: false),
            ProductFakes.SimpleProduct(isDeleted: true),
        }.AsQueryable();

            // Act
            var result = products.ApplyStatusFilter(FilterStatus.Inactive).ToList();

            // Assert
            result.ShouldAllBe(p => p.IsDeleted);
            result.Count.ShouldBe(1);
        }

        [Fact]
        public void ApplyStatusFilter_Should_Return_All_Products_When_Status_Is_All()
        {
            // Arrange
            var products = new[]
            {
            ProductFakes.SimpleProduct(isDeleted: false),
            ProductFakes.SimpleProduct(isDeleted: true),
        }.AsQueryable();

            // Act
            var result = products.ApplyStatusFilter(FilterStatus.All).ToList();

            // Assert
            result.Count.ShouldBe(2);
        }
    }

    public class GetProductQueryExtensions_PriceFilter_Tests
    {
        [Fact]
        public void ApplyPriceFilter_Should_Exclude_Products_Below_MinPrice_Without_Variants()
        {
            // Arrange
            var products = new[]
            {
            ProductFakes.SimpleProduct(unitPrice: 50_000m),
            ProductFakes.SimpleProduct(unitPrice: 100_000m),
            ProductFakes.SimpleProduct(unitPrice: 200_000m),
        }.AsQueryable();

            // Act
            var result = products.ApplyPriceFilter(minPrice: 100_000m, maxPrice: null).ToList();

            // Assert
            result.Count.ShouldBe(2);
            result.ShouldAllBe(p => p.UnitPrice >= 100_000m);
        }

        [Fact]
        public void ApplyPriceFilter_Should_Exclude_Products_Above_MaxPrice_Without_Variants()
        {
            // Arrange
            var products = new[]
            {
            ProductFakes.SimpleProduct(unitPrice: 50_000m),
            ProductFakes.SimpleProduct(unitPrice: 100_000m),
            ProductFakes.SimpleProduct(unitPrice: 200_000m),
        }.AsQueryable();

            // Act
            var result = products.ApplyPriceFilter(minPrice: null, maxPrice: 100_000m).ToList();

            // Assert
            result.Count.ShouldBe(2);
            result.ShouldAllBe(p => p.UnitPrice <= 100_000m);
        }

        [Fact]
        public void ApplyPriceFilter_Should_Filter_By_MinVariantPrice_For_Products_With_Variants()
        {
            // Arrange – variant prices: 80k, 120k → min = 80k (below 100k threshold)
            var belowMin = ProductFakes.ProductWithVariants(prices: new[] { 80_000m, 120_000m });
            // variant prices: 110k, 150k → min = 110k (above 100k threshold)
            var aboveMin = ProductFakes.ProductWithVariants(prices: new[] { 110_000m, 150_000m });

            var products = new[] { belowMin, aboveMin }.AsQueryable();

            // Act
            var result = products.ApplyPriceFilter(minPrice: 100_000m, maxPrice: null).ToList();

            // Assert
            result.Count.ShouldBe(1);
            result[0].ShouldBe(aboveMin);
        }

        [Fact]
        public void ApplyPriceFilter_Should_Return_All_Products_When_Both_Filters_Are_Null()
        {
            // Arrange
            var products = new[]
            {
            ProductFakes.SimpleProduct(unitPrice: 50_000m),
            ProductFakes.SimpleProduct(unitPrice: 500_000m),
        }.AsQueryable();

            // Act
            var result = products.ApplyPriceFilter(minPrice: null, maxPrice: null).ToList();

            // Assert
            result.Count.ShouldBe(2);
        }
    }

    public class GetProductQueryExtensions_SearchFilter_Tests
    {
        [Fact]
        public void ApplySearch_Should_Return_All_Products_When_Search_Is_Null()
        {
            // Arrange
            var products = new[]
            {
            ProductFakes.SimpleProduct(name: "Áo thun"),
            ProductFakes.SimpleProduct(name: "Quần jeans"),
        }.AsQueryable();

            // Act
            var result = products.ApplySearch(search: null).ToList();

            // Assert
            result.Count.ShouldBe(2);
        }

        [Fact]
        public void ApplySearch_Should_Match_ProductName_Case_Insensitively()
        {
            // Arrange
            var products = new[]
            {
            ProductFakes.SimpleProduct(name: "Áo Thun Nam"),
            ProductFakes.SimpleProduct(name: "Quần Jeans"),
        }.AsQueryable();

            // Act
            var result = products.ApplySearch(search: "áo thun").ToList();

            // Assert
            result.Count.ShouldBe(1);
            result[0].ProductName.ShouldBe("Áo Thun Nam");
        }

        [Fact]
        public void ApplySearch_Should_Match_Sku()
        {
            // Arrange
            var products = new[]
            {
            ProductFakes.SimpleProduct(sku: "SKU-001"),
            ProductFakes.SimpleProduct(sku: "SKU-999"),
        }.AsQueryable();

            // Act
            var result = products.ApplySearch(search: "SKU-001").ToList();

            // Assert
            result.Count.ShouldBe(1);
            result[0].Sku.ShouldBe("SKU-001");
        }

        [Fact]
        public void ApplySearch_Should_Return_Empty_When_No_Product_Matches()
        {
            // Arrange
            var products = new[]
            {
            ProductFakes.SimpleProduct(name: "Áo Thun"),
        }.AsQueryable();

            // Act
            var result = products.ApplySearch(search: "xyz_no_match").ToList();

            // Assert
            result.ShouldBeEmpty();
        }
    }

    public class GetProductQueryExtensions_SortFilter_Tests
    {
        [Fact]
        public void ApplySortFilter_Should_Sort_By_CreatedTime_Ascending_When_Oldest()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var products = new[]
            {
            ProductFakes.SimpleProduct(createdTime: now.AddDays(-1)),
            ProductFakes.SimpleProduct(createdTime: now.AddDays(-3)),
            ProductFakes.SimpleProduct(createdTime: now.AddDays(-2)),
        }.AsQueryable();

            // Act
            var result = products.ApplySortFilter(FilterSort.Oldest).ToList();

            // Assert
            result[0].CreatedTime.ShouldBe(now.AddDays(-3));
            result[2].CreatedTime.ShouldBe(now.AddDays(-1));
        }

        [Fact]
        public void ApplySortFilter_Should_Sort_By_CreatedTime_Descending_When_Newest()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var products = new[]
            {
            ProductFakes.SimpleProduct(createdTime: now.AddDays(-1)),
            ProductFakes.SimpleProduct(createdTime: now.AddDays(-3)),
        }.AsQueryable();

            // Act
            var result = products.ApplySortFilter(FilterSort.Newest).ToList();

            // Assert
            result[0].CreatedTime.ShouldBe(now.AddDays(-1));
        }

        [Fact]
        public void ApplySortFilter_Should_Sort_By_Name_Ascending_When_NameAsc()
        {
            // Arrange
            var products = new[]
            {
            ProductFakes.SimpleProduct(name: "Zebra"),
            ProductFakes.SimpleProduct(name: "Apple"),
            ProductFakes.SimpleProduct(name: "Mango"),
        }.AsQueryable();

            // Act
            var result = products.ApplySortFilter(FilterSort.NameAsc).ToList();

            // Assert
            result.Select(p => p.ProductName).ShouldBe(new[] { "Apple", "Mango", "Zebra" });
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // 3. ENDPOINT PRICE VALIDATION LOGIC TESTS
    // ─────────────────────────────────────────────────────────────────────────────

    public class GetProduct_PriceValidation_Tests
    {
        // Extracted helper that mirrors the endpoint validation logic
        private static List<ValidationFailure> ValidatePriceFilter(decimal? minPrice, decimal? maxPrice)
        {
            var errors = new List<ValidationFailure>();

            if (minPrice.HasValue || maxPrice.HasValue)
            {
                if (minPrice == 0 && maxPrice == 0)
                    errors.Add(new ValidationFailure("price_filter_error", "Vui lòng điền khoảng giá phù hợp"));
                else if (minPrice > maxPrice)
                    errors.Add(new ValidationFailure("price_filter_error", "Vui lòng điền khoảng giá phù hợp"));
            }

            return errors;
        }

        [Fact]
        public void Should_Have_No_Error_When_Price_Filter_Is_Not_Provided()
        {
            var errors = ValidatePriceFilter(minPrice: null, maxPrice: null);
            errors.ShouldBeEmpty();
        }

        [Fact]
        public void Should_Have_Error_When_Both_MinPrice_And_MaxPrice_Are_Zero()
        {
            var errors = ValidatePriceFilter(minPrice: 0, maxPrice: 0);

            errors.Count.ShouldBe(1);
            errors[0].PropertyName.ShouldBe("price_filter_error");
        }

        [Fact]
        public void Should_Have_Error_When_MinPrice_Is_Greater_Than_MaxPrice()
        {
            var errors = ValidatePriceFilter(minPrice: 500_000m, maxPrice: 100_000m);

            errors.Count.ShouldBe(1);
            errors[0].PropertyName.ShouldBe("price_filter_error");
        }

        [Fact]
        public void Should_Have_No_Error_When_MinPrice_Equals_MaxPrice_And_Not_Zero()
        {
            var errors = ValidatePriceFilter(minPrice: 100_000m, maxPrice: 100_000m);
            errors.ShouldBeEmpty();
        }

        [Fact]
        public void Should_Have_No_Error_When_Only_MinPrice_Is_Provided_And_Greater_Than_Zero()
        {
            var errors = ValidatePriceFilter(minPrice: 50_000m, maxPrice: null);
            errors.ShouldBeEmpty();
        }

        [Fact]
        public void Should_Have_No_Error_When_Only_MaxPrice_Is_Provided_And_Greater_Than_Zero()
        {
            var errors = ValidatePriceFilter(minPrice: null, maxPrice: 200_000m);
            errors.ShouldBeEmpty();
        }

        [Fact]
        public void Should_Have_No_Error_When_MinPrice_Is_Less_Than_MaxPrice()
        {
            var errors = ValidatePriceFilter(minPrice: 100_000m, maxPrice: 500_000m);
            errors.ShouldBeEmpty();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // 4. CATEGORY TREE TRAVERSAL TESTS (in-memory logic)
    // ─────────────────────────────────────────────────────────────────────────────

    public class GetProduct_CategoryFilter_InMemory_Tests
    {
        /// <summary>
        /// Mirrors the BFS category traversal logic from ApplyCategoryFilterAsync,
        /// extracted so it can be unit-tested without a real DbContext.
        /// </summary>
        private static List<Guid?> BuildCategoryIdTree(
            IEnumerable<(Guid Id, Guid? ParentId)> allCategories,
            Guid rootId)
        {
            var childrenLookup = allCategories
                .Where(c => c.ParentId.HasValue)
                .GroupBy(c => c.ParentId!.Value)
                .ToDictionary(g => g.Key, g => g.Select(c => c.Id).ToList());

            var result = new List<Guid?>();
            var queue = new Queue<Guid>();
            queue.Enqueue(rootId);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                result.Add(current);

                if (childrenLookup.TryGetValue(current, out var children))
                    foreach (var child in children)
                        queue.Enqueue(child);
            }

            return result;
        }

        [Fact]
        public void BuildCategoryTree_Should_Include_Root_When_It_Has_No_Children()
        {
            // Arrange
            var rootId = Guid.NewGuid();
            var categories = new[] { (rootId, (Guid?)null) };

            // Act
            var ids = BuildCategoryIdTree(categories, rootId);

            // Assert
            ids.ShouldContain(rootId);
            ids.Count.ShouldBe(1);
        }

        [Fact]
        public void BuildCategoryTree_Should_Include_Direct_Children_Of_Root()
        {
            // Arrange
            var rootId = Guid.NewGuid();
            var child1 = Guid.NewGuid();
            var child2 = Guid.NewGuid();

            var categories = new[]
            {
            (rootId, (Guid?)null),
            (child1, (Guid?)rootId),
            (child2, (Guid?)rootId),
        };

            // Act
            var ids = BuildCategoryIdTree(categories, rootId);

            // Assert
            ids.ShouldContain((Guid?)rootId);
            ids.ShouldContain((Guid?)child1);
            ids.ShouldContain((Guid?)child2);
            ids.Count.ShouldBe(3);
        }

        [Fact]
        public void BuildCategoryTree_Should_Include_Grandchildren_Via_BFS()
        {
            // Arrange  –  root → child → grandchild
            var rootId = Guid.NewGuid();
            var childId = Guid.NewGuid();
            var grandChildId = Guid.NewGuid();

            var categories = new[]
            {
            (rootId,       (Guid?)null),
            (childId,      (Guid?)rootId),
            (grandChildId, (Guid?)childId),
        };

            // Act
            var ids = BuildCategoryIdTree(categories, rootId);

            // Assert
            ids.ShouldContain((Guid?)grandChildId);
            ids.Count.ShouldBe(3);
        }

        [Fact]
        public void BuildCategoryTree_Should_Not_Include_Unrelated_Categories()
        {
            // Arrange
            var rootId = Guid.NewGuid();
            var unrelatedId = Guid.NewGuid();

            var categories = new[]
            {
            (rootId,      (Guid?)null),
            (unrelatedId, (Guid?)null),   // another root, not a child
        };

            // Act
            var ids = BuildCategoryIdTree(categories, rootId);

            // Assert
            ids.ShouldNotContain((Guid?)unrelatedId);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // FAKES / TEST DATA BUILDERS
    // ─────────────────────────────────────────────────────────────────────────────

    internal static class ProductFakes
    {
        public static Product SimpleProduct(
            decimal unitPrice = 100_000m,
            string name = "Test Product",
            string sku = "SKU-TEST",
            bool isDeleted = false,
            DateTime? createdTime = null)
        {
            return new Product
            {
                Id = Guid.NewGuid(),
                ProductName = name,
                Description = "Description",
                Sku = sku,
                UnitPrice = unitPrice,
                TotalQuantity = 10,
                TotalSell = 0,
                IsDeleted = isDeleted,
                CreatedTime = createdTime ?? DateTime.UtcNow,
                ProductVariants = new List<ProductVariant>(),
                ProductImages = new List<ProductImage>(),
            };
        }

        public static Product ProductWithVariants(decimal[] prices)
        {
            var product = SimpleProduct();
            product.ProductVariants = prices
                .Select(p => new ProductVariant { UnitPrice = p })
                .ToList();
            return product;
        }

        public static Product ProductWithImages(int[]? sortOrders = null, string? altText = "alt")
        {
            var product = SimpleProduct();
            var orders = sortOrders ?? new[] { 1 };

            product.ProductImages = orders
                .Select(order => new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = $"https://cdn.example.com/{order}.jpg",
                    AltText = altText,
                    SortOrder = order,
                    IsPrimary = order == orders.Min(),
                })
                .ToList();

            return product;
        }
    }
}

