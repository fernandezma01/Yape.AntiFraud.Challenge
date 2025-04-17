
using Reto.Tecnico.Yape.Models;

namespace Reto.Tecnico.Yape.Anti_Fraud.Processors;

public interface IValidationProcessor
{
    Task ValidateTransaction(Transaction transaction, CancellationToken cancellationToken = default);
}
