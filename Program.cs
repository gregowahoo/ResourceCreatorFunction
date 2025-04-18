using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.ClientModel;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

#region Chat Client Configuration
// Chat client setup for Azure, GitHub, and OpenAI
var innerChatClientAzure = new AzureOpenAIClient(
    new Uri(builder.Configuration["AI:Endpoint"] ?? throw new InvalidOperationException("Missing AI:Endpoint")),
    new ApiKeyCredential(builder.Configuration["AI:Key"] ?? throw new InvalidOperationException("Missing AI:Key")))
    .AsChatClient("gpt-35-turbo");

var innerChatClientGithub = new AzureOpenAIClient(
    new Uri(builder.Configuration["GithubAI:Endpoint"] ?? throw new InvalidOperationException("Missing GithubAI:Endpoint")),
    new ApiKeyCredential(builder.Configuration["GithubAI:Key"] ?? throw new InvalidOperationException("Missing GithubAI:Key")))
    .AsChatClient("gpt-4o");

var innerChatClientOpenAI = new OpenAI.Chat.ChatClient("gpt-4o-mini",
    builder.Configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("Missing OpenAI:ApiKey"))
    .AsChatClient();

//builder.Services.AddChatClient(innerChatClientAzure); // Azure-based GPT-3.5
builder.Services.AddChatClient(innerChatClientGithub); // Azure-based GPT-3.5
//builder.Services.AddChatClient(innerChatClientOpenAI); // “gpt-4o-mini” from OpenAI
#endregion

builder.Build().Run();
