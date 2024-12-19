# AICore Overview

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

# AI Core API References

- [Auth, User Management, and Permissions API.md](https://github.com/VIAcode/AICore/blob/main/docs/API/Auth%2C%20User%20Management%2C%20and%20Permissions.md)
- [Chat API](https://github.com/VIAcode/AICore/blob/main/docs/API/Chat.md)
- [Connections API](https://github.com/VIAcode/AICore/blob/main/docs/API/Connections.md)
- [Cost API](https://github.com/VIAcode/AICore/blob/main/docs/API/Cost.md)
- [Data Sources API](https://github.com/VIAcode/AICore/blob/main/docs/API/Data%20Sources.md)
- [Groups API](https://github.com/VIAcode/AICore/blob/main/docs/API/Groups.md)
- [Search API](https://github.com/VIAcode/AICore/blob/main/docs/API/Search.md)
- [Settings API](https://github.com/VIAcode/AICore/blob/main/docs/API/Settings.md)
- [Tags API](https://github.com/VIAcode/AICore/blob/main/docs/API/Tags.md)


# Deployment

This guide explains how to deploy VIAcode AI Core using the pre-built Docker images. Follow these steps to get the solution up and running efficiently.

---

## Prerequisites

Before deploying, ensure you have the following installed and configured on your system:

1. **Docker**:
   - Download and install Docker from [Docker's official website](https://hub.docker.com/r/viacode/ai-core/).
   - Verify the installation by running:
     ```bash
     docker --version
     ```
2. **Docker Hub Account** (Optional):
   - If the Docker image is private, log in to Docker Hub using your credentials:
     ```bash
     docker login
     ```
---

## Steps to Deploy the Solution

### 1. Pull the Docker Image

The pre-built Docker image for AI Core is hosted on Docker Hub. To download the image, run:

```bash
docker pull viacode/aicore:latest
```

---

### 2. Create Environment Configuration (Optional)

Some configurations may require environment variables. Create a `.env` file in your project directory to define these variables:

```env
APP_ENV=production
APP_PORT=8080
DB_HOST=your-database-host
DB_USER=your-database-user
DB_PASSWORD=your-database-password
```

Ensure you replace the placeholders with actual values.

---

### 3. Run the Docker Container

Start the container using the following command:

```bash
docker run -d \
  --name aicore \
  -p [host-port]:[container-port] \
  --env-file .env \
  viacode/aicore:[tag]
```

- Replace `[host-port]` with the port on your host machine (e.g., `8080`).
- Replace `[container-port]` with the port the application listens to inside the container (e.g., `8080`).
- Replace [tag] with the specific version tag (e.g., latest, v1.0.0). If unsure, use `latest`.

---

### 4. Verify the Deployment

To ensure the container is running, execute:

```bash
docker ps
```

You should see your container listed. Verify the logs to ensure everything is working correctly:

```bash
docker logs aicore
```

Access the application in your browser at `http://localhost:8080`.

---

### 5. Manage the Docker Container

Here are some useful commands to manage the container:

- **Stop the container**:
  ```bash
  docker stop [container-name]
  ```

- **Restart the container**:
  ```bash
  docker restart [container-name]
  ```

- **Remove the container**:
  ```bash
  docker rm [container-name]
  ```

- **Remove the Docker image** (if needed):
  ```bash
  docker rmi [your-dockerhub-username]/[your-solution-name]:[tag]
  ```

---

## Additional Notes

1. **Customizing the Deployment**:
   - If your application requires persistent storage (e.g., for databases), use Docker volumes. Example:
     ```bash
     docker run -d \
       --name aicore \
       -p 8080:8080 \
       -v /path/to/data:/data \
       --env-file .env \
       viacode/aicore:latest
     ```

2. **Scaling with Docker Compose**:
   - For more complex deployments involving multiple containers (e.g., frontend, backend, and database), consider using Docker Compose. Create a `docker-compose.yml` file and include the relevant services.

3. **Troubleshooting**:
   - If you encounter issues, check the container logs:
     ```bash
     docker logs aicore
     ```
   - Make sure all dependencies (e.g., database, external APIs) are accessible.

---

Happy deploying!


## Usage examples
// todo
