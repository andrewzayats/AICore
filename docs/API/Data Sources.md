# Data Sources

## Overview

Data Sources in AI Core enable seamless data ingestion from various locations into a Vector Database, allowing Agents to leverage the ingested information for tasks such as Vector Search and Retrieval-Augmented Generation (RAG). Administrators can set up Data Sources with granular control over connections, file exclusions, embedding configurations, and access permissions.


## Introduction

The Data Sources feature in AI Core allows administrators to connect various data sources, such as SharePoint, web URLs, and file uploads, and ingest data into a Vector Database. This data ingestion process is designed to support downstream applications by enabling rich search and retrieval functionalities that AI Agents, like the Vector Search and RAG Agents, can utilize.

This document outlines the setup, configuration, and management of Data Sources, as well as best practices for ensuring data is ingested efficiently and securely.



---

## Data Source Types

AI Core currently supports the following Data Source types for ingestion:

1. **SharePoint**: Allows for connection to specific SharePoint folders for file ingestion.
2. **Web URL**: Enables ingestion of web-based content from specific URLs.
3. **File Upload**: Supports direct upload of individual or ZIP-compressed files for ingestion.

Each type has unique parameters required to establish a connection and configure the ingestion process effectively.


---

## Ingestion Parameters and Configuration

Each Data Source type includes configurable ingestion parameters that define the specifics of data ingestion, such as connection credentials, paths, file exclusions, and embeddings.

### 1. SharePoint Ingestion

The SharePoint Ingestion type allows ingestion of files from designated folders within SharePoint. Parameters for SharePoint Ingestion include:

- **Embedding Connection Name**  
  - *Purpose*: Specifies the connection used to generate embeddings for ingested data.  
  - *Format*: Must match an existing embedding connection in the system.  
  - *Example*: `"MyAzureEmbeddingConnection"`

- **Vector DB Connection**  
  - *Purpose*: Azure AI Search / Qdrant connection to store vector data.  
  - *Format*: Must match an existing Azure AI Search connection or Internal Qdrant.  
  - *Example*: `"MyAzureAiSeachConnection"`

- **SharePoint Connection**  
  - *Purpose*: Defines the SharePoint connection settings, such as URLs and authentication credentials.  
  - *Format*: Configured separately under connections, with details specific to SharePoint environments.  
  - *Example*: `"CompanySharePointConnection"`

- **Path**  
  - *Purpose*: Specifies the folder path within SharePoint where files are located.  
  - *Format*: Start with a forward slash `/` and follow the directory structure.  
  - *Example*: `"/Shared Documents/Project Files"`

- **Exclude Folders**  
  - *Purpose*: Allows exclusion of certain folders from ingestion to focus on relevant files only.  
  - *Format*: Comma-separated paths with quotes for folders with spaces.  
  - *Example*: `"/Folder1", "/Folder2/SubFolder3", "/Folder Name"`

- **Exclude Extensions**  
  - *Purpose*: Excludes files with specific extensions to prevent ingestion of unsupported formats.  
  - *Format*: Comma-separated list of extensions, including the dot prefix.  
  - *Example*: `".pdf, .docx, .xlsx"`

### 2. Web URL Ingestion

The Web URL Ingestion type enables ingestion from publicly accessible URLs. _(Web crawing is not supported, just specified URL will be ingested)_ Parameters include:

- **Embedding Connection Name**  
  - *Purpose*: Embedding connection for generating embeddings.  
  - *Format*: An existing embedding connection name.  
  - *Example*: `"MyWebEmbeddingConnection"`

- **Vector DB Connection**  
  - *Purpose*: Azure AI Search / Qdrant connection to store vector data.  
  - *Format*: Must match an existing Azure AI Search connection or Internal Qdrant.  
  - *Example*: `"MyAzureAiSeachConnection"`

- **URL**  
  - *Purpose*: Specifies the URL to ingest content from.  
  - *Format*: Full web URL.  
  - *Example*: `"https://www.microsoft.com/en-us/legal/"`

### 3. File Upload Ingestion

The File Upload Ingestion type supports direct file uploads, either as single files or as ZIP archives. Parameters include:

- **Embedding Connection Name**  
  - *Purpose*: Embedding connection for uploaded file data.  
  - *Format*: Must match an existing embedding connection.  
  - *Example*: `"FileUploadEmbeddingConnection"`

- **Vector DB Connection**  
  - *Purpose*: Azure AI Search / Qdrant connection to store vector data.  
  - *Format*: Must match an existing Azure AI Search connection or Internal Qdrant.  
  - *Example*: `"MyAzureAiSeachConnection"`

- **File**  
  - *Purpose*: The file(s) to upload and ingest. Supports ZIP archives for multiple files.  
  - *Format*: Direct file upload.  
  - *Example*: `"project_documents.zip"`


---

## Scheduling and Synchronization

### Synchronization for SharePoint

SharePoint Data Sources support scheduled synchronization, allowing AI Core to:
- Detect changes, additions, deletions, and modifications within the designated SharePoint folder.
- Update the Vector DB to reflect these changes, ensuring that ingested data is current.


---

## Tags and Access Control

Tags are a central part of access control within AI Core, ensuring only authorized users can access specific data sources and embeddings.

### Tag Assignment and Synchronization

- **Assignment**: Tags can be assigned to Data Sources, restricting access to users or groups with matching tags.
- **Synchronization**: AI Core syncs tags with Active Directory RBAC groups, enabling role-based access management.
- **Use Case**: A Data Source with a “Finance” tag can only be accessed by users with the same “Finance” tag.

--- 

## Best Practices

### Data Size and Scope

- Avoid ingesting large, unnecessary files by setting **Exclude Folders** and **Exclude Extensions** to limit data scope.
- Use tags to restrict access to sensitive data.

### Embedding Efficiency

- Choose embedding settings appropriate for the type and volume of data.
- Optimize embedding frequency for data types that do not require frequent updates.

---

## Troubleshooting and Limitations

### Common Issues

- **Failed Authentication**: Verify connection settings for SharePoint or other sources.
- **Embedding Errors**: Ensure the embedding connection is correctly configured and active.

### Limitations

- **Data Types**: Currently, Data Sources only support text and certain encoded binary data.
- **Logging**: Interaction logs are temporarily stored; Debug Mode provides insights for troubleshooting but does not offer persistent logs.

---

## API


### 1. **Get Ingestion Details**

**GET** `/api/v1/ingestions/{ingestionId}`

Retrieve details of a specific data ingestion.

#### Parameters:
- **ingestionId** (required) - `integer($int32)`

#### Responses:
- **200 OK**: Returns the details of the specified ingestion.


#### Response Sample:
```
{
    "ingestionId": 3,
    "name": "AI Accelerator docs",
    "note": "",
    "type": 1,
    "content": {
        "Path": "/AI Accelerator docs/",
        "ConnectionId": "8",
        "EmbeddingConnection": "7"
    },
    "tags": [
        {
            "tagId": 2,
            "name": "Docs",
            "description": "Wiki copy tag",
            "color": "#fdeccc",
            "created": "2024-12-02T03:49:23.556267Z",
            "createdBy": "admin@viacode.com",
            "groups": [],
            "logins": [],
            "ingestions": []
        }
    ],
    "created": "2024-12-02T03:50:20.534604Z",
    "createdBy": "admin@viacode.com",
    "updated": "2024-12-02T03:50:20.557439Z",
    "lastSync": "2024-12-16T12:11:40.041368Z",
    "isLastSyncFailed": false,
    "lastSyncFailedMessage": null,
    "status": 1
}
```

---

### 2. **Update Ingestion**

**PUT** `/api/v1/ingestions/{ingestionId}`

Update the details of a specific data ingestion.

#### Parameters:
- **ingestionId** (required) - `string`

#### Request Body:
```json
{
  "ingestionId": 0,
  "name": "string",
  "note": "string",
  "type": 1,
  "content": {
    "additionalProp1": "string",
    "additionalProp2": "string",
    "additionalProp3": "string"
  },
  "tags": [
    {
      "tagId": 0,
      "name": "string",
      "description": "string",
      "color": "string"
    }
  ],
  "status": 1
}
```

#### Responses:
- **200 OK**: Indicates successful update.


---

### 3. **Delete Ingestion**

**DELETE** `/api/v1/ingestions/{ingestionId}`

Delete a specific data ingestion.

#### Parameters:
- **ingestionId** (required) - `string`


#### Responses:
- **200 OK**: Indicates successful update.


---

### 4. **Get All Ingestions**

**GET** `/api/v1/ingestions`

Retrieve a list of all data ingestions.

#### Parameters:
- None


#### Responses:
- **200 OK**: Returns a list of ingestions.


---

### 5. **Create New Ingestion**

**POST** `/api/v1/ingestions`

Create a new data ingestion.

#### Request Body:
```json
{
  "ingestionId": 0,
  "name": "string",
  "note": "string",
  "type": 1,
  "content": {
    "additionalProp1": "string",
    "additionalProp2": "string",
    "additionalProp3": "string"
  },
  "tags": [
    {
      "tagId": 0,
      "name": "string",
      "description": "string",
      "color": "string"
    }
  ],
  "status": 1
}
tatus": 1
}
```


#### Responses:
- **200 OK**: Indicates successful creation.


---

### 6. **Get Ingestion Tasks**

**GET** `/api/v1/ingestions/tasks`

Retrieve a list of tasks related to data ingestions.

#### Parameters:
- None


#### Responses:
- **200 OK**: Returns a list of ingestion tasks.


---

### 7. **Synchronize Ingestion**

**POST** `/api/v1/ingestions/{ingestionId}/sync`

Trigger a synchronization for a specific ingestion.

#### Parameters:
- **ingestionId** (required) - `integer($int32)`


#### Responses:
- **200 OK**: Indicates successful synchronization.



---

### 8. **Autocomplete for Parameters**

**POST** `/api/v1/ingestions/autocomplete/{parameterName}`

Retrieve autocomplete suggestions for specified parameters.

#### Parameters:
- **parameterName** (required) - `string`

#### Request Body:
```json
{
  "name": "string",
  "type": 1,
  "content": {
    "additionalProp1": "string"
  }
}

```


#### Responses:
- **200 OK**: Returns autocomplete suggestions for the parameter.




## Status Codes
- **200 OK**: Operation completed successfully.
- **400 Bad Request**: Request contains invalid parameters or data.
- **404 Not Found**: The specified resource does not exist.
- **500 Internal Server Error**: An error occurred on the server.


## Best Practices
- Use appropriate tags and metadata to categorize and manage ingestions effectively.
- Monitor synchronization tasks and handle any errors promptly using logs or system alerts.



## Limitations
- Currently, the API supports text-based data sources and selected binary formats.
- Web crawling is not supported; only specified URLs can be ingested.