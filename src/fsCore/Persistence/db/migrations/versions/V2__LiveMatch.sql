CREATE TABLE public."active_live_match" (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    group_id UUID NOT NULL,
    match_name TEXT NOT NULL,
    serialised_match_rules TEXT NOT NULL,
    match_status INTEGER NOT NULL,
    match_win_strategy INTEGER NOT NULL,
    match_leader_id UUID NOT NULL,
    commences_at TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    ends_at TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    CONSTRAINT active_live_match_group_id_fk FOREIGN KEY (group_id) REFERENCES public."group"(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT active_live_match_match_leader_id_fk FOREIGN KEY (match_leader_id) REFERENCES public."user"(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE public."active_live_match_catch" (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    user_id UUID NOT NULL,
    species TEXT NOT NULL,
    weight DECIMAL NOT NULL,
    length DECIMAL NOT NULL,
    description TEXT,
    match_id UUID NOT NULL,
    longitude DECIMAL NOT NULL,
    latitude DECIMAL NOT NULL,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    caught_at TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    world_fish_taxocode TEXT,
    CONSTRAINT active_live_match_catch_world_fish_taxocode_fk FOREIGN KEY (world_fish_taxocode) REFERENCES public.world_fish(taxocode) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT active_live_match_match_id_fk FOREIGN KEY (match_id) REFERENCES public.active_live_match(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT active_live_match_user_id_fk FOREIGN KEY (user_id) REFERENCES public."user"(id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE public."active_live_match_participant" (
    id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    user_id UUID NOT NULL,
    match_id UUID NOT NULL,
    CONSTRAINT active_live_match_participant_match_id_fk FOREIGN KEY (match_id) REFERENCES public.active_live_match(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT active_live_match_participant_user_id_fk FOREIGN KEY (user_id) REFERENCES public."user"(id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT active_live_match_participant_unique UNIQUE (user_id, match_id)
);
