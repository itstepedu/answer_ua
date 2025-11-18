namespace AnswerUA.Models
{
    public class TargetCategoryProductType
    {
        public int TargetCategoriesId { get; set; }
        public TargetCategories TargetCategories { get; set; }

        public int ProductTypesId { get; set; }
        public ProductTypes ProductTypes { get; set; }
    }
}
