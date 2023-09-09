CREATE TABLE public.user (
    email TEXT PRIMARY KEY UNIQUE,
    password_hash TEXT NOT NULL,
    phone_number TEXT,
    created_at TIMESTAMP DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC')
);