--Copyright 1999-2004 CambridgeSoft Corporation. All rights reserved

-- Migrate STRUCTURES.BASE64_CDX and TEMPORARY_STRUCTURES.BASE64_CDX
-- from LONG to CLOB columns using Oracle 9i strategy.

prompt 'converting structure fields to CLOB...'

alter table structures modify base64_cdx clob;
alter table temporary_structures modify base64_cdx clob;


-- Move CLOBs to their own tablespace

prompt 'moving clobs to their own tablespace...'

alter table structures move lob (base64_cdx) store as
(tablespace &&lobsTableSpaceName disable storage in row nocache chunk 2K pctversion 10);
alter table temporary_structures move lob (base64_cdx) store as
(tablespace &&lobsTableSpaceName disable storage in row nocache chunk 2K pctversion 10);


-- Note: Oracle recommends rebuilding indexes after alter table modify. 
-- We will rebuild all indexes later so no need to do it here. 




