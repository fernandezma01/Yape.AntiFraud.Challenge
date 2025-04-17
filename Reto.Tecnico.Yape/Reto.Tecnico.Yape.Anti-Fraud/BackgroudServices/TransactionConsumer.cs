using Confluent.Kafka;
using Newtonsoft.Json;
using Reto.Tecnico.Yape.Anti_Fraud.Processors;
using Reto.Tecnico.Yape.Models;

namespace Reto.Tecnico.Yape.Anti_Fraud.BackgroudServices;

public class TransactionConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TransactionConsumer> _logger;
    private readonly ConsumerConfig _config;

    public TransactionConsumer(
               IServiceProvider serviceProvider,
               ConsumerConfig config,
               ILogger<TransactionConsumer> logger)
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

            consumer.Subscribe("transactions-validation"); // Ideally this value should be configured in a config file/ env variable
            try
            {
                while (true)
                {
                    var message = consumer.Consume(stoppingToken);
                    var transaction = JsonConvert.DeserializeObject<Transaction>(message.Message.Value);
                    var _validationProcessor = _serviceProvider.GetRequiredService<IValidationProcessor>();
                    _logger.LogDebug($"Received validation status for transaction: {transaction!.ExternalTransactionID}");
                    var validation = message.Message.Value;
                    await _validationProcessor.ValidateTransaction(transaction, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}

