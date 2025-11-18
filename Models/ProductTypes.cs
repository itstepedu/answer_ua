

namespace AnswerUA.Models {
    public class ProductTypes
    {
        public int Id { get; set; }
        public string Name { get; set; }

      

        public ICollection<Subcategories> Subcategories { get; set; }

        public ICollection<TargetCategoryProductType> TargetCategoryProductType { get; set; }
    }
}
