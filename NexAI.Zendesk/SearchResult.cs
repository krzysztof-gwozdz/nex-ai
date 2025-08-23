namespace NexAI.Zendesk;

public record SearchResult(ZendeskTicket ZendeskTicket, double Score, string Method)
{
    public static SearchResult FullTextSearchResult(ZendeskTicket zendeskTicket, double score) =>
        new(zendeskTicket, score, "full-text");
    
    public static SearchResult EmbeddingBasedSearchResult(ZendeskTicket zendeskTicket, double score) =>
        new(zendeskTicket, score, "embedding-based");
}