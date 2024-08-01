using FluentValidation;

namespace Common.Models.Validators
{
    public class SpecificSpeciesMatchCatchRuleValidator : BaseValidator<SpecificSpeciesLiveMatchCatchRule>
    {
        public SpecificSpeciesMatchCatchRuleValidator()
        {
            RuleFor(x => x).Must(WorldFishListReflectsSpeciesNameList).WithMessage(LiveMatchConstants.WorldFishListMustReflectSpeciesNameList);
            RuleFor(x => x.SpeciesNames).NotEmpty().WithMessage(LiveMatchConstants.AtLeastOneSpeciesForRule);
            RuleFor(x => x.SpeciesNames).Must(AllSpeciesNamesAreUnique).WithMessage(LiveMatchConstants.SpeciesInListArentUnique);
            RuleFor(x => x.SpeciesNames).Must(AllSpeciesNamesNotEmpty).WithMessage(ErrorConstants.InvalidSpeciesInCatch);
        }
        private static bool AllSpeciesNamesAreUnique(ICollection<string> speciesNames)
        {
            return speciesNames.Distinct().Count() == speciesNames.Count;
        }
        private static bool AllSpeciesNamesNotEmpty(ICollection<string> speciesNames)
        {
            return speciesNames.All(LettersAndWhiteSpaceOnly);
        }
        private static bool WorldFishListReflectsSpeciesNameList(SpecificSpeciesLiveMatchCatchRule rule)
        {
            if (rule.WorldFish.Count == 0) return true;
            return rule.WorldFish.Select(x => x.EnglishName).SequenceEqual(rule.SpeciesNames);
        }
    }
}