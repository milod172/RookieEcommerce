using System;
using System.Collections.Generic;
using System.Text;

namespace NovaFashion.SharedViewModels.CategoryDtos
{
    public class CategoryDetailsDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ParentCategoryName { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
