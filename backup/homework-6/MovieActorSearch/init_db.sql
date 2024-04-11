create database "actorsdb";

create table if not exists actors
(
    actor_id bigint not null,
    name varchar(255) not null
);

