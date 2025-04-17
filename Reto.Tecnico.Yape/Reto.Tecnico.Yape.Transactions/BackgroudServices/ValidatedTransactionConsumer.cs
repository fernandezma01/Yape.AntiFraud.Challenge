using Confluent.Kafka;
using Newtonsoft.Json;
using Reto.Tecnico.Yape.Models;
using Reto.Tecnico.Yape.Transactions.Procressors;

namespace Reto.Tecnico.Yape.Transactions.BackgroudServices;

public class ValidatedTransactionConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ValidatedTransactionConsumer> _logger;
    private readonly ConsumerConfig _config;

    public ValidatedTransactionConsumer(
               IServiceProvider serviceProvider,
               ConsumerConfig config,
               ILogger<ValidatedTransactionConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        while (!stoppingToken.IsCancellationRequested)
        {
            using var consumer = new ConsumerBuilder<Null, string>(_config)
                        .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                        .Build();

            consumer.Subscribe("validated-transactions-topic"); // Ideally this value should be configured in a config file/ env variable
            try
            {
                while (true)
                {
                    var message = consumer.Consume(stoppingToken);

                    var statusUpdate = JsonConvert.DeserializeObject<TransactionStatusUpdate>(message.Message.Value);
                    _logger.LogDebug($"Received validation status for transaction: {statusUpdate!.ExternalTransactionID}");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var _transactionsProcessor = scope.ServiceProvider.GetRequiredService<ITransactionsProcessor>();
                        await _transactionsProcessor.UpdateTransaction(statusUpdate.ExternalTransactionID,
                            statusUpdate.ValidationSucceeded ? TransactionStatus.Approved : TransactionStatus.Rejected);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}

