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
8. **ChatbotWithThinking** — Streams a concise “Thinking Process” summary followed by the answer as Ollama generates it.

### 2. Giving agents capabilities: tools and MCP

🚧 Examples coming soon.

### 3. Orchestrating agents and workflows

🚧 Examples coming soon.

## 🛠️ Prerequisites

These examples use [Ollama](https://ollama.com/) to run language models locally. Everyone who wants to run the examples needs to:

1. Install Ollama on their computer.
2. Download the model required by the project they want to run. For example:

   ```bash
   ollama pull qwen3.5:9b
   ```

3. Make sure Ollama is running. The examples connect to `http://localhost:11434`.

The model name can vary by project. Check that project's `Program.cs` and download the model specified there with `ollama pull <model-name>`.

You also need the .NET SDK version targeted by the project. The current projects target .NET 11 (`net11.0`), which requires the .NET 11 SDK.

## 🚀 Getting started

1. Clone or download this repository.
2. Install the prerequisites above and start Ollama.
3. Open `SimpleChatbot.slnx` in your IDE.
4. Choose a project and run it. For example, run ChatbotWithThinking with:

   ```bash
   dotnet run --project "Examples/1. From LLM calls to agents/8-ChatbotWithThinking/ChatbotWithThinking.csproj"
   ```

Each example has its own project folder under `Examples`.



