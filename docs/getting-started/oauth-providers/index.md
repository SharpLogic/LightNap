---
title: OAuth Providers
layout: home
parent: Getting Started
nav_order: 250
---

# {{ page.title }}

OAuth enables users to log in using their existing accounts from third-party providers like Google and Microsoft. This reduces friction in the authentication flow and leverages the security infrastructure of established identity providers.

## Supported Providers

LightNap supports the following OAuth providers out of the box:

- **Google** - Sign in with Google accounts
- **Microsoft** - Sign in with Microsoft/Azure AD accounts
- **GitHub** - Sign in with GitHub accounts

You can easily add more providers following the same convention. There are at least 80 major providers available via supported NuGet packages.

## Configuration

OAuth providers are configured in `appsettings.json` (or environment-specific variants) under the `Authentication.OAuth` section:

```json
{
  "Authentication": {
    "OAuth": {
      "Google": {
        "ClientId": "YOUR_GOOGLE_CLIENT_ID",
        "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
      },
      "Microsoft": {
        "ClientId": "YOUR_MICROSOFT_CLIENT_ID",
        "ClientSecret": "YOUR_MICROSOFT_CLIENT_SECRET"
      },
      "GitHub": {
        "ClientId": "YOUR_GITHUB_CLIENT_ID",
        "ClientSecret": "YOUR_GITHUB_CLIENT_SECRET"
      }
    }
  }
}
```

{: .note }
Only providers with both `ClientId` and `ClientSecret` configured will be available to users. Omit any providers you don't intend to use.

## Redirect URLs

When registering your application with OAuth providers, you'll need to configure redirect URLs. LightNap uses the following pattern:

```url
https://yourdomain.com/signin-{provider}
```

For local development with a backend running on `https://localhost:7266`:

```url
https://localhost:7266/signin-google
https://localhost:7266/signin-microsoft
https://localhost:7266/signin-github
```

### Special considerations for the development environment

Testing OAuth in the development environment requires an additional manual step to complete each login attempt. Assuming everything succeeds up after the OAuth login, the provider will return the user to the `/signin-{provider}` middleware endpoint. This will then confirm the login and forward the user to a URL that fails to load if the frontend has not been deployed to the development server (which is usually will not be).

Simply update the backend origin to the frontend origin, which is usually replacing `https://localhost:7266` with `http://localhost:4200`.

Such as changing:

```url
https://localhost:7266/identity/external-logins/callback?token=...
```

to:

```url
http://localhost:4200/identity/external-logins/callback?token=...
```

Navigating to that URL will complete the login process as expected. Note that this step is not necessary when the frontend and backend are hosted at the same origin.

## Setting Up Google OAuth

### 1. Create a Google Cloud Project

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select an existing one
3. Enable the Google+ API for your project

### 2. Create OAuth 2.0 Credentials

1. Navigate to **Credentials** in the left sidebar
2. Click **Create Credentials** and select **OAuth 2.0 Client ID**
3. If prompted to create a consent screen first:
   - Select **External** as the user type
   - Fill in the required information (app name, user support email, etc.)
   - Add yourself as a test user if still in development
4. Select **Web application** as the application type
5. Add authorized redirect URIs:
   - `https://yourdomain.com/signin-google`
   - For local development: `https://localhost:7266/signin-google`

### 3. Copy Credentials

1. Download or copy your **Client ID** and **Client Secret**
2. Add them to your `appsettings.json`:

```json
{
  "Authentication": {
    "OAuth": {
      "Google": {
        "ClientId": "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com",
        "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
      }
    }
  }
}
```

## Setting Up Microsoft OAuth

### 1. Create an Azure AD Application

1. Go to [Azure Portal](https://portal.azure.com/)
2. Navigate to **Azure Active Directory** > **App registrations**
3. Click **New registration**
4. Enter an application name
5. For **Supported account types**, select your desired scope (typically "Accounts in this organizational directory only" for internal apps or "Multitenant" for public access)
6. Click **Register**

### 2. Configure Redirect URIs

1. In your app registration, go to **Authentication**
2. Under **Platform configurations**, click **Add a platform**
3. Select **Web**
4. Add redirect URIs:
   - `https://yourdomain.com/signin-microsoft`
   - For local development: `https://localhost:7266/signin-microsoft`
5. Click **Configure** to save

### 3. Create Client Secret

1. Navigate to **Certificates & secrets**
2. Under **Client secrets**, click **New client secret**
3. Enter a description and select an expiration period
4. Click **Add**
5. Copy the secret value immediately (you won't be able to see it again)

### 4. Note Your Credentials

1. Go to **Overview** to find your **Application (client) ID**
2. Add both to your `appsettings.json`:

```json
{
  "Authentication": {
    "OAuth": {
      "Microsoft": {
        "ClientId": "YOUR_MICROSOFT_CLIENT_ID",
        "ClientSecret": "YOUR_MICROSOFT_CLIENT_SECRET"
      }
    }
  }
}
```

## Setting Up GitHub OAuth

### 1. Create a GitHub OAuth Application

1. Go to GitHub **Settings** > **Developer settings** > **OAuth Apps**
2. Click **New OAuth App**
3. Fill in the form:
   - **Application name**: Your app name
   - **Homepage URL**: `https://yourdomain.com`
   - **Authorization callback URL**: `https://yourdomain.com/signin-github`
   - For local development: `https://localhost:7266/signin-github`
4. Click **Register application**

### 2. Copy Credentials

1. Copy your **Client ID** from the application page
2. Click **Generate a new client secret** and copy the value
3. Add them to your `appsettings.json`:

```json
{
  "Authentication": {
    "OAuth": {
      "GitHub": {
        "ClientId": "YOUR_GITHUB_CLIENT_ID",
        "ClientSecret": "YOUR_GITHUB_CLIENT_SECRET"
      }
    }
  }
}
```

## Usage

### Frontend

Once OAuth providers are configured, users will see external login options on the login page. The UI displays icons for each provider and directs users to the OAuth provider's authentication flow.

Users can also link OAuth accounts to their existing LightNap account from their profile page. This allows them to sign in using multiple methods.

### Backend API Endpoints

The following API endpoints manage OAuth:

#### Get Supported External Logins

```
GET /api/externallogin/supported
```

Returns a list of configured external login providers.

**Response:**

```json
{
  "type": "Success",
  "result": [
    {
      "providerName": "Google",
      "displayName": "Google"
    },
    {
      "providerName": "Microsoft",
      "displayName": "Microsoft"
    }
  ]
}
```

#### Initiate External Login

```
GET /api/externallogin/login/{provider}?returnUrl={url}
```

Initiates the OAuth flow with the specified provider. The user's browser is redirected to the provider's login page.

**Parameters:**
- `provider`: The name of the provider (e.g., `Google`, `Microsoft`)
- `returnUrl`: (Optional) The URL to redirect to after successful authentication

#### Handle Callback

```
GET /api/externallogin/callback?returnUrl={url}&remoteError={error}
```

Handles the OAuth provider's callback. This is called automatically by the authentication flow and typically doesn't need to be called directly.

#### Get External Login Result

```
GET /api/externallogin/result/{confirmationToken}
```

Retrieves the result of an external login attempt after the OAuth callback.

**Response:**
```json
{
  "type": "Success",
  "result": {
    "successType": "LinkedAccount",
    "accessToken": "eyJhbGc...",
    "refreshToken": "...",
    "expiresIn": 7200
  }
}
```

#### Complete External Login

```
POST /api/externallogin/complete/{confirmationToken}
```

Completes the external login for an already-linked account.

**Request Body:**
```json
{
  "rememberMe": true,
  "deviceDetails": {
    "deviceName": "Chrome on Windows",
    "ipAddress": "192.168.1.1"
  }
}
```

#### Complete External Login Registration

```
POST /api/externallogin/register/{confirmationToken}
```

Completes the external login registration for a new user.

**Request Body:**
```json
{
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "rememberMe": true,
  "deviceDetails": {
    "deviceName": "Chrome on Windows",
    "ipAddress": "192.168.1.1"
  }
}
```

#### Get User's External Logins

```
GET /api/users/me/external-logins
```

Retrieves all OAuth providers linked to the authenticated user.

**Response:**
```json
{
  "type": "Success",
  "result": [
    {
      "loginProvider": "Google",
      "providerDisplayName": "Google",
      "providerKey": "118234567890"
    }
  ]
}
```

#### Remove External Login

```
DELETE /api/users/me/external-logins/{loginProvider}/{providerKey}
```

Removes an OAuth provider link from the authenticated user's account.

## Security Considerations

1. **Client Secret Protection**: Never commit your OAuth client secrets to version control. Use environment variables or secure configuration management.

2. **Redirect URL Validation**: Ensure your redirect URL in `appsettings.json` exactly matches what you've registered with the OAuth provider (including protocol and port).

3. **State Parameter**: LightNap automatically handles the OAuth state parameter for CSRF protection.

4. **Token Storage**: OAuth tokens are never stored or persisted. After successful authentication, only the user's identity is retained and a LightNap JWT token is issued.

5. **Email Verification**: If `RequireEmailVerification` is enabled in your authentication settings, external logins will verify the email provided by the OAuth provider.

6. **Account Linking**: Users can link multiple OAuth providers to a single account for flexible login options.

## Environment-Specific Configuration

For different environments (development, staging, production), use environment-specific `appsettings` files:

- `appsettings.json` - Default configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production overrides

```json
{
  "Authentication": {
    "OAuth": {
      "Google": {
        "ClientId": "prod-client-id",
        "ClientSecret": "prod-client-secret"
      }
    }
  }
}
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "Invalid redirect URI" error from provider | Ensure the redirect URL in your provider settings exactly matches the pattern: `/api/externallogin/callback` |
| OAuth provider not appearing on login page | Verify that both `ClientId` and `ClientSecret` are configured in `appsettings.json` and that the application has been restarted |
| "OAuth session expired" error | The confirmation token has exceeded its 10-minute validity window. User should try logging in again |
| Email conflicts when registering | If the OAuth provider's email matches an existing account, users can link the account instead of registering a new one |
| Provider secrets showing in logs | Check that sensitive configuration is not being logged. Use `appsettings.Development.json` for development only |

## Next Steps

- [Authentication & Tokens Overview](../../concepts/authentication.md) - Learn about JWT tokens and the authentication flow
- [Managing Devices and Sessions](../../concepts/devices.md) - Understand refresh tokens and device management
- [Email Providers](../email-providers/index.md) - Configure email for OAuth workflows
