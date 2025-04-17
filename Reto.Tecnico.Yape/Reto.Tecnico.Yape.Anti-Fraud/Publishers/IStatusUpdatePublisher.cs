using Reto.Tecnico.Yape.Models;

namespace Reto.Tecnico.Yape.Transactions.Publishers;

public interface IStatusUpdatePublisher
{
    Task Publish(TransactionStatusUpdate transaction, CancellationToken cancellationToken = default);
}
