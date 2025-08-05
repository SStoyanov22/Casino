using Microsoft.Extensions.Logging;
using Casino.Infrastructure.Interfaces;
using Casino.Core.Constants;

namespace Casino.Application.Engines;

public class GameEngine : IGameEngine
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<GameEngine> _logger;

    public GameEngine(IApplicationService applicationService, ILogger<GameEngine> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    public async Task StartAsync()
    {
        _logger.LogInformation(LogMessages.GameEngineStarting);
        await _applicationService.RunAsync();
    }

    public async Task StopAsync()
    {
        _logger.LogInformation(LogMessages.GameEngineStopping);
        await Task.CompletedTask;
    }
}