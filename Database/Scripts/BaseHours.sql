-- Creates the (basehours) database with the appropiate enconding and locale settings
CREATE DATABASE basehours
WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'pt_BR.UTF-8'
    LC_CTYPE = 'pt_BR.UTF-8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;


-- Stores user account information, including authentication and verification details
CREATE TABLE user_accounts (
    id UUID PRIMARY KEY, -- The .NET Core backend should generate a UUID v7 before inserting
    name VARCHAR(100) NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    cpf BYTEA NOT NULL, -- CPF encrypted using AES-256 for security and LGPD compliance
    email_verified BOOLEAN DEFAULT FALSE, -- Indicates whether the email has been verified
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Stores email verification tokens to validate user email addresses
CREATE TABLE email_verification_tokens (
    id UUID PRIMARY KEY, -- The .NET Core backend should generate a UUID v7 before inserting
    user_id UUID NOT NULL REFERENCES user_accounts(id) ON DELETE CASCADE, -- Link to the user
    verification_code VARCHAR(6) NOT NULL, -- 6-digit verification code
    expires_at TIMESTAMP NOT NULL, -- Expiration timestamp
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Stores client information
CREATE TABLE clients (
    id UUID PRIMARY KEY, -- The .NET Core backend should generate a UUID v7 before inserting
    name VARCHAR(200) NOT NULL,
    normalized_name varchar(200) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

create unique index ix_clients_normalized_name on clients(normalized_name);

-- Stores project information
create table projects (
    id uuid primary key,
    name varchar(200) not null,
    normalized_name varchar(255) not null,
    created_at timestamp not null
);

create unique index ix_projects_normalized_name on projects (normalized_name);

-- Stores task information
CREATE TABLE tasks (
    id UUID PRIMARY KEY, -- The .NET Core backend should generate a UUID v7 before inserting
    project_id UUID REFERENCES projects(id) ON DELETE CASCADE, -- Link to the project
    user_id UUID REFERENCES users(id) ON DELETE SET NULL, -- Link to the assigned user
    name VARCHAR(255) NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Stores tag information
CREATE TABLE tags (
    id UUID PRIMARY KEY, -- The .NET Core backend should generate a UUID v7 before inserting
    name VARCHAR(50) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Stores the relationship between tasks and tags
CREATE TABLE task_tags (
    task_id UUID REFERENCES tasks(id) ON DELETE CASCADE, -- Link to the task
    tag_id UUID REFERENCES tags(id) ON DELETE CASCADE, -- Link to the tag
    PRIMARY KEY (task_id, tag_id)
);

-- Stores time tracking entries for tasks
CREATE TABLE time_entries (
    id UUID PRIMARY KEY, -- The .NET Core backend should generate a UUID v7 before inserting
    task_id UUID REFERENCES tasks(id) ON DELETE CASCADE, -- Link to the task
    user_id UUID REFERENCES users(id) ON DELETE SET NULL, -- Link to the user who logged time
    start_time TIMESTAMP NOT NULL,
    end_time TIMESTAMP,
    duration INTERVAL GENERATED ALWAYS AS (end_time - start_time) STORED, -- Auto-calculated duration
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
