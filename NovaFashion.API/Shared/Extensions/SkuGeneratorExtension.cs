
using NovaFashion.API.Entities;

namespace NovaFashion.API.Shared.Extensions
{
    public static class SkuGeneratorExtension
    {
        // Áo thun nam --> AO-TH-NA
        public static string GenerateSku(this Product product)
        {
            var nameParts = product.ProductName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var skuParts = nameParts.Select(part =>
                (part.Length >= 2 ? part[..2] : part).ToUpperInvariant()
            );

            var generatedSku = string.Join("-", skuParts);
            
            return generatedSku;
        }

        public static string GenerateVariantSku(this ProductVariant variant)
        {
            var productName = variant.Product?.ProductName ?? "SP";
           
            var nameParts = productName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var prefixParts = nameParts.Select(part =>
                (part.Length >= 2 ? part[..2] : part).ToUpperInvariant()
            );
            var prefix = string.Join("-", prefixParts);

            // Size from enum
            var sizePart = variant.Size.ToString().ToUpperInvariant();

            return $"{prefix}-{sizePart}";
        }
    }
}
