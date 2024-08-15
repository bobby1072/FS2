namespace Common.Models
{
    public abstract class LiveMatchCatchSingleRule : BaseModel
    {
        public abstract string BuildRuleDescription();
        public abstract IList<(Func<LiveMatchCatch, bool> ValidatorFunctions, string ErrorMessage)> BuildMatchCatchRuleValidatorFunctions();
    }
}