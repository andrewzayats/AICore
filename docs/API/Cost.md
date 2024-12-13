# Cost API

This document provides an overview of the AI Core Cost Management API, which enables administrators to monitor token consumption, expenses, and calculate predictive costs programmatically. This API is designed to support cost tracking, usage analysis, and future cost forecasting for AI Core services.

## Table of Contents

1. [Overview](#overview)
2. [API Methods](#api-methods)
   - [Get Total Spent](#1-get-total-spent)
   - [Get Token Cost](#2-get-token-cost)
   - [Get AKS Price](#3-get-aks-price)
   - [Get Available Locations](#4-get-available-locations)
3. [Best Practices for Cost Management](#best-practices-for-cost-management)
4. [Limitations and Considerations](#limitations-and-considerations)

---

## Overview

The AI Core Cost Management API provides programmatic access to key cost-related data points, enabling administrators to:

1. **Monitor Spending**: Retrieve information about historical token usage and associated costs.
2. **Analyze Usage**: Gain insights into resource consumption and optimize usage trends.
3. **Calculate Costs**: Estimate future costs based on customizable inputs, such as location and service tier.

This API is intended for use by developers and system administrators managing AI Core services.

---

## API Methods

### 1. Get Total Spent

**Endpoint:** `/api/v1/spent`

**Method:** GET

**Description:** Retrieves the total spending for AI Core services.

**Parameters:** None

**Response:**
- **Code 200:** Returns total spending data.

**Example response:**

```
[
    {
        "costDayByDay": [
            0.0,
            0.0,
            0.0003168,
            0.0003168,
            0.0003168,
            0.0003168,
            0.44654365,
            0.47947975,
            0.48242845,
            0.53037305,
            1.07168300,
            1.07714220,
            1.09335470,
            1.11970220,
            1.11970220
        ],
        "loginId": 0,
        "login": "Total",
        "loginType": "",
        "tokensOutgoing": 1732051,
        "tokensIncoming": 136690,
        "cost": 15.06060385
    },
    {
        "costDayByDay": [
            0.0,
            0.0,
            0.0003168,
            0.0003168,
            0.0003168,
            0.09838815,
            0.44654365,
            0.48185035,
            0.48242845,
            0.53037305,
            1.07714220,
            1.09335470,
            1.09335470,
            1.11970220,
            1.11970220
        ],
        "loginId": 1,
        "login": "admin@viacode.com",
        "loginType": "Password",
        "tokensOutgoing": 1732051,
        "tokensIncoming": 136690,
        "cost": 15.06060385
    }
]
```

---

### 2. Get Token Cost

**Endpoint:** `/api/v1/spent/tokenCost`

**Method:** GET

**Description:** Retrieves the cost of tokens consumed by AI Core services.

**Parameters:** None

**Response:**
- **Code 200:** Returns token cost data.

---

### 3. Get AKS Price

**Endpoint:** `/api/v1/spent/price/aks`

**Method:** GET

**Description:** Retrieves the Azure Kubernetes Service (AKS) pricing for a specified location.

**Parameters:**
- `location` (string): The geographic region for which the AKS price is requested.

**Response:**
- **Code 200:** Returns AKS pricing data for the specified location.

---

### 4. Get Available Locations

**Endpoint:** `/api/v1/spent/locations`

**Method:** GET

**Description:** Retrieves a list of available locations for pricing and service configuration.

**Parameters:** None

**Response:**
- **Code 200:** Returns a list of supported locations.

---

## Best Practices for Cost Management

1. **Regularly Monitor Spending**: Use the `/api/v1/spent` endpoint to track overall expenses and identify trends.
2. **Optimize Resource Allocation**: Analyze token usage via `/api/v1/spent/tokenCost` to adjust usage policies and reduce unnecessary consumption.
3. **Leverage Predictive Analysis**: Use `/api/v1/spent/price/aks` to forecast AKS costs based on location and planned resource requirements.
4. **Utilize Supported Locations**: Ensure resource configurations align with the available locations retrieved from `/api/v1/spent/locations`.

---

## Limitations and Considerations

1. **Cost Data Availability**: The Cost page is limited to token and service usage; it does not cover other system costs or indirect expenses.
2. **Static Viacode Support Fee**: Viacode Support costs are fixed and cannot be adjusted for different support levels or requirements.
3. **No Dynamic Scaling Prediction**: The calculator does not dynamically adjust for scaling needs beyond the manual tier selection provided.

By leveraging the AI Core Cost Management API, administrators can effectively manage expenses, forecast costs, and align usage with budgetary goals.
