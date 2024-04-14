# Create a Copilot on your data using Azure.AI.OpenAI

This repository contains a simple .NET console application demonstrating the [Azure.AI.OpenAI NuGet package](https://www.nuget.org/packages/Azure.AI.OpenAI) to create a Copilot on your own data.

![Header](./docs/header.png)

## Usage

You need to have your data prepared within an [Azure CosmosDB for MongoDB vCore](https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/vcore/) and you also need to ave access for the [Azure OpenAI](https://azure.microsoft.com/en-us/products/ai-services/openai-service) services within Azure.

Insert the needed values into the `Settings.cs` file or simply run the application, because in this case the console application is asking to enter all the needed values.