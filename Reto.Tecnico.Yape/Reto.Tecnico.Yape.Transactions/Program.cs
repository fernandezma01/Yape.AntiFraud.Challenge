using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Reto.Tecnico.Yape.Data;
using Reto.Tecnico.Yape.Transactions.BackgroudServices;
using Reto.Tecnico.Yape.Transactions.Procressors;
using Reto.Tecnico.Yape.Transactions.Providers;
using Reto.Tecnico.Yape.Transactions.Publishers;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        $"Host={builder.Configuration["DbContext:Host"]};Port={builder.Configuration["DbContext:Port"]};Database={builder.Configuration["DbContext:Database"]};Username={builder.Configuration["DbContext:UserName"]};Password={builder.Configuration["DbContext:Password"]};"));
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddScoped<ITransactionsProcessor, TransactionsProcessor>();
builder.Services.AddScoped<ITransactionsProvider, TransactionsProvider>();
builder.Services.AddSingleton(new ConsumerConfig
{
    BootstrapServers = builder.Configuration["Messaging:Consumer_Host"],
    GroupId = builder.Configuration["Messaging:Consumer_GroupId"],
    AutoOffsetReset = AutoOffsetReset.Earliest,
});

builder.Services.AddSingleton(new ProducerConfig
{
    BootstrapServers = builder.Configuration["Messaging:Producer_Host"],
});
builder.Services.AddSingleton<ITransactionsPublisher, TransactionsPublisher>();
builder.Services.AddHostedService<ValidatedTransactionConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
