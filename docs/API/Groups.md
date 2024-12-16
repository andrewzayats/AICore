# Groups Management API

## Overview

The **Groups Management API** provides functionality to manage user groups efficiently. User groups allow administrators to organize users and manage their access by assigning tags to groups rather than individual users. This simplifies administration and enhances scalability when dealing with large user bases or roles requiring similar access levels.

### Key Features:
- **Add/Edit Groups**: Create new groups or update existing ones.
- **Assign Users to Groups**: Bulk management of users within a group.
- **Assign Tags to Groups**: Tags applied to a group are automatically applied to all group members.

---

## API Endpoints

### 1. **Get Group by ID**
**Endpoint**: `GET /api/v1/groups/{groupId}`  
**Description**: Retrieve details of a specific group.  

#### Parameters:
- `groupId` *(required)*: `integer($int32)` - The unique identifier for the group.

#### Response:
- **200 OK**: Returns the details of the group.  

---

### 2. **Update a Group**
**Endpoint**: `PUT /api/v1/groups/{groupId}`  
**Description**: Update an existing group, including its name, description, tags, and members.  

#### Parameters:
- `groupId` *(required)*: `string` - The unique identifier for the group.  

#### Request Body:
```json
{
  "groupId": 0,
  "name": "string",
  "description": "string",
  "created": "2024-12-16T11:43:24.915Z",
  "createdBy": "string",
  "tags": [
    "string"
  ],
  "logins": [
    {
      "loginId": 0,
      "login": "string",
      "fullName": "string",
      "role": "string",
      "loginType": "string",
      "created": "2024-12-16T11:43:24.915Z",
      "createdBy": "string",
      "tags": [
        "string"
      ],
      "groups": [
        "string"
      ],
      "tokensLimit": 0
    }
  ]
}
```

#### Response:
- **200 OK**: The group was successfully updated.

---

### 3. **Get All Groups**
**Endpoint**: `GET /api/v1/groups`  
**Description**: Retrieve a list of all groups. 

#### Parameters:
- No parameters required.


#### Response:
- **200 OK**: Returns a list of all groups.

---

### 4. **Create a New Group**

**Endpoint**: `POST /api/v1/groups`  
**Description**: Create a new group with specified details, tags, and members. 


#### Request Body:

```json
{
  "groupId": 0,
  "name": "string",
  "description": "string",
  "created": "2024-12-16T11:43:24.917Z",
  "createdBy": "string",
  "tags": [
    "string"
  ],
  "logins": [
    {
      "loginId": 0,
      "login": "string",
      "fullName": "string",
      "role": "string",
      "loginType": "string",
      "created": "2024-12-16T11:43:24.917Z",
      "createdBy": "string",
      "tags": [
        "string"
      ],
      "groups": [
        "string"
      ],
      "tokensLimit": 0
    }
  ]
}
```

#### Response:
- **200 OK**: The group was successfully created.

---

## Notes

1. **ag Assignment:**: Tags assigned to a group are automatically applied to all its members, streamlining permissions management.
2. **User Management:**: Users can be added or removed from a group to adjust their access dynamically.
3. **Scalability**: Grouping facilitates efficient administration for large organizations with role-based access control needs.