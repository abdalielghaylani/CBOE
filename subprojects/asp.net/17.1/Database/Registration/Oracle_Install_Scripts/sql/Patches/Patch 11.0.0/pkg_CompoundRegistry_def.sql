prompt 
prompt Starting "pkg_CompoundRegistry_def.sql"...
prompt 

CREATE OR REPLACE PACKAGE REGDB.CompoundRegistry AS

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
    ******************************************************************************/

    FUNCTION ValidateCompoundMulti(AStructure CLOB, ARegIDToValidate Number:=NULL, AConfigurationID Number:=1, AXMLCompound XmlType, AXMLFragmentEquivalent XmlType, AXMLRegNumberDuplicatedHidden OUT NOCOPY XmlType) RETURN CLOB;

    PROCEDURE CreateMultiCompoundRegistry(AXml IN CLOB, ARegNumber out NOCOPY VW_RegistryNumber.RegNumber%type, AMessage OUT CLOB, ADuplicateCheck Char:='C', ARegNumGeneration IN CHAR := 'Y', AConfigurationID Number:=1);
    PROCEDURE RetrieveMultiCompoundRegistry(ARegNumber  in VW_RegistryNumber.RegNumber%type, AXml out NOCOPY clob,ASectionsList in Varchar2:='');
    PROCEDURE UpdateMultiCompoundRegistry(AXml in CLOB, AMessage OUT CLOB, ADuplicateCheck Char:='C', AConfigurationID Number:=1);
    PROCEDURE DeleteMultiCompoundRegistry(ARegNumber in VW_RegistryNumber.RegNumber%type);
    PROCEDURE RetrieveMultiCompoundRegList(AXmlRegNumbers in clob, AXmlCompoundList out NOCOPY clob);
    PROCEDURE CanCreateMultiCompoundRegistry(AXml IN CLOB, AMessage OUT CLOB, ADuplicateCheck Char:='C', AConfigurationID Number:=1);

    PROCEDURE CreateMultiCompoundRegTmp(AXml in CLOB,  ATempID out Number);
    PROCEDURE RetrieveMultiCompoundRegTmp( ATempID  in Number, AXml out NOCOPY clob);
    PROCEDURE UpdateMultiCompoundRegTmp( AXml in clob);
    PROCEDURE DeleteMultiCompoundRegTmp( ATempID  in Number);
    FUNCTION  GetFragmentXML( ATempCompundID  in Number) RETURN CLOB;

    PROCEDURE MoveBatch(ABatchID in Number, ARegNumber in VW_RegistryNumber.RegNumber%type);
    PROCEDURE DeleteBatch (ABatchID Number);

    PROCEDURE RetrieveCompoundRegistryList(AXmlRegNumbers in clob, AXmlCompoundList out NOCOPY clob);

    PROCEDURE LoadMultiCompoundRegRecord(ARegistryXml IN CLOB, ADuplicateAction IN CHAR, AAction OUT NOCOPY CHAR, ARegNumber OUT NOCOPY VW_RegistryNumber.RegNumber%TYPE, ARegistration IN CHAR := 'Y', ARegNumGeneration IN CHAR := 'Y', AConfigurationID IN Number := 1);
    PROCEDURE LoadMultiCompoundRegRecordList(ARegistryListXml In CLOB, ADuplicateAction IN CHAR, ARegistration IN CHAR, ARegNumGeneration IN CHAR, AConfigurationID IN Number, LogID IN NUMBER);
    PROCEDURE RetrieveTempIDList(Ahitlistid IN Number, Aid OUT NOCOPY CLOB);
    PROCEDURE RetrieveBatch( AID IN NUMBER, AXml OUT NOCOPY CLOB);
    PROCEDURE UpdateBatch( AXml IN CLOB);
    PROCEDURE RetrieveBatchTmp( ATempID IN NUMBER, AXml OUT NOCOPY CLOB);
    PROCEDURE UpdateBatchTmp( AXml IN CLOB);

    PROCEDURE InsertLog(ALogProcedure CLOB,ALogComment CLOB);

    PROCEDURE LogBULKREGITRATIONID(ALOG_ID OUT NUMBER, ADUPLICATE_ACTION IN Varchar2, AUSER_ID IN Varchar2);
    PROCEDURE LogBULKREGISTRATION(LogID IN NUMBER, LATempID IN VARCHAR2, AAction IN char, RegNumber IN VARCHAR2, BatchID IN NUMBER, Result IN VARCHAR2);

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

    eSetSessionParameter           Constant Number:=-20025;

    eSetColumnsForXmlInsert        Constant Number:=-20026;
    eSameIdentityBetweenBatches    Constant Number:=-20027;

    eWildcardValidation            Constant Number:=-20028;

    eOnlyOneBatch                  Constant Number:=-20030;
    eInvalidRegNum                 Constant Number:=-20031;

END CompoundRegistry;
/