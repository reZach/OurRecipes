namespace OurRecipes.Models.API.IFSAC
{
    public class FoodProcessingTypeAPI
    {
        public int Id { get; set; }
        public int? FoodSubTypeId { get; set; }
        public int? FoodSubTypeVarieteyId { get; set; }
        public int? ParentFoodProcessingTypeId { get; set; }
        public string Name { get; set; }
    }
}
