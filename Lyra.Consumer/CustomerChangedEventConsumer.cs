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
        private static readonly Random Rnd = new Random();

        public async Task Consume(ConsumeContext<CustomerChangedEvent> context)
        {
            var description = $"{context.Message.CustomerId:00}:{context.Message.Balance:000} {context.Message.Timestamp:HH:mm:ss:fff}";
            Console.Out.WriteLine($"Consumer {description} started.");

            await Task.Delay(Rnd.Next(50, 500));

            try
            {
                using var connection = new SqlConnection(Program.SqlConnectionString);
                await connection.OpenAsync();

                var updatedCount = await connection.ExecuteAsync(
@"update [Customers] 
set [Balance] = @Balance, [Timestamp] = @Timestamp
where [Id] = @Id and [Timestamp] <= @Timestamp",
                    new 
                    { 
                        Id = context.Message.CustomerId, 
                        Balance = context.Message.Balance,
                        Timestamp = context.Message.Timestamp,
                    });

                Console.Out.WriteLine($"Consumer {description} {(updatedCount > 0 ? "completed" : "SKIPPED")}.");
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"Consumer {description}: {ex.Message}");
                throw;
            }
        }
    }
}
