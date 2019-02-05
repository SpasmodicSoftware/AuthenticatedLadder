using Microsoft.Extensions.Logging;
using System;

namespace AuthenticatedLadder.Logging
{
    public class LoggerAdapter<T> : ILoggerAdapter<T>
    {
        private ILogger<T> _logger;

        public LoggerAdapter(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, message, args);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }
    }
}
