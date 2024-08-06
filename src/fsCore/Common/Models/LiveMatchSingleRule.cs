namespace Common.Models
{
    public abstract class LiveMatchSingleRule : BaseModel
    {
        public abstract string BuildRuleDescription();
        public abstract IList<LiveMatchCatchSingleRuleValidatorFunction> BuildRuleValidatorFunctions();
    }
}