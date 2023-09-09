CREATE TABLE public.permissions (
    buzzword TEXT PRIMARY KEY
);

CREATE TABLE public.user_role (
    role_name TEXT PRIMARY KEY,
    affect_user TEXT NOT NULL,
    CONSTRAINT fk_user_buzzword FOREIGN KEY (affect_user) REFERENCES public.permissions(buzzword),
    affect_group TEXT NOT NULL,
    CONSTRAINT fk_group_buzzword FOREIGN KEY (affect_group) REFERENCES public.permissions(buzzword),

);

CREATE TABLE public.user (
    username TEXT PRIMARY KEY UNIQUE,
    first_name TEXT NOT NULL,
    description TEXT, 
    verified BOOLEAN DEFAULT false,
    email TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    phone_number TEXT,
    created_at TIMESTAMP DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
    role_name TEXT NOT NULL,
    CONSTRAINT fk_role_name FOREIGN KEY (role_name) REFERENCES public.user_role(role_name)
);