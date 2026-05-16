using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GeoDataDisney.Models;

namespace GeoDataDisney.Services
{
    public class DisneyService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://api.disneyapi.dev/character";

        public DisneyService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Aceita a página e o texto da pesquisa como parâmetros
        /// </summary>
        /// <param name="page"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public async Task<DisneyResponse?> GetCharactersAsync(int page = 1, string searchText = "")
        {
            try
            {
                /// Inicia a URL já pedindo a página específica
                string url = $"{_apiUrl}?page={page}";

                /// Se o usuário digitou algo na busca, adiciona o filtro de nome
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    /// O Uri.EscapeDataString transforma espaços em "%20" (padrão de internet)
                    url += $"&name={Uri.EscapeDataString(searchText.Trim())}";
                }

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();
                DisneyResponse? disneyData = JsonSerializer.Deserialize<DisneyResponse>(jsonString);

                return disneyData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao buscar dados: " + ex.Message);
                return null;
            }
        }
    }
}