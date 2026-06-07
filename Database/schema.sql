-- ============================================================
--  auuuxMS — Database Schema
--  Run this script on Supabase: SQL Editor → New query
-- ============================================================

create extension if not exists "uuid-ossp";

-- ============================================================
--  USERS
-- ============================================================
create table if not exists users (
    id          uuid         primary key default uuid_generate_v4(),
    username    varchar(50)  not null unique,
    email       varchar(255) not null unique,
    avatar_url  text,
    bio         text,
    created_at  timestamptz  not null default now(),
    updated_at  timestamptz  not null default now()
);

-- ============================================================
--  FOLLOWS
-- ============================================================
create table if not exists follows (
    follower_id uuid        not null references users(id) on delete cascade,
    following_id uuid       not null references users(id) on delete cascade,
    created_at  timestamptz not null default now(),
    primary key (follower_id, following_id),
    constraint no_self_follow check (follower_id <> following_id)
);

-- ============================================================
--  MOOD TAGS
-- ============================================================
create table if not exists mood_tags (
    id    smallserial  primary key,
    name  varchar(50)  not null unique   -- es. "nostalgia", "late night", "high energy"
);

insert into mood_tags (name) values
    ('nostalgia'),
    ('melancholy'),
    ('late night'),
    ('high energy'),
    ('chill'),
    ('hype'),
    ('heartbreak'),
    ('euphoria'),
    ('focus'),
    ('road trip')
on conflict do nothing;

-- ============================================================
--  ARTISTS  (cache Spotify)
-- ============================================================
create table if not exists artists (
    id          uuid         primary key default uuid_generate_v4(),
    spotify_id  varchar(50)  not null unique,
    name        varchar(255) not null,
    image_url   text,
    genres      text[],
    created_at  timestamptz  not null default now()
);

-- ============================================================
--  ALBUMS  (cache Spotify)
-- ============================================================
create table if not exists albums (
    id           uuid         primary key default uuid_generate_v4(),
    spotify_id   varchar(50)  not null unique,
    artist_id    uuid         not null references artists(id) on delete cascade,
    title        varchar(255) not null,
    release_date date,
    cover_url    text,
    total_tracks smallint,
    album_type   varchar(20),             -- 'album' | 'single' | 'compilation'
    created_at   timestamptz  not null default now()
);

-- ============================================================
--  TRACKS  (cache Spotify)
-- ============================================================
create table if not exists tracks (
    id           uuid         primary key default uuid_generate_v4(),
    spotify_id   varchar(50)  not null unique,
    album_id     uuid         not null references albums(id) on delete cascade,
    title        varchar(255) not null,
    track_number smallint,
    duration_ms  int,
    created_at   timestamptz  not null default now()
);

-- ============================================================
--  CONNECTS
--  L'utente "connette" un brano, album o artista — con testo e mood opzionali
-- ============================================================
create table if not exists connects (
    id           uuid         primary key default uuid_generate_v4(),
    user_id      uuid         references users(id) on delete set null,
    track_id     uuid         references tracks(id) on delete cascade,
    album_id     uuid         references albums(id) on delete cascade,
    artist_id    uuid         references artists(id) on delete cascade,
    content      text,
    mood_tag_id  smallint     references mood_tags(id) on delete set null,
    created_at   timestamptz  not null default now(),

    -- deve puntare a esattamente uno tra track, album, artista
    constraint connect_target_check check (
        (track_id  is not null)::int +
        (album_id  is not null)::int +
        (artist_id is not null)::int = 1
    )
);

-- ============================================================
--  RATES (voti da 0.0 a 10.0)
-- ============================================================
create table if not exists rates (
    id           uuid           primary key default uuid_generate_v4(),
    user_id      uuid           references users(id) on delete set null,
    track_id     uuid           references tracks(id) on delete cascade,
    album_id     uuid           references albums(id) on delete cascade,
    rating       numeric(3,1)   not null check (rating >= 0.0 and rating <= 10.0),
    content      text,
    mood_tag_id  smallint       references mood_tags(id) on delete set null,
    created_at   timestamptz    not null default now(),
    updated_at   timestamptz    not null default now(),

    constraint rate_target_check check (
        (track_id is not null)::int +
        (album_id is not null)::int = 1
    ),
    unique (user_id, track_id),
    unique (user_id, album_id)
);

-- ============================================================
--  MOMENTS  (post live con finestra di 27 minuti)
-- ============================================================
create table if not exists moments (
    id          uuid         primary key default uuid_generate_v4(),
    user_id     uuid         references users(id) on delete set null,
    track_id    uuid         not null references tracks(id) on delete cascade,
    content     text,
    mood_tag_id smallint     references mood_tags(id) on delete set null,
    created_at  timestamptz  not null default now(),
    expires_at  timestamptz  not null default (now() + interval '27 minutes')
);

-- ============================================================
--  MY VINYL  (12 tracce identitarie per utente)
-- ============================================================
create table if not exists my_vinyl (
    user_id         uuid      primary key references users(id) on delete cascade,
    swaps_remaining smallint  not null default 2 check (swaps_remaining >= 0),
    updated_at      timestamptz not null default now()
);

create table if not exists my_vinyl_tracks (
    user_id   uuid      not null references my_vinyl(user_id) on delete cascade,
    track_id  uuid      not null references tracks(id) on delete cascade,
    position  smallint  not null check (position between 1 and 12),
    primary key (user_id, position),
    unique (user_id, track_id)
);

-- ============================================================
--  TOP 3 ARTISTI / ALBUM  (pinnati sul profilo)
-- ============================================================
create table if not exists user_top_artists (
    user_id   uuid     not null references users(id) on delete cascade,
    artist_id uuid     not null references artists(id) on delete cascade,
    position  smallint not null check (position between 1 and 3),
    primary key (user_id, position),
    unique (user_id, artist_id)
);

create table if not exists user_top_albums (
    user_id  uuid     not null references users(id) on delete cascade,
    album_id uuid     not null references albums(id) on delete cascade,
    position smallint not null check (position between 1 and 3),
    primary key (user_id, position),
    unique (user_id, album_id)
);

-- ============================================================
--  INDEXES
-- ============================================================
create index if not exists idx_follows_follower      on follows(follower_id);
create index if not exists idx_follows_following     on follows(following_id);
create index if not exists idx_albums_artist_id      on albums(artist_id);
create index if not exists idx_tracks_album_id       on tracks(album_id);
create index if not exists idx_connects_user_id      on connects(user_id);
create index if not exists idx_connects_created_at   on connects(created_at desc);
create index if not exists idx_rates_user_id         on rates(user_id);
create index if not exists idx_rates_created_at      on rates(created_at desc);
create index if not exists idx_moments_user_id       on moments(user_id);
create index if not exists idx_moments_expires_at    on moments(expires_at);
create index if not exists idx_my_vinyl_tracks_user  on my_vinyl_tracks(user_id);

-- ============================================================
--  AUTO-UPDATE updated_at
-- ============================================================
create or replace function update_updated_at()
returns trigger as $$
begin
    new.updated_at = now();
    return new;
end;
$$ language plpgsql;

create or replace trigger trg_users_updated_at
    before update on users
    for each row execute function update_updated_at();

create or replace trigger trg_rates_updated_at
    before update on rates
    for each row execute function update_updated_at();

create or replace trigger trg_my_vinyl_updated_at
    before update on my_vinyl
    for each row execute function update_updated_at();
