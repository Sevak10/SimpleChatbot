using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;


string endpoint = "http://localhost:11434";

string model = "qwen3.5:9b";

IChatClient ollamaClient = new OllamaApiClient(
    new Uri(endpoint), model);

using IChatClient chatClient = ollamaClient
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

var getCurrentTime = AIFunctionFactory.Create(DateTimeTools.GetCurrentTime);
var getCurrentDate = AIFunctionFactory.Create(DateTimeTools.GetCurrentDate);
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

    var response = await agent.RunAsync(input, session);
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
