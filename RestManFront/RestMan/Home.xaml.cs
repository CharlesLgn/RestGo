using Newtonsoft.Json;
using RestMan.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using RestManDataAccess;
using Windows.UI.Xaml.Input;


// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace RestMan
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        private string[] customHeaders = { "Accept", "Accept-Charset", "Accept-Encoding", "Accept-Language", "Access-Control-Request-Headers", "Access-Control-Request-Method", "Authorization", "Cache-Control", "Connection", "Content-Language", "Content-Type", "Cookie", "DNT", "Date", "DPR", "Early-Data", "Expect", "Forwarded", "From", "Host", "if-Match", "If-Modified-Since", "If-None-Match", "If-Range", "If-Unmodified-Since", "Keep-Alive", "Max-Forwards", "Origin", "Pragma", "Proxy-Authorization", "Range", "If-Unmodified-Since", "Referer", "Save-Data", "TE", "Trailer", "Transfer-Encoding", "Upgrade", "Upgrade-Insecure-Requests", "User-Agent", "Vary", "Via", "Viewport-Width", "Warning", "Width" };
        private string[] customValuesContentType = { "application/json", "application/x-www-form-urlencoded", "application/xhtml+xml", "application/xml", "multipart/form-data", "text/html", "text/plain", "text/xml" };
        private int iterateurCustomHeaders = 0;
        private Dictionary<Button, Dictionary<StackPanel, TextBlock>> listCustomHeaders = new Dictionary<Button, Dictionary<StackPanel, TextBlock>>();
        private Dictionary<Dictionary<StackPanel, TextBlock>, bool> EtatCustomHeader = new Dictionary<Dictionary<StackPanel, TextBlock>, bool>();
        double actualPivotHeaderHeight = 200;
        HttpWebRequest webRequest;
        ResourceLoader resourceLoader;
        private List<HeaderElement> HeaderElements = new List<HeaderElement>();
        private string receivedResponse = string.Empty;
        private string receivedHeaders = string.Empty;
        private string contentype = string.Empty;
        enum sendMethods { POST, PATCH, PUT, GET };

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
            addCustomHeader();
            PopulateBasiqueListView();
        }

        private void PopulateBasiqueListView()
        {
            ListViewBasique.Items.Clear();
            List<string> data = DataAccess.GetData("BASICTOKEN");
            if(data.Count == 0)
            {
                DeleteBasiqueAuthentication.Visibility = Visibility.Collapsed;
            }
            else
            {
                DeleteBasiqueAuthentication.Visibility = Visibility.Visible;
            }

            foreach (string item in data)
            {
                var splitItem = item.Split('|');
                ListViewBasique.Items.Add(splitItem[0] + " Libellé : " + splitItem[4] + " | Identifiant : " + splitItem[1] + " | Date : " + splitItem[3]);
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
                ResponseHTML.Visibility = Visibility.Collapsed;
                BrowseWeb.Visibility = Visibility.Collapsed;
                ImageDescription.Text = string.Empty;
                ResponseGridView.ItemsSource = null;
                HeaderElements.Clear();
                string method = ((TextBlock)Methode.SelectedItem).Text;
                if (method == "GET")
                {
                    getQuery();
                }
                else
                {
                    QuerySender();
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

        public async void QuerySender()
        {
            try
            {
                string body = string.Empty;
                string headers = string.Empty;
                Loader.Visibility = Visibility.Visible;
                WebResponse response = await ExecuteHttpWebRequest();
                Response.Visibility = Visibility.Visible;
                if (((System.Net.HttpWebResponse)response).StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Response.BorderBrush = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    Response.BorderBrush = new SolidColorBrush(Colors.Green);
                    var stream = response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        Response.Text = reader.ReadToEnd();
                    }
                    getHeaders(response.Headers.ToString());
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("Intitulé de l'erreur : \n" + ex.Message) { Title = "Erreur lors de la requête" };
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                var res = dialog.ShowAsync();
            }
            Loader.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// Effectue une requête get
        /// </summary>
        private async void getQuery()
        {
            try
            {
                string body = string.Empty;
                string headers = string.Empty;
                Loader.Visibility = Visibility.Visible;
                WebResponse response = await ExecuteHttpWebRequest();
                var stream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream))
                {
                    body = reader.ReadToEnd();
                }

                contentype = response.ContentType;
                if (contentype.Contains("application"))
                {
                    Response.Visibility = Visibility.Visible;
                    receivedResponse += body;
                    getHeaders(response.Headers.ToString());
                    Response.Text = receivedResponse;
                }
                else if (contentype.Contains("image"))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    Uri uri = response.ResponseUri;
                    bitmapImage.UriSource = uri;
                    ResponseImage.Source = bitmapImage;
                    receivedResponse += "Voir image ci-dessous";
                    getHeaders(response.Headers.ToString());
                    Response.Text = receivedResponse;
                    ResponseImage.Visibility = Visibility.Visible;
                    ImageDescription.Visibility = Visibility.Visible;

                    try
                    {
                        ImageDescription.Text = "Type : " + response.ContentType;
                    }
                    catch
                    {
                        ImageDescription.Text = "Nom du fichier introuvable";
                    }
                }
                else if (contentype.Contains("video"))
                {
                    Uri uri = response.ResponseUri;
                    ResponseVideo.Source = MediaSource.CreateFromUri(uri);
                    receivedResponse += "Voir vidéo ci-dessous";
                    getHeaders(response.Headers.ToString());
                    Response.Text = receivedResponse;
                    ResponseVideo.Visibility = Visibility.Visible;

                }
                else if (contentype.Contains("html"))
                {
                    Uri url = response.ResponseUri;
                    ResponseHTML.Navigate(url);
                    SpaceWeb.Height = 20;
                    SpaceWeb2.Height = 20;
                    receivedResponse += body;
                    Response.Text = receivedResponse;
                    BrowseWeb.Visibility = Visibility.Visible;
                    Response.Visibility = Visibility.Visible;
                    getHeaders(response.Headers.ToString());
                }
                else
                {
                    Response.Visibility = Visibility.Visible;
                    receivedResponse += body;
                    getHeaders(response.Headers.ToString());
                    Response.Text = receivedResponse;
                }

                if (((System.Net.HttpWebResponse)response).StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Response.BorderBrush = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    Response.BorderBrush = new SolidColorBrush(Colors.Green);
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("Intitulé de l'erreur : \n" + ex.Message) { Title = "Erreur lors de la requête" };
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                var res = dialog.ShowAsync();
            }

            Loader.Visibility = Visibility.Collapsed;
        }

        private async void deleteQuery()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(this.Query.Text);
                HttpContent content = response.Content;
                //getHeaders(response);
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
                        //response = await client.PatchAsync(this.Query.Text, content);
                        try { contentres = response.Content; } catch { };
                        break;
                    case sendMethods.PUT:
                        response = await client.PutAsync(this.Query.Text, content);
                        try { contentres = response.Content; } catch { };
                        break;
                }

                //getHeaders(response);
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

        private WebHeaderCollection AddAuthorization(WebHeaderCollection wc)
        {

            if (expanderBasique.IsExpanded)
            {
                if (!String.IsNullOrEmpty(BasiqueUserName.Text) && !String.IsNullOrEmpty(BasiquePassword.Password))
                {
                    string username = BasiqueUserName.Text.Trim();
                    string password = BasiquePassword.Password.Trim();
                    string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                    wc.Add("Authorization", "Basic " + encoded);
                }
            }
            else if (expanderCustom.IsExpanded)
            {
                if (!String.IsNullOrEmpty(CustomScheme.Text) && !String.IsNullOrEmpty(CustomToken.Text))
                {
                    string username = CustomScheme.Text.Trim();
                    string password = CustomToken.Text.Trim();
                    string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                    wc.Add("Authorization", "Basic " + encoded);
                }
            }

            return wc;
        }

        private WebHeaderCollection BuildHeaderCollection(WebHeaderCollection whc)
        {
            foreach (KeyValuePair<Button, Dictionary<StackPanel, TextBlock>> entry in listCustomHeaders)
            {
                foreach (KeyValuePair<StackPanel, TextBlock> val in entry.Value)
                {
                    StackPanel sp = val.Key;
                    string entete = string.Empty;
                    string valeur = string.Empty;
                    foreach (UIElement item in sp.Children)
                    {
                        if (item.GetType() == typeof(AutoSuggestBox))
                        {
                            if (((AutoSuggestBox)item).PlaceholderText == "Entête")
                            {
                                entete = ((AutoSuggestBox)item).Text;
                            }

                            if (((AutoSuggestBox)item).PlaceholderText == "Valeur")
                            {
                                valeur = ((AutoSuggestBox)item).Text;
                            }

                            if (!String.IsNullOrEmpty(entete) && !String.IsNullOrEmpty(valeur))
                            {
                                whc.Add(entete, valeur);
                            }
                        }
                    }
                }
            }

            return whc;
        }

        /// <summary>
        /// Contruit et éxecute la requête
        /// </summary>
        /// <returns></returns>
        private async Task<WebResponse> ExecuteHttpWebRequest()
        {
            var selectedMethode = Methode.SelectedItem as TextBlock;
            string method = selectedMethode.Text;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Query.Text);
            WebHeaderCollection myWebHeaderCollection = webRequest.Headers;
            myWebHeaderCollection = BuildHeaderCollection(myWebHeaderCollection);
            myWebHeaderCollection = AddAuthorization(myWebHeaderCollection);
            webRequest.PreAuthenticate = true;
            webRequest.Method = method;
            webRequest.UserAgent = "something";
            if (webRequest.Method == "POST" || webRequest.Method == "PUT" || webRequest.Method == "PATCH")
            {
                byte[] data = Encoding.ASCII.GetBytes(Body.Text);
                webRequest.ContentLength = data.Length;
                using (var stream = webRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return await webRequest.GetResponseAsync();
        }

        private void BrowseWeb_Click(object sender, RoutedEventArgs e)
        {
            ResponseHTML.Visibility = Visibility.Visible;
        }

        private void getHeaders(string response)
        {
            List<string> listdata = new List<string>();
            string entetes = response.ToString().Replace("{", string.Empty).Replace("}", string.Empty).Replace("\r", string.Empty);
            var splitentete = entetes.Split("\n");
            //splitentete[0] = splitentete[0].Replace(":", string.Empty);
            foreach (var item in splitentete)
            {
                if (!String.IsNullOrEmpty(item))
                {
                    listdata.Add(item);
                }
            }

            listdata.Sort();
            foreach (string item in listdata)
            {

                if (item.Contains(':'))
                {
                    var element = item.Split(new[] { ':' }, 2); // Split par le premier caractère uniquement
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
            Query.Text = "http://localhost:8000/fun/lissa";
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
            lb_reponse.Text = resourceLoader.GetString("Reponse");
            lb_entetes.Text = resourceLoader.GetString("Entetes");
            lb_visiterPage.Text = resourceLoader.GetString("VisiterPage");
        }

        /// <summary>
        /// Sur cet évènement, on ajoute un nouveau couple de controls pour ajouter une entête
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AjouterHeader_Click(object sender, RoutedEventArgs e)
        {
            addCustomHeader();
        }

        private void addCustomHeader()
        {
            pivot.Height = pivot.Height + 40;
            TextBlock tbh10 = new TextBlock();
            tbh10.Height = 10;
            TextBlock tbw30 = new TextBlock();
            tbw30.Width = 30;
            TextBlock tbw30n2 = new TextBlock();
            tbw30n2.Width = 30;
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            AutoSuggestBox tbHeader = new AutoSuggestBox();
            tbHeader.PlaceholderText = "Entête";
            tbHeader.Width = 300;
            tbHeader.TextChanged += AutoSuggestBox_TextChanged_Entete;
            tbHeader.GotFocus += AutoSuggestBox_GotFocus_Entete;
            AutoSuggestBox tbValue = new AutoSuggestBox();
            tbValue.PlaceholderText = "Valeur";
            tbValue.Width = 750;
            tbValue.TextChanged += AutoSuggestBox_TextChanged_Entete;
            tbValue.GotFocus += AutoSuggestBox_GotFocus_Valeur;
            Button btRemove = new Button();
            btRemove.Height = 32;
            btRemove.Width = 100;
            btRemove.Click += RetirerHeader_Click;
            StackPanel spbtRemove = new StackPanel();
            spbtRemove.Orientation = Orientation.Horizontal;
            TextBlock tbMinusPic = new TextBlock();
            tbMinusPic.FontFamily = new FontFamily("Segoe MDL2 Assets");
            tbMinusPic.Text = "";
            spbtRemove.Children.Add(tbMinusPic);
            btRemove.Content = spbtRemove;
            sp.Children.Add(tbHeader);
            sp.Children.Add(tbw30);
            sp.Children.Add(tbValue);
            sp.Children.Add(tbw30n2);
            sp.Children.Add(btRemove);
            multipleEntete.Children.Add(tbh10);
            multipleEntete.Children.Add(sp);
            actualPivotHeaderHeight = pivot.Height;

            Dictionary<StackPanel, TextBlock> dicTempo = new Dictionary<StackPanel, TextBlock>();
            dicTempo.Add(sp, tbh10);
            listCustomHeaders.Add(btRemove, dicTempo);
            EtatCustomHeader.Add(dicTempo, false);
        }

        private void TbHeader_GotFocus(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Windows.UI.Xaml.Controls.ItemCollection items = pivot.Items;
            PivotItem pi = ((PivotItem)pivot.SelectedItem);
            if (pi.Name == "PivotItemHeaders")
            {
                pivot.Height = actualPivotHeaderHeight;
            }
            else if (pi.Name == "PivotItemBody")
            {
                pivot.Height = 200;
            }
            else if (pi.Name == "PivotItemAuthorization")
            {
                pivot.Height = 400;
            }
        }

        /// <summary>
        /// Retire le custom header
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RetirerHeader_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<Button, Dictionary<StackPanel, TextBlock>> entry in listCustomHeaders)
            {
                if (entry.Key == (Button)sender)
                {
                    if (getCustomHeaderState(entry.Key))
                    {
                        foreach (KeyValuePair<StackPanel, TextBlock> val in entry.Value)
                        {
                            multipleEntete.Children.Remove(val.Key);
                            multipleEntete.Children.Remove(val.Value);
                            pivot.Height = pivot.Height - 40;
                        }
                    }
                }
            }
        }

        private string[] GetSuggestions(string text)
        {
            return customHeaders.Where(x => x.ToLower().StartsWith(text.ToLower())).ToArray();
        }

        /// <summary>
        /// Indique si le custom header est vide ou non
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private bool getCustomHeaderState(UIElement element)
        {
            var parent = getParent(element);
            foreach (KeyValuePair<Dictionary<StackPanel, TextBlock>, bool> entry in EtatCustomHeader)
            {
                foreach (KeyValuePair<StackPanel, TextBlock> val in entry.Key)
                {
                    if (val.Key == parent)
                    {
                        if (!entry.Value)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void CustomHeadersManager(UIElement sender)
        {
            StackPanel selectedStack = new StackPanel();
            bool isModified = false;
            ((AutoSuggestBox)sender).ItemsSource = this.GetSuggestions(((AutoSuggestBox)sender).Text);
            var parent = getParent(sender);
            foreach (KeyValuePair<Dictionary<StackPanel, TextBlock>, bool> entry in EtatCustomHeader)
            {
                foreach (KeyValuePair<StackPanel, TextBlock> val in entry.Key)
                {
                    if (val.Key == parent)
                    {
                        if (!entry.Value && ((AutoSuggestBox)sender).Text.Length > 0)
                        {
                            EtatCustomHeader[entry.Key] = true;
                            addCustomHeader();
                            isModified = true;
                            break;
                        }
                        else if (entry.Value && ((AutoSuggestBox)sender).Text.Length < 1)
                        {
                            ((AutoSuggestBox)sender).ItemsSource = customHeaders;
                            multipleEntete.Children.Remove(val.Key);
                            multipleEntete.Children.Remove(val.Value);
                            pivot.Height = pivot.Height - 40;
                        }
                    }
                }

                if (isModified)
                {
                    break;
                }
            }
        }

        private void AutoSuggestBox_TextChanged_Entete(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            CustomHeadersManager(sender);
        }

        private void AutoSuggestBox_TextChanged_Value(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            AutoSuggestBox_TextChanged_Entete(sender, args);
        }

        private void AutoSuggestBox_GotFocus_Entete(object sender, RoutedEventArgs e)
        {
            ((AutoSuggestBox)sender).ItemsSource = customHeaders;
        }

        private UIElement getParent(UIElement element)
        {
            return VisualTreeHelper.GetParent(element) as UIElement;
        }

        private void AutoSuggestBox_GotFocus_Valeur(object sender, RoutedEventArgs e)
        {
            var parent = getParent((AutoSuggestBox)sender);

            foreach (UIElement val in ((StackPanel)parent).Children)
            {
                if (val is AutoSuggestBox)
                {
                    if (((AutoSuggestBox)val).Text == "Content-Type")
                    {
                        ((AutoSuggestBox)sender).ItemsSource = customValuesContentType;
                    }
                }
            }
        }

        private void CollapseExpander()
        {
            if (!expanderBasique.IsExpanded)
            {
                expanderBasique.Height = 50;
            }
            else
            {
                expanderCustom.Height = 50;
            }
            expanderCustom.Visibility = Visibility.Visible;
            expanderBasique.Visibility = Visibility.Visible;
        }

        private void ExpandExpander()
        {
            if (expanderBasique.IsExpanded)
            {
                expanderBasique.Height = 400;
                expanderCustom.Visibility = Visibility.Collapsed;
            }
            else
            {
                expanderCustom.Height = 400;
                expanderBasique.Visibility = Visibility.Collapsed;
            }
        }

        private void ExpanderBasique_Expanded(object sender, EventArgs e)
        {
            ExpandExpander();
        }

        private void ExpanderBasique_Collapsed(object sender, EventArgs e)
        {
            CollapseExpander();
        }

        private void ExpanderCustom_Expanded(object sender, EventArgs e)
        {
            ExpandExpander();
        }

        private void ExpanderCustom_Collapsed(object sender, EventArgs e)
        {
            CollapseExpander();
        }

        /// <summary>
        /// Charge les authentications précédentes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void buildSavedAuthenticationButton()
        {

        }

        /// <summary>
        /// Une simple boite de dialogue pour entrer du texte
        /// </summary>
        /// <returns></returns>
        private async Task<string> InputTextDialogAsync()
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 32;
            ContentDialog dialog = new ContentDialog();
            dialog.Content = inputTextBox;
            dialog.Title = "Entrez un nom pour votre sauvegarde";
            dialog.IsSecondaryButtonEnabled = false;
            dialog.PrimaryButtonText = "Enregistrer";
            //dialog.SecondaryButtonText = "Cancel";
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return inputTextBox.Text;
            else
                return "";
        }

        /// <summary>
        /// Enregistre l'authentication
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveAuthentication_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string libelle = await InputTextDialogAsync();
                DataAccess.AddData("BASICTOKEN", BasiqueUserName.Text, BasiquePassword.Password, DateTime.Now.ToString(), libelle);
                PopulateBasiqueListView();
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("Intitulé de l'erreur : \n" + ex.Message) { Title = "Erreur lors de l'enregistrement" };
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                var res = dialog.ShowAsync();
            }
        }

        private void ExpanderBasique_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void ListViewBasique_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void ListViewBasique_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ListViewBasique.SelectedItem;
            if (!String.IsNullOrEmpty(item as string))
            {
                var splitItem = ((string)item).Split(' ');
                string strID = splitItem[0];
                int ID = Int32.Parse(splitItem[0]);
                List<string> data = DataAccess.GetByID("BASICTOKEN", ID);
                string result = data[0];
                var splitResult = result.Split('|');
                BasiqueUserName.Text = splitResult[0];
                BasiquePassword.Password = splitResult[1];
            }
        }

        /// <summary>
        /// supprime les objets sélectionnés de la base
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteBasiqueAuthentication_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var foo = ListViewBasique.SelectedItems;
                foreach (var item in foo)
                {
                    var splitItem = ((string)item).Split(' ');
                    string strID = splitItem[0];
                    int ID = Int32.Parse(splitItem[0]);
                    DataAccess.DeleteByID("BASICTOKEN", ID);
                }

                PopulateBasiqueListView();
            }
            catch(Exception ex)
            {
                var dialog = new MessageDialog("Intitulé de l'erreur : \n" + ex.Message) { Title = "Erreur lors de l'enregistrement" };
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                var res = dialog.ShowAsync();
            }
        }
    }
}

