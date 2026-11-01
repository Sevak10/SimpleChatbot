using Microsoft.Extensions.AI;
using OllamaSharp;
using StructuredOutput;
using System.Diagnostics;
using System.Text.Json;

Console.WriteLine("Hello, World!");

string endpoint = "http://localhost:11434";

string model = "qwen3.5:9b";

var httpClient = new HttpClient
{
    BaseAddress = new Uri(endpoint),
    Timeout = TimeSpan.FromMinutes(10)
};

using IChatClient chatClient = new OllamaApiClient(
    httpClient,
    model);

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
        Console.WriteLine($"Ollama connection error: {ex.Message}");
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