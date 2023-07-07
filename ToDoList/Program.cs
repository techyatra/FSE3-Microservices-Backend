
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using ToDoList.Interfaces;
using ToDoList.RabitMQ;
using ToDoList.Services;

var builder = WebApplication.CreateBuilder(args);
//Start adding Congiguration for cosmos db
static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
{
    var databaseName = configurationSection["DatabaseName"];
    var containerName = configurationSection["ContainerName"];
    var account = configurationSection["Account"];
    var key = configurationSection["Key"];
    var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
    var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
    var cosmosDbService = new CosmosDbService(client, databaseName, containerName);
    return cosmosDbService;
}

var section = builder.Configuration.GetSection("CosmosDb");
builder.Services.AddSingleton<ICosmosDbService>(
    InitializeCosmosClientInstanceAsync(section).GetAwaiter().GetResult());

//end
// Add services to the container.
builder.Services.AddScoped<IRabitMQProducer, RabitMQProducer>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
});

builder.Services.AddFluentValidation(conf =>
{
    conf.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
    conf.AutomaticValidationEnabled = false;
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


