# NexAI

NexAI is an AI agent as a kind of '2.5 line of support.

# Versions

### 0.10.0

- Switch to the incremental export Zendesk tickets
- Split Zendesk ticket data stored in Qdrant to 3 different types
- Introduce RabbitMQ for message queueing and split data importing into separate steps

### 0.9.0

- Clean up/normalizing data imported from Zendesk.
- Polish search results.

### 0.8.0

- Data importer improvements.

### 0.7.0

- Add support for local LLMs by integrating with Ollama.

### 0.6.0

- Integrate with Zendesk API.

### 0.5.0

- Integrate with Azure DevOps to fetch work items.

### 0.4.0

- Add an option to use Full-Text Search over Zendesk tickets.

### 0.3.0

- Refactoring of the codebase - change the whole structure of project.
- Add MongoDB database integration.
- Store Zendesk tickets into two databases.

### 0.2.0

- Get Zendesk tickets from a sample JSON file.
- Add Qdrant database integration.
- Add docker compose.

### 0.1.0

- Initial version with Semantic Kernel and OpenAI integration.