using System.Globalization;
using System.Text;
using NovaFashion.API.Entities;

namespace NovaFashion.API.Shared.Extensions
{
    public static class SkuGeneratorExtension
    {
        public static string GenerateSku(this Product product)
        {
            var nameParts = product.ProductName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var skuParts = nameParts.Select(part =>
            {
                var noDiacritics = RemoveDiacritics(part);


                if (!noDiacritics.Any(char.IsLetter))
                    return noDiacritics;

                return noDiacritics.ToUpperInvariant();
            });

            var generatedSku = string.Join("-", skuParts);
            return generatedSku;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
