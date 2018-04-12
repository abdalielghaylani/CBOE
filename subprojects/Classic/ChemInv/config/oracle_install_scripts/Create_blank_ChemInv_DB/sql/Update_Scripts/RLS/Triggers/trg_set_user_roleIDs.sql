CREATE OR REPLACE TRIGGER Set_User_RoleIDs
AFTER logon ON DATABASE
BEGIN 			 
						 CTX_Cheminv_Mgr.SetRoleIDs;
END;         
/    
