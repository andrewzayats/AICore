# Tags Management API

## Overview

Tags are essential for controlling access to data sources and agents. They can be assigned to data sources to restrict access based on user roles and permissions.

### Key Features
- **Tags Assignment**: Tags can be assigned to data sources.
- **User Tags**: Users have tags that limit their access to specific data sources.
- **Tag Synchronization**: Tags are synced with Active Directory RBAC Groups and Roles.
- Tags are color-coded for easier recognition.

---

## API Endpoints

### 1. **Get Tag by ID**
**Endpoint**: `GET /api/v1/tags/{tagId}`  
**Description**: Retrieve details of a specific tag.

#### Parameters:
- `tagId` *(required)*: `integer($int32)` - The unique identifier for the tag.

#### Response:
- **200 OK**: Returns the tag details.

---

### 2. **Update a Tag**
**Endpoint**: `PUT /api/v1/tags/{tagId}`  
**Description**: Update an existing tag.

#### Parameters:
- `tagId` *(required)*: `string` - The unique identifier for the tag.

#### Request Body:
```json
{
  "tagId": 0,
  "name": "string",
  "description": "string",
  "color": "string",
  "created": "2024-12-16T11:18:22.597Z",
  "createdBy": "string",
  "groups": [
    {
      "groupId": 0,
      "name": "string",
      "description": "string",
      "created": "2024-12-16T11:18:22.597Z",
      "createdBy": "string",
      "tags": ["string"],
      "logins": [
        {
          "loginId": 0,
          "login": "string",
          "fullName": "string",
          "role": "string",
          "loginType": "string",
          "created": "2024-12-16T11:18:22.597Z",
          "createdBy": "string",
          "tags": ["string"],
          "groups": ["string"],
          "tokensLimit": 0
        }
      ]
    }
  ],
  "logins": [
    {
      "loginId": 0,
      "login": "string",
      "fullName": "string",
      "role": "string",
      "loginType": "string",
      "created": "2024-12-16T11:18:22.597Z",
      "createdBy": "string",
      "tags": ["string"],
      "groups": ["string"],
      "tokensLimit": 0
    }
  ],
  "ingestions": [
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
      "tags": ["string"],
      "created": "2024-12-16T11:18:22.597Z",
      "createdBy": "string",
      "updated": "2024-12-16T11:18:22.597Z",
      "lastSync": "2024-12-16T11:18:22.597Z",
      "isLastSyncFailed": true,
      "lastSyncFailedMessage": "string",
      "status": 1
    }
  ]
}
```

- **200 OK**: Returns the tag details.

---

### 3. *Get All Tags**
**Endpoint**: `GET /api/v1/tags`  
**Description**: Retrieve a list of all tags.

#### Response:
- **200 OK**: Returns the tag details.

---

### 4. *Create a New Tag**
**Endpoint**: `POST /api/v1/tags`  
**Description**: Create a new tag..

#### Response:
- **200 OK**: Returns the tag details.

#### Request Body:
```json
{
  "tagId": 0,
  "name": "string",
  "description": "string",
  "color": "string",
  "created": "2024-12-16T11:18:22.600Z",
  "createdBy": "string",
  "groups": [
    {
      "groupId": 0,
      "name": "string",
      "description": "string",
      "created": "2024-12-16T11:18:22.600Z",
      "createdBy": "string",
      "tags": ["string"],
      "logins": [
        {
          "loginId": 0,
          "login": "string",
          "fullName": "string",
          "role": "string",
          "loginType": "string",
          "created": "2024-12-16T11:18:22.600Z",
          "createdBy": "string",
          "tags": ["string"],
          "groups": ["string"],
          "tokensLimit": 0
        }
      ]
    }
  ],
  "logins": [
    {
      "loginId": 0,
      "login": "string",
      "fullName": "string",
      "role": "string",
      "loginType": "string",
      "created": "2024-12-16T11:18:22.600Z",
      "createdBy": "string",
      "tags": ["string"],
      "groups": ["string"],
      "tokensLimit": 0
    }
  ],
  "ingestions": [
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
      "tags": ["string"],
      "created": "2024-12-16T11:18:22.600Z",
      "createdBy": "string",
      "updated": "2024-12-16T11:18:22.600Z",
      "lastSync": "2024-12-16T11:18:22.600Z",
      "isLastSyncFailed": true,
      "lastSyncFailedMessage": "string",
      "status": 1
    }
  ]
}
```

- **200 OK**: Returns the tag details.

---

### 5. *Get Tags for Current User**
**Endpoint**: `GET /api/v1/tags/my`
**Description**: List tags available for the current user. Users cannot perform chat or search actions without a tag if the TAGGING feature is enabled.

#### Response:
- **200 OK**:  Returns a list of tags available to the current user.

---

## Notes
- Color-Coding: Tags are color-coded for easier recognition.
- Tag Restrictions: Tags restrict access to users or groups who have matching tags.
- RBAC Integration: Tags synchronize with Active Directory RBAC Groups and Roles.