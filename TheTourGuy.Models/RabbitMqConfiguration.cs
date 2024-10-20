namespace TheTourGuy.Models;

public class RabbitMqConfiguration
{
    public string Host { get; set; } = "rabbitmq";
    public string User { get; set; } = "user";
    public string Password { get; set; } = "password";
    public int TimeoutMilliseconds { get; set; } = 50000;
}