using Newtonsoft.Json;
using Octokit;
using OurRecipes.Business.Interfaces;
using OurRecipes.Models;
using OurRecipes.Models.API;
using OurRecipes.Models.API.IFSAC;
using OurRecipes.Models.IFSAC;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OurRecipes.Business
{
    public class Client : IClient
    {
        private readonly string _author = "reZach";
        private readonly string _repo = "OurRecipes";
        private readonly string _entitiesPath = "Data/Entities/";
        private readonly GitHubClient _githubClient;

        public Client()
        {
            _githubClient = new GitHubClient(new ProductHeaderValue("OurRecipes"))
            {
                Credentials = new Credentials("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")
            };
        }

        public async Task<int> Main()
        {
            // pull raw
            IReadOnlyList<RepositoryContent> foodGroupsRepo = await RetrieveFromAPI("FoodGroup");
            IReadOnlyList<RepositoryContent> foodMajorTypesRepo = await RetrieveFromAPI("FoodMajorType");
            IReadOnlyList<RepositoryContent> foodProcessingTypesRepo = await RetrieveFromAPI("FoodProcessingType");
            IReadOnlyList<RepositoryContent> foodSubTypesRepo = await RetrieveFromAPI("FoodSubType");
            IReadOnlyList<RepositoryContent> foodSubTypeVarietiesRepo = await RetrieveFromAPI("FoodSubTypeVariety");

            IReadOnlyList<RepositoryContent> languagesRepo = await RetrieveFromAPI("Languages");
            IReadOnlyList<RepositoryContent> measurementsRepo = await RetrieveFromAPI("Measurements");
            IReadOnlyList<RepositoryContent> timesRepo = await RetrieveFromAPI("Times");
            IReadOnlyList<RepositoryContent> unitsRepo = await RetrieveFromAPI("Units");

            // cast
            FoodGroupAPIContainer foodGroupAPI = JsonConvert.DeserializeObject<FoodGroupAPIContainer>(foodGroupsRepo.ElementAt(0).Content);
            FoodMajorTypeAPIContainer foodMajorTypeAPI = JsonConvert.DeserializeObject<FoodMajorTypeAPIContainer>(foodMajorTypesRepo.ElementAt(0).Content);
            FoodProcessingTypeAPIContainer foodProcessingTypeAPI = JsonConvert.DeserializeObject<FoodProcessingTypeAPIContainer>(foodProcessingTypesRepo.ElementAt(0).Content);
            FoodSubTypeAPIContainer foodSubTypeAPI = JsonConvert.DeserializeObject<FoodSubTypeAPIContainer>(foodSubTypesRepo.ElementAt(0).Content);
            FoodSubTypeVarietyAPIContainer foodSubTypeVarietyAPI = JsonConvert.DeserializeObject<FoodSubTypeVarietyAPIContainer>(foodSubTypeVarietiesRepo.ElementAt(0).Content);

            LanguageAPIContainer languageAPI = JsonConvert.DeserializeObject<LanguageAPIContainer>(languagesRepo.ElementAt(0).Content);
            MeasurementAPIContainer measurementAPI = JsonConvert.DeserializeObject<MeasurementAPIContainer>(measurementsRepo.ElementAt(0).Content);
            TimeAPIContainer timeAPI = JsonConvert.DeserializeObject<TimeAPIContainer>(timesRepo.ElementAt(0).Content);
            UnitAPIContainer unitAPI = JsonConvert.DeserializeObject<UnitAPIContainer>(unitsRepo.ElementAt(0).Content);

            // convert
            IReadOnlyList<FoodGroup> foodGroups = foodGroupAPI.FoodGroups.Select(fg => new FoodGroup
            {
                Id = fg.Id,
                Name = fg.Name
            }).ToList();
            IReadOnlyList<FoodMajorType> foodMajorTypes = foodMajorTypeAPI.FoodMajorTypes.Select(fmt => new FoodMajorType
            {
                Id = fmt.Id,
                FoodGroup = foodGroups.First(fg => fg.Id == fmt.FoodGroupId),
                Name = fmt.Name
            }).ToList();
            IReadOnlyList<FoodSubType> foodSubTypes = foodSubTypeAPI.FoodSubTypes.Select(fst => new FoodSubType
            {
                Id = fst.Id,
                FoodMajorType = foodMajorTypes.First(fmt => fmt.Id == fst.FoodMajorTypeId),
                Name = fst.Name
            }).ToList();
            IReadOnlyCollection<FoodSubTypeVariety> foodSubTypeVarieties = foodSubTypeVarietyAPI.FoodSubTypeVarieties.Select(fstv => new FoodSubTypeVariety
            {
                Id = fstv.Id,
                FoodSubType = foodSubTypes.FirstOrDefault(fst => fst.Id == fstv.FoodSubTypeId),
                Name = fstv.Name
            }).ToList();
            IReadOnlyCollection<FoodProcessingType> foodProcessingTypes = foodProcessingTypeAPI.FoodProcessingTypes.Select(fpt => new FoodProcessingType
            {
                Id = fpt.Id,
                FoodSubType = foodSubTypes.FirstOrDefault(fst => fst.Id == fpt.FoodSubTypeId),
                FoodSubTypeVariety = foodSubTypeVarieties.FirstOrDefault(fstv => fstv.Id == fpt.FoodSubTypeVarieteyId),
                Name = fpt.Name
            }).ToList();

            IReadOnlyList<Models.Language> languages = languageAPI.Languages.Select(l => new Models.Language
            {
                Id = l.Id,
                Code = l.Code,
                Name = l.Name
            }).ToList();
            IReadOnlyList<Unit> units = unitAPI.Units.Select(u => new Unit
            {
                Id = u.Id,
                Language = languages.First(l => l.Id == u.LanguageId),
                MetricSystem = u.MetricSystem,
                Singular = u.Singular,
                Plural = u.Plural,
                AbbreviationSingle = u.AbbreviationSingle,
                AbbreviationPlural = u.AbbreviationPlural
            }).ToList();
            IReadOnlyList<Measurement> measurements = measurementAPI.Measurements.Select(m => new Measurement
            {
                Id = m.Id,
                Unit = units.First(u => u.Id == m.UnitId),
                Amount = m.Amount,
                MaxAmount = m.MaxAmount
            }).ToList();
            IReadOnlyList<Time> times = timeAPI.Times.Select(t => new Time
            {
                Id = t.Id,
                Days = t.Days,
                Hours = t.Hours,
                Minutes = t.Minutes,
                Seconds = t.Seconds
            }).ToList();

            // assign parents
            foreach (FoodProcessingTypeAPI processingTypeAPI in foodProcessingTypeAPI.FoodProcessingTypes.Where(f => f.ParentFoodProcessingTypeId.HasValue))
            {
                foodProcessingTypes.First(fpt => fpt.Id == processingTypeAPI.Id).ParentFoodProcessingType = foodProcessingTypes.First(fpt2 => fpt2.Id == processingTypeAPI.ParentFoodProcessingTypeId.Value);
            }
            foreach (FoodSubTypeVarietyAPI subTypeVarietyAPI in foodSubTypeVarietyAPI.FoodSubTypeVarieties.Where(f => f.ParentFoodSubTypeVarietyId.HasValue))
            {
                foodSubTypeVarieties.First(fpt => fpt.Id == subTypeVarietyAPI.Id).ParentFoodSubTypeVariety = foodSubTypeVarieties.First(fpt2 => fpt2.Id == subTypeVarietyAPI.ParentFoodSubTypeVarietyId.Value);
            }

            return 0;
        }

        /// <summary>
        /// Queries the github api for the repository contents of a given <paramref name="filename"/> at <see cref="_entitiesPath"/>.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private async Task<IReadOnlyList<RepositoryContent>> RetrieveFromAPI(string filename)
        {
            return await _githubClient.Repository.Content.GetAllContents(_author, _repo, $"{_entitiesPath}{filename}.json");
        }
    }
}
