using System;
using System.Collections.Generic;
using System.Text;

namespace NovaFashion.SharedViewModels.CategoryDtos
{
    public class CategoryPickerDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
    }
}
