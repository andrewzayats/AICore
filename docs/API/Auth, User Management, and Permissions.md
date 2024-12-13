# Authentication, User Management, and Permissions

This document provides details about the Authentication, User Management, and Permissions APIs for AI Core. These APIs enable secure authentication, user profile management, and fine-grained access control for agents and data sources.

---

## Overview

This document details the authentication, user management, and permissions configuration options in AI Core. The platform provides multiple authentication methods, including Microsoft SSO integration, and offers fine-grained control over user access through tags, groups, and roles, all of which are configurable via an Active Directory (AD) integration. The settings described here allow administrators to manage access, establish user limits, and ensure secure interactions with AI Core services.

---

## Introduction
Authentication and user management in AI Core ensure that only authorized users can access the platform and use agents or data sources as intended. AI Core leverages both internal user accounts and SSO integration with Microsoft for user login, with additional configurations available to enforce access controls through Active Directory (AD) synchronization. By using tags, roles, and group-based permissions, administrators can tightly manage access to sensitive data and restrict interactions with the system’s capabilities.

## Authentication API

### Internal User Accounts
AI Core provides an internal login mechanism for users who do not utilize SSO. When enabled, users can enter their AI Core username and password directly on the login page to authenticate.

### Microsoft SSO
For organizations using Microsoft environments, AI Core supports SSO through Microsoft, allowing users to authenticate via their Microsoft AD credentials. When SSO is enabled, users see a "Login with Microsoft" option on the AI Core login page.

### Basic Authentication for API Endpoints
AI Core allows Basic Authentication for API endpoints if PKCE Authentication Code Flow is challenging to implement on the client side.

### 1. Authentication Endpoint

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

The Users API provides an interface for adding, editing, and managing user profiles. Admin users can define user roles, daily token limits, and assign tags for role-based access control.

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

### Tag-Based Access Control
Tags provide a flexible mechanism to manage access to agents and data sources. Tags restrict access to users or groups who have matching tags.

### Roles and Groups Synchronization with AD
Roles link AI Core tags with AD roles, enabling seamless synchronization. When an SSO user logs into AI Core, the system checks their AD role and assigns tags accordingly.

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

### Managing SSO Clients
SSO (Single Sign-On) Clients allow users to authenticate via external identity providers, such as Microsoft Azure Active Directory, providing a seamless and secure login experience for users.

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
