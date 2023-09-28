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
    email TEXT NOT NULL UNIQUE,
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