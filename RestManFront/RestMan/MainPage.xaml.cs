using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using RestManDataAccess;


// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RestMan
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var isDark = Application.Current.RequestedTheme == ApplicationTheme.Dark;

            if (isDark)
            {
                Logo.Source = new BitmapImage(new Uri("ms-appx:///Assets/RestGo_logo_white_text.png"));
            }
            else
            {
                Logo.Source = new BitmapImage(new Uri("ms-appx:///Assets/RestGo_logo_black_text.png")); 
            }


            hideRec();
        }

        private void hideRec()
        {
            Accueil.Padding = new Thickness(5, 5, 0, 5);
            Langue.Padding = new Thickness(5, 5, 0, 5);
            Propos.Padding = new Thickness(5, 5, 0, 5);
            RecAccueil.Visibility = Visibility.Collapsed;
            RecLangue.Visibility = Visibility.Collapsed;
            RecPropos.Visibility = Visibility.Collapsed;
        }

        private void Hamburger_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void Sousmenus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            hideRec();
            if (Accueil.IsSelected)
            {
                Accueil.Padding = new Thickness(0, 5, 0, 5);
                Langue.Padding = new Thickness(5, 5, 0, 5);
                Propos.Padding = new Thickness(5, 5, 0, 5);
                RecAccueil.Visibility = Visibility.Visible;
                MyFrame.Navigate(typeof(Home));
            }
            if (Langue.IsSelected)
            {
                Accueil.Padding = new Thickness(5, 5, 0, 5);
                Langue.Padding = new Thickness(0, 5, 0, 5);
                Propos.Padding = new Thickness(5, 5, 0, 5);
                RecLangue.Visibility = Visibility.Visible;
                MyFrame.Navigate(typeof(Langue));
            }
            if (Propos.IsSelected)
            {
                Accueil.Padding = new Thickness(5, 5, 0, 5);
                Langue.Padding = new Thickness(5, 5, 0, 5);
                Propos.Padding = new Thickness(0, 5, 0, 5);
                RecPropos.Visibility = Visibility.Visible;
                MyFrame.Navigate(typeof(Propos));
            }
        }

        private void Page_GettingFocus(UIElement sender, GettingFocusEventArgs args)
        {
            ResourceLoader resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            lb_Accueil.Text = resourceLoader.GetString("Accueil");
            lb_langue.Text = resourceLoader.GetString("Langue");
            lb_propos.Text = resourceLoader.GetString("Propos");
        }

        private void Principal_Loaded(object sender, RoutedEventArgs e)
        {
            MyFrame.Navigate(typeof(Home));
        }
    }
}
