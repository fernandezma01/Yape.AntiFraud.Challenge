using Newtonsoft.Json;

namespace Reto.Tecnico.Yape.Models;

public class Transaction(
    Guid sourceAccountId,
    Guid targetAccountId,
    decimal value
    )
{
    [JsonProperty("transactionID")]
    public Guid TransactionID { get; set; } = Guid.NewGuid();
    [JsonProperty("externalTransactionID")]
    public Guid ExternalTransactionID { get; set; } = Guid.NewGuid();
    [JsonProperty("sourceAccountId")]
    public Guid SourceAccountId { get; set; } = sourceAccountId;
    [JsonProperty("targetAccountId")]
    public Guid TargetAccountId { get; set; } = targetAccountId;
    [JsonProperty("transactionDate")]
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    [JsonProperty("lastUpdate")]
    public DateTime LastUpdate { get; set; } = DateTime.Now;
    [JsonProperty("status")]
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    [JsonProperty("value")]
    public decimal Value { get; set; } = value;
    [JsonProperty("dailyTransfer")]
    public decimal DailyTransfer { get; set; }
}
