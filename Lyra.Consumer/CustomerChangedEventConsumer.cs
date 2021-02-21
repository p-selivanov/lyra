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
            var description = $"{context.Message.CustomerId:00}:{context.Message.Balance:000}";
            Console.Out.WriteLine($"Consumer {description} started.");

            await Task.Delay(Rnd.Next(50, 500));

            int updatedCount = 0;
            try
            {
                using var connection = new SqlConnection(Program.SqlConnectionString);
                await connection.OpenAsync();

                updatedCount = await connection.ExecuteAsync(
@"update [Customers] 
set [Balance] = @Balance, [Version] = @Version + 1
where [Id] = @Id and [Version] = @Version",
                    new 
                    { 
                        Id = context.Message.CustomerId, 
                        Balance = context.Message.Balance, 
                        Version = context.Message.TargetEntityVersion 
                    });
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"Consumer {description}: {ex.Message}");
                throw;
            }

            if (updatedCount <= 0)
            {
                Console.Error.WriteLine($"Consumer {description} RETRY.");
                throw new Exception();
            }

            Console.Out.WriteLine($"Consumer {description} completed. Try count {context.GetRetryCount() + 1}");
        }
    }
}
