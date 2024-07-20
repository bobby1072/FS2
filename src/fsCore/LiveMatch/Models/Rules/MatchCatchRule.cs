
namespace LiveMatch.Models.Rules
{
    public abstract class MatchCatchRule
    {
        public virtual RuleType RuleType { get; set; }
        public abstract bool ValidateCatch(MatchCatch groupCatch);
    }
}