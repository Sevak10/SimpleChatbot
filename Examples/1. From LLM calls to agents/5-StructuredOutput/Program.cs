using Microsoft.Extensions.AI;
using OllamaSharp;
using StructuredOutput;
using System.Diagnostics;
using System.Text.Json;
using OpenAI;
using System.ClientModel;

Console.WriteLine("Hello, World!");

bool useGemini = true;
string apiKey = "YOUR_GEMINI_API_KEY";
string endpoint = "https://generativelanguage.googleapis.com/v1beta/openai/";
string model = "gemini-3.5-flash-lite";
IChatClient configuredClient;
if (useGemini)
    configuredClient = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions { Endpoint = new Uri(endpoint) }).GetChatClient(model).AsIChatClient();
else
    configuredClient = new OllamaApiClient(new HttpClient { BaseAddress = new Uri("http://localhost:11434"), Timeout = TimeSpan.FromMinutes(10) }, "qwen3.5:9b");
using IChatClient chatClient = configuredClient;

Console.WriteLine("Generate a fictional employee with structured output. Type 'quit' to exit.");

while (true)
{
    Console.Write("[USER]: ");
    string? input = Console.ReadLine();

    if (input is null || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
        break;

    if (string.IsNullOrWhiteSpace(input))
        continue;

    using var cts = new CancellationTokenSource();

    var spinnerTask = ShowSpinnerAsync(cts.Token);

    try
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await chatClient.GetResponseAsync<Employee>(
        $"Generate one fictional adult employee based on this request: {input}. " +
        "Name is the family name. Salary is the annual gross salary in EUR.",
        useJsonSchemaResponseFormat: true);

        Employee employee = response.Result;

        cts.Cancel();
        await spinnerTask;
        stopwatch.Stop();

        Console.WriteLine($"[ASSISTANT]: {JsonSerializer.Serialize(employee, new JsonSerializerOptions { WriteIndented = true })}");

    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Chat service connection error: {ex.Message}");
    }
}

static async Task ShowSpinnerAsync(CancellationToken cancellationToken)
{
    char[] frames = ['|', '/', '-', '\\'];
    var i = 0;

    try
    {
        while (true)
        {
            Console.Write($"\r[AI WORKING] {frames[i++ % frames.Length]}");
            await Task.Delay(100, cancellationToken);
        }
    }
    catch (OperationCanceledException)
    {
        Console.Write("\r                 \r");
    }
}
