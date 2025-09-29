namespace NexAI.LLMs;

public class PromptReader
{
    public string Read(string prompt)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Prompts", $"{prompt}.txt");
        return !File.Exists(path) ? throw new($"Prompt file {path} not found") : File.ReadAllText(path);
    }
}