using System.Text;
using System.Text.Json;

namespace NexAI.Zendesk;

public class ZendeskIssueStore()
{
    private readonly List<ZendeskIssue> _issues = [];
    private bool _initialized;

    public ZendeskIssue? GetIssueByIdAsync(string id)
    {
        Initialize();
        return _issues.FirstOrDefault(i => i.Id == id);
    }

    public List<SimilarIssue>? FindSimilarIssuesById(string issueId)
    {
        Initialize();
        var targetIssue = GetIssueByIdAsync(issueId)!;
        var targetText = BuildIssueText(targetIssue);
        var similarIssues = new List<SimilarIssue>();

        foreach (var issue in _issues.Where(i => i.Id != issueId))
        {
            var issueText = BuildIssueText(issue);
            var similarity = CalculateSimilarity(targetText, issueText);
            similarIssues.Add(new(
                issue.Id,
                issue.Title,
                similarity
            ));
        }

        return similarIssues
            .OrderByDescending(similarIssue => similarIssue.Similarity)
            .Take(10)
            .ToList();
    }
    
    public List<SimilarIssue>? FindSimilarIssuesByPhrase(string phrase)
    {
        Initialize();
        var similarIssues = new List<SimilarIssue>();

        foreach (var issue in _issues)
        {
            var issueText = BuildIssueText(issue);
            var similarity = CalculateSimilarity(phrase, issueText);
            similarIssues.Add(new(
                issue.Id,
                issue.Title,
                similarity
            ));
        }

        return similarIssues
            .OrderByDescending(similarIssue => similarIssue.Similarity)
            .Take(10)
            .ToList();
    }

    private static string BuildIssueText(ZendeskIssue issue)
    {
        var textBuilder = new StringBuilder();

        // Add title
        textBuilder.AppendLine(issue.Title ?? "");
        textBuilder.AppendLine(issue.Description ?? "");

        // Add messages in chronological order
        if (issue.Messages != null)
        {
            foreach (var message in issue.Messages.OrderBy(m => m.CreatedAt))
            {
                textBuilder.AppendLine(message.Content ?? "");
            }
        }

        return textBuilder.ToString().ToLowerInvariant();
    }

    private static double CalculateSimilarity(string text1, string text2)
    {
        var words1 = text1.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        var words2 = text2.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries).ToHashSet();

        var intersection = words1.Intersect(words2).Count();
        var union = words1.Union(words2).Count();

        return union > 0 ? (double)intersection / union : 0;
    }

    private void Initialize()
    {
        if (_initialized)
            return;
        PopulateSampleData();
        _initialized = true;
    }

    private void PopulateSampleData()
    {
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Zendesk", "sample-issues.json");
        if (!File.Exists(jsonPath))
        {
            throw new SomethingIsNotYesException($"Sample issues file not found at: {jsonPath}");
        }

        var jsonContent = File.ReadAllText(jsonPath);
        var issues = JsonSerializer.Deserialize<ZendeskIssue[]>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (issues == null)
        {
            throw new SomethingIsNotYesException("Failed to deserialize sample issues from JSON");
        }

        _issues.AddRange(issues);
    }
}