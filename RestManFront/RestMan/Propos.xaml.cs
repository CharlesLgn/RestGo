using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using RestManDataAccess;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace RestMan
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Propos : Page
    {
        public Propos()
        {
            this.InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Bt_Cyril_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://www.linkedin.com/in/cyril-challouatte-824021160/");
            Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private void Bt_Charles_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://www.linkedin.com/in/charles-ligony-893177134/");
            Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private void Sujet_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://drive.google.com/file/d/1d78jWWdqbD4-om1J4g6c1Xv7Xp2LF9DJ/view?usp=sharing");
            Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            ResourceLoader resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            lb_propos.Text = resourceLoader.GetString("Propos");
            lb_titreSujet.Text = resourceLoader.GetString("TitreSujet");
            lb_IntroSujet.Text = resourceLoader.GetString("IntroSujet");
            lb_sujet.Text = resourceLoader.GetString("Sujet");
            lb_btsujet.Text = resourceLoader.GetString("btSujet");
            lb_indication.Text = resourceLoader.GetString("Indication");
            lb_indicationcontenu.Text = resourceLoader.GetString("IndicationContenu");
            lb_livrable.Text = resourceLoader.GetString("Livrables");
        }

        /// <summary>
        /// Supprime toutes les données de l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteConfirmation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataAccess.DropDatabase();
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("Intitulé de l'erreur : \n" + ex.Message) { Title = "Erreur lors de l'affichage la page" };
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                var res = dialog.ShowAsync();
            }

        }
    }
}
