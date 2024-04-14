using Azure;
using Azure.AI.OpenAI;
using CopilotOnData.Services;
using CopilotOnData.Utils;
using Spectre.Console;
using System.Text;

// Get Endpoint
string openAiEndpoint = string.IsNullOrEmpty(Settings.OpenAiEndpoint)
    ? ConsoleService.GetUrl("Please insert your [yellow]Azure OpenAI endpoint[/]:")
    : Settings.OpenAiEndpoint;

// Get Azure OpenAI Key
string azureOpenAiKey = string.IsNullOrEmpty(Settings.OpenAiKey)
    ? ConsoleService.GetString("Please insert your [yellow]Azure OpenAI API key[/]:")
    : Settings.OpenAiKey;

// Get deployment name
string chatDeploymentName = string.IsNullOrEmpty(Settings.ChatDeploymentName)
    ? ConsoleService.GetString("Please insert the [yellow]deployment name of the chat model[/]:")
    : Settings.ChatDeploymentName;

// Get deployment name
string embeddingsDeploymentName = string.IsNullOrEmpty(Settings.EmbeddingDeploymentName)
    ? ConsoleService.GetString("Please insert the [yellow]deployment name of the embeddings model[/]:")
    : Settings.EmbeddingDeploymentName;

// Get Cosmos DB for MongoDB connection string
string cosmosDbConnectionString = string.IsNullOrEmpty(Settings.CosmosDbConnectionString)
    ? ConsoleService.GetString("Please insert the [yellow]Cosmos DB for MongoDB vCore[/] connection string:", false)
    : Settings.CosmosDbConnectionString;

// Get Search Index name
string cosmosDbIndexName = string.IsNullOrEmpty(Settings.CosmosDbIndexName)
    ? ConsoleService.GetString("Please insert the [yellow]search index name[/]:")
    : Settings.CosmosDbIndexName;

// Get Database name
string cosmosDbDatabaseName = string.IsNullOrEmpty(Settings.CosmosDbDatabaseName)
    ? ConsoleService.GetString("Please insert the [yellow]database name[/]:")
    : Settings.CosmosDbDatabaseName;

// Get Container name
string cosmosDbContainerName = string.IsNullOrEmpty(Settings.CosmosDbContainerName)
    ? ConsoleService.GetString("Please insert the [yellow]container name[/]:")
    : Settings.CosmosDbContainerName;

// Get Container name
string cosmosDbContentFieldName = string.IsNullOrEmpty(Settings.CosmosDbContentFieldName)
    ? ConsoleService.GetString("Please insert the [yellow]name of the content field[/]:")
    : Settings.CosmosDbContentFieldName;

// Get Container name
string cosmosDbVectorFieldName = string.IsNullOrEmpty(Settings.CosmosDbVectorFieldName)
    ? ConsoleService.GetString("Please insert the [yellow]name of the vector field[/]:")
    : Settings.CosmosDbVectorFieldName;

// Show Header
ConsoleService.CreateHeader();

// Create OpenAI Client
OpenAIClient openAIClient = new(new Uri(openAiEndpoint), new AzureKeyCredential(azureOpenAiKey));

// Create ChatCompletionsOptions
ChatCompletionsOptions options = CreateChatCompletionsOptions();

// Chat implementation
while (true)
{
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[green]User:[/]");

    var userMessage = Console.ReadLine();
    options.Messages.Add(new ChatRequestUserMessage(userMessage));

    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[green]Copilot on Data:[/]");

    StringBuilder stringBuilder = new();
    await foreach (var chatUpdate in await openAIClient.GetChatCompletionsStreamingAsync(options))
    {
        if (chatUpdate.ChoiceIndex.HasValue && chatUpdate.ContentUpdate is not null)
        {
            AnsiConsole.Write(chatUpdate.ContentUpdate);
            stringBuilder.Append(chatUpdate.ContentUpdate);
        }
    }

    AnsiConsole.WriteLine();
    options.Messages.Add(new ChatRequestAssistantMessage(stringBuilder.ToString()));
}

ChatCompletionsOptions CreateChatCompletionsOptions()
{
    AzureCosmosDBChatExtensionConfiguration dbChatConfiguration = new()
    {
        Authentication = new OnYourDataConnectionStringAuthenticationOptions(cosmosDbConnectionString),
        IndexName = cosmosDbIndexName,
        DatabaseName = cosmosDbDatabaseName,
        ContainerName = cosmosDbContainerName,
        FieldMappingOptions = new AzureCosmosDBFieldMappingOptions(
            [cosmosDbContentFieldName], [cosmosDbVectorFieldName]),
        ShouldRestrictResultScope = true,
        DocumentCount = 5,
        Strictness = 1,
        RoleInformation = "You are an AI assistant to help with the provided data.",
        VectorizationSource = new OnYourDataDeploymentNameVectorizationSource(embeddingsDeploymentName)
    };

    AzureChatExtensionsOptions azureChatExtensionsOptions = new();
    azureChatExtensionsOptions.Extensions.Add(dbChatConfiguration);

    ChatCompletionsOptions chatCompletionsOptions = new()
    {
        MaxTokens = 1000,
        Temperature = 0.7f,
        DeploymentName = chatDeploymentName,
        AzureExtensionsOptions = azureChatExtensionsOptions
    };

    return chatCompletionsOptions;
}