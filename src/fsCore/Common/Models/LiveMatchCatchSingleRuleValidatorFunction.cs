namespace Common.Models
{
    public class LiveMatchCatchSingleRuleValidatorFunction
    {
        public Func<LiveMatchCatch, bool> ValidatorFunction { get; set; }
        public string ErrorMessage { get; set; }
        public LiveMatchCatchSingleRuleValidatorFunction(Func<LiveMatchCatch, bool> validatorFunction, string errorMessage)
        {
            ValidatorFunction = validatorFunction;
            ErrorMessage = errorMessage;
        }
    }
}