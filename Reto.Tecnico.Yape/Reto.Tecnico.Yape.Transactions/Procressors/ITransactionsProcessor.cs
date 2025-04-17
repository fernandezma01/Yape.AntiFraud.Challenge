using Reto.Tecnico.Yape.Models;

namespace Reto.Tecnico.Yape.Transactions.Procressors;

public interface ITransactionsProcessor
{
    Task<Guid> CreateTransaction(CreateTransactionRequest request);

    Task UpdateTransaction(Guid transactionId, TransactionStatus status);
}
