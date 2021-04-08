using OurRecipes.Business;
using System;
using System.Threading.Tasks;

namespace OurRecipes.Driver
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Client client = new Client();
            await client.Main();

            return 0;
        }
    }
}
