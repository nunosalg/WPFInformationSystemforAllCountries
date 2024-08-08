using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WPFProjetoFinalPaises.Models;
using WPFProjetoFinalPaises.Services;

namespace WPFProjetoFinalPaises
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributes

        private List<Country> Countries;

        private NetworkService networkService;

        private ApiService apiService;

        private DataService dataService;

        #endregion

        // API key for google maps
        private const string GoogleMapsApiKey = "yourGoogleMapsAPIKey";

        public MainWindow()
        {
            InitializeComponent();
            networkService = new NetworkService();
            apiService = new ApiService();
            dataService = new DataService();
            LoadCountries();
        }

        /// <summary>
        /// Loads the countries' data
        /// </summary>
        private async void LoadCountries()
        {
            bool load;

            txtStatus.Text = "Loading countries' list...";

            var connection = networkService.CheckConnection();
            // Creates a progress reporter that updates the value of the progress bar control
            Progress<int> progress = new Progress<int>(value => { progressBarLoad.Value = value; });

            //If the connection is sucessful, the countries are loaded via API, if not they're loaded from the local database
            if (connection.IsSucess)
            {
                await LoadApiCountriesAsync(progress);
                load = true;
            }
            else
            {
                LoadLocalCountries();
                load = false;
            }

            // If you try to load the countries from the database, for the first time, before loading from the API
            if (Countries.Count == 0)
            {
                txtStatus.Text = "First, you need to connect to the internet to load the countries' list...";
                return;
            }

            if (load)
            {
                txtStatus.Text = $"Countries' list loaded from the API at {DateTime.Now:HH:mm dd/MM/yyyy}";
            }
            else
            {
                txtStatus.Text = "Countries' list loaded from the database";
            }

            // Indicates the source for the listbox and orders the countries by their name
            listBoxCountries.ItemsSource = Countries.OrderBy(c => c.Name.Common);
            // Sets the display as their Name.Common
            listBoxCountries.DisplayMemberPath = "Name.Common";

            // Hides the progress bar when its value reaches 100
            if (progressBarLoad.Value == 100)
            {
                progressBarLoad.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Loads the countries information from the database
        /// </summary>
        private void LoadLocalCountries()
        {
            Countries = dataService.GetData();

            progressBarLoad.Value = 100;
        }

        /// <summary>
        /// Loads the countries' information from the API
        /// </summary>
        /// <returns></returns>
        private async Task LoadApiCountriesAsync(IProgress<int> progress)
        {
            var response = await apiService.GetCountries("https://restcountries.com", "/v3.1/all");
            // List of countries loaded from the API
            Countries = (List<Country>)response.Result;

            int totalCountries = Countries.Count;
            int currentCountry = 0;
            int milisecondsDelay = 5; // Delay's duration

            foreach (var country in Countries)
            {
                currentCountry++;
                // Value for the progress bar
                progress.Report((currentCountry * 100) / totalCountries);
                // Small delay between the load of each country
                await Task.Delay(milisecondsDelay);
            }
        }

        /// <summary>
        /// Shows the selected country's information
        /// </summary>
        private void ShowCountryInfo()
        {
            Country selectedCountry = listBoxCountries.SelectedItem as Country;

            if (selectedCountry != null)
            {
                txtCountryName.Text = $"Name: {selectedCountry.Name.Common}";
                txtCca3.Text = $"Code: {selectedCountry.Cca3}";
                txtIndependent.Text = $"Independent: {selectedCountry.IndependentDisplay}";
                txtCapital.Text = $"Capital: {selectedCountry.CapitalsDisplay}";
                txtRegion.Text = $"Region: {selectedCountry.Region}";
                txtSubregion.Text = $"Subregion: {selectedCountry.Subregion}";
                txtPopulation.Text = $"Population: {selectedCountry.PopulationDisplay}";
                txtArea.Text = $"Area: {selectedCountry.AreaDisplay}";
                txtBorders.Text = $"Borders: {selectedCountry.BordersDisplay}";
                txtGini.Text = $"Gini index: {selectedCountry.GiniDisplay}";
                txtLanguages.Text = $"Languages: {selectedCountry.LanguagesDisplay}";
                txtCurrencies.Text = $"Currencies: {selectedCountry.CurrenciesDisplay}";

                LoadFlags(selectedCountry);

                LoadMapAsync(selectedCountry);
            }
        }

        /// <summary>
        /// Loads the selected country's google maps view
        /// </summary>
        /// <param name="selectedCountry"></param>
        private async void LoadMapAsync(Country selectedCountry)
        {
            var connection = networkService.CheckConnection();
            // If the connection to the internet is sucessfull
            if (connection.IsSucess)
            {
                // URL with Google Maps API Key and selected country's name
                string mapUrl = $"https://www.google.com/maps/embed/v1/place?key={GoogleMapsApiKey}&q={selectedCountry.Name.Common}";
                // Initializes WebView2 controller
                await webViewMap.EnsureCoreWebView2Async();
                // HTML code with iframe tag
                webViewMap.CoreWebView2.NavigateToString($@"
                    <html>
                        <head>
                            <meta http-equiv='X-UA-Compatible' content='IE=Edge'/>
                        </head>
                        <body style='margin:0;padding:0;overflow:hidden'>
                            <iframe width='100%' height='100%' frameborder='0' style='border:0' src='{mapUrl}' allowfullscreen></iframe>
                        </body>
                    </html>"
                );
            }
        }

        /// <summary>
        /// Loads the selected country's flag
        /// </summary>
        /// <param name="selectedCountry"></param>
        private void LoadFlags(Country selectedCountry)
        {
            var connection = networkService.CheckConnection();
            // If the connection to the internet is sucessfull
            if (connection.IsSucess)
            {
                Uri imageUri = new Uri(selectedCountry.Flags.Png);
                imageFlag.Source = new BitmapImage(imageUri);
            }
            else // If not
            {
                string localPath = @$"ImagesFlags\{selectedCountry.Name.Common}.png";
                // If the country has a flag, gets the selected country's flag from the directory
                if (!string.IsNullOrWhiteSpace(selectedCountry.Flags.Png))
                {
                    Uri uri = new Uri(Path.GetFullPath(localPath));
                    imageFlag.Source = new BitmapImage(uri);
                }
                else // If not, gets a null png from the directory
                {
                    Uri uri = new Uri(Path.GetFullPath(@$"ImagesFlags\Null.png"));
                    imageFlag.Source = new BitmapImage(uri);
                }
            }
        }

        private void listBoxCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowCountryInfo();
        }
    }
}