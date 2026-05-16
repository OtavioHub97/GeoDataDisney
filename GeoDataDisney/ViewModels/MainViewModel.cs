using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GeoDataDisney.Models;
using GeoDataDisney.Services;

namespace GeoDataDisney.ViewModels
{
    public class MainViewModel
    {
        private readonly DisneyService _disneyService;

        public ObservableCollection<Character> Characters { get; set; }

        /// <summary>
        ///  Propriedade do Comando
        /// </summary>
        public ICommand ShowDetailsCommand { get; }

        public MainViewModel()
        {
            _disneyService = new DisneyService();
            Characters = new ObservableCollection<Character>();

            /// Avisa que o comando vai disparar o método ShowDetails
            ShowDetailsCommand = new RelayCommand(ShowDetails);

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            DisneyResponse? response = await _disneyService.GetCharactersAsync();

            if (response != null && response.Data != null)
            {
                Characters.Clear();
                foreach (var character in response.Data)
                {
                    Characters.Add(character);
                }
            }
        }

        /// <summary>
        ///  O método que roda quando clica no botão
        /// </summary>
        /// <param name="parameter"></param>
        private void ShowDetails(object parameter)
        {
            /// Verificamos se o parâmetro que o botão mandou é realmente um Personagem
            if (parameter is Character character)
            {
                /// Formata as listas. Se vier vazio, escreve "Nenhum"
                string filmes = character.Films != null && character.Films.Count > 0 ? string.Join(", ", character.Films) : "Nenhum";
                string series = character.TvShows != null && character.TvShows.Count > 0 ? string.Join(", ", character.TvShows) : "Nenhuma";

                MessageBox.Show($"Filmes: {filmes}\n\nSéries de TV: {series}",
                                $"Detalhes de {character.Name}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }
    }
}