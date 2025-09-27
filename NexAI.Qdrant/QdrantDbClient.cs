using NexAI.Config;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace NexAI.Qdrant;

public class QdrantDbClient(Options options) : 
    QdrantClient(new QdrantGrpcClient(options.Get<QdrantOptions>().Host, options.Get<QdrantOptions>().Port, options.Get<QdrantOptions>().ApiKey));