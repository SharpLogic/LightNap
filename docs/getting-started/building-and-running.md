---
title: Building and Running the Application
layout: home
parent: Getting Started
nav_order: 100
---

# {{ page.title }}

## Prerequisites

- .NET 9 SDK
- Node.js and npm
- Angular CLI

## Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/sharplogic/LightNap.git
   cd LightNap
   ```

2. **Backend Setup:**

   - Navigate to the `src` directory:

     ```bash
     cd src
     ```

   - Run the application:

     ```bash
     dotnet run --project LightNap.WebApi
     ```

    {: .important }
    The application will log the error and quit if anything in the startup or seeding process fails. This
    includes database migrations and user/role seeding. Please check the logs if a deployment fails to start.

3. **Frontend Setup:**

   - In a separate terminal, navigate to the `lightnap-ng` directory:

     ```bash
     cd src/lightnap-ng
     ```

   - Install Angular dependencies:

     ```bash
     npm install
     ```

   - Run the Angular application:

     ```bash
     ng serve
     ```

## Usage

- Access the application at `http://localhost:4200`.
- By default, an administrator account is created with:
  - **Email**: `admin@admin.com`
  - **UserName**: `admin`
  - **Password**: `A2m!nPassword`

  {: .note }
  If you are debugging the backend in Visual Studio you may see an **Exception User-Unhandled** dialog in the `IdentityService.LogInAsync` method for a `UserFriendlyApiException`. It is recommended that you disable this behavior by unchecking the `Break when this exception type is user-unhandled` since those exceptions are thrown regularly.
