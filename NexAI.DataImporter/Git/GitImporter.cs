using NexAI.Config;
using NexAI.Git;
using Spectre.Console;

namespace NexAI.DataImporter.Git;

internal class GitImporter(GitRepositoryClient gitRepositoryClient, IMessageSession messageSession, Options options)
{
    public async Task Import()
    {
        var gitOptions = options.Get<GitOptions>();
        var commitsCount = 0;
        foreach (var repositoryPath in gitOptions.RepositoryPaths)
        {
            AnsiConsole.MarkupLine($"[yellow]Importing commits from repository: {repositoryPath}[/]");
            try
            {
                var commits = gitRepositoryClient.ExtractCommits(repositoryPath).ToList();
                foreach (var commit in commits)
                {
                    await messageSession.Publish(commit.ToGitCommitImportedEvent());
                    commitsCount++;
                }
                AnsiConsole.MarkupLine($"[green]Imported {commits.Count} commits from {repositoryPath}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Failed to import from {repositoryPath}: {ex.Message}[/]");
            }
        }
        AnsiConsole.MarkupLine($"[green]Sent {commitsCount} commits to RabbitMQ.[/]");
    }
}
