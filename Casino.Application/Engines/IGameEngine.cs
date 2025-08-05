namespace Casino.Application.Engines;

public interface IGameEngine
{
    Task StartAsync();
    Task StopAsync();
}