# Privacy Notice — MyId Demo UI

_Last updated: November 2025_

---

## Overview

This document describes the privacy practices for the MyId Demo UI ("Demo"). The Demo is a technical demonstration and testing environment for the MyId identity system. It is intended for developers and contributors to validate integration scenarios (including OAuth flows such as Facebook Login). This Demo is not a production, consumer-facing service.

## Key Points (TL;DR)

- The Demo is for development and testing only.
- **No personal data is written to persistent storage or retained beyond the session.**
- Facebook access tokens are validated server-side before any user information is trusted.
- The Demo displays minimal profile information only to confirm login succeeded.

---

## 1. Purpose

The Demo exists to demonstrate how the MyId backend integrates with external identity providers and to facilitate development and testing of authentication flows (for example, Facebook Login). It is not intended for production use or to collect real user data long-term.

## 2. Data Collection and Use

### 2.1 What We Collect

When you sign in with an external provider (e.g., Facebook) the Demo may obtain the following from the provider:

- Identifier: the external provider's user id for the authenticated user
- Profile data: minimal profile attributes as permitted by requested scopes (for example, name and email)

The Demo only requests the scopes required to demonstrate authentication (for Facebook typically `public_profile` and `email`).

### 2.2 How We Use the Data

Data retrieved from the identity provider is used only for the following purposes:

- Authentication: confirm the user identity for the current session
- Display: show basic profile information in the Demo UI to illustrate a successful login

The Demo does not use data for analytics, marketing, or other secondary purposes.

### 2.3 Server-side Verification

All third-party access tokens are verified server-side using the provider's verification endpoints (for Facebook, the `debug_token` endpoint). The Demo only relies on data returned from the provider's API after verification; client-submitted identity claims are not trusted.

## 3. Retention and Storage

- No personal data is stored in a persistent database by the Demo.
- Personal data is held in-memory only for the duration of the session and is discarded when the session or browser tab ends.
- The Demo does not write PII to logs, persistent storage, or external services.

## 4. Data Deletion Instructions

Facebook requires applications to provide a method for users to request the deletion of their data.

**Since the MyId Demo UI is non-persistent and does not store or retain any personal data (as detailed in Section 3), no formal data deletion process is required or provided.**

### How to Delete Your Data

To "delete" your data from this demo application, you only need to **close the browser tab or window** running the Demo. Any ephemeral data used for the demonstration is instantly purged upon session end and is never recorded to persistent storage.

If you have concerns about the data shared with the original identity provider (e.g., Facebook) itself, you must manage that data directly through the settings and tools provided by the respective third-party provider.

## 5. Security

- Access tokens are validated and checked to ensure they belong to the configured application.
- Secrets such as app secrets should never be embedded in source control. The Demo allows the operator to provide secrets using secure configuration mechanisms (user secrets, environment variables, or managed secret stores) — how secrets are supplied is left to the Demo operator.
- The Demo uses HTTPS for all communication with external providers.

## 6. Limitations and Warnings

- This is a demo environment. Do not use real production user accounts or sensitive data for extended periods.
- The Demo may change between releases. It is provided "as-is" for development and testing purposes only.

## 7. Contact and Repository

Project repository: https://github.com/shanieMoonlight/spider-baby-identity

For questions or issues, please open an issue on the repository.

---

**Acknowledgement**: By using the Demo you acknowledge that it is a developer-facing tool and that any profile data shown is for demonstration only.
```eof

You can now use the URL for this updated `PRIVACY-NOTICE.md` document for both the **Privacy Policy URL** and the **Data Deletion URL** in your Facebook App configuration.

Is there anything else I can help you with regarding your app setup or image editing?