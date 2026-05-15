using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GeoDataDisney.Models;
using GeoDataDisney.Services;

namespace GeoDataDisney.ViewModels
{
    public class MainViewModel
    {
        /// <summary>
        /// Conexão com o nosso serviço que busca os dados
        /// </summary>
        private readonly DisneyService _disneyService;

        /// <summary>
        /// A lista que vai avisar a tela quando os personagens chegarem
        /// </summary>
        public ObservableCollection<Character> Characters { get; set; }

        public MainViewModel()
        {
            _disneyService = new DisneyService();
            Characters = new ObservableCollection<Character>();

            /// Dispara a busca dos personagens na internet
            /// Usamos o _ = para o C# não reclamar que não estamos esperando a tarefa terminar na mesma linha
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            /// Pede para o Service ir buscar os dados
            DisneyResponse? response = await _disneyService.GetCharactersAsync();

           
            if (response != null && response.Data != null)
            {
                /// Limpa a lista por garantia e adiciona os personagens um a um
                Characters.Clear();

                foreach (var character in response.Data)
                {
                    Characters.Add(character);
                }
            }
        }
    }
}