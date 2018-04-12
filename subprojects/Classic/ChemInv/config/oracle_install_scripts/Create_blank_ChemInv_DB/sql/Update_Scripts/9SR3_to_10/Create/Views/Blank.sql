-- This is just a dummy view so that we can create the public synonym in just one location.  The actual
-- view will be created by the reg integration to 10 update script.
CREATE VIEW &&schemaName..INV_VW_COMPOUNDS
AS select null as dummy from dual;

