CREATE TABLE public."group_venues"(
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    group_id UUID NOT NULL,
    name TEXT NOT NULL,
    description TEXT,
    latitude DECIMAL NOT NULL,
    longitude DECIMAL NOT NULL,
    CONSTRAINT group_venues_group_id_fk FOREIGN KEY (group_id) REFERENCES public."group"(id) ON UPDATE CASCADE ON DELETE CASCADE
);