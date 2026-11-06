# 🤖 LLM and Agent Examples

A collection of small projects that show how to build applications with large language models, from basic chat completions to agents and multimodal features.

## 📚 Examples

### 1. From LLM calls to agents

1. **SimpleChatbot** — A basic chatbot using an LLM.
2. **ChatbotWithHistory** — Maintains conversation history across turns.
3. **ChatbotWithTokenUsage** — Displays token usage for LLM calls.
4. **ChatbotPersonas** — Uses system instructions to define chatbot personas.
5. **StructuredOutput** — Produces responses in a structured format.
6. **MultimodalImages** — Sends images to a multimodal model.
7. **MultimodalAudio** — Works with audio input or output.
8. **ChatbotWithThinking** — Streams a concise “Thinking Process” summary followed by the answer.

### 2. Giving agents capabilities: tools and MCP

1. **ChatbotAgentWithTools** — Gives the agent direct date and time tools.
2. **ChatbotAgentWithDependencyInjection** — Resolves `TimeProvider` through dependency injection for its date and time tools.
3. **ChatbotAgentWithMcpServer** — Connects to a separate `DateTimeMcpServer` over MCP and gives the returned tools to the agent.

### 3. Orchestrating agents and workflows

1. **TerribleStartupPitchManualOrchestration** — Runs a Founder agent to create a playful startup pitch, then passes that pitch to an Investor agent for review. The application controls the sequence and gives each agent a separate session.

## 🛠️ Prerequisites and provider setup

Chat and agent examples default to Gemini through Google's OpenAI-compatible API. Before running one, open its `Program.cs` and replace `Sevak_key` with your Gemini API key. The endpoint and model are already set to Google's Gemini API and Gemini 3.5 Flash-Lite:

```csharp
bool useGemini = true;
string apiKey = "YOUR_GEMINI_API_KEY";
string endpoint = "https://generativelanguage.googleapis.com/v1beta/openai/";
string model = "gemini-3.5-flash-lite";
```

Set `useGemini` to `false` to use Ollama instead. Install [Ollama](https://ollama.com/), download the model required by the project, and make sure it is running. For example:

```bash
ollama pull qwen3.5:9b
```

Ollama examples connect to `http://localhost:11434`. Most projects use `qwen3.5:9b` when Ollama is selected.

Install the .NET SDK matching the project's target framework. The solution contains projects targeting both .NET 10 and .NET 11; check the project's `.csproj` file if needed.

The **MultimodalAudio** example transcribes speech locally with Whisper, then translates the transcription using the selected Gemini or Ollama provider. It expects `welcome-fr.wav` in the project's output directory and downloads the Whisper model on first run.

The **ChatbotAgentWithMcpServer** example also requires the `DateTimeMcpServer` project to be running. Start that server before starting the chatbot.

## 🚀 Getting started

1. Clone or download this repository.
2. Replace `Sevak_key` in the chosen project's `Program.cs` with your Gemini API key, or configure Ollama and set `useGemini` to `false`.
3. Open `SimpleChatbot.slnx` in your IDE.
4. Choose a project and run it. For example, run ChatbotWithThinking with:

   ```bash
   dotnet run --project "Examples/1. From LLM calls to agents/8-ChatbotWithThinking/ChatbotWithThinking.csproj"
   ```

Each example has its own project folder under `Examples`. The additional agent projects are at the repository root.




