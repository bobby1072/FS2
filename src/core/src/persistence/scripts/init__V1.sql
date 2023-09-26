-- Creating tables
CREATE TABLE public.world_fish (
    scientific_name TEXT,
    taxocode TEXT PRIMARY KEY,
    isscaap INTEGER,
    a3_code TEXT,
    english_name TEXT,
    nickname TEXT
);

CREATE TABLE public.user_role (
    role_name TEXT PRIMARY KEY,
    group_permissions TEXT[] NOT NULL
);

CREATE TABLE public.user (
    username TEXT PRIMARY KEY,
    name TEXT,
    description TEXT, 
    verified BOOLEAN DEFAULT false,
    email TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    phone_number TEXT UNIQUE,
    created_at TIMESTAMP DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
    role_name TEXT NOT NULL,
    CONSTRAINT fk_role_name FOREIGN KEY (role_name) REFERENCES public.user_role(role_name)
);


-- Inserting values
INSERT INTO
    public.user_role(role_name, group_permissions)
VALUES
    ('AdminUser', ARRAY['Read', 'Create', 'Update', 'Delete']),
    ('StandardUser', ARRAY['Read']);