namespace OurRecipes.Models.IFSAC
{
    public class FoodSubType
    {
        public int Id { get; set; }
        public FoodMajorType FoodMajorType { get; set; }
        public string Name { get; set; }
    }
}
