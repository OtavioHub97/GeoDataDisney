using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GeoDataDisney.Models;
using GeoDataDisney.Services;

namespace GeoDataDisney.ViewModels
{
    /// <summary>
    ///  Adiciona a interface INotifyPropertyChanged
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DisneyService _disneyService;

        public ObservableCollection<Character> Characters { get; set; }
        public ICommand ShowDetailsCommand { get; }

        /// <summary>
        /// Cria uma propriedade privada para guardar o texto
        /// </summary>
        private string? _statusMessage;

        /// <summary>
        ///  Cria a propriedade pública que a tela View vai ler
        /// </summary>
        public string? StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage)); // Avisa a tela que o texto mudou!
            }
        }

        public MainViewModel()
        {
            _disneyService = new DisneyService();
            Characters = new ObservableCollection<Character>();
            ShowDetailsCommand = new RelayCommand(ShowDetails);

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            /// Mostra a mensagem ANTES de ir na internet
            StatusMessage = "Buscando personagens na Disney...";

            DisneyResponse? response = await _disneyService.GetCharactersAsync();

            if (response != null && response.Data != null)
            {
                Characters.Clear();
                foreach (var character in response.Data)
                {
                    Characters.Add(character);
                }

                /// Limpa a mensagem quando os dados chegam
                StatusMessage = "";
            }
            else
            {
                /// Se der erro ex: sem internet, avisa ao usuário!
                StatusMessage = "Falha ao buscar os dados. Verifique sua conexão. ❌";
            }
        }

        private void ShowDetails(object parameter)
        {
            if (parameter is Character character)
            {
                string filmes = character.Films != null && character.Films.Count > 0 ? string.Join(", ", character.Films) : "Nenhum";
                string series = character.TvShows != null && character.TvShows.Count > 0 ? string.Join(", ", character.TvShows) : "Nenhuma";

                MessageBox.Show($"Filmes: {filmes}\n\nSéries de TV: {series}",
                                $"Detalhes de {character.Name}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}