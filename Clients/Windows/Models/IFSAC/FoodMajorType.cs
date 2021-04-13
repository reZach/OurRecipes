namespace OurRecipes.Models.IFSAC
{
    public class FoodMajorType
    {
        public int Id { get; set; }
        public FoodGroup FoodGroup { get; set; }
        public string Name { get; set; }
    }
}
