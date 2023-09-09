-- Creating tables
CREATE TABLE public.permissions (
    buzzword TEXT PRIMARY KEY
);
CREATE TABLE public.accessibility(
    title TEXT PRIMARY KEY
)
CREATE TABLE public.user_role (
    role_name TEXT PRIMARY KEY,
    user_permissions TEXT[] NOT NULL,
    user_affect TEXT NOT NULL,
    CONSTRAINT fk_user_affect FOREIGN KEY (user_affect) REFERENCES public.accessibility(title)  
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


-- Creating functions
CREATE OR REPLACE FUNCTION validate_permissions()
RETURNS TRIGGER AS $$
BEGIN
  IF (
    NOT EVERY (element IN NEW.user_permissions
               WHERE element IN (SELECT buzzword FROM public.permissions))
    OR
    NOT EVERY (element IN NEW.group_permissions
               WHERE element IN (SELECT buzzword FROM public.permissions))
  ) THEN
    RAISE EXCEPTION 'Invalid buzzwords in user_permissions or group_permissions arrays';
  END IF;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;


-- Adding triggers
CREATE TRIGGER validate_permissions_before_insert
BEFORE INSERT ON public.user_role
FOR EACH ROW
EXECUTE FUNCTION validate_permissions();

CREATE TRIGGER validate_permissions_before_update
BEFORE UPDATE ON public.user_role
FOR EACH ROW
EXECUTE FUNCTION validate_permissions();


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
