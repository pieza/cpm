using NLog;

namespace cpm.Utils
{
    static class Logger
    {
        public static readonly ILogger Instance = LogManager.GetCurrentClassLogger();
    }
}
