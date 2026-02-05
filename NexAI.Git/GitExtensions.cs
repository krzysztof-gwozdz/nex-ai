using Microsoft.Extensions.DependencyInjection;
using NexAI.Git.Commands;

namespace NexAI.Git;

public static class GitExtensions
{
    public static IServiceCollection AddGit(this IServiceCollection services) =>
        services
            .AddScoped<GitRepositoryClient>()
            .AddScoped<UpsertGitCommitCommand>();
}