using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aplikacja_pogodowa
{
    public class WeatherViewModel
    {
        private readonly Open_Wheather _ow;
        private readonly MainWindow _mainWindow;

        public WeatherViewModel(MainWindow mainWindow)
        {
            _ow = new Open_Wheather();
            _mainWindow = mainWindow;
        }

        //Metoda api_querry przypisana do Buttonu Zatwierdź
        //Pobranie danych od użytkownika i wywołanie metody "PobierzIPokazPogode" oraz metody "PobierzPrognoze" która wysyła zapytanie do API i wyświetla odpowiedz
        public async Task ApiQuery()
        {
            ComboBox CityComboBox=_mainWindow.CityComboBox;
            TextBox CityTextBox=_mainWindow.CityTextBox;
            TextBlock wyswietlacz=_mainWindow.wyswietlacz;
            StackPanel StackPanelForecast =_mainWindow.StackPanelForecast;

            string city = "";

            if (CityComboBox.SelectedItem != null && CityComboBox.SelectedItem.ToString() != "--WYBIERZ--")
            {
                city = CityComboBox.SelectedItem.ToString();
                CityTextBox.Text = "";                              //wyczyszczenie TextBoxa
            }
            else if (!string.IsNullOrWhiteSpace(CityTextBox.Text))
            {
                city = CityTextBox.Text;
                CityComboBox.SelectedItem = "--WYBIERZ--";                   //wyczyszczenie ComboBoxa
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                wyswietlacz.Text = "Wpisz lub wybierz miasto!";
                return;
            }

            await PobierzIPokazPogode(city);
            await PobierzPrognoze(city);

            StackPanelForecast.Visibility = Visibility.Visible;
        }



        //Wykonanie zapytania do Api i wyświetlenie danych
        private async Task PobierzIPokazPogode(string city)
        {
            _mainWindow.Loader.Visibility = Visibility.Visible;  //Pasek ladowania

            WeatherData? weatherData = await _ow.GetWeatherAsync(city);     //zapytanie do API

            if (weatherData != null)
            {
                string? description = weatherData.Weather?[0]?.Description?.ToLower();

                //Dobranie tła
                if (description != null)
                {
                    if (description.Contains("bezchmurn") || description.Contains("słonecz"))
                        (_mainWindow.FindResource("SunnyBackground") as Storyboard)?.Begin();
                    else if (description.Contains("chmur") || description.Contains("pochmur") || description.Contains("mg"))
                        (_mainWindow.FindResource("CloudyBackground") as Storyboard)?.Begin();
                    else if (description.Contains("deszcz") || description.Contains("śnieg") || description.Contains("burza") || description.Contains("opad"))
                        (_mainWindow.FindResource("RainyBackground") as Storyboard)?.Begin();
                }

                string desc = char.ToUpper(description[0]) + description.Substring(1);

                //Wyswietlenie danych na ekran
                string weatherInfo = $"Temperatura: {(int)weatherData?.Main?.Temperature}°C\n";
                weatherInfo += $"Ciśnienie: {weatherData?.Main?.Pressure} hPa\n";
                weatherInfo += $"Wilgotność: {weatherData?.Main?.Humidity}%\n";
                weatherInfo += $"Opis: {desc}";

                _mainWindow.CityLabel.Content = city.ToUpper();
                _mainWindow.wyswietlacz.Text = weatherInfo;

                //Wyswietlnie ikony z animacja
                if (!string.IsNullOrEmpty(weatherData.Weather?[0]?.Icon))
                {
                    string iconCode = weatherData.Weather[0].Icon;
                    string iconUrl = $"http://openweathermap.org/img/wn/{iconCode}@2x.png";
                    _mainWindow.WeatherIcon.Source = new BitmapImage(new Uri(iconUrl));

                    //Animacja ikony
                    _mainWindow.WeatherIcon.Opacity = 0;
                    var fadeIn = _mainWindow.FindResource("FadeInIconStoryboard") as Storyboard;
                    fadeIn?.Begin();
                }

                //Tworzenie mapy - ale w wersji premium
                /*string maplayer = "clouds_new";
                double lat = weatherData.Coord?.Lat ?? 0;
                double lon = weatherData.Coord?.Lon ?? 0;
                string mapUrl = _ow.GetWeatherMap(maplayer,lat,lon);      // Uzyskanie URL do mapy
                _mainWindow.WeatherMap.Source = new BitmapImage(new Uri(mapUrl));                // Ładowanie mapy do Image
                */
            }
            else
            {
                _mainWindow.wyswietlacz.Text = "Błąd pobierania danych pogodowych.";
                _mainWindow.WeatherIcon.Source = null;
                _mainWindow.WeatherMap.Source = null;
            }

            _mainWindow.Loader.Visibility = Visibility.Collapsed;

        }



        //Metoda do prognozy
        private async Task PobierzPrognoze(string city)
        {
            WeatherForecastData? forecastData = await _ow.GetWeatherForecastAsync(city);

            if (forecastData != null)
            {
                var forecastList = new List<ForecastViewModel>();

                foreach (var item in forecastData.ForecastItems)
                {
                    var iconCode = item.Weather?[0]?.Icon;
                    var iconUrl = $"http://openweathermap.org/img/wn/{iconCode}@2x.png";
                    BitmapImage icon = new BitmapImage(new Uri(iconUrl));
                    var temperature = $"Temp. : {(int)item.Main?.Temperature}°C";
                    var pressure = $"Ciśn. : {item.Main?.Pressure} hPa";
                    string? description = item.Weather[0].Description;


                    forecastList.Add(new ForecastViewModel
                    {
                        DateTime = item.DateTime,
                        Icon = icon,
                        Temperature = temperature,
                        Pressure = pressure,
                        Description = char.ToUpper(description[0]) + description.Substring(1)
                    });
                }

                _mainWindow.ForecastListBox.ItemsSource = forecastList;
            }
            else
            {
                _mainWindow.wyswietlacz.Text = "Błąd pobierania prognozy.";
                _mainWindow.ForecastListBox.ItemsSource = null;
            }
        }
    
    }

    
    
    //element prognozy (item)
    public class ForecastViewModel
    {
        public string DateTime { get; set; } = string.Empty;
        public BitmapImage Icon { get; set; }
        public string Temperature { get; set; } = string.Empty;
        public string Pressure { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }


}


//Metoda przypisana do SelectionChange ComboBoxa pozwalająca na automatyczne wysyłanie zapytania do API po wybraniu elementu z ComboBoxa 
//Nie używana ze względu na ograniczoną ilość zapytań do API
/*private async void CityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
{
    if (CityComboBox.SelectedItem is string selectedCity) 
    {
        CityTextBox.Text = "";
        await PobierzIPokazPogode(selectedCity);
    }
}*/


