
alter table REGDB.SEQUENCE add (AUTOSELCOMPDUPCHK NCHAR(1) DEFAULT 'F' );

begin

 &&schemaName..configurationcompoundregistry.addfieldtoview('AUTOSELCOMPDUPCHK', 'AUTOSELCOMPDUPCHK', 'VW_SEQUENCE', 'SEQUENCE');

end;
/