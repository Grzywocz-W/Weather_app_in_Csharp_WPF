using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; // Potrzebne do deserializacji JSON

namespace Aplikacja_pogodowa
{
    class Open_Wheather
    {
        private const string ApiKey = "PASTE HERE YOUR API KEY";
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";
        private const string ForecastUrl = "https://api.openweathermap.org/data/2.5/forecast";
        //private const string MapUrl = "https://tile.openweathermap.org/map/";
        private readonly HttpClient _client;

        public Open_Wheather()
        {
            _client = new HttpClient();
        }

        //Metoda do generowania mapy - brak wersji premium u mnie
        /*public string GetWeatherMap(string layer, double lat, double lon)
        {
            return $"{MapUrl}{layer}/{7}/{lat}/{lon}.png?appid={ApiKey}";
        }*/

        public async Task<WeatherData?> GetWeatherAsync(string city)
        {
            try
            {
                // Wysyłamy asynchroniczne żądanie GET do API
                HttpResponseMessage response = await _client.GetAsync($"{BaseUrl}?q={city}&appid={ApiKey}&units=metric&lang=pl");

                // Sprawdzenie, czy odpowiedź jest poprawna (status 200 OK)
                response.EnsureSuccessStatusCode();

                // Pobranie treści odpowiedzi jako string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserializacja JSON do obiektu WeatherData
                WeatherData? weatherData = JsonConvert.DeserializeObject<WeatherData>(responseBody);

                return weatherData;
            }
            catch (HttpRequestException ex)
            {
                // Obsługa błędu HTTP
                Console.WriteLine($"Błąd podczas pobierania danych: {ex.Message}");
                return null;
            }
        }


        //metoda do pobierania prognozy
        public async Task<WeatherForecastData?> GetWeatherForecastAsync(string city)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync($"{ForecastUrl}?q={city}&appid={ApiKey}&units=metric&lang=pl");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                WeatherForecastData? forecastData = JsonConvert.DeserializeObject<WeatherForecastData>(responseBody);

                return forecastData;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Błąd podczas pobierania prognozy: {ex.Message}");
                return null;
            }
        }
    }


    public class WeatherData
    {
        [JsonProperty("main")]
        public MainData? Main { get; set; }

        [JsonProperty("weather")]
        public WeatherInfo[]? Weather { get; set; }
        
        [JsonProperty("coord")]
        public Coord? Coord { get; set; }
    }

    public class MainData
    {
        [JsonProperty("temp")]
        public float Temperature { get; set; } // Lepsza nazwa niż "Temp"

        [JsonProperty("humidity")]
        public float Humidity { get; set; }

        [JsonProperty("pressure")]
        public float Pressure {  get; set; }
    }

    public class WeatherInfo
    {
        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("icon")]
        public string? Icon { get; set;}
    }

    
    public class Coord       //Potrzebne do mapy
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }  // Szerokość geograficzna

        [JsonProperty("lon")]
        public double Lon { get; set; }  // Długość geograficzna
    }


    
    public class WeatherForecastData
    {
        [JsonProperty("list")]
        public List<ForecastItem> ForecastItems { get; set; } = new List<ForecastItem>();
    }

    public class ForecastItem
    {
        [JsonProperty("dt_txt")]
        public string DateTime { get; set; }

        [JsonProperty("main")]
        public MainData Main { get; set; }

        [JsonProperty("weather")]
        public WeatherInfo[]? Weather { get; set; }
    }
}
