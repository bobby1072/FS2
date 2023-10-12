CREATE TABLE public."group" (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    name TEXT NOT NULL UNIQUE,
    description TEXT,
    leader_email TEXT NOT NULL REFERENCES "user" (email),
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    positions TEXT[] NOT NULL,
    public BOOLEAN NOT NULL DEFAULT FALSE,
    listed BOOLEAN NOT NULL DEFAULT FALSE,
    emblem BYTEA NOT NULL
);

CREATE TABLE public.group_member (
    group_id UUID NOT NULL REFERENCES "group" (id),
    member_email TEXT NOT NULL REFERENCES "user" (email),
    PRIMARY KEY (group_id, member_email)
);
