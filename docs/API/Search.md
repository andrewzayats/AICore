# Search

## Overview

The Search API in AI Core empowers users to perform full-text and vector-based searches across indexed documents. With support for role-based access control (RBAC), and embedding models, the Search API serves as a powerful resource for locating relevant content with precision.

Users interact with the Search API through several parameters, including the connection name, query string, tagging options. AI Core’s search prioritize relevance, allowing users to quickly access critical information across large data sets.

## Sample API Request Format

The Search API retrieve search results based on the query. Admins can replicate or automate these requests using the following format:

- **API Endpoint**: `/api/v1/copilot/search`
- **Query Parameters**:
  - `connection_name`: Specifies the embedding connection name, e.g., `My Embedding`.
  - `q`: Contains the search query, e.g., `search text`.
  - `tags`: Comma-separated list of tag IDs, e.g., `1,2,3,4,5,6,7,8`.

- **Sample Request**:
```
  GET: http://localhost:7878/api/v1/copilot/search?connection_name=My%20Embedding&q=search%20text&tags=1,2,3,4,5,6,7,8
  HEADER: Authorization: Bearer bearer-token-value
```

  - **Response Sample**:
```
  [
    {
        "sender": "",
        "text": "",
        "link": "https://viacodeai.sharepoint.com/sites/AICore/Shared%20Documents/AI%20Accelerator%20docs/Search%20-%20Overview.pdf",
        "createTime": "2024-12-02T03:39:38Z",
        "updatedTime": "2024-12-02T03:39:38Z",
        "sourceContentType": "application/pdf",
        "sourceName": "Search - Overview.pdf",
        "texts": [
            {
                "text": "Last updated by | Andrey Kerchin | Nov 5, 2024 at 6:09 PM GMT+7\nSearch\nAI Core: Search Page Documentation\nOverview\nTable of Contents\n1. Introduction\n2. Enabling the Search Page\n3. Search Page Components\n4. Embedding Model Selection\n5. Tag Management and Security\n6. Executing Searches\n7. Interpreting Search Results\n8. Using Audio Search\n9. Sample API Request Format\n10. Legacy Vector Search Functionality\n11. Best Practices and Considerations\nIntroduction\nThe AI Core Search Page is designed to facilitate efficient and flexible information retrieval by leveraging\nadvanced vector search capabilities. This page enables users to query indexed data, utilize embedding models,\nand apply security tags for controlled access, all within an intuitive interface. The integration of AI models,\nembedding connections, and Whisper transcription offers users a versatile, searchable knowledge base.\nThe Search Page in AI Core empowers users to perform full-text and vector-based searches across indexed\ndocuments. With support for role-based access control (RBAC), embedding models, and audio transcription, the\nSearch Page serves as a powerful resource for locating relevant content with precision.\nUsers interact with the Search Page through several key components, including the search field, model\nselection, tagging options, and search results display. AI Core’s search features prioritize relevance, allowing\n12/2/24, 10:26 AM Search - Overview\nhttps://dev.azure.com/viacode/AI Catalyst/_wiki/wikis/AI-Catalyst.wiki/1786/Search 1/6Enabling the Search Page\n1. Navigate to Settings in the AI Core platform.\n2. Select General settings.\n3. Set USE SEARCH TAB IN MAIN MENU to Yes.\nusers to quickly access critical information across large data sets.\nBy default, the Search Page tab is not visible to users. Admin users can enable it by following these steps:\n12/2/24, 10:26 AM Search - Overview\nhttps://dev.azure.com/viacode/AI Catalyst/_wiki/wikis/AI-Catalyst.wiki/1786/Search 2/64. Wait 10 seconds and refresh the page.\nOnce enabled, the Search Page tab will appear in the main menu, making it accessible for user\ninteractions.\nSearch Page Components\n1. Tags: Tags are used to apply security filters, allowing users to restrict which indexed documents are\nsearchable based on assigned tags.\n2. Model Selection: Admins can select embedding models configured in the Connections tab. These models\ngenerate vector embeddings from the user’s search queries.\n3. Search Area (Results): Displays search results, including document chunks organized by relevance.\n4. Search Field: A single-line text box where users can enter search terms.\n5. Audio Recording Button: An optional feature for initiating voice search through Whisper transcription.\n6. Send Request Button: Executes the search request based on entered text or recorded audio.\nEmbedding Model Selection\n1. Embedding Models: Select from available embedding models in the Connections tab. These models\nconvert search terms into vector embeddings, which enable similarity-based searching.\n2. Usage in Vector DB: Embedding models generate embeddings for both document text and search terms.\nThe specified model is used to generate embeddings for the search query, and these embeddings are\nmatched with the document embeddings stored in the Vector Database.\n3. Supported Models: Only Azure OpenAI Embedding models are supported within AI Core for now.\nExample Model Selection Choose an embedding model such as text-embedding-ada-002 in the Connections tab for vector-based\nsearching.\nThis model will then be used to convert search text into vector representations.\nTag Management and Security\nThe Search Page includes several components, each integral to performing and interpreting searches. These\ncomponents are:\nThe embedding model selection determines how the AI Core processes search queries and indexes data for",
                "relevance": 0.37947773933410645,
                "partNumber": 0
            }
        ]
    }
  ]
```

This format provides flexibility for automated search requests and programmatic interactions with AI Core.

## Best Practices and Considerations

1. **Tag Utilization**: Apply relevant tags to indexed documents to enforce security and filter search results.
2. **Model Selection**: Choose embedding models that align with the document corpus, ensuring vector search quality.
3. **Optimize Queries**: Use concise and specific search terms for more accurate retrieval.
