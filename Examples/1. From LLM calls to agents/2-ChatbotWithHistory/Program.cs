using Microsoft.Extensions.AI;
using OllamaSharp;
using OpenAI;
using System.ClientModel;

Console.WriteLine("Hello, World!");

bool useGemini = true;
string apiKey = "YOUR_GEMINI_API_KEY";
string endpoint = "https://generativelanguage.googleapis.com/v1beta/openai/";
string model = "gemini-3.5-flash-lite";

IChatClient configuredClient;
if (useGemini)
{
    configuredClient = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
    {
        Endpoint = new Uri(endpoint)
    }).GetChatClient(model).AsIChatClient();
}
else
{
    var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:11434"), Timeout = TimeSpan.FromMinutes(10) };
    configuredClient = new OllamaApiClient(httpClient, "qwen3.5:9b");
}
using IChatClient chatClient = configuredClient;

List<ChatMessage> history = [];

Console.WriteLine("Chatbot with history. Type 'quit' to exit.");

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
        var response = await chatClient.GetResponseAsync(input);
        history.AddRange(response.Messages);

        cts.Cancel();
        await spinnerTask;

        Console.WriteLine($"[ASSISTANT]: {response.Text}");
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
