using MassTransit;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using ToDoListQuery.Consumer;
using System.Text;
using ToDoListQuery.Services;
using ToDoListQuery.Interface;
using Newtonsoft.Json;
using ToDoListQuery.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

static async Task<CosmosDBConsumerService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
{
    var databaseName = configurationSection["DatabaseName"];
    var containerName = configurationSection["ContainerName"];
    var account = configurationSection["Account"];
    var key = configurationSection["Key"];
    var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
    var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
    var cosmosDbService = new CosmosDBConsumerService(client, databaseName, containerName);
    return cosmosDbService;
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

var section = builder.Configuration.GetSection("CosmosDb");
builder.Services.AddSingleton<ICosmosDBConsumer>(
    InitializeCosmosClientInstanceAsync(section).GetAwaiter().GetResult());

#region RabbitMQ receiver
//Here we specify the Rabbit MQ Server. we use rabbitmq docker image and use it
var factory = new ConnectionFactory
{
    HostName = "localhost"
};

//Create the RabbitMQ connection using connection factory details as i mentioned above
var connection = factory.CreateConnection();

//Here we create channel with session and model
using var channel = connection.CreateModel();

//declare the queue after mentioning name and a few property related to that
channel.QueueDeclare("todoList", exclusive: false);

//Set Event object which listen message from chanel which is sent by producer
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"todoList message received: {message}");
    var section = builder.Configuration.GetSection("CosmosDb");
    var databaseName = section["DatabaseName"];
    var containerName = section["ContainerName"];
    var account = section["Account"];
    var key = section["Key"];
    var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
    CosmosDBConsumerService obj = new CosmosDBConsumerService(client, databaseName, containerName);
    try
    {
        var result = JsonConvert.DeserializeObject<Item>(message);
        obj.UpsertItemAsync(result);
    }
    catch (JsonSerializationException)
    {
        var result = JsonConvert.DeserializeObject<string>(message);
        obj.DeleteAsync(result);
    }
   
    //if (result is Item)
    //{
    //    obj.UpsertItemAsync(result);
    //}
    //else
    //{
    //    obj.DeleteAsync(result.ToString());
    //}
};

//read the message
channel.BasicConsume(queue: "todoList", autoAck: true, consumer: consumer);
#endregion

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
