using System.ComponentModel.DataAnnotations;

namespace eCommerce.Model.Requests;

public class CategoriesInsertRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int? ParentCategoryId { get; set; }

    public bool IsActive { get; set; } = true;
}