

namespace AnswerUA.Models
{
    public class BrandTargetCategories
    {

        public int Id { get; set; }

        public int BrandsId { get; set; }
        public Brand Brands { get; set; }

        public int TargetCategoriesId { get; set; }
        public TargetCategories TargetCategories { get; set; }

    }
}
