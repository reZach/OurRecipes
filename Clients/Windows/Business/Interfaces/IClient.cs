using System.Threading.Tasks;

namespace OurRecipes.Business.Interfaces
{
    public interface IClient
    {
        Task<bool> PullLatestFromGithub();
        Task<int> Main();
    }
}
