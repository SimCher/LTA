using LTA.API.Infrastructure.Loggers.Interfaces;
using NLog;

namespace LTA.API.Infrastructure.Loggers.Services;

public class LoggerService : ILoggerService
{
    private static ILogger _logger;

    public LoggerService()
    {
        _logger = LogManager.GetCurrentClassLogger();
    }

    public void LogInformation(string message)
    {
        _logger.Info(message);
    }

    public void LogWarning(string message)
    {
        _logger.Warn(message);
    }

    public void LogDebug(string message)
    {
        _logger.Debug(message);
    }

    public void LogError(string message)
    {
        _logger.Error(message);
    }
}