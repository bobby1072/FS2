CREATE TABLE public.world_fish (
    scientific_name TEXT,
    taxocode TEXT PRIMARY KEY,
    isscaap TEXT,
    a3_code TEXT,
    english_name TEXT,
    nickname TEXT
);

CREATE TABLE public.user (
    email TEXT PRIMARY KEY,
    name TEXT,
    email_verified BOOLEAN NOT NULL DEFAULT FALSE,
    username TEXT UNIQUE,
);
