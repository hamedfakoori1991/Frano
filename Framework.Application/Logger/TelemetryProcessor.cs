using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Framework.Infrastructure.Logger;

public class TelemetryProcessor : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next;
    private readonly string[] _exclusions = { "health" };

    public TelemetryProcessor(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        if (FilterItem(item))
            return;
        _next.Process(item);
    }

    private bool FilterItem(ITelemetry item)
    {
        switch (item)
        {
            case RequestTelemetry telemetry:
                {
                    var request = telemetry;
                    if (_exclusions.Any(n => request.Name.ToLower().Contains(n)))
                    {
                        return true;
                    }

                    break;
                }
            case DependencyTelemetry telemetry:
                {
                    var dep = telemetry;

                    if (_exclusions.Any(n => dep.Name.ToLower().Contains(n)))
                    {
                        return true;
                    }

                    break;
                }
        }

        return false;
    }
}