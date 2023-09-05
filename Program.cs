using Coding_Exercise.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//add Kafka configuration to config loaded from KafkaConfig.json
builder.Configuration
.AddJsonFile("KafkaConfig.json", optional: false)
.Build();



//load kafka config into services configuration
builder.Services.Configure<KafkaConfig>(builder.Configuration.GetSection("KafkaConfig"));

//Register Weather Consumer hosted service, kafka config loaded into service through dependency injection
builder.Services.AddHostedService<WeatherConsumer>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
