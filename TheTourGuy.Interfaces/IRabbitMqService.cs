using TheTourGuy.Interfaces;

namespace TheTourGuy.Interfaces;

public interface IRabbitMqService
{
    string RequestQueue { get; }
    bool Configured { get; }
    Task ConnectExternalSearcher(IExternalRepository repository);
}