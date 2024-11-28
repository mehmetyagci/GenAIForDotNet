using Microsoft.Extensions.Configuration;
using Azure.AI.OpenAI;
using Azure;

var configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .Build();

var endPoint = configuration["AZURE_OPENAI_API_ENDPOINT"];

var apiKey = configuration["AZURE_OPENAI_API_KEY"];

var deploymentName = configuration["AZURE_OPENAI_API_DEPLOYMENT_NAME"];

var credential = new AzureKeyCredential(apiKey);

var client = new OpenAIClient(new Uri(endPoint), credential);

ChatCompletionsOptions completionsOptions = new();
completionsOptions.DeploymentName = deploymentName;
// completionsOptions.MaxTokens = 5; error handling

// System message
ChatRequestSystemMessage systemMsg =
    new(@"You are an annoyingly friendly AI 
        chatbot at the Azure Saturday AI 
        conference in Seattle. Be brief.");
completionsOptions.Messages.Add(systemMsg);

try
{
    while (true)
    {
        // AI's answer
        Console.ForegroundColor = ConsoleColor.White;
        var result = client.GetChatCompletions(completionsOptions);
        var firstChoice = result?.Value?.Choices?.FirstOrDefault();
        if (firstChoice == null)
        {
            Console.WriteLine("No chat completion choices available.");
            break;
        }

        if (firstChoice.FinishReason != CompletionsFinishReason.Stopped)
        {
            Console.WriteLine($"Chat completion didn't finish normally." +
                              $"Reason : {firstChoice.FinishReason}");
            break;
        }

        var answer = firstChoice?.Message?.Content;

        // Answer added to the Chat History
        completionsOptions.Messages.Add(
            new ChatRequestAssistantMessage(answer));

        Console.WriteLine(answer);
        Console.WriteLine();

        // User's Input
        Console.ForegroundColor = ConsoleColor.Yellow;
        var userInput = Console.ReadLine();
        ChatRequestUserMessage userMessage = new(userInput);
        completionsOptions.Messages.Add(userMessage);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error has occured:\n{ex.Message}");
    throw;
}

