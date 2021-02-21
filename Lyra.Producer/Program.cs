using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lyra.Contracts;
using MassTransit;

namespace Lyra.Producer
{
    public static class Program
    {
        private const string RabbitHost = "localhost";
        private const string RabbitUserName = "admin";
        private const string RabbitPassword = "admin";
        private const int CustomerCount = 10;
        private const int CustomerEventCount = 10;

        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(busConfig =>
            {
                busConfig.Host(RabbitHost, "/", hostConfig =>
                {
                    hostConfig.Username(RabbitUserName);
                    hostConfig.Password(RabbitPassword);
                });
            });

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);

            var events = GenerateEvents();
            foreach (var @event in events)
            {
                await busControl.Publish<CustomerChangedEvent>(@event);

                Console.Out.WriteLine($"Published {@event.CustomerId:00}:{@event.Balance:000} v{@event.TargetEntityVersion:00}");
            }

            Console.WriteLine("DONE");
        }

        private static IEnumerable<CustomerChangedEvent> GenerateEvents()
        {
            var rnd = new Random();
            var events = new List<CustomerChangedEvent>();
            var eventCount = CustomerCount * CustomerEventCount;

            while (events.Count < eventCount)
            {
                var customerId = rnd.Next(1, CustomerCount + 1);
                var customerEventCount = events.Count(x => x.CustomerId == customerId);
                if (customerEventCount >= CustomerEventCount)
                {
                    continue;
                }

                var maxBalance = 0;
                if (customerEventCount > 0)
                {
                    maxBalance = events.Where(x => x.CustomerId == customerId).Max(x => x.Balance);
                }

                events.Add(new CustomerChangedEvent
                {
                    CustomerId = customerId,
                    Balance = maxBalance + 10,
                    TargetEntityVersion = customerEventCount,
                });
            }

            return events;
        }
    }
}
