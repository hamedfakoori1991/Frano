using FluentValidation;
using VantageView.Contracts.CrawlerConfigurations;

namespace VantageView.RestApi.Validators;

public class SetupNewCrawlerConfigurationValidator : AbstractValidator<SetupNewCrawlerConfigurationRequest>
{
    public SetupNewCrawlerConfigurationValidator()
    {
        RuleFor(z => z.Title).NotEmpty();
    }
}
