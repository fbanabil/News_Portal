# 📰 News Portal

**A Role-Based ASP.NET Core MVC News Publishing Platform**

News Portal is a **multi-role ASP.NET Core MVC application** for publishing and managing news content. It supports **public news browsing**, **author workflows** for creating articles, and **admin tools** for moderation, pinning, and user management.

---

## ✨ Features

### 🔐 Role-Based Access & Security

- Role-based access with **ASP.NET Core Identity**: **Admin**, **Author**, **User**
- Email confirmation enabled for accounts
- Password reset workflow
- Google external authentication support

### ✍️ Author Workflows

- Create, edit, and delete articles
- Upload article images with resizing
- Optional video embed support
- Manage drafts and published articles

### 🛡️ Admin Tools

- Review and moderate content
- Change article **status**, **priority**, and **type**
- Pin/Unpin important articles
- Manage authors and users

### 🌍 Public Site

- Home page sections (carousel, top news, suggestions)
- News details page
- Comments (authentication required)

### 🖼️ Media & Storage

- Local image storage with resizing and default assets
- Cloudinary configuration available (optional)
- Profile images + news images stored under `wwwroot`

### 📧 Email Workflows

- Registration confirmation emails
- Password reset emails
- SMTP email delivery via **MailKit**

---

## 🏗️ Architecture

The solution is organized into three main projects:

### `News_Portal.UI`

- ASP.NET Core MVC UI
- Areas: **Admin**, **Author**, **Identity**
- Razor views, view components
- Middleware and filters

### `News_Portal.Core`

- Domain entities
- DTOs
- Services and helpers
- Enums

### `News_Portal.Infrastructure`

- EF Core DbContext
- Repositories
- Migrations

---

## 🧰 Tech Stack

- **.NET 8** / **ASP.NET Core MVC**
- **Entity Framework Core** + **SQL Server**
- **ASP.NET Core Identity**
- **Google Authentication**
- **MailKit** for SMTP email
- **ImageSharp** for image resizing

---

## 🚀 Getting Started

### ✅ Prerequisites

- .NET SDK **8.x**
- SQL Server instance (LocalDB or full SQL Server)

---

## ▶️ Run Locally

1. Configure app secrets (see [Configuration](#️-configuration)).
2. Restore and run:

```powershell
dotnet restore
dotnet run --project News_Portal.UI/News_Portal.UI.csproj
```

3. Browse:

- `https://localhost:7125`
- `http://localhost:5159`

> Ports are defined in:
> `News_Portal.UI/Properties/launchSettings.json`

You can also open `News_Portal.UI/News_Portal.UI.sln` in Visual Studio and run the `News_Portal.UI` project.

---

## ⚙️ Configuration

The app reads configuration from:

- `News_Portal.UI/appsettings.json`
- Environment variables
- User secrets

### Example `appsettings.json`

> Use your own values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Cloudinary": {
    "CloudName": "...",
    "ApiKey": "...",
    "ApiSecret": "..."
  },
  "Authentication": {
    "Google": {
      "ClientId": "...",
      "ClientSecret": "..."
    }
  },
  "Smtp": {
    "Host": "smtp.example.com",
    "Port": 465,
    "UserName": "...",
    "Password": "...",
    "UseSsl": true,
    "FromEmail": "no-reply@example.com",
    "FromName": "News Portal"
  },
  "AdminCredentials": {
    "AdminEmail": "fahadbinaziznabil@gmail.com"
  }
}
```

---

## 🗄️ Database and Migrations

- MSSQL is used in this project
- Migrations are in:
  - `News_Portal.Infrastructure/Migrations`
- The application automatically applies migrations on startup using:
  - `Database.Migrate()`

### Manual Migration (Optional)

```powershell
dotnet ef database update --project News_Portal.Infrastructure/News_Portal.Infrastructure.csproj --startup-project News_Portal.UI/News_Portal.UI.csproj
```

---

## 🌱 Seed Data

On first load of the home page, **if there are no users**, the app seeds sample data from:

- Sample data available for developement, can use through HomeController/Index method. Can modify it.
- Add the AdminCredential in appsettings.json and login with google using admin email. It will autometically give this user Admin permission.

---

## 📁 File Storage

- Profile images:`News_Portal.UI/wwwroot/images/profiles`
- News images:`News_Portal.UI/wwwroot/images/news`
- Default images:
  `News_Portal.UI/wwwroot/images/defaults`

✅ Ensure the deployment environment has **write access** to `wwwroot`.

---

## 📝 Notes

- Comments require authentication
- The UI includes Bangla display labels and messages in several places
- Cloudinary support is optional (local storage works by default)

---

## 📌 Quick Folder Map

```text
News_Portal.UI/
  wwwroot/
    images/
      profiles/
      news/
      defaults/

News_Portal.Core/
News_Portal.Infrastructure/
```

---

## 🤝 Contributing

Contributions are welcome:

- Bug fixes
- UI improvements
- Feature additions
- Refactoring & performance enhancements

---

## 📄 License

This project is intended for educational/portfolio usage.
Add a license file if you plan to open-source it officially.

---

⭐ If you like this project, consider starring the repository!
