
namespace AnswerUA.Models
{
    public class Subcategories
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int TargetCategoriesId { get; set; }
        public TargetCategories TargetCategories { get; set; }

        // public ICollection<Subcategories> Subcategories { get; set; }

        public int ProductTypesId { get; set; }
        public ProductTypes ProductTypes { get; set; }

        public ICollection<Product> Product { get; set; }

        public int? GenderId { get; set; }
        public Gender Genders { get; set; }
    }
}