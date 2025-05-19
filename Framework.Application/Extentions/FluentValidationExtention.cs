using System.Linq.Expressions;
using System.Text.RegularExpressions;
using FluentValidation;
using Framework.Domain.Entities;
using Framework.Domain.Messages;

namespace Framework.Application.Extentions;

public static class FluentValidationRegexConstant
{
    public const string ValidLabelRegex = @"^(?=.{0,150}$)([a-zA-Z]([a-zA-Z0-9_ ]*[a-zA-Z0-9_])*)?$";
    public const string ValidNameRegex = @"^(?=.{0,150}$)([a-zA-Z][a-zA-Z0-9_]*)?$";
}

public static class FluentValidationExtention
{
    public static readonly Regex ValidLabelRegex = new Regex(
        FluentValidationRegexConstant.ValidLabelRegex,
        RegexOptions.Compiled
    );
    public static readonly Regex ValidNameRegex = new Regex(
        FluentValidationRegexConstant.ValidNameRegex,
        RegexOptions.Compiled
    );
    public static IRuleBuilderOptions<T, TProperty> WithMessageAndCode<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        Message message,
        params Expression<Func<T, string>>[] valueSelectors)
    {
        rule.Configure(config =>
        {
            config.MessageBuilder = ctx =>
            {
                var values = valueSelectors.Select(selector => selector.Compile().Invoke(ctx.InstanceToValidate)).ToArray();
                return message.WithParams(values.ToArray()).Text;
            };
            config.Current.ErrorCode = message.Code;
        });

        return rule;
    }

    public static IRuleBuilderOptions<T, string?> IsValidLabel<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(value => ValidLabelRegex.IsMatch(value)
        ).WithMessage((t, value) => GeneralMessages.InvalidLabelChars().WithParams("{PropertyName}", value).Text);
    }

    public static IRuleBuilderOptions<T, string?> IsValidName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(value => ValidNameRegex.IsMatch(value)
        ).WithMessage((t, value) => GeneralMessages.InvalidNameChars().WithParams("{PropertyName}", value).Text);
    }

}
