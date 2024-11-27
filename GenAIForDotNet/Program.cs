using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Azure.AI.OpenAI;

var configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .Build();

string key = configuration["Azure:ApiKey"];
string deploymentName = configuration["Azure:DeploymentName"];

var client = new OpenAIClient(key);

ChatCompletionsOptions completionsOptions = new();
completionsOptions.DeploymentName = deploymentName;

// System message
ChatRequestSystemMessage systemMsg =
    new(@"You are an annoyingly friendly AI 
        chatbot at the Azure Saturday AI 
        conference in Seattle. Be brief.");
completionsOptions.Messages.Add(systemMsg);

while (true)
{
    // AI's answer
    Console.ForegroundColor = ConsoleColor.White;
    var result = client.GetChatCompletions(completionsOptions);
    var firstChoice = result.Value.Choices.FirstOrDefault();
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