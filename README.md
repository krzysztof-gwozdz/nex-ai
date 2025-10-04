# NexAI

### The original idea

The original goal of this project was to create an autonomous AI agent that would operate as an support assistant.
The agent should be triggered every time a Zendesk ticket is assigned to the 3rd line of support.
It should then add a comment to the ticket containing a summary of the ticket, links to similar tickets and links to any related Azure Devops work items, as well as proposing solutions if it sees any.

### The current and final state

Currently, the application allows for:

- importing tickets from Zendesk into a document database (MongoDB) and a vector database (Qdrant),
- searching databases by id, semantically and via full-text search,
- retrieving related tasks from Azure DevOps,
- interacting with the data through a console application or an MCP server,
- summarizing Zendesk tickets with LLM.

There are no plans for further development of the application due to a change in the tools used for support management.

## Technologies

- C# 13
- .NET 9
- Semantic Kernel
- Ollama
- OpenAI
- MongoDB
- Qdrant
- RabbitMQ
- Docker
- xUnit

## How to run

1. Run `docker-compose up`
2. To work with Ollama run ollama or run `docker compose --profile ollama up` instead of first command.
3. Fill `appsettings.json` with your keys and settings or create `appsettings.local.json`.
4. Fill `.\NexAI.DataImporter\appsettings.data_importer.json`
5. Run `NexAI.DataImporter` project.
6. Fill `.\NexAI.DataProcessor\appsettings.data_processor.json`
7. Run `NexAI.DataProcessor` project.
8. Run `NexAI.Console` project or use the MCP server from `NexAI.MCP`.

## Versions

### 0.12.0

- Improved prompts storing.
- Improved algorithm for getting similar tickets.
- Add MCP server.

### 0.11.0

- Store more info about Zendesk Tickets.
- Improve Zendesk Tickets import and processing.
- Add an option to summarize Ticket.
- Switched to DI in the whole solution.
- Add cancellation tokens.

### 0.10.0

- Switch to the incremental export of Zendesk tickets
- Split Zendesk ticket data stored in Qdrant into 3 different types
- Introduce RabbitMQ for message queueing and split data importing into separate steps

### 0.9.0

- Clean up/normalize data imported from Zendesk.
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

- Refactoring of the codebase - change the whole structure of the project.
- Add MongoDB database integration.
- Store Zendesk tickets into two databases.

### 0.2.0

- Get Zendesk tickets from a sample JSON file.
- Add Qdrant database integration.
- Add docker compose.

### 0.1.0

- Initial version with Semantic Kernel and OpenAI integration.