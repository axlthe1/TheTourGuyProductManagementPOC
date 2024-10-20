using TheTourGuy.Interfaces;

namespace TheTourGuy.Interfaces;

public interface IRabbitMqService
{
    string RequestQueue { get; }
    string ReplyQueue { get; }
    bool Configured { get; }
    Task ConnectExternalSearcher(IExternalRepository repository);
}