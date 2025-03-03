# BaseHours

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

- PostgreSQL 13+

## 📌 Project Structure

```
BaseHours/
│── Database/
│   ├── Scripts/
│   │   ├── BaseHours.sql  # Database creation script
│   │   ├── ... other scripts # Additional scripts
│── README.md
```