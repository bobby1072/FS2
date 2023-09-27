
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