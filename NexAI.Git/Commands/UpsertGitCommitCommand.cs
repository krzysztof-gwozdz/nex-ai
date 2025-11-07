// ReSharper disable InconsistentNaming

using NexAI.Neo4j;

namespace NexAI.Git.Commands;

public class UpsertGitCommitCommand(Neo4jDbClient neo4jDbClient)
{
    public async Task Handle(GitCommit gitCommit)
    {
        const string query = @"
            MERGE (commit:Commit { sha: $sha })
            ON CREATE SET commit.id = $id
            SET
                commit.name = $name,
                commit.description = $description,
                commit.committedAt = $committedAt            
            WITH commit
            MERGE (author:User { email: $authorEmail })
            ON CREATE SET author.id = $authorId
            SET author.name = $authorName
            MERGE (author)-[:AUTHOR_OF]->(commit)";

        var parameters = new Dictionary<string, object>
        {
            { "id", (string)gitCommit.Id },
            { "sha", gitCommit.Sha },
            { "name", gitCommit.Name },
            { "authorId", (string)gitCommit.Author.Id },
            { "authorName", gitCommit.Author.Name },
            { "authorEmail", gitCommit.Author.Email },
            { "description", gitCommit.Description },
            { "committedAt", gitCommit.CommittedAt }
        };

        await neo4jDbClient.ExecuteQuery(query, parameters);
    }
}