using Octokit;
using OurRecipes.Business.Interfaces;
using System;
using System.Collections.Generic;

namespace OurRecipes.Business
{
    public class Client : IClient
    {
        public Client()
        {

        }

        public async void Main()
        {
            GitHubClient githubClient = new GitHubClient(new ProductHeaderValue("OurRecipes"))
            {
                Credentials = new Credentials("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")
            };

            IReadOnlyList<RepositoryContent> results;

            try
            {
                results = await githubClient.Repository.Content.GetAllContents("reZach", "OurRecipes", "Data/Entities/Measurements.json");
            }
            catch (Exception ex)
            {

                
            }
        }
    }
}
