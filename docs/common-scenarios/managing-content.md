---
title: Managing Content
layout: default
parent: Common Scenarios
nav_order: 410
---

# Managing Content

This guide walks you through using LightNap's Content Management System (CMS) web interface to create, edit, and publish content. You'll learn how to work with both zones and pages, manage multilingual content, and configure access controls through the admin interface.

## Prerequisites

- Understanding of [Content Management System concepts](../concepts/content-management)
- User account in the `Administrator` or `ContentEditor` role
- Access to the content management interface
- Basic knowledge of Markdown or HTML formatting

## Accessing the Content Management Interface

1. **Log in** to your LightNap application with an account that has content editing permissions
2. **Navigate to the Admin section** Accessible from the **Content | Manage** sidebar menu
3. You'll see the content management dashboard with options to create, edit, and manage content

## Creating Your First Content

### Step 1: Create an Instructions Landing Page

Let's start by creating a simple instructions landing page that can be sent out in email to new users.

1. **Click the Create button** in the content management interface
2. **Set the key** to `landing-instructions` in the dialog and press **Create**
3. **Click the newly created key** in the list of Contents available

### Step 2: Add English Language Content

After creating the content item, you'll be redirected to the content editor:

1. **Select the Languages Tab**: Choose "English (en)" to start with the default language
2. **Click "Create Language Content"** to create a default language stub
3. **Choose Content Format**: Select "Markdown" for easy formatting
4. **Enter Your Content**:

   ```markdown
   # Welcome to Our Application

   We're glad you're here!

   [Get Started](/home)
   ```

5. **Review the preview** at the bottom of the page
6. **Save the Content**: Click "Save" to store the English version

### Step 3: Publish the Content

Once you're satisfied with your content:

1. **Navigate back** to the `landing-instructions` editor page
2. **Change the "Status"** from **Draft** to **Published**
3. **Change the "Read Access"** from **Explicit** to **Public**
4. **Save the changes**

The content is now live and accessible to everyone with access to your site.

### Step 4: Test the page

Visit `/content/landing-instructions`.

1. **Copy the URL** displayed below the **Type**
2. **Open a private browser window** and navigate to the URL created for your page
3. **Review the content** that matches your markdown and try the link, which should ask you to log in since it's to a secured part of the app

## Configuring Access Control

### Creating Private Content for Authenticated Users

To create content that only logged-in users can see:

1. **Return to the `landing-instructions` editor** if not already there
2. **In the Settings tab** update **Read Access** to **Authenticated**
3. **Save** the content
4. **Visit the page from a private window** to confirm that it now asks you to log in before you can see the page

This content will only be visible to users who are logged into your application.

### Restricting Read Access for Specific Users

For content restricted to specific users:

1. **Return to the `landing-instructions` editor** if not already there
2. **In the Settings tab** update **Read Access** to **Explicit**
3. **Save** the content
4. **In the Readers tab** use the **Add User** form to locate and add the specific users you want to grant read access to

Only those users (and those with edit permission for this item) will be able to see this item.

### Allowing Editor Access for Specific Users

Just like the **Readers** tab, the **Editors** tab allows specific users to have editor access to this item. This is useful for scenarios where they need the ability to manage the specific item as well as its language content.

{: .note }
Allowing edit access to specific users should be used sparingly. If there are many content items they should have edit access for, consider adding them to the `ContentEditor` role.

## Content Management Workflow

### Content Lifecycle

Understanding the content lifecycle helps you manage content effectively:

1. **Draft**: Content is being worked on, not visible to end users
2. **Published**: Content is live and accessible based on permissions
3. **Archived**: Content is no longer active but preserved

### Organizing Your Content

**Use Clear, Descriptive Keys:**

- ✅ Good: `privacy-policy`, `marketing-hero-banner`, `help-getting-started`
- ❌ Avoid: `page1`, `content_2`, `banner`

**Group Related Content and Consider Locations for Zones:**

```text
public-index-banner
public-index-welcome
public-index-footer
```

**Plan Content Types:**

- **Zones** for reusable content blocks (welcome messages, announcements)
- **Pages** for standalone content (terms of service, help pages)

### Working with Multiple Languages

1. **Start with your primary language** (usually English)
2. **Create and publish content** in the primary language first
3. **Add translations** by clicking "Add Language" for each content item
4. **Consider cultural differences**, not just direct translations
5. **Test language switching** to ensure proper fallbacks

### Content Formats

Choose the right format for your content:

- **Markdown**: Best for most content, easy to write and maintain
- **HTML**: When you need complex formatting or custom styling
- **Plain Text**: For simple messages and notifications

## Searching and Managing Content

### Using the Content Search

1. **Access the search function** in the content management interface
2. **Enter search terms** to find content by key, title, or content text
3. **Filter results** by:
   - Content type (Pages, Zones)
   - Status (Draft, Published, Archived)
   - Language
   - Access level
4. **Sort results** by creation date, modification date, or alphabetically

## Best Practices

### Content Creation

1. **Write clear, concise content** that serves your users' needs
2. **Use consistent styling and formatting** across similar content types
3. **Include relevant links** to other pages or resources
4. **Keep content up-to-date** with regular reviews
5. **Test content** in different contexts before publishing

### Organization and Maintenance

1. **Regular content audits** to identify outdated or unused content
2. **Consistent naming conventions** for easy searching and management
3. **Document your content strategy** for team members
4. **Plan translation workflows** for multilingual content
5. **Back up important content** before major changes

### Performance Considerations

1. **Keep individual content pieces** reasonably sized for fast loading
2. **Use zones appropriately** - they're cached for better performance
3. **Optimize images and media** included in HTML content
4. **Monitor page load times** for content-heavy pages

## Troubleshooting

### Common Issues

#### Content not appearing on the website

**Check these items:**

- Is the content status set to "Published"?
- Does the user have the required permissions to view the content?
- Is the content key correctly referenced in your templates?
- Are you using the correct language code?

#### Can't edit content

**Possible solutions:**

- Verify you have `ContentEditor` or `Administrator` role
- Check if you have specific edit permissions for that content
- Ensure you're logged in with the correct account
- Clear your browser cache and try again

#### Translation not showing

**Troubleshooting steps:**

- Confirm the language file was created and saved
- Check the language code matches what your application expects
- Verify the content is published, not just saved as draft
- Test with a different language to isolate the issue

### Getting Help

When you encounter issues:

1. **Check the application logs** for any error messages
2. **Verify your user permissions** with an administrator
3. **Test with a simple content item** to isolate complex issues
4. **Document the steps** that led to the problem for easier troubleshooting

The CMS interface is designed to be intuitive and user-friendly, making content management accessible to non-technical team members while providing the power and flexibility needed for complex content scenarios.
 
 
 
 