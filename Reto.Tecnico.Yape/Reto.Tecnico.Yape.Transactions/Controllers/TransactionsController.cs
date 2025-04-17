using Microsoft.AspNetCore.Mvc;
using Reto.Tecnico.Yape.Models;
using Reto.Tecnico.Yape.Transactions.Procressors;
using Reto.Tecnico.Yape.Transactions.Providers;

namespace Reto.Tecnico.Yape.Transactions.Controllers;

[Route("api/transactions")]
[ApiController]
public class TransactionsController(
    ITransactionsProcessor transactionsProcessor,
    ITransactionsProvider transactionsProvider,
    ILogger<TransactionsController> logger) : ControllerBase
{
    private readonly ITransactionsProcessor _transactionsProcessor = transactionsProcessor;
    private readonly ITransactionsProvider _transactionsProvider = transactionsProvider;
    private readonly ILogger<TransactionsController> _logger = logger;

    [HttpGet("{externalTransactionId}")]
    public async Task<IActionResult> Get(string externalTransactionId)
    {
        if (!Guid.TryParse(externalTransactionId, out var transactionId))
        {
            return BadRequest("Invalid transaction ID");
        }

        var transaction = await _transactionsProvider.GetTransaction(transactionId);

        return Ok(transaction);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateTransactionRequest value)
    {
        try
        {
            var created = await _transactionsProcessor.CreateTransaction(value);
            return Created(this.Request.Path, created);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            return StatusCode(500, "Internal server error");
        }

    }
}
