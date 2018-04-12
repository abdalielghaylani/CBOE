Declare IsExists NUMBER(3,1);
BEGIN
	Select count(id) into IsExists from INV_REPORTFORMATS where REPORTFORMAT ='XLSX';
	if IsExists != 1 then 
	   Insert into INV_REPORTFORMATS (ID,FORMATDISPLAYNAME,REPORTFORMAT,AVAILABLE) 
	   values (5,'Microsoft Excel (.xlsx)','XLSX','1');
	end if;
END;
/
Commit;

