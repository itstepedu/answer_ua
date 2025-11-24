namespace AnswerUA.ViewModels
{
    public class ProductDetailsVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brands { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Images { get; set; } = new();

        // public List<string>? Size { get; set; }

        public string TargetSlug { get; set; }
        public string ProductTypesSlug { get; set; }
        public string SubcategoriesName { get; set; }
    }
}
