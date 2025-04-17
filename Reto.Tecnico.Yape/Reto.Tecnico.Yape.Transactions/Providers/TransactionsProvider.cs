using Microsoft.EntityFrameworkCore;
using Reto.Tecnico.Yape.Data;
using Reto.Tecnico.Yape.Models;

namespace Reto.Tecnico.Yape.Transactions.Providers;

public class TransactionsProvider(
    ApplicationDbContext dbContext) 
    : ITransactionsProvider
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    public async Task<Transaction?> GetTransaction(Guid externalTransactionId)
    {
        return await _dbContext.Transactions.FirstOrDefaultAsync(t => t.ExternalTransactionID == externalTransactionId);
    }
}
