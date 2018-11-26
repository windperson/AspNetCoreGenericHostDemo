using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GenericHostDemo.HostedServices
{
    public class LifetimeEventsHostedService : IHostedService
    {
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly ILogger<LifetimeEventsHostedService> _logger;

        public LifetimeEventsHostedService(IApplicationLifetime applicationLifetime, ILogger<LifetimeEventsHostedService> logger)
        {
            _applicationLifetime = applicationLifetime;
            _logger = logger;
        }

#pragma warning disable 1998
        public async Task StartAsync(CancellationToken cancellationToken)
#pragma warning restore 1998
        {
            _logger.LogInformation("Call OnStart()");

            _applicationLifetime.ApplicationStarted.Register(OnStarted);
            _applicationLifetime.ApplicationStopping.Register(OnStopping);
            _applicationLifetime.ApplicationStopped.Register(OnStopped);
        }

#pragma warning disable 1998
        public async Task StopAsync(CancellationToken cancellationToken)
#pragma warning restore 1998
        {
            _logger.LogInformation("Call OnStop()");
        }

        private void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");

            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");

            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");

            // Perform post-stopped activities here
        }
    }
}