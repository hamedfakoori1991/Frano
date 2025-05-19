using Framework.Application.Interfaces;
using VantageView.Domain.CrawlerConfigurations;

namespace VantageView.Application.Repositories;

public interface ICrawlerConfigurationRepository : IRepository<CrawlerConfiguration, string>
{
}

