using Reto.Tecnico.Yape.Models;

namespace Reto.Tecnico.Yape.Transactions.Publishers;

public interface ITransactionsPublisher
{
    Task Publish(Transaction transaction, CancellationToken cancellationToken = default);
}
