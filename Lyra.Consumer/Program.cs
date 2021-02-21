using System;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;

namespace Lyra.Consumer
{
    public static class Program
    {
        private const string RabbitHost = "localhost";
        private const string RabbitUserName = "admin";
        private const string RabbitPassword = "admin";

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
                    //endpointConfig.SetQueueArgument("x-single-active-consumer", true);
                    //endpointConfig.UseConcurrencyLimit(1);

                    endpointConfig.Consumer<CustomerChangedEventConsumer>();
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
