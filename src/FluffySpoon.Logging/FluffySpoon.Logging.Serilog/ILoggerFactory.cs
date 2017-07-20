using Serilog;

namespace FluffySpoon.Logging.Serilog
{
    public interface ILoggerFactory
    {
        ILogger CreateWithProductName(string productName);
    }
}