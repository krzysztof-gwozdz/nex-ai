namespace NexAI.LLMs.Common;

public static class LLM
{
    private const string OpenAI = "OpenAI";
    private const string Ollama = "Ollama";
    private const string Fake = "Fake";

    public static TResult ForAll<TResult>(
        string llm,
        Func<TResult> forOpenAI,
        Func<TResult> forOllama,
        Func<TResult> forFake) =>
        llm switch
        {
            OpenAI => forOpenAI(),
            Ollama => forOllama(),
            Fake => forFake(),
            _ => throw new ArgumentException($"Unknown LLM: {llm}", nameof(llm))
        };

    public static void ForAll(
        string llm,
        Action forOpenAI,
        Action forOllama,
        Action forFake)
    {
        switch (llm)
        {
            case OpenAI:
                forOpenAI();
                break;
            case Ollama:
                forOllama();
                break;
            case Fake:
                forFake();
                break;
            default:
                throw new ArgumentException($"Unknown LLM: {llm}", nameof(llm));
        }
    }
}