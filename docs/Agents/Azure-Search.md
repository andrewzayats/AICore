# Azure AI Search Agent

## Overview

The Azure AI Search Agent is a powerful tool within the AI Core framework, enabling seamless integration with Azure AI Search services. It facilitates Search, Add, Update, and Delete operations in Azure AI Search indexes, allowing users to efficiently manage and query their data. By leveraging AI Core’s robust agent system, the Azure AI Search Agent can be embedded in larger AI workflows, empowering businesses to build intelligent, data-driven applications.

-----------------

## Table of Contents

1.  [Introduction to Azure AI Search Agent](#introduction-to-azure-ai-search-agent)
2.  [Key Features](#key-features)
3.  [How It Works](#how-it-works)
4.  [Parameters and Configuration](#parameters-and-configuration)
5.  [Sample Use Case](#sample-use-case)
5.  [Sample Configuration](#sample-configuration)
6.  [Where It Can Be Useful](#where-it-can-be-useful)
7.  [Best Practices](#best-practices)
8.  [Limitations](#limitations)

------------

## Introduction to Azure AI Search Agent

The Azure AI Search Agent connects AI Core workflows with Azure AI Search services. Azure AI Search is a cloud-based search-as-a-service solution that provides sophisticated search capabilities, including full-text search, filtering, and scoring. The agent simplifies the process of interacting with Azure AI Search by abstracting complex API calls into an intuitive configuration.
The agent supports both **document management** (Add, Update, Delete) and **query execution** (Search), making it a versatile tool for managing large datasets.

------------

## Key Features

-   **Full CRUD Support**: Perform Create, Read, Update, and Delete operations on Azure AI Search indexes.
-   **Dynamic Query Execution**: Build and execute powerful search queries using placeholders.
-   **Auto-Retrieval of Indexes**: Automatically fetch available indexes when configuring the agent.
-   **Integration with Composite Agents**: Use within workflows to combine search operations with other tasks.
-   **Debug Mode**: Provides detailed logs for troubleshooting and optimization.

------------

## How It Works

### Goal

The primary goal of the Azure AI Search Agent is to simplify interactions with Azure AI Search, enabling users to:
1.  Efficiently retrieve and manage data stored in Azure AI Search indexes.
2.  Build intelligent workflows by combining search capabilities with AI Core’s planning and orchestration features.

### Logic

The agent communicates with Azure AI Search services via REST APIs. Depending on the operation (Search, Add, Update, or Delete), the agent dynamically constructs API requests using the provided parameters.
1.  **Search Operations**:
(https://learn.microsoft.com/en-us/rest/api/searchservice/preview-api/search-documents)
    *   Constructs a search query string using placeholders.
    *   Sends the query to the appropriate Azure AI Search index.
    *   Returns search results in a structured JSON format.
2.  **Add/Update/Delete Operations**:
(https://learn.microsoft.com/en-us/rest/api/searchservice/preview-api/add-update-delete-documents)
    *   Interprets the action type (`@search.action`) from the query string.
    *   Directs the request to the indexing endpoint for document modifications.

### Hidden Capabilities

-   **Dynamic Query Construction**: The agent supports up to nine placeholders (`{{parameter1}}` to `{{parameter9}}`) for building complex, dynamic queries.
-   **Output Guidance**: The agent provides descriptions for LLM-based workflows to determine subsequent steps automatically.
-   **Planner Integration**: Supports conditional execution and logic-driven workflows in Composite Agents.

------------

## Parameters and Configuration

### Input Parameters

| Parameter Name | Description | Type | Required | Additional Notes |
| --- | --- | --- | --- | --- |
| **Azure AI Search Connection Name** | The name of the connection to Azure AI Search. | Connection | Yes | Must be preconfigured in AI Core with valid API key and endpoint details. |
| **Index Name** | The name of the Azure AI Search index to query or modify. | Azure AI Search Index | Yes | Automatically retrieved based on the connection. |
| **Parameters Description** | Describes the parameters used in the query. | Text | No | Used for documentation purposes within the agent configuration. |
| **Search/Add/Update/Delete Query String** | The query string for the operation. Supports placeholders (`{{parameter1}}`, etc.). | Text Area | Yes | Query syntax must align with Azure AI Search API standards. |
| **Output Description** | Description of the expected output. Helps guide LLMs on subsequent actions. | Text | Yes | Used to improve planner decisions and workflow integration. |
| **Planner Instruction** | Optional text for the planner, providing additional guidance for workflows. | Text | No | Useful for embedding search logic in Composite Agents. |

------------


### Output

The Azure AI Search Agent returns results in JSON format. Results are structured as an array of matching documents, with each document containing fields defined in the Azure AI Search index schema.
**Example Output**:

    [  
      {  
        "id": "1",  
        "title": "Document 1",  
        "content": "This is the first document."  
      },  
      {  
        "id": "2",  
        "title": "Document 2",  
        "content": "This is the second document."  
      }  
    ]  
    

------------

## Sample Use Case

### Scenario: Building a Search-Driven Chatbot

**Objective**: Enhance a customer support chatbot by integrating it with Azure AI Search to retrieve product information.
1.  **Setup**:
    -   Configure an Azure AI Search index with product details (e.g., name, description, price).
    -   Create an Azure AI Search Agent in AI Core and link it to the index.
2.  **Workflow**:
    -   User inputs a query (e.g., "Show me laptops under $1000").
    -   The agent parses the query and searches the Azure AI Search index for relevant products.
    -   Results are formatted and presented to the user via the chatbot interface.

------------

## Sample Configuration

### Create Index
Create the new Index Using Azure Portal:

#### Columns: 
- id
- text
- name

### Create Connection in AI Core

Configure the new Azure AI Search connection in AI Core:

### Add the data
Setup ad run the following Agent:

**Agent Type:** Azure AI Search
**Name:** Azure AI Search - Add Data
**Description:**  Sample agent to add Azure Services specific data in Azure AI Search
**Index:** aicore-index
**Parameters Description:** _leave empty_
**Search / Add / Update / Delete Query String:**
```json
{
    "value": [
        {
            "id": "1",
            "name": "Azure App Service",
            "text": "Azure App Service is a fully managed platform for building, deploying, and scaling web apps. You can host web apps, mobile app backends, and RESTful APIs. It supports a variety of programming languages and frameworks, such as .NET, Java, Node.js, Python, and PHP. The service offers built-in auto-scaling and load balancing capabilities. It also provides integration with other Azure services, such as Azure DevOps, GitHub, and Bitbucket.",
            "@search.action": "upload"
        },
        {
            "id": "2",
            "name": "Azure Functions",
            "text": "Azure Functions is a serverless compute service that enables you to run event-driven code without having to manage infrastructure. It supports various triggers like HTTP requests, queue messages, and timers. It is ideal for scenarios like data processing, automation, and backend logic for apps.",
            "@search.action": "upload"
        },
        {
            "id": "3",
            "name": "Azure Virtual Machines",
            "text": "Azure Virtual Machines provide on-demand, scalable computing resources. They support a wide range of operating systems and can host applications requiring high performance, security, and reliability. Ideal for development, testing, and running enterprise applications.",
            "@search.action": "upload"
        },
        {
            "id": "4",
            "name": "Azure Kubernetes Service (AKS)",
            "text": "Azure Kubernetes Service simplifies the deployment and management of containerized applications using Kubernetes. It offers automated upgrades, scaling, and monitoring, integrating seamlessly with Azure services and CI/CD pipelines.",
            "@search.action": "upload"
        },
        {
            "id": "5",
            "name": "Azure SQL Database",
            "text": "Azure SQL Database is a fully managed relational database service. It provides built-in high availability, backups, and scaling, supporting workloads requiring high performance and security. Features include AI-powered performance optimization and compliance certifications.",
            "@search.action": "upload"
        },
        {
            "id": "6",
            "name": "Azure Cosmos DB",
            "text": "Azure Cosmos DB is a globally distributed, multi-model database service designed for high availability and low latency. It supports document, key-value, graph, and column-family data models, making it ideal for applications with diverse data needs.",
            "@search.action": "upload"
        },
        {
            "id": "7",
            "name": "Azure Storage",
            "text": "Azure Storage provides scalable, secure, and durable cloud storage solutions for various types of data. It includes Blob storage, File storage, Queue storage, and Table storage, supporting data-intensive workloads and archiving.",
            "@search.action": "upload"
        },
        {
            "id": "8",
            "name": "Azure Logic Apps",
            "text": "Azure Logic Apps enable you to automate workflows and integrate systems with minimal code. It supports connecting cloud-based and on-premises services, offering pre-built connectors for popular platforms like Salesforce, Office 365, and SAP.",
            "@search.action": "upload"
        },
        {
            "id": "9",
            "name": "Azure Cognitive Services",
            "text": "Azure Cognitive Services provide pre-built AI models for vision, speech, language, and decision-making. They enable developers to integrate advanced AI features into applications without requiring extensive machine learning expertise.",
            "@search.action": "upload"
        },
        {
            "id": "10",
            "name": "Azure DevOps",
            "text": "Azure DevOps is a comprehensive suite of tools for CI/CD, project management, and code collaboration. It includes Azure Pipelines, Repos, Boards, Test Plans, and Artifacts, supporting agile workflows and DevSecOps practices.",
            "@search.action": "upload"
        },
        {
            "id": "11",
            "name": "Azure Data Factory",
            "text": "Azure Data Factory is a cloud-based data integration service that allows you to create data-driven workflows for orchestrating data movement and transformation. It supports ETL processes and integrates with various data stores.",
            "@search.action": "upload"
        },
        {
            "id": "12",
            "name": "Azure Monitor",
            "text": "Azure Monitor provides full-stack monitoring for applications, infrastructure, and networks. It includes capabilities for logging, metrics, and diagnostics, enabling proactive management of performance and health.",
            "@search.action": "upload"
        },
        {
            "id": "13",
            "name": "Azure Machine Learning",
            "text": "Azure Machine Learning is a cloud-based environment for building, training, and deploying machine learning models. It supports MLOps, provides pre-built algorithms, and integrates with other Azure services for scalable deployments.",
            "@search.action": "upload"
        }
    ]
}
```
**Output Description:** -

### Search the data
Setup ad run the following Agent:

**Agent Type:** Azure AI Search
**Name:** Azure AI Search - Run Search
**Description:** Sample agent to search the data in Azure AI Search
**Index:** aicore-index
**Parameters Description:** Search String
**Search / Add / Update / Delete Query String:**
```json
{
  "search": "{{parameter1}}",
  "select": "text, name",
  "top": 3
}
```
**Output Description:** Search results from Azure AI Search


------------


## Where It Can Be Useful

### Typical Use Cases

-   **Enterprise Search Solutions**: Integrate with corporate databases to build internal search tools.
-   **eCommerce Applications**: Enhance product search functionality for online stores.
-   **Content Management Systems**: Organize and retrieve large volumes of documents efficiently.
-   **Data-Driven Chatbots**: Power conversational agents with precise, context-aware search capabilities.
-   **Dynamic Reporting**: Fetch and format data for dashboards and analytics workflows.

------------

## Best Practices


1.  **Optimize Queries**: Use placeholders and dynamic query strings to tailor searches to user inputs.
2.  **Test in Debug Mode**: Always test new agents in Debug Mode to identify and resolve issues early.
3.  **Combine with Composite Agents**: Leverage Composite Agents for more complex workflows that combine search with additional logic.
4.  **Document Index Schema**: Maintain clear documentation for Azure AI Search index schemas to streamline agent configuration.

------------

## Limitations

-   **Query Complexity**: The agent cannot validate query syntax. Errors in query strings may cause failures.
-   **Data Types**: Limited to string-based inputs and outputs. Binary data must be encoded as Base64.

------------

By understanding and leveraging the Azure AI Search Agent, users can build robust, intelligent workflows that harness the full power of Azure AI Search. Its integration with AI Core ensures that search capabilities can seamlessly blend with other AI-driven processes, opening the door to innovative solutions across industries.