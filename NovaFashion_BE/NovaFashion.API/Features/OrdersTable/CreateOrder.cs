using System.Security.Claims;
using System.Text.Json.Serialization;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.CartDtos;
using NovaFashion.SharedViewModels.OrderDtos;

namespace NovaFashion.API.Features.OrdersTable
{
    public class CreateOrderRequest
    {
        public List<CartItemRequest> Items { get; set; } = [];

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentMethod PaymentMethod { get; set; }
    }

    public class CreateOrderValidator : Validator<CreateOrderRequest>
    {
        public const string FirstNameRequired = "Họ không được để trống";
        public const string FirstNameTooLong = "Họ không được vượt quá 100 ký tự";
        public const string LastNameRequired = "Tên không được để trống";
        public const string LastNameTooLong = "Tên không được vượt quá 100 ký tự";
        public const string PhoneNumberRequired = "Số điện thoại không được để trống";
        public const string AddressRequired = "Địa chỉ không được để trống";
        public const string AddressTooLong = "Địa chỉ không được vượt quá 225 ký tự";
        public const string PaymentMethodInvalid = "Phương thức thanh toán phải là 'COD' hoặc 'VNPay'";

        public CreateOrderValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(FirstNameRequired)
                .MaximumLength(100)
                .WithMessage(FirstNameTooLong);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(LastNameRequired)
                .MaximumLength(100)
                .WithMessage(LastNameTooLong);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage(PhoneNumberRequired);
          

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage(AddressRequired)
                .MaximumLength(225)
                .WithMessage(AddressTooLong);

            RuleFor(x => x.PaymentMethod)
                .IsInEnum()
                .WithMessage(PaymentMethodInvalid);
        }
    }

    public class CreateOrderMapper : Mapper<CreateOrderRequest, OrderDetailsDto, Orders>
    {

        public override OrderDetailsDto FromEntity(Orders e)
        {
            return new OrderDetailsDto
            {
                Id = e.Id,
                FullName = $"{e.FirstName} {e.LastName}",
                PhoneNumber = e.PhoneNumber,
                Address = e.ShippingAddress,
                CreateTime = e.CreatedTime.ToString("dd/MM/yyyy HH:mm"),
                PaymentMethod = e.PaymentMethod.ToString(),
                OrderStatus = e.OrderStatus.ToString(),
                OrderItems = e.OrderItems.Select(oi => new CartItemDto
                {
                    ProductVariantId = oi.ProductVariantId,
                    ProductId = oi.ProductVariant!.ProductId,
                    ProductName = oi.ProductVariant!.Product!.ProductName,
                    ImageUrl = oi.ProductVariant!.Product!.ProductImages
                        .FirstOrDefault(x => x.IsPrimary)?.ImageUrl ?? string.Empty,
                    Size = oi.ProductVariant!.Size.ToString(),
                    Sku = oi.ProductVariant!.VariantSku,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.UnitPrice * oi.Quantity,
                }).ToList()
            };
        }
    }
    public class CreateOrder(AppDbContext db) : Endpoint<CreateOrderRequest, OrderDetailsDto, CreateOrderMapper>
    {
        public override void Configure()
        {
            Post("checkout");
            Roles(Role.Customer.ToString());
            Group<OrderGroup>();
            DontThrowIfValidationFails();
        }

        public override async Task HandleAsync(CreateOrderRequest req, CancellationToken ct)
        {
            var currentUserId = User.FindFirstValue("sub");

            if (currentUserId == null)
            {
                AddError("Vui lòng đăng nhập lại");
                ThrowIfAnyErrors();
            }

            if (req.Items is not { Count: > 0 })
            {
                AddError(r => r.Items, "Giỏ hàng không được để trống");
                ThrowIfAnyErrors();
            }

            // Load variants
            var variantIds = req.Items.Select(x => x.ProductVariantId).ToList();

            var variants = await db.ProductVariants
                .Include(x => x.Product)
                .ThenInclude(x => x.ProductImages)
                .Where(x => variantIds.Contains(x.Id))
                .ToListAsync(ct);

            var variantDict = variants.ToDictionary(x => x.Id);

            // Validate stock
            foreach (var item in req.Items)
            {
                if (!variantDict.TryGetValue(item.ProductVariantId, out var variant))
                {
                    AddError($"Sản phẩm {item.ProductVariantId} không tồn tại");
                    continue;
                }

                if (item.Quantity <= 0)
                {
                    AddError($"Số lượng không hợp lệ cho {variant.Product.ProductName}");
                }

                if (item.Quantity > variant.StockQuantity)
                {
                    AddError($"Sản phẩm {variant.Product.ProductName} chỉ còn {variant.StockQuantity}");
                }
            }

            ThrowIfAnyErrors();

            // Build OrderItems + total
            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var item in req.Items)
            {
                var variant = variantDict[item.ProductVariantId];

                var orderItem = new OrderItem
                {
                    ProductVariantId = variant.Id,
                    Quantity = item.Quantity,
                    UnitPrice = variant.UnitPrice
                };

                orderItems.Add(orderItem);
                totalAmount += orderItem.UnitPrice * orderItem.Quantity;
            }

            // Create Order
            var order = new Orders
            {
                FirstName = req.FirstName,
                LastName = req.LastName,
                PhoneNumber = req.PhoneNumber,
                ShippingAddress = req.Address,
                PaymentMethod = req.PaymentMethod,
                OrderStatus = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                TotalAmount = totalAmount,
                CustomerId = currentUserId,
                OrderItems = orderItems
            };

            db.Orders.Add(order);
            await db.SaveChangesAsync(ct);

            var result = await db.Orders
                .Where(x => x.Id == order.Id)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.ProductVariant)
                        .ThenInclude(x => x.Product)
                            .ThenInclude(x => x.ProductImages)
                .FirstAsync(ct);

            await Send.OkAsync(Map.FromEntity(result), cancellation: ct);
        }
    }
}
