using System.Collections.Generic;

namespace OurRecipes.Models.API.IFSAC
{
    public class FoodProcessingTypeAPIContainer
    {
        public List<FoodProcessingTypeAPI> FoodProcessingTypes { get; set; }

        public FoodProcessingTypeAPIContainer()
        {
            FoodProcessingTypes = new List<FoodProcessingTypeAPI>();
        }
    }
}
