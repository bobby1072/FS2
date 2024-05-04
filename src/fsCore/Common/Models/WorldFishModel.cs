using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Common.Models
{
    public class WorldFish : BaseModel
    {
        [JsonPropertyName("taxocode")]
        public string Taxocode { get; set; }
        [JsonPropertyName("scientificName")]
        public string? ScientificName { get; set; }
        [JsonPropertyName("isscaap")]
        public string? Isscaap { get; set; }
        [JsonPropertyName("a3Code")]
        public string? A3Code { get; set; }
        [JsonPropertyName("englishName")]
        public string? EnglishName { get; set; }
        [JsonPropertyName("nickname")]
        public string? Nickname { get; set; }
        [JsonConstructor]
        public WorldFish(string taxocode, string? isscaap, string? a3Code, string? scientificName, string? englishName, string? nickname)
        {
            Taxocode = taxocode;
            Isscaap = isscaap;
            A3Code = a3Code;
            ScientificName = scientificName;
            EnglishName = TrimEnglishName(englishName);
            if (nickname == null && englishName != null)
            {
                Nickname = GetNickname(englishName);
            }
            else
            {
                Nickname = nickname;
            }
        }
        private static string? TrimEnglishName(string? englishName)
        {
            if (englishName is null) return null;
            var fishRegexPattern = @"\(([^)]*)\)";
            var myRegex = new Regex(fishRegexPattern, RegexOptions.IgnoreCase);
            var regexMatch = myRegex.Match(englishName);
            return regexMatch.Success ? englishName.Replace(regexMatch.Value, " ") : englishName;
        }
        private static string? GetNickname(string engName)
        {
            var fishRegexPattern = @"\(([^)]*)\)";
            var myRegex = new Regex(fishRegexPattern, RegexOptions.IgnoreCase);
            var regexMatch = myRegex.Match(engName);
            if (regexMatch.Success)
            {
                var aka = regexMatch.Value.Replace("(=", "").Replace(")", "").Replace("(", "").Replace("=", " ");
                return aka;
            }
            return null;
        }
    }
    public class JsonFileWorldFish
    {
        [JsonPropertyName("taxocode")]
        public string Taxocode { get; set; }

        [JsonPropertyName("scientific_name")]
        public string? ScientificName { get; set; }

        private string? _isscaap;
        [JsonPropertyName("isscaap")]
        public object? Isscaap
        {
            get => _isscaap;
            set
            {
                _isscaap = value?.ToString();
            }
        }


        [JsonPropertyName("a3_code")]
        public string? A3Code { get; set; }

        [JsonPropertyName("english_name")]
        public string? EnglishName { get; set; }

        public WorldFish ToWorldFishRegular()
        {
            return new WorldFish(Taxocode, _isscaap, A3Code, ScientificName, EnglishName, null);
        }
    }

}