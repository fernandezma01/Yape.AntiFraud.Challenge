using Reto.Tecnico.Yape.Models;
using Reto.Tecnico.Yape.Transactions.Publishers;

namespace Reto.Tecnico.Yape.Anti_Fraud.Processors;

public class ValidationProcessor: IValidationProcessor
{
    private readonly ILogger<ValidationProcessor> _logger;
    private readonly IStatusUpdatePublisher _statusUpdatePublisher; 
    public ValidationProcessor(
        IStatusUpdatePublisher statusUpdatePublisher,
        ILogger<ValidationProcessor> logger)
    {
        _statusUpdatePublisher = statusUpdatePublisher;
        _logger = logger;
    }

    public async Task ValidateTransaction(Transaction transaction, CancellationToken cancellationToken = default)
    {
        //NOTE: This limits shoud be configured in a config file/ env variable
        //It is hardcoded for the sake of simplicity
        var validTransfer = (transaction.Value <= 2000 && transaction.DailyTransfer <= 20000);

        _logger.LogInformation($"Transaction {transaction.ExternalTransactionID} was validated with status: {validTransfer}");
        await _statusUpdatePublisher.Publish(new TransactionStatusUpdate
        {
            ExternalTransactionID = transaction.ExternalTransactionID,
            ValidationSucceeded = validTransfer
        }, cancellationToken);
    }
}
