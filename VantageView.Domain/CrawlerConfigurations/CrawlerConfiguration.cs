using Framework.Domain.Entities;

namespace VantageView.Domain.CrawlerConfigurations;

public class CrawlerConfiguration :BaseEntity<string>
{
    public string Title { get; set; }
}
