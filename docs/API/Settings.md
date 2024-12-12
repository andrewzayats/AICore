# Settings

This document provides detailed information about the **Settings** API. The Settings API allows users to retrieve, modify, and manage system configurations. This documentation is designed to assist developers in understanding and utilizing these APIs effectively.

---

## Overview
The Settings API enables external systems to interact with configuration parameters of the AI solution. These settings control various functionalities such as database connections, authentication, ingestion limits, and more. By using this API, users can:
- Retrieve current system configurations.
- Update specific settings programmatically.
- Reboot the service to apply certain configuration changes.

---

## API Endpoints

### /api/v1/settings (GET)

#### Description
Retrieve the current configuration settings of the system.

#### Request
- **Method**: `GET`
- **Parameters**: None

#### Response
- **HTTP Status Code**: `200`
- **Body**: A JSON object containing all the current settings categorized into their respective sections.

#### Example Response
```json
[
  {
    "settingId": "qdrantUrl",
    "description": "The URL of the Qdrant database used for vector search.",
    "value": "http://host.docker.internal:6338",
    "dataType": "string",
    "tooltip": "Connection endpoint for the Qdrant vector search engine.",
    "category": "General Settings"
  }
]
```

---

### /api/v1/settings (POST)

#### Description
Update one or more system settings.

#### Request
- **Method**: `POST`
- **Content-Type**: `application/json-patch+json`
- **Body**: An array of settings to update.

#### Request Body Example
```json
[
  {
    "settingId": "MaxFileSize",
    "description": "The maximum file size in bytes that can be ingested into the system. Files larger than this size will not be processed.",
    "value": "209715200 (200 MB)",
    "dataType": "Integer",
    "tooltip": "The maximum file size.",
    "category": "General Settings"
  }
]
```

#### Response
- **HTTP Status Code**: `200`

---

### /api/v1/settings/reboot (POST)

#### Description
Reboot the service to apply configuration changes that require a restart.

#### Request
- **Method**: `POST`
- **Parameters**: None

#### Response
- **HTTP Status Code**: `200`

---

### /api/v1/settings/ui (GET)

#### Description
Retrieve settings specific to the application's user interface.

#### Request
- **Method**: `GET`
- **Parameters**: None

#### Response
- **HTTP Status Code**: `200`
- **Body**: A JSON object containing UI-related settings.

#### Example Response
```json
[
  {
    "settingId": "mainColor",
    "description": "Specifies the main color of the application.",
    "value": "#1976d2",
    "dataType": "string",
    "tooltip": "Primary color for branding.",
    "category": "UI Theme Settings"
  }
]
```

---

## Examples

### Retrieve Settings
**Request**:
```http
GET /api/v1/settings HTTP/1.1
Host: your-ai-domain.com
```

**Response**:
```json
[
  {
    "settingId": "qdrantUrl",
    "description": "The URL of the Qdrant database used for vector search.",
    "value": "http://host.docker.internal:6338",
    "dataType": "string",
    "tooltip": "Connection endpoint for the Qdrant vector search engine.",
    "category": "Common Settings"
  }
]
```

---

### Update Settings
**Request**:
```http
POST /api/v1/settings HTTP/1.1
Host: your-ai-domain.com
Content-Type: application/json-patch+json

[
  {
    "settingId": "useSearchTab",
    "description": "Enable or disable the Search tab in the application.",
    "value": "true",
    "dataType": "boolean",
    "tooltip": "Toggle the Search tab visibility.",
    "category": "Common Settings"
  }
]
```

**Response**:
```http
HTTP/1.1 200 OK
```

---

### Reboot Service
**Request**:
```http
POST /api/v1/settings/reboot HTTP/1.1
Host: your-ai-domain.com
```

**Response**:
```http
HTTP/1.1 200 OK
```

---

## Notes
- Some settings changes may require a reboot to take effect.
- Ensure that the proper authorization headers are included in all requests for security.
- All endpoints return HTTP status codes to indicate the success or failure of the operation.