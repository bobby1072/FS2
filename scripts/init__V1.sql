-- Creating tables
CREATE TABLE public.world_fish (
    scientific_name TEXT,
    taxocode TEXT PRIMARY KEY,
    isscaap INTEGER,
    a3_code TEXT,
    english_name TEXT,
    nickname TEXT
);

CREATE TABLE public.user (
    name TEXT,
    email_verified BOOLEAN DEFAULT false,
    email TEXT NOT NULL UNIQUE
);

CREATE TABLE public.group (
    group_id uuid PRIMARY KEY DEFAULT gen_random_uuid()
);

CREATE TABLE public.custom_marker (
    custom_marker_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    raw_json TEXT NOT NULL,
    custom_marker_image BYTEA,
    group_id uuid NOT NULL,
    CONSTRAINT fk_group_id FOREIGN KEY (group_id) REFERENCES public.group(group_id)
);

-- Inserting test values
-- Insert into public.world_fish
INSERT INTO public.world_fish (scientific_name, taxocode, isscaap, a3_code, english_name, nickname)
VALUES ('TestFish', 'TF001', 123, 'A3', 'Test English Fish', 'Test Nickname');

-- Insert into public.user
INSERT INTO public.user (name, email_verified, email)
VALUES ('Test User', false, 'test@example.com');

-- Insert into public.group
INSERT INTO public.group (group_id)
VALUES (gen_random_uuid());

-- Insert into public.custom_marker
INSERT INTO public.custom_marker (raw_json, custom_marker_image, group_id)
VALUES ('{"data": "Test JSON"}', E'\\x0123456789ABCDEF', (SELECT group_id FROM public.group LIMIT 1));