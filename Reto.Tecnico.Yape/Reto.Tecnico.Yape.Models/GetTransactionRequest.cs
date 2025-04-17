namespace Reto.Tecnico.Yape.Models;

public class GetTransactionRequest
{
    public Guid TransactionExternalId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public TransactionStatus Status { get; set; }
}
