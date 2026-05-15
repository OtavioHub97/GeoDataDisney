using System.Collections.Generic;
using System.Text.Json.Serialization; // Necessário para usar o [JsonPropertyName]

namespace GeoDataInsight.WPF.Models
{
    public class Character
    {
        [JsonPropertyName("_id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("films")]
        public List<string> Films { get; set; }

        [JsonPropertyName("shortFilms")]
        public List<string> ShortFilms { get; set; }

        [JsonPropertyName("tvShows")]
        public List<string> TvShows { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }

    }

    public class DisneyResponse
    {
        // Captura a lista principal que vem da API
        [JsonPropertyName("data")]
        public List<Character> Data { get; set; }
    }
}