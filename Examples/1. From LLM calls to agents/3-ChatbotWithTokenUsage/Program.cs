using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Diagnostics;
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
