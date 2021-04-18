using OurRecipes.Models.IFSAC;
using System.Collections.Generic;

namespace OurRecipes.Models
{
    public class LocalDataFile
    {
        public string LatestCommit { get; set; }

        public List<FoodGroup> FoodGroups { get; set; }
        public List<FoodMajorType> FoodMajorTypes { get; set; }
        public List<FoodProcessingType> FoodProcessingTypes { get; set; }
        public List<FoodSubType> FoodSubTypes { get; set; }
        public List<FoodSubTypeVariety> FoodSubTypeVarieties { get; set; }

        public LocalDataFile()
        {
            FoodGroups = new List<FoodGroup>();
            FoodMajorTypes = new List<FoodMajorType>();
            FoodProcessingTypes = new List<FoodProcessingType>();
            FoodSubTypes = new List<FoodSubType>();
            FoodSubTypeVarieties = new List<FoodSubTypeVariety>();
        }
    }
}
