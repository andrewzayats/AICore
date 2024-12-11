# AICore

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/VIAcode/AICore/blob/main/LICENSE)

<img src="https://github.com/VIAcode/AICore/blob/main/aicore.jpg?raw=true" alt="AI Core">

The AI Core is an open-source toolkit that streamlines the development, deployment, and management of AI-based applications. Whether you’re leveraging Azure AI, Azure OpenAI, or custom models, this platform provides an all-in-one solution. With built-in support for agent workflows, session management, user management, and cost tracking, the platform ensures a secure and scalable environment for all your AI needs. The platform is available as a source code and a Docker image for seamless integration.

## 💡 Key Features
-	AI Models Integration: Manage different types of AI models effortlessly, with built-in support for Azure Open AI, Azure AI, Azure AI Document Intelligence and custom models.
-	Agents & Flows: Enable complex workflows and AI-driven tasks with customizable agents and composite workflows.
-	RAG skills: link with data sources and use retrieval-augmented generation to gather information
-	User Management: Manage users and groups securely, with support for Single Sign-On (SSO) and Microsoft Entra ID integration.
-	Cost Control: Gain full visibility and control over AI-related costs by managing usage across users, models, and agents.
-	AI Jobs Scheduler: Schedule and run background agents or workflows, automating repetitive tasks to improve efficiency.

## 🤖 Agents
AI Core includes agents, modular components that enhance your AI workflows by automating specific tasks or integrating third-party services. Agents act as independent processes within the system, facilitating functions such as:

- Data preprocessing or enrichment
- Triggering external APIs
- Scheduling tasks or recurring jobs
- Supporting domain-specific operations
  
Developers can create custom agents or use predefined ones available in the platform to tailor solutions to specific needs. This modular design ensures flexibility and reduces development time for complex AI-driven applications.

## 🪄 Available LLMs
Large Language Models (LLMs) are advanced AI models trained on vast amounts of text data to understand and generate human-like language. They can perform a variety of tasks, including answering questions, summarizing documents, writing code, and engaging in conversational interactions. LLMs are essential for building intelligent, language-based applications such as chatbots, virtual assistants, and content generation tools.

AI Core currently supports the following LLMs:

- GPT-4o-mini: A lightweight version optimized for fast inference and low-resource environments, making it ideal for quick-response scenarios with minimal infrastructure requirements.
- GPT-4o: A more powerful model offering greater accuracy and broader language capabilities, suitable for demanding tasks like advanced natural language understanding and summarization.
  
With these models, AI Core ensures a balanced approach, providing options for both performance-efficient and high-accuracy solutions, meeting a variety of project needs.

## 💬 Chat
Playground 
AI Core offers a Chat Area for data handling and models testing where users can:

- Experiment with data inputs and outputs, gaining insights into model behavior in real time.
- Test various models and configurations.
- Simulate workflows, ensuring that agents, data flows, and AI models work together as intended.
- Visualize key metrics such as response times, accuracy, and cost impact of various AI tasks.
- Chat provides a safe, user-friendly environment to prototype ideas quickly and debug workflows before full-scale deployment.

# AI Core API

## Search

### Overview

The Search API in AI Core empowers users to perform full-text and vector-based searches across indexed documents. With support for role-based access control (RBAC), and embedding models, the Search API serves as a powerful resource for locating relevant content with precision.

Users interact with the Search API through several parameters, including the connection name, query string, tagging options. AI Core’s search prioritize relevance, allowing users to quickly access critical information across large data sets.

### Sample API Request Format

The Search API retrieve search results based on the query. Admins can replicate or automate these requests using the following format:

- **API Endpoint**: `/api/v1/copilot/search`
- **Query Parameters**:
  - `connection_name`: Specifies the embedding connection name, e.g., `My Embedding`.
  - `q`: Contains the search query, e.g., `search text`.
  - `tags`: Comma-separated list of tag IDs, e.g., `1,2,3,4,5,6,7,8`.

- **Sample Request**:
  ```plaintext
  GET: http://localhost:7878/api/v1/copilot/search?connection_name=My%20Embedding&q=search%20text&tags=1,2,3,4,5,6,7,8
  HEADER: Authorization: Bearer bearer-token-value
  ```

This format provides flexibility for automated search requests and programmatic interactions with AI Core.

### Best Practices and Considerations

1. **Tag Utilization**: Apply relevant tags to indexed documents to enforce security and filter search results.
2. **Model Selection**: Choose embedding models that align with the document corpus, ensuring vector search quality.
3. **Optimize Queries**: Use concise and specific search terms for more accurate retrieval.






## Settings, Users, Debug
// todo

## Deployment
// todo

## Usage examples
// todo
