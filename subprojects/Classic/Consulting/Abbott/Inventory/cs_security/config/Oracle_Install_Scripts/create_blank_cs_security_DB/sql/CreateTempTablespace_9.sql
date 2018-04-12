--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

--- Oracle 9i Temporary tablespace syntax

CREATE TEMPORARY TABLESPACE &&tempTableSpaceName
	TEMPFILE '&&tempTableSpaceFile' SIZE &&tempTablespaceSize REUSE
	AUTOEXTEND ON MAXSIZE UNLIMITED
    EXTENT MANAGEMENT LOCAL UNIFORM SIZE &&tempTablespaceExtent;
