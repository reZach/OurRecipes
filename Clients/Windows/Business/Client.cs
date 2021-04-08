using Octokit;
using System;
using System.Collections.Generic;
using System.Text;

namespace OurRecipes.Business
{
    public class Client
    {
        public Client()
        {

        }

        public async void Get()
        {
            GitHubClient githubClient = new GitHubClient(new ProductHeaderValue("OurRecipes"));            
            githubClient.Credentials = new Credentials("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");

            await githubClient.Repository.Content.GetAllContents()
        }
    }
}
