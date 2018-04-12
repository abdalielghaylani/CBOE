CREATE OR REPLACE  PACKAGE "STRINGUTILS"  AS
	TYPE t_char IS TABLE of varchar2(8000) INDEX BY BINARY_INTEGER;

	FUNCTION split(pString_in in varchar2, delimiter in varchar2:=',') RETURN t_char;

END STRINGUTILS;
/
show errors;
