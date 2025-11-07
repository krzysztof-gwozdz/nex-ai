// ReSharper disable InconsistentNaming

using NexAI.Git;
using NexAI.Git.Commands;
using Spectre.Console;

namespace NexAI.DataProcessor.Git;

public class GitCommitNeo4jExporter(UpsertGitCommitCommand upsertGitCommitCommand)
{
    public Task CreateSchema(CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[green]Current setup does not require schema creation for Git commits in Neo4j.[/]");
        return Task.CompletedTask;
    }

    public async Task Export(GitCommit gitCommit, CancellationToken cancellationToken)
    {
        await upsertGitCommitCommand.Handle(gitCommit);
        AnsiConsole.MarkupLine($"[gold3_1]Successfully exported Git commit {gitCommit.Sha}: \"{gitCommit.Name.EscapeMarkup()}\" into Neo4j.[/]");
    }
}