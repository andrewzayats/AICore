# Chat

## Overview
The Chat API provides an interface for interacting with the AI Core platform. This API is designed to facilitate communication with AI agents, enabling users to send text-based requests, attach files, and access debug information. It is a central tool for setting up, testing, and debugging agent workflows.

## Endpoint
### `POST /api/v1/copilot/chat`
The Chat endpoint is the main entry point for communication with AI Core.

---

### Parameters

| Parameter         | Type    | Description                                                                                                                                     | Default Value  |
|-------------------|---------|-------------------------------------------------------------------------------------------------------------------------------------------------|----------------|
| `tags`            | string  | Comma-separated Tag IDs. Use `/api/v1/tags/my` to fetch available tags for the user. If no tags are specified and the TAGGING feature is enabled, no response is returned. | `0`            |
| `connection_name` | string  | Connection Name from the 'Connections' tab. Used as the default LLM for all agents. If not specified, the first connection is used.           |                |
| `use_markdown`    | boolean | Indicates whether to use markdown in the response output.                                                                                     | `true`         |
| `use_bing`        | boolean | Specifies whether to use the Bing Agent (if enabled). Ignored if the Bing Agent is disabled.                                                 | `false`        |
| `use_cached_plan` | boolean | Allows executing the request without using the cached plan.                                                                                  | `true`         |
| `use_debug`       | boolean | Enables debug information in the response.                                                                                                  | `false`        |

---

### Request Body

#### Schema
```json
{
  "messages": [
    {
      "sender": "string",
      "text": "string",
      "sources": [
        {
          "name": "string",
          "url": "string"
        }
      ],
      "files": [
        {
          "name": "string",
          "size": 0,
          "base64Data": "string"
        }
      ],
      "debugMessages": [
        {
          "sender": "string",
          "dateTime": "2024-12-13T09:11:51.362Z",
          "title": "string",
          "details": "string"
        }
      ],
      "spentTokens": {
        "additionalProp1": {
          "request": 0,
          "response": 0
        },
        "additionalProp2": {
          "request": 0,
          "response": 0
        },
        "additionalProp3": {
          "request": 0,
          "response": 0
        }
      },
      "options": [
        {
          "type": 1,
          "name": "string",
          "parameters": {
            "additionalProp1": "string",
            "additionalProp2": "string",
            "additionalProp3": "string"
          }
        }
      ]
    }
  ]
}
```

#### Example
```json
{
  "messages": [
    {
      "sender": "user",
      "text": "Who is Harry Potter?",
      "files": [],
      "sources": []
    }
  ]
}
```

---

### Response

#### Schema
```json
{
  "messages": [
    {
      "sender": "string",
      "text": "string",
      "sources": [
        {
          "name": "string",
          "url": "string"
        }
      ],
      "files": [
        {
          "name": "string",
          "size": 0,
          "base64Data": "string"
        }
      ],
      "debugMessages": [
        {
          "sender": "string",
          "dateTime": "2024-12-13T09:11:51.363Z",
          "title": "string",
          "details": "string"
        }
      ],
      "spentTokens": {
        "additionalProp1": {
          "request": 0,
          "response": 0
        },
        "additionalProp2": {
          "request": 0,
          "response": 0
        },
        "additionalProp3": {
          "request": 0,
          "response": 0
        }
      },
      "options": [
        {
          "type": 1,
          "name": "string",
          "parameters": {
            "additionalProp1": "string",
            "additionalProp2": "string",
            "additionalProp3": "string"
          }
        }
      ]
    }
  ]
}
```

#### Example
```json
{
  "messages": [
    {
      "sender": "assistant",
      "text": "Harry Potter is a fictional character and the protagonist of the \"Harry Potter\" series of books written by British author J.K. Rowling. The series consists of seven books that follow Harry's life from his childhood to adulthood. Harry discovers on his eleventh birthday that he is a wizard, and he is invited to attend Hogwarts School of Witchcraft and Wizardry."
    }
  ]
}
```

---

### Sample Usage Scenarios

#### Example 1: Regular Request

**Request**
```
POST http://ai-core-service/api/v1/copilot/chat?connection_name=GPT4o&use_bing=true&use_debug=true&use_cached_plan=true&tags=1,2,3,4,5,6,7,8
```
**Body**
```json
{
  "messages": [
    {
      "sender": "user",
      "text": "Who is Harry Potter?",
      "files": [],
      "sources": []
    }
  ]
}
```

**Response**
```json
{
  "messages": [
    {
      "sender": "user",
      "text": "Who is Harry Potter?"
    },
    {
      "sender": "assistant",
      "text": "Harry Potter is a fictional character and the protagonist of the \"Harry Potter\" series of books written by British author J.K. Rowling."
    }
  ]
}
```

#### Example 2: Debug Mode Request with Options

**Request**
```
POST http://localhost:7878/api/v1/copilot/chat?connection_name=GPT4o&use_bing=false&use_debug=true&use_cached_plan=true&tags=5,6,7,8
```
**Body**
```json
{
  "messages": [
    {
      "sender": "user",
      "text": "",
      "options": [
        {
          "type": 1,
          "name": "Answer the question",
          "parameters": {
            "parameter1": "Who is Harry Potter?"
          }
        }
      ]
    }
  ]
}
```

**Response**
```json
{
  "messages": [
    {
      "sender": "assistant",
      "text": "Harry Potter is a fictional character and the protagonist of the \"Harry Potter\" series."
    }
  ]
}
