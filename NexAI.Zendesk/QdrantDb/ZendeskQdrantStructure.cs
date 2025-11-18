using Microsoft.Extensions.Logging;
using NexAI.LLMs.Common;
using NexAI.Qdrant;
using Qdrant.Client.Grpc;

namespace NexAI.Zendesk.QdrantDb;

public class ZendeskQdrantStructure(ILogger<ZendeskQdrantStructure> logger, QdrantDbClient qdrantDbClient, TextEmbedder textEmbedder)
{
    public async Task Create(bool recreate, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start exporting Zendesk tickets into Qdrant...");
        if (recreate && await qdrantDbClient.CollectionExistsAsync(ZendeskTicketQdrantCollection.Name, cancellationToken))
        {
            await qdrantDbClient.DeleteCollectionAsync(ZendeskTicketQdrantCollection.Name, cancellationToken: cancellationToken);
            logger.LogInformation("Deleted collection for Zendesk tickets in Qdrant.");
        }

        if (!await qdrantDbClient.CollectionExistsAsync(ZendeskTicketQdrantCollection.Name, cancellationToken))
        {
            await qdrantDbClient.CreateCollectionAsync(ZendeskTicketQdrantCollection.Name, new VectorParams { Size = textEmbedder.EmbeddingDimension, Distance = Distance.Dot }, cancellationToken: cancellationToken);
            logger.LogInformation("Created schema for Zendesk tickets in Qdrant.");
        }
        else
        {
            logger.LogInformation("Collection for Zendesk tickets already exists in Qdrant. Skipping schema creation.");
        }
    }
}