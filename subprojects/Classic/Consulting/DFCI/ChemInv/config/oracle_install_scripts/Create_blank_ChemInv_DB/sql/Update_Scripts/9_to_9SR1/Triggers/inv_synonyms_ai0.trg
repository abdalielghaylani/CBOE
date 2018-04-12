CREATE OR REPLACE TRIGGER TRG_AUDIT_INV_SYNONYMS_AI0
  AFTER INSERT
  ON inv_synonyms
  FOR EACH ROW
DECLARE
  raid number(10);
BEGIN
 	SELECT seq_audit.NEXTVAL INTO raid FROM dual;
  audit_trail.record_transaction
    (raid, 'INV_SYNONYMS', :NEW.rid, 'I');
END;
/