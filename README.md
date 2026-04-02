# 📰 News Portal

**A Role-Based ASP.NET Core MVC News Publishing Platform**

News Portal is a **multi-role ASP.NET Core MVC application** for publishing and managing news content. It supports **public news browsing**, **author workflows** for creating articles, and **admin tools** for moderation, pinning, and user management.

---

## 🚀 Live Demo

> **Demo URL**: [newsportal.runasp.net/](newsportal.runasp.net/)

### 🔑 Demo Credentials

| Role | Email | Password | 
|------|-------|----------|
| 👑 **Admin** | `TesterAdmin@gmail.com` | `TesterAdmin` | 
| ✍️ **Author** | `TesterAuthor1@gmail.com` | `TesterAuthor1` |
| 👤 **User** | `Register To Check` |



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