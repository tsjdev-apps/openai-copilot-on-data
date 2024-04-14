using CopilotOnData.Services;

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

// Show Header
ConsoleService.CreateHeader();

Console.ReadKey();