namespace OurRecipes.Models.API.IFSAC
{
    public class FoodSubTypeVarietyAPI
    {
        public int Id { get; set; }
        public int? FoodSubTypeId { get; set; }
        public int? ParentFoodSubTypeVarietyId { get; set; }
        public string Name { get; set; }
    }
}
