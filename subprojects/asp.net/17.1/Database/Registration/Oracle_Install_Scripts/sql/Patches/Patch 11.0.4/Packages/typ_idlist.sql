-- AUTHOR: Jeff Dugas / 15-FEB-2011
-- Creates an  associative array that can be re-used for collections
-- of primary key values.
CREATE OR REPLACE TYPE "REGDB"."IDLIST" AS TABLE OF Number
/