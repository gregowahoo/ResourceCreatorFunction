using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.ClientModel;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ResourceCreatorFunction.Data;


var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

#region Chat Client Configuration

// Load configuration
var configuration = builder.Configuration;

#region MyRegion
Console.WriteLine($"AI:Key: {configuration["AI:Key"]}");

var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogInformation($"AI:Key: {configuration["AI:Key"]}"); 
#endregion


// Configure AzureOpenAIClient
var innerChatClientAzure = new AzureOpenAIClient(
    new Uri(configuration["AI:Endpoint"] ?? throw new InvalidOperationException("Missing AI:Endpoint")),
    new ApiKeyCredential(configuration["AI:Key"] ?? throw new InvalidOperationException("Missing AI:Key")))
    .AsChatClient("gpt-35-turbo");

var innerChatClientGithub = new AzureOpenAIClient(
    new Uri(configuration["AI:Endpoint"] ?? throw new InvalidOperationException("Missing AI:Endpoint")),
    new ApiKeyCredential(configuration["AI:Key"] ?? throw new InvalidOperationException("Missing AI:Key")))
    .AsChatClient("gpt-35-turbo");

var innerChatClientOpenAI = new AzureOpenAIClient(
    new Uri(configuration["AI:Endpoint"] ?? throw new InvalidOperationException("Missing AI:Endpoint")),
    new ApiKeyCredential(configuration["AI:Key"] ?? throw new InvalidOperationException("Missing AI:Key")))
    .AsChatClient("gpt-35-turbo");

//builder.Services.AddChatClient(innerChatClientAzure); // Azure-based GPT-3.5
builder.Services.AddChatClient(innerChatClientGithub); // Azure-based GPT-3.5
//builder.Services.AddChatClient(innerChatClientOpenAI); // “gpt-4o-mini” from OpenAI
#endregion

// Add DbContext with SQL Server
builder.Services.AddDbContext<ResourceCreatorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Build().Run();
