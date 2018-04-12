-- docmgr_external_links_au0.trg
-- Script to create AFTER-UPDATE audit trigger for the 
-- changed docmgr_external_links fields.

CREATE OR REPLACE TRIGGER TRG_AUDIT_DMEL_AU0 
AFTER UPDATE OF 
    APPNAME, LINKTYPE, LINKID, DOCID,
    LINKFIELDNAME, SUBMITTER, DATE_SUBMITTED
ON DOCMGR_EXTERNAL_LINKS FOR EACH ROW 
DECLARE
  raid number(10);
BEGIN
  SELECT seq_audit.NEXTVAL INTO raid FROM dual;

  audit_trail.record_transaction
    (raid, 'DOCMGR_EXTERNAL_LINKS', :old.rid, 'U');

  IF UPDATING('APPNAME') THEN
    IF NVL(:old.APPNAME,' ') != NVL(:new.APPNAME,' ') THEN
       audit_trail.column_update
         (raid, 'APPNAME',
         :old.APPNAME, :new.APPNAME);
    END IF;
  END IF;

  IF UPDATING('LINKTYPE') THEN
    IF NVL(:old.LINKTYPE,' ') != NVL(:new.LINKTYPE,' ') THEN
       audit_trail.column_update
         (raid, 'LINKTYPE',
         :old.LINKTYPE, :new.LINKTYPE);
    END IF;
  END IF;

  IF UPDATING('LINKID') THEN
    IF NVL(:old.LINKID,' ') != NVL(:new.LINKID,' ') THEN
       audit_trail.column_update
         (raid, 'LINKID',
         :old.LINKID, :new.LINKID);
    END IF;
  END IF;

  IF UPDATING('DOCID') THEN
    IF NVL(:old.DOCID,0) != NVL(:new.DOCID,0) THEN
       audit_trail.column_update
         (raid, 'DOCID',
         :old.DOCID, :new.DOCID);
    END IF;
  END IF;

  IF UPDATING('LINKFIELDNAME') THEN
    IF NVL(:old.LINKFIELDNAME,' ') != NVL(:new.LINKFIELDNAME,' ') THEN
       audit_trail.column_update
         (raid, 'LINKFIELDNAME',
         :old.LINKFIELDNAME, :new.LINKFIELDNAME);
    END IF;
  END IF;

  IF UPDATING('SUBMITTER') THEN
    IF NVL(:old.SUBMITTER,' ') != NVL(:new.SUBMITTER,' ') THEN
       audit_trail.column_update
         (raid, 'SUBMITTER',
         :old.SUBMITTER, :new.SUBMITTER);
    END IF;
  END IF;

  IF UPDATING('DATE_SUBMITTED') THEN
    IF :old.DATE_SUBMITTED != :new.DATE_SUBMITTED THEN
       audit_trail.column_update
         (raid, 'DATE_SUBMITTED',
         :old.DATE_SUBMITTED, :new.DATE_SUBMITTED);
    END IF;
  END IF;

END;
/
