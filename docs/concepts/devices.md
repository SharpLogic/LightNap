---
title: Devices
layout: home
parent: Concepts
nav_order: 300
---

# {{ page.title }}

When a user logs into the backend they have the option for their device to be remembered across sessions. This is done by setting a refresh token cookie in the response to successful login requests.

Learn more about authentication in [this article](./authentication).

## What Is A Device?

In the context of LightNap, a _device_ is a single logged-in user session tracked across multiple browser sessions. The term isn't completely precise as a given device may have multiple browsers and/or browser profiles that are viewed as separate devices since they all manage their own cookies.

## The Relationship Between Devices And Refresh Tokens

The frontend **device** and backend **refresh token** are the same thing. The naming just varies based on the perspective of the frontend user and the backend developer.

Users interact with devices by viewing a list of the places they've logged in from. If they revoke a device, it tells the backend to flag that refresh token as revoked so that future attempts to retrieve an access token with them will be rejected.

{: .important }
Revoking a device does not log it out until its access token expires, which can take up to 120 minutes based on the default settings.
