using NexAI.Config;
using NexAI.Git;
using NexAI.RabbitMQ;
using Spectre.Console;

namespace NexAI.DataImporter.Git;

internal class GitImporter(GitRepositoryClient gitRepositoryClient, RabbitMQClient rabbitMQClient, Options options)
{
    public async Task Import(CancellationToken cancellationToken)
    {
        var gitOptions = options.Get<GitOptions>();
        var allCommits = new List<GitCommit>();

        foreach (var repositoryPath in gitOptions.RepositoryPaths)
        {
            AnsiConsole.MarkupLine($"[yellow]Importing commits from repository: {repositoryPath}[/]");
            try
            {
                var commits = gitRepositoryClient.ExtractCommits(repositoryPath).ToList();
                allCommits.AddRange(commits);
                AnsiConsole.MarkupLine($"[green]Imported {commits.Count} commits from {repositoryPath}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Failed to import from {repositoryPath}: {ex.Message}[/]");
            }
        }

        if (allCommits.Count > 0)
        {
            AnsiConsole.MarkupLine($"[yellow]Sending {allCommits.Count} commits to RabbitMQ...[/]");
            await rabbitMQClient.Send(RabbitMQStructure.GitCommitExchangeName, allCommits.ToArray(), cancellationToken);
            AnsiConsole.MarkupLine($"[green]Sent {allCommits.Count} commits to RabbitMQ.[/]");
        }
    }
}