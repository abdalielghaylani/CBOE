prompt 
prompt Starting "pkg_CompoundRegistry_def.sql"...
prompt 

CREATE OR REPLACE PACKAGE REGDB."COMPOUNDREGISTRY" AS

    /******************************************************************************
       NAME:       CompoundRegistry
       PURPOSE:

       REVISIONS:
       Ver        Date        Author           Description
       ---------  ----------  ---------------  ------------------------------------
       1.0        4/25/2007   Fari               Created this package.
       1.0.1      8/14/2008   Bill Claff         Change &lt; to &gt; where needed
       1.0.2      8/28/2008   Bill Claff         Refactor LoadMultiCompoundRegRecordList
       1.0.3      10/10/2008  Bill Claff         Change BOOLEANs to CHARs
       1.0.4      10/29/2008  Bill Claff         Add initial Batch procedures
       1.0.5      11/26/2008  Fari               Support XML without the fragments tags in Registration
       1.0.6      12/15/2008  Bill Claff         Add parameters to LoadMultiCompoundRegRecord
       1.0.7      12/17/2008  Fari               Exceptions of Structures duplicated: show a row for each registry that hold the same compound duplicate
       1.0.8      12/19/2008  Fari               Add MoveBatch and DeleteBatch
       1.0.9      03/17/2009  Fari               CreateMultiCompoundRegistry and UpdateMultiCompoundRegistry return an Out parameter
                                                 rather than throw an error on duplicates
                  12/01/2009  Jeff Dugas         'AMessage' OUT parameter in Update and Create/Load methods now return a result via
                                                 CreateRegistrationResponse which may contain the MultiCompoundRegistryRecord xml
                  01/08/2010  Jeff Dugas         Added 'AppendError' function and support for its usage in exception tracing.
    ******************************************************************************/

    --Types
    TYPE TNumericIdList IS TABLE OF NUMBER INDEX BY binary_integer;

    FUNCTION GetFragmentXML( ATempCompundID  in Number) RETURN CLOB;
    FUNCTION GetIdentifierCompundXML( ATempCompundID  in Number) RETURN CLOB;
    FUNCTION GetSaltDescription(AXmlTables IN XmlType) RETURN VARCHAR2;
    FUNCTION GetSaltCode(AXmlTables IN XmlType) RETURN VARCHAR2;
    FUNCTION ValidateCompoundMulti(AStructure CLOB, ARegIDToValidate Number:=NULL, AConfigurationID Number:=1, AXMLCompound XmlType, AXMLFragmentEquivalent XmlType, AXMLRegNumberDuplicatedHidden OUT NOCOPY XmlType) RETURN CLOB;

    --Security
    PROCEDURE CanCreateMultiCompoundRegistry(AXml IN CLOB, AMessage OUT CLOB, ADuplicateCheck Char:='C', AConfigurationID Number:=1);

    --PERM methods
    PROCEDURE CreateMultiCompoundRegistry(AXml IN CLOB, ARegNumber out NOCOPY VW_RegistryNumber.RegNumber%type, AMessage OUT CLOB, ADuplicateCheck Char:='C', ARegNumGeneration IN CHAR := 'Y', AConfigurationID Number:=1, ASectionsList IN Varchar2:=NULL);
    PROCEDURE LoadMultiCompoundRegRecord(ARegistryXml IN CLOB, ADuplicateAction IN CHAR, AAction OUT NOCOPY CHAR, AMessage OUT NOCOPY CLOB, ARegNumber OUT NOCOPY VW_RegistryNumber.RegNumber%TYPE, ARegistration IN CHAR := 'Y', ARegNumGeneration IN CHAR := 'Y', AConfigurationID IN Number := 1, ASectionsList IN Varchar2:=NULL);
    PROCEDURE RetrieveMultiCompoundRegistry(ARegNumber IN VW_RegistryNumber.RegNumber%type, AXml out NOCOPY clob, ASectionsList in Varchar2:=NULL);
    PROCEDURE UpdateMultiCompoundRegistry(AXml in CLOB, AMessage OUT CLOB, ADuplicateCheck Char:='C', AConfigurationID Number:=1, ASectionsList in Varchar2:=NULL);
    PROCEDURE DeleteMultiCompoundRegistry(ARegNumber in VW_RegistryNumber.RegNumber%type);

    PROCEDURE MoveBatch(ABatchID in Number, ARegNumber in VW_RegistryNumber.RegNumber%type);
    PROCEDURE RetrieveBatch(AID IN NUMBER, AXml OUT NOCOPY CLOB);
    PROCEDURE UpdateBatch(AXml IN CLOB);
    PROCEDURE DeleteBatch (ABatchID Number);

    --LIST methods
    PROCEDURE RetrieveMultiCompoundRegList(AXmlRegNumbers in clob, AXmlCompoundList out NOCOPY clob);
    PROCEDURE LoadMultiCompoundRegRecordList(ARegistryListXml In CLOB, ADuplicateAction IN CHAR, ARegistration IN CHAR, ARegNumGeneration IN CHAR, AConfigurationID IN Number, LogID IN NUMBER);
    PROCEDURE RetrieveCompoundRegistryList(AXmlRegNumbers in clob, AXmlCompoundList out NOCOPY clob);
    PROCEDURE RetrieveTempIDList(Ahitlistid IN Number, Aid OUT NOCOPY CLOB);

    --TEMP methods
    PROCEDURE CreateMultiCompoundRegTmp(AXml in CLOB, ATempID out Number, AMessage OUT CLOB, ASectionsList in Varchar2 := NULL);
    PROCEDURE RetrieveMultiCompoundRegTmp(ATempID  in Number, AXml out NOCOPY clob);
    PROCEDURE UpdateMultiCompoundRegTmp(AXml in clob, AMessage OUT CLOB, ASectionsList in Varchar2:=NULL);
    PROCEDURE DeleteMultiCompoundRegTmp( ATempID  in Number);

    PROCEDURE RetrieveBatchTmp(ATempID IN NUMBER, AXml OUT NOCOPY CLOB);
    PROCEDURE UpdateBatchTmp( AXml IN CLOB);

    PROCEDURE ConvertHitlistTempsToPerm(Ahitlistid IN NUMBER, ADuplicateAction IN CHAR, ADescription IN VARCHAR2, AUserID IN VARCHAR2, ALogID OUT NUMBER, AMessage OUT NOCOPY CLOB, ARegistration IN CHAR := 'Y', ARegNumGeneration IN CHAR := 'Y', AConfigurationID IN NUMBER := 1, ASectionsList IN VARCHAR2 := NULL);
    PROCEDURE LogBulkregistrationId(ALogID OUT NUMBER, ADuplicateAction IN Varchar2, AUserID IN Varchar2, ADescription IN Varchar2 DEFAULT NULL);
    PROCEDURE LogBulkregistration(ALogID IN NUMBER, LATempID IN VARCHAR2, AAction IN char, RegNumber IN VARCHAR2, BatchID IN NUMBER, Result IN VARCHAR2);

    --Utility
    PROCEDURE InsertLog(ALogProcedure CLOB, ALogComment CLOB);

    --Constants
    Debuging                       Constant BOOLEAN:=False;  -- Must always be False in PerForce

    eGenericException              Constant Number:=-20000;
    eNoRowsReturned                Constant Number:=-20020;

    eRetrieveSingleCompoundReg     Constant Number:=-20002;

    eCreateMultiCompoundRegistry   Constant Number:=-20009;
    eRetrieveMultiCompoundRegistry Constant Number:=-20010;
    eUpdateMultiCompoundRegistry   Constant Number:=-20011;
    eDeleteMultiCompoundRegistry   Constant Number:=-20012;
    eRetrieveMultiCompoundRegList  Constant Number:=-20023;
    eInsertData                    Constant Number:=-20029;

    eCreateMultiCompoundRegTmp     Constant Number:=-20013;
    eRetrieveMultiCompoundRegTmp   Constant Number:=-20014;
    eUpdateMultiCompoundRegTmp     Constant Number:=-20015;
    eDeleteMultiCompoundRegTmp     Constant Number:=-20016;

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
    --eFragmentNotMatchIdentityFalse Constant Number:=-20036;
    eMoreComponentsTarget          Constant Number:=-20037; --Components don't match between registries. The batch was not moved.
    eMoreComponentsSource          Constant Number:=-20038; --Components don't match between registries. The batch was not moved.
    
    eOnlyOneBatchDeleting          Constant Number:=-20039; 
    

END CompoundRegistry;
/