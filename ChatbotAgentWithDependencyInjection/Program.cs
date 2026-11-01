using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;


string endpoint = "http://localhost:11434";

string model = "qwen3.5:9b";

using var ollamaHttpClient = new HttpClient
{
    BaseAddress = new Uri(endpoint),
    Timeout = TimeSpan.FromMinutes(10)
};

IChatClient ollamaClient = new OllamaApiClient(ollamaHttpClient, model);

using var services = new ServiceCollection()
    .AddSingleton<TimeProvider>(TimeProvider.System)
    .BuildServiceProvider();

using IChatClient chatClient = new FunctionInvokingChatClient(
    ollamaClient,
    functionInvocationServices: services);

var getCurrentTime = AIFunctionFactory.Create(DependencyInjectionTools.GetCurrentTime);
var getCurrentDate = AIFunctionFactory.Create(DependencyInjectionTools.GetCurrentDate);
AIAgent agent = chatClient.AsAIAgent(new ChatClientAgentOptions
{
    ChatHistoryProvider = new InMemoryChatHistoryProvider(),
    ChatOptions = new ChatOptions
    {
        Tools = [getCurrentTime, getCurrentDate]
    }
});
AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine("Local Ollama chatbot with tools. Ask for the current date or time, or type 'quit' to exit.");

while (true)
{
    Console.Write("[USER]: ");
    var input = Console.ReadLine();
    if (input is null || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
        break;

    if (string.IsNullOrWhiteSpace(input))
        continue;

    using var cts = new CancellationTokenSource();
    var spinnerTask = ShowSpinnerAsync(cts.Token);
    AgentResponse response;
    try
    {
        response = await agent.RunAsync(input, session);
    }
    finally
    {
        cts.Cancel();
        await spinnerTask;
    }
    Console.WriteLine($"[ASSISTANT]: {response.Text}");
    DebugHistory(session);
}

static void DebugHistory(AgentSession session)
{
    Console.WriteLine("\n--- DEBUG: AgentSession local in-memory history ---");
    if (!session.TryGetInMemoryChatHistory(out var messages))
        throw new InvalidOperationException("Expected local chat history in the AgentSession.");

    Console.WriteLine($"Stored messages: {messages.Count}");
    for (var i = 0; i < messages.Count; i++)
    {
        var message = messages[i];
        foreach (var content in message.Contents)
        {
            switch (content)
            {
                case FunctionCallContent functionCall:
                    Console.WriteLine(
                        $"  {i + 1}. [FunctionCallContent] CallId: {functionCall.CallId}, Name: {functionCall.Name}");
                    break;
                case FunctionResultContent functionResult:
                    Console.WriteLine(
                        $"  {i + 1}. [FunctionResultContent] CallId: {functionResult.CallId}, Result: {functionResult.Result}");
                    break;
                default:
                    Console.WriteLine($"  {i + 1}. [{message.Role}]: {content}");
                    break;
            }
        }
    }

    Console.WriteLine("--- END HISTORY ---\n");
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