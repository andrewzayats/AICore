# Connections

## Overview
AI Core Connections provide a streamlined approach to configuring and managing the necessary credentials and parameters for accessing various external resources. Once a connection is defined, it can be referenced within agents, eliminating the need to repeatedly specify credentials and endpoints. This ensures secure, efficient, and scalable AI workflow management.

## Benefits of Using Connections
- **Efficiency**: Define credentials and endpoints once and reuse them across multiple agents.
- **Security**: Centralize and secure sensitive information like API keys and passwords.
- **Scalability**: Simplify the management of credentials and configurations as workflows grow.
- **Consistency**: Ensure uniformity in how resources are accessed across different agents and workflows.

## Supported Connection Types
- SharePoint Connection
- Azure OpenAI LLM Connection
- Azure OpenAI Embedding Connection
- Bing API Connection
- Azure Whisper Connection
- Azure Document Intelligence Connection
- Azure AI Content Safety Connection
- Azure Storage Account Connection
- PostgreSQL Connection
- SQL Server Connection
- Redis Connection
- Azure AI Translator Connection
- Azure AI Speech Connection

## API Endpoints

### List All Connections
#### `GET /api/v1/connections`
Retrieve all defined connections.

**Responses**
- **200 OK**: A list of connections is returned.

---

### Create a New Connection
#### `POST /api/v1/connections`
Create a new connection with the required parameters.

**Request Body**
```json
{
  "connectionId": 0,
  "name": "string",
  "type": 1,
  "content": {
    "additionalProp1": "string",
    "additionalProp2": "string",
    "additionalProp3": "string"
  },
  "created": "2024-12-12T09:34:26.946Z",
  "createdBy": "string",
  "canBeDeleted": true
}
```

**Responses**
- **200 OK**: Connection created successfully.

---

### Update an Existing Connection
#### `PUT /api/v1/connections/{connectionId}`
Update an existing connection by specifying its ID.

**Parameters**
- `connectionId` (string): The ID of the connection to update.

**Request Body**
```json
{
  "connectionId": 0,
  "name": "string",
  "type": 1,
  "content": {
    "additionalProp1": "string",
    "additionalProp2": "string",
    "additionalProp3": "string"
  },
  "created": "2024-12-12T09:34:26.947Z",
  "createdBy": "string",
  "canBeDeleted": true
}
```

**Responses**
- **200 OK**: Connection updated successfully.

---

### Delete a Connection
#### `DELETE /api/v1/connections/{connectionId}`
Delete a specific connection by its ID.

**Parameters**
- `connectionId` (integer): The ID of the connection to delete.

**Responses**
- **200 OK**: Connection deleted successfully.

---

### Retrieve a Specific Connection
#### `GET /api/v1/connections/{connectionId}`
Retrieve details of a specific connection by its ID.

**Parameters**
- `connectionId` (integer): The ID of the connection to retrieve.

**Responses**
- **200 OK**: Connection details returned.

## Best Practices
- **Centralize Connection Management**: Define connections at the start of your project to ensure consistent and secure access across all agents.
- **Regularly Update Credentials**: Periodically update API keys and passwords to maintain security.
- **Use Descriptive Names**: When naming connections, use clear and descriptive names to easily identify their purpose and associated resource.
- **Test Connections**: After creating a connection, always test it to ensure it works correctly before deploying agents that rely on it.
- **Monitor Usage**: Keep track of token usage and API call limits to avoid unexpected interruptions in service.

## Conclusion
By leveraging AI Core Connections, users can efficiently manage the credentials and configurations needed to interact with various external resources. This centralized approach enhances security, reduces redundancy, and simplifies the setup of complex AI-driven workflows. Whether accessing data storage, performing API calls, or integrating language models, AI Core Connections provide a robust and scalable solution for orchestrating AI functionalities.

By following best practices and understanding the capabilities and limitations of connections, users can maximize the effectiveness of their AI workflows, ensuring reliable and secure interactions with external resources.