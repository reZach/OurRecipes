using System.Collections.Generic;

namespace OurRecipes.Models.API.IFSAC
{
    public class FoodSubTypeAPIContainer
    {
        public List<FoodSubTypeAPI> FoodSubTypes { get; set; }

        public FoodSubTypeAPIContainer()
        {
            FoodSubTypes = new List<FoodSubTypeAPI>();
        }
    }
}
