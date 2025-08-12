using System.Text;
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
    public partial class MainWindow : Window
    {
        private WeatherViewModel _weatherViewModel;

        public MainWindow()
        {
            InitializeComponent();
            _weatherViewModel = new WeatherViewModel(this);
            CityComboBox.ItemsSource = new List<string>
            {
                "--WYBIERZ--","Londyn","Paryż","Berlin", "Rzym", "Madryt", "Warszawa", "Praga", "Wiedeń",
                "Budapeszt", "Ateny", "Amsterdam", "Bruksela", "Oslo", "Kopenhaga", "Helsinki"
            };
            CityComboBox.SelectedItem = "--WYBIERZ--";
        }


        //Metoda api_querry przypisana do Buttonu Zatwierdź
        private async void api_querry(object sender, RoutedEventArgs e)
        {
            await _weatherViewModel.ApiQuery();   // Wywołanie metody z nowej klasy
        }

        //Metoda clear_text przypisana do Buttonu Wyczyść która czyści ComboBoxa i pole tekstowe
        private async void clear_text(object sender, RoutedEventArgs e)
        {
            CityComboBox.SelectedItem = "--WYBIERZ--";
            CityTextBox.Text = "";
        }

    }
}