namespace Common.Models
{
    public abstract class LiveMatchCatchSingleRule : BaseModel
    {
        public abstract string BuildRuleDescription();
        public abstract IList<LiveMatchCatchSingleRuleValidatorFunction> BuildRuleValidatorFunctions();
    }
}