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


ALTER TABLE public.squadmember DROP CONSTRAINT squadmember_squad_fk;

-- Then, add a new foreign key constraint with ON DELETE CASCADE
ALTER TABLE public.squadmember
ADD CONSTRAINT squadmember_squad_fk
FOREIGN KEY (squadid)
REFERENCES public.squad (squadid)
ON DELETE CASCADE;


CREATE TABLE attendanceType (
	attendanceCode serial PRIMARY KEY,
	attendanceDescription character varying(255) NOT NULL
);


CREATE TABLE IF NOT EXISTS public.eventmemberattendance
(
    attendanceid serial primary key,
    eventid bigint,
    memberid bigint,
    attendanceCode bigint,
    CONSTRAINT attendance_event_fk FOREIGN KEY (eventid)
        REFERENCES public.squadevent (eventid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE, -- Enable cascading delete on the eventid column
    CONSTRAINT attendance_member_fk FOREIGN KEY (memberid)
        REFERENCES public.squadmember (memberid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE, -- Enable cascading delete on the memberid column
	CONSTRAINT attendance_code_fk FOREIGN KEY (attendanceCode)
        REFERENCES public.attendancetype (attendanceCode) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE CASCADE -- Enable cascading delete on the memberid column
)

--ALTER TABLE, shareablelink
ALTER TABLE public.squadevent
ADD COLUMN shareableLink text;