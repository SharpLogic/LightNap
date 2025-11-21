---
title: Seeding Users
layout: home
parent: Application Configuration
nav_order: 500
---

# {{ page.title }}

User accounts can be [seeded in `appsettings.json`](./index) or your deployment host to help bootstrap the application.

- `SeededUsers`: Dictionary of roles with user lists to seed.
  - `Administrator`: List of administrator accounts to seed. This is included by default and may be removed if you prefer.
    - `Email`: The email address of the user.
    - `UserName`: The username of the user.
    - `Password`: The password of the user. If this field is blank or missing then a random password will be generated and the user will need to reset their password via the site to log in.
  - `(empty)`: List of users to be created but not automatically added to any roles. If present, this list is evaluated before any other role list.
  - `[other roles]`: Other roles with their seeded users, such as `ContentEditor` or `Moderator`, as implemented and required by your application.

For example, the default configuration looks like this:

``` json
"SeededUsers": {
  "Administrator": [
    {
      "Email": "Admin@lightnap.azurewebsites.net",
      "UserName": "Admin",
      "Password": "P@ssw0rd"
    }
  ],
  "ContentEditor": [
    {
      "Email": "ContentEditor@lightnap.azurewebsites.net",
      "UserName": "ContentEditor",
      "Password": "P@ssw0rd"
    }
  ],
  "": [
    {
      "Email": "RegularUser@lightnap.azurewebsites.net",
      "UserName": "RegularUser",
      "Password": "P@ssw0rd"
    }
  ]
}
```

This example demonstrates seeding three different types of users:

- An `Administrator` user with full administrative privileges
- A `ContentEditor` user with content editing capabilities
- A `RegularUser` with no special role assignments (indicated by the empty string `""` key)

The seeder will loop through the keys of the `SeededUsers` dictionary in alphabetic order. For each it will confirm the role exists (if not empty) and then loop through the list of provided users.

{: .note }
The roles used as keys in the seeded user list must exactly match the name of the role in the app. For example `Administrator` and not `Administrators`.

For each user it will check for an existing user with the specified email address and then create it if it does not yet exist. If the user already exists it will not confirm the provided user name matches or change the password.

Next, it will add the specified user to the specified role. It will not remove existing users who are not in the role as configured. As a result, users can be removed from roles in the configuration after creation, if desired.

## Seeding users without roles

To seed a user without automatically assigning them to any role, use an empty string (`""`) as the role key. This is useful for creating regular user accounts that don't require special permissions.

## Seeding in multiple roles

To seed a user in multiple roles, include it in all required roles. It will be created (if necessary) in the earliest alphabetic role it's listed under. After that, you only need to include its email since the user name and password will be ignored due to the user email already existing.

For example, here is a way to configure a new user that will be both an administrator and content editor.

``` json
"SeededUsers": {
  "Administrator": [
    {
      "Email": "Admin@lightnap.azurewebsites.net",
      "UserName": "Admin",
      "Password": "P@ssw0rd"
    }
  ],
  "ContentEditor": [
    {
      "Email": "Admin@lightnap.azurewebsites.net"
    }
  ]
}
