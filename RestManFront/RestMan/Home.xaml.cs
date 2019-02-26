using System;
using System.Net.Http;
using System.Text;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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

        private void Reponse_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(receivedHeaders) || String.IsNullOrEmpty(receivedResponse))
            {
                Lancer_Click(sender, e);
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

        private void Lancer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                receivedHeaders = string.Empty;
                receivedResponse = string.Empty;
                switch (((TextBlock)Methode.SelectedItem).Text)
                {
                    case "GET":
                        using (HttpClient client = new HttpClient())
                        {
                            HttpResponseMessage response = client.GetAsync(this.Query.Text).Result;
                            HttpContent content = response.Content;
                            receivedResponse += content.ReadAsStringAsync().Result;
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

                            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                Response.BorderBrush = new SolidColorBrush(Colors.Red);
                            }
                            else
                            {
                                Response.BorderBrush = new SolidColorBrush(Colors.Green);
                            }
                        }
                        break;
                    case "POST":
                        using (HttpClient client = new HttpClient())
                        {
                            string json = Body.Text;
                            var content = new StringContent(json, Encoding.UTF8, "application/json");
                            HttpResponseMessage response = client.PostAsync(this.Query.Text, content).Result;
                            getHeaders(response);
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
                        break;
                    case "DELETE":
                        using (HttpClient client = new HttpClient())
                        {
                            HttpResponseMessage response = client.DeleteAsync(this.Query.Text).Result;
                            HttpContent content = response.Content;
                            getHeaders(response);
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
                        break;
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("Intitulé de l'erreur : \n" + ex.Message);
                dialog.Title = "Erreur lors de la requête";
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                var res = dialog.ShowAsync();
            }

        }

        private void getHeaders (HttpResponseMessage response)
        {
            string charset = response.Content.Headers.ContentType.CharSet;
            string mediatype = response.Content.Headers.ContentType.MediaType;
            string Date = response.Headers.Date.Value.ToString();
            string StatusCode = response.StatusCode.ToString();
            string Method = response.RequestMessage.Method.Method;
            string URI = response.RequestMessage.RequestUri.ToString();
            string Version = response.RequestMessage.Version.ToString();
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
            switch (((TextBlock)Methode.SelectedItem).Text)
            {
                case "GET":
                    Body.IsEnabled = false;
                    break;
                case "POST":
                    Body.IsEnabled = true;
                    break;
                case "DELETE":
                    Body.IsEnabled = false;
                    break;
            }
        }
    }
}
