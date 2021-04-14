using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OurRecipes.Business;
using OurRecipes.Business.Interfaces;
using System.Windows;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;

        public App()
        {
            host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IClient, Client>();
                })
                .Build();
        }
    }
}
