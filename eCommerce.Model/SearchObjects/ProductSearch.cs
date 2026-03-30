namespace eCommerce.Model.SearchObjects;

public class ProductSearchObject : BaseSearchObject
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? ProductTypeId { get; set; }
}