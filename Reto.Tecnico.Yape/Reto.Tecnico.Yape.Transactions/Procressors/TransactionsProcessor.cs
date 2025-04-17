using Microsoft.EntityFrameworkCore;
using Reto.Tecnico.Yape.Data;
using Reto.Tecnico.Yape.Models;
using Reto.Tecnico.Yape.Transactions.Publishers;

namespace Reto.Tecnico.Yape.Transactions.Procressors;

public class TransactionsProcessor(
    ApplicationDbContext dbContext,
    ILogger<TransactionsProcessor> logger,
    ITransactionsPublisher transactionsPublisher
    ) : ITransactionsProcessor
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly ILogger<TransactionsProcessor> _logger = logger;
    private readonly ITransactionsPublisher _transactionsPublisher = transactionsPublisher;

    public async Task<Guid> CreateTransaction(CreateTransactionRequest request)
    {
        if (request.TranferTypeId != 1)
        { 
            throw new ArgumentException("Transfer Type not allowed");
        }

        if(request.Value <= 0)
        {
            throw new ArgumentException("The amount of the transfer must be greater than 0");
        }

        var dailyTransfer = await _dbContext.Transactions
            .Where(t => t.SourceAccountId == request.SourceAccountId && t.TransactionDate.Date == DateTime.UtcNow.Date)
            .SumAsync(t => t.Value);

        var transaction = new Transaction(request.SourceAccountId, request.TargetAccountId, request.Value);
        transaction.DailyTransfer = dailyTransfer + request.Value;

        _dbContext.Set<Transaction>().Add(transaction);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Transaction created with ID: {TransactionId}", transaction.TransactionID);
        await _transactionsPublisher.Publish(transaction);
        return transaction.ExternalTransactionID;
    }

    public async Task UpdateTransaction(Guid transactionId, TransactionStatus status)
    {
        await _dbContext.Transactions
            .Where(t =>t.ExternalTransactionID == transactionId)
            .ExecuteUpdateAsync(setters=> setters.SetProperty(p => p.Status, status));
    }
}
