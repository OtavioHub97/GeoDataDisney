using GeoDataDisney.Models;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GeoDataDisney.Services
{
    public class DisneyService
    {
        /// <summary>
        /// O HttpClient é a ferramenta do C# para acessar URLs.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Esta é a URL da API da Disney que retorna os personagens
        /// </summary>
        private readonly string _apiUrl = "https://api.disneyapi.dev/character";

        public DisneyService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Método assíncrono (Task) para que a tela do WPF não congele enquanto a internet carrega os dados
        /// </summary>
        /// <returns></returns>
        public async Task<DisneyResponse> GetCharactersAsync()
        {
            try
            {
                /// Vai até a URL e "puxa" a resposta do site
                HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl);

                /// Garante que o site respondeu com sucesso (Código 200 OK)
                response.EnsureSuccessStatusCode();

                /// Lê o corpo da resposta (o texto do JSON)
                string jsonString = await response.Content.ReadAsStringAsync();

                /// Desserializa o texto JSON para a nossa classe DisneyResponse
                DisneyResponse disneyData = JsonSerializer.Deserialize<DisneyResponse>(jsonString);

                return disneyData;
            }
            catch (Exception ex)
            {
                /// Se der algum erro (ex: sem internet), ele retorna nulo para não quebrar o aplicativo
                Console.WriteLine("Erro ao buscar dados: " + ex.Message);
                return null;
            }
        }
    }
}