

namespace AnswerUA.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<BrandTargetCategories> BrandTargetCategories { get; set; }

    }
}
