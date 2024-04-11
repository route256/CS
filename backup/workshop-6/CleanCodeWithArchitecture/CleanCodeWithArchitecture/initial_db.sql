create database "CleanCodeWorkshop";

create table if not exists exchange_rates
(
	date date not null,
	currency_code bpchar(3) not null,
	rate numeric(12,4) not null
);

