using System;

namespace AuthenticatedLadder.Logging
{
    public interface ILoggerAdapter<T>
    {
        void LogError(Exception ex, string message, params object[] args);
        void LogWarning(string message);
    }
}
