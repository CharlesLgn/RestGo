using RestMan.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.Media.Core;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace RestMan
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        ResourceLoader resourceLoader;
        private List<HeaderElement> HeaderElements = new List<HeaderElement>();
        private string receivedResponse = string.Empty;
        private string receivedHeaders = string.Empty;
        private string contentype = string.Empty;
        enum sendMethods { POST, PATCH, PUT };

        public Home()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 0;
            ContentType.SelectedIndex = 0;
            PrefLang.SelectedIndex = 0;
            BrowseWeb.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Lance la requête, récupère et affiche les résultats
        /// </summary>
        private async void Lancer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                contentype = string.Empty;
                receivedHeaders = string.Empty;
                receivedResponse = string.Empty;
                ResponseImage.Source = null;
                ResponseVideo.Source = null;
                Response.Visibility = Visibility.Collapsed;
                ResponseImage.Visibility = Visibility.Collapsed;
                ResponseVideo.Visibility = Visibility.Collapsed;
                ResponseHTML.Visibility = Visibility.Collapsed;
                BrowseWeb.Visibility = Visibility.Collapsed;
                ImageDescription.Text = string.Empty;
                ResponseGridView.ItemsSource = null;
                HeaderElements.Clear();
                switch (((TextBlock)Methode.SelectedItem).Text)
                {
                    case "GET":
                        getQuery();
                        break;
                    case "POST":
                        sendQuery(sendMethods.POST);
                        break;
                    case "DELETE":
                        deleteQuery();
                        break;
                    case "PATCH":
                        sendQuery(sendMethods.PATCH);
                        break;
                    case "PUT":
                        sendQuery(sendMethods.PUT);
                        break;
                }
            }
            catch (Exception ex)
            {
                Loader.Visibility = Visibility.Collapsed;
                var dialog = new MessageDialog("Intitulé de l'erreur : \n" + ex.Message) { Title = "Erreur lors de la requête" };
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                var res = dialog.ShowAsync();
            }

        }

        private async void deleteQuery()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(this.Query.Text);
                HttpContent content = response.Content;
                getHeaders(response);
                Response.Visibility = Visibility.Visible;
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    receivedResponse = "Echec";
                    Response.BorderBrush = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    receivedResponse = "Succès";
                    Response.BorderBrush = new SolidColorBrush(Colors.Green);
                }

                Response.Text = receivedResponse;
            }
        }

        private async void sendQuery(sendMethods Method)
        {
            using (HttpClient client = new HttpClient())
            {
                string json = Body.Text;
                string selectedContentType = ((TextBlock)ContentType.SelectedItem).Text;
                var content = new StringContent(json, Encoding.UTF8, selectedContentType);
                HttpResponseMessage response = null;
                HttpContent contentres = null;

                switch (Method)
                {
                    case sendMethods.POST:
                        response = await client.PostAsync(this.Query.Text, content);
                        try { contentres = response.Content; } catch { };
                        break;
                    case sendMethods.PATCH:
                        response = await client.PatchAsync(this.Query.Text, content);
                        try { contentres = response.Content; } catch { };
                        break;
                    case sendMethods.PUT:
                        response = await client.PutAsync(this.Query.Text, content);
                        try { contentres = response.Content; } catch { };
                        break;
                }

                getHeaders(response);
                Response.Visibility = Visibility.Visible;
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    receivedResponse = "Echec";
                    Response.BorderBrush = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    if (contentres != null)
                    {
                        receivedResponse = await contentres.ReadAsStringAsync();
                    }
                    else
                    {
                        receivedResponse = "Succès";
                    }
                    Response.BorderBrush = new SolidColorBrush(Colors.Green);
                }

                Response.Text = receivedResponse;
            }
        }

        private async void getQuery()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    Loader.Visibility = Visibility.Visible;
                    HttpResponseMessage response = await client.GetAsync(this.Query.Text);
                    Loader.Visibility = Visibility.Collapsed;
                    HttpContent content = response.Content;
                    contentype = response.Content.Headers.ContentType.MediaType;

                    if (contentype.Contains("application"))
                    {
                        Response.Visibility = Visibility.Visible;
                        receivedResponse += await content.ReadAsStringAsync();
                        getHeaders(response);
                        Response.Text = receivedResponse;
                    }
                    else if (contentype.Contains("image"))
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        Uri uri = new Uri(response.RequestMessage.RequestUri.AbsoluteUri);
                        bitmapImage.UriSource = uri;
                        ResponseImage.Source = bitmapImage;
                        receivedResponse += "Voir image ci-dessous";
                        getHeaders(response);
                        Response.Text = receivedResponse;
                        ResponseImage.Visibility = Visibility.Visible;
                        ImageDescription.Visibility = Visibility.Visible;

                        try
                        {
                            ImageDescription.Text = "Nom du fichier : " + response.Content.Headers.ContentDisposition.FileName + "\n" + "Type : " + response.Content.Headers.ContentType.MediaType;
                        }
                        catch
                        {
                            string fileName = response.RequestMessage.RequestUri.AbsolutePath.Substring(response.RequestMessage.RequestUri.AbsolutePath.LastIndexOf('/') + 1);
                            ImageDescription.Text = "Nom du fichier : " + fileName + "\n" + "Type : " + response.Content.Headers.ContentType.MediaType;
                        }

                    }
                    else if (contentype.Contains("video"))
                    {
                        Uri uri = new Uri(response.RequestMessage.RequestUri.AbsoluteUri);
                        ResponseVideo.Source = MediaSource.CreateFromUri(uri);
                        receivedResponse += "Voir vidéo ci-dessous";
                        getHeaders(response);
                        Response.Text = receivedResponse;
                        ResponseVideo.Visibility = Visibility.Visible;

                    }
                    else if (contentype.Contains("html"))
                    {
                        Uri url = new Uri(response.RequestMessage.RequestUri.AbsoluteUri);
                        ResponseHTML.Navigate(url);
                        SpaceWeb.Height = 20;
                        SpaceWeb2.Height = 20;
                        receivedResponse += await content.ReadAsStringAsync();
                        Response.Text = receivedResponse;
                        BrowseWeb.Visibility = Visibility.Visible;
                        Response.Visibility = Visibility.Visible;
                        getHeaders(response);

                    }
                    else
                    {
                        Response.Visibility = Visibility.Visible;
                        receivedResponse += await content.ReadAsStringAsync();
                        getHeaders(response);
                        Response.Text = receivedResponse;
                    }

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Response.BorderBrush = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        Response.BorderBrush = new SolidColorBrush(Colors.Green);
                    }
                }
            }
            catch (Exception ex)
            {
                Loader.Visibility = Visibility.Collapsed;
                var dialog = new MessageDialog("Intitulé de l'erreur : \n" + ex.Message) { Title = "Erreur lors de la requête" };
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                var res = dialog.ShowAsync();
            }
        }

        private void BrowseWeb_Click(object sender, RoutedEventArgs e)
        {
            ResponseHTML.Visibility = Visibility.Visible;
        }

        private void getHeaders(HttpResponseMessage response)
        {
            List<string> listdata = new List<string>();
            string entetes = response.ToString().Replace("{", string.Empty).Replace("}", string.Empty).Replace(" ", string.Empty).Replace("\r", string.Empty);
            var splitentete = entetes.Split("\n");
            splitentete[0] = splitentete[0].Replace(":", string.Empty);
            foreach (var item in splitentete)
            {
                if (!String.IsNullOrEmpty(item))
                {
                    listdata.Add(item);
                }
            }

            listdata.Sort();

            int iterateur = 1;
            foreach (string item in listdata)
            {

                if (item.Contains(':'))
                {
                    var element = item.Split(':');
                    HeaderElements.Add(new HeaderElement { Entête = element[0], Valeur = element[1] });
                }
                else
                {
                    HeaderElements.Add(new HeaderElement { Entête = item, Valeur = string.Empty });
                }
            }

            ResponseGridView.ItemsSource = HeaderElements;
        }

        private void Methode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Body.Text = string.Empty;
            Response.Text = string.Empty;
            Response.ClearValue(TextBox.BorderBrushProperty);
            receivedHeaders = string.Empty;
            receivedResponse = string.Empty;
            Response.Visibility = Visibility.Visible;
            ResponseImage.Visibility = Visibility.Collapsed;
            ImageDescription.Text = string.Empty;
            ResponseVideo.Visibility = Visibility.Collapsed;
            ResponseHTML.Visibility = Visibility.Collapsed;
            ResponseGridView.ItemsSource = null;
            HeaderElements.Clear();

            switch (((TextBlock)Methode.SelectedItem).Text)
            {
                case "GET":
                case "DELETE":
                    Body.IsEnabled = false;
                    break;
                case "POST":
                case "PATCH":
                case "PUT":
                    Body.IsEnabled = true;
                    break;
            }
        }

        private void allCateg(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 0;
            Query.Text = "http://localhost:8000/categories";
            Lancer_Click(sender, e);
        }

        private void allArticles(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 0;
            Query.Text = "http://localhost:8000/articles";
            Lancer_Click(sender, e);
        }

        private void TranslateLeet(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 1;
            Query.Text = "http://localhost:8000/fun/trad/l33t";
            Body.Text = "{ \"message\":\"coucou ca va ?\"}";
        }

        private void TranslateMorse(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 1;
            Query.Text = "http://localhost:8000/fun/trad/morse";
            Body.Text = "{ \"message\":\"coucou ca va ?\"}";
        }

        private void addCateg(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 1;
            Query.Text = "http://localhost:8000/categorie";
            Body.Text = "{ \"lib\":\"nomdelacatégorie\"}";
        }

        private void addArticle(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 1;
            Query.Text = "http://localhost:8000/article";
            Body.Text = "{\"lib\": \"nomArtcile\",\"price\": 4.99,\"idCateg\": 1}";
        }

        private void delCateg(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 4;
            Query.Text = "http://localhost:8000/categorie/1";
        }

        private void delArticle(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 4;
            Query.Text = "http://localhost:8000/article/1";
        }

        private void getVideo(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 0;
            Query.Text = "http://fr.vid.web.acsta.net/nmedia/33/19/02/04/06//19581974_hd_013.mp4";
            Lancer_Click(sender, e);
        }

        private void getImage(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 0;
            Query.Text = "http://localhost:8001/fun/lissa";
            Lancer_Click(sender, e);
        }

        private void getReponse(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 0;
            Query.Text = "http://localhost:8000/fun/yes";
            Lancer_Click(sender, e);
        }

        private void getCouleur(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 0;
            Query.Text = "http://localhost:8000/fun/color";
            Lancer_Click(sender, e);
        }

        private void getWiki(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 0;
            Query.Text = "http://localhost:8000/fun/wiki/Go_(programming_language)";
            Lancer_Click(sender, e);
        }


        private void getWeb(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 0;
            Query.Text = "http://php.net/";
            Lancer_Click(sender, e);
        }

        private void updateCateg(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 2;
            Query.Text = "http://localhost:8000/categorie/idCategAModifier";
            Body.Text = "{\"lib\":\"nouveauNom\"}";
        }

        private void updateArticle(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 2;
            Query.Text = "http://localhost:8000/article/idArticleAModifier";
            Body.Text = "{\"lib\": \"nouveauNom\",\"price\": 4.99,\"idCateg\": 1}";
        }

        private void putCateg(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 3;
            Query.Text = "http://localhost:8000/categorie/idCategARemplacer";
            Body.Text = "{\"lib\":\"nouveauNom\"}";
        }

        private void putArticle(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 3;
            Query.Text = "http://localhost:8000/article/idArticleARemplacer";
            Body.Text = "{\"lib\": \"nomArticle\",\"price\": 4.99,\"idCateg\": 1}";
        }

        private void QuerySampleOpened(object sender, object e)
        {
            QuerySample.Foreground = new SolidColorBrush(Colors.Red);
            QuerySampleRotate.Rotation = 90;
        }

        private void QuerySampleClosed(object sender, object e)
        {
            QuerySample.ClearValue(Button.ForegroundProperty);
            QuerySampleRotate.Rotation = 0;
        }

        private void Page_GettingFocus(UIElement sender, Windows.UI.Xaml.Input.GettingFocusEventArgs args)
        {
            /*ResourceLoader resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            string foo = resourceLoader.GetString("LancerName");
            lb_lancer.Text = foo;*/
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            lb_lancer.Text = resourceLoader.GetString("LancerName");
            lb_body.Text = resourceLoader.GetString("Body");
            lb_reponse.Text = resourceLoader.GetString("Reponse");
            lb_entetes.Text = resourceLoader.GetString("Entetes");
            lb_visiterPage.Text = resourceLoader.GetString("VisiterPage");
        }
    }
}
