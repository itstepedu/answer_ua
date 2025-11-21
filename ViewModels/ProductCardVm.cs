using AnswerUA.Models;

namespace AnswerUA.ViewModels
{
    public class ProductCardVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brands { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string? OldPriceStr { get; set; }   // якщо треба покажеш стару ціну
        public string? Color {get; set;}
        public string? Size {get; set;}
        //public List<string> Size { get; set; } = new();
    }

    public class ProductFiltersVm
    {
        public int TargetId { get; set; }         // 1..4
        public int ProductTypesId { get; set; }    // 2=Одяг, 3=Взуття, 4=Аксесуари, ...

        public List<int>? SubcategoriesId { get; set; } = new();
        public List<int>? BrandsId { get; set; }
        public List<string>? Color { get; set; }
        public List<string>? Size { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public string? Sort { get; set; }         // popular|price_asc|price_desc...
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 24;
    }

    public class CatalogVm
    {
        public int TargetId { get; set; }
        public int ProductTypesId { get; set; }
        public List<Subcategories> Subcategories { get; set; } = new();
        public List<(int Id, string Name)> AllowedProductTypes { get; set; } = new();
        public List<ProductCardVm> Products { get; set; } = new();
    }
}
