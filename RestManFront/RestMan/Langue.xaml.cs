using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace RestMan
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Langue : Page
    {
        MainPage mp = new MainPage();

        public Langue()
        {
            this.InitializeComponent();
        }

        private void FrImage_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            this.FrImage.Height = 250;
        }

        private void FrImage_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.FrImage.Height = 300;
        }

        private void EnglImage_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.EnglImage.Height = 300;
        }

        private void EnglImage_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            this.EnglImage.Height = 250;
        }

        private void FrImage_Click(object sender, RoutedEventArgs e)
        {
        }

        private void EnglImage_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
