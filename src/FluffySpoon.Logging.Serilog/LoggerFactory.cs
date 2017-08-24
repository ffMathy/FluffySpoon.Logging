using SelfLog = Serilog.Debugging.SelfLog;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;
using Destructurama;
using Serilog.Events;

namespace FluffySpoon.Logging.Serilog
{
    public class LoggerFactory : ILoggerFactory
    {
        private readonly IEnumerable<ILogEventFilter> _filters;
        private readonly IEnumerable<IDestructuringPolicy> _destructuringPolicies;
        private readonly IEnumerable<ILogEventEnricher> _enrichers;
        private readonly IEnumerable<ILogEventSink> _sinks;

        public LoggerFactory(
            IEnumerable<ILogEventSink> sinks,
            IEnumerable<ILogEventEnricher> enrichers,
            IEnumerable<IDestructuringPolicy> destructuringPolicies,
            IEnumerable<ILogEventFilter> filters)
        {
            _sinks = sinks;
            _enrichers = enrichers;
            _destructuringPolicies = destructuringPolicies;
            _filters = filters;
        }

        public ILogger CreateWithProductName(string productName)
        {
            var configuration = new LoggerConfiguration();
            SelfLog.Enable(message =>
            {
                var finalMessage = "FluffySpoon.Logging.Serilog: " + message;
                Debug.WriteLine(finalMessage);
            });

            configuration = SetupSettings(configuration, productName);
            configuration = ConfigureFilters(configuration);
            configuration = SetupDestructuringPolicies(configuration);
            configuration = SetupSinks(configuration);
            configuration = ConfigureEnrichment(configuration);
            
            var logger = configuration.CreateLogger();
            Log.Logger = logger;

            return logger;
        }

        private LoggerConfiguration ConfigureEnrichment(
            LoggerConfiguration configuration)
        {
            configuration = configuration.Enrich.FromLogContext();
            configuration = configuration.Enrich.With(_enrichers.ToArray());

            return configuration;
        }

        private LoggerConfiguration ConfigureFilters(
            LoggerConfiguration configuration)
        {
            configuration = configuration.Filter.With(_filters.ToArray());
            return configuration;
        }

        private LoggerConfiguration SetupSinks(
            LoggerConfiguration configuration)
        {
            configuration = configuration.WriteTo.Console();
            foreach (var sink in _sinks)
            {
                configuration = configuration.WriteTo.Sink(sink);
            }
            return configuration;
        }

        private LoggerConfiguration SetupDestructuringPolicies(
            LoggerConfiguration configuration)
        {
            configuration = configuration.Destructure.ToMaximumDepth(2);
            configuration = configuration.Destructure.UsingAttributes();

            foreach (var destructuringPolicy in _destructuringPolicies)
            {
                configuration = configuration.Destructure.With(destructuringPolicy);
            }
            return configuration;
        }

        private static LoggerConfiguration SetupSettings(
            LoggerConfiguration configuration,
            string productName)
        {
            configuration = configuration.Enrich.WithProperty("ProductName", productName);
            configuration = configuration.MinimumLevel.Is(LogEventLevel.Verbose);
            return configuration;
        }
    }
}
