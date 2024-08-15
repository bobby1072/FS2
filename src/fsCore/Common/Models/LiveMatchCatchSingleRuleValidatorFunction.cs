namespace Common.Models
{
    public class LiveMatchCatchSingleRuleValidatorFunction
    {
        public Func<LiveMatchCatch, bool> ValidatorFunctionForSingle { get; set; }
        public Func<IEnumerable<LiveMatchCatch>, bool> ValidatorFunctionForList { get; set; }
        public string ErrorMessage { get; set; }
        public LiveMatchCatchSingleRuleValidatorFunction(Func<LiveMatchCatch, bool> validatorFunctionForSingle, Func<IEnumerable<LiveMatchCatch>, bool> validatorFunctionForDouble, string errorMessage)
        {
            ValidatorFunctionForSingle = validatorFunctionForSingle;
            ValidatorFunctionForList = validatorFunctionForDouble;
            ErrorMessage = errorMessage;
        }
    }
}