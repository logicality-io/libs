using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Logicality.Extensions.Hosting
{
    public class ExampleHostedService: IHostedService
    {
        private readonly Context _context;

        public ExampleHostedService(Context context)
        {
            this._context = context;
        }

        public bool OnStartCalled { get; set; }

        public bool OnStopCalled { get; set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            OnStartCalled = true;
            _context.Increment();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            OnStopCalled = true;
            return Task.CompletedTask;
        }
    }
}
