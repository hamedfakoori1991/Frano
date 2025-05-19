using Framework.Application.Interfaces;
using Framework.DataAccess.MongoDb.MongoDb;
using VantageView.Application.Repositories;
using VantageView.Domain.CrawlerConfigurations;

namespace VantageView.Infrastructure.Repositories;

public class CrawlerConfigurationRepository : BaseRepository<CrawlerConfiguration, string>, ICrawlerConfigurationRepository
{
    public CrawlerConfigurationRepository(IDbContext context) : base(context)
    {
    }
}
