# LightNap

**Light**weight **.N**ET/**A**ngular/**P**rimeNG full-stack starter kit for building modern Single Page Applications (SPA) with enterprise-grade authentication and identity management out of the box.

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-20-DD0031)](https://angular.io/)
[![PrimeNG](https://img.shields.io/badge/PrimeNG-20-007BFF)](https://primeng.org/)

---

## 🚀 Quick Start

LightNap provides a complete foundation for building secure, scalable web applications with minimal setup. Perfect for developers who want to skip the boilerplate and start building features immediately.

**[📖 Documentation](https://lightnap.sharplogic.com)** | **[🎥 YouTube Channel](https://www.youtube.com/@LightNap)** | **[🌐 Live Demo](https://lightnap.azurewebsites.net)**

---

## ✨ Features

### Backend (.NET 10)

- **ASP.NET Core Web API** - RESTful API with best practices
- **ASP.NET Identity** - Complete authentication & authorization framework
- **JWT Token Management** - Secure token-based authentication with refresh tokens
- **Rate Limiting** - Built-in API rate limiting with configurable policies per endpoint
- **Multiple Database Providers** - SQL Server, SQLite, and In-Memory options
- **Redis Caching** - Distributed caching with hybrid cache support
- **SignalR** - Real-time communication for notifications and live updates
- **Email Integration** - Templated email system for authentication flows
- **User Management** - Full CRUD operations for users, roles, and permissions
- **Device Tracking** - Monitor and manage user sessions across devices
- **Maintenance Service** - Background task processing with Azure WebJobs support

### Frontend (Angular 20)

- **Angular** - Latest Angular framework with standalone components
- **PrimeNG** - Comprehensive UI component library (70+ components)
- **Tailwind CSS** - Utility-first CSS framework for rapid styling
- **Progressive Web App (PWA)** - Installable with service worker for asset caching (good starting point for offline support)
- **Content Management System (CMS)** - Basic support for zones and pages with permissions so users can update static content without rebuilds
- **In-App Notifications** - Real-time notification system
- **Form Validation** - Built-in validation with custom error handling
- **Route Guards** - Role-based access control at the routing level

### Developer Experience

- **Code Scaffolding** - Generate complete CRUD infrastructure from entity classes
  - Backend API controllers and services
  - Frontend components and routing
  - Form validation and data models
  - All in seconds!
- **C#/TypeScript** - Type safety across the full stack
- **Comprehensive Testing** - Unit test setup for backend and frontend services
- **Docker Support** - Containerization ready with Dockerfiles
- **CI/CD Ready** - GitHub Actions workflows included

---

## 🏗️ Project Structure

```text
src/
├── lightnap-ng/                  # Angular frontend (PrimeNG + Tailwind)
├── LightNap.WebApi/              # ASP.NET Core Web API
├── LightNap.Core/                # Business logic & data access
├── LightNap.DataProviders.Sqlite/  # SQLite data provider
├── LightNap.DataProviders.SqlServer/  # SQL Server data provider
├── LightNap.MaintenanceService/  # Background tasks web job
├── LightNap.Core.Tests/          # Backend init tests
└── Scaffolding/                  # Code generation templates
```

---

## 📋 Prerequisites

- **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)** or later
- **[Node.js](https://nodejs.org/)** (LTS version recommended)

---

## 🔧 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/SharpLogic/LightNap.git
cd LightNap
```

### 2. Start the Backend

```bash
cd src/LightNap.WebApi
dotnet run
```

The API will be available at `https://localhost:7266` (or check console output).

> **Note:** The backend runs with a SQLite database by default and logs emails to the console for development. See [Application Configuration](https://lightnap.sharplogic.com/getting-started/application-configuration.html) to configure SQL Server, email providers, rate limiting, caching, or other settings.

### 3. Start the Frontend

In a new terminal:

```bash
cd src/lightnap-ng
npm install
npm start
```

The application will open at `http://localhost:4200`.

### 4. Login with Default Credentials

The application comes with three pre-seeded user accounts for testing:

| Role           | Email                                      | Password   | Description                                                       |
| -------------- | ------------------------------------------ | ---------- | ----------------------------------------------------------------- |
| Administrator  | `Admin@lightnap.azurewebsites.net`         | `P@ssw0rd` | Full access to manage users, roles, and system settings.          |
| Content Editor | `ContentEditor@lightnap.azurewebsites.net` | `P@ssw0rd` | Can manage CMS aspects of the site.                               |
| Regular User   | `RegularUser@lightnap.azurewebsites.net`   | `P@ssw0rd` | Basic user with limited access for standard application features. |

---

## 📖 Next Steps

For detailed guides on configuration, customization, and advanced features, visit the **[official documentation](https://lightnap.sharplogic.com)**:

- **[Application Configuration](https://lightnap.sharplogic.com/getting-started/application-configuration.html)** - Database providers, email setup, JWT configuration, rate limiting, and caching
- **[Adding Entities](https://lightnap.sharplogic.com/common-scenarios/adding-entities.html)** - Using the scaffolding system
- **[Authentication Concepts](https://lightnap.sharplogic.com/concepts/authentication.html)** - Understanding JWT, roles, and permissions
- **[Content Management](https://lightnap.sharplogic.com/concepts/content-management.html)** - Working with dynamic content
- **[Common Scenarios](https://lightnap.sharplogic.com/common-scenarios/)** - Step-by-step guides for extending functionality

---

## 📚 Documentation

Comprehensive documentation is available at **[lightnap.sharplogic.com](https://lightnap.sharplogic.com)**:

- **Getting Started** - Setup and configuration guides
- **Application Configuration** - Complete configuration reference including database providers, authentication, rate limiting, caching, and email setup
- **Common Scenarios** - Step-by-step tutorials for adding features
- **Concepts** - Architecture and design decisions
- **API Reference** - Complete endpoint documentation
- **GitHub Actions** - CI/CD workflow guides

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📺 Video Tutorials

Check out the **[LightNap YouTube Channel](https://www.youtube.com/@LightNap)** for video tutorials, feature demonstrations, and development tips.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Built with [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
- Frontend powered by [Angular](https://angular.io/)
- UI components by [PrimeNG](https://primeng.org/)
- Styled with [Tailwind CSS](https://tailwindcss.com/)

---

## 📞 Support

- 📖 **Documentation**: [lightnap.sharplogic.com](https://lightnap.sharplogic.com)
- 🐛 **Issues**: [GitHub Issues](https://github.com/SharpLogic/LightNap/issues)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/SharpLogic/LightNap/discussions)
- 🎥 **YouTube**: [@LightNap](https://www.youtube.com/@LightNap)

---

Made with ❤️ by [SharpLogic](https://github.com/SharpLogic)
