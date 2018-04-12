alter table inv_locations add constraint INV_Locationw_PrincipalID_FK
FOREIGN KEY (PRINCIPAL_ID_FK)
   REFERENCES coedb.coeprincipal (PRINCIPAL_ID);
   
alter table inv_containers add constraint INV_Contianors_PrincipalID_FK 
FOREIGN KEY (PRINCIPAL_ID_FK)
   REFERENCES coedb.coeprincipal (PRINCIPAL_ID); 
   
   
alter table inv_plates add constraint INV_Plates_PrincipalID_FK 
FOREIGN KEY (PRINCIPAL_ID_FK)
   REFERENCES coedb.coeprincipal (PRINCIPAL_ID); 