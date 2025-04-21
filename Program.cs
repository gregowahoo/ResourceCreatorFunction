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
using WiseUpDude.Services;
using WiseUpDude.Data.Repositories;
using WiseUpDude.Data;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Load configuration
var configuration = builder.Configuration;

#region MyRegion
Console.WriteLine($"AI:Key: {configuration["AI:Key"]}");

var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogInformation($"AI:Key: {configuration["AI:Key"]}");
#endregion


// Chat client setup for Azure, GitHub, and OpenAI
var innerChatClientAzure = new AzureOpenAIClient(
    new Uri(configuration["AI:Endpoint"] ?? throw new InvalidOperationException("Missing AI:Endpoint")),
    new ApiKeyCredential(configuration["AI:Key"] ?? throw new InvalidOperationException("Missing AI:Key")))
    .AsChatClient("gpt-35-turbo");

var innerChatClientGithub = new AzureOpenAIClient(
    new Uri(configuration["GithubAI:Endpoint"] ?? throw new InvalidOperationException("Missing GithubAI:Endpoint")),
    new ApiKeyCredential(configuration["GithubAI:Key"] ?? throw new InvalidOperationException("Missing GithubAI:Key")))
    .AsChatClient("gpt-4o");

var innerChatClientOpenAI = new OpenAI.Chat.ChatClient("gpt-4o-mini",
    configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("Missing OpenAI:ApiKey"))
    .AsChatClient();


//Do not remove commentting as may need to change to a different model.
//builder.Services.AddChatClient(innerChatClientAzure); // Azure-based GPT-3.5
builder.Services.AddChatClient(innerChatClientGithub); // Azure-based GPT-3.5
//builder.Services.AddChatClient(innerChatClientOpenAI); // �gpt-4o-mini� from OpenAI

// Register the LLM name as a configuration or service
builder.Services.AddSingleton(llmName => "gpt-4o"); // Replace with the actual LLM name being used

// Add DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

Console.WriteLine($"DefaultConnection: {builder.Configuration.GetConnectionString("DefaultConnection")}");
logger.LogInformation($"DefaultConnection: {builder.Configuration.GetConnectionString("DefaultConnection")}");

// Register Services
builder.Services.AddScoped<QuizTopicService>(); // Register QuizTopicService
builder.Services.AddScoped<TopicRepository>();  // Register TopicRepository
builder.Services.AddScoped<TopicCreationRunRepository>();  // Register TopicRepository

builder.Build().Run();
