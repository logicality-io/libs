using System;
using System.Threading;
using System.Threading.Tasks;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace Logicality.Extensions.Hosting.Example
{
    public class MySqlHostedService : DockerHostedService
    {
        private readonly ExampleContext _context;
        private const string SAPassword = "E@syP@ssw0rd";
        private const int HostPort = 3306;

        public MySqlHostedService(ExampleContext context, ILogger<DockerHostedService> logger, bool leaveRunning = false)
            : base(logger, leaveRunning)
        {
            _context = context;
        }

        public MySqlConnectionStringBuilder CreateConnectionStringBuilder(string? database = null) =>
            new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                Port = HostPort,
                UserID = "root",
                Password = SAPassword,
                Database = database
            };

        protected override string ContainerName => "extensions-mysel";

        protected override IContainerService CreateContainerService()
         => new Builder()
             .UseContainer()
             .WithName(ContainerName)
             .UseImage("mysql/mysql-server:5.6")
             .WithEnvironment($"MYSQL_ROOT_PASSWORD={SAPassword}", "MYSQL_ROOT_HOST=%")
             .ReuseIfExists()
             .ExposePort(HostPort, 3306)
             .Wait("profiles-sql-server", (service, retryCount) =>
             {
                 if (retryCount > 60)
                 {
                     throw new Exception("");
                 }

                 using (var connection = new MySqlConnection(CreateConnectionStringBuilder().ConnectionString))
                 {
                     try
                     {
                         connection.Open();
                         return 0;
                     }
                     catch (Exception)
                     {
                         return 1000; //How long to wait before retrying
                     }
                 }
             })
             .Build();

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);

            _context.MySql = this;
        }
    }
}