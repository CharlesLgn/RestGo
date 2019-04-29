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
        WebResponse response;
        private string[] customHeaders = { "Accept", "Accept-Charset", "Accept-Encoding", "Accept-Language", "Access-Control-Request-Headers", "Access-Control-Request-Method", "Authorization", "Cache-Control", "Connection", "Content-Language", "Content-Type", "Cookie", "DNT", "Date", "DPR", "Early-Data", "Expect", "Forwarded", "From", "Host", "if-Match", "If-Modified-Since", "If-None-Match", "If-Range", "If-Unmodified-Since", "Keep-Alive", "Max-Forwards", "Origin", "Pragma", "Proxy-Authorization", "Range", "If-Unmodified-Since", "Referer", "Save-Data", "TE", "Trailer", "Transfer-Encoding", "Upgrade", "Upgrade-Insecure-Requests", "User-Agent", "Vary", "Via", "Viewport-Width", "Warning", "Width" };
        private string[] customValuesContentType = { "application/json", "application/x-www-form-urlencoded", "application/xhtml+xml", "application/xml", "multipart/form-data", "text/html", "text/plain", "text/xml" };
        private Dictionary<Button, Dictionary<StackPanel, TextBlock>> listCustomHeaders = new Dictionary<Button, Dictionary<StackPanel, TextBlock>>();
        private Dictionary<Dictionary<StackPanel, TextBlock>, bool> EtatCustomHeader = new Dictionary<Dictionary<StackPanel, TextBlock>, bool>();
        double actualPivotHeaderHeight = 200;
        HttpWebRequest webRequest;
        ResourceLoader resourceLoader;
        private List<HeaderElement> HeaderElements = new List<HeaderElement>();
        private string receivedResponse = string.Empty;
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
            PopulateCustomListView();
            PopulateSaves();
            populateHistory();
        }

        /// <summary>
        /// Lance la requête, récupère et affiche les résultats
        /// </summary>
        private async void Lancer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                contentype = string.Empty;
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

        /// <summary>
        /// Méthode utilisée pour les requêtes autres que get
        /// </summary>
        public async void QuerySender()
        {
            try
            {
                string body = string.Empty;
                string headers = string.Empty;
                Loader.Visibility = Visibility.Visible;
                WebResponse response = await ExecuteHttpWebRequest();
                string bodySave = Body.Text;
                string type = ((TextBlock)Methode.SelectedItem).Text;
                string url = Query.Text;
                string date = DateTime.Now.ToString();
                DataAccess.AddData("HISTORY", type, url, date, bodySave);
                populateHistory();
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

                response = await ExecuteHttpWebRequest();
                var stream = response.GetResponseStream();
                string bodySave = Body.Text;
                string type = ((TextBlock)Methode.SelectedItem).Text;
                string url = Query.Text;
                string date = DateTime.Now.ToString();
                DataAccess.AddData("HISTORY", type, url, date, body);
                populateHistory();
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

        /// <summary>
        /// Ajoute un enregistrement d'une authentification dans la base locale
        /// </summary>
        /// <param name="wc"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Ajoute les entêtes de l'utilisateur à l'objet entête
        /// </summary>
        /// <param name="whc"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Affiche le navigateur web et navigue à l'adresse indiquée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseWeb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Uri url = response.ResponseUri;
                ResponseHTML.Navigate(url);
                ResponseHTML.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("Intitulé de l'erreur : \n" + ex.Message) { Title = "Erreur lors de l'affichage la page" };
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                var res = dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Obtient les entêtes réponse de la requête
        /// </summary>
        /// <param name="response"></param>
        private void getHeaders(string response)
        {
            List<string> listdata = new List<string>();
            string entetes = response.ToString().Replace("{", string.Empty).Replace("}", string.Empty).Replace("\r", string.Empty);
            var splitentete = entetes.Split("\n");
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

        /// <summary>
        /// Applique des changements à l'interface en fonction du type de requête sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Methode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Response.Text = string.Empty;
            Response.ClearValue(TextBox.BorderBrushProperty);
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

        /// <summary>
        /// Colore le bouton pour indiquer son ouverture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuerySampleOpened(object sender, object e)
        {
            QuerySample.Foreground = new SolidColorBrush(Colors.Red);
            QuerySampleRotate.Rotation = 90;
        }

        /// <summary>
        /// Réapplique la couleur par défaut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuerySampleClosed(object sender, object e)
        {
            QuerySample.ClearValue(Button.ForegroundProperty);
            QuerySampleRotate.Rotation = 0;
        }

        /// <summary>
        /// TODO : Faire la langue
        /// Évènement lancé au chargement de la page, récupère les strings et change la langue en conséquence
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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

        /// <summary>
        /// Méthode d'ajoute de controls dans l'affichage pour entrer des entêtes
        /// </summary>
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

        /// <summary>
        /// Méthode de redimensionnement pour les tiroirs
        /// </summary>
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

        /// <summary>
        /// Requête Linq pour indiquer des suggestions
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Ajoute ou retire un bloc d'éléments graphiques pour entrer une requête
        /// </summary>
        private void CustomHeadersManager(UIElement sender)
        {
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

        /// <summary>
        /// Évènement lié dans le code behind aux controls générés dynamiquement pour les entêtes à ajouter
        /// Il est lié aux zone de texte ou entrer le nom de l'entête
        /// </summary>
        private void AutoSuggestBox_TextChanged_Entete(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            CustomHeadersManager(sender);
        }

        /// <summary>
        /// Évènement lié dans le code behind aux controls générés dynamiquement pour les entêtes à ajouter
        /// Il est lié aux zone de texte ou entrer la valeur de l'entête
        /// </summary>
        private void AutoSuggestBox_TextChanged_Value(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            AutoSuggestBox_TextChanged_Entete(sender, args);
        }

        /// <summary>
        /// Associe à ce control un dictionnaire.
        /// Ce dictionnaire contient les entêtes les plus populaires utilisées
        /// </summary>
        private void AutoSuggestBox_GotFocus_Entete(object sender, RoutedEventArgs e)
        {
            ((AutoSuggestBox)sender).ItemsSource = customHeaders;
        }

        /// <summary>
        /// Obtient le control parent
        /// </summary>
        private UIElement getParent(UIElement element)
        {
            return VisualTreeHelper.GetParent(element) as UIElement;
        }

        /// <summary>
        /// Associe à la zone de texte un dictionnaire si l'entête est Content-Type
        /// Le dictionnaire contient les content-type les plus populaires
        /// </summary>
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

        /// <summary>
        /// Méthode de redimensionnement pour les tiroirs
        /// </summary>
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

        /// <summary>
        /// Méthode de redimensionnement pour les tiroirs
        /// </summary>
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
        /// Une simple boite de dialogue pour confirmer un changement
        /// </summary>
        /// <returns></returns>
        private async Task<bool> YesNoDialogAsync(string titre)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = titre;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "Annuler";
            dialog.SecondaryButtonText = "Oui";
            if (await dialog.ShowAsync() == ContentDialogResult.Secondary)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Enregistre l'authentication basique
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

        /// <summary>
        ///  Enregistre l'authentication avec token
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CustomSaveAuthentication_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string libelle = await InputTextDialogAsync();
                DataAccess.AddData("CUSTOMTOKEN", CustomScheme.Text, CustomToken.Text, DateTime.Now.ToString(), libelle);
                PopulateCustomListView();
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("Intitulé de l'erreur : \n" + ex.Message) { Title = "Erreur lors de l'enregistrement" };
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                var res = dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Récupère dans la base locale l'authentification sélectionnée et remplie les zones de texte
        /// Pour l'authentification basique
        /// </summary>
        private void ListViewBasique_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock item = (TextBlock)ListViewBasique.SelectedItem;
            if (item != null)
            {
                string id = (string)item.Tag;
                string[] data = DataAccess.GetByID("BASICTOKEN", id);
                BasiqueUserName.Text = data[1];
                BasiquePassword.Password = data[2];
            }
        }

        /// <summary>
        /// Récupère dans la base locale l'authentification sélectionnée et remplie les zones de texte
        /// Pour l'authentification par token
        /// </summary>
        private void ListViewCustom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock item = (TextBlock)ListViewCustom.SelectedItem;
            if (item != null)
            {
                string id = (string)item.Tag;
                string[] data = DataAccess.GetByID("CUSTOMTOKEN", id);
                CustomScheme.Text = data[1];
                CustomToken.Text = data[2];
            }
        }

        /// <summary>
        /// Supprime les objets sélectionnés de la base
        /// pour l'authentification basique
        /// </summary>
        private async void DeleteBasiqueAuthentication_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool reponse = await YesNoDialogAsync("Supprimer les authentifications enregistrées ?");
                if (reponse)
                {
                    var foo = ListViewBasique.SelectedItems;
                    foreach (var item in foo)
                    {
                        if (item is TextBlock)
                        {
                            string id = (string)((TextBlock)item).Tag;
                            DataAccess.DeleteByID("BASICTOKEN", id);
                        }
                    }

                    PopulateBasiqueListView();
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("Intitulé de l'erreur : \n" + ex.Message) { Title = "Erreur lors de la suppression" };
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                var res = dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Supprime les objets sélectionnés de la base
        /// pour l'authentification par token
        /// </summary>
        private async void DeleteCustomAuthentication_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool reponse = await YesNoDialogAsync("Supprimer les authentifications enregistrées ?");
                if (reponse)
                {
                    var foo = ListViewCustom.SelectedItems;
                    foreach (var item in foo)
                    {
                        if (item is TextBlock)
                        {
                            string id = (string)((TextBlock)item).Tag;
                            DataAccess.DeleteByID("CUSTOMTOKEN", id);
                        }
                    }

                    PopulateCustomListView();
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("Intitulé de l'erreur : \n" + ex.Message) { Title = "Erreur lors de la suppression" };
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                var res = dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Affiche l'historique des requêtes
        /// </summary>
        private void populateHistory()
        {
            ListViewHistory.Items.Clear();
            List<string[]> data = DataAccess.GetData("HISTORY");
            foreach (string[] item in data)
            {
                TextBlock tb = new TextBlock();
                tb.Text = item[3] + "\n" + item[1] + " " + item[2];
                tb.Tag = item[0];
                tb.TextWrapping = TextWrapping.Wrap;
                if (ListViewHistory.Items.Count <= 5)
                    ListViewHistory.Items.Add(tb);
            }
        }

        /// <summary>
        /// Récupère et écrit les valeurs sauvegardées
        /// </summary>
        private void PopulateBasiqueListView()
        {
            ListViewBasique.Items.Clear();
            List<string[]> data = DataAccess.GetData("BASICTOKEN");
            if (data.Count == 0)
            {
                DeleteBasiqueAuthentication.Visibility = Visibility.Collapsed;
            }
            else
            {
                DeleteBasiqueAuthentication.Visibility = Visibility.Visible;
            }

            foreach (string[] item in data)
            {
                TextBlock tb = new TextBlock();
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Tag = item[0];
                tb.Text = "Libellé : " + item[4] + " | Identifiant : " + item[1] + " | Date : " + item[3];
                ListViewBasique.Items.Add(tb);
            }
        }

        /// <summary>
        /// Récupère et écrit les valeurs sauvegardées
        /// </summary>
        private void PopulateCustomListView()
        {
            ListViewCustom.Items.Clear();
            List<string[]> data = DataAccess.GetData("CUSTOMTOKEN");
            if (data.Count == 0)
            {
                DeleteCustomAuthentication.Visibility = Visibility.Collapsed;
            }
            else
            {
                DeleteCustomAuthentication.Visibility = Visibility.Visible;
            }

            foreach (string[] item in data)
            {
                TextBlock tb = new TextBlock();
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Tag = item[0];
                tb.Text = "Libellé : " + item[4] + " | Identifiant : " + item[1] + " | Date : " + item[3];
                ListViewCustom.Items.Add(tb);
            }
        }



        /// <summary>
        /// Affiche les boutons de sauvegarde
        /// </summary>
        private void PopulateSaves()
        {
            Configs.Children.Clear();
            List<string[]> data = DataAccess.GetData("CONFIG");
            foreach (string[] item in data)
            {
                string id = item[0];
                string type = item[1];
                string url = item[2];
                string body = item[3];
                string label = item[4];
                TextBlock hauteur = new TextBlock();
                hauteur.Height = 10;
                Button bt = new Button();
                bt.Tag = id;
                bt.Height = 32;
                bt.Width = 250;
                bt.FontFamily = new FontFamily("Segoe UI");
                bt.Content = label;
                bt.Click += PopulateConfig;
                Configs.Children.Add(bt);
                Configs.Children.Add(hauteur);
            }

            if (data.Count > 0)
            {
                Button btDelete = new Button();
                btDelete.Height = 50;
                btDelete.Width = 250;
                btDelete.FontFamily = new FontFamily("Segoe UI");
                btDelete.Content = "Supprimer toutes les requêtes";
                btDelete.Click += DeleteConfigData;
                btDelete.Foreground = new SolidColorBrush(Colors.DarkOrange);
                Configs.Children.Add(btDelete);
            }
        }

        /// <summary>
        /// Supprime toutes les configurations enregistrées de l'utilisateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteConfigData(object sender, RoutedEventArgs e)
        {
            bool response = await YesNoDialogAsync("Supprimer les enregistrements ?");
            if (response)
            {
                DataAccess.DeleteAllData("CONFIG");
                PopulateSaves();
            }
        }

        /// <summary>
        /// Charge l'élément enregistré
        /// </summary>
        /// <param name="id"></param>
        private void PopulateConfig(object sender, RoutedEventArgs e)
        {
            string id = (string)((Button)sender).Tag;
            string[] data = DataAccess.GetByID("CONFIG", id);
            string type = data[1];
            string url = data[2];
            string body = data[4];
            foreach (TextBlock item in Methode.Items)
            {
                if (item.Text == type)
                {
                    Methode.SelectedItem = item;
                }
            }

            Query.Text = url;
            Body.Text = body;
        }



        /// <summary>
        /// Sauvegarde la configuration actuelle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string libelle = await InputTextDialogAsync();
                string type = ((TextBlock)Methode.SelectedItem).Text;
                string url = Query.Text;
                string body = Body.Text;
                DataAccess.AddData("CONFIG", type, url, body, libelle);
                PopulateSaves();
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("Intitulé de l'erreur : \n" + ex.Message) { Title = "Erreur lors de l'enregistrement" };
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                var res = dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Adaptation de la taille de la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //pivot.Width = Body.Width;
            double size = ((Frame)Window.Current.Content).ActualWidth;
            if (size > 1700)
            {
                pivot.Width = 1300;
                Second.Padding = new Thickness(0, 0, 0, 0);
                BasiqueUserName.Width = 300;
                BasiquePassword.Width = 300;
                saveAuthentication.Width = 150;
                CustomScheme.Width = 300;
                CustomToken.Width = 300;
                CustomSaveAuthentication.Width = 150;
                expanderCustomStackFormulaire.Orientation = Orientation.Horizontal;
                expanderBasiqueStackFormulaire.Orientation = Orientation.Horizontal;
                foreach (UIElement val in multipleEntete.Children)
                {
                    if (val is StackPanel)
                    {
                        foreach (UIElement elmt in ((StackPanel)val).Children)
                        {
                            if (elmt is AutoSuggestBox)
                            {
                                if (((AutoSuggestBox)elmt).PlaceholderText == "Entête")
                                {
                                    ((AutoSuggestBox)elmt).Width = 350;
                                }
                                else if (((AutoSuggestBox)elmt).PlaceholderText == "Valeur")
                                {
                                    ((AutoSuggestBox)elmt).Width = 750;
                                }
                            }
                            else if (elmt is Button)
                            {
                                ((Button)elmt).Width = 100;
                            }
                            else if (elmt is TextBlock)
                            {
                                ((TextBlock)elmt).Width = 20;
                            }
                        }
                    }
                }
            }
            else if (size < 1700 && size > 900)
            {
                pivot.Width = 900;
                Second.Padding = new Thickness(0, 25, 0, 0);
                BasiqueUserName.Width = 200;
                BasiquePassword.Width = 200;
                saveAuthentication.Width = 100;
                CustomScheme.Width = 200;
                CustomToken.Width = 200;
                CustomSaveAuthentication.Width = 100;
                expanderCustomStackFormulaire.Orientation = Orientation.Horizontal;
                expanderBasiqueStackFormulaire.Orientation = Orientation.Horizontal;
                foreach (UIElement val in multipleEntete.Children)
                {
                    if (val is StackPanel)
                    {
                        foreach (UIElement elmt in ((StackPanel)val).Children)
                        {
                            if (elmt is AutoSuggestBox)
                            {
                                if (((AutoSuggestBox)elmt).PlaceholderText == "Entête")
                                {
                                    ((AutoSuggestBox)elmt).Width = 225;
                                }
                                else if (((AutoSuggestBox)elmt).PlaceholderText == "Valeur")
                                {
                                    ((AutoSuggestBox)elmt).Width = 500;
                                }
                            }
                            else if (elmt is Button)
                            {
                                ((Button)elmt).Width = 100;
                            }
                            else if (elmt is TextBlock)
                            {
                                ((TextBlock)elmt).Width = 20;
                            }
                        }
                    }
                }
            }
            else if (size < 900)
            {
                pivot.Width = Double.NaN;
                Second.Padding = new Thickness(0, 25, 0, 0);
                BasiqueUserName.Width = 150;
                BasiquePassword.Width = 150;
                saveAuthentication.Width = 100;
                CustomScheme.Width = 150;
                CustomToken.Width = 150;
                CustomSaveAuthentication.Width = 100;
                expanderCustomStackFormulaire.Orientation = Orientation.Vertical;
                expanderBasiqueStackFormulaire.Orientation = Orientation.Vertical;
                foreach (UIElement val in multipleEntete.Children)
                {
                    if (val is StackPanel)
                    {
                        foreach (UIElement elmt in ((StackPanel)val).Children)
                        {
                            if (elmt is AutoSuggestBox)
                            {
                                if (((AutoSuggestBox)elmt).PlaceholderText == "Entête")
                                {
                                    ((AutoSuggestBox)elmt).Width = 125;
                                }
                                else if (((AutoSuggestBox)elmt).PlaceholderText == "Valeur")
                                {
                                    ((AutoSuggestBox)elmt).Width = 125;
                                }
                            }
                            else if (elmt is Button)
                            {
                                ((Button)elmt).Width = 50;
                            }
                            else if (elmt is TextBlock)
                            {
                                ((TextBlock)elmt).Width = 5;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Remplie les champs à partir de la requête cliquée dans l'historique
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock tb = (TextBlock)ListViewHistory.SelectedItem;
            if (tb != null)
            {
                string id = (string)tb.Tag;
                string[] data = DataAccess.GetByID("HISTORY", id);
                string type = data[1];
                string url = data[2];
                string body = data[4];
                foreach (TextBlock item in Methode.Items)
                {
                    if (item.Text == type)
                    {
                        Methode.SelectedItem = item;
                    }
                }

                Query.Text = url;
                Body.Text = body;
            }
        }
    }
}

