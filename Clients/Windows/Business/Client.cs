using Newtonsoft.Json;
using Octokit;
using OurRecipes.Business.Interfaces;
using OurRecipes.Models;
using OurRecipes.Models.API;
using OurRecipes.Models.API.IFSAC;
using OurRecipes.Models.IFSAC;
using System;
using System.Collections.Generic;
using System.IO;
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

        private readonly string _localDataFile = "OurRecipesLocalData";

        // Data objects
        private IReadOnlyList<FoodGroup> _foodGroups;
        private IReadOnlyList<FoodMajorType> _foodMajorTypes;
        private IReadOnlyList<FoodSubType> _foodSubTypes;
        private IReadOnlyList<FoodSubTypeVariety> _foodSubTypeVarieties;
        private IReadOnlyList<FoodProcessingType> _foodProcessingTypes;

        private IReadOnlyList<Models.Language> _languages;
        private IReadOnlyList<Unit> _units;
        private IReadOnlyList<Measurement> _measurements;
        private IReadOnlyList<Time> _times;

        public Client()
        {
            _githubClient = new GitHubClient(new ProductHeaderValue("OurRecipes"))
            {
                Credentials = new Credentials("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")
            };
        }

        public async Task<bool> PullLatestFromGithub()
        {
            IReadOnlyList<GitHubCommit> commits = await _githubClient.Repository.Commit.GetAll(_author, _repo);
            LocalDataFile localDataFile = GetLocalDataFile();

            // Update the local data file
            if (localDataFile == null ||
                string.IsNullOrEmpty(localDataFile.LatestCommit))
            {
                // Pull from API
                FoodGroupAPIContainer foodGroupAPI = await RetrieveContainerFromAPI<FoodGroupAPIContainer>("FoodGroups");
                FoodMajorTypeAPIContainer foodMajorTypeAPI = await RetrieveContainerFromAPI<FoodMajorTypeAPIContainer>("FoodMajorTypes");
                FoodProcessingTypeAPIContainer foodProcessingTypeAPI = await RetrieveContainerFromAPI<FoodProcessingTypeAPIContainer>("FoodProcessingTypes");
                FoodSubTypeAPIContainer foodSubTypeAPI = await RetrieveContainerFromAPI<FoodSubTypeAPIContainer>("FoodSubTypes");
                FoodSubTypeVarietyAPIContainer foodSubTypeVarietyAPI = await RetrieveContainerFromAPI<FoodSubTypeVarietyAPIContainer>("FoodSubTypeVarieties");

                LanguageAPIContainer languageAPI = await RetrieveContainerFromAPI<LanguageAPIContainer>("Languages");
                MeasurementAPIContainer measurementAPI = await RetrieveContainerFromAPI<MeasurementAPIContainer>("Measurements");
                TimeAPIContainer timeAPI = await RetrieveContainerFromAPI<TimeAPIContainer>("Times");
                UnitAPIContainer unitAPI = await RetrieveContainerFromAPI<UnitAPIContainer>("Units");

                // Cast
                _foodGroups = foodGroupAPI.FoodGroups.Select(fg => new FoodGroup
                {
                    Id = fg.Id,
                    Name = fg.Name
                }).ToList();
                _foodMajorTypes = foodMajorTypeAPI.FoodMajorTypes.Select(fmt => new FoodMajorType
                {
                    Id = fmt.Id,
                    FoodGroup = _foodGroups.First(fg => fg.Id == fmt.FoodGroupId),
                    Name = fmt.Name
                }).ToList();
                _foodSubTypes = foodSubTypeAPI.FoodSubTypes.Select(fst => new FoodSubType
                {
                    Id = fst.Id,
                    FoodMajorType = _foodMajorTypes.First(fmt => fmt.Id == fst.FoodMajorTypeId),
                    Name = fst.Name
                }).ToList();
                _foodSubTypeVarieties = foodSubTypeVarietyAPI.FoodSubTypeVarieties.Select(fstv => new FoodSubTypeVariety
                {
                    Id = fstv.Id,
                    FoodSubType = _foodSubTypes.FirstOrDefault(fst => fst.Id == fstv.FoodSubTypeId),
                    Name = fstv.Name
                }).ToList();
                _foodProcessingTypes = foodProcessingTypeAPI.FoodProcessingTypes.Select(fpt => new FoodProcessingType
                {
                    Id = fpt.Id,
                    FoodSubType = _foodSubTypes.FirstOrDefault(fst => fst.Id == fpt.FoodSubTypeId),
                    FoodSubTypeVariety = _foodSubTypeVarieties.FirstOrDefault(fstv => fstv.Id == fpt.FoodSubTypeVarieteyId),
                    Name = fpt.Name
                }).ToList();

                _languages = languageAPI.Languages.Select(l => new Models.Language
                {
                    Id = l.Id,
                    Code = l.Code,
                    Name = l.Name
                }).ToList();
                _units = unitAPI.Units.Select(u => new Unit
                {
                    Id = u.Id,
                    Language = _languages.First(l => l.Id == u.LanguageId),
                    MetricSystem = u.MetricSystem,
                    Singular = u.Singular,
                    Plural = u.Plural,
                    AbbreviationSingle = u.AbbreviationSingle,
                    AbbreviationPlural = u.AbbreviationPlural
                }).ToList();
                _measurements = measurementAPI.Measurements.Select(m => new Measurement
                {
                    Id = m.Id,
                    Unit = _units.First(u => u.Id == m.UnitId),
                    Amount = m.Amount,
                    MaxAmount = m.MaxAmount
                }).ToList();
                _times = timeAPI.Times.Select(t => new Time
                {
                    Id = t.Id,
                    Days = t.Days,
                    Hours = t.Hours,
                    Minutes = t.Minutes,
                    Seconds = t.Seconds
                }).ToList();

                // Assign parents
                foreach (FoodProcessingTypeAPI processingTypeAPI in foodProcessingTypeAPI.FoodProcessingTypes.Where(f => f.ParentFoodProcessingTypeId.HasValue))
                {
                    _foodProcessingTypes.First(fpt => fpt.Id == processingTypeAPI.Id).ParentFoodProcessingType = _foodProcessingTypes.First(fpt2 => fpt2.Id == processingTypeAPI.ParentFoodProcessingTypeId.Value);
                }
                foreach (FoodSubTypeVarietyAPI subTypeVarietyAPI in foodSubTypeVarietyAPI.FoodSubTypeVarieties.Where(f => f.ParentFoodSubTypeVarietyId.HasValue))
                {
                    _foodSubTypeVarieties.First(fpt => fpt.Id == subTypeVarietyAPI.Id).ParentFoodSubTypeVariety = _foodSubTypeVarieties.First(fpt2 => fpt2.Id == subTypeVarietyAPI.ParentFoodSubTypeVarietyId.Value);
                }

                localDataFile = new LocalDataFile
                {
                    LatestCommit = commits.ElementAt(0).Commit.Sha,
                    FoodGroups = _foodGroups.ToList(),
                    FoodMajorTypes = _foodMajorTypes.ToList(),
                    FoodProcessingTypes = _foodProcessingTypes.ToList(),
                    FoodSubTypes = _foodSubTypes.ToList(),
                    FoodSubTypeVarieties = _foodSubTypeVarieties.ToList()
                };

                WriteLocalDataFile(localDataFile);
            }

            return true;
        }

        public async Task<int> Main()
        {
            // pull raw
            FoodGroupAPIContainer foodGroupAPI = await RetrieveContainerFromAPI<FoodGroupAPIContainer>("FoodGroups");
            FoodMajorTypeAPIContainer foodMajorTypeAPI = await RetrieveContainerFromAPI<FoodMajorTypeAPIContainer>("FoodMajorTypes");
            FoodProcessingTypeAPIContainer foodProcessingTypeAPI = await RetrieveContainerFromAPI<FoodProcessingTypeAPIContainer>("FoodProcessingTypes");
            FoodSubTypeAPIContainer foodSubTypeAPI = await RetrieveContainerFromAPI<FoodSubTypeAPIContainer>("FoodSubTypes");
            FoodSubTypeVarietyAPIContainer foodSubTypeVarietyAPI = await RetrieveContainerFromAPI<FoodSubTypeVarietyAPIContainer>("FoodSubTypeVarieties");

            LanguageAPIContainer languageAPI = await RetrieveContainerFromAPI<LanguageAPIContainer>("Languages");
            MeasurementAPIContainer measurementAPI = await RetrieveContainerFromAPI<MeasurementAPIContainer>("Measurements");
            TimeAPIContainer timeAPI = await RetrieveContainerFromAPI<TimeAPIContainer>("Times");
            UnitAPIContainer unitAPI = await RetrieveContainerFromAPI<UnitAPIContainer>("Units");

            // convert
            _foodGroups = foodGroupAPI.FoodGroups.Select(fg => new FoodGroup
            {
                Id = fg.Id,
                Name = fg.Name
            }).ToList();
            IReadOnlyList<FoodMajorType> foodMajorTypes = foodMajorTypeAPI.FoodMajorTypes.Select(fmt => new FoodMajorType
            {
                Id = fmt.Id,
                FoodGroup = _foodGroups.First(fg => fg.Id == fmt.FoodGroupId),
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
        /// Converts data from the api into a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<T> RetrieveContainerFromAPI<T>(string model)
        {
            IReadOnlyList<RepositoryContent> fromRepo = await RetrieveFromAPI(model);

            if (fromRepo == null || fromRepo.Count == 0)
                return default;

            return JsonConvert.DeserializeObject<T>(fromRepo.ElementAt(0).Content);
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

        #region Local data access

        /// <summary>
        /// Writes <paramref name="localDataFile"/> to file.
        /// </summary>
        /// <param name="localDataFile"></param>
        private void WriteLocalDataFile(LocalDataFile localDataFile)
        {
            string localFile = $"{_localDataFile}.json";

            string raw = JsonConvert.SerializeObject(localDataFile);

            File.WriteAllText(localFile, raw);
        }

        /// <summary>
        /// Retrieves a <see cref="LocalDataFile"/> from file.
        /// </summary>
        /// <returns></returns>
        private LocalDataFile GetLocalDataFile()
        {
            string localFile = $"{_localDataFile}.json";

            if (!File.Exists(localFile))
                return null;

            string raw = File.ReadAllText(localFile);

            return JsonConvert.DeserializeObject<LocalDataFile>(raw);
        }
        #endregion
    }
}
