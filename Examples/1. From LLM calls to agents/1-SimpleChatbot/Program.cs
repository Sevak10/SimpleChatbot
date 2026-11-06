using Microsoft.Extensions.AI;
using OpenAI;
using OllamaSharp;
using System.ClientModel;
using System.Diagnostics;

Console.WriteLine("Hello, World!");

// Set these values to use Gemini. Change useGemini to false to use local Ollama instead.
bool useGemini = true;
string apiKey = "YOUR_GEMINI_API_KEY";
string endpoint = "https://generativelanguage.googleapis.com/v1beta/openai/";
string model = "gemini-3.5-flash-lite";

IChatClient configuredChatClient;
if (useGemini)
{
    var openAiOptions = new OpenAIClientOptions
    {
        Endpoint = new Uri(endpoint)
    };

    configuredChatClient = new OpenAIClient(
        new ApiKeyCredential(apiKey),
        openAiOptions)
        .GetChatClient(model)
        .AsIChatClient();
}
else
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:11434"),
        Timeout = TimeSpan.FromMinutes(10)
    };

    configuredChatClient = new OllamaApiClient(httpClient, "qwen3.5:9b");
}

using IChatClient chatClient = configuredChatClient;

Console.WriteLine("Local Chatbot - no history. Type 'quit' to exit.");

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
        var response = await chatClient.GetResponseAsync(input);

        cts.Cancel();
        await spinnerTask;
        stopwatch.Stop();

        Console.WriteLine($"[ASSISTANT]: {response.Text}");
        Console.WriteLine($"[TIME]: {stopwatch.Elapsed.TotalSeconds:F2} seconds");

        Console.WriteLine();
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
