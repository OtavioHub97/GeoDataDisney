using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GeoDataDisney.Models;

namespace GeoDataDisney.Services
{
    /// <summary>
    /// Serviço responsável por gerenciar a sincronização e persistência de dados nas APIs externas.
    /// Lida com a gravação no Firebase Realtime Database e o envio de dados para o sistema parceiro.
    /// </summary>
    public class SyncService
    {
        private readonly HttpClient _httpClient;

        private readonly string _firebaseUrl = "";

        private readonly string _colegaApiUrl = "";

        /// <summary>
        /// Inicializa uma nova instância da classe SyncService configurando o cliente HTTP.
        /// </summary>
        public SyncService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Serializa o objeto Character e realiza uma requisição PUT para o Firebase Realtime Database.
        /// O uso do PUT com o ID do personagem garante que o registro seja criado ou atualizado.
        /// </summary>
        /// <param name="character">O objeto personagem que será salvo no banco de dados.</param>
        /// <returns>Retorna true se a requisição HTTP retornar um status de sucesso; caso contrário, false.</returns>
        public async Task<bool> SalvarNoFirebaseAsync(Character character)
        {
            try
            {
                /// Converte o personagem em texto JSON
                string jsonString = JsonSerializer.Serialize(character);
                StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                /// No Firebase REST API, usa o PUT com o ID para salvar ou atualizar o registro específico.
                string urlUnica = $"{_firebaseUrl}/{character.Id}.json";

                HttpResponseMessage response = await _httpClient.PutAsync(urlUnica, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Serializa o objeto Character e realiza uma requisição POST para o endpoint da API externa.
        /// </summary>
        /// <param name="character">O objeto personagem que será enviado para a API.</param>
        /// <returns>Retorna true se a requisição HTTP retornar um status de sucesso; caso contrário, false.</returns>
        public async Task<bool> EnviarParaColegaAsync(Character character)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(character);
                StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                /// Envia um POST para o endpoint configurado
                HttpResponseMessage response = await _httpClient.PostAsync(_colegaApiUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}