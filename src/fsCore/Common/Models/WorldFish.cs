using System.Text.Json.Serialization;

namespace Common.Models
{
    public class WorldFish : BaseRuntime
    {
        public string Taxocode { get; set; }
        public string? ScientificName { get; set; }
        public int? Isscaap { get; set; }
        public string? A3Code { get; set; }
        public string? EnglishName { get; set; }
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