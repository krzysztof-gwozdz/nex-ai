// ReSharper disable InconsistentNaming

using NexAI.Git;
using NexAI.Git.Commands;
using NexAI.Git.Messages;
using Spectre.Console;

namespace NexAI.DataProcessor.Git;

public class GitCommitImportedEventHandler(UpsertGitCommitCommand upsertGitCommitCommand) : IHandleMessages<GitCommitImportedEvent>
{
    public async Task Handle(GitCommitImportedEvent message, IMessageHandlerContext context)
    {
        var gitCommit = GitCommit.FromGitCommitImportedEvent(message);
        await upsertGitCommitCommand.Handle(gitCommit);
        AnsiConsole.MarkupLine($"[gold3_1]Successfully exported Git commit {gitCommit.Sha}: \"{gitCommit.Name.EscapeMarkup()}\" into Neo4j.[/]");
    }
}