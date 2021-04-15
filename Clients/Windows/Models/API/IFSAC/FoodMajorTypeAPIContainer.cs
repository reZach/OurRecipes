using System.Collections.Generic;

namespace OurRecipes.Models.API.IFSAC
{
    public class FoodMajorTypeAPIContainer
    {
        public List<FoodMajorTypeAPI> FoodMajorTypes { get; set; }

        public FoodMajorTypeAPIContainer()
        {
            FoodMajorTypes = new List<FoodMajorTypeAPI>();
        }
    }
}
