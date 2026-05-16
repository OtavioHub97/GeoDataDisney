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
    /// ViewModel principal responsável por gerenciar a lógica de apresentação da MainWindow.
    /// Intermedeia a comunicação entre a UI (View) e os serviços de dados (Model/Services).
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DisneyService _disneyService;
        private readonly SyncService _syncService; /// Serviço de persistência e integração

        /// <summary>
        /// Coleção observável de personagens. Qualquer alteração nesta lista reflete automaticamente na UI.
        /// </summary>
        public ObservableCollection<Character> Characters { get; set; }

        /// <summary>
        /// Comandos da Interface
        /// </summary>
        public ICommand ShowDetailsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        /// <summary>
        /// Comando responsável por disparar a sincronização dos dados com o Firebase e a API externa.
        /// </summary>
        public ICommand SyncCommand { get; }

        private string? _statusMessage;
        /// <summary>
        /// Mensagem de status exibida para o usuário.
        /// </summary>
        public string? StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(nameof(StatusMessage)); }
        }

        private string _searchText = "";
        /// <summary>
        /// Texto vinculado a barra de pesquisa da interface.
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(nameof(SearchText)); }
        }

        private int _currentPage = 1;
        /// <summary>
        /// indice da página atual de paginação da API.
        /// </summary>
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(PageDisplay));
            }
        }

        /// <summary>
        /// Texto formatado para exibição do número da página atual na interface.
        /// </summary>
        public string PageDisplay => $"Página {CurrentPage}";

        /// <summary>
        /// Inicializa os serviços, comandos e carrega a primeira página de dados.
        /// </summary>
        public MainViewModel()
        {
            _disneyService = new DisneyService();
            _syncService = new SyncService(); // Inicialização do novo serviço
            Characters = new ObservableCollection<Character>();

            // Mapeamento dos Comandos
            ShowDetailsCommand = new RelayCommand(ShowDetails);
            SearchCommand = new RelayCommand(_ => RealizarPesquisa());
            NextPageCommand = new RelayCommand(_ => ProximaPagina());
            PreviousPageCommand = new RelayCommand(_ => PaginaAnterior());
            SyncCommand = new RelayCommand(SyncCharacter); 

            _ = LoadDataAsync();
        }

        /// <summary>
        /// Reinicia a paginação e busca personagens com base no texto de pesquisa.
        /// </summary>
        private void RealizarPesquisa()
        {
            CurrentPage = 1;
            _ = LoadDataAsync();
        }

        /// <summary>
        /// Avança para a próxima página de personagens na API da Disney.
        /// </summary>
        private void ProximaPagina()
        {
            CurrentPage++;
            _ = LoadDataAsync();
        }

        /// <summary>
        /// Retorna para a página anterior de personagens, impedindo navegação para páginas inválidas menores que 1.
        /// </summary>
        private void PaginaAnterior()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                _ = LoadDataAsync();
            }
        }

        /// <summary>
        /// Método assíncrono que consome a API da Disney e atualiza a coleção de personagens na tela.
        /// </summary>
        private async Task LoadDataAsync()
        {
            StatusMessage = "Buscando personagens... ⏳";
            Characters.Clear();

            DisneyResponse? response = await _disneyService.GetCharactersAsync(CurrentPage, SearchText);

            if (response != null && response.Data != null)
            {
                foreach (var character in response.Data)
                {
                    Characters.Add(character);
                }

                if (Characters.Count == 0)
                {
                    StatusMessage = "Nenhum personagem encontrado.";
                }
                else
                {
                    StatusMessage = "";
                }
            }
            else
            {
                StatusMessage = "Falha ao buscar os dados. Verifique a conexão.";
            }
        }

        /// <summary>
        /// Exibe uma caixa de diálogo com as mídias Filmes e Séries, associadas ao personagem.
        /// </summary>
        /// <param name="parameter">O objeto Character selecionado pelo usuário.</param>
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

        /// <summary>
        /// Envia o personagem selecionado para o Firebase e para a API da colega.
        /// Fornece feedback visual sobre o sucesso ou falha da operação.
        /// </summary>
        /// <param name="parameter">O objeto Character a ser sincronizado.</param>
        private async void SyncCharacter(object parameter)
        {
            if (parameter is Character character)
            {
                StatusMessage = $"Sincronizando {character.Name}";

                /// Executa a persistência através do serviço
                bool salvouFirebase = await _syncService.SalvarNoFirebaseAsync(character);
                bool enviouColega = await _syncService.EnviarParaColegaAsync(character);

                /// Feedback visual para o usuário
                if (salvouFirebase && enviouColega)
                {
                    MessageBox.Show($"{character.Name} foi salvo no Firebase e enviado para sua colega com sucesso!",
                                    "Sucesso na Integração", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    string msgErro = "Ocorreu um problema de sincronização:\n";
                    if (!salvouFirebase) msgErro += "- Falha ao salvar no Firebase.\n";
                    if (!enviouColega) msgErro += "- Falha ao enviar para o endpoint externo.\n";

                    MessageBox.Show(msgErro, "Aviso de Integração", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                StatusMessage = "";
            }
        }

        /// Implementação do INotifyPropertyChanged 
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifica a interface de que uma propriedade sofreu alteração.
        /// </summary>
        /// <param name="propertyName">Nome da propriedade alterada.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}