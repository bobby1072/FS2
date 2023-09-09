-- Creating tables
CREATE TABLE public.permission (
    buzzword TEXT PRIMARY KEY
);

CREATE TABLE public.accessibility(
    title TEXT PRIMARY KEY
);

CREATE TABLE public.user_role (
    role_name TEXT PRIMARY KEY,
    user_permissions TEXT[] NOT NULL,
    user_affect TEXT NOT NULL,
    CONSTRAINT fk_user_affect FOREIGN KEY (user_affect) REFERENCES public.accessibility(title),  
    group_permissions TEXT[] NOT NULL,
    group_affect TEXT NOT NULL,
    CONSTRAINT fk_group_affect FOREIGN KEY (group_affect) REFERENCES public.accessibility(title)
);

CREATE TABLE public.user (
    username TEXT PRIMARY KEY UNIQUE,
    name TEXT NOT NULL,
    description TEXT, 
    verified BOOLEAN DEFAULT false,
    email TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    phone_number TEXT,
    created_at TIMESTAMP DEFAULT (CURRENT_TIMESTAMP AT TIME ZONE 'UTC'),
    role_name TEXT NOT NULL,
    CONSTRAINT fk_role_name FOREIGN KEY (role_name) REFERENCES public.user_role(role_name)
);


-- Inserting values
INSERT INTO
    public.permissions(buzzword)
VALUES
    ('Read'),
    ('Create'),
    ('Update'),
    ('Delete');

INSERT INTO
    public.accessibility
VALUES
    ('All'),
    ('Relative'),
    ('Self');

INSERT INTO
    public.user_role(role_name, user_permissions, user_affect, group_permissions, group_affect)
VALUES
    ('Admin', ARRAY['Read', 'Create', 'Update', 'Delete'], 'All', ARRAY['Read', 'Create', 'Update', 'Delete'], 'All'),
    ('StandardUser', ARRAY['Read', 'Update', 'Delete'], 'Self', ARRAY['Read', 'Update', 'Create', 'Delete'], 'Relative')
