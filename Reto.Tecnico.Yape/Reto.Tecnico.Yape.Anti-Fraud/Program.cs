using Confluent.Kafka;
using Reto.Tecnico.Yape.Anti_Fraud.BackgroudServices;
using Reto.Tecnico.Yape.Anti_Fraud.Processors;
using Reto.Tecnico.Yape.Transactions.Publishers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IValidationProcessor, ValidationProcessor>();
builder.Services.AddSingleton(new ProducerConfig
{
    BootstrapServers = builder.Configuration["Messaging:Producer_Host"],
});
builder.Services.AddSingleton<IStatusUpdatePublisher, StatusUpdatePublisher>();
builder.Services.AddSingleton(new ConsumerConfig
{
    BootstrapServers = builder.Configuration["Messaging:Consumer_Host"],
    GroupId = builder.Configuration["Messaging:Consumer_GroupId"],
    AutoOffsetReset = AutoOffsetReset.Earliest,
});
builder.Services.AddHostedService<TransactionConsumer>();
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
