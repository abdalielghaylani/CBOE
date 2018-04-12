alter table INV_REQUESTS modify  
QUANTITY_LIST varchar2(4000);

alter table INV_REQUESTS add (
	Required_Unit_id_fk number(4,0)
);
