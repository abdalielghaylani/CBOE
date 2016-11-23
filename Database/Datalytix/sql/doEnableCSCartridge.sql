-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

prompt
prompt Enable PerkinElmer CSCartridge in Datalytix
prompt

@@create_VALID_CARTRIDGE_Table.sql
@@insert_cartridge_type.sql 1 'PerkinElmer CSCartridge' &cs_cartridge_owner
