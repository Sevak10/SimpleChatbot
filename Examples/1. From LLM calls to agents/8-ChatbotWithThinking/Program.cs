using Microsoft.Extensions.AI;
using OllamaSharp;
using OpenAI;
using System.ClientModel;

const bool useGemini = true;
const string apiKey = "YOUR_GEMINI_API_KEY";
const string endpoint = "https://generativelanguage.googleapis.com/v1beta/openai/";
const string model = "gemini-3.5-flash-lite";
IChatClient configuredClient;
if (useGemini)
    configuredClient = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions { Endpoint = new Uri(endpoint) }).GetChatClient(model).AsIChatClient();
else
    configuredClient = new OllamaApiClient(new HttpClient { BaseAddress = new Uri("http://localhost:11434"), Timeout = TimeSpan.FromMinutes(10) }, "qwen3.5:9b");
using IChatClient chatClient = configuredClient;

var options = new ChatOptions
{
    Instructions = """
        Answer the user directly. Format every response with these two headings:
        Thinking Process: Give a brief, high-level explanation summary in one or two sentences.
        Do not provide hidden chain-of-thought or private step-by-step reasoning.
        Answer: Give the direct answer. If the user asks for a number, include the number clearly.
        """
};

Console.WriteLine("Local Ollama chatbot with brief explanation summaries. Type 'quit' to exit.");

while (true)
{
    Console.Write("user: ");
    string? input = Console.ReadLine();

    if (input is null || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
        break;

    if (string.IsNullOrWhiteSpace(input))
        continue;

    Console.WriteLine("Thinking...");

    try
    {
        await foreach (var update in chatClient.GetStreamingResponseAsync(input, options))
        {
            if (!string.IsNullOrEmpty(update.Text))
                Console.Write(update.Text);
        }

        Console.WriteLine();
        Console.WriteLine("...done thinking.\n");
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Chat service connection error: {ex.Message}");
    }
}
