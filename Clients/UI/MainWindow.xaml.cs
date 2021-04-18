using OurRecipes.Business;
using OurRecipes.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UI.Pages;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IClient _client;

        public MainWindow()
        {
            InitializeComponent();

            _client = new Client();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {

        }

        #region Menu
        private async void MenuUpdate_Click(object sender, RoutedEventArgs e)
        {
            await _client.PullLatestFromGithub();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuRecipeNew_Click(object sender, RoutedEventArgs e)
        {
            NewRecipe newRecipePage = new NewRecipe();
            Content = newRecipePage;
        }
        #endregion
    }
}
