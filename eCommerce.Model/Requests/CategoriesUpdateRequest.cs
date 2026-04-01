namespace eCommerce.Model.Requests
{
    public class CategoriesUpdateRequest
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public int? ParentCategoryId { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
