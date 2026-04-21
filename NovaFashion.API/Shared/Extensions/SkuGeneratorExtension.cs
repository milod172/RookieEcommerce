
using System.Globalization;
using System.Text;
using NovaFashion.API.Entities;

namespace NovaFashion.API.Shared.Extensions
{
    public static class SkuGeneratorExtension
    {
        // BANANA REPUBLIC Jean --> BRJ - 6 chars from guid
        public static string GenerateSku(this Product product)
        {
            var nameParts = product.ProductName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var prefix = string.Concat(
                nameParts.Select(part => RemoveDiacritics(part)[..1].ToUpperInvariant())
            );

            var guidPart = product.Id.ToString("N")[..6].ToUpperInvariant();

            return $"{prefix}-{guidPart}";
        }

        public static string GenerateVariantSku(this ProductVariant variant, string productName, Guid productId)
        {
            var nameParts = productName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var prefix = string.Concat(
                nameParts.Select(part => RemoveDiacritics(part)[..1].ToUpperInvariant())
            );

            var guidPart = productId.ToString("N")[..6].ToUpperInvariant();
            var sizePart = variant.Size.ToString().ToUpperInvariant();

            return $"{prefix}-{guidPart}-{sizePart}";
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
