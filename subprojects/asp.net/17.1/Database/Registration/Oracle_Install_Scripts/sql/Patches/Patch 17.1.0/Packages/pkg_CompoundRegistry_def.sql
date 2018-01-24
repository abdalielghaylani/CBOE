prompt 
prompt Starting "pkg_CompoundRegistry_def.sql"...
prompt 
create or replace
PACKAGE "COMPOUNDREGISTRY" AS

  -- Author  : Fari
  -- Created : 4/25/2007
  -- Purpose : CRUD for MultiCompoundRegistryRecord

	/* NOTE: I eliminated the change histors becuase it isn't maintained (JED) */

    --Types
    TYPE TNumericIdList IS TABLE OF NUMBER INDEX BY binary_integer;
    TYPE CURSOR_TYPE IS REF CURSOR;
    TYPE T_ARRAY IS TABLE OF VARCHAR2(500);    
    FUNCTION GetFragmentXML( ATempCompundID  in Number) RETURN CLOB;
    FUNCTION GetIdentifierCompundXML( ATempCompundID  in Number) RETURN CLOB;
    FUNCTION ValidateCompoundMulti(AStructure CLOB, AStructureIDToValidate Number:=NULL, AConfigurationID Number:=1, AXMLCompound XmlType, AXMLFragmentEquivalent XmlType) RETURN CLOB;
    FUNCTION GetDuplicatedList(ARegNumber IN VW_RegistryNumber.RegNumber%type) RETURN CLOB;
    FUNCTION GetActiveRLS RETURN VARCHAR2;
    FUNCTION SplitClob (p_clob_in CLOB, p_delim VARCHAR2) RETURN T_ARRAY;

    --Security
    PROCEDURE CanCreateMultiCompoundRegistry(AXml IN CLOB, AMessage OUT CLOB, ADuplicateCheck Char:='C', AConfigurationID Number:=1);

    --PERM methods
    PROCEDURE CreateMultiCompoundRegistry(AXml IN CLOB, ARegNumber out NOCOPY VW_RegistryNumber.RegNumber%type, AMessage OUT CLOB, ADuplicateCheck Char:='C', ARegNumGeneration IN CHAR := 'Y', AConfigurationID Number:=1, ASectionsList IN Varchar2:=NULL, ASetBatchNumber IN Number := 1);
    PROCEDURE LoadMultiCompoundRegRecord(ARegistryXml IN CLOB, ADuplicateAction IN CHAR, AAction OUT NOCOPY CHAR, AMessage OUT NOCOPY CLOB, ARegNumber OUT NOCOPY VW_RegistryNumber.RegNumber%TYPE, ARegistration IN CHAR := 'Y', ARegNumGeneration IN CHAR := 'Y', AConfigurationID IN Number := 1, ASectionsList IN Varchar2:=NULL, ASetBatchNumber IN Number := 1);
    PROCEDURE RetrieveMultiCompoundRegistry(ARegNumber IN VW_RegistryNumber.RegNumber%type, AXml out NOCOPY clob, ASectionsList in Varchar2:=NULL);
    PROCEDURE GetRegistrationByBatch(ABatchID in vw_batch.batchid%type, AXml out NOCOPY clob);
    PROCEDURE GetRegistrationByBatch(ABatchRegNumber in vw_batch.fullregnumber%type, AXml out NOCOPY clob);
    PROCEDURE UpdateMultiCompoundRegistry(AXml in CLOB, AMessage OUT CLOB, ADuplicateCheck Char:='C', AConfigurationID Number:=1, ASectionsList in Varchar2:=NULL);
    PROCEDURE DeleteMultiCompoundRegistry(ARegNumber IN VW_RegistryNumber.RegNumber%type);

    PROCEDURE MoveBatch(ABatchID in Number, ARegNumber in VW_RegistryNumber.RegNumber%type);
    PROCEDURE RetrieveBatch(AID IN NUMBER, AXml OUT NOCOPY CLOB);
    PROCEDURE UpdateBatch(AXml IN CLOB);
    PROCEDURE DeleteBatch (ABatchID Number);
    PROCEDURE SaveDuplicates(AXMLDuplicated IN clob,APersonID IN VARCHAR2:=null);
    --LIST methods
    PROCEDURE RetrieveMultiCompoundRegList(AXmlRegNumbers in clob, AXmlCompoundList out NOCOPY clob);
    PROCEDURE LoadMultiCompoundRegRecordList(ARegistryListXml In CLOB, ADuplicateAction IN CHAR, ARegistration IN CHAR, ARegNumGeneration IN CHAR, AConfigurationID IN Number, LogID IN NUMBER);
    PROCEDURE RetrieveCompoundRegistryList(AXmlRegNumbers in clob, AXmlCompoundList out NOCOPY clob);
    PROCEDURE RetrieveTempIDList(Ahitlistid IN Number, Aid OUT NOCOPY CLOB);

    --TEMP methods
    PROCEDURE CreateTemporaryRegistration(AXml in CLOB, ATempID out Number, AMessage OUT CLOB, ASectionsList in Varchar2 := NULL);
    PROCEDURE RetrieveTemporaryRegistration(ATempID in Number, AXml out NOCOPY clob);
    PROCEDURE UpdateTemporaryRegistration(AXml in clob, AMessage OUT CLOB, ASectionsList in Varchar2:=NULL);
    PROCEDURE DeleteTemporaryRegistration(ATempID  in Number);

    PROCEDURE RetrieveBatchTmp(ATempID IN NUMBER, AXml OUT NOCOPY CLOB);
    PROCEDURE UpdateBatchTmp(AXml IN CLOB);

    PROCEDURE ConvertTempRegRecordsToPerm(ATempIds IN tNumericIdList, ADuplicateAction IN CHAR, ADescription IN VARCHAR2, AUserID IN VARCHAR2, ALogID OUT NUMBER, AMessage OUT NOCOPY CLOB, ARegistration IN CHAR := 'Y', ARegNumGeneration IN CHAR := 'Y', AConfigurationID IN Number := 1, ASectionsList IN Varchar2 := NULL);
  	PROCEDURE ConvertHitlistTempsToPerm(Ahitlistid IN NUMBER, ADuplicateAction IN CHAR, ADescription IN VARCHAR2, AUserID IN VARCHAR2, ALogID OUT NUMBER, AMessage OUT NOCOPY CLOB, ARegistration IN CHAR := 'Y', ARegNumGeneration IN CHAR := 'Y', AConfigurationID IN NUMBER := 1, ASectionsList IN VARCHAR2 := NULL);
    PROCEDURE LogBulkregistrationId(ALogID OUT NUMBER, ADuplicateAction IN Varchar2, AUserID IN Varchar2, ADescription IN Varchar2 DEFAULT NULL);
    PROCEDURE LogBulkregistration(ALogID IN NUMBER, LATempID IN VARCHAR2, AAction IN char, RegNumber IN VARCHAR2, BatchID IN NUMBER, Result IN VARCHAR2);

	PROCEDURE UpdateApprovedStatus(ATempID IN NUMBER, AStatusID IN NUMBER);
  PROCEDURE UpdateLockedStatus(APermIDList IN VARCHAR2, AStatusID IN NUMBER);
  PROCEDURE  GetCompoundLockStatus(ACompoundid NUMBER,AStatusID OUT NOCOPY NUMBER);
  PROCEDURE  GetLockedRegsiteryList (vRegNumbers IN varchar2 , O_RS OUT CURSOR_TYPE);
  PROCEDURE SetApprovePerson(ATempID IN NUMBER, AStatusID IN NUMBER, APersonID IN NUMBER);

    -- UTILITY
    PROCEDURE InsertLog(ALogProcedure CLOB, ALogComment CLOB);
    /* NOTE: dynamic invocation requires public scope for 'GetFilledPropertyList' */
    FUNCTION GetFilledPropertyList(APropertyListType IN varchar2, AParentIdentifier IN number) RETURN CLOB;
	FUNCTION GetIsBatchEditable(ARegNumber in VW_RegistryNumber.RegNumber%type, pBatchNumber in VW_Batch.BatchNumber%type) RETURN VARCHAR2;

    -- TEST
    FUNCTION GenerateBatchRegNumber(p_batchId vw_batch.batchid%type) RETURN VARCHAR2;

    -- CONSTANTS
    -- must always be false in PerForce
    Debuging                       Constant BOOLEAN := FALSE;
    TraceEnabled                   Constant BOOLEAN := FALSE;

    -- for comparison-testing
    FillPropertyTemplate           Constant BOOLEAN := FALSE;

    eGenericException              Constant Number:=-20000;
    eNoRowsReturned                Constant Number:=-20020;
    eRetrieveSingleCompoundReg     Constant Number:=-20002;
    eCreateMultiCompoundRegistry   Constant Number:=-20009;
    eRetrieveMultiCompoundRegistry Constant Number:=-20010;
    eUpdateMultiCompoundRegistry   Constant Number:=-20011;
    eDeleteMultiCompoundRegistry   Constant Number:=-20012;
    eRetrieveMultiCompoundRegList  Constant Number:=-20023;
    eInsertData                    Constant Number:=-20029;
    eIsNotEditableDeleting         Constant Number:=-20040;
    eCreateMultiCompoundRegTmp     Constant Number:=-20013;
    eRetrieveMultiCompoundRegTmp   Constant Number:=-20014;
    eUpdateMultiCompoundRegTmp     Constant Number:=-20015;
    eDeleteMultiCompoundRegTmp     Constant Number:=-20016;
    eIsNotEditableDeletingTmp      Constant Number:=-20041;
    eRetrieveCompoundRegistryList  Constant Number:=-20024;
    eCompoundValidation            Constant Number:=-20018;
    eRegNumberValidation           Constant Number:=-20033;
    eSetSessionParameter           Constant Number:=-20025;
    eSetColumnsForXmlInsert        Constant Number:=-20026;
    eSameIdentityBetweenBatches    Constant Number:=-20027;
    eWildcardValidation            Constant Number:=-20028;
    eOnlyOneBatch                  Constant Number:=-20030;
    eInvalidRegNum                 Constant Number:=-20031;
    eFragmentNotMatchIdentityTrue  Constant Number:=-20035;
    -- Components don't match between registries. The batch was not moved.
    eMoreComponentsTarget          Constant Number:=-20037;
    --Components don't match between registries. The batch was not moved.
    eMoreComponentsSource          Constant Number:=-20038;
    eOnlyOneBatchDeleting          Constant Number:=-20039;

END CompoundRegistry;
/
