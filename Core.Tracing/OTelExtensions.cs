using Core.Tracing.Domain;
using Core.Tracing.Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reactive;

namespace Core.Tracing
{
    public static class OTelExtensions
    {
        public static IServiceCollection AddOTelIntegration(this IServiceCollection services, IConfiguration configuration, Action<OpenTelemetryOptions>? openTelemetryOptions = null)
        {
            var options = configuration.GetSection(nameof(OpenTelemetryOptions)).Get<OpenTelemetryOptions>();

            if (options is null || !options.Enabled)
                return services;

            services.AddOptions<OpenTelemetryOptions>().Bind(configuration.GetSection(nameof(OpenTelemetryOptions))).ValidateDataAnnotations();

            services.AddOpenTelemetry().WithTracing(builder =>
            {
                openTelemetryOptions?.Invoke(options);

                ConfigureSampler(builder, options);
                ConfigureInstrumentation(builder);
                ConfigureExporters(builder, options);

                if (options.Services is not null)
                {
                    foreach (var service in options.Services)
                    {
                        builder.SetResourceBuilder(resourceBuilder: ResourceBuilder.CreateDefault().AddService(service));
                    }
                }


            });
            return services;
        }


        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (onNext == null) throw new ArgumentNullException(nameof(onNext));

            return source.Subscribe(new AnonymousObserver<T>(onNext));
        }

        private static TextMapPropagator GetMapPropagator()
        {
            var propagators = new List<TextMapPropagator>
        {
            new TraceContextPropagator(),
            new BaggagePropagator(),
        };

            return new CompositeTextMapPropagator(propagators);
        }

        private static void ConfigureSampler(TracerProviderBuilder builder, OpenTelemetryOptions options)
        {
            if (options.AlwaysOnSampler)
            {
                builder.SetSampler(new AlwaysOnSampler());
            }
        }

        private static void ConfigureInstrumentation(TracerProviderBuilder builder)
        {
            Sdk.SetDefaultTextMapPropagator(GetMapPropagator());

            builder.AddAspNetCoreInstrumentation()
                   .AddHttpClientInstrumentation()
                   .AddSource(OtelMediatrOptions.OtelMediatrName)
                   .AddSource(OtelDomainOptions.OtelEventHandlerName)
                   .AddRabbitMQInstrumentation();
        }

        private static void ConfigureExporters(TracerProviderBuilder builder, OpenTelemetryOptions options)
        {
            builder.AddOtlpExporter(o =>
            {
                o.Endpoint = options.JaegerOptions.Endpoint;
                o.Protocol = options.JaegerOptions.Endpoint.ToString().Contains("4318")
                         ? OtlpExportProtocol.HttpProtobuf
                         : OtlpExportProtocol.Grpc;
            });
        }
    }
}
