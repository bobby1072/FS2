using Common.Utils;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Models.Validators
{
    public static class ModelValidatorsServiceCollectionExtensions
    {
        public static IServiceCollection AddModelValidators(this IServiceCollection services)
        {
            services
                .AddSingleton<IValidator<GroupCatchComment>, GroupCatchCommentValidator>()
                .AddSingleton<IValidator<GroupCatch>, GroupCatchValidator>()
                .AddSingleton<IValidator<Group>, GroupValidator>()
                .AddSingleton<IValidator<GroupPosition>, GroupPositionValidator>()
                .AddSingleton<IValidator<User>, UserValidator>()
                .AddSingleton<IValidator<SpecificSpeciesLiveMatchCatchRule>, SpecificSpeciesMatchCatchRuleValidator>()
                .AddSingleton<IValidator<InAreaLiveMatchCatchRule>, InAreaLiveMatchCatchRuleValidator>()
                .AddSingleton<IValidator<LiveMatch>, LiveMatchDIValidator>()
                .AddSingleton<IValidator<LiveMatchCatch>, LiveMatchCatchValidator>()
                .AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IValidator<LiveMatchCatch>>().CreateEnumerableValidator());

            return services;
        }
    }
}