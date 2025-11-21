

namespace AnswerUA.Models
{
    public class TargetCategories
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<TargetCategoryProductType> TargetCategoryProductType { get; set; }

    }
}
