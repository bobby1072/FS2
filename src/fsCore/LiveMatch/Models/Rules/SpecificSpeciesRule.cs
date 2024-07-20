using Common.Models;

namespace LiveMatch.Models.Rules
{
    public class SpecificSpeciesRule : MatchCatchRule
    {
        public WorldFish? WorldFish { get; set; }
        public string? SpeciesName { get; set; }
        public new RuleType RuleType = RuleType.SpecificSpecies;
        public override bool ValidateCatch(MatchCatch matchCatch)
        {
            if (WorldFish is WorldFish notNullWorldFish)
            {
                if (matchCatch.WorldFish is WorldFish matchWorldFish)
                {

                    return matchWorldFish.Taxocode == notNullWorldFish.Taxocode;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return matchCatch.Species == SpeciesName;
            }
        }
    }
}