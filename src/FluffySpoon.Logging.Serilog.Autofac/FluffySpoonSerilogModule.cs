using Autofac;
using AutofacSerilogIntegration;
using Serilog.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FluffySpoon.Logging.Serilog.Autofac
{
    public class FluffySpoonSerilogModule : Module
    {
        private readonly string _productName;

        public FluffySpoonSerilogModule(string productName)
        {
            _productName = productName;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                {
                    var factory = new LoggerFactory(
                        c.Resolve<IEnumerable<ILogEventSink>>(),
                        c.Resolve<IEnumerable<ILogEventEnricher>>(),
                        c.Resolve<IEnumerable<IDestructuringPolicy>>(),
                        c.Resolve<IEnumerable<ILogEventFilter>>());
                    factory.CreateWithProductName(_productName);

                    return factory;
                })
                .AutoActivate()
                .SingleInstance();

            builder.RegisterLogger();
        }
    }
}
