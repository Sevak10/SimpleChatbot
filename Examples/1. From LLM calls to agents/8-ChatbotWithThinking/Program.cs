using Microsoft.Extensions.AI;
using OllamaSharp;

const string endpoint = "http://localhost:11434";
const string model = "qwen3.5:9b";

using var httpClient = new HttpClient
{
    BaseAddress = new Uri(endpoint),
    Timeout = TimeSpan.FromMinutes(10)
};

using IChatClient chatClient = new OllamaApiClient(httpClient, model);

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
        Console.WriteLine($"Ollama connection error: {ex.Message}");
    }
}
