using System;
using System.Threading.Tasks;
using Dapper;
using Lyra.Contracts;
using MassTransit;
using Microsoft.Data.SqlClient;

namespace Lyra.Consumer
{
    public class CustomerChangedEventConsumer : IConsumer<CustomerChangedEvent>
    {
        private const string ConnectionString = "Server=.;Initial Catalog=LyraConsumer;Integrated Security=True;";

        private static readonly Random Rnd = new Random();

        public async Task Consume(ConsumeContext<CustomerChangedEvent> context)
        {
            var description = $"{context.Message.CustomerId:00}:{context.Message.Balance:000}";
            Console.Out.WriteLine($"Consumer {description} started.");

            await Task.Delay(Rnd.Next(50, 500));

            try
            {
                using var connection = new SqlConnection(ConnectionString);
                await connection.OpenAsync();

                var updatedCount = await connection.ExecuteAsync(
                    "update [Customers] set [Balance] = @Balance where [Id] = @Id",
                    new { Id = context.Message.CustomerId, Balance = context.Message.Balance });

                if (updatedCount <= 0)
                {
                    await connection.ExecuteAsync(
                        "insert into [Customers] ([Id], [Balance]) values (@Id, @Balance)",
                        new { Id = context.Message.CustomerId, Balance = context.Message.Balance });
                }
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"Consumer {description}: {ex.Message}");
                throw;
            }

            Console.Out.WriteLine($"Consumer {description} completed.");
        }
    }
}
