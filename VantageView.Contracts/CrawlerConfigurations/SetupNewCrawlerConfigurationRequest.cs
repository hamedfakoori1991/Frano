namespace VantageView.Contracts.CrawlerConfigurations;

public record SetupNewCrawlerConfigurationRequest(string Title);
public record UpdateCrawlerConfigurationRequest(string Id, string Title);
public record DeleteCrawlerConfigurationRequest(string Id);
public record CrawlerConfigurationResponse(string Id, string Title);
