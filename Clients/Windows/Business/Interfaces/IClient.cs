using System.Threading.Tasks;

namespace OurRecipes.Business.Interfaces
{
    public interface IClient
    {
        Task<int> Main();
    }
}
