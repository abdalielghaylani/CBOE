CREATE OR REPLACE TRIGGER TRG_AUDIT_INV_CTNR_CHK_DTL_AI0
  AFTER INSERT
  ON inv_container_checkin_details
  FOR EACH ROW
DECLARE
  raid number(10);
BEGIN
 	SELECT seq_audit.NEXTVAL INTO raid FROM dual;
  audit_trail.record_transaction
    (raid, 'INV_CONTAINER_CHECKIN_DETAILS', :NEW.rid, 'I');
END;
/