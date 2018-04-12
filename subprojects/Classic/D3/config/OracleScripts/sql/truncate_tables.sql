
alter table "D3DATA"."DRUGDEG_EXPTS" disable constraint "REFDRUGDEG_CONDS1";
alter table "D3DATA"."DRUGDEG_MECHS" disable constraint "REFDRUGDEG_DEGS4";
alter table "D3DATA"."DRUGDEG_DEGS" disable constraint "REFDRUGDEG_EXPTS2";
alter table "D3DATA"."DRUGDEG_EXPTS" disable constraint "REFDRUGDEG_PARENTS7";
alter table "D3DATA"."DRUGDEG_DEGSFGROUP" disable constraint "REF_DEGSFGROUP1";
alter table "D3DATA"."DRUGDEG_DEGSFGROUP" disable constraint "REF_DEGSFGROUP2";

truncate table D3DATA.DRUGDEG_MECHS;
truncate table D3DATA.DRUGDEG_DEGSFGROUP;
truncate table D3DATA.DRUGDEG_BASE64;
truncate table D3DATA.DRUGDEG_SALTS;
truncate table D3DATA.DRUGDEG_STATUSES;
truncate table D3DATA.DRUGDEG_DEGS;
truncate table D3DATA.DRUGDEG_EXPTS;
truncate table D3DATA.DRUGDEG_PARENTS;
truncate table D3DATA.DRUGDEG_CONDS;
truncate table D3DATA.DRUGDEG_FGROUPS;

