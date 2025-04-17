namespace Reto.Tecnico.Yape.Models;

public class TransactionStatusUpdate
{
    public Guid ExternalTransactionID { get; set; }

    public bool ValidationSucceeded { get; set; }
}
