using Octokit;
using OurRecipes.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OurRecipes.Business
{
    public class Client : IClient
    {
        public Client()
        {

        }

        public async Task<int> Main()
        {
            GitHubClient githubClient = new GitHubClient(new ProductHeaderValue("OurRecipes"))
            {
                Credentials = new Credentials("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")
            };

            IReadOnlyList<RepositoryContent> results;

            try
            {
                var user = await githubClient.User.Get("reZach");
                results = await githubClient.Repository.Content.GetAllContents("reZach", "OurRecipes", "Data/Entities/Measurements.json");
            }
            catch (Exception ex)
            {

                
            }

            return 0;
        }
    }
}
