using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Diagnostics;

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

List<ChatMessage> history = [];

Console.WriteLine("Chatbot with history and API token usage. Type 'quit' to exit.");

while (true)
{
    Console.Write("[USER]: ");
    string? input = Console.ReadLine();

    if (input is null || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
        break;

    if (string.IsNullOrWhiteSpace(input))
        continue;

    history.Add(new ChatMessage(ChatRole.User, input));
    using var cts = new CancellationTokenSource();

    var spinnerTask = ShowSpinnerAsync(cts.Token);

    try
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await chatClient.GetResponseAsync(input);
        history.AddRange(response.Messages);

        cts.Cancel();
        await spinnerTask;
        stopwatch.Stop();

        Console.WriteLine($"[ASSISTANT]: {response.Text}");
        Console.WriteLine($"[USAGE]: Input tokens: {response.Usage?.InputTokenCount?.ToString() ?? "not reported"}");
        Console.WriteLine($"[USAGE]: Output tokens: {response.Usage?.OutputTokenCount?.ToString() ?? "not reported"}");

        Console.WriteLine($"[TIME]: {stopwatch.Elapsed.TotalSeconds:F2} seconds");

        Console.WriteLine();
        Console.WriteLine();

        //Console.WriteLine($"json {JsonConvert.SerializeObject(response)}");
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