using System;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;

namespace Lyra.Consumer
{
    public static class Program
    {
        public const string SqlConnectionString = "Server=.;Initial Catalog=LyraConsumer;Integrated Security=True;";
        public const string RabbitHost = "localhost";
        public const string RabbitUserName = "admin";
        public const string RabbitPassword = "admin";

        public static async Task Main()
        {
            Console.Out.WriteLine("Lyra Consumer started");
            Console.Out.WriteLine("Press Enters to stop...");

            var busControl = Bus.Factory.CreateUsingRabbitMq(busConfig =>
            {
                busConfig.Host(RabbitHost, "/", hostConfig =>
                {
                    hostConfig.Username(RabbitUserName);
                    hostConfig.Password(RabbitPassword);
                });

                busConfig.ReceiveEndpoint("lyra-consumer-events", endpointConfig =>
                {
                    endpointConfig.Consumer<CustomerChangedEventConsumer>();

                    endpointConfig.UseMessageRetry(retryConfig =>
                    {
                        retryConfig.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                    });
                });
            });

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);
            try
            {
                await Task.Run(() => Console.ReadLine());
            }
            finally
            {
                await busControl.StopAsync();
            }

            Console.Out.WriteLine("DONE");
        }
    }
}
