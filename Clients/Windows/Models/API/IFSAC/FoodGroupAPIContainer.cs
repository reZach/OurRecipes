using System.Collections.Generic;

namespace OurRecipes.Models.API.IFSAC
{
    public class FoodGroupAPIContainer
    {
        public List<FoodGroupAPI> FoodGroups { get; set; }

        public FoodGroupAPIContainer()
        {
            FoodGroups = new List<FoodGroupAPI>();
        }
    }
}
