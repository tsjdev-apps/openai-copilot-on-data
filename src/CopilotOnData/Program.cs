using Azure;
using Azure.AI.OpenAI;
using CopilotOnData.Services;
using Spectre.Console;
using System.Text;

// Get Endpoint
string openAiEndpoint = 
    ConsoleService.GetUrl("Please insert your [yellow]Azure OpenAI endpoint[/]:");

// Get Azure OpenAI Key
string azureOpenAiKey = 
    ConsoleService.GetString("Please insert your [yellow]Azure OpenAI API key[/]:");

// Get deployment name
string chatDeploymentName = 
    ConsoleService.GetString("Please insert the [yellow]deployment name of the chat model[/]:");

// Get deployment name
string embeddingsDeploymentName = 
    ConsoleService.GetString("Please insert the [yellow]deployment name of the embeddings model[/]:");

// Get Cosmos DB for MongoDB connection string
string cosmosDbConnectionString = 
    ConsoleService.GetString("Please insert the [yellow]Cosmos DB for MongoDB vCore[/] connection string:", false);

// Get Search Index name
string cosmosDbIndexName = 
    ConsoleService.GetString("Please insert the [yellow]search index name[/]:");

// Get Database name
string cosmosDbDatabaseName = 
    ConsoleService.GetString("Please insert the [yellow]database name[/]:");

// Get Container name
string cosmosDbContainerName = 
    ConsoleService.GetString("Please insert the [yellow]container name[/]:");

// Get Container name
string cosmosDbContentFieldName =
    ConsoleService.GetString("Please insert the [yellow]name of the content field[/]:");

// Get Container name
string cosmosDbVectorFieldName =
    ConsoleService.GetString("Please insert the [yellow]name of the vector field[/]:");

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