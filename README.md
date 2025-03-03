# BaseHours

![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-blue?logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-blue?logo=postgresql)

**BaseHours** is an educational project designed to manage project time tracking and personal task management, following the principles of **Clean Architecture**. The project utilizes PostgreSQL as its database and modern technologies to ensure scalability and organization.

## 📂 Database Structure

The database consists of the following main entities:

- `user_accounts`: Manager users and authentication.
- `email_verification_tokens`: Store email verification tokens.
- `clients`: Registeres client information.
- `projects`: Tracks projects linjed to clients.
- `tags`: Allows task categorization using labels.
- `task_tags`: Many-to-many relationship between tasks and tags.
- `time_entries`: Logs hours worked by users on tasks.

## 🚀 How to Create the Database

To create database and its tables, run the following command in PostgreSQL:

```sql
psql -U postgres -f Scripts/BaseHours.sql
```

## 📜 Requirements

- PostgreSQL 17
- .NET Core 8

## 🛠 Technologies Used

BaseHOurs is built using the follwing technologies and libraries:

- 🚀 **.NET Core 8**
- 🟦 **[Microsoft.EntityFrameworkCore (9.0.2)](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/)**
- 🔄 **[Npgsql.EntityFrameworkCore.PostgreSQL (9.0.3)](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL/)**
- 🔑 **[Flavio.Santos.UuidV7.NetCore (1.0.2)](https://www.nuget.org/packages/Flavio.Santos.UuidV7.NetCore/)**
- 📡 **[Flavio.Santos.NetCore.ApiResponse (1.0.1)](https://www.nuget.org/packages/Flavio.Santos.NetCore.ApiResponse/)**

## 📌 Project Structure

```
BaseHours/
│── Api/                   # (Presentation) API and controllers
│── Application/           # Use cases and application interfaces
│── Domain/                # Business entities and rules
│── Infrastructure/        # Data persistence and repositories
│── Database/
│   ├── Scripts/
│   │   ├── BaseHours.sql  # Database creation script
│   │   ├── ... other scripts
│── README.md              # Project documentation
```