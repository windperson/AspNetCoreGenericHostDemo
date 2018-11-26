using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GenericHostDemo.HostedServices
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> _logger;
        private Timer _timer;

        public TimedHostedService(ILogger<TimedHostedService> logger)
        {
            _logger = logger;
        }

#pragma warning disable CS1998
        public async Task StartAsync(CancellationToken cancellationToken)
#pragma warning restore CS1998
        {
            _logger.LogInformation("Timed background service is starting");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        private void DoWork(object state)
        {
            _logger.LogInformation($"Timed background service is working, Now: {DateTime.Now.ToLongTimeString()}");
        }

#pragma warning disable 1998
        public async Task StopAsync(CancellationToken cancellationToken)
#pragma warning restore 1998
        {
            _logger.LogInformation("Timed background service is stopping");

            _timer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
