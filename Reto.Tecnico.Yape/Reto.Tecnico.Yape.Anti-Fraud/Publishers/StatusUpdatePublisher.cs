using Confluent.Kafka;
using Newtonsoft.Json;
using Reto.Tecnico.Yape.Models;

namespace Reto.Tecnico.Yape.Transactions.Publishers;

public class StatusUpdatePublisher(
    ProducerConfig config,
    ILogger<StatusUpdatePublisher> logger) : IStatusUpdatePublisher
{
    private readonly ProducerConfig _config = config;
    private readonly ILogger<StatusUpdatePublisher> _logger = logger;

    public async Task Publish(TransactionStatusUpdate transaction, CancellationToken cancellationToken = default)
    {
        //Decided to serialize before sending to Kafka, to not struggle with schemas and schema registry
        var serializedTransaction = JsonConvert.SerializeObject(transaction);
        using (var producer = new ProducerBuilder<Null, string>(_config).Build())
        {
            var message = new Message<Null, string> { Value = serializedTransaction };
            // The topic name should be configured in a config file. For the purpose of the example, it is hardcoded.
            await producer.ProduceAsync("validated-transactions-topic", message, cancellationToken);
        }

        _logger.LogInformation($"Transaction {transaction.ExternalTransactionID} was valudated and published for update");
    }
}