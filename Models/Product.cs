

namespace AnswerUA.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int BrandsId { get; set; }
        public Brands Brands { get; set; }

        public int SubcategoriesId { get; set; }
        public Subcategories Subcategories { get; set; }
        public int ProductTypesId { get; set; }

        public ProductTypes ProductTypes { get; set; }

        public int TargetCategoriesId { get; set; }

        public TargetCategories TargetCategories { get; set; }

        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }

        public string? Size { get; set; }
        public string? Color { get; set; }
        public ICollection<ProductImages> Images { get; set; }
    }
}
