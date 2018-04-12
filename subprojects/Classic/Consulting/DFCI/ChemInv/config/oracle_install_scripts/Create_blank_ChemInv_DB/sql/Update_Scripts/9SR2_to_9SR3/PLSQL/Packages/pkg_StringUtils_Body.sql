CREATE OR REPLACE  PACKAGE BODY "STRINGUTILS"    
AS
	FUNCTION split(pString_in in varchar2, delimiter in varchar2:=',') RETURN t_char
	IS
		v_char t_char;
		s varchar2(8000);
		d varchar2(8000);
	BEGIN
		d := delimiter;
		s := d || pString_in || d;
		For i in 1.. Length(s)
		  Loop v_char(i) := Substr(s, instr(s,d,1,i)+1, instr(s,d,1,i+1) - instr(s,d,1,i)-1);
		  if v_char(i) IS NULL then 
		    v_char.DELETE(i);
		  end if;
		  End Loop;
		  RETURN v_char;
	END split;
END STRINGUTILS;
/
show errors;
