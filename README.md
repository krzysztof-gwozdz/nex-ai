# NexAI

NexAI is an AI agent as a kind of '2.5 line of support.

# Flows
## Parsing Zendesk Tickets

```mermaid
flowchart TD
    A["Start Import"] --> C("Get Groups from API") & B("Get Employees from API") & D("Get Tickets from API")
    C --> E{"For every ticket"}
    B --> E
    D --> E
    E --> F("Get Comments")
    F --> G("Map to internal model")
    G --> H1["Id"] & I1["Number"] & J1["Title"] & K1["Description"] & M1["CreatedAt"] & N1["UpdatedAt"] & L1["Messages"]
    H1 --> H2("Generate Guid")
    I1 --> I2("Get ticket.Id")   
    J1 --> J2("Get ticket.Subject")
    J2 --> J3("Normalize Text")   
    K1 --> K2("Get ticket.Description")
    K2 --> K3("Mask Email Addresses")
    K3 --> K4("Mask Phone Numbers")
    K4 --> K5("Mask Image Urls")
    K5 --> K6("Normalize Text")
    M1 --> M2("Get ticket.CreatedAt")
    N1 --> N2("Get ticket.UpdatedAt")
    L1 --> L2("Get Comments from API")
    L2 --> L3{"For every comment"}
    L3 --> L41["Content"]
    L41 --> L411("Get comment.PlainBody")
    L411 --> L412("Mask Email Addresses")
    L412 --> L413("Mask Phone Numbers")
    L413 --> L414("Mask Image Urls")
    L414 --> L415("Normalize Text")
    L415 --> L4("Zendesk Comment")
    L3 --> L42["Author"]
    L42 --> L421("Get employee by comme.AuthorId")
    L421 --> L4
    L3 --> L43["CreatedAt"]
    L43 --> L431("Get comment.CreatedAt")
    L431 --> L4
    H2 --> Z1("Zendesk Ticket")
    I2 --> Z1
    J3 --> Z1
    K6 --> Z1
    M2 --> Z1
    N2 --> Z1
    L4 --> Z1
```

# Versions

### 0.0.9

- Clean up/normalizing data imported from Zendesk.
- Polish search results.

### 0.0.8

- Data importer improvements.

### 0.0.7

- Add support for local LLMs by integrating with Ollama.

### 0.0.6

- Integrate with Zendesk API.

### 0.0.5

- Integrate with Azure DevOps to fetch work items.

### 0.0.4

- Add an option to use Full-Text Search over Zendesk tickets.

### 0.0.3

- Refactoring of the codebase - change the whole structure of project.
- Add MongoDB database integration.
- Store Zendesk tickets into two databases.

### 0.0.2

- Get Zendesk tickets from a sample JSON file.
- Add Qdrant database integration.
- Add docker compose.

### 0.0.1

- Initial version with Semantic Kernel and OpenAI integration.