namespace OurRecipes.Models.IFSAC
{
    public class FoodProcessingType
    {
        public int Id { get; set; }
        public FoodSubType FoodSubType { get; set; }
        public FoodSubTypeVariety FoodSubTypeVariety { get; set; }
        public FoodProcessingType ParentFoodProcessingType { get; set; }
        public string Name { get; set; }
    }
}
