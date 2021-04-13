namespace OurRecipes.Models.IFSAC
{
    public class FoodSubTypeVariety
    {
        public int Id { get; set; }
        public FoodSubType FoodSubType { get; set; }
        public FoodSubTypeVariety ParentFoodSubTypeVariety { get; set; }
        public string Name { get; set; }
    }
}
