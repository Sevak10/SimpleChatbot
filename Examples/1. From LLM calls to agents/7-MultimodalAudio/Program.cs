
using System.Diagnostics;
using System.Text.Json;

const bool useGemini = true;
const string apiKey = "YOUR_GEMINI_API_KEY";
const string endpoint = "https://generativelanguage.googleapis.com/v1beta/openai/";
const string model = "gemini-3.5-flash-lite";

var audioPath = Path.Combine(
    AppContext.BaseDirectory,
    "welcome-fr.wav");

var whisperModelPath = Path.Combine(
    AppContext.BaseDirectory,
    "ggml-base.bin");

if (!File.Exists(audioPath))
{
    Console.WriteLine("Audio file not found.");
    return;
}

// 1. Download Whisper model if needed
if (!File.Exists(whisperModelPath))
{
    Console.WriteLine("Downloading Whisper model...");

    using var modelStream =
        await WhisperGgmlDownloader.Default
            .GetGgmlModelAsync(GgmlType.Base);

    using var fileWriter = File.Create(whisperModelPath);
    await modelStream.CopyToAsync(fileWriter);
}

// 2. Transcribe audio locally
Console.WriteLine("\nTranscribing audio...");

var transcriptionTimer = Stopwatch.StartNew();

string transcription;

{
    using var transcriptionCts = new CancellationTokenSource();
    var transcriptionSpinnerTask = ShowSpinnerAsync(transcriptionCts.Token);
    try
    {
        using var whisperFactory = WhisperFactory.FromPath(whisperModelPath);
        using var processor = whisperFactory.CreateBuilder()
            .WithLanguage("fr")
            .Build();
        using var audio = File.OpenRead(audioPath);
        var segments = new List<string>();

        await foreach (var result in processor.ProcessAsync(audio))
        {
            segments.Add(result.Text);
        }

        transcription = string.Join(" ", segments);
    }
    finally
    {
        transcriptionCts.Cancel();
        await transcriptionSpinnerTask;
    }
}

transcriptionTimer.Stop();

Console.WriteLine($"\n[TRANSCRIPTION]: {transcription}");
Console.WriteLine(
    $"Transcription time: {transcriptionTimer.Elapsed}");

// 3. Translate using the selected chat provider
Console.WriteLine(useGemini ? "\nTranslating with Gemini..." : "\nTranslating with Ollama...");

var translationTimer = Stopwatch.StartNew();

using var httpClient = new HttpClient
{
    BaseAddress = new Uri(useGemini ? endpoint : "http://localhost:11434"),
    Timeout = TimeSpan.FromMinutes(10)
};
if (useGemini)
    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

var request = new
{
    model = useGemini ? model : "qwen3.5:9b",
    stream = false,
    messages = new[]
    {
        new
        {
            role = "system",
            content = """
                You are a professional translator.
                Translate the provided French text into English.
                Preserve the original meaning, tone and context.
                Do not add explanations or additional information.
                Return only the translated text.
                """
        },
        new
        {
            role = "user",
            content = transcription
        }
    }
};

string? translatedText;
using (var translationCts = new CancellationTokenSource())
{
    var translationSpinnerTask = ShowSpinnerAsync(translationCts.Token);
    try
    {
        using var response = await httpClient.PostAsJsonAsync(useGemini ? "chat/completions" : "/api/chat", request);
        response.EnsureSuccessStatusCode();
        JsonElement json = await response.Content.ReadFromJsonAsync<JsonElement>();
        translatedText = useGemini
            ? json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()
            : json.GetProperty("message").GetProperty("content").GetString();
    }
    finally
    {
        translationCts.Cancel();
        await translationSpinnerTask;
    }
}

translationTimer.Stop();

Console.WriteLine($"\n[TRANSLATION]: {translatedText}");
Console.WriteLine(
    $"Translation time: {translationTimer.Elapsed}");

Console.WriteLine("\nCompleted!");

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
