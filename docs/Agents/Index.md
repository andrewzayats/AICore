# AI Core Agents

## Overview

Agents in AI Core provide a flexible, low-code approach to designing AI workflows, allowing users to orchestrate, schedule, and manage various tasks that work with large language models (LLMs), data storage, APIs, and more. Similar in concept to Semantic Kernel plugins, these agents enable users to create sophisticated, automated AI-related flows with minimal development effort.

---

### Table of Contents

1. [Introduction](#introduction)
2. [Agent Types](#agent-types)
3. [Setting Up and Managing Agents](#setting-up-and-managing-agents)
4. [Composite Agents](#composite-agents)
5. [Tags and Permissions](#tags-and-permissions)
6. [Scheduling and Execution](#scheduling-and-execution)
7. [Import and Export](#import-and-export)
8. [Error Handling and Debugging](#error-handling-and-debugging)
9. [Agent Configuration](#agent-configuration)
10. [Best Practices and Limitations](#best-practices-and-limitations)

---

## Introduction

Agents are modular components that execute specific tasks, whether handling LLM prompts, connecting to data stores, calling APIs, or transforming media such as audio or images. These agents can be individually configured, scheduled, tagged, and managed. Admins can create and edit agents to build flows that range from simple task sequences to complex, interdependent workflows managed by planners.

### Key Features
- **Flexible Agent Types**: Includes LLM agents, data storage connectors, API callers, and more.
- **Scheduling Capabilities**: CRON-based scheduling for periodic execution.
- **Composite Agents**: Allows for the creation of multi-step workflows with other agents.
- **Tags & Permissions**: Access can be restricted using Active Directory-based tags.
- **Debug Mode**: Comprehensive troubleshooting information is available when running agents in Debug Mode.

---

## Agent Types

Agents serve different purposes and can handle tasks that vary from language processing to data storage and beyond. Each agent type has unique parameters and settings, defined at creation.

### Supported Agent Types

1. **Azure Search Agent**
   - *Purpose*: Enabling seamless integration with Azure AI Search services.

2. **Azure AI Speech (Text to Speech) Agent**
    - *Purpose*: Enables the conversion of text data into spoken words using Azure's cognitive services.

3. **Azure AI Translator Agent**
    - *Purpose*: Enables seamless translation of text from one language to another using Microsoft's Cognitive Services.

4. **Redis Agent**
    - *Purpose*: Enables efficient, scalable caching and data storage management using Redis.

5. **SQL Server Agent**
    - *Purpose*: Enables automated execution of SQL queries within workflows.

6. **PostgreSQL Agent**
    - *Purpose*: Designed to connect seamlessly to PostgreSQL databases, execute SQL scripts, and return structured results in JSON format.

7. **Scheduler Agent**
    - *Purpose*: Designed to automate the execution of other agents at predefined intervals. 

8. **Storage Account Agent**
    - *Purpose*: Facilitates the interaction with Azure Storage Accounts, enabling users to perform a variety of storage operations such as listing files, adding files, deleting files, and retrieving file contents. 

9. **Vector Search Agent**
    - *Purpose*: Enables users to perform vector-based searches within their AI workflows.

10. **Whisper Agent**
    - *Purpose*: Enables the transcription of audio files into text, leveraging Azure's Whisper service.

11. **Image To Text Agent**
    - *Purpose*: Designed to convert images into text using Optical Character Recognition (OCR) technology and process the recognized text through large language models (LLMs).

12. **Content Safety Agent**
    - *Purpose*: Designed to ensure that content adheres to safety standards by analyzing text for hate, self-harm, sexual content, violence, protected materials, and jail break attacks.

13. **Background Worker Agent**
    - *Purpose*: Enables the asynchronous execution of Composite Agents, providing a scalable and resilient solution for managing long-running or resource-intensive tasks.

14. **OCR Agent**
    - *Purpose*: Designed to process images and extract text data using Optical Character Recognition (OCR) technology.

15. **RAG Prompt Agent**
    - *Purpose*: Designed to enhance the capabilities of Large Language Models (LLMs) by retrieving relevant information from a vector database before generating responses.

16. **Messages History Agent**
    - *Purpose*: This agent allows users to access a specified number of recent messages, enabling them to maintain context in conversations and streamline interactions with large language models (LLMs).

17. **Bing Agent**
    - *Purpose*: Accesses Bing search and related services.

18. **Code Execution Agents (C# / Python)**
   - *Purpose*: Executes custom code, either in C# or Python, to handle specific logic.

19. **Composite Agent**
    - *Purpose*: Designed for creating complex, multi-step AI workflows.

20. **Contains Agent**
    - *Purpose*: Designed to identify specific data patterns within a given text using regular expressions (regex).

21. **JSON Transform Agent**
    - *Purpose*: Enables users to transform JSON data efficiently and dynamically as part of a larger workflow.

22. **API Call Agent**
    - *Purpose*: Allows users to make HTTP requests to external APIs. This agent type is versatile, supporting RESTful requests, CRUD operations, custom headers, authentication, and various response handling methods.

23. **Prompt Agent**
    - *Purpose*: Designed to execute tasks centered around language generation and interaction with Large Language Models (LLMs) via prompt-based inputs.


---

## Setting Up and Managing Agents

### Permissions and Roles

Only **Admin** users have permission to create, edit, or manage agents. Additionally:
- Each Admin can enable or disable agents and assign tags to limit access.
- Tags can be defined at the user or user group level, synced with Active Directory.

### Creating an Agent

1. **Choose Agent Template**: Select a template that defines the fields and configuration required for the agent type.
2. **Specify Parameters**: Define all parameters relevant to the agent type (e.g., prompt and connection for LLM agents).
3. **Assign Tags** (optional): Limit access by assigning tags, making the agent available only to users with matching tags.

---

## Composite Agents

Composite Agents are advanced agents that consist of multiple other agents and can execute complex workflows. These agents have their own planner, allowing for hierarchical workflows and logical branching.

### Key Features of Composite Agents

- **Hierarchical Flow**: Composite Agents can execute other agents, including other Composite Agents, enabling complex workflows.
- **Planner with Handlebars Logic**: Composite Agents use a Handlebars-based plan to determine the sequence and conditions of agent execution.
- **Agent-Specific Enable/Disable**: Even if an agent is disabled globally, it can be enabled within a Composite Agent’s scope.

### Sample Handlebars Plan

Composite Agents are defined using Handlebars logic. Below is a sample plan that demonstrates key-value extraction and conditional execution:

```handlebars
{{!-- Step 0: Extract key values --}}
{{set "ticketNumber" "{{parameter1}}"}}
{{set "comment" "{{parameter2}}"}}

{{!-- Step 1: Get ticket details --}}
{{set "ticketDetails" (getticketdetailsPlugin-getticketdetails ticketNumber)}}

{{!-- Step 2: Prettify comment --}}
{{set "prettifiedComment" (prettifycommentPlugin-prettifycomment comment ticketDetails)}}

{{!-- Step 3: Output the prettified comment --}}
{{json prettifiedComment}}
```

---

## Tags and Permissions

Tags in AI Core are used to control access to agents. Tags are managed at the user or user group level and can be synced with Active Directory for Role-Based Access Control (RBAC).

### Key Points

- **Tag Assignment**: Tags can be applied to an agent to restrict access to users or groups with matching tags.
- **Access Control**: Agents without a matching tag cannot be accessed by users, and the global planner will not consider them.
- **Tag Synchronization**: Tags are synced with Active Directory RBAC Groups and Roles for seamless management.

---

## Scheduling and Execution

Scheduling in AI Core is managed using **CRON expressions** that define the frequency and timing of agent execution.

- **Supported Format**: Only CRON expressions are supported; there are no custom scheduling options.
- **Error Handling and Retries**: Currently, there is no built-in retry mechanism for failed tasks.

---

## Import and Export

Agents can be imported and exported between instances of AI Core, enabling easy transfer of workflows.

### Import/Export Format

- **Format**: Agents are exported as **ZIP** files containing individual JSON files for each agent.
- **Usage**: Files can be imported/exported across any AI Core instances, and there are no built-in encryption or signing features.

---

## Error Handling and Debugging

### Debug Mode

Debug Mode allows users to view detailed execution information for each agent, including:
- **Agent Execution**: Details on each executed agent, including parameters, input, output, and execution time.
- **Token Usage**: Summary of token consumption for each LLM or external API call.
- **Error Information**: If an agent fails, Debug Mode displays the failure details.

### Enable Debug mode
- Settings -> General, switch to "Yes"
- In Chat window enable "Use Debug Mode":
- And test it:

### Error Reporting in Composite Agents

If an agent within a Composite Agent fails:
- The flow stops at the failed agent.
- Debug Mode highlights the failed agent and provides detailed error information.

---

## Agent Configuration

Each agent instance includes a set of configuration options that vary depending on its type. For example:
- **LLM Agent**: Prompt text, connection configuration.
- **C# Code Agent**: Code content and input/output descriptions.

### Standard Agent Parameters

Agents support **string** inputs and outputs. Even if the data is binary, it is encoded as a **base64 string**. This allows agents to handle a wide variety of data types without complex conversions.

---

## Best Practices and Limitations

### Best Practices

1. **Use Predefined Plans**: Predefined Handlebars plans are preferred, as they save time and tokens and ensure tested accuracy.
2. **Limit Dynamic Plans**: While planners can create plans dynamically, it is better to use predefined logic where possible for predictability and efficiency.

### Limitations

- **Error Handling**: There is no built-in error retry mechanism.
- **No Persistent Logs**: Interaction logs are not permanently stored; only Debug Mode offers temporary insights.
- **Limited Data Types**: Agents only support string inputs and outputs.

---

By leveraging AI Core's robust agent framework, users can create and manage a diverse range of AI-driven workflows efficiently, achieving complex orchestration with minimal coding. Through features such as tags, Composite Agents, Debug Mode, and import/export, AI Core provides a versatile platform for orchestrating AI functionalities at scale.