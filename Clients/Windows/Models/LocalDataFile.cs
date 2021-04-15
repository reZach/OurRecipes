using OurRecipes.Models.API.IFSAC;

namespace OurRecipes.Models
{
    public class LocalDataFile
    {
        public string LatestCommit { get; set; }

        public FoodGroupAPIContainer FoodGroupAPIContainer { get; set; }
        public FoodMajorTypeAPIContainer FoodMajorTypeAPIContainer { get; set; }
        public FoodProcessingTypeAPIContainer FoodProcessingTypeAPIContainer { get; set; }
        public FoodSubTypeAPIContainer FoodSubTypeAPIContainer { get; set; }
        public FoodSubTypeVarietyAPIContainer FoodSubTypeVarietyAPIContainer { get; set; }

        public LocalDataFile()
        {
            FoodGroupAPIContainer = new FoodGroupAPIContainer();
            FoodMajorTypeAPIContainer = new FoodMajorTypeAPIContainer();
            FoodProcessingTypeAPIContainer = new FoodProcessingTypeAPIContainer();
            FoodSubTypeAPIContainer = new FoodSubTypeAPIContainer();
            FoodSubTypeVarietyAPIContainer = new FoodSubTypeVarietyAPIContainer();
        }
    }
}
