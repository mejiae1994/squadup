CREATE TABLE IF NOT EXISTS public.squad
(
    squadid serial PRIMARY KEY,
    squadname character varying(255) COLLATE pg_catalog."default" NOT NULL,
    "createdAt" timestamp with time zone NOT NULL DEFAULT now()
);


CREATE TABLE IF NOT EXISTS public.squadmember
(
    memberid serial PRIMARY KEY,
    membername character varying(255) COLLATE pg_catalog."default" NOT NULL,
	"createdAt" timestamp with time zone NOT NULL DEFAULT now(),
    squadid bigint,
    CONSTRAINT squadmember_squad_fk FOREIGN KEY (squadid) REFERENCES public.squad (squadid) MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION
);


CREATE TABLE IF NOT EXISTS public.squadevent
(
    eventid serial PRIMARY KEY,
    eventname character varying(255) COLLATE pg_catalog."default" NOT NULL,
    eventdate timestamp with time zone, -- Add 'eventdate' column,
    "createdAt" timestamp with time zone NOT NULL DEFAULT now(),
    squadid bigint,
    CONSTRAINT squadevent_squad_fk FOREIGN KEY (squadid) REFERENCES public.squad (squadid) MATCH SIMPLE ON UPDATE NO ACTION ON DELETE NO ACTION
);