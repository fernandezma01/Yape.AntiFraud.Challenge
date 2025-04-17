using Reto.Tecnico.Yape.Models;

namespace Reto.Tecnico.Yape.Transactions.Providers;

public interface ITransactionsProvider
{
    Task<Transaction?> GetTransaction(Guid externalTransactionId);
}
