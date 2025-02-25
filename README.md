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

**[Full Agents Guide](https://github.com/VIAcode/AICore/blob/main/docs/Agents/Index.md)**

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


# Deployment with Helm

AI Core services can be deployed to Kubernetes using Helm. The Helm charts are located in the `./helm` folder.

The Helm charts include a `values.yaml` file that defines default configurations. However, the following values must be explicitly specified during deployment:

- `global.app.domain`: The application domain (e.g., `ai.application.com`).
- `tls.crt`: Base64-encoded TLS certificate.
- `tls.key`: Base64-encoded TLS certificate key.

## Deployment Examples

### Minimal Deployment
Deploy AI Core API with an internal PostgreSQL instance, using the latest images from DockerHub. The File Ingestion service is not deployed.

```sh
helm upgrade aicore "./helm" --namespace $namespace --create-namespace --install --kubeconfig $kubeconfig \
    --set global.app.domain=$hostName \
    --set tls.crt=$tlsCrt \
    --set tls.key=$tlsKey
```

### Deploying from Azure Container Registry
Deploy AI Core API with an internal PostgreSQL instance, using images from Azure Container Registry with specified tags. The File Ingestion service is not deployed.

```sh
helm upgrade aicore "./helm" --namespace $namespace --create-namespace --install --kubeconfig $kubeconfig \
    --set global.app.domain=$hostName \
    --set tls.crt=$tlsCrt \
    --set tls.key=$tlsKey \
    --set global.containerRegistry.name=$acrName.azurecr.io \
    --set global.containerRegistry.dockerConfig=$containerRegistryAuthBase64 \
    --set global.aicore.service.tag=$aiCoreTag \
    --set global.ingestion.service.tag=$ingestionTag
```

### Deploying from Azure Container Registry with Entra ID Authentication
If using Azure Kubernetes Service with a managed identity that has access to ACR, you do not need to specify the `dockerConfig` file.

```sh
helm upgrade aicore "./helm" --namespace $namespace --create-namespace --install --kubeconfig $kubeconfig \
    --set global.app.domain=$hostName \
    --set tls.crt=$tlsCrt \
    --set tls.key=$tlsKey \
    --set global.containerRegistry.name=$acrName.azurecr.io \
    --set global.aicore.service.tag=$aiCoreTag \
    --set global.ingestion.service.tag=$ingestionTag
```

### Deploying with an External PostgreSQL Server
In this example, AI Core will use an external PostgreSQL server.

```sh
helm upgrade aicore "./helm" --namespace $namespace --create-namespace --install --kubeconfig $kubeconfig \
    --set global.app.domain=$hostName \
    --set tls.crt=$tlsCrt \
    --set tls.key=$tlsKey \
    --set global.containerRegistry.name=$acrName.azurecr.io \
    --set global.aicore.service.tag=$aiCoreTag \
    --set global.ingestion.service.tag=$ingestionTag \
    --set api.postgres.internal=false \
    --set api.postgres.host=$dbHost \
    --set api.postgres.port=5432 \
    --set api.postgres.userName=$dbAdministratorLogin \
    --set api.postgres.password=$dbAdministratorPassword
```

## Values File Reference

### Global Values

| Key | Default Value | Description |
|------|---------------|-------------|
| `tls.crt` |  | Base64-encoded TLS certificate |
| `tls.key` |  | Base64-encoded TLS certificate key |
| `fileIngestion.enabled` | `false` | Enable or disable File Ingestion service |
| `global.app.name` | `aicore` | Application name |
| `global.app.domain` |  | Application domain |
| `global.app.logLevel` | `Information` | Logging level |
| `global.app.enableMonitoring` | `false` | Enable monitoring |
| `global.containerRegistry.dockerConfig` |  | Docker registry configuration |
| `global.containerRegistry.name` | `docker.io/viacode` | Container registry name |
| `global.containerRegistry.imagePullPolicy` | `Always` | Image pull policy |
| `global.environment.namespace` | `aicore-ns` | Kubernetes namespace |
| `global.environment.name` | `myapp` | Environment name |
| `global.aicore.service.tag` | `latest` | AI Core service image tag |
| `global.aicore.service.port` | `8005` | AI Core service port |
| `global.ingestion.maxParallelism` | `2` | Maximum parallel ingestion operations |
| `global.ingestion.requestTimeout` | `"00:15:00"` | Ingestion request timeout |
| `global.ingestion.service.tag` | `latest` | Ingestion service image tag |
| `global.ingestion.service.urlPrefix` | `"ingestion-api"` | Ingestion service URL prefix |
| `global.ingestion.service.port` | `8021` | Ingestion service port |
| `global.ingestion.qdrant.port` | `8016` | Qdrant service port |

### AI Core API Values

| Key | Default Value | Description |
|------|---------------|-------------|
| `api.postgres.storageSize` | `16Gi` | PostgreSQL storage size |
| `api.postgres.internal` | `True` | Use internal PostgreSQL |
| `api.postgres.host` |  | External PostgreSQL host |
| `api.postgres.port` | `5432` | PostgreSQL port |
| `api.postgres.dbName` | `aicoredb` | Database name |
| `api.postgres.userName` | `aicoredbuser@viacode.com` | Database user name |
| `api.postgres.password` | `default` | Database password |
| `api.redis.port` | `6379` | Redis cache port |
| `api.redis.userName` | `aicatalyst@viacode.com` | Redis cache user name |
| `api.redis.password` | `AiCatalystIsTheBest!` | Redis cache password |
| `api.aicore.containerPort` | `8080` | AI Core service container port |
| `api.aicore.ingestion.delay` | `10` | |
| `api.aicore.ingestion.maxFileSize` | `209715200` | |


### File Ingestion Service Values

| Key | Default Value | Description |
|------|---------------|-------------|
| `fileIngestion.service.containerPort` | `7880` | Service container port |
| `fileIngestion.qdrant.containerPort` | `6333` | Qdrant container port |



# Deployment with Docker

This guide explains how to deploy AI Core using the pre-built Docker images. Follow these steps to get the solution up and running efficiently.

---

## Prerequisites

Before deploying, ensure you have the following installed and configured on your system:

1. **Docker**:
   - Download and install Docker from [Docker's official website](https://www.docker.com/).
   - Verify the installation by running:
     ```bash
     docker --version
     ```
2. **Docker Hub Account** (Optional):
   - If the Docker images are private, log in to Docker Hub using your credentials:
     ```bash
     docker login
     ```

---

## Steps to Deploy the Solution

### 1. Pull the Docker Images

The pre-built Docker images for AI Core are hosted on Docker Hub. To download the images, run:

#### Ingestion Service Image
```bash
docker pull viacode/ai-core-file-ingestion:latest
```

#### API Service Image
```bash
docker pull viacode/ai-core:latest
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
INGESTION_PORT=5000
API_PORT=8081
```

Ensure you replace the placeholders with actual values.

---

### 3. Run the Docker Containers

#### 3.1 Start the Ingestion Service Container

Start the ingestion service container using the following command:

```bash
docker run -d \
  --name ai-core-file-ingestion \
  -p 8080:8080 \
  --env-file .env \
  viacode/ai-core-file-ingestion:latest
```

#### 3.2 Start the API Service Container

Start the API service container using the following command:

```bash
docker run -d \
  --name ai-core \
  -p 8081:8081 \
  --env-file .env \
  viacode/ai-core:latest
```

---

### 4. Verify the Deployment

To ensure the containers are running, execute:

```bash
docker ps
```

You should see both containers listed. Verify the logs to ensure everything is working correctly:

#### Ingestion Service Logs
```bash
docker logs ai-core-file-ingestion
```

#### API Service Logs
```bash
docker logs ai-core
```

Access the services in your browser:

- Ingestion Service: `http://localhost:8080`
- API Service: `http://localhost:8081`

---

### 5. Manage the Docker Containers

Here are some useful commands to manage the containers:

- **Stop the containers**:
  ```bash
  docker stop ai-core-file-ingestion ai-core
  ```

- **Restart the containers**:
  ```bash
  docker restart ai-core-file-ingestion ai-core
  ```

- **Remove the containers**:
  ```bash
  docker rm ai-core-file-ingestion ai-core
  ```

- **Remove the Docker images** (if needed):
  ```bash
  docker rmi viacode/ai-core-file-ingestion:latest viacode/ai-core:latest
  ```

---
 
Happy deploying!

