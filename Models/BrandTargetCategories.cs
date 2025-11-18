

namespace AnswerUA.Models
{
    public class BrandTargetCategories
    {

        public int Id { get; set; }

        public int BrandsId { get; set; }
        public Brands Brands { get; set; }

        public int TargetCategoriesId { get; set; }
        public TargetCategories TargetCategories { get; set; }

    }
}
