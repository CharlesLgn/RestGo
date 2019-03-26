using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

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
            lb_auteurs.Text = resourceLoader.GetString("Auteurs");
            lb_projet.Text = resourceLoader.GetString("Projet");
            lb_titreSujet.Text = resourceLoader.GetString("TitreSujet");
            lb_IntroSujet.Text = resourceLoader.GetString("IntroSujet");
            lb_sujet.Text = resourceLoader.GetString("Sujet");
            lb_btsujet.Text = resourceLoader.GetString("btSujet");
            lb_indication.Text = resourceLoader.GetString("Indication");
            lb_indicationcontenu.Text = resourceLoader.GetString("IndicationContenu");
            lb_livrable.Text = resourceLoader.GetString("Livrables");
        }
    }
}
