namespace LTA.API.Infrastructure.Loggers.Interfaces;

public interface ILoggerService
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogDebug(string message);
    void LogError(string message);
}