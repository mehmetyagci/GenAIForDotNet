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
string serviceHub = configuration["Azure:ServiceHub"]; // Potential use in your setup
string projectName = configuration["Azure:ProjectName"]; // Potential use in tracking or logging


var client = new OpenAIClient(key);

ChatCompletionsOptions completionsOptions = new();
completionsOptions.DeploymentName = deploymentName;

ChatRequestSystemMessage systemMsg =
    new(@"You are an annoyingly friendly AI 
        chatbot at the Azure Saturday AI 
        conference in Seattle. Be brief.");

completionsOptions.Messages.Add(systemMsg);
var result = client.GetChatCompletions(completionsOptions);

var firstChoice = result.Value.Choices.FirstOrDefault();

Console.WriteLine(firstChoice?.Message?.Content ?? "No content returned");




