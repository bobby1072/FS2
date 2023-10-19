
CREATE TABLE public."group" (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    name TEXT NOT NULL UNIQUE,
    description TEXT,
    leader_email TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    public BOOLEAN NOT NULL DEFAULT FALSE,
    listed BOOLEAN NOT NULL DEFAULT FALSE,
    emblem BYTEA,
    CONSTRAINT group_leader_email_fk FOREIGN KEY (leader_email) REFERENCES public."user"(email) ON UPDATE CASCADE
);
CREATE TABLE public."group_position" (
    id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    group_id UUID NOT NULL,
    group_permissions_id INTEGER NOT NULL,
    name TEXT NOT NULL,
    can_manage_group BOOLEAN NOT NULL DEFAULT FALSE,
    can_read_catches BOOLEAN NOT NULL DEFAULT TRUE,
    can_manage_catches BOOLEAN NOT NULL DEFAULT FALSE,
    can_read_members BOOLEAN NOT NULL DEFAULT TRUE,
    can_manage_members BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT group_position_unique UNIQUE (group_id, name),
    CONSTRAINT group_position_group_permissions_id_fk FOREIGN KEY (group_permissions_id) REFERENCES public."group_permissions"(id) ON UPDATE CASCADE,
    CONSTRAINT group_position_group_id_fk FOREIGN KEY (group_id) REFERENCES public."group"(id) ON UPDATE CASCADE
);
CREATE TABLE public."group_member" (
    id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    group_id UUID NOT NULL,
    user_email TEXT NOT NULL,
    position_id INTEGER NOT NULL,
    CONSTRAINT group_member_unique UNIQUE (group_id, user_email),
    CONSTRAINT group_member_position_id_fk FOREIGN KEY (position_id) REFERENCES public."group_position"(id) ON UPDATE CASCADE,
    CONSTRAINT group_member_group_id_fk FOREIGN KEY (group_id) REFERENCES public."group"(id) ON UPDATE CASCADE,
    CONSTRAINT group_member_member_email_fk FOREIGN KEY (user_email) REFERENCES public."user"(email) ON UPDATE CASCADE
);