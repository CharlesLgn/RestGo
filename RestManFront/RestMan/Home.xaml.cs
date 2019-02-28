using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Windows.Media.Core;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace RestMan
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Home : Page
    {
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
        }

        /// <summary>
        /// Change l'affichage pour afficher soit la réponse soit les entêtes
        /// </summary>
        private void Reponse_Click(object sender, RoutedEventArgs e)
        {
            if (((TextBlock)Methode.SelectedItem).Text == "GET")
            {
                Response.Visibility = Visibility.Collapsed;
                ResponseImage.Visibility = Visibility.Collapsed;
                ResponseVideo.Visibility = Visibility.Collapsed;
                if (String.IsNullOrEmpty(receivedHeaders) || String.IsNullOrEmpty(receivedResponse))
                {
                    Lancer_Click(sender, e);
                }

                if (contentype.Contains("application"))
                {
                    Response.Visibility = Visibility.Visible;
                    if (this.ReponseName.Text == "Réponse")
                    {
                        ReponseName.Text = "Entête";
                        Response.Text = receivedResponse;
                    }
                    else
                    {
                        this.ReponseName.Text = "Réponse";
                        Response.Text = receivedHeaders;
                    }
                }
                else if (contentype.Contains("image"))
                {
                    if (this.ReponseName.Text == "Réponse")
                    {
                        ReponseName.Text = "Entête";
                        Response.Visibility = Visibility.Collapsed;
                        ImageDescription.Visibility = Visibility.Visible;
                        ResponseImage.Visibility = Visibility.Visible;
                        Response.Text = receivedResponse;
                    }
                    else
                    {
                        this.ReponseName.Text = "Réponse";
                        Response.Text = receivedHeaders;
                        Response.Visibility = Visibility.Visible;
                        ResponseImage.Visibility = Visibility.Collapsed;
                        ImageDescription.Visibility = Visibility.Collapsed;
                    }
                }
                else if (contentype.Contains("video"))
                {
                    if (this.ReponseName.Text == "Réponse")
                    {
                        ReponseName.Text = "Entête";
                        Response.Visibility = Visibility.Collapsed;
                        ResponseVideo.Visibility = Visibility.Visible;
                        Response.Text = receivedResponse;
                    }
                    else
                    {
                        this.ReponseName.Text = "Réponse";
                        Response.Text = receivedHeaders;
                        Response.Visibility = Visibility.Visible;
                        ResponseVideo.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                if (this.ReponseName.Text == "Réponse")
                {
                    ReponseName.Text = "Entête";
                    Response.Text = receivedResponse;
                }
                else
                {
                    this.ReponseName.Text = "Réponse";
                    Response.Text = receivedHeaders;
                }
            }
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
                ImageDescription.Text = string.Empty;
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

                if (this.ReponseName.Text == "Réponse")
                {
                    ReponseName.Text = "Entête";
                    Response.Text = receivedResponse;
                }
                else
                {
                    this.ReponseName.Text = "Réponse";
                    Response.Text = receivedHeaders;
                }
            }
        }

        private async void sendQuery(sendMethods Method)
        {
            using (HttpClient client = new HttpClient())
            {
                string json = Body.Text;
                //var content = new StringContent(json, Encoding.UTF8, "application/json");
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

                if (this.ReponseName.Text == "Réponse")
                {
                    ReponseName.Text = "Entête";
                    Response.Text = receivedResponse;
                }
                else
                {
                    this.ReponseName.Text = "Réponse";
                    Response.Text = receivedHeaders;
                }
            }
        }

        private async void getQuery()
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
                    if (this.ReponseName.Text == "Réponse")
                    {
                        ReponseName.Text = "Entête";
                        Response.Text = receivedResponse;
                    }
                    else
                    {
                        this.ReponseName.Text = "Réponse";
                        Response.Text = receivedHeaders;
                    }
                }
                else if (contentype.Contains("image"))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    Uri uri = new Uri(response.RequestMessage.RequestUri.AbsoluteUri);
                    bitmapImage.UriSource = uri;
                    ResponseImage.Source = bitmapImage;
                    receivedResponse += "Voir image ci-dessous";
                    getHeaders(response);
                    if (this.ReponseName.Text == "Réponse")
                    {
                        ReponseName.Text = "Entête";
                        Response.Text = receivedResponse;
                        Response.Visibility = Visibility.Collapsed;
                        ResponseImage.Visibility = Visibility.Visible;
                        ImageDescription.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.ReponseName.Text = "Réponse";
                        Response.Text = receivedHeaders;
                        Response.Visibility = Visibility.Visible;
                        ResponseImage.Visibility = Visibility.Collapsed;
                        ImageDescription.Visibility = Visibility.Collapsed;
                    }

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
                    if (this.ReponseName.Text == "Réponse")
                    {
                        ReponseName.Text = "Entête";
                        Response.Text = receivedResponse;
                        Response.Visibility = Visibility.Collapsed;
                        ResponseVideo.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.ReponseName.Text = "Réponse";
                        Response.Text = receivedHeaders;
                        Response.Visibility = Visibility.Visible;
                        ResponseVideo.Visibility = Visibility.Collapsed;
                    }
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

        private void getHeaders(HttpResponseMessage response)
        {
            string charset = string.Empty;
            string mediatype = string.Empty;
            string Date = string.Empty;
            string StatusCode = string.Empty;
            string Method = string.Empty;
            string URI = string.Empty;
            string Version = string.Empty;

            try { charset = response.Content.Headers.ContentType.CharSet; } catch { }
            try { mediatype = response.Content.Headers.ContentType.MediaType; } catch { }
            try { Date = response.Headers.Date.Value.ToString(); } catch { }
            try { StatusCode = response.StatusCode.ToString(); } catch { }
            try { Method = response.RequestMessage.Method.Method; } catch { }
            try { URI = response.RequestMessage.RequestUri.ToString(); } catch { }
            try { Version = response.RequestMessage.Version.ToString(); } catch { }

            receivedHeaders += "Encodage : " + charset + "\n";
            receivedHeaders += "Type : " + mediatype + "\n";
            receivedHeaders += "Date : " + Date + "\n";
            receivedHeaders += "Code status : " + StatusCode + "\n";
            receivedHeaders += "Méthode : " + Method + "\n";
            receivedHeaders += "Requête : " + URI + "\n";
            receivedHeaders += "Version : " + Version + "\n";
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
            Query.Text = "http://fr.vid.web.acsta.net/nmedia/33/18/11/12/18/19581096_hd_013.mp4";
            Lancer_Click(sender, e);
        }

        private void getImage(object sender, RoutedEventArgs e)
        {
            Methode.SelectedIndex = 0;
            Query.Text = "https://upload.wikimedia.org/wikipedia/en/thumb/4/4c/University_of_Lorraine_%28logo%29.png/200px-University_of_Lorraine_%28logo%29.png";
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
    }
}
