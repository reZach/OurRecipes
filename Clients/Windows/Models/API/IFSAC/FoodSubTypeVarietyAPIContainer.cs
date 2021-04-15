using System.Collections.Generic;

namespace OurRecipes.Models.API.IFSAC
{
    public class FoodSubTypeVarietyAPIContainer
    {
        public List<FoodSubTypeVarietyAPI> FoodSubTypeVarieties { get; set; }

        public FoodSubTypeVarietyAPIContainer()
        {
            FoodSubTypeVarieties = new List<FoodSubTypeVarietyAPI>();
        }
    }
}
