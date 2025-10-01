namespace NexAI.Zendesk;

public record SearchResult(ZendeskTicket ZendeskTicket, double Score, string Method, string Info)
{
    public static SearchResult FullTextSearchResult(ZendeskTicket zendeskTicket, double score) =>
        new(zendeskTicket, score, "full-text", "");

    public static SearchResult EmbeddingBasedSearchResult(ZendeskTicket zendeskTicket, double score, string info) =>
        new(zendeskTicket, score, "embedding-based", info);
}