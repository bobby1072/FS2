using System.Text.Json.Serialization;

namespace Common.Models
{
    public class WorldFish : BaseModel
    {
        [JsonPropertyName("taxocode")]
        public string Taxocode { get; set; }
        [JsonPropertyName("scientificName")]
        public string? ScientificName { get; set; }
        [JsonPropertyName("isscaap")]
        public int? Isscaap { get; set; }
        [JsonPropertyName("a3Code")]
        public string? A3Code { get; set; }
        [JsonPropertyName("englishName")]
        public string? EnglishName { get; set; }
        [JsonPropertyName("nickname")]
        public string? Nickname { get; set; }
        [JsonConstructor]
        public WorldFish(string taxocode, int? isscaap, string? a3Code, string? scientificName, string? englishName, string? nickName)
        {
            Taxocode = taxocode;
            Isscaap = isscaap;
            A3Code = a3Code;
            ScientificName = scientificName;
            EnglishName = englishName;
            Nickname = nickName;
        }
    }
}