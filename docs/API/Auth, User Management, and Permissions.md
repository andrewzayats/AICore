# Authentication, User Management, and Permissions

This document provides details about the Authentication, User Management, and Permissions APIs for AI Core. These APIs enable secure authentication, user profile management, and fine-grained access control for agents and data sources.

---

## Overview

AI Core provides robust authentication mechanisms, including internal login, Microsoft SSO integration, and API Basic Authentication. User Management allows the creation and management of users, roles, and groups, while Permissions offer flexible tag-based access control.

---

## Authentication API

### 1. Authorization Endpoint

#### **GET** `/api/v1/connect/authorize`

**Parameters:**
- `client_id` (string): Application's client ID.
- `redirect_uri` (string): Redirect URI.
- `code_challenge` (string): Code challenge for PKCE.
- `code_challenge_method` (string): Code challenge method.
- `acr_values` (string): Authentication context class reference values.
- `scope` (string): Scope of the request.
- `response_type` (string): Type of response.
- `state` (string): State to maintain request-response integrity.

**Responses:**
- `200`: Successful authorization.

#### **POST** `/api/v1/connect/authorize`

**Request Body:**
- `multipart/form-data` with the same parameters as the `GET` request.

**Responses:**
- `200`: Authorization successful.

### 2. Token Endpoint

#### **POST** `/api/v1/connect/token`

**Request Body:**
- `grant_type` (string): Authorization grant type.

**Responses:**
- `200`: Token issued successfully.

### 3. Logout Endpoint

#### **GET** `/api/v1/connect/logout`

**Parameters:**
- `id_token_hint` (string): ID token to indicate logout.
- `post_logout_redirect_uri` (string): Redirect URI after logout.

**Responses:**
- `200`: Logout successful.

### 4. Callback Endpoint

#### **POST** `/api/v1/connect/callback`

**Request Body:**
- `code` (string): Authorization code.
- `state` (string): State from the request.
- `error_description` (string): Error details (if applicable).

**Responses:**
- `200`: Callback handled successfully.

---

## User Management API

### 1. User Profile Management

#### **GET** `/api/v1/user/{loginId}`

**Parameters:**
- `loginId` (integer): Unique identifier of the user.

**Responses:**
- `200`: User data retrieved.

#### **PUT** `/api/v1/user/{loginId}`

**Parameters:**
- `loginId` (integer): Unique identifier of the user.

**Request Body:**
- `fullName` (string): Full name of the user.
- `email` (string): Email of the user.
- `role` (string): Role assigned to the user.
- `isEnabled` (boolean): Status of the user.

**Responses:**
- `200`: User updated successfully.

---

## Permissions API

### 1. RBAC Group Synchronization

#### **GET** `/api/v1/rbac/groups`

**Responses:**
- `200`: Group data retrieved.

#### **POST** `/api/v1/rbac/groups`

**Request Body:**
- JSON structure with `rbacGroupSyncId`, `rbacGroupName`, `aiCoreGroupName`, and metadata fields.

**Responses:**
- `200`: Group created successfully.

#### **PUT** `/api/v1/rbac/groups`

**Request Body:**
- Same as `POST` request.

**Responses:**
- `200`: Group updated successfully.

#### **DELETE** `/api/v1/rbac/groups/{rbacGroupSyncId}`

**Parameters:**
- `rbacGroupSyncId` (integer): ID of the group to delete.

**Responses:**
- `200`: Group deleted successfully.

### 2. RBAC Role Synchronization

#### **GET** `/api/v1/rbac/roles`

**Responses:**
- `200`: Role data retrieved.

#### **POST** `/api/v1/rbac/roles`

**Request Body:**
- JSON structure with `rbacRoleSyncId`, `rbacRoleName`, and metadata fields.

**Responses:**
- `200`: Role created successfully.

#### **PUT** `/api/v1/rbac/roles`

**Request Body:**
- Same as `POST` request.

**Responses:**
- `200`: Role updated successfully.

#### **DELETE** `/api/v1/rbac/roles/{rbacRoleSyncId}`

**Parameters:**
- `rbacRoleSyncId` (integer): ID of the role to delete.

**Responses:**
- `200`: Role deleted successfully.

---

## SSO API

### 1. SSO Client Management

#### **GET** `/api/v1/sso/clients/{clientSsoId}`

**Parameters:**
- `clientSsoId` (integer): ID of the SSO client.

**Responses:**
- `200`: SSO client data retrieved.

#### **DELETE** `/api/v1/sso/clients/{clientSsoId}`

**Parameters:**
- `clientSsoId` (integer): ID of the SSO client.

**Responses:**
- `200`: SSO client deleted.

#### **PUT** `/api/v1/sso/clients/{clientSsoId}`

**Request Body:**
- JSON structure with SSO client details and metadata fields.

**Responses:**
- `200`: SSO client updated.

#### **GET** `/api/v1/sso/clients`

**Responses:**
- `200`: List of SSO clients retrieved.

#### **POST** `/api/v1/sso/clients`

**Request Body:**
- JSON structure with `clientSsoId`, `name`, `loginType`, and metadata fields.

**Responses:**
- `200`: SSO client created successfully.

---

## Best Practices

- **AD Synchronization:** Use Active Directory for role and group management to reduce manual errors.
- **Tag-Based Permissions:** Use tags sparingly and thoughtfully to avoid complexity.
- **Troubleshooting:** Ensure SSO settings and AD configurations are correct for seamless operations.

---
