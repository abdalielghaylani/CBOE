prompt 
prompt Starting "pkg_CompoundRegistry_body.sql"...
prompt

CREATE OR REPLACE PACKAGE BODY "COMPOUNDREGISTRY" IS

PROCEDURE InsertLog(ALogProcedure CLOB,ALogComment CLOB) IS
    PRAGMA AUTONOMOUS_TRANSACTION;
BEGIN
    INSERT INTO LOG(LogProcedure,LogComment) VALUES($$plsql_unit||'.'||ALogProcedure,ALogComment);
    COMMIT;
EXCEPTION
    WHEN OTHERS THEN NULL; --If logs don't work then don't stop
END;

PROCEDURE SetSessionParameter IS
    PRAGMA AUTONOMOUS_TRANSACTION;
BEGIN
    DBMS_SESSION.set_nls('NLS_DATE_FORMAT','''YYYY-MM-DD HH:Mi:SS''');
    DBMS_SESSION.set_nls('NLS_NUMERIC_CHARACTERS', '''.,''');
    COMMIT; --It is necesary to finished the Autonomou-Transaction
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eSetSessionParameter, DBMS_UTILITY.FORMAT_ERROR_STACK);
    END;
END;

PROCEDURE SaveRegNumberDuplicatedHidden(ARegNumber VARCHAR2, AXMLRegNumberDuplicatedHidden XmlType)IS
BEGIN
    $if CompoundRegistry.Debuging $then InsertLog('SaveRegNumberDuplicatedHidden','ARegNumber->'||ARegNumber); $end null;

    $if CompoundRegistry.Debuging $then InsertLog('SaveRegNumberDuplicatedHidden','AXMLRegNumberDuplicatedHidden->'||AXMLRegNumberDuplicatedHidden.GetStringVal()); $end null;

    INSERT INTO VW_Duplicates(RegNumber ,RegNumberDuplicated,PersonID,Created)
        (SELECT ARegNumber,extract(value(RegNumberDuplicated), '//REGNUMBER/text()').GetStringVal(),P.Person_ID,SYSDATE
         FROM Table(XMLSequence(Extract(AXMLRegNumberDuplicatedHidden, '/ROWSET/REGNUMBER'))) RegNumberDuplicated, People P
         WHERE sys_context('userenv','session_user')=P.User_ID(+));
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eSetSessionParameter, DBMS_UTILITY.FORMAT_ERROR_STACK);
    END;
END;

PROCEDURE AddTags(AXmlSource IN XmlType,AXmlTarget IN OUT NOCOPY XmlType,APathSource IN Varchar2,AAttributeSource IN Varchar2) IS

    LIndex               Number;
    LNodesCount          Number;
    LIndexTarget         Number;
    LTagTarget           Varchar2(4000);
    LNodeName            Varchar2(300);
    LNodePath            Varchar2(4000);

    LDOMDocumentSource   DBMS_XMLDom.DOMDocument;
    LElementSource       DBMS_XMLDom.DOMElement;
    LSourceNode          DBMS_XMLDom.DOMNode;
    LSourceParentNode    DBMS_XMLDom.DOMNode;
    LSourceParentNodeAux DBMS_XMLDom.DOMNode;

    LNodeListSource      DBMS_XMLDom.DOMNodelist;

    LNodeListTarget      DBMS_XMLDom.DOMNodelist;

    LDOMDocumentTarget   DBMS_XMLDom.DOMDocument;
    LNodeTarget          DBMS_XMLDom.DOMNode;

    LAttrs               DBMS_XMLDom.DOMNamedNodeMap;
    LAttr                DBMS_XMLDom.DOMNode;
    LIndexAttr           Number;
    LXPath               Varchar2(1000);
    LAttrName            Varchar2(300);

BEGIN
    LDOMDocumentTarget := DBMS_XMLDom.NewDOMDocument(AXmlTarget);
    LDOMDocumentSource := DBMS_XMLDom.NewDOMDocument(AXmlSource);

    LNodeListSource:=DBMS_XMLDom.GetElementsByTagName(LDOMDocumentSource, APathSource);
    LNodesCount:=DBMS_XMLDom.GetLength(LNodeListSource);
    FOR LIndex IN 0..LNodesCount-1 LOOP
        LNodeTarget       := DBMS_XMLDom.MakeNode(LDOMDocumentTarget);
        LSourceNode       := DBMS_XMLDom.Item(LNodeListSource, LIndex);
        LSourceParentNode := DBMS_XMLDom.GetParentNode(LSourceNode);
        LNodePath:='';
        LOOP
            LNodeName:=DBMS_XMLDom.GetNodeName(LSourceParentNode);
            EXIT WHEN DBMS_XMLDom.ISNULL(LSourceParentNode) OR UPPER(LNodeName)=UPPER('#document');
            LNodePath         := '/'||LNodeName||LNodePath;
            LSourceParentNode := DBMS_XMLDom.GetParentNode(LSourceParentNode);
        END LOOP;
        LSourceParentNode := DBMS_XMLDom.GetParentNode(LSourceNode);
        LXPath:=LNodePath;
        IF AAttributeSource IS NOT NULL THEN
            LAttrs := dbms_xmldom.getattributes(LSourceParentNode);
            FOR LIndexAttr IN 0 .. dbms_xmldom.getLength(LAttrs) - 1 LOOP
                LAttr  := dbms_xmldom.item(LAttrs, LIndexAttr);
                LAttrName :=dbms_xmldom.getNodeName(LAttr);
                IF UPPER(LAttrName)=UPPER(AAttributeSource) THEN
                    LXPath:=LXPath||'[@'||LAttrName||'="'||dbms_xmldom.getNodeValue(LAttr)||'"]';
                    EXIT;
                END IF;
            END LOOP;
        END IF;

        LNodeListTarget:=DBMS_XSLProcessor.SelectNodes(LNodeTarget,LXPath);

        FOR LIndexTarget IN 0..DBMS_XMLDom.GetLength(LNodeListTarget) - 1 LOOP
            LNodeTarget := DBMS_XMLDom.Item(LNodeListTarget, LIndexTarget);
            LSourceNode := DBMS_XMLDom.ImportNode(LDOMDocumentTarget,LSourceNode,TRUE);
            LSourceNode := DBMS_XMLDom.AppendChild(LNodeTarget,LSourceNode);
        END LOOP;
    END LOOP;
END;


FUNCTION ValidateCompoundFragment(ACompoundID VW_Compound.CompoundID%Type, AXMLCompound XmlType,AXMLFragmentEquivalent XmlType) RETURN CLOB IS
    LqryCtx                DBMS_XMLGEN.ctxHandle;
    LFragmentsIdsValue     VARCHAR2(4000);
    LQuery                 VARCHAR2(4000);
    LFragmentCount         INTEGER;
    LPosOld                INTEGER;
    LPos                   INTEGER;
    LSameFragment          VARCHAR2(4000);
    LSameEquivalent        VARCHAR2(4000);
    LResult                VARCHAR2(4000);

BEGIN
    --FragmentsID
    SELECT XmlTransform(extract(AXMLCompound,'/Component/Compound/FragmentList/Fragment/FragmentID'),XmlType.CreateXml('
          <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
            <xsl:template match="/FragmentID">
                  <xsl:for-each select=".">
                      <xsl:value-of select="."/>,</xsl:for-each>
            </xsl:template>
          </xsl:stylesheet>')).GetClobVal()
      INTO LFragmentsIdsValue
      FROM dual;

    IF LFragmentsIdsValue IS NOT NULL THEN

        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment','LFragmentsIdsValue->' ||LFragmentsIdsValue); $end null;

        LFragmentsIdsValue := SUBSTR(LFragmentsIdsValue, 1, Length(LFragmentsIdsValue) - 1);

        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'LFragmentsIdsValue->' || LFragmentsIdsValue); $end null;

        LFragmentCount := 1;
        LPosOld        := 0;
        LOOP
            LPos := NVL(INSTR(LFragmentsIdsValue, ',', LPosOld + 1), 0);
            $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', ' LFragmentsIdsValue->' || LFragmentsIdsValue || ' LPos->' || LPos || ' LPosOld->' || LPosOld); $end null;

            EXIT WHEN LPos = 0;
            LPosOld        := LPos;
            LFragmentCount := LFragmentCount + 1;
        END LOOP;

        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'LFragmentCount->' || LFragmentCount); $end null;

        LQuery := '
            SELECT 1
                FROM VW_Compound_Fragment CF,VW_BatchComponentFragment BCF, VW_BatchComponent BC
                WHERE CF.CompoundID=''' || ACompoundID ||
                ''' AND CF.ID=BCF.CompoundFragmentID AND BCF.BatchComponentID=BC.ID AND
                      CF.FragmentID IN (' ||
                LFragmentsIdsValue || ')
                GROUP BY CF.CompoundID
                HAVING COUNT(1)=' || LFragmentCount ||
                ' AND ' || LFragmentCount ||
                '=(SELECT Count(1) FROM VW_Compound_Fragment CF1,VW_BatchComponentFragment BCF1, VW_BatchComponent BC1
                WHERE CF.CompoundID=CF1.CompoundID AND CF1.ID=BCF1.CompoundFragmentID AND BCF1.BatchComponentID=BC1.ID)';

        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'LQuery->' || LQuery); $end null;

        LQryCtx := DBMS_XMLGEN.newContext(LQuery);
        DBMS_XMLGEN.setMaxRows(LqryCtx, 3);
        DBMS_XMLGEN.setRowTag(LqryCtx, '');
        LSameFragment := DBMS_XMLGEN.getXML(LqryCtx);
        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'LSameFragment->' || LSameFragment); $end null;
        DBMS_XMLGEN.closeContext(LqryCtx);

        IF LSameFragment IS NOT NULL THEN

            --Equivalent

            LQuery := 'SELECT 1 FROM
                (SELECT FragmentID,Equivalent
                    FROM VW_Compound_Fragment CF,VW_BatchComponentFragment BCF, VW_BatchComponent BC
                    WHERE CF.CompoundID=''' || ACompoundID ||
                  ''' AND CF.ID=BCF.CompoundFragmentID AND BCF.BatchComponentID=BC.ID AND
                          CF.FragmentID IN (' ||
                  LFragmentsIdsValue || ')
                    ) Fragments WHERE Equivalent<>ExtractValue(Xmltype(''' ||
                  AXMLFragmentEquivalent.GetClobVal ||
                  '''),''/BatchComponentFragmentList/BatchComponentFragment[FragmentID=''||FragmentID||'']/Equivalents'')';

            $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'Equivalent LQuery->' || LQuery); $end null;

            LQryCtx := DBMS_XMLGEN.newContext(LQuery);
            DBMS_XMLGEN.setMaxRows(LqryCtx, 3);
            DBMS_XMLGEN.setRowTag(LqryCtx, '');
            LSameEquivalent := DBMS_XMLGEN.getXML(LqryCtx);
            $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'LSameEquivalent->' || LSameEquivalent); $end null;
            DBMS_XMLGEN.closeContext(LqryCtx);

            IF LSameEquivalent IS NOT NULL THEN
                LResult := 'SAMEFRAGMENT="True" SAMEEQUIVALENT="False"';
            ELSE
                LResult := 'SAMEFRAGMENT="True" SAMEEQUIVALENT="True"';
            END IF;
        ELSE
            LResult := 'SAMEFRAGMENT="False" SAMEEQUIVALENT="False"';
        END IF;
    ELSE
        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment','LFragmentsIdsValue->' || LFragmentsIdsValue); $end null;

        LQuery := '
            SELECT 1
                FROM VW_Compound_Fragment CF,VW_BatchComponentFragment BCF, VW_BatchComponent BC
                WHERE CF.CompoundID=''' || ACompoundID ||
                ''' AND CF.ID=BCF.CompoundFragmentID AND BCF.BatchComponentID=BC.ID';

        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'LQuery->' || LQuery); $end null;

        LQryCtx := DBMS_XMLGEN.newContext(LQuery);
        DBMS_XMLGEN.setMaxRows(LqryCtx, 3);
        DBMS_XMLGEN.setRowTag(LqryCtx, '');
        LSameFragment := DBMS_XMLGEN.getXML(LqryCtx);
        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundFragment', 'LSameFragment->' || LSameFragment); $end null;
        DBMS_XMLGEN.closeContext(LqryCtx);

        IF LSameFragment IS NULL THEN
            LResult := 'SAMEFRAGMENT="True" SAMEEQUIVALENT="True"';
        ELSE
            LResult := 'SAMEFRAGMENT="False" SAMEEQUIVALENT="False"';
        END IF;
    END IF;

    RETURN LResult;

END;


FUNCTION ValidateCompoundMulti(AStructure CLOB, ARegIDToValidate Number:=NULL, AConfigurationID Number:=1, AXMLCompound XmlType, AXMLFragmentEquivalent XmlType,AXMLRegNumberDuplicatedHidden OUT NOCOPY XmlType) RETURN CLOB IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    LqryCtx              DBMS_XMLGEN.ctxHandle;
    LResult              CLOB;
    LResultXML           CLOB;
    LDuplicateCount      Number;
    LParameters          Varchar2(1000);
    LResultSerealized    Varchar2(4000);

    LCompoundID          VW_Compound.CompoundID%Type;
    LRegNumber           VW_RegistryNumber.RegNumber%Type;
    LCount               NUMBER := 0;

    LFormulaWeight       VW_TEMPORARYCOMPOUND.FormulaWeight%Type;
    LPosTag              INTEGER;

    LFragmentsData       VARCHAR2(4000);

    LRLSState            Boolean;
    LResultXMLType       XMLType;

    CURSOR C_RegNumbers(ACoumpoundID in VW_Compound.CompoundID%type) IS
        SELECT RegNumber
            FROM VW_RegistryNumber RN,VW_Mixture M,VW_Mixture_Component MC
            WHERE RN.RegID=M.RegID AND M.MixtureID=MC.MixtureID AND MC.CompoundID=ACoumpoundID
            ORDER BY MC.CompoundID;

BEGIN
    BEGIN
        SELECT CSCartridge.MolWeight(AStructure)
            INTO LFormulaWeight
            FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
            LFormulaWeight:=0;
        END;
    END;

    IF LFormulaWeight!= 0 THEN
        --Duplicate Validation

        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','AStructureXML to validate->'||AStructure); $end null;

        INSERT INTO CsCartridge.TempQueries (Query, Id) VALUES (AStructure, 0);

        SELECT XmlTransform(Xml,XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/ROWSET/ROW">
                  <xsl:for-each select="node()">
                    <xsl:variable name="V1" select="."/>
                    <xsl:choose>
                      <xsl:when test="$V1 != ''''">
                        <xsl:value-of select="name()"/>=<xsl:value-of select="$V1"/>,</xsl:when>
                    </xsl:choose>
                  </xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')).GetStringVal()
        INTO LParameters
        FROM VW_CONFIGURATION
        WHERE ID=AConfigurationID;

        LParameters:=SUBSTR(LParameters,1,LENGTH(LParameters)-1);

        LRLSState:=RegistrationRLS.GetStateRLS;
        IF LRLSState THEN
            RegistrationRLS.SetEnableRLS(False);
        END IF;

        IF NVL(ARegIDToValidate,0)<>0 THEN
            LQryCtx := DBMS_XMLGEN.newContext('
              SELECT C.COMPOUNDID
                  FROM VW_Compound C, VW_Structure S
                  WHERE C.RegID<>'||ARegIDToValidate||' AND C.StructureId = S.StructureId AND
                        CsCartridge.MoleculeContains(S.Structure, ''select query from cscartridge.tempqueries where id = 0'','''','''||LParameters||''') = 1
                  ORDER BY C.RegID');
        ELSE
            LQryCtx := DBMS_XMLGEN.newContext('
                SELECT /*+ ORDERED FULL(C) INDEX(S MX)*/ C.COMPOUNDID
                    FROM  VW_Compound C, VW_Structure S
                    WHERE C.StructureId = S.StructureId AND
                          CsCartridge.MoleculeContains(S.Structure, ''select query from cscartridge.tempqueries where id = 0'','''','''||LParameters||''') = 1
                    ORDER BY C.RegID');
        END IF;


        DBMS_XMLGEN.setMaxRows(LqryCtx,30);
        DBMS_XMLGEN.setRowTag(LqryCtx, '');
        LResultXML := DBMS_XMLGEN.getXML(LqryCtx);
        DBMS_XMLGEN.closeContext(LqryCtx);

        IF LRLSState THEN
            RegistrationRLS.SetEnableRLS(LRLSState);
        END IF;

        LResult:=replace(LResultXML,chr(10),'');
        COMMIT;

        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','LDuplicateCount->'||LDuplicateCount||' LResult->'||LResult); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','AStructureXML to validate->'||AStructure); $end null;

        IF LRLSState AND LResultXML IS NOT NULL THEN
            $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','1ºLResultXML->'||LResultXML); $end null;
            $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','LResult->'||LResult); $end null;

            SELECT XmlTransform(XmlType.CreateXml(LResultXML),XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/ROWSET">
                  <xsl:for-each select="node()">
                    <xsl:variable name="V1" select="."/>
                    <xsl:choose>
                      <xsl:when test="$V1 != ''''">
                        _REPLACE_<xsl:value-of select="$V1"/>_REPLACE_,</xsl:when>
                    </xsl:choose>
                  </xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')).GetClobVal()
                INTO LResultSerealized
                FROM DUAL;
              $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','1º LResultSerealized->'||LResultSerealized); $end null;
            LResultSerealized:=SUBSTR(LResultSerealized,1,LENGTH(LResultSerealized)-1);
            LResultSerealized:=REPLACE(LResultSerealized,'_REPLACE_','''');
            $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','2º LResultSerealized->'||LResultSerealized); $end null;

            $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','LQryCtx->'||'
                SELECT extract(value(RegNumberDuplicatedHidden), ''//COMPOUNDID/text()'').getStringVal() COMPOUNDID FROM Table(XMLSequence(Extract(XmlType.CreateXml('''||LResult||'''), ''/ROWSET/COMPOUNDID''))) RegNumberDuplicatedHidden
                MINUS
                SELECT C.CompoundID
                    FROM  VW_Compound C, VW_Structure S
                    WHERE C.StructureId = S.StructureId AND C.CompoundID IN ('||LResultSerealized||')
                ORDER BY 1'); $end null;

            LQryCtx := DBMS_XMLGEN.newContext('
                SELECT extract(value(RegNumberDuplicatedHidden), ''//COMPOUNDID/text()'').getStringVal() REGNUMBER FROM Table(XMLSequence(Extract(XmlType.CreateXml('''||LResult||'''), ''/ROWSET/COMPOUNDID''))) RegNumberDuplicatedHidden
                MINUS
                SELECT C.CompoundID
                    FROM  VW_Compound C, VW_Structure S
                    WHERE C.StructureId = S.StructureId AND C.CompoundID IN ('||LResultSerealized||')
                ORDER BY 1');

            DBMS_XMLGEN.setMaxRows(LqryCtx,30);
            DBMS_XMLGEN.setRowTag(LqryCtx, '');
            LResultXML := DBMS_XMLGEN.getXML(LqryCtx);
            $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','2ºLResultXML->'||LResultXML); $end null;
            DBMS_XMLGEN.closeContext(LqryCtx);
            IF LResultXML IS NOT NULL THEN
                AXMLRegNumberDuplicatedHidden:=XmlType.CreateXml(LResultXML);
                $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','AXMLRegNumberDuplicatedHidden->'||AXMLRegNumberDuplicatedHidden.GetClobVal); $end null;
            END IF;
            LQryCtx := DBMS_XMLGEN.newContext('
                SELECT extract(value(RegNumberDuplicatedHidden), ''//COMPOUNDID/text()'').getStringVal() COMPOUNDID FROM Table(XMLSequence(Extract(XmlType.CreateXml('''||LResult||'''), ''/ROWSET/COMPOUNDID''))) RegNumberDuplicatedHidden
                MINUS
                SELECT extract(value(RegNumberDuplicated), ''//COMPOUNDID/text()'').getStringVal() COMPOUNDID FROM Table(XMLSequence(Extract(XmlType.CreateXml('''||LResultXML||'''), ''/ROWSET/COMPOUNDID''))) RegNumberDuplicated
                ORDER BY 1');

            DBMS_XMLGEN.setMaxRows(LqryCtx,30);
            DBMS_XMLGEN.setRowTag(LqryCtx, '');
            LResult := DBMS_XMLGEN.getXML(LqryCtx);
            DBMS_XMLGEN.closeContext(LqryCtx);
            LResult:=replace(LResult,chr(10),'');
            $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','2ºLResult->'||LResult); $end null;

        END IF;

        LPosTag:=1;
        LDuplicateCount:=0;
        LOOP
            LPosTag := INSTR(LResult,'<COMPOUNDID',LPosTag+13);
            $if CompoundRegistry.Debuging $then InsertLog('ValidateCompoundMulti','LPosTag='||LPosTag); $end null;
        EXIT WHEN (LPosTag=0 or LPosTag is null);
            LDuplicateCount:=LDuplicateCount+1;
        END LOOP;

        --LDuplicateCount:=DBMS_XMLGEN.getNumRowsProcessed(LqryCtx); --did not work

        IF LDuplicateCount>0 THEN

            LResultXMLType:=XmlType.CreateXML(LResult);
            LResult:='<REGISTRYLIST>';

            FOR LIndexAux IN 1..LDuplicateCount LOOP
                SELECT extractvalue(LResultXMLType,'node()/node()['||LIndexAux||']')
                    INTO LCompoundID
                    FROM dual;

                IF LCompoundID IS NOT NULL THEN
                    SELECT COUNT (*)
                        INTO LCount
                        FROM VW_Mixture_Component MC
                        WHERE MC.CompoundID = LCompoundID
                            AND (SELECT COUNT (*)
                                FROM VW_Mixture m, VW_Mixture_Component mcc
                                WHERE M.MixtureID = MC.MixtureID AND M.MixtureID = MCC.MixtureID) = 1;

                    LFragmentsData:=ValidateCompoundFragment(LCompoundID,AXMLCompound,AXMLFragmentEquivalent);

                    FOR R_RegNumbers IN C_RegNumbers(LCompoundID) LOOP
                        LResult := LResult||'<REGNUMBER count="'||LCount||'" CompoundID="'||LCompoundID||'" '||LFragmentsData||'>'||R_RegNumbers.RegNumber||'</REGNUMBER>';
                    END LOOP;
                END IF;
            END LOOP;
            LResult:=LResult||'</REGISTRYLIST>';

            RETURN LResult;
        ELSE
            RETURN '';
        END IF;
    ELSE
        RETURN '';
    END IF;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eCompoundValidation, 'Error validating the compound.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
    END;
    RETURN '';
END;


FUNCTION TakeOffAndGetClob(AXml IN OUT NOCOPY Clob,ABeginTag VARCHAR2) RETURN CLOB IS
    LValue           CLOB;
    LTagBegin        Number;
    LTagEnd          Number;
    LEndTag          VARCHAR2(255);
    LBeginTag        VARCHAR2(255);
BEGIN

    LBeginTag:='<'||ABeginTag||'>';
    LEndTag:='</'||ABeginTag||'>';

    LTagBegin:=INSTR(AXml,LBeginTag);
    LTagEnd:=INSTR(AXml,LEndTag);

    $if CompoundRegistry.Debuging $then InsertLog('TakeOffAndGetClob','LTagBegin->'||LTagBegin||' LTagEnd->'||LTagEnd); $end null;

    IF (LTagBegin<>0) AND (LTagEnd<>0) THEN
        LValue:=SUBSTR(AXml,LTagBegin+LENGTH(LBeginTag),LTagEnd-LTagBegin-LENGTH(LBeginTag));
        AXml:=SUBSTR(AXml,1,LTagBegin-1)||SUBSTR(AXml,LTagEnd+LENGTH(LEndTag),LENGTH(AXml));

        $if CompoundRegistry.Debuging $then InsertLog('TakeOffAndGetClob','LValue->'); $end null;

    ELSE
        LValue:='';
    END IF;

    RETURN LValue;
END;

-- Helper procedure to trim the element name
-- Returns the element name ignoring any provided '<', '>', or spaces
FUNCTION TrimElement(ATag VARCHAR2) RETURN VARCHAR2 IS
    LTag  VARCHAR2(255);
BEGIN
    -- Get the element name ignoring any provided '<', '>', or spaces
    LTag := Trim(ATag);
    LTag := Ltrim(Ltrim(LTag, '<'));
    LTag := Rtrim(Rtrim(LTag, '>'));
    RETURN LTag;
END;

-- Helper procedure for locating tags in XML
-- ALft and ARht will be the positions of the '<' and '>' respectively. Both are 0 if the element is not found.
FUNCTION LocateElement(AXml IN CLOB, ATag VARCHAR2, ALft OUT NUMBER, ARht OUT NUMBER, AStart IN NUMBER := 1) RETURN VARCHAR2 IS
    LTag  VARCHAR2(255);
    LTemp VARCHAR2(255);
    LLft  NUMBER;
BEGIN
    -- Initialize return values
    ALft := 0;
    ARht := 0;
    -- Get the element name ignoring any provided '<', '>', or spaces
    LTag := TrimElement(ATag);
    -- Locate the left end of the element
    LLft := AStart;
    LOOP
        LLft := NVL(Instr(AXml, '<' || LTag, LLft), 0);
        IF LLft = 0 THEN
          RETURN NULL; -- Unable to locate the tag
        END IF;
        LTemp := Substr(AXml, LLft, 1 + Length(LTag) + 1);
        EXIT WHEN (LTemp = '<' || LTag || '>');
        EXIT WHEN (LTemp = '<' || LTag || ' ');
        LLft := LLft + 1;
    END LOOP;
    -- Locate the right end of the element
    ARht := NVL(Instr(AXml, '>', LLft), 0);
    IF ARht = 0 THEN
        RETURN NULL; -- Unable to locate the matching '>' (should not happen)
    END IF;
    ALft := LLft; -- Located the right end so now we can safely return the left end
    RETURN Substr(AXml, ALft, ARht + 1 - ALft);
END;

-- Helper procedure for locating a matching end element
-- This is not trivial if elements with the same tag name are nested
-- AStart must point to the '<' of the begin element
FUNCTION LocateMatchingElement(AXml IN CLOB, AStart IN NUMBER, ALft OUT NUMBER, ARht OUT NUMBER) RETURN VARCHAR2 IS
    LLft          NUMBER;
    LDepth        NUMBER := 0;
    LTag          VARCHAR2(255);
    LBegin        VARCHAR2(255);
    LBeginLft     NUMBER;
    LBeginRht     NUMBER;
    LEnd          VARCHAR2(255);
    LEndLft       NUMBER;
    LEndRht       NUMBER;
BEGIN
    -- Initialize return values
    ALft := 0;
    ARht := 0;
    -- Get begin element
    LBeginLft := AStart;
    LBeginRht := NVL(Instr(AXml, '>', LBeginLft), 0);
    IF LBeginRht = 0 THEN
        RETURN NULL; -- Unable to locate the matching '>' (should not happen)
    END IF;
    LBegin := Substr(AXml, LBeginLft, LBeginRht + 1 - LBeginLft);
    -- Initial end
    LEnd := '';
    LEndLft := LBeginRht;
    LEndRht := LBeginRht;
    -- Trim begin element to tag
    LTag := LBegin;
    LTag := Ltrim(LTag, '<');
    LTag := Rtrim(LTag, '>');
    LTag := Substr(LTag, NVL(Instr(LTag, ' '), Length(LTag) + 1) - 1);
    -- Set initial depth
    IF Instr(LBegin, Length(LBegin) - 1, 1) != '/' THEN
        LDepth := LDepth + 1; -- Increase for begin element
    END IF;
    LOOP
        EXIT WHEN LDepth = 0; -- Balancing match has been found
        -- Error if previous begin element was not valid
        IF LBeginLft = 0 OR LBeginLft > LEndLft THEN
            RETURN NULL;
        END IF;
        -- Locate the next end element
        LEnd := LocateElement(AXml, '/' || LTag, LEndLft, LEndRht, LEndRht + 1);
        IF LEndLft = 0 THEN
            RETURN NULL;  -- Could not locate balancing end tag (should not happen)
        END IF;
        LDepth := LDepth - 1; -- Decrease for end element
        -- Locate the next begin element before end element that affects level
        LOOP
            LBegin := LocateElement(AXml, LTag, LBeginLft, LBeginRht, LBeginRht + 1);
            EXIT WHEN LBeginLft = 0;  -- No more begin elements
            EXIT WHEN LBeginLft > LEndLft;  -- No more begin lemenets before end elememt
            IF Instr(LBegin, Length(LBegin) - 1, 1) != '/' THEN
              LDepth := LDepth + 1; -- Increase for begin element
              EXIT WHEN TRUE;
            END IF;
        END LOOP;
    END LOOP;
    ALft := LEndLft;
    ARht := LEndRht;
    RETURN Substr(AXml, ALft, ARht + 1 - ALft);
END;

FUNCTION TakeOffAndGetClobsList(AXml IN OUT NOCOPY Clob, ABeginTag VARCHAR2,ABeginTagParent VARCHAR2:=NULL,ABeginTagGranParent VARCHAR2:=NULL,ASourceDeleteTagName Boolean:=TRUE, AUpdateVerify Boolean:=FALSE) RETURN CLOB IS
    LReturn       CLOB := '';
    LSearchPos    NUMBER;
    LElement      VARCHAR2(255);
    LElementLft   NUMBER;
    LElementRht   NUMBER;
    LBegin        VARCHAR2(255);
    LBeginLft     NUMBER;
    LBeginRht     NUMBER;
    LEnd          VARCHAR2(255);
    LEndLft       NUMBER;
    LEndRht       NUMBER;
    LSaveInnerXML Boolean;
BEGIN
    LSearchPos := 1;
    LOOP
        -- Locate grandparent if any
        IF ABeginTagGranParent IS NOT NULL THEN
            LElement := LocateElement(AXml, ABeginTagGranParent, LElementLft, LElementRht, LSearchPos);
            EXIT WHEN LElementLft = 0; -- Unable to locate the grandparent
            EXIT WHEN Substr(LElement, Length(LElement) - 1, 1) = '/'; -- Grandparent is empty
            LSearchPos := LElementRht + 1; -- Establish the new search position
        END IF;
        -- Locate parent if any
        IF ABeginTagParent IS NOT NULL THEN
            LElement := LocateElement(AXml, ABeginTagParent, LElementLft, LElementRht, LSearchPos);
            EXIT WHEN LElementLft = 0; -- Unable to locate the parent
            EXIT WHEN Substr(LElement, Length(LElement) - 1, 1) = '/'; -- Parent is empty
            LSearchPos := LElementRht + 1; -- Establish the new search position
        END IF;
        -- Locate tag
        LElement := LocateElement(AXml, ABeginTag, LElementLft, LElementRht, LSearchPos);
        EXIT WHEN LElementLft = 0; -- Unable to locate the tag
        LBegin := LElement;
        LBeginLft := LElementLft;
        LBeginRht := LElementRht;
        LSearchPos := LElementRht + 1; -- Establish the new search position
        IF Substr(LElement, Length(LElement) - 1, 1) != '/' THEN
            -- WJC a LOOP must be added in the future to handle the case of nested element of the same name !!!
            LElement := LocateElement(AXml, '/' || TrimElement(ABeginTag), LElementLft, LElementRht, LSearchPos);
            EXIT WHEN LElementLft = 0; -- Unable to locate the end tag (should not happen)
            LEnd := LElement;
            LEndLft := LElementLft;
            LEndRht := LElementRht;
        ELSE
            -- The element is empty so we make an empty end tag
            LEnd := '';
            LEndLft := LBeginRht;
            LEndRht := LEndLft;
        END IF;

        -- Decide if we are saving the inner XML
        IF AUpdateVerify THEN
            $if CompoundRegistry.Debuging $then InsertLog('TakeOffAndGetClobsList',' LBeginElement='||LBegin); $end null;
            IF Instr(Upper(LBegin), 'UPDATE="YES"') = 0 AND
                Instr(Upper(LBegin), 'UPDATE=''YES''') = 0 AND
                Instr(Upper(LBegin), 'INSERT="YES"') = 0 AND
                Instr(Upper(LBegin), 'INSERT=''YES''') = 0 THEN
                LSaveInnerXML := FALSE; -- Not Update="yes" or Insert="yes"
            ELSE
                IF Instr(Upper(LBegin), 'DELETE="YES"') = 0 AND
                   Instr(Upper(LBegin), 'DELETE=''YES''') = 0 THEN
                  LSaveInnerXML := TRUE;  -- Update="yes" or Insert="yes" but not Delete="yes"
                ELSE
                  LSaveInnerXML := FALSE; -- Update="yes" or Insert="yes" and Delete="yes"
                END IF;
            END IF;
        ELSE
            LSaveInnerXML := TRUE;
        END IF;

        -- Save or discard the inner XML
        IF LSaveInnerXML THEN
            LReturn := LReturn || '<Clob>' || Substr(AXml, (LBeginRht + 1), (LEndLft + 1) - 1 - (LBeginRht + 1)) || '</Clob>';
        ELSE
            LReturn := LReturn || '<Clob></Clob>';
        END IF;

        LSearchPos := LElementRht + 1; -- Establish the new search position
        -- Temporarily convert LSearchPos to be relative to the end of the string
        LSearchPos := Length(AXml) - LSearchPos;
        -- Remove inner XML or outer XML depending on ASourceDeleteTagName
        -- If inner XML is removed then substitute placeholder
        IF ASourceDeleteTagName THEN
            -- Remove outer XML
            AXml := Substr(AXml, 1, LBeginLft - 1) || Substr(AXml, LEndRht + 1, Length(AXml));
        ELSE
            -- Remove inner XML
            AXml := Substr(AXml, 1, LBeginRht) || '(Removed' || TrimElement(ABeginTag) || ')' || Substr(AXml, LEndLft, Length(AXml));
        END IF;
        -- Convert LSearchPos back to relative to the start of the string
        LSearchPos := Length(AXml) - LSearchPos;

    END LOOP;
    LReturn := '<ClobList>' || LReturn || '</CLobList>';
    RETURN LReturn;
END;

FUNCTION TakeOnAndGetXml(AXml IN Clob,ATagName VARCHAR,AStructuresList IN OUT NOCOPY Clob) RETURN Clob IS
    LValue             CLOB;
    LStrcutureValue    CLOB;
    LStructuresList    CLOB;
    LTagBegin          Number;
    LValueStr          Varchar2(255);
    LStrcutureTagBegin Number;
BEGIN
    LValue:=AXml;
    LStructuresList:=AStructuresList;
    LValueStr:='(Removed'||ATagName||')';
    LOOP
        LTagBegin:=INSTR(LValue,LValueStr);
    EXIT WHEN (LTagBegin=0);
        LStrcutureValue:=TakeOffAndGetClob(LStructuresList,'Clob');

        LStrcutureTagBegin:=INSTR(LValue,LValueStr);
        IF LStrcutureTagBegin<>0 THEN
            LValue:=SUBSTR(LValue,1,LStrcutureTagBegin-1)||LStrcutureValue||SUBSTR(LValue,LStrcutureTagBegin+LENGTH(LValueStr),LENGTH(LValue));
        END IF;
    END LOOP;
    RETURN LValue;
END;

FUNCTION GetRegNumber(ASequenceID in VW_SEQUENCE.SequenceId%Type, ARootNumber out NOCOPY VW_REGISTRYNUMBER.RootNumber%Type,ASequenceNumber out VW_REGISTRYNUMBER.SequenceNumber%Type) RETURN VW_REGISTRYNUMBER.RegNumber%Type IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    LRegDelimiter VW_SEQUENCE.RegDelimiter%Type;
    LRegNumber VW_REGISTRYNUMBER.RegNumber%Type;
    LSaltName VARCHAR2(100):=''; --"Salt didn't implement yet.
BEGIN
    $if CompoundRegistry.Debuging $then InsertLog('GetRegNumber','ASequenceID'||'->'||ASequenceID); $end null;
    SELECT Prefix||PrefixDelimiter||lpad(NVL(NextInSequence,1),RegNumberLength,'0'),RegDelimiter,NVL(NextInSequence,1)
        INTO ARootNumber,LRegDelimiter,ASequenceNumber
        FROM VW_SEQUENCE
        WHERE SequenceID=ASequenceID
        FOR UPDATE;
    $if CompoundRegistry.Debuging $then InsertLog('GetRegNumber','ARootNumber'||'->'||ARootNumber); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('GetRegNumber','LRegDelimiter'||'->'||LRegDelimiter); $end null;
    UPDATE VW_SEQUENCE SET NextInSequence=NVL(NextInSequence,1)+1;
    COMMIT;
    IF LSaltName IS NOT NULL THEN
        RETURN   ARootNumber||LRegDelimiter||LSaltName;
    ELSE
        RETURN   ARootNumber;
    END IF;
END;

PROCEDURE InsertData(ATableName IN CLOB,AXmlRows IN CLOB,AStructureValue IN CLOB,AStructureAggregationValue IN CLOB,AFragmentXmlValue IN CLOB,AStructureID IN Number,AMixtureID IN Number,AMessage IN OUT NOCOPY CLOB,ARowsInserted IN OUT Number) IS
    LinsCtx       DBMS_XMLSTORE.ctxType;
BEGIN
   --Create the Table Context
    LinsCtx := DBMS_XMLSTORE.newContext(ATableName);
    DBMS_XMLSTORE.clearUpdateColumnList(LinsCtx);

    $if CompoundRegistry.Debuging $then InsertLog('InsertData','ATableName->'||ATableName||' AXmlRows->'||AXmlRows); $end null;

    --Insert Rows and get count it inserted
    ARowsInserted := DBMS_XMLSTORE.insertXML(LinsCtx, AXmlRows);

    --Build Message Logs
    AMessage := AMessage || ' ' || cast(ARowsInserted as string) || ' Row/s Inserted on "' || ATableName || '".' || CHR(13);

    $if CompoundRegistry.Debuging $then InsertLog('InsertData','Message->'||AMessage); $end null;

    --Close the Table Context
    DBMS_XMLSTORE.closeContext(LinsCtx);
    CASE UPPER(ATableName)
        WHEN 'VW_STRUCTURE' THEN
            BEGIN
                IF AStructureValue IS NOT NULL THEN
                    $if CompoundRegistry.Debuging $then InsertLog('InsertData',' LStructureID:'||AStructureID||' LStructureValue:'||AStructureValue); $end null;
                    UPDATE VW_STRUCTURE
                        SET STRUCTURE=AStructureValue
                        WHERE STRUCTUREID=AStructureID;
                END IF;
            END;
        WHEN 'VW_MIXTURE' THEN
            BEGIN
                IF AStructureAggregationValue IS NOT NULL THEN
                   $if CompoundRegistry.Debuging $then InsertLog('InsertData',' LMixtureID:'||AMixtureID||' LStructureAggregationValue:'||AStructureAggregationValue); $end null;
                    UPDATE VW_MIXTURE
                        SET StructureAggregation=AStructureAggregationValue
                        WHERE MixtureID=AMixtureID;
                END IF;
            END;
        WHEN 'VW_FRAGMENT' THEN
            BEGIN
                IF UPPER(ATableName)='VW_FRAGMENT' AND AFragmentXmlValue IS NOT NULL THEN
                    $if CompoundRegistry.Debuging $then InsertLog('InsertData',' LStructureID:'||AStructureID||' LFragmentXmlValue:'||AFragmentXmlValue); $end null;
                    UPDATE VW_STRUCTURE
                        SET STRUCTURE=AFragmentXmlValue
                        WHERE StructureID=AStructureID;
                END IF;
            END;
        ELSE NULL;
    END CASE;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        $if CompoundRegistry.Debuging $then InsertLog('InsertData','Error inseting registry.'||chr(10)||AMessage||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
        RAISE_APPLICATION_ERROR(eInsertData, 'Error inseting registry.' || chr(13) || DBMS_UTILITY.FORMAT_ERROR_STACK);
    END;
END;

PROCEDURE DeleteCompound(ACompoundIDToDelete IN Number, AMictureID IN Number,AMessage IN OUT NOCOPY Varchar2) IS
    LCountMixture Number;
    LCountStructure Number;
    LStructureID Number;
BEGIN
    DELETE VW_BatchComponentFragment WHERE BatchComponentID IN (SELECT ID FROM VW_BatchComponent WHERE MixtureComponentID IN (SELECT MixtureComponentID FROM VW_Mixture_Component WHERE CompoundID=ACompoundIDToDelete and MixtureID=AMictureID));
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_BatchComponentFragment".';
    DELETE VW_BatchComponent WHERE MixtureComponentID IN (SELECT MixtureComponentID FROM VW_Mixture_Component WHERE CompoundID=ACompoundIDToDelete and MixtureID=AMictureID);
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_BatchComponent".';
    DELETE VW_Mixture_Component WHERE MixtureID=AMictureID AND CompoundID=ACompoundIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Mixture_Component".';
    /* We will not use this next code but it works fine
    --Delete the compound
    SELECT  NVL(Count(1),0)
        INTO  LCountMixture
        FROM  VW_Mixture_Component
        WHERE CompoundID=ACompoundIDToDelete;
    $if CompoundRegistry.Debuging $then InsertLog('DeleteCompound','ACompoundIDToDelete'||ACompoundIDToDelete||' LCountMixture:'||LCountMixture); $end null;
    IF LCountMixture=0 THEN
        DELETE VW_Fragment WHERE FragMentID IN ( SELECT FragmentID FROM VW_Compound_Fragment WHERE CompoundID=ACompoundIDToDelete);
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Fragment".';
        DELETE VW_Compound_Fragment WHERE CompoundID=ACompoundIDToDelete;
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound_Fragment".';


        BEGIN
            SELECT StructureID
                INTO LStructureID
                FROM VW_Compound
                WHERE  CompoundID=ACompoundIDToDelete;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                LStructureID:=0;
        END;
        DELETE VW_Compound WHERE CompoundID=ACompoundIDToDelete;
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound".';
        $if CompoundRegistry.Debuging $then InsertLog('DeleteCompound',' DELETE VW_Compound WHERE:'||SQL%ROWCOUNT); $end null;
        IF LStructureID<>0 and LStructureID<>-1 THEN
            SELECT  NVL(Count(1),0)
                INTO  LCountStructure
                FROM  VW_Compound
                WHERE CompoundID<>ACompoundIDToDelete AND StructureID=LStructureID;

            SELECT  NVL(Count(1),0)+LCountStructure
                INTO  LCountStructure
                FROM  VW_Fragment
                WHERE StructureID=LStructureID;
            IF LCountMixture=0 THEN
                DELETE VW_Structure WHERE StructureID=LStructureID;
                AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Structure".';
            END IF;
        END IF;
    END IF;*/
END;

PROCEDURE DeleteFragment(ACompoundfragmentIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
    LCountMixture Number;
    LCountStructure Number;
    LStructureID Number;
BEGIN
    DELETE VW_BatchComponentFragment WHERE CompoundFragmentID=ACompoundfragmentIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_BatchComponentFragment".';
    DELETE VW_Compound_Fragment WHERE ID=ACompoundfragmentIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound_Fragment ".';
END;

FUNCTION ValidateMixture(ARegIDs IN CLOB, ADuplicateCount OUT Number, AMixtureID IN Varchar2:='0',ACompoundIdsValueDeleting IN Varchar2:=NULL,AXmlTables XmlType) RETURN CLOB IS
    LResult                CLOB;
    LComponentCount        NUMBER;
    LPos                   NUMBER;
    LPosOld                NUMBER;
    LQuery                 Varchar2(3000);
    LValidation            Varchar2(1000);
    LSameFragment          Varchar2(1000);
    LSameEquivalent        Varchar2(1000);
    LRegNumber             VW_RegistryNumber.RegNumber%type;

    LXMLCompound           XmlType;
    LXMLFragmentEquivalent XmlType;
    LRegID            Number;

    TYPE CursorType        IS REF CURSOR;
    C_RegNumbers           CursorType;

BEGIN

  --Duplicate Validation

   $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture','Mixture ARegIDs to validate->'||ARegIDs||' ACompoundIdsValueDeleting->'||ACompoundIdsValueDeleting); $end null;

    LComponentCount:=1;
    LPosOld:=0;
    LOOP
        LPos:=NVL(INSTR(ARegIDs,',',LPosOld+1),0);
        $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture',' ARegIDs->'||ARegIDs||' LPos->'||LPos||' LPosOld->'||LPosOld); $end null;

    EXIT WHEN LPos=0;
        LPosOld:=LPos;
        LComponentCount:=LComponentCount+1;
    END LOOP;

    LQuery:='
       SELECT R.RegNumber
          FROM VW_Mixture M,VW_Mixture_Component MC,VW_Compound C,VW_RegistryNumber R
          WHERE  R.RegID=M.RegID AND M.MixtureID=MC.MixtureID AND C.CompoundID=MC.CompoundID AND '||'M.MixtureID<>'||AMixtureID||' AND C.RegID IN ('||ARegIDs||') ';
    IF ACompoundIdsValueDeleting IS NOT NULL THEN
        LQuery:=LQuery||' AND C.CompoundID NOT IN ('||ACompoundIdsValueDeleting||') ';
    END IF;
    LQuery:=LQuery||' GROUP BY M.RegID, M.MixtureID,R.RegNumber
          HAVING COUNT(1) ='||LComponentCount||' AND (SELECT COUNT(1) FROM VW_Mixture_Component MC WHERE MC.MIXTUREID=M.MIXTUREID)='||LComponentCount;

    $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture','LQuery->'||LQuery); $end null;

    OPEN C_RegNumbers FOR LQuery;
    FETCH C_RegNumbers INTO LRegNumber;
    $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture','C_RegNumbers%ROWCOUNT->'||C_RegNumbers%ROWCOUNT); $end null;

    IF  NOT C_RegNumbers%NOTFOUND THEN
        LSameFragment:='SAMEFRAGMENT="True"';
        LSameEquivalent:='SAMEEQUIVALENT="True"';

        LPosOld:=0;
        LOOP
            LPos:=NVL(INSTR(ARegIDs,',',LPosOld+1),0);

            IF LPos=0 THEN
                LRegID:=DBMS_LOB.SUBSTR(ARegIDs,Length(ARegIDs),LPosOld+1);
            ELSE
                LRegID:=DBMS_LOB.SUBSTR(ARegIDs,LPos-LPosOld-1,LPosOld+1);
            END IF;
            LPosOld:=LPos;
            $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture','LRegID->'||LRegID||' ARegIDs->'||ARegIDs); $end null;

            SELECT extract(AXmlTables,'/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[CompoundID=/MultiCompoundRegistryRecord/ComponentList/Component/Compound[RegNumber/RegID='||LRegID||']/CompoundID]/BatchComponentFragmentList')
                INTO LXMLFragmentEquivalent
                FROM dual;

             $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture','LXMLFragmentEquivalent->'||LXMLFragmentEquivalent.Getclobval()); $end null;

            SELECT extract(AXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[Compound/RegNumber/RegID='||LRegID||']'),extractValue(AXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component/Compound[RegNumber/RegID='||LRegID||']/CompoundID')
              INTO LXMLCompound,LRegID
              FROM dual;

             $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture','LXMLCompound->'||LXMLCompound.Getclobval()); $end null;
             $if CompoundRegistry.Debuging $then InsertLog('ValidateMixture','LRegID->'||LRegID); $end null;

            LValidation:=ValidateCompoundFragment(LRegID,LXMLCompound,LXMLFragmentEquivalent);
            IF INSTR(LValidation,'SAMEFRAGMENT="False"')<>0 THEN
                LSameFragment:='SAMEFRAGMENT="False"';
            END IF;
            IF INSTR(LValidation,'SAMEEQUIVALENT="False"')<>0 THEN
                LSameEquivalent:='SAMEEQUIVALENT="False"';
            END IF;
            EXIT WHEN LPos=0;
        END LOOP;

        LResult:='<REGISTRYLIST>';

        LOOP
            LResult:=LResult||'<REGNUMBER '||LSameFragment||' '||LSameEquivalent||'>'||LRegNumber||'</REGNUMBER>';
            FETCH C_RegNumbers INTO LRegNumber;
            EXIT WHEN C_RegNumbers%NOTFOUND;
        END LOOP;
        CLOSE C_RegNumbers;

        LResult:=LResult||'</REGISTRYLIST>';

    ELSE
        IF C_RegNumbers%ISOPEN THEN
            CLOSE C_RegNumbers;
        END IF;
        LResult:='';
    END IF;

    RETURN LResult;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        IF C_RegNumbers%ISOPEN THEN
            CLOSE C_RegNumbers;
        END IF;
        RAISE_APPLICATION_ERROR(eCompoundValidation, 'Error validating the mixture.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
    END;
    RETURN '';
END;

PROCEDURE ValidateIdentityBetweenBatches(AXmlTables IN XmlType) IS
    LIndex  Number;
    LIndex1 Number;
    LComponentIndex Number;
    LFragmentsIdsValue Varchar2(4000);
    LFragmentsIdsValueLast Varchar2(4000);
BEGIN
    $if CompoundRegistry.Debuging $then InsertLog('ValidateIdentityBetweenBatches','AXmlTables ->'||AXmlTables.getclobval()); $end null;
    IF COEDB.ConfigurationManager.RetrieveParameter('Registration','SameBatchesIdentity')='True' THEN
        LIndex:=0;
        LOOP
            LIndex:=LIndex+1;
            $if CompoundRegistry.Debuging $then InsertLog('ValidateIdentityBetweenBatches','LIndex ->'||LIndex); $end null;

            SELECT extractValue(AXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component['||LIndex||']/ComponentIndex')
              INTO LComponentIndex
              FROM dual;

            $if CompoundRegistry.Debuging $then InsertLog('ValidateIdentityBetweenBatches', 'LComponentIndex ->' || LComponentIndex); $end null;

            LFragmentsIdsValueLast:=NULL;
        EXIT WHEN LComponentIndex IS NULL;

            LIndex1:=0;
            LOOP
                LIndex1:=LIndex1+1;
                $if CompoundRegistry.Debuging $then InsertLog('ValidateIdentityBetweenBatches','LIndex1 ->'||LIndex1); $end null;

                SELECT XmlTransform(extract(AXmlTables,'/MultiCompoundRegistryRecord/BatchList/Batch['||LIndex1||']/BatchComponentList/BatchComponent[ComponentIndex='||LComponentIndex||']/BatchComponentFragmentList/BatchComponentFragment/FragmentID'),XmlType.CreateXml('
                  <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                    <xsl:template match="/FragmentID">
                          <xsl:for-each select=".">
                              <xsl:value-of select="."/>,</xsl:for-each>
                    </xsl:template>
                  </xsl:stylesheet>')).GetClobVal()
                    INTO LFragmentsIdsValue
                    FROM dual;

                 $if CompoundRegistry.Debuging $then InsertLog('ValidateIdentityBetweenBatches','LFragmentsIdsValue->'||LFragmentsIdsValue); $end null;

                EXIT WHEN LFragmentsIdsValue IS NULL;

                IF LFragmentsIdsValueLast IS NOT NULL THEN
                    IF LFragmentsIdsValueLast<>LFragmentsIdsValue THEN
                        RAISE_APPLICATION_ERROR(eSameIdentityBetweenBatches, 'Error validating the compound '||ABS(LComponentIndex)||'. The comopund should have the same identity of fragments between batches. (The "SameBatchesIdentity" flag is set in "true")');
                    END IF;
                END IF;

                LFragmentsIdsValueLast:=LFragmentsIdsValue;

            END LOOP;

        END LOOP;

    END IF;
END;

FUNCTION ValidateFragment(ARegIDs IN CLOB, ADuplicateCount OUT Number, AMixtureID IN Varchar2:='0',ACompoundIdsValueDeleting IN Varchar2:=NULL) RETURN CLOB IS
        LqryCtx DBMS_XMLGEN.ctxHandle;
        LResult CLOB;
        LComponentCount NUMBER;
        LPos NUMBER;
        LPosOld NUMBER;
        LQuery Varchar2(3000);
BEGIN

  --Duplicate Validation

   $if CompoundRegistry.Debuging $then InsertLog('ValidateFragment','Mixture ARegIDs to validate->'||ARegIDs||' ACompoundIdsValueDeleting->'||ACompoundIdsValueDeleting); $end null;

    LComponentCount:=1;
    LPosOld:=0;
    LOOP
      LPos:=NVL(INSTR(ARegIDs,',',LPosOld+1),0);
      $if CompoundRegistry.Debuging $then InsertLog('ValidateFragment',' ARegIDs->'||ARegIDs||' LPos->'||LPos||' LPosOld->'||LPosOld); $end null;

    EXIT WHEN LPos=0;
      LPosOld:=LPos;
      LComponentCount:=LComponentCount+1;
    END LOOP;

    LQuery:='
       SELECT R.RegNumber
          FROM VW_Mixture M,VW_Mixture_Component MC,VW_Compound C,VW_RegistryNumber R
          WHERE  R.RegID=M.RegID AND M.MixtureID=MC.MixtureID AND C.CompoundID=MC.CompoundID AND '||'M.MixtureID<>'||AMixtureID||' AND C.RegID IN ('||ARegIDs||') ';
    IF ACompoundIdsValueDeleting IS NOT NULL THEN
        LQuery:=LQuery||' AND C.CompoundID NOT IN ('||ACompoundIdsValueDeleting||') ';
    END IF;
    LQuery:=LQuery||' GROUP BY M.RegID, M.MixtureID,R.RegNumber
          HAVING COUNT(1) ='||LComponentCount||' AND (SELECT COUNT(1) FROM VW_Mixture_Component MC WHERE MC.MIXTUREID=M.MIXTUREID)='||LComponentCount;

    $if CompoundRegistry.Debuging $then InsertLog('ValidateFragment','LQuery->'||LQuery); $end null;

    LQryCtx := DBMS_XMLGEN.newContext(LQuery);
    DBMS_XMLGEN.setMaxRows(LqryCtx,3);
    DBMS_XMLGEN.setRowTag(LqryCtx, '');
    LResult := DBMS_XMLGEN.getXML(LqryCtx);
    ADuplicateCount:=DBMS_XMLGEN.getNumRowsProcessed(LqryCtx);
    $if CompoundRegistry.Debuging $then InsertLog('ValidateFragment','Mixture LDuplicateCount->'||ADuplicateCount||' LResult->'||LResult); $end null;
    DBMS_XMLGEN.closeContext(LqryCtx);
    IF ADuplicateCount>0 THEN
        LResult:=replace(LResult,'<?xml version="1.0"?>','');
        LResult:=replace(LResult,'ROWSET','REGISTRYLIST');
        RETURN LResult;
    ELSE
        RETURN '';
    END IF;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eCompoundValidation, 'Error validating the fragment.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
    END;
    RETURN '';
END;

FUNCTION  ValidateWildcardStructure(AStructureValue CLOB) RETURN Boolean IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    LInternalID Integer;
BEGIN

    INSERT INTO CsCartridge.TempQueries (Query, Id) VALUES (AStructureValue, 0);

    SELECT  CPD_Internal_ID
        INTO LInternalID
        FROM  Structures
        WHERE cscartridge.moleculecontains(base64_cdx,'SELECT Query FROM CSCartridge.TempQueries WHERE ID = 0', '', 'IDENTITY=YES') =1 AND
              cpd_internal_id = -1;

     $if CompoundRegistry.Debuging $then InsertLog('ValidateWildcardStructure','LInternalID= '||LInternalID); $end null;

    COMMIT;

    IF LInternalID = -1 THEN
        RETURN False;
    ELSE
        RETURN True;
    END IF;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        BEGIN
            COMMIT;
            RETURN True;
        END;
    WHEN OTHERS THEN
        BEGIN
            COMMIT;
            $if CompoundRegistry.Debuging $then InsertLog('ValidateWildcardStructure','Error validating Wildcard Structure.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
            RAISE_APPLICATION_ERROR(eWildcardValidation, 'Error validating Wildcard Structure.'||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
END;

PROCEDURE CanCreateMultiCompoundRegistry(AXml IN CLOB, AMessage OUT CLOB, ADuplicateCheck Char:='C', AConfigurationID Number:=1) AS
        /*
            Autor: Fari
            Date: 17-Mar-09
            Object: Verify if a Registry is available to be inserted.
            Description: The process calls to the CreateMultiCompoundRegistry procedure and it only validates.
            Choice of ADuplicateCheck:
                ADuplicateCheck='C'  --> Structure Validation, Mixture Validation
                ADuplicateCheck='M'  --> Mixture Validation
            Choice of AMessage:
                AMessage=NULL        --> Validation OK, there isn't duplicated
                AMessage IS NOT NULL --> Validation failed, AMessage gets the duplicated
        */
    LRegNumber VW_RegistryNumber.RegNumber%type;
BEGIN

    IF Upper(ADuplicateCheck)='C' THEN
        CreateMultiCompoundRegistry(AXml, LRegNumber, AMessage, 'V');
    ELSIF Upper(ADuplicateCheck)='M' THEN
        CreateMultiCompoundRegistry(AXml, LRegNumber, AMessage, 'L');
    END IF;

END;

PROCEDURE CreateMultiCompoundRegistry(AXml IN CLOB, ARegNumber OUT NOCOPY VW_RegistryNumber.RegNumber%type, AMessage OUT CLOB, ADuplicateCheck Char:='C', ARegNumGeneration IN CHAR := 'Y', AConfigurationID Number:=1) IS
       /*
            Autor: Fari
            Date:07-Mar-07
            Object: Insert a new Registry
            Description: Look over a Xml searching each Table and insert the rows on it.
            Choice of ADuplicateCheck:
                ADuplicateCheck='C' --> Structure Validation, Mixture Validation and Registry Insert
                ADuplicateCheck='M' --> Mixture Validation and Registry Insert
                ADuplicateCheck='V' --> Structure Validation, Mixture Validation
                ADuplicateCheck='L' --> Mixture Validation
                ADuplicateCheck='N' or others  --> either Validation, Registry Insert, option to duplicate
            Choice of AMessage:
                AMessage=NULL        --> Validation OK, there isn't duplicated
                AMessage IS NOT NULL --> Validation failed, AMessage gets the duplicated
        */

    LXmlTables                XmlType;
    LXslTablesTransformed     XmlType;
    LXmlCompReg               CLOB;
    LXmlRows                  CLOB;
    LXmlTypeRows              XmlType;
    LXmlSequenceType          XmlSequenceType;

    LIndex                    Number:=0;
    LRowsInserted             Number:=0;
    LTableName                CLOB;
    LMessage                  CLOB:='';
    LDuplicate                Varchar2(100);

    LStructureValue           CLOB;
    LRegNumberRegID           Number:=0;
    LDuplicatedCompoundID     Number;
    LDuplicatedStructures     CLOB;
    LListDulicatesCompound    CLOB;
    LDuplicateComponentCount  Number:=0;

    LStructuresList            CLOB;
    LStructuresToValidateList  CLOB;
    LFragmentXmlValue          CLOB;
    LFragmentXmlList           CLOB;
    LNormalizedStructureList   CLOB;
    LStructureAggregationList  CLOB;
    LStructureAggregationValue CLOB;

    LXMLCompound                  XmlType;
    LXMLFragmentEquivalent        XmlType;
    LXMLRegNumberDuplicatedHidden XmlType;

    LRegDBIdsValue         Varchar2(4000);
    LDuplicatedMixtureCount   Number;
    LDuplicatedMixtureRegIds  Varchar2(4000);
    LDuplicatedAuxStructureID Number:=0;

    LRegID                      Number:=0;
    LBatchID                    Number:=0;
    LCompoundID                 Number:=0;
    LFragmentID                 Number:=0;
    LStructureID                Number:=0;
    LMixtureID                  Number:=0;
    LBatchNumber                Number:=0;
    LMixtureComponentID         Number:=0;
    LBatchComponentID           Number:=0;
    LCompoundFragmentID         Number:=0;
    LRegNumber                  VW_REGISTRYNUMBER.RegNumber%Type;
    LRootNumber                 VW_REGISTRYNUMBER.RootNumber%Type;
    LSequenceNumber             VW_REGISTRYNUMBER.SequenceNumber%Type;

    LSequenceID                   Number:=0;
    LProcessingMixture            Varchar2(1);

    LRegIDAux                      Number:=0;
    LExistentRegID                 Number:=0;
    LExistentComponentIndex        Number:=0;


    LXslTables XmlType:=XmlType.CreateXml('<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/MultiCompoundRegistryRecord">
        <xsl:for-each select="RegNumber">
            <VW_RegistryNumber>
                <ROW>
                    <REGID>0</REGID>
                    <ROOTNUMBER>" "</ROOTNUMBER>
                    <SEQUENCENUMBER>
                        <xsl:value-of select="SequenceNumber"/>
                    </SEQUENCENUMBER>
                    <REGNUMBER>" "</REGNUMBER>
                    <SEQUENCEID>
                        <xsl:value-of select="SequenceID"/>
                    </SEQUENCEID>
                </ROW>
            </VW_RegistryNumber>
        </xsl:for-each>
        <VW_Mixture>
            <ROW>
                <MIXTUREID>0</MIXTUREID>
                <REGID>0</REGID>
                <CREATED>
                    <xsl:value-of select="DateCreated"/>
                </CREATED>
                <PERSONCREATED>
                    <xsl:value-of select="PersonCreated"/>
                </PERSONCREATED>
                <MODIFIED>
                    <xsl:value-of select="DateLastModified"/>
                </MODIFIED>
                <xsl:for-each select="PropertyList/Property">
                    <xsl:variable name="V1" select="."/>
                    <xsl:for-each select="@name">
                        <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:for-each>
                </xsl:for-each>
            </ROW>
        </VW_Mixture>
        <xsl:for-each select="ProjectList/Project">
            <xsl:if test="ProjectID!=''''">
                <VW_RegistryNumber_Project>
                    <ROW>
                        <PROJECTID><xsl:value-of select="ProjectID"/></PROJECTID>
                        <REGID>0</REGID>
                        <ORDERINDEX><xsl:value-of select="OrderIndex"/></ORDERINDEX>
                    </ROW>
                </VW_RegistryNumber_Project>
            </xsl:if>
        </xsl:for-each>
        <xsl:for-each select="IdentifierList/Identifier">
            <VW_Compound_Identifier>
                <ROW>
                    <ID>0</ID>
                    <TYPE>
                        <xsl:value-of select="IdentifierID"/>
                    </TYPE>
                    <VALUE>
                        <xsl:value-of select="Value"/>
                    </VALUE>
                    <REGID>0</REGID>
                    <ORDERINDEX>
                        <xsl:value-of select="OrderIndex"/>
                    </ORDERINDEX>
                </ROW>
            </VW_Compound_Identifier>
        </xsl:for-each>
        <xsl:for-each select="BatchList/Batch">
            <VW_Batch>
                <ROW>
                    <BATCHID>0</BATCHID>
                    <BATCHNUMBER>0</BATCHNUMBER>
                    <DATECREATED>
                        <xsl:value-of select="DateCreated"/>
                    </DATECREATED>
                    <PERSONCREATED>
                        <xsl:value-of select="PersonCreated"/>
                    </PERSONCREATED>
                    <PERSONREGISTERED>
                        <xsl:value-of select="PersonRegistered"/>
                    </PERSONREGISTERED>
                    <DATELASTMODIFIED>
                        <xsl:value-of select="DateLastModified"/>
                    </DATELASTMODIFIED>
                    <REGID>0</REGID>
                    <TEMPBATCHID>
                        <xsl:value-of select="TempBatchID"/>
                    </TEMPBATCHID>
                    <xsl:for-each select="PropertyList/Property">
                        <xsl:variable name="V1" select="."/>
                        <xsl:for-each select="@name">
                            <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                            <xsl:choose>
                                <xsl:when test="$V2 = ''DELIVERYDATE'' and $V1 != ''''">
        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:when>
                                <xsl:when test="$V2 = ''DATEENTERED'' and $V1 != ''''">
        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:when>
                                <xsl:otherwise>
        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:otherwise>
                            </xsl:choose>
                        </xsl:for-each>
                    </xsl:for-each>
                </ROW>
            </VW_Batch>
            <xsl:for-each select="ProjectList/Project">
                <VW_Batch_Project>
                    <ROW>
                        <xsl:for-each select="ProjectID">
                            <PROJECTID>
                                <xsl:value-of select="."/>
                            </PROJECTID>
                            <BATCHID>0</BATCHID>
                        </xsl:for-each>
                    </ROW>
                </VW_Batch_Project>
             </xsl:for-each>
        </xsl:for-each>
        <xsl:for-each select="ComponentList/Component">
            <xsl:variable name="VComponentIndex" select="ComponentIndex/."/>
            <xsl:for-each select="Compound">
                <xsl:for-each select="RegNumber">
                    <VW_RegistryNumber>
                        <ROW>
                            <REGID>
                                <xsl:value-of select="RegID"/>
                            </REGID>
                            <ROOTNUMBER>" "</ROOTNUMBER>
                            <SEQUENCENUMBER>
                                <xsl:value-of select="SequenceNumber"/>
                            </SEQUENCENUMBER>
                            <REGNUMBER>" "</REGNUMBER>
                            <SEQUENCEID>
                                <xsl:value-of select="SequenceID"/>
                            </SEQUENCEID>
                        </ROW>
                    </VW_RegistryNumber>
                </xsl:for-each>
                <xsl:choose>
                    <xsl:when test="RegNumber/RegID=''0''">
                        <xsl:for-each select="BaseFragment/Structure">
                            <VW_Structure>
                                <ROW>
                                    <STRUCTUREID>0</STRUCTUREID>
                                    <STRUCTUREFORMAT>
                                        <xsl:value-of select="StructureFormat"/>
                                    </STRUCTUREFORMAT>
                                </ROW>
                            </VW_Structure>
                        </xsl:for-each>
                        <VW_Compound>
                            <ROW>
                                <COMPOUNDID>0</COMPOUNDID>
                                <DATECREATED>
                                    <xsl:value-of select="DateCreated"/>
                                </DATECREATED>
                                <PERSONCREATED>
                                    <xsl:value-of select="PersonCreated"/>
                                </PERSONCREATED>
                                <PERSONREGISTERED>
                                    <xsl:value-of select="PersonRegistered"/>
                                </PERSONREGISTERED>
                                <DATELASTMODIFIED>
                                    <xsl:value-of select="DateLastModified"/>
                                </DATELASTMODIFIED>
                                <REGID>0</REGID>
                                <STRUCTUREID>0</STRUCTUREID>
                                <xsl:for-each select="BaseFragment/Structure/Structure">
                                    <MOLECULARFORMULA>
                                        <xsl:value-of select="@formula"/>
                                    </MOLECULARFORMULA>
                                    <FORMULAWEIGHT>
                                        <xsl:value-of select="@molWeight"/>
                                    </FORMULAWEIGHT>
                                </xsl:for-each>
                                <xsl:for-each select="PropertyList/Property">
                                    <xsl:variable name="V1" select="."/>
                                    <xsl:for-each select="@name">
                                        <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                       LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:for-each>
                                </xsl:for-each>
                            </ROW>
                        </VW_Compound>
                        <VW_Mixture_Component>
                          <ROW>
                            <MIXTURECOMPONENTID>0</MIXTURECOMPONENTID>
                            <MIXTUREID>0</MIXTUREID>
                            <COMPOUNDID>0</COMPOUNDID>
                          </ROW>
                        </VW_Mixture_Component>
                        <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex=$VComponentIndex]">
                            <VW_BatchComponent>
                              <ROW>
                               <ID>0</ID>
                                 <BATCHID>0</BATCHID>
                                 <MIXTURECOMPONENTID>0</MIXTURECOMPONENTID>
                                 <ORDERINDEX><xsl:value-of select="OrderIndex"/></ORDERINDEX>
                                 <xsl:for-each select="PropertyList/Property">
                                    <xsl:variable name="V1" select="."/>
                                    <xsl:for-each select="@name">
                                        <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                          LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;
                                     </xsl:for-each>
                                </xsl:for-each>
                              </ROW>
                            </VW_BatchComponent>
                        </xsl:for-each>
                        <xsl:for-each select="Project/ProjectList">
                            <VW_RegistryNumber_Project>
                                <ROW>
                                    <xsl:for-each select="ProjectID">
                                        <PROJECTID>
                                            <xsl:value-of select="."/>
                                        </PROJECTID>
                                        <REGID>0</REGID>
                                    </xsl:for-each>
                                </ROW>
                            </VW_RegistryNumber_Project>
                        </xsl:for-each>
                        <xsl:for-each select="FragmentList/Fragment">
                            <xsl:variable name="VFragmentID" select="FragmentID"/>
                            <VW_Fragment>
                                <ROW>
                                    <FRAGMENTID>
                                        <xsl:value-of select="FragmentID"/>
                                    </FRAGMENTID>
                                </ROW>
                            </VW_Fragment>
                            <VW_Compound_Fragment>
                                <ROW>
                                    <ID>0</ID>
                                    <COMPOUNDID>0</COMPOUNDID>
                                    <FRAGMENTID>0</FRAGMENTID>
                                    <EQUIVALENTS>
                                        <xsl:value-of select="Equivalents"/>
                                    </EQUIVALENTS>
                                </ROW>
                            </VW_Compound_Fragment>

                            <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex=$VComponentIndex]">
                                <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment[FragmentID=$VFragmentID]">
                                  <VW_BatchComponentFragment>
                                    <ROW>
                                        <BATCHCOMPONENTID>0</BATCHCOMPONENTID>
                                        <COMPOUNDFRAGMENTID>0</COMPOUNDFRAGMENTID>
                                        <EQUIVALENT>
                                            <xsl:value-of select="Equivalents"/>
                                        </EQUIVALENT>
                                        <ORDERINDEX><xsl:value-of select="OrderIndex"/></ORDERINDEX>
                                    </ROW>
                                  </VW_BatchComponentFragment>
                                </xsl:for-each>
                            </xsl:for-each>
                        </xsl:for-each>
                        <xsl:for-each select="IdentifierList/Identifier">
                            <VW_Compound_Identifier>
                                <ROW>
                                    <ID>0</ID>
                                    <TYPE>
                                        <xsl:value-of select="Type"/>
                                    </TYPE>
                                    <VALUE>
                                        <xsl:value-of select="Value"/>
                                    </VALUE>
                                    <REGID>0</REGID>
                                </ROW>
                            </VW_Compound_Identifier>
                        </xsl:for-each>
                    </xsl:when>
                    <xsl:when test="RegNumber/RegID!=''0''">
                        <VW_Mixture_Component>
                            <ROW>
                                <MIXTURECOMPONENTID>0</MIXTURECOMPONENTID>
                                <MIXTUREID>0</MIXTUREID>
                                <COMPOUNDID>0</COMPOUNDID>
                            </ROW>
                        </VW_Mixture_Component>
                        <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex=$VComponentIndex]">
                            <VW_BatchComponent>
                              <ROW>
                               <ID>0</ID>
                                 <BATCHID>0</BATCHID>
                                 <MIXTURECOMPONENTID>0</MIXTURECOMPONENTID>
                                 <ORDERINDEX><xsl:value-of select="OrderIndex"/></ORDERINDEX>
                                 <xsl:for-each select="PropertyList/Property">
                                    <xsl:variable name="V1" select="."/>
                                    <xsl:for-each select="@name">
                                        <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                          LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;
                                     </xsl:for-each>
                                </xsl:for-each>
                              </ROW>
                            </VW_BatchComponent>
                        </xsl:for-each>
                    </xsl:when>
                </xsl:choose>
            </xsl:for-each>
        </xsl:for-each>
    </xsl:template>
</xsl:stylesheet>
');

BEGIN
    SetSessionParameter;

    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','Begin AXml= '||AXml); $end null;
    LXmlCompReg:=AXml;

    -- Take Out the Structures because XmlType don't suport > 64k.
    LFragmentXmlList:=TakeOffAndGetClobsList(LXmlCompReg,'<Structure ','Structure','<Fragment>');
    LStructuresList:=TakeOffAndGetClobsList(LXmlCompReg,'<Structure ','Structure','<BaseFragment>',NULL,FALSE);
    LStructuresToValidateList:=LStructuresList;
    LNormalizedStructureList:=TakeOffAndGetClobslist(LXmlCompReg,'<NormalizedStructure','<Structure>');
    LStructureAggregationList:=TakeOffAndGetClobsList(LXmlCompReg,'<StructureAggregation');

    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LFragmentXmlList= '||LFragmentXmlList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LStructuresListt= '||LStructuresList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LNormalizedStructureList= '||LNormalizedStructureList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LStructureAggregationList= '||LStructureAggregationList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LXmlCompReg sin Structures= '||LXmlCompReg); $end null;

    -- Get the xml
    LXmlTables:=XmlType.createXML(LXmlCompReg);

    ValidateIdentityBetweenBatches(LXmlTables);

    AMessage:=NULL;

    IF Upper(ADuplicateCheck)='C' OR Upper(ADuplicateCheck)='V'THEN
        --Validate Components Strcuture
        LIndex:=0;
        LOOP
            LIndex:=LIndex+1;
            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LIndex ->'||LIndex); $end null;

            SELECT extract(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component['||LIndex||']')--.getClobVal()
              INTO LXMLCompound
              FROM dual;

        EXIT WHEN LXMLCompound IS NULL;
            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LXMLCompound ->'||LXMLCompound.getClobVal()); $end null;

            SELECT extractValue(LXMLCompound,'/Component/Compound/RegNumber/RegID'),extractValue(LXMLCompound,'/Component/Compound/BaseFragment/Structure/StructureID')
                INTO LRegNumberRegID,LDuplicatedAuxStructureID
                FROM dual;
            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegNumberRegID ->'||LRegNumberRegID); $end null;

            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','Valdiation-LStructuresList='||LStructuresList); $end null;
            LStructureValue:=TakeOffAndGetClob(LStructuresToValidateList,'Clob');
            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','Valdiation-LStructureValue= '|| LStructureValue ); $end null;

            IF NVL(LRegNumberRegID,0) = 0 AND NVL(LDuplicatedAuxStructureID,0)<>-1 THEN
                IF ValidateWildcardStructure(LStructureValue) THEN
                    SELECT extractValue(LXMLCompound,'/Component/Compound/CompoundID'),extractValue(LXMLCompound,'/Component/ComponentIndex')
                        INTO LDuplicatedCompoundID,LExistentComponentIndex
                        FROM dual;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LDuplicatedCompoundID ->'||LDuplicatedCompoundID); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LExistentComponentIndex ->'||LExistentComponentIndex); $end null;

                    IF LDuplicatedCompoundID IS NOT NULL THEN

                        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','Before ValidateCompoundMulti'); $end null;
                        SELECT extract(LXmlTables,'/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex='||LExistentComponentIndex||']/BatchComponentFragmentList')--.getClobVal()
                            INTO LXMLFragmentEquivalent
                            FROM dual;
                        $if CompoundRegistry.Debuging $then IF LXMLFragmentEquivalent IS NOT NULL THEN InsertLog('CreateMultiCompoundRegistry', 'LXMLFragmentEquivalent->'||LXMLFragmentEquivalent.getClobVal()); END IF; $end null;
                        LDuplicatedStructures:=ValidateCompoundMulti(LStructureValue, NULL, AConfigurationID,LXMLCompound,LXMLFragmentEquivalent,LXMLRegNumberDuplicatedHidden);
                        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LDuplicatedStructures->'||LDuplicatedStructures); $end null;
                        IF LDuplicatedStructures IS NOT NULL THEN
                            SELECT extractValue(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component['||LIndex||']/Compound/CompoundID')
                              INTO LDuplicatedCompoundID
                              FROM dual;
                            LListDulicatesCompound:=LListDulicatesCompound||'<COMPOUND>'||'<TEMPCOMPOUNDID>'||LDuplicatedCompoundID||'</TEMPCOMPOUNDID>'||LDuplicatedStructures||'</COMPOUND>';
                            LDuplicateComponentCount:=LDuplicateComponentCount+1;
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LDuplicateComponentCount->'||LDuplicateComponentCount); $end null;
                        END IF;
                    END IF;
                END IF;
            END IF;
        END LOOP;
        IF LListDulicatesCompound IS NOT NULL THEN
            LListDulicatesCompound:='<COMPOUNDLIST>'||LListDulicatesCompound||'</COMPOUNDLIST>';
            IF LDuplicateComponentCount=1 THEN
              AMessage:='1 duplicated component.'||LListDulicatesCompound;
              $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'AMessage->'||AMessage); $end null;
              RETURN;
            ELSE
              AMessage:=LDuplicateComponentCount||' duplicated components.'||LListDulicatesCompound;
              $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'AMessage->'||AMessage); $end null;
              RETURN;
            END IF;
        END IF;
    END IF;

    IF (Upper(ADuplicateCheck)='M') OR (Upper(ADuplicateCheck)='C') OR (Upper(ADuplicateCheck)='V') OR (Upper(ADuplicateCheck)='L')THEN

        SELECT XmlTransform(extract(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component/Compound/RegNumber/RegID'),XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/RegID">
                      <xsl:for-each select=".">
                          <xsl:value-of select="."/>,</xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')).GetClobVal()
            INTO LRegDBIdsValue
            FROM dual;

        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegDBIdsValue->'||LRegDBIdsValue); $end null;
        LRegDBIdsValue:=SUBSTR(LRegDBIdsValue,1,Length(LRegDBIdsValue)-1);

        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegDBIdsValue->'||LRegDBIdsValue); $end null;

        LDuplicatedMixtureRegIds:=ValidateMixture(LRegDBIdsValue,LDuplicatedMixtureCount,'0',null,LXmlTables);

        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LDuplicatedMixtureCount->'||LDuplicatedMixtureCount||' LDuplicatedMixtureRegIds->'||LDuplicatedMixtureRegIds); $end null;

        IF LDuplicatedMixtureRegIds IS NOT NULL THEN
            IF LDuplicatedMixtureCount>1 THEN
              AMessage:=LDuplicatedMixtureCount||' duplicated mixtures.'||LDuplicatedMixtureRegIds;
              $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'AMessage->'||AMessage); $end null;
              RETURN;
            ELSE
              AMessage:='1 duplicated mixture.'||LDuplicatedMixtureRegIds;
              $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'AMessage->'||AMessage); $end null;
              RETURN;
            END IF;
        END IF;

    END IF;

    IF (Upper(ADuplicateCheck)='V') OR (Upper(ADuplicateCheck)='L') THEN
        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'RETURN ADuplicateCheck='||ADuplicateCheck||' AMessage->'||AMessage); $end null;
        RETURN;
    END IF;

    LMessage := LMessage || 'Compound Validation OK' ||CHR(13);

    -- Build a new formatted Xml
    LXslTablesTransformed:=LXmlTables.Transform(LXslTables);
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LXslTablesTransformed= '||LXslTablesTransformed.getClobVal()); $end null;

    --Look over Xml searching each Table and insert the rows of it.
    SELECT XMLSequence(LXslTablesTransformed.Extract('/node()'))
        INTO LXmlSequenceType
        FROM DUAL;

    LStructureValue:='';
    LProcessingMixture:='Y';

    FOR LIndex IN  LXmlSequenceType.FIRST..LXmlSequenceType.LAST LOOP

        --Search each Table
        LXmlTypeRows:=LXmlSequenceType(LIndex);
        LTableName:= LXmlTypeRows.GetRootElement();

        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LTableName='||LTableName||' LXmlTypeRows='||LXmlTypeRows.getclobval()); $end null;

        --Build Message Logs
        LMessage := LMessage || chr(10) || 'Processing ' || LTableName || ': ';

        --Customization for each View - Use of Sequences
        CASE UPPER(LTableName)
            WHEN 'VW_STRUCTURE' THEN
                BEGIN
                    LStructureID:= LXmlTypeRows.extract('VW_Structure/ROW/STRUCTUREID/text()').getNumberVal();

                    IF LStructureID<>-1 THEN
                        SELECT MOLID_SEQ.NEXTVAL INTO LStructureID FROM DUAL;

                        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_STRUCTURE LStructuresList='||LStructuresList); $end null;
                        LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_STRUCTURE LStructureValue= '|| LStructureValue ); $end null;

                        SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/STRUCTUREID/text()',LStructureID)
                            INTO LXmlTypeRows
                            FROM dual;

                        InsertData(LTableName,LXmlTypeRows.getClobVal(),LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                    END IF;
                END;
            WHEN 'VW_REGISTRYNUMBER' THEN
                BEGIN
                    IF LProcessingMixture='Y' THEN
                        LRegID:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/REGID/text()').getNumberVal();
                        IF LRegID=0 THEN
                            SELECT SEQ_REG_NUMBERS.NEXTVAL INTO LRegID FROM DUAL;

                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegID'||'->'||LRegID); $end null;
                            IF AregNumGeneration = 'Y' THEN
                                LSequenceID:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/SEQUENCEID/text()').getNumberVal();

                                $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LSequenceID'||'->'||LSequenceID); $end null;

                                IF LSequenceID IS NOT NULL THEN
                                  LRegNumber:=GetRegNumber(LSequenceID,LRootNumber,LSequenceNumber);
                                END IF;

                                $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegNumber'||'->'||LRegNumber); $end null;
                                SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/REGID/text()'     ,LRegID
                                                             ,'/node()/ROW/REGNUMBER/text()'  ,LRegNumber
                                                             ,'/node()/ROW/ROOTNUMBER/text()' ,LRootNumber
                                                             ,'/node()/ROW/SEQUENCENUMBER/text()',LSequenceNumber)
                                    INTO LXmlTypeRows
                                    FROM dual;

                                IF ARegNumber IS NULL THEN  --The first regid
                                  ARegNumber:=LRegNumber;
                                END IF;
                            ELSE
                                SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/REGID/text()'     ,LRegID)
                                    INTO LXmlTypeRows
                                    FROM dual;
                            END IF;

                            LXmlRows:=LXmlTypeRows.getClobVal;
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LXmlRows'||'->'||LXmlRows); $end null;

                            InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                        END IF;
                    ELSE
                        LRegIDAux:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/REGID/text()').getNumberVal();

                        IF LRegIDAux=0 THEN

                            SELECT SEQ_REG_NUMBERS.NEXTVAL INTO LRegID FROM DUAL;
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegID'||'->'||LRegID); $end null;
                            LSequenceID:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/SEQUENCEID/text()').getNumberVal();
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LSequenceID'||'->'||LSequenceID); $end null;

                            IF LSequenceID IS NOT NULL THEN
                              LRegNumber:=GetRegNumber(LSequenceID,LRootNumber,LSequenceNumber);
                            END IF;

                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegNumber'||'->'||LRegNumber); $end null;
                            SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/REGID/text()'  ,LRegID
                                                         ,'/node()/ROW/REGNUMBER/text()'  ,LRegNumber
                                                         ,'/node()/ROW/ROOTNUMBER/text()' ,LRootNumber
                                                         ,'/node()/ROW/SEQUENCENUMBER/text()',LSequenceNumber)
                                    INTO LXmlTypeRows
                                    FROM dual;
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LXmlRows'||'->'||LXmlRows); $end null;
                            IF ARegNumber IS NULL THEN  --The first regid
                              ARegNumber:=LRegNumber;
                            END IF;

                            InsertData(LTableName,LXmlTypeRows.getClobVal(),LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                        ELSE
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_REGISTRYNUMBER LRegIDAux->'||LRegIDAux); $end null;
                            SELECT CompoundID
                                INTO LCompoundID
                                FROM VW_Compound WHERE RegID=LRegIDAux;

                            LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LStructureValue= '|| LStructureValue ); $end null;
                        END IF;
                    END IF;
                END;
            WHEN 'VW_COMPOUND_IDENTIFIER' THEN
                BEGIN
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/REGID/text()',LRegID)
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=LXmlTypeRows.getClobVal();
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
            WHEN 'VW_BATCH' THEN
                BEGIN
                    SELECT SEQ_BATCHES.NEXTVAL INTO LBatchID FROM DUAL;

                    SELECT NVL(MAX(BatchNumber),0)+1
                        INTO LBatchNumber
                        FROM VW_Batch
                        WHERE REGID=LRegID;

                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/BATCHID/text()'  ,LBatchID
                                                 ,'/node()/ROW/REGID/text()'  ,LRegID
                                                 ,'/node()/ROW/BATCHNUMBER/text()'  ,LBatchNumber
                                                             ,'/node()/ROW/DATECREATED/text()' ,TO_CHAR(SYSDATE)
                                                             ,'/node()/ROW/DATELASTMODIFIED/text()',TO_CHAR(SYSDATE))
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=replace(replace(LXmlTypeRows.getClobVal,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
            WHEN 'VW_BATCH_PROJECT' THEN
                BEGIN
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/BATCHID/text()', LBatchID)
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=LXmlTypeRows.getClobVal();
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
            WHEN 'VW_REGISTRYNUMBER_PROJECT' THEN
                BEGIN
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/REGID/text()', LRegId)
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=LXmlTypeRows.getClobVal();
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
            WHEN 'VW_COMPOUND' THEN
                BEGIN
                    SELECT SEQ_COMPOUND_MOLECULE.NEXTVAL INTO LCompoundID FROM DUAL;
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/COMPOUNDID/text()'  ,LCompoundID
                                                 ,'/node()/ROW/REGID/text()'  ,LRegID
                                                 ,'/node()/ROW/STRUCTUREID/text()'  ,LStructureID
                                                 ,'/node()/ROW/DATECREATED/text()' ,TO_CHAR(SYSDATE)
                                                 ,'/node()/ROW/DATELASTMODIFIED/text()',TO_CHAR(SYSDATE))
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=replace(replace(LXmlTypeRows.getClobVal,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_COMPOUND LXmlRows='||LXmlRows); $end null;
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
             WHEN 'VW_FRAGMENT' THEN
                BEGIN
                    LFragmentID:= LXmlTypeRows.extract('VW_Fragment/ROW/FRAGMENTID/text()').getNumberVal();
                END;
              WHEN 'VW_COMPOUND_FRAGMENT' THEN
                BEGIN
                    SELECT SEQ_COMPOUND_FRAGMENT.NEXTVAL INTO LCompoundFragmentID FROM DUAL;

                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/ID/text()'  ,LCompoundFragmentID
                                                 ,'/node()/ROW/FRAGMENTID/text()'  ,LFragmentID
                                                 ,'/node()/ROW/COMPOUNDID/text()'  ,LCompoundID)
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=LXmlTypeRows.getClobVal();
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_COMPOUND_FRAGMENT LXmlRows='||LXmlRows); $end null;
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
              WHEN 'VW_MIXTURE' THEN
                BEGIN
                    SELECT SEQ_MIXTURE.NEXTVAL INTO LMixtureID FROM DUAL;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_MIXTURE LRegID->'||LRegID); $end null;

                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','VW_MIXTURE LStructureAggregationList='||LStructureAggregationList); $end null;
                    LStructureAggregationValue:=TakeOffAndGetClob(LStructureAggregationList,'Clob');
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LStructureAggregationValue= '||LStructureAggregationValue); $end null;

                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/MIXTUREID/text()'  ,LMixtureID
                                                 ,'/node()/ROW/REGID/text()'  ,LRegID
                                                 ,'/node()/ROW/CREATED/text()' ,TO_CHAR(SYSDATE)
                                                 ,'/node()/ROW/MODIFIED/text()',TO_CHAR(SYSDATE))
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=replace(replace(LXmlTypeRows.getClobVal,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);

                    LProcessingMixture:='N';
                END;
              WHEN 'VW_MIXTURE_COMPONENT' THEN
                BEGIN
                    SELECT SEQ_MIXTURE_COMPONENT.NEXTVAL INTO LMixtureComponentID FROM DUAL;
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/MIXTURECOMPONENTID/text()'  ,LMixtureComponentID
                                                 ,'/node()/ROW/MIXTUREID/text()'  ,LMixtureID
                                                 ,'/node()/ROW/COMPOUNDID/text()'  ,LCompoundID)
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=LXmlTypeRows.getClobVal();
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
              WHEN 'VW_BATCHCOMPONENT' THEN
                BEGIN
                    SELECT SEQ_BATCHCOMPONENT.NEXTVAL INTO LBatchComponentID FROM DUAL;
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/ID/text()',LBatchComponentID
                                                 ,'/node()/ROW/MIXTURECOMPONENTID/text()',LMixtureComponentID
                                                 ,'/node()/ROW/BATCHID/text()'  ,LBatchId)
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=replace(replace(LXmlTypeRows.getClobVal,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
              WHEN 'VW_BATCHCOMPONENTFRAGMENT' THEN
                BEGIN
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/BATCHCOMPONENTID/text()',LBatchComponentID
                                                 ,'/node()/ROW/COMPOUNDFRAGMENTID/text()', LCompoundFragmentID)
                        INTO LXmlTypeRows
                        FROM dual;
                    LXmlRows:=replace(replace(LXmlTypeRows.getClobVal,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;

              ELSE  LMessage := LMessage || ' "' || LTableName || '" table stranger.' ||chr(13);

        END CASE;

    END LOOP;

    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','Fin ARegNumber='||ARegNumber); $end null;
    IF ARegNumber IS NOT NULL AND LXMLRegNumberDuplicatedHidden IS NOT NULL THEN
        SaveRegNumberDuplicatedHidden(ARegNumber, LXMLRegNumberDuplicatedHidden);
    END IF;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        LMessage := LMessage || ' ' || cast(0 as string) || ' Row/s Inserted on "' || LTableName || '".';
        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK||'Process Log:'||CHR(13)||LMessage);$end null;
        RAISE_APPLICATION_ERROR(eCreateMultiCompoundRegistry,CHR(13)||DBMS_UTILITY.FORMAT_ERROR_STACK||'Process Log:'||CHR(13)||LMessage||CHR(13));
    END;
END;

PROCEDURE RetrieveMultiCompoundRegistry(ARegNumber in VW_RegistryNumber.RegNumber%type, AXml out NOCOPY clob,ASectionsList in Varchar2:='') IS
    LQryCtx                   DBMS_XMLGEN.ctxHandle;
    LResult                   CLOB;
    LXml                      CLOB;
    LXmlTemp                  CLOB;

    LXmlTables                XmlType;
    LXslTablesTransformed     XmlType;
    LXmlResult                XmlType;
    LMessage                  CLOB:='';
    LStructuresList           CLOB;
    LNormalizedStructureList  CLOB;
    LStructureAggregationList CLOB;

    LRegID                    reg_numbers.reg_id%type;
    LCompundRegID             reg_numbers.reg_id%type;


    LMixtureFields            CLOB;
    LCompoundFields           CLOB;
    LBatchFields              CLOB;
    LBatchComponentFields     CLOB;


    LCoeObjectConfigField     XmlType;

    CURSOR C_Batch(LRegID reg_numbers.reg_id%type)  IS
      SELECT BatchID FROM VW_Batch WHERE RegID=LRegID ORDER BY BatchID;

    CURSOR C_CompoundRegIDs(ARegID in VW_RegistryNumber.regid%type) IS
      SELECT C.RegID
        FROM VW_Mixture M,VW_Mixture_Component MC, VW_Compound C
        WHERE M.MixtureID=MC.MixtureID AND C.CompoundID=MC.CompoundID AND M.RegID=ARegID ORDER BY C.RegID;

    CURSOR C_BatchComponentIDs(ABatchID BatchComponent.batchid%Type) IS
      SELECT P.ID
        FROM VW_BatchComponent P
       WHERE P.BatchID=ABatchID
       ORDER BY P.OrderIndex;

    CURSOR C_BatchComponentFragmentIDs(ABatchComponetID BatchComponentFragment.BatchComponentID%Type) IS
      SELECT BCF.ID
        FROM VW_BatchComponentFragment BCF
       WHERE BCF.BatchComponentID=ABatchComponetID
       ORDER BY BCF.OrderIndex;

    LXslTables XmlType:=XmlType.CreateXml('
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/MultiCompoundRegistryRecord">
		<MultiCompoundRegistryRecord>
            <xsl:attribute name="SameBatchesIdentity"><xsl:value-of select="@SameBatchesIdentity"/></xsl:attribute>
			<xsl:variable name="VMultiCompoundRegistryRecord" select="."/>
			<xsl:variable name="VTypeRegistryRecord" select="@TypeRegistryRecord"/>
			<xsl:for-each select="Mixture">
				<ID>
					<xsl:for-each select="MIXTUREID">
						<xsl:value-of select="."/>
					</xsl:for-each>
				</ID>
				<DateCreated>
					<xsl:for-each select="CREATED">
						<xsl:value-of select="."/>
					</xsl:for-each>
				</DateCreated>
				<DateLastModified>
					<xsl:for-each select="MODIFIED">
						<xsl:value-of select="."/>
					</xsl:for-each>
				</DateLastModified>
				<PersonCreated>
					<xsl:for-each select="PERSONCREATED">
						<xsl:value-of select="."/>
					</xsl:for-each>
				</PersonCreated>
				<StructureAggregation>
					<xsl:for-each select="STRUCTUREAGGREGATION">
						<xsl:value-of select="."/>
					</xsl:for-each>
				</StructureAggregation>
				<PropertyList>
					<xsl:for-each select="PropertyList/ROW">
						<xsl:for-each select="node()">
                    LESS_THAN_SIGN;Property name="<xsl:value-of select="name()"/>"GREATER_THAN_SIGN;<xsl:value-of select="."/>LESS_THAN_SIGN;/PropertyGREATER_THAN_SIGN;</xsl:for-each>
					</xsl:for-each>
				</PropertyList>
				<RegNumber>
					<RegID>
						<xsl:for-each select="REGID">
							<xsl:value-of select="."/>
						</xsl:for-each>
					</RegID>
					<RootNumber>
						<xsl:for-each select="ROOTNUMBER">
							<xsl:value-of select="."/>
						</xsl:for-each>
					</RootNumber>
					<SequenceNumber>
						<xsl:for-each select="SEQUENCENUMBER">
							<xsl:value-of select="."/>
						</xsl:for-each>
					</SequenceNumber>
					<RegNumber>
						<xsl:for-each select="REGNUMBER">
							<xsl:value-of select="."/>
						</xsl:for-each>
					</RegNumber>
					<SequenceID>
						<xsl:for-each select="SEQUENCEID">
							<xsl:value-of select="."/>
						</xsl:for-each>
					</SequenceID>
				</RegNumber>
				<IdentifierList>
					<xsl:for-each select="Identifier/ROW">
						<xsl:variable name="VROW5" select="."/>
						<Identifier>
							<ID>
								<xsl:for-each select="ID">
									<xsl:value-of select="."/>
								</xsl:for-each>
							</ID>
							<IdentifierID>
								<xsl:for-each select="TYPE">
									<xsl:for-each select="$VROW5/DESCRIPTION">
										<xsl:attribute name="Description"><xsl:value-of select="."/></xsl:attribute>
									</xsl:for-each>
                                    <xsl:for-each select="$VROW5/INPUTTEXT">
										<xsl:attribute name="Name"><xsl:value-of select="."/></xsl:attribute>
									</xsl:for-each>
                                    <xsl:for-each select="$VROW5/ACTIVE">
										<xsl:attribute name="Active"><xsl:value-of select="."/></xsl:attribute>
									</xsl:for-each>
									<xsl:value-of select="."/>
								</xsl:for-each>
							</IdentifierID>
							<Value>
								<xsl:for-each select="VALUE">
									<xsl:value-of select="."/>
								</xsl:for-each>
							</Value>
						</Identifier>
					</xsl:for-each>
				</IdentifierList>
				<ProjectList>
					<xsl:for-each select="RegistryRecord_Project/ROW">
                        <Project>
    						<xsl:variable name="VROW6" select="."/>
                            <ID>
                                <xsl:for-each select="ID">
									<xsl:value-of select="."/>
								</xsl:for-each>
                            </ID>
    						<ProjectID>
    							<xsl:for-each select="PROJECTID">
    								<xsl:for-each select="$VROW6/DESCRIPTION">
    									<xsl:attribute name="Description"><xsl:value-of select="."/></xsl:attribute>
    								</xsl:for-each>
                                    <xsl:for-each select="$VROW6/NAME">
    									<xsl:attribute name="Name"><xsl:value-of select="."/></xsl:attribute>
    								</xsl:for-each>
                                    <xsl:for-each select="$VROW6/ACTIVE">
    									<xsl:attribute name="Active"><xsl:value-of select="."/></xsl:attribute>
    								</xsl:for-each>
    								<xsl:value-of select="."/>
    							</xsl:for-each>
    						</ProjectID>
                        </Project>
					</xsl:for-each>
				</ProjectList>
			</xsl:for-each>
			<xsl:if test="$VTypeRegistryRecord=''Mixture''">
				<ComponentList>
					<xsl:for-each select="Compound/ROW">
						<Component>
							<ID/>
							<ComponentIndex>
								<xsl:for-each select="COMPONENTINDEX">
									<xsl:value-of select="."/>
								</xsl:for-each>
							</ComponentIndex>
							<Compound>
								<CompoundID>
									<xsl:for-each select="COMPOUNDID">
										<xsl:value-of select="."/>
									</xsl:for-each>
								</CompoundID>
								<DateCreated>
									<xsl:for-each select="DATECREATED">
										<xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
										<xsl:value-of select="."/>
									</xsl:for-each>
								</DateCreated>
								<PersonCreated>
									<xsl:for-each select="PERSONCREATED">
										<xsl:value-of select="."/>
									</xsl:for-each>
								</PersonCreated>
								<PersonRegistered>
									<xsl:for-each select="PERSONREGISTERED">
										<xsl:value-of select="."/>
									</xsl:for-each>
								</PersonRegistered>
								<DateLastModified>
									<xsl:for-each select="DATELASTMODIFIED">
										<xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
										<xsl:value-of select="."/>
									</xsl:for-each>
								</DateLastModified>
								<xsl:for-each select="PropertyList/ROW">
									<PropertyList>
										<xsl:for-each select="node()">
                LESS_THAN_SIGN;Property name="<xsl:value-of select="name()"/>"GREATER_THAN_SIGN;<xsl:value-of select="."/>LESS_THAN_SIGN;/PropertyGREATER_THAN_SIGN;</xsl:for-each>
									</PropertyList>
								</xsl:for-each>
								<RegNumber>
									<RegID>
										<xsl:for-each select="REGID">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</RegID>
									<RootNumber>
										<xsl:for-each select="ROOTNUMBER">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</RootNumber>
									<SequenceNumber>
										<xsl:for-each select="SEQUENCENUMBER">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</SequenceNumber>
									<RegNumber>
										<xsl:for-each select="REGNUMBER">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</RegNumber>
									<SequenceID>
										<xsl:for-each select="SEQUENCEID">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</SequenceID>
								</RegNumber>
								<BaseFragment>
									<Structure>
										<StructureID>
											<xsl:for-each select="STRUCTUREID">
												<xsl:value-of select="."/>
											</xsl:for-each>
										</StructureID>
										<StrucureFormat>
											<xsl:for-each select="STRUCTUREFORMAT">
												<xsl:value-of select="."/>
											</xsl:for-each>
										</StrucureFormat>
										<xsl:variable name="VROW" select="."/>
										<Structure>
											<xsl:for-each select="STRUCTURE">
												<xsl:for-each select="$VROW/FORMULAWEIGHT">
													<xsl:attribute name="molWeight"><xsl:value-of select="."/></xsl:attribute>
												</xsl:for-each>
												<xsl:for-each select="$VROW/MOLECULARFORMULA">
													<xsl:attribute name="formula"><xsl:value-of select="."/></xsl:attribute>
												</xsl:for-each>
												<xsl:value-of select="."/>
											</xsl:for-each>
										</Structure>
										<NormalizedStructure>
											<xsl:for-each select="NORMALIZEDSTRUCTURE">
												<xsl:value-of select="."/>
											</xsl:for-each>
										</NormalizedStructure>
									</Structure>
								</BaseFragment>
								<FragmentList>
									<xsl:for-each select="Fragment/ROW">
								        <Fragment>
        									<CompoundFragmentID>
        										<xsl:for-each select="ID">
        											<xsl:value-of select="."/>
        										</xsl:for-each>
        									</CompoundFragmentID>
        									<FragmentID>
        										<xsl:for-each select="FRAGMENTID">
        											<xsl:value-of select="."/>
        										</xsl:for-each>
        									</FragmentID>
											<Code>
												<xsl:for-each select="CODE">
													<xsl:value-of select="."/>
												</xsl:for-each>
											</Code>
											<Name>
												<xsl:for-each select="DESCRIPTION">
													<xsl:value-of select="."/>
												</xsl:for-each>
											</Name>
                                            LESS_THAN_SIGN;FragmentTypeID lookupTable="FragmentType" lookupField="ID" displayField="Description" displayValue="<xsl:value-of select="TYPEDESCRIPTION"/>"GREATER_THAN_SIGN;<xsl:for-each select="FRAGMENTTYPEID">
            							            <xsl:value-of select="."/>
            						            </xsl:for-each>LESS_THAN_SIGN;/FragmentTypeIDGREATER_THAN_SIGN;
											<DateCreated>
												<xsl:for-each select="CREATED">
													<xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
													<xsl:value-of select="."/>
												</xsl:for-each>
											</DateCreated>
											<DateLastModified>
												<xsl:for-each select="MODIFIED">
													<xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
													<xsl:value-of select="."/>
												</xsl:for-each>
											</DateLastModified>
											<Equivalents>
												<xsl:for-each select="EQUIVALENTS">
													<xsl:value-of select="."/>
												</xsl:for-each>
											</Equivalents>
											<Structure>
												<StructureFormat>
													<xsl:for-each select="STRUCTUREFORMAT">
														<xsl:value-of select="."/>
													</xsl:for-each>
												</StructureFormat>
												<xsl:variable name="VROW1" select="."/>
												<Structure>
													<xsl:for-each select="STRUCTURE">
														<xsl:for-each select="$VROW1/MOLWEIGHT">
															<xsl:attribute name="molWeight"><xsl:value-of select="."/></xsl:attribute>
														</xsl:for-each>
														<xsl:for-each select="$VROW1/FORMULA">
															<xsl:attribute name="formula"><xsl:value-of select="."/></xsl:attribute>
														</xsl:for-each>
														<xsl:value-of select="."/>
													</xsl:for-each>
												</Structure>
											</Structure>
										</Fragment>
									</xsl:for-each>
								</FragmentList>
								<IdentifierList>
									<xsl:for-each select="Identifier/ROW">
										<xsl:variable name="VROW3" select="."/>
										<Identifier>
											<ID>
												<xsl:for-each select="ID">
													<xsl:value-of select="."/>
												</xsl:for-each>
											</ID>
											<Type>
												<xsl:for-each select="TYPE">
													<xsl:for-each select="$VROW3/DESCRIPTION">
														<xsl:attribute name="Description"><xsl:value-of select="."/></xsl:attribute>
													</xsl:for-each>
                                                    <xsl:for-each select="$VROW3/NAME">
														<xsl:attribute name="Name"><xsl:value-of select="."/></xsl:attribute>
													</xsl:for-each>
                                                    <xsl:for-each select="$VROW3/ACTIVE">
														<xsl:attribute name="Active"><xsl:value-of select="."/></xsl:attribute>
													</xsl:for-each>
													<xsl:value-of select="."/>
												</xsl:for-each>
											</Type>
											<Value>
												<xsl:for-each select="VALUE">
													<xsl:value-of select="."/>
												</xsl:for-each>
											</Value>
										</Identifier>
									</xsl:for-each>
								</IdentifierList>
							</Compound>
						</Component>
					</xsl:for-each>
				</ComponentList>
			</xsl:if>
			<!--Only Compound-->
			<xsl:if test="$VTypeRegistryRecord=''WithoutMixture''">
				<xsl:for-each select="Compound/ROW">
					<Compound>
						<CompoundID>
							<xsl:for-each select="COMPOUNDID">
								<xsl:value-of select="."/>
							</xsl:for-each>
						</CompoundID>
						<DateCreated>
							<xsl:for-each select="DATECREATED">
								<xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
								<xsl:value-of select="."/>
							</xsl:for-each>
						</DateCreated>
						<PersonCreated>
							<xsl:for-each select="PERSONCREATED">
								<xsl:value-of select="."/>
							</xsl:for-each>
						</PersonCreated>
						<PersonRegistered>
							<xsl:for-each select="PERSONREGISTERED">
								<xsl:value-of select="."/>
							</xsl:for-each>
						</PersonRegistered>
						<DateLastModified>
							<xsl:for-each select="DATELASTMODIFIED">
								<xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
								<xsl:value-of select="."/>
							</xsl:for-each>
						</DateLastModified>
						<xsl:for-each select="PropertyList/ROW">
							<PropertyList>
								<xsl:for-each select="node()">
                LESS_THAN_SIGN;Property name="<xsl:value-of select="name()"/>"GREATER_THAN_SIGN;<xsl:value-of select="."/>LESS_THAN_SIGN;/PropertyGREATER_THAN_SIGN;</xsl:for-each>
							</PropertyList>
						</xsl:for-each>
						<RegNumber>
							<RegID>
								<xsl:for-each select="REGID">
									<xsl:value-of select="."/>
								</xsl:for-each>
							</RegID>
							<RootNumber>
								<xsl:for-each select="ROOTNUMBER">
									<xsl:value-of select="."/>
								</xsl:for-each>
							</RootNumber>
							<SequenceNumber>
								<xsl:for-each select="SEQUENCENUMBER">
									<xsl:value-of select="."/>
								</xsl:for-each>
							</SequenceNumber>
							<RegNumber>
								<xsl:for-each select="REGNUMBER">
									<xsl:value-of select="."/>
								</xsl:for-each>
							</RegNumber>
							<SequenceID>
								<xsl:for-each select="SEQUENCEID">
									<xsl:value-of select="."/>
								</xsl:for-each>
							</SequenceID>
						</RegNumber>
						<BaseFragment>
							<Structure>
								<StructureID>
									<xsl:for-each select="STRUCTUREID">
										<xsl:value-of select="."/>
									</xsl:for-each>
								</StructureID>
								<StrucureFormat>
									<xsl:for-each select="STRUCTUREFORMAT">
										<xsl:value-of select="."/>
									</xsl:for-each>
								</StrucureFormat>
								<xsl:variable name="VROW" select="."/>
								<Structure>
									<xsl:for-each select="STRUCTURE">
										<xsl:for-each select="$VROW/FORMULAWEIGHT">
											<xsl:attribute name="molWeight"><xsl:value-of select="."/></xsl:attribute>
										</xsl:for-each>
										<xsl:for-each select="$VROW/MOLECULARFORMULA">
											<xsl:attribute name="formula"><xsl:value-of select="."/></xsl:attribute>
										</xsl:for-each>
										<xsl:value-of select="."/>
									</xsl:for-each>
								</Structure>
								<NormalizedStructure>
									<xsl:for-each select="NORMALIZEDSTRUCTURE">
										<xsl:value-of select="."/>
									</xsl:for-each>
								</NormalizedStructure>
							</Structure>
						</BaseFragment>
						<FragmentList>
							<xsl:for-each select="Fragment/ROW">
								<Fragment>
                                    <CompoundFragmentID>
										<xsl:for-each select="ID">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</CompoundFragmentID>
									<FragmentID>
										<xsl:for-each select="FRAGMENTID">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</FragmentID>
									<Code>
										<xsl:for-each select="CODE">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</Code>
									<Name>
										<xsl:for-each select="DESCRIPTION">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</Name>
                                    LESS_THAN_SIGN;FragmentTypeID lookupTable="FragmentType" lookupField="ID" displayField="Description" displayValue="<xsl:value-of select="TYPEDESCRIPTION"/>"GREATER_THAN_SIGN;<xsl:for-each select="FRAGMENTTYPEID"><xsl:value-of select="."/></xsl:for-each>LESS_THAN_SIGN;/FragmentTypeIDGREATER_THAN_SIGN;
									<DateCreated>
										<xsl:for-each select="CREATED">
											<xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
											<xsl:value-of select="."/>
										</xsl:for-each>
									</DateCreated>
									<DateLastModified>
										<xsl:for-each select="MODIFIED">
											<xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
											<xsl:value-of select="."/>
										</xsl:for-each>
									</DateLastModified>
									<Equivalents>
										<xsl:for-each select="EQUIVALENTS">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</Equivalents>
									<Structure>
										<StructureFormat>
											<xsl:for-each select="STRUCTUREFORMAT">
												<xsl:value-of select="."/>
											</xsl:for-each>
										</StructureFormat>
										<xsl:variable name="VROW1" select="."/>
										<Structure>
											<xsl:for-each select="STRUCTURE">
												<xsl:for-each select="$VROW1/MOLWEIGHT">
													<xsl:attribute name="molWeight"><xsl:value-of select="."/></xsl:attribute>
												</xsl:for-each>
												<xsl:for-each select="$VROW1/FORMULA">
													<xsl:attribute name="formula"><xsl:value-of select="."/></xsl:attribute>
												</xsl:for-each>
												<xsl:value-of select="."/>
											</xsl:for-each>
										</Structure>
									</Structure>
								</Fragment>
							</xsl:for-each>
						</FragmentList>
						<IdentifierList>
							<xsl:for-each select="Identifier/ROW">
								<xsl:variable name="VROW3" select="."/>
								<Identifier>
									<ID>
										<xsl:for-each select="ID">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</ID>
									<Type>
										<xsl:for-each select="TYPE">
											<xsl:for-each select="$VROW3/DESCRIPTOR">
												<xsl:attribute name="displayValue"><xsl:value-of select="."/></xsl:attribute>
											</xsl:for-each>
											<xsl:value-of select="."/>
										</xsl:for-each>
									</Type>
									<Value>
										<xsl:for-each select="VALUE">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</Value>
								</Identifier>
							</xsl:for-each>
						</IdentifierList>
					</Compound>
				</xsl:for-each>
			</xsl:if>
			<BatchList>
				<xsl:for-each select="$VMultiCompoundRegistryRecord/Batch/ROW">
					<Batch>
						<BatchID>
							<xsl:for-each select="BATCHID">
								<xsl:value-of select="."/>
							</xsl:for-each>
						</BatchID>
						<BatchNumber>
							<xsl:for-each select="BATCHNUMBER">
								<xsl:value-of select="."/>
							</xsl:for-each>
						</BatchNumber>
						<DateCreated>
							<xsl:for-each select="DATECREATED">
								<xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
								<xsl:value-of select="."/>
							</xsl:for-each>
						</DateCreated>
						<xsl:variable name="VBATCH" select="."/>
						<PersonCreated>
							<xsl:for-each select="PERSONCREATED">
								<xsl:for-each select="$VBATCH/PERSONCREATEDDISPLAY">
									<xsl:attribute name="displayName"><xsl:value-of select="."/></xsl:attribute>
								</xsl:for-each>
								<xsl:value-of select="."/>
							</xsl:for-each>
						</PersonCreated>
						<PersonRegistered>
							<xsl:for-each select="PERSONREGISTERED">
								<xsl:for-each select="$VBATCH/PERSONREGISTEREDDISPLAY">
									<xsl:attribute name="displayName"><xsl:value-of select="."/></xsl:attribute>
								</xsl:for-each>
								<xsl:value-of select="."/>
							</xsl:for-each>
						</PersonRegistered>
						<DateLastModified>
							<xsl:for-each select="DATELASTMODIFIED">
								<xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
								<xsl:value-of select="."/>
							</xsl:for-each>
						</DateLastModified>
                        <ProjectList>
        					<xsl:for-each select="Batch_Project/ROW">
                                <Project>
            						<xsl:variable name="VBatchProject" select="."/>
                                    <ID>
                                        <xsl:for-each select="ID">
        									<xsl:value-of select="."/>
        								</xsl:for-each>
                                    </ID>
            						<ProjectID>
            							<xsl:for-each select="PROJECTID">
            								<xsl:for-each select="$VBatchProject/DESCRIPTION">
            									<xsl:attribute name="Description"><xsl:value-of select="."/></xsl:attribute>
            								</xsl:for-each>
                                            <xsl:for-each select="$VBatchProject/NAME">
            									<xsl:attribute name="Name"><xsl:value-of select="."/></xsl:attribute>
            								</xsl:for-each>
                                            <xsl:for-each select="$VBatchProject/ACTIVE">
            									<xsl:attribute name="Active"><xsl:value-of select="."/></xsl:attribute>
            								</xsl:for-each>
            								<xsl:value-of select="."/>
            							</xsl:for-each>
            						</ProjectID>
                                </Project>
        					</xsl:for-each>
        				</ProjectList>
						<xsl:for-each select="PropertyList/ROW">
							<PropertyList>
								<xsl:for-each select="node()">
        LESS_THAN_SIGN;Property name="<xsl:variable name="V3" select="name()"/>
									<xsl:value-of select="name()"/>"GREATER_THAN_SIGN;<xsl:variable name="V4" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
									<xsl:choose>
										<xsl:when test="$V3 = ''DELIVERYDATE'' and . != ''''">
											<xsl:value-of select="$V4"/>
										</xsl:when>
										<xsl:when test="$V3  = ''DATEENTERED'' and . != ''''">
											<xsl:value-of select="$V4"/>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="."/>
										</xsl:otherwise>
									</xsl:choose>LESS_THAN_SIGN;/PropertyGREATER_THAN_SIGN;</xsl:for-each>
							</PropertyList>
						</xsl:for-each>
						<BatchComponentList>
							<xsl:for-each select="BatchComponent/ROW">
								<BatchComponent>
									<ID>
										<xsl:for-each select="ID">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</ID>
									<BatchID>
										<xsl:for-each select="BATCHID">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</BatchID>
									<CompoundID>
										<xsl:for-each select="COMPOUNDID">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</CompoundID>
									<MixtureComponentID>
										<xsl:for-each select="MIXTURECOMPONENTID">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</MixtureComponentID>
									<ComponentIndex>
										<xsl:for-each select="COMPONENTINDEX">
											<xsl:value-of select="."/>
										</xsl:for-each>
									</ComponentIndex>
									<xsl:for-each select="PropertyList/ROW">
										<PropertyList>
											<xsl:for-each select="node()">
                                  LESS_THAN_SIGN;Property name="<xsl:variable name="V3" select="name()"/>
												<xsl:value-of select="name()"/>"GREATER_THAN_SIGN;<xsl:value-of select="."/>LESS_THAN_SIGN;/PropertyGREATER_THAN_SIGN;
                                </xsl:for-each>
										</PropertyList>
									</xsl:for-each>
									<BatchComponentFragmentList>
										<xsl:for-each select="BatchComponentFragment/ROW">
											<BatchComponentFragment>
                                                <ID>
													<xsl:for-each select="ID">
														<xsl:value-of select="."/>
													</xsl:for-each>
												</ID>
												<FragmentID>
													<xsl:for-each select="FRAGMENTID">
														<xsl:value-of select="."/>
													</xsl:for-each>
												</FragmentID>
												<Equivalents>
													<xsl:for-each select="EQUIVALENT">
														<xsl:value-of select="."/>
													</xsl:for-each>
												</Equivalents>
											</BatchComponentFragment>
										</xsl:for-each>
									</BatchComponentFragmentList>
								</BatchComponent>
							</xsl:for-each>
						</BatchComponentList>
					</Batch>
				</xsl:for-each>
			</BatchList>
		</MultiCompoundRegistryRecord>
	</xsl:template>
</xsl:stylesheet>
');

    PROCEDURE AddNullFields(AFields IN CLOB,AXml IN OUT NOCOPY CLOB) IS
        LPosBegin                 NUMBER;
        LPoslast                  NUMBER;
        LField                    VARCHAR2(30);
        LFields                   CLOB;
        LPosField                 NUMBER;
    BEGIN
        LFields:=AFields||',';
        LPosBegin:=0;
        LPoslast:=1;
        LOOP
            LPosBegin:=INSTR(LFields,',',LPoslast);
            LField:=UPPER(SUBSTR(LFields,LPoslast,LPosBegin-LPoslast));
            LPoslast:=LPosBegin+1;
            EXIT WHEN LField IS NULL;
                $if CompoundRegistry.Debuging $then InsertLog('AddNullFields','Into AddNullFields AXml='||AXml||' LField:'||LField ); $end null;
                LPosField:=INSTR(AXml, '<'||LField||'>');
                IF LPosField=0 THEN
                    AXml:=REPLACE(AXml,'</ROW>',' <'||LField||'/>'||CHR(10)||' </ROW>');
                END IF;
        END LOOP;
    END;

    PROCEDURE CompundProcess(ARegID Number) IS
    BEGIN
        --**Get Compound**
        IF (ASectionsList IS NULL) OR (INSTR(ASectionsList,'Compound')<>0) THEN
            LQryCtx:=DBMS_XMLGEN.newContext(
            'SELECT C.CompoundID,C.DateCreated,C.PersonCreated,C.PersonRegistered,C.DateLastModified,C.FormulaWeight,C.MolecularFormula,
                    R.RegID,R.RootNumber,R.SequenceNumber,R.RegNumber,R.SequenceID,
                    S.StructureID,S.StructureFormat,S.Structure,-C.CompoundID COMPONENTINDEX
               FROM VW_Compound C,VW_RegistryNumber R,VW_Structure S
               WHERE C.RegID(+)=R.RegID AND S.StructureID(+)=C.StructureID AND C.RegID='||ARegID );


            LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
            DBMS_XMLGEN.closeContext(LQryCtx);
            $if CompoundRegistry.Debuging $then InsertLog('CompundProcess','ARegID='||ARegID||' Select Compound LXml:'|| chr(10)||LXml); $end null;

            IF LXml IS NULL THEN
                RAISE_APPLICATION_ERROR(eNoRowsReturned,'No rows returned.'||CHR(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
            END IF;

            LXml:=Replace(LXml,'</ROWSET>','');
            LXml:=Replace(LXml,'ROWSET','Compound');
            LXml:=Replace(LXml,'</ROW>','');

            LResult:=LResult||LXml;

            --**Get the PropertyList Fields from the XML field
            SELECT XmlTransform(LCoeObjectConfigField,XmlType.CreateXml('
                <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                  <xsl:template match="/MultiCompoundRegistryRecord">
                        <xsl:for-each select="ComponentList/Component/Compound/PropertyList/Property">
                            <xsl:value-of select="@name"/>,</xsl:for-each>
                  </xsl:template>
                </xsl:stylesheet>')).GetStringVal()
              INTO LCompoundFields
              FROM DUAL;

            IF LCompoundFields IS NOT NULL THEN
                --take out the last character  ,'
                LCompoundFields:=SUBSTR(LCompoundFields,1,LENGTH(LCompoundFields)-1);

                --**Get the Compound's property list **
                LQryCtx:=DBMS_XMLGEN.newContext(
                    'SELECT '||LCompoundFields||'
                        FROM VW_Compound C,VW_RegistryNumber R,VW_Structure S
                        WHERE C.RegID(+)=R.RegID AND S.StructureID(+)=C.StructureID AND C.RegID='||ARegID);
                LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
                DBMS_XMLGEN.closeContext(LQryCtx);
                LXml:=Replace(LXml,'ROWSET','PropertyList');
                --Add NULL fields
                AddNullFields(LCompoundFields,LXml);
                LResult:=LResult ||LXml;
            END IF;

            --**Get Fragment**

            IF (ASectionsList IS NULL) OR (INSTR(ASectionsList,'Fragment')<>0) THEN
               LQryCtx:=DBMS_XMLGEN.newContext(
               'SELECT F.Fragmentid,F.Code,F.Description,F.FragmentTypeID,FT.Description TypeDescription,F.Molweight,F.Formula,F.Created,F.Modified,F.MolWeight,F.Formula,
                       F.StructureFormat,F.Structure,
                       CF.Equivalents,CF.ID
                  FROM VW_Fragment F, VW_Compound_Fragment CF, VW_Compound C,VW_FragmentType FT
                  WHERE F.FragmentID=CF.FragmentID AND C.CompoundID=CF.CompoundID AND FT.ID=F.FragmentTypeID AND C.RegID='||ARegID);

               LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
               DBMS_XMLGEN.closeContext(LQryCtx);
               $if CompoundRegistry.Debuging $then InsertLog('CompundProcess','Select Fragment:'|| chr(10)||LXml); $end null;
               LXml:=Replace(LXml,'ROWSET','Fragment');
               LResult:=LResult ||LXml;
            END IF;

            --**Get Compound_Identifier**
            IF (ASectionsList IS NULL) OR (INSTR(ASectionsList,'Identifier')<>0) THEN
                LQryCtx:=DBMS_XMLGEN.newContext(
                'SELECT CI.ID,CI.Type,CI.Regid,CI.Value,I.Description Description,I.Name Name,I.Active
                   FROM VW_Compound_Identifier CI,VW_IdentifierType I
                   WHERE CI.Type=I.ID(+) AND CI.RegID='||ARegID||' ORDER BY CI.OrderIndex');

                LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
                DBMS_XMLGEN.closeContext(LQryCtx);
                $if CompoundRegistry.Debuging $then InsertLog('CompundProcess','Select:'|| chr(10)||LXml); $end null;
                LXml:=Replace(LXml,'ROWSET','Identifier');
            END IF;

            LResult:=LResult ||LXml||'</ROW></Compound>';
        END IF;
    END;
BEGIN
    $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','ARegNumber:'|| ARegNumber); $end null;
    SetSessionParameter;

    --Get Query or Get empty template xml
    IF (ARegNumber IS NOT NULL) THEN

        SELECT XmlType.CreateXml(XML)
          INTO LCoeObjectConfigField
          FROM COEOBJECTCONFIG
          WHERE ID=2;

        IF (ASectionsList IS NULL) OR (INSTR(ASectionsList,'Mixture')<>0) THEN
            LResult:='<MultiCompoundRegistryRecord '||'SameBatchesIdentity="'||CoeDB.ConfigurationManager.RetrieveParameter('Registration','SameBatchesIdentity')||'"'||' TypeRegistryRecord="Mixture">';
            LResult:=LResult||'<Mixture>';
            LQryCtx:=DBMS_XMLGEN.newContext(
                'SELECT M.MixtureID,M.Created,M.Modified,M.PersonCreated,
                        R.RegID,R.RootNumber,R.SequenceNumber,R.RegNumber,R.SequenceID,M.StructureAggregation
                   FROM VW_Mixture M,VW_RegistryNumber R
                   WHERE M.RegID=R.RegID AND R.RegNumber='''||ARegNumber||'''');

            LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
            DBMS_XMLGEN.closeContext(LQryCtx);
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Mixture LXml:'|| chr(10)||LXml); $end null;

            IF LXml IS NULL THEN
                IF RegistrationRLS.GetStateRLS THEN
                    RAISE_APPLICATION_ERROR(eInvalidRegNum,'The Registry "'||ARegNumber||'" doesn''t exist or isen''t available.'||CHR(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
                ELSE
                    RAISE_APPLICATION_ERROR(eInvalidRegNum,'The Registry "'||ARegNumber||'" doesn''t exist.'||CHR(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
                END IF;    
            END IF;

            LStructureAggregationList:=TakeOffAndGetClobslist(LXml,'<STRUCTUREAGGREGATION>',NULL,Null,FALSE);

            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LStructureAggregationList:'|| LStructureAggregationList); $end null;

            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LXml Mixture SIN ESTRUCTURA:'|| LXml); $end null;

             SELECT  extractvalue(XmlType(LXml),'ROWSET/ROW/REGID')
                INTO  LRegID
                FROM  DUAL;

            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LRegID:'|| LRegID); $end null;

            LXml:=Replace(LXml,'</ROWSET>','');
            LXml:=Replace(LXml,'<ROWSET>','');
            LXml:=Replace(LXml,'<ROW>','');
            LXml:=Replace(LXml,'</ROW>','');
            LResult:=LResult ||LXml;

            --Get the PropertyList Fields from the XML field

            SELECT XmlTransform(LCoeObjectConfigField,XmlType.CreateXml('
                <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                  <xsl:template match="/MultiCompoundRegistryRecord">
                        <xsl:for-each select="PropertyList/Property">
                            <xsl:value-of select="@name"/>,</xsl:for-each>
                  </xsl:template>
                </xsl:stylesheet>')).GetStringVal()
                INTO LMixtureFields
                FROM DUAL;

            --Take out the last character (',')
            LMixtureFields:=SUBSTR(LMixtureFields,1,LENGTH(LMixtureFields)-1);
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LMixtureFields Mixture :'|| LMixtureFields); $end null;
            IF (LMixtureFields IS NOT NULL) THEN
                --**Get the Compound's property list **

                LQryCtx:=DBMS_XMLGEN.newContext(
                   'SELECT '||LMixtureFields||'
                       FROM VW_Mixture M,VW_RegistryNumber R
                       WHERE M.RegID=R.RegID AND M.RegID='||LRegID);

                LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
                DBMS_XMLGEN.closeContext(LQryCtx);
                LXml:=Replace(LXml,'ROWSET','PropertyList');
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select MixturePropertyList:'|| chr(10)||LXml); $end null;
                AddNullFields(LMixtureFields,LXml);
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','with AddNullFields'|| chr(10)||LXml); $end null;
                LResult:=LResult ||LXml;
            END IF;

            LQryCtx:=DBMS_XMLGEN.newContext(
                'SELECT CI.ID,CI.Type,CI.Regid,CI.Value,I.Description Description,I.Name Name,I.Active
                   FROM VW_Compound_Identifier CI,VW_IdentifierType I
                   WHERE CI.Type=I.ID(+) AND CI.RegID='||LRegID||' ORDER BY CI.OrderIndex');

            LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
            DBMS_XMLGEN.closeContext(LQryCtx);
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Mixture Identifier:'|| chr(10)||LXml); $end null;
            LXml:=Replace(LXml,'ROWSET','Identifier');
            LResult:=LResult ||LXml;

            -- Get and RegistryNumberProject record
            LQryCtx:=DBMS_XMLGEN.newContext(
            'SELECT RP.ID,RP.ProjectID , P.Description, P.Name, P.Active
               FROM VW_RegistryNumber_Project RP,VW_Project P
               WHERE RP.ProjectID=P.ProjectID AND RP.RegID='||LRegID||' ORDER BY RP.OrderIndex');

            LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
            DBMS_XMLGEN.closeContext(LQryCtx);
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Registry Project:'|| chr(10)||LXml); $end null;
            LXml:=Replace(LXml,'ROWSET','RegistryRecord_Project');

            LResult:=LResult ||LXml||'</Mixture>';
        ELSE
            LResult:='<MultiCompoundRegistryRecord TypeRegistryRecord="WithoutMixture">';
        END IF;

        IF (ASectionsList IS NULL) OR (INSTR(ASectionsList,'Compound')<>0) THEN
            IF  (ASectionsList IS NULL) OR INSTR(ASectionsList,'Mixture')<>0 THEN
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Into compound LRegID='||LRegID); $end null;
                FOR R_CompoundRegIDs IN C_CompoundRegIDs(LRegID) LOOP
                    $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','BEFORE CompundProcess('||R_CompoundRegIDs.RegID||') LResult:'|| LResult); $end null;
                    CompundProcess(R_CompoundRegIDs.RegID);
                    $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','AFTER CompundProcess('||R_CompoundRegIDs.RegID||') LResult:'|| LResult); $end null;
                END LOOP;
            ELSE
                SELECT RegID
                    INTO LCompundRegID
                    FROM VW_RegistryNumber
                    WHERE RegNumber=ARegNumber;
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LCompundRegID:'|| LCompundRegID); $end null;
                CompundProcess(LCompundRegID);
            END IF;
            LStructuresList:=TakeOffAndGetClobslist(LResult,'<STRUCTURE>',NULL,Null,FALSE);
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LStructuresList:'|| LStructuresList); $end null;
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','LResult STRUCTURE SIN ESTRUCTURA:'|| LResult); $end null;
        END IF;

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','**ASectionsList'|| chr(10)||ASectionsList); $end null;

        IF (ASectionsList IS NULL) OR (INSTR(ASectionsList,'Batch')<>0) THEN
            LResult:=LResult||'<Batch>';
            --Get Batch
            FOR  R_Batch IN C_Batch(LRegID) LOOP
                LQryCtx:=DBMS_XMLGEN.newContext(
                'SELECT B.Batchid, B.Batchnumber,B.Datecreated, B.Personcreated, B.PersonRegistered, B.Datelastmodified,
                        P1.User_ID PersonCreatedDisplay
                   FROM VW_Batch B,CS_SECURITY.People P1,CS_SECURITY.People P3
                   WHERE P1.Person_ID(+)=B.Personcreated AND P3.Person_ID(+)=B.PersonRegistered AND B.BatchID='||R_Batch.BatchID||' ORDER BY B.Batchid');

                LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
                DBMS_XMLGEN.closeContext(LQryCtx);
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Batch:'|| chr(10)||LXml); $end null;
                LXml:=Replace(LXml,'<ROWSET>','');
                LXml:=Replace(LXml,'</ROWSET>','');
                LXml:=Replace(LXml,'</ROW>','');
                LResult:=LResult ||LXml;
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','1'||LCoeObjectConfigField.GetStringVal()); $end null;
                --Get the PropertyList Fields from the XML field
                SELECT XmlTransform(LCoeObjectConfigField,XmlType.CreateXml('
                <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/MultiCompoundRegistryRecord">
                        <xsl:for-each select="BatchList/Batch/PropertyList/Property">
                            <xsl:value-of select="@name"/>,</xsl:for-each>
                  </xsl:template>
                </xsl:stylesheet>')).GetStringVal()
                    INTO LBatchFields
                    FROM COEOBJECTCONFIG
                    WHERE ID=2;

                IF LBatchFields IS NOT NULL THEN
                    --Take out the last character  ,'
                    LBatchFields:=SUBSTR(LBatchFields,1,LENGTH(LBatchFields)-1);

                    --Get and add Batch Property List record
                    LQryCtx:=DBMS_XMLGEN.newContext(
                    'SELECT '||LBatchFields|| '
                        FROM VW_Batch B
                        WHERE B.BatchID='||R_Batch.BatchID);

                    LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
                    DBMS_XMLGEN.closeContext(LQryCtx);
                    $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Batch Custom:'|| chr(10)||LXml); $end null;
                    LXml:=Replace(LXml,'ROWSET','PropertyList');
                    AddNullFields(LBatchFields,LXml);
                    LResult:=LResult ||LXml;
                END IF;

                -- Get and add Batch Project record
                LQryCtx:=DBMS_XMLGEN.newContext(
                'SELECT BP.ID, BP.ProjectID , P.Description, P.Active, P.Name
                   FROM VW_Batch_Project BP,VW_Project P
                   WHERE BP.ProjectID=P.ProjectID AND BP.BatchID='||R_Batch.BatchID);

                LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
                DBMS_XMLGEN.closeContext(LQryCtx);
                $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Batch Project:'|| chr(10)||LXml); $end null;
                LXml:=Replace(LXml,'ROWSET','Batch_Project');
                LResult:=LResult ||LXml;

                --Get the PropertyList Fields from the XML field
                SELECT XmlTransform(LCoeObjectConfigField,XmlType.CreateXml('
                        <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                          <xsl:template match="/MultiCompoundRegistryRecord">
                                <xsl:for-each select="BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property">
                                    <xsl:value-of select="@name"/>,</xsl:for-each>
                          </xsl:template>
                        </xsl:stylesheet>')).GetStringVal()
                 INTO LBatchComponentFields
                 FROM COEOBJECTCONFIG
                WHERE ID=2;
                LBatchComponentFields:=SUBSTR(LBatchComponentFields,1,LENGTH(LBatchComponentFields)-1);

                -- Get and add Batch Component record
                LResult:=LResult ||' <BatchComponent>' ;
                FOR R_BatchComponentIDs in C_BatchComponentIDs(R_Batch.BatchID) LOOP
                    LResult:=LResult ||' <ROW>' ;
                    LQryCtx:=DBMS_XMLGEN.newContext(
                    'SELECT P.ID,P.BatchID,M.CompoundID,M.MixtureComponentID,-M.CompoundID COMPONENTINDEX
                       FROM VW_BatchComponent P, VW_Mixture_Component M
                       WHERE P.MixtureComponentID=M.MixtureComponentID AND P.ID='||R_BatchComponentIDs.ID);

                    LXmlTemp:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
                    DBMS_XMLGEN.closeContext(LQryCtx);
                    $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Batch Component:'|| chr(10)||LXmlTemp); $end null;
                    LXml:=TakeOffAndGetClob(LXmlTemp,'ROW');
                    LResult:=LResult ||LXml;

                    IF LBatchComponentFields IS NOT NULL THEN
                        --Get and add Batch Property List record
                        LQryCtx:=DBMS_XMLGEN.newContext(
                        'SELECT '||LBatchComponentFields|| '
                            FROM VW_BatchComponent B
                            WHERE B.ID='||R_BatchComponentIDs.ID);
                            LXml:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
                            DBMS_XMLGEN.closeContext(LQryCtx);
                            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Batch Component Custom:'|| chr(10)||LXml); $end null;
                            LXml:=Replace(LXml,'ROWSET','PropertyList');
                            AddNullFields(LBatchComponentFields,LXml);
                            LResult:=LResult ||LXml;
                    END IF;

                    LResult:=LResult ||' <BatchComponentFragment>' ;
                    FOR R_BatchComponentFragmentIDs in C_BatchComponentFragmentIDs(R_BatchComponentIDs.ID) LOOP
                        LResult:=LResult ||' <ROW>' ;
                        LQryCtx:=DBMS_XMLGEN.newContext(
                        'SELECT CF.FragmentID,BCF.EQUIVALENT,BCF.ID
                           FROM VW_BatchComponentFragment BCF, VW_COMPOUND_FRAGMENT CF
                           WHERE  BCF.COMPOUNDFRAGMENTID=CF.ID AND BCF.ID='||R_BatchComponentFragmentIDs.ID);

                        LXmlTemp:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
                        DBMS_XMLGEN.closeContext(LQryCtx);
                        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Select Batch Component Fragment:'|| chr(10)||LXmlTemp); $end null;
                        LXml:=TakeOffAndGetClob(LXmlTemp,'ROW');
                        LResult:=LResult ||LXml;

                        LResult:=LResult ||' </ROW>' ;
                    END LOOP;
                    LResult:=LResult ||' </BatchComponentFragment>' ;

                    LResult:=LResult ||' </ROW>' ;
                END LOOP;
                LResult:=LResult ||' </BatchComponent>' ;

                LResult:=LResult ||' </ROW>' ;
             END LOOP;
             LResult:=LResult||'</Batch>';
        END IF;

        LResult:=LResult || '</MultiCompoundRegistryRecord>';

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','MultiCompoundRegistryRecord SIN TRANSFORMACION:'|| chr(10)||LResult); $end null;

        LXmlTables:=XmlType.CreateXml(LResult);

        --Build a new formatted Xml
        SELECT XmlTransform(LXmlTables,LXslTables).GetClobVal()
            INTO AXml
            FROM DUAL;

        --Replace '&lt;' and '&lt;'  by '<'' and '>''. I can't to do it using "XmlTransform"
        AXml:=replace(replace(replace(AXml,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>'),'&quot;','"');

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','AXml:'|| chr(10)||AXml); $end null;
        LXmlResult:=XmlType(AXml);
        AddTags(LCoeObjectConfigField,LXmlResult,'AddIns',Null);
        AddTags(LCoeObjectConfigField,LXmlResult,'ValidationRuleList','name');

        AXml:= TakeOnAndGetXml(LXmlResult.GetClobVal(),'STRUCTURE',LStructuresList);
        AXml:= TakeOnAndGetXml(AXml,'STRUCTUREAGGREGATION',LStructureAggregationList);

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','RetrieveMultiCompoundRegistry:'|| chr(10)||AXml); $end null;

    ELSE
        --Validate xml template with the CreateXml object and get it.
        SELECT XmlType.CreateXml(XML).GetClobVal()
            INTO AXml
            FROM COEOBJECTCONFIG
            WHERE ID=2;
    END IF;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegistry','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
        RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegistry, CHR(13) || DBMS_UTILITY.FORMAT_ERROR_STACK || CHR(13));
    END;
END;

PROCEDURE UpdateMultiCompoundRegistry(AXml in CLOB, AMessage OUT CLOB, ADuplicateCheck Char:='C', AConfigurationID Number:=1) IS
    /*
         Autor: Fari
         Date:07-Mar-07
         Object: Insert a single compound record
         Description: Look over a Xml searching each Table and insert the rows on it.
         Pending.
             Use of setUpdateColumn;
             Optimize use INSTR with XSLT or REGEXP_INSTR
             Optimize use SUBSTR with XSLT or REGEXPR_SUBSTR
             Optimize repase of'&lt;' and &gt;'
             Optimize XSLT
     */

    LinsCtx                     DBMS_XMLSTORE.ctxType;
    LXmlTables                  XmlType;
    LXslTablesTransformed       XmlType;
    LXmlCompReg                 CLOB;
    LXmlRows                    CLOB;
    LFieldToUpdate              CLOB;
    LStructureUpdating          CLOB;
    LRowsUpdated                Number:=0;
    LRowsProcessed              Number:=0;

    LIndex                      Number:=0;
    LIndexField                 Number:=0;
    LRowsInserted               Number:=0;
    LTableName                  CLOB;
    LFieldName                  CLOB;
    LMessage                    CLOB:='';
    LUpdate                     boolean;
    LSomeUpdate                 boolean:=FALSE;
    LSectionInsert              boolean;
    LSectionDelete              boolean;

    LRegID                      Number:=0;
    LRegIDTag CONSTANT          VARCHAR2(10):='<REGID>';
    LRegIDTagEnd CONSTANT       VARCHAR2(10):='</REGID>';

    LBatchID                    Number:=0;
    LBatchIDTag CONSTANT        VARCHAR2(10):='<BATCHID>';
    LBatchIDTagEnd CONSTANT     VARCHAR2(10):='</BATCHID>';

    LCompoundID                 Number:=0;
    LCompoundIDTag CONSTANT     VARCHAR2(15):='<COMPOUNDID>';
    LCompoundIDTagEnd CONSTANT  VARCHAR2(15):='</COMPOUNDID>';

    LFragmentID                 Number:=0;
    LFragmentIDTag CONSTANT     VARCHAR2(15):='<FRAGMENTID>';
    LFragmentIDTagEnd CONSTANT  VARCHAR2(15):='</FRAGMENTID>';

    LCompoundFragmentID                 Number:=0;
    LCompoundFragmentIdTag CONSTANT     VARCHAR2(15):='<ID>';
    LCompoundFragmentIdTagEnd CONSTANT  VARCHAR2(15):='</ID>';

    LBatchComponentIdTag CONSTANT       VARCHAR2(20):='<ID>';
    LBatchComponentIdTagEnd CONSTANT    VARCHAR2(20):='</ID>';

    LBatchComponentID                   Number:=0;
    LBatchCompFragIdTag CONSTANT        VARCHAR2(20):='<BATCHCOMPONENTID>';
    LBatchCompFragIdTagEnd CONSTANT     VARCHAR2(25):='</BATCHCOMPONENTID>';
    LBatchCompoundFragIdTag CONSTANT    VARCHAR2(20):='<COMPOUNDFRAGMENTID>';
    LBatchCompoundFragIdTagEnd CONSTANT VARCHAR2(25):='</COMPOUNDFRAGMENTID>';

    LStructureID                        Number:=0;
    LStructureIDTag CONSTANT            VARCHAR2(15):='<STRUCTUREID>';
    LStructureIDTagEnd CONSTANT         VARCHAR2(15):='</STRUCTUREID>';

    LMixtureComponentID                 Number:=0;
    LMixtureComponentIDTag CONSTANT     VARCHAR2(25):='<MIXTURECOMPONENTID>';
    LMixtureComponentIDTagEnd CONSTANT  VARCHAR2(25):='</MIXTURECOMPONENTID>';

    LMolecularFormula                VW_TEMPORARYCOMPOUND.MOLECULARFORMULA%Type;
    LMolecularFormulaTag CONSTANT    VARCHAR2(20):='<MOLECULARFORMULA>';
    LMolecularFormulaTagEnd CONSTANT VARCHAR2(20):='</MOLECULARFORMULA>';
    LFormulaWeight                   VW_TEMPORARYCOMPOUND.FormulaWeight%Type;
    LFormulaWeightTag CONSTANT       VARCHAR2(20):='<FORMULAWEIGHT>';
    LFormulaWeightTagEnd CONSTANT    VARCHAR2(20):='</FORMULAWEIGHT>';

    LBatchNumber                     Number:=0;
    LBatchNumberTag CONSTANT         VARCHAR2(15):='<BATCHNUMBER>';
    LBatchNumberTagEnd CONSTANT      VARCHAR2(15):='</BATCHNUMBER>';

    LRegNumber                       VW_REGISTRYNUMBER.RegNumber%Type;
    LRegNumberTag CONSTANT           VARCHAR2(15):='<REGNUMBER>';
    LRegNumberTagEnd CONSTANT        VARCHAR2(15):='</REGNUMBER>';

    LRootNumber                      VW_REGISTRYNUMBER.RootNumber%Type;
    LRootNumberTag CONSTANT          VARCHAR2(15):='<ROOTNUMBER>';
    LRootNumberTagEnd CONSTANT       VARCHAR2(15):='</ROOTNUMBER>';

    LSequenceNumber                  VW_REGISTRYNUMBER.SequenceNumber%Type;
    LSequenceNumberTag CONSTANT      VARCHAR2(20):='<SEQUENCENUMBER>';
    LSequenceNumberTagEnd CONSTANT   VARCHAR2(20):='</SEQUENCENUMBER>';


    LMixtureRegID                    Number:=0;


    LMixtureID                       Number:=0;
    LMixtureIDTag CONSTANT           VARCHAR2(15):='<MIXTUREID>';
    LMixtureIDTagEnd CONSTANT        VARCHAR2(15):='</MIXTUREID>';

    LComponentID                     Number:=0;

    LStructureValue                  CLOB;
    LStructuresList                  CLOB;
    LStructuresToValidateList        CLOB;
    LFragmentXmlValue                CLOB;
    LFragmentXmlList                 CLOB;
    LNormalizedStructureList         CLOB;
    LNormalizedStructureValue        CLOB;
    LStructureAggregationList        CLOB;
    LStructureAggregationValue       CLOB;

    LDuplicatedCompoundID            Number;
    LDuplicatedStructures            CLOB;
    LListDulicatesCompound           CLOB;
    LDuplicateComponentCount         Number:=0;
    LRegIDToValidate                 Number;
    LRegIdsValue                Varchar2(4000);
    LDuplicatedMixtureRegIds         Varchar2(4000);
    LDuplicatedMixtureCount          Number;
    LMixtureIDAux                    Varchar2(20);
    LCompoundIdsValueDeleting        Varchar2(4000);

    LXMLCompound                     XmlType;
    LXMLFragmentEquivalent           XmlType;
    LXMLRegNumberDuplicatedHidden    XmlType;
    LCompoundIDToDelete              Number;
    LCompoundFragmentIDTodelete      Number;
    LExistentComponentIndex          Number:=0;

    LSequenceID                      Number:=0;

    LRegIDAux                        Number:=0;
    LExistentRegID                   Number:=0;

    LXslTables                       XmlType;

    LXslTablesXML Clob:='<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/MultiCompoundRegistryRecord">
		<MultiCompoundRegistryRecord>
			<xsl:variable name="VMixtureID" select="ID"/>
			<VW_Mixture>
				<ROW>
					<xsl:for-each select="ID">
						<MIXTUREID>
							<xsl:value-of select="."/>
						</MIXTUREID>
					</xsl:for-each>
					<xsl:for-each select="DateCreated[@update=''yes'']">
						<CREATED>
							<xsl:value-of select="."/>
						</CREATED>
					</xsl:for-each>
					<xsl:for-each select="PersonCreated[@update=''yes'']">
						<PERSONCREATED>
							<xsl:value-of select="."/>
						</PERSONCREATED>
					</xsl:for-each>
					<xsl:for-each select="DateLastModified">
						<MODIFIED>
							<xsl:value-of select="."/>
						</MODIFIED>
					</xsl:for-each>
					<xsl:for-each select="StructureAggregation[@update=''yes'']">
						<STRUCTUREAGGREGATION>
							<xsl:copy-of select="."/>
						</STRUCTUREAGGREGATION>
					</xsl:for-each>
					<xsl:for-each select="PropertyList">
						<xsl:for-each select="Property[@update=''yes'']">
							<xsl:variable name="V1" select="."/>
							<xsl:for-each select="@name">
								<xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:for-each>
						</xsl:for-each>
					</xsl:for-each>
				</ROW>
			</VW_Mixture>
			<xsl:variable name="VMixtureRegID" select="RegNumber/RegID/."/>
			<VW_RegistryNumber>
				<xsl:for-each select="RegNumber">
					<ROW>
						<xsl:for-each select="RegID">
							<REGID>
								<xsl:value-of select="."/>
							</REGID>
						</xsl:for-each>
						<xsl:for-each select="RootNumber[@update=''yes'']">
							<ROOTNUMBER>
								<xsl:value-of select="."/>
							</ROOTNUMBER>
						</xsl:for-each>
						<xsl:for-each select="SequenceNumber[@update=''yes'']">
							<SEQUENCENUMBER>
								<xsl:value-of select="."/>
							</SEQUENCENUMBER>
						</xsl:for-each>
						<xsl:for-each select="RegNumber[@update=''yes'']">
							<REGNUMBER>
								<xsl:value-of select="."/>
							</REGNUMBER>
						</xsl:for-each>
						<xsl:for-each select="SequenceID[@update=''yes'']">
							<SEQUENCEID>
								<xsl:value-of select="."/>
							</SEQUENCEID>
						</xsl:for-each>
					</ROW>
				</xsl:for-each>
			</VW_RegistryNumber>
            <xsl:for-each select="ProjectList/Project">
                <VW_RegistryNumber_Project>
            		<ROW>
                        <ID><xsl:value-of select="ID"/></ID>
                        <xsl:for-each select="ProjectID[@update=''yes'']">
            				<PROJECTID>
            					<xsl:value-of select="."/>
            				</PROJECTID>
                        </xsl:for-each>
        				<xsl:for-each select="RegID[@update=''yes'']">
        					<REGID>
        						<xsl:value-of select="."/>
        					</REGID>
        				</xsl:for-each>
            		</ROW>
                </VW_RegistryNumber_Project>
            </xsl:for-each>
			<VW_Compound_Identifier>
				<xsl:for-each select="IdentifierList/Identifier">
					<ROW>
						<xsl:for-each select="ID">
							<ID>
								<xsl:value-of select="."/>
							</ID>
						</xsl:for-each>
						<xsl:for-each select="Type[@update=''yes'']">
							<TYPE>
								<xsl:value-of select="."/>
							</TYPE>
						</xsl:for-each>
						<xsl:for-each select="Value[@update=''yes'']">
							<VALUE>
								<xsl:value-of select="."/>
							</VALUE>
						</xsl:for-each>
					</ROW>
				</xsl:for-each>
			</VW_Compound_Identifier>
			<xsl:for-each select="ComponentList/Component">
				<xsl:variable name="VComponentIndex" select="ComponentIndex/."/>
				<xsl:variable name="VDeleteComponent" select="@delete"/>
				<xsl:variable name="VInsertComponent" select="@insert"/>
				<xsl:variable name="VCompoundID" select="Compound/CompoundID"/>
				<xsl:for-each select="Compound">
					<xsl:variable name="VRegID" select="RegNumber/RegID"/>
					<VW_RegistryNumber>
						<xsl:choose>
							<xsl:when test="$VInsertComponent=''yes''">insert="yes"</xsl:when>
						</xsl:choose>
						<xsl:for-each select="RegNumber">
							<ROW>
								<xsl:for-each select="RegID">
									<REGID>
										<xsl:value-of select="."/>
									</REGID>
								</xsl:for-each>
								<xsl:for-each select="RootNumber[@update=''yes'' or $VInsertComponent=''yes'']">
									<ROOTNUMBER>
										<xsl:value-of select="."/>
									</ROOTNUMBER>
								</xsl:for-each>
								<xsl:for-each select="SequenceNumber[@update=''yes''or $VInsertComponent=''yes'']">
									<SEQUENCENUMBER>
										<xsl:value-of select="."/>
									</SEQUENCENUMBER>
								</xsl:for-each>
								<xsl:for-each select="RegNumber[@update=''yes'' or $VInsertComponent=''yes'']">
									<REGNUMBER>
										<xsl:value-of select="."/>
									</REGNUMBER>
								</xsl:for-each>
								<xsl:for-each select="SequenceID[@update=''yes'' or $VInsertComponent=''yes'']">
									<SEQUENCEID>
										<xsl:value-of select="."/>
									</SEQUENCEID>
								</xsl:for-each>
							</ROW>
						</xsl:for-each>
					</VW_RegistryNumber>
					<xsl:choose>
						<xsl:when test="$VRegID=''0'' and $VInsertComponent=''yes'' or $VDeleteComponent=''yes'' or string-length($VInsertComponent)=0 ">
							<VW_Structure>
								<xsl:choose>
									<xsl:when test="$VInsertComponent=''yes''">insert="yes"</xsl:when>
								</xsl:choose>
								<xsl:for-each select="BaseFragment/Structure">
									<ROW>
										<xsl:for-each select="StructureID">
											<STRUCTUREID>
												<xsl:value-of select="."/>
											</STRUCTUREID>
										</xsl:for-each>
										<xsl:for-each select="StructureFormat[@update=''yes''or $VInsertComponent=''yes'']">
											<STRUCTUREFORMAT>
												<xsl:value-of select="."/>
											</STRUCTUREFORMAT>
										</xsl:for-each>
										<xsl:for-each select="Structure[@update=''yes''or $VInsertComponent=''yes'']">
											<STRUCTURE>
												<xsl:value-of select="."/>
											</STRUCTURE>
										</xsl:for-each>
									</ROW>
								</xsl:for-each>
							</VW_Structure>
							<VW_Compound>
								<xsl:choose>
									<xsl:when test="$VDeleteComponent=''yes''">delete="yes"</xsl:when>
								</xsl:choose>
								<xsl:choose>
									<xsl:when test="$VInsertComponent=''yes''">insert="yes"</xsl:when>
								</xsl:choose>
								<ROW>
									<xsl:for-each select="CompoundID">
										<COMPOUNDID>
											<xsl:value-of select="."/>
										</COMPOUNDID>
									</xsl:for-each>
									<xsl:for-each select="DateCreated[@update=''yes''or $VInsertComponent=''yes'']">
										<DATECREATED>
											<xsl:value-of select="."/>
										</DATECREATED>
									</xsl:for-each>
									<xsl:for-each select="PersonCreated[@update=''yes''or $VInsertComponent=''yes'']">
										<PERSONCREATED>
											<xsl:value-of select="."/>
										</PERSONCREATED>
									</xsl:for-each>
									<xsl:for-each select="PersonRegistered[@update=''yes''or $VInsertComponent=''yes'']">
										<PERSONREGISTERED>
											<xsl:value-of select="."/>
										</PERSONREGISTERED>
									</xsl:for-each>
									<xsl:for-each select="DateLastModified">
										<DATELASTMODIFIED>
											<xsl:value-of select="."/>
										</DATELASTMODIFIED>
									</xsl:for-each>
									<xsl:for-each select="RegNumber[@update=''yes''or $VInsertComponent=''yes'']">
										<xsl:for-each select="RegID">
											<REGID>
												<xsl:value-of select="."/>
											</REGID>
										</xsl:for-each>
									</xsl:for-each>
									<xsl:for-each select="BaseFragment">
										<xsl:for-each select="Structure">
											<xsl:for-each select="StructureID[@update=''yes'' or .=-1 or $VInsertComponent=''yes'']">
												<STRUCTUREID>
													<xsl:value-of select="."/>
												</STRUCTUREID>
											</xsl:for-each>
											<xsl:for-each select="Structure[@update=''yes'' or $VInsertComponent=''yes'']">
												<MOLECULARFORMULA/>
												<FORMULAWEIGHT/>
											</xsl:for-each>
										</xsl:for-each>
									</xsl:for-each>
									<xsl:for-each select="PropertyList">
										<xsl:for-each select="Property[@update=''yes'' or $VInsertComponent=''yes'']">
											<xsl:variable name="V1" select="."/>
											<xsl:for-each select="@name">
												<xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                      LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:for-each>
										</xsl:for-each>
									</xsl:for-each>
								</ROW>
							</VW_Compound>
							<xsl:variable name="VReg" select="."/>
							<xsl:for-each select="FragmentList">
								<xsl:for-each select="Fragment">
                                    <xsl:variable name="VInsertFragment" select="@insert"/>
                                    <xsl:variable name="VDeleteFragment" select="@delete"/>
                                    <xsl:variable name="VFragmentID" select="FragmentID"/>
                                    <xsl:for-each select="FragmentID[$VInsertFragment=''yes'' or $VInsertComponent=''yes'']">
    									<VW_Compound_Fragment>insert="yes"<ROW>
                                                <ID>0</ID>
                                                <COMPOUNDID><xsl:value-of select="$VCompoundID"/></COMPOUNDID>
                                                <FRAGMENTID><xsl:value-of select="."/></FRAGMENTID>
                                            </ROW>
                                        </VW_Compound_Fragment>
                                    </xsl:for-each>
                                    <xsl:for-each select="CompoundFragmentID[$VDeleteFragment=''yes'']">
    									<VW_Compound_Fragment>delete="yes"<ROW>
                                                <ID><xsl:value-of select="."/></ID>
                                            </ROW>
                                        </VW_Compound_Fragment>
                                    </xsl:for-each>
								</xsl:for-each>
							</xsl:for-each>
							<xsl:variable name="VCompound" select="."/>
							<xsl:for-each select="IdentifierList/Identifier">
								<VW_Compound_Identifier>
									<xsl:choose>
										<xsl:when test="$VInsertComponent=''yes''">insert="yes"</xsl:when>
									</xsl:choose>
									<ROW>
										<xsl:for-each select="ID">
											<ID>
												<xsl:value-of select="."/>
											</ID>
										</xsl:for-each>
										<xsl:for-each select="Type[@update=''yes'' or $VInsertComponent=''yes'']">
											<TYPE>
												<xsl:value-of select="."/>
											</TYPE>
										</xsl:for-each>
										<xsl:for-each select="Value[@update=''yes'' or $VInsertComponent=''yes'']">
											<VALUE>
												<xsl:value-of select="."/>
											</VALUE>
										</xsl:for-each>
										<xsl:for-each select="$VCompound/RegNumber">
											<xsl:for-each select="RegID[@update=''yes'' or $VInsertComponent=''yes'']">
												<REGID>
													<xsl:value-of select="."/>
												</REGID>
											</xsl:for-each>
										</xsl:for-each>
									</ROW>
								</VW_Compound_Identifier>
							</xsl:for-each>
						</xsl:when>
					</xsl:choose>
                    <VW_Mixture_Component>
                    	<xsl:choose>
                    		<xsl:when test="$VInsertComponent=''yes''">insert="yes"</xsl:when>
                    	</xsl:choose>
                    	<ROW>
                    		<MIXTURECOMPONENTID>0</MIXTURECOMPONENTID>
                    		<MIXTUREID>0</MIXTUREID>
                    		<COMPOUNDID>0</COMPOUNDID>
                    	</ROW>
                    </VW_Mixture_Component>
					<xsl:choose>
						<xsl:when test="$VDeleteComponent!=''yes'' or string-length($VDeleteComponent)=0">
							<xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[((ComponentIndex=$VComponentIndex) and ((@delete!=''yes'') or (string-length(@delete)=0))) ]">
								<VW_BatchComponent>
									<xsl:choose>
										<xsl:when test="$VInsertComponent=''yes''">insert="yes"</xsl:when>
									</xsl:choose>
									<ROW>
										<xsl:for-each select="ID">
											<ID>
												<xsl:value-of select="."/>
											</ID>
										</xsl:for-each>
										<xsl:for-each select="CompoundID[(@update=''yes'') or ($VInsertComponent=''yes'')]">
											<MIXTURECOMPONENTID>
												<xsl:value-of select="."/>
											</MIXTURECOMPONENTID>
										</xsl:for-each>
										<xsl:for-each select="BatchID[(@update=''yes'') or ($VInsertComponent=''yes'')]">
											<BATCHID>
												<xsl:value-of select="."/>
											</BATCHID>
										</xsl:for-each>
										<xsl:for-each select="PropertyList">
											<xsl:for-each select="Property[(@update=''yes'' or $VInsertComponent=''yes'')]">
												<xsl:variable name="V1" select="."/>
												<xsl:for-each select="@name">
													<xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                      LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;
                    </xsl:for-each>
											</xsl:for-each>
										</xsl:for-each>
									</ROW>
								</VW_BatchComponent>
                                <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment">
                                    <xsl:variable name="VFragmentID" select="FragmentID"/>
                                    <VW_BatchComponentFragment>
                                        <xsl:choose>
										    <xsl:when test="($VInsertComponent=''yes'') or (/MultiCompoundRegistryRecord/ComponentList/Component[ComponentIndex=$VComponentIndex]/Compound/FragmentList/Fragment[@insert=''yes'']/FragmentID=$VFragmentID)">insert="yes"<ROW>
                                                    <BATCHCOMPONENTID><xsl:value-of select="../../ID"/></BATCHCOMPONENTID>
                                                    <COMPOUNDFRAGMENTID>0</COMPOUNDFRAGMENTID>
                                                    <EQUIVALENT>
                                                        <xsl:value-of select="Equivalents"/>
                                                    </EQUIVALENT>
                                                </ROW>
                                            </xsl:when>
                                            <xsl:when test="Equivalents/@update=''yes''">update="yes"
                                                <ROW>
                                                    <ID><xsl:value-of select="ID"/></ID>
                                                    <xsl:for-each select="Equivalents[@update=''yes'']">
                                                        <EQUIVALENT><xsl:value-of select="."/></EQUIVALENT>
                                                    </xsl:for-each>
                                                </ROW>
                                            </xsl:when>
                                        </xsl:choose>
                                    </VW_BatchComponentFragment>
                                    <xsl:choose>
						                <xsl:when test="FragmentID/@update=''yes''">update="yes"
                                            <VW_Compound_Fragment>
                                                <ROW>
                                                    <ID><xsl:value-of select="/MultiCompoundRegistryRecord/Component/Compound/FragmentList/Fragment/CompoundFragmentID"/></ID>
                                                    <FRAGMENTID>><xsl:value-of select="FragmentID"/></FRAGMENTID>
                                                </ROW>
                                            </VW_Compound_Fragment>
                                        </xsl:when>
                                    </xsl:choose>
                                </xsl:for-each>
							</xsl:for-each>
						</xsl:when>
					</xsl:choose>
				</xsl:for-each>
			</xsl:for-each>
			<xsl:variable name="VCompound" select="."/>
			<xsl:for-each select="BatchList/Batch">
				<xsl:if test="BatchID =0 and @insert=''yes''">
					<xsl:variable name="VInsertBacth" select="@insert"/>
				</xsl:if>
				<xsl:if test="BatchID !=0 and @insert=''yes''">
					<xsl:variable name="VUpdateTable" select="''yes''"/>
				</xsl:if>
				<VW_Batch>
					<xsl:choose>
						<xsl:when test="$VInsertBacth=''yes''">insert="yes"</xsl:when>
					</xsl:choose>
					<ROW>
						<xsl:for-each select="BatchID">
							<BATCHID>
								<xsl:value-of select="."/>
							</BATCHID>
						</xsl:for-each>
						<xsl:for-each select="BatchNumber[@update=''yes'' or $VInsertBacth=''yes'']">
							<BATCHNUMBER>
								<xsl:value-of select="."/>
							</BATCHNUMBER>
						</xsl:for-each>
						<xsl:for-each select="DateCreated[@update=''yes'' or $VInsertBacth=''yes'']">
							<DATECREATED>
								<xsl:value-of select="."/>
							</DATECREATED>
						</xsl:for-each>
						<xsl:for-each select="PersonCreated[@update=''yes'' or $VInsertBacth=''yes'']">
							<PERSONCREATED>
								<xsl:value-of select="."/>
							</PERSONCREATED>
						</xsl:for-each>
						<xsl:for-each select="PersonRegistered[@update=''yes'' or $VInsertBacth=''yes'']">
							<PERSONREGISTERED>
								<xsl:value-of select="."/>
							</PERSONREGISTERED>
						</xsl:for-each>
						<xsl:for-each select="DateLastModified">
							<DATELASTMODIFIED>
								<xsl:value-of select="."/>
							</DATELASTMODIFIED>
						</xsl:for-each>
						<xsl:for-each select="BatchID[$VInsertBacth=''yes'' or $VUpdateTable=''yes'']">
							<REGID>
								<xsl:value-of select="$VMixtureRegID"/>
							</REGID>
						</xsl:for-each>
						<xsl:for-each select="TempBatchID[@update=''yes'' or $VInsertBacth=''yes'']">
							<TEMPBATCHID>
								<xsl:value-of select="."/>
							</TEMPBATCHID>
						</xsl:for-each>
						<xsl:for-each select="PropertyList">
							<xsl:for-each select="Property[@update=''yes'' or $VInsertBacth=''yes'']">
								<xsl:variable name="V1" select="."/>
								<xsl:for-each select="@name">
									<xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
									<xsl:choose>
										<xsl:when test="$V2 = ''DELIVERYDATE'' and $V1 != ''''">
        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:when>
										<xsl:when test="$V2 = ''DATEENTERED'' and $V1 != ''''">
        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:when>
										<xsl:otherwise>
        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:otherwise>
									</xsl:choose>
								</xsl:for-each>
							</xsl:for-each>
						</xsl:for-each>
					</ROW>
				</VW_Batch>
				<xsl:variable name="VBatch" select="."/>
                <xsl:choose>
					<xsl:when test="$VUpdateTable=''yes'' ">
						<xsl:for-each select="$VBatch/BatchComponentList/BatchComponent">
							<VW_BatchComponent>
								<ROW>
									<ID>0</ID>
									<xsl:for-each select="$VBatch/BatchID">
										<BATCHID>
											<xsl:value-of select="."/>
										</BATCHID>
									</xsl:for-each>
									<xsl:for-each select="MixtureComponentID">
										<MIXTURECOMPONENTID>
											<xsl:value-of select="."/>
										</MIXTURECOMPONENTID>
									</xsl:for-each>
									<xsl:for-each select="PropertyList/Property">
										<xsl:variable name="V1" select="."/>
										<xsl:for-each select="@name">
											<xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                             LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;
                         </xsl:for-each>
									</xsl:for-each>
								</ROW>
							</VW_BatchComponent>
						</xsl:for-each>
					</xsl:when>
					<xsl:when test="$VInsertBacth=''yes''">
						<xsl:for-each select="BatchComponentList/BatchComponent[(@insert=''yes'')]">
							<VW_BatchComponent>
                                    insert="yes"
                                    <ROW>
									<ID>0</ID>
									<xsl:for-each select="MixtureComponentID">
										<MIXTURECOMPONENTID>
											<xsl:value-of select="."/>
										</MIXTURECOMPONENTID>
									</xsl:for-each>
									<COMPOUNDID>
										<xsl:value-of select="CompoundID"/>
									</COMPOUNDID>
									<BATCHID>0</BATCHID>
									<xsl:for-each select="PropertyList">
										<xsl:for-each select="Property">
											<xsl:variable name="V1" select="."/>
											<xsl:for-each select="@name">
												<xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                      LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;
                    </xsl:for-each>
										</xsl:for-each>
									</xsl:for-each>
								</ROW>
							</VW_BatchComponent>
                            <xsl:variable name="VComponentIndex" select="ComponentIndex/."/>
                            <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment[(@insert=''yes'')]">
                                    <VW_BatchComponentFragment>
                                            insert="yes"
                                            <ROW>
                                                    <BATCHCOMPONENTID>0</BATCHCOMPONENTID>
                                                    <COMPOUNDFRAGMENTID><xsl:value-of select="/MultiCompoundRegistryRecord/ComponentList/Component[ComponentIndex=$VComponentIndex]/Compound/FragmentList/Fragment/CompoundFragmentID"/></COMPOUNDFRAGMENTID>
                                                    <EQUIVALENT>
                                                        <xsl:value-of select="Equivalents"/>
                                                    </EQUIVALENT>
                                            </ROW>
                                    </VW_BatchComponentFragment>
                            </xsl:for-each>
						</xsl:for-each>
					</xsl:when>
				</xsl:choose>
			</xsl:for-each>
		</MultiCompoundRegistryRecord>
	</xsl:template>
</xsl:stylesheet>
';

    PROCEDURE SetKeyValue(AID VARCHAR2,AIDTag VARCHAR2,AIDTagEnd VARCHAR2) IS
        LPosTag                   Number:=0;
        LPosTagNull                       Number:=0;
        LPosTagEnd                Number:=0;
    BEGIN
        LPosTag:=1;
        LOOP
            LPosTagNull := INSTR(LXmlRows,SUBSTR(AIDTag,1,LENGTH(AIDTag)-1)||'/>',LPosTag);
            IF LPosTagNull<>0 THEN
                LXmlRows:=REPLACE(LXmlRows,SUBSTR(AIDTag,1,LENGTH(AIDTag)-1)||'/>',AIDTag||AIDTagEnd);
            END IF;
           LPosTag := INSTR(LXmlRows,AIDTag,LPosTag);
        EXIT WHEN LPosTag=0;
            LPosTag  := LPosTag + LENGTH(AIDTag)- 1;
            LPosTagEnd := INSTR(LXmlRows,AIDTagEnd,LPosTag);
            LXmlRows:=SUBSTR(LXmlRows,1,LPosTag)||AID||SUBSTR(LXmlRows,LPosTagEnd,LENGTH(LXmlRows));
        END LOOP;
    END;

BEGIN

    LXslTables :=XmlType.CreateXml(LXslTablesXML);

    SetSessionParameter;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','AXml->'||AXml); $end null;

    LXmlCompReg:=AXml;

    LSomeUpdate:=False;
    -- Take Out the Structures because the nodes of XmlType don't suport a size grater 64k.
    --LFragmentXmlList:=TakeOffAndGetClobslist(LXmlCompReg,'<FragmentList>',NULL,NULL,TRUE);
    LFragmentXmlList:=TakeOffAndGetClobsList(LXmlCompReg,'<Structure ','Structure','<Fragment>');
    LStructuresList:=TakeOffAndGetClobsList(LXmlCompReg,'<Structure ','Structure','<BaseFragment>',NULL,FALSE);
    LStructuresToValidateList:=LStructuresList;
    LNormalizedStructureList:=TakeOffAndGetClobslist(LXmlCompReg,'<NormalizedStructure',NULL,NULL,TRUE,TRUE);
    LStructureAggregationList:=TakeOffAndGetClobslist(LXmlCompReg,'<StructureAggregation',NULL,NULL,TRUE,TRUE);

    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LFragmentXmlList= '||LFragmentXmlList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructuresListt= '||LStructuresList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LNormalizedStructureList= '||LNormalizedStructureList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureAggregationList= '||LStructureAggregationList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LXmlCompReg sin Structures= '||LXmlCompReg); $end null;

    --Get the xml
    LXmlTables:=XmlType.createXML(LXmlCompReg);
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','after'); $end null;

    ValidateIdentityBetweenBatches(LXmlTables);

    IF Upper(ADuplicateCheck)='C' THEN
        --Validate Components Strcuture
        LIndex:=0;
        LOOP
            LIndex:=LIndex+1;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LIndex ->'||LIndex); $end null;

            SELECT extract(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component['||LIndex||']')
              INTO LXMLCompound
              FROM dual;

        EXIT WHEN LXMLCompound IS NULL;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LXMLCompound='||LXMLCompound.getclobVal()); $end null;


            SELECT extract(LXMLCompound,'/Component/Compound/BaseFragment/Structure/Structure[@update="yes" or @insert="yes"]/text()').getClobVal()
              INTO LStructureUpdating
              FROM dual;

            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Valdiation-LStructuresList='||LStructuresList); $end null;
            LStructureValue:=TakeOffAndGetClob(LStructuresToValidateList,'Clob');
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Valdiation-LStructureValue='||LStructureValue); $end null;

            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureUpdating='||LStructureUpdating); $end null;

            IF LStructureValue IS NOT NULL AND INSTR(UPPER(LXMLCompound.getClobVal()),'DELETE="YES"')=0 AND LStructureUpdating IS NOT NULL THEN
                IF ValidateWildcardStructure(LStructureValue) THEN
                    SELECT extractValue(LXMLCompound,'/Component/Compound/RegNumber/RegID'),extractValue(LXMLCompound,'/Component/ComponentIndex')
                        INTO LRegIDToValidate,LExistentComponentIndex
                        FROM dual;

                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LRegIDToValidate ->'||LRegIDToValidate); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry','LExistentComponentIndex ->'||LExistentComponentIndex); $end null;

                    SELECT extract(LXmlTables,'/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex='||LExistentComponentIndex||']/BatchComponentFragmentList')--.getClobVal()
                            INTO LXMLFragmentEquivalent
                            FROM dual;
                    $if CompoundRegistry.Debuging $then IF LXMLFragmentEquivalent IS NOT NULL  THEN InsertLog('UpdateMultiCompoundRegistry', 'LXMLFragmentEquivalent->'||LXMLFragmentEquivalent.getClobVal()); END IF; $end null;
                    LDuplicatedStructures:=ValidateCompoundMulti(LStructureValue,LRegIDToValidate, AConfigurationID, LXMLCompound,LXMLFragmentEquivalent,LXMLRegNumberDuplicatedHidden);
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LRegIDToValidate='||LRegIDToValidate||'LDuplicatedStructures->'||LDuplicatedStructures); $end null;
                    IF LDuplicatedStructures IS NOT NULL THEN
                        SELECT extractValue(LXMLCompound,'/Component/Compound/CompoundID')
                            INTO LDuplicatedCompoundID
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LDuplicatedCompoundID='||LDuplicatedCompoundID); $end null;
                        LListDulicatesCompound:=LListDulicatesCompound||'<COMPOUND>'||'<TEMPCOMPOUNDID>'||LDuplicatedCompoundID||'</TEMPCOMPOUNDID>'||LDuplicatedStructures||'</COMPOUND>';
                        LDuplicateComponentCount:=LDuplicateComponentCount+1;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LDuplicateComponentCount->'||LDuplicateComponentCount); $end null;
                    END IF;
                END IF;
            END IF;
        END LOOP;
        IF LListDulicatesCompound IS NOT NULL THEN
            LListDulicatesCompound:='<COMPOUNDLIST>'||LListDulicatesCompound||'</COMPOUNDLIST>';
            IF LDuplicateComponentCount=1 THEN
              AMessage:='1 duplicated component.'||LListDulicatesCompound;
              $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry', 'AMessage->'||AMessage); $end null;
              RETURN;
            ELSE
              AMessage:=LDuplicateComponentCount||' duplicated components.'||LListDulicatesCompound;
              $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry', 'AMessage->'||AMessage); $end null;
              RETURN;
            END IF;
        END IF;
    END IF;

    IF (Upper(ADuplicateCheck)='M') OR (Upper(ADuplicateCheck)='C') THEN

        SELECT XmlTransform(extract(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[((@delete!=''yes'') or (string-length(@delete)=0))]/Compound/RegNumber/RegID'),XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/RegID">
                      <xsl:for-each select=".">
                          <xsl:value-of select="."/>,</xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')).GetClobVal()
          INTO LRegIdsValue
          FROM dual;

        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LRegIdsValue->'||LRegIdsValue); $end null;
        LRegIdsValue:=SUBSTR(LRegIdsValue,1,Length(LRegIdsValue)-1);

        SELECT extractValue(LXmlTables,'/MultiCompoundRegistryRecord/ID'),XmlTransform(extract(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[@delete=''yes'']/Compound/CompoundID'),XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/CompoundID">
                      <xsl:for-each select=".">
                          <xsl:value-of select="."/>,</xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')).GetClobVal()
            INTO LMixtureIDAux,LCompoundIdsValueDeleting
            FROM dual;
        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LMixtureIDAux:'||LMixtureIDAux); $end null;

        LCompoundIdsValueDeleting:=SUBSTR(LCompoundIdsValueDeleting,1,Length(LCompoundIdsValueDeleting)-1);

        LDuplicatedMixtureRegIds:=ValidateMixture(LRegIdsValue,LDuplicatedMixtureCount,LMixtureIDAux,null,LXmlTables);
        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LDuplicatedMixtureCount->'||LDuplicatedMixtureCount||' LDuplicatedMixtureRegIds->'||LDuplicatedMixtureRegIds); $end null;

        IF LDuplicatedMixtureRegIds IS NOT NULL THEN
             IF LDuplicatedMixtureCount>1 THEN
              AMessage:=LDuplicatedMixtureCount||' duplicated mixtures.'||LDuplicatedMixtureRegIds;
              $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'AMessage->'||AMessage); $end null;
              RETURN;
            ELSE
              AMessage:='1 duplicated mixture.'||LDuplicatedMixtureRegIds;
              $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'AMessage->'||AMessage); $end null;
              RETURN;
            END IF;
        END IF;

    END IF;

   LMessage := LMessage || 'Compound Validation OK' ||CHR(13);

    --Build a new formatted Xml
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','before LXslTablesTransformed->'||LXmlTables.getClobVal()); $end null;
    SELECT XmlTransform(LXmlTables,LXslTables)
        INTO LXslTablesTransformed
        FROM DUAL;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LXslTablesTransformed->'||LXslTablesTransformed.getClobVal()); $end null;

    LStructureValue:='';

    LIndex:=0;
    --Look over Xml searching each Table and update the rows of it.
    LOOP

        --Search each Table
        LIndex:=LIndex+1;
        SELECT LXslTablesTransformed.extract('/MultiCompoundRegistryRecord/node()['||LIndex||']').getClobVal()
        INTO LXmlRows
        FROM dual;


    EXIT WHEN LXmlRows IS NULL;

        --Replace '&lt;' and '&gt;'  by '<'' and '>''. I can't to do it using "XmlTransform"
        LXmlRows:=replace(replace(replace(LXmlRows,'&quot;','"'),'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');

        --Get Table Name. Remove  '<' and '>'
        LTableName:= substr(LXmlRows,2,INSTR(LXmlRows,'>')-2);

        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LXmlRows->'||LXmlRows); $end null;

        IF INSTR(LXmlRows,'insert="yes"')=0 THEN
            LSectionInsert:=FALSE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionInsert->FALSE'); $end null;
        ELSE
            LSectionInsert:=TRUE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionInsert->TRUE'); $end null;
        END IF;

        IF INSTR(LXmlRows,'delete="yes"')=0 THEN
            LSectionDelete:=FALSE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionDelete->FALSE'); $end null;
        ELSE
            LSectionDelete:=TRUE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionDelete->TRUE'); $end null;
        END IF;

        IF LSectionDelete THEN
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Processing delete on ' || LTableName ); $end null;
            CASE UPPER(LTableName)
                WHEN 'VW_COMPOUND' THEN
                    BEGIN
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionDelete LXmlRows->'||LXmlRows); $end null;
                        SELECT extractvalue(XmlType(LXmlRows),'VW_Compound/ROW/COMPOUNDID')
                            INTO LCompoundIDTodelete
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','DeleteCompound LCompoundIDToDelete='||LCompoundIDToDelete||' LMixtureID='||LMixtureID); $end null;
                        DeleteCompound(LCompoundIDToDelete,LMixtureID,LMessage);
                    END;
                 WHEN 'VW_COMPOUND_FRAGMENT' THEN
                    BEGIN
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSectionDelete LXmlRows->'||LXmlRows); $end null;
                        SELECT extractvalue(XmlType(LXmlRows),'VW_Compound_Fragment/ROW/ID')
                            INTO LCompoundFragmentIDTodelete
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','DeleteFragment LCompoundFragmentIDTodelete='||LCompoundFragmentIDTodelete||' LMixtureID='||LMixtureID); $end null;
                        DeleteFragment(LCompoundFragmentIDTodelete,LMessage);
                    END;
                 ELSE  LMessage := LMessage || ' "' || LTableName || '" table stranger.' ||CHR(13);
            END CASE;
        ELSIF LSectionInsert THEN
        /*Insert*/
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LTableName 3->'||LTableName||' LXmlRows='||LXmlRows); $end null;

            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Processing insert on ' || LTableName ); $end null;

            --Customization for each View - Use of Sequences
            CASE UPPER(LTableName)
                WHEN 'VW_BATCH' THEN
                    BEGIN
                        SELECT SEQ_BATCHES.NEXTVAL INTO LBatchID FROM DUAL;
                        SetKeyValue(LBatchID,LBatchIdTag,LBatchIdTagEnd);
                        SELECT  extractvalue(XmlType(LXmlRows),'VW_Batch/ROW[1]/REGID')
                            INTO LMixtureRegID
                            FROM dual;

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Inserting VW_BATCH LMixtureRegID->'||LMixtureRegID); $end null;

                        SELECT NVL(MAX(BatchNumber),0)+1
                            INTO LBatchNumber
                            FROM VW_Batch
                            WHERE REGID=LMixtureRegID;

                        SetKeyValue(LBatchNumber,LBatchNumberTag,LBatchNumberTagEnd);
                        SetKeyValue(SYSDATE,'<DATECREATED>','</DATECREATED>');
                        SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
                        InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                    END;
                WHEN 'VW_BATCH_PROJECT' THEN
                    BEGIN
                        SetKeyValue(LBatchID,LBatchIdTag,LBatchIdTagEnd);
                        InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                    END;
                WHEN 'VW_BATCHCOMPONENT' THEN
                    BEGIN
                        SELECT SEQ_BATCHCOMPONENT.NEXTVAL INTO LBatchComponentID FROM DUAL;
                        SetKeyValue(LBatchComponentID,LBatchComponentIdTag,LBatchComponentIdTagEnd);

                        IF NVL(LBatchId,0)<>0 THEN
                          SetKeyValue(LBatchId,LBatchIdTag,LBatchIdTagEnd);
                        END IF;

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LBatchId='||LBatchId||' LMixtureComponentID='||LMixtureComponentID||' LMixtureComponentIDTag='||LMixtureComponentIDTag||' LMixtureComponentIDTagEnd='||LMixtureComponentIDTagEnd||'LXmlRows1='||LXmlRows); $end null;

                        SELECT ExtractValue (XmlType (LXmlRows), 'VW_BatchComponent/ROW/COMPOUNDID')
                              INTO LComponentID
                              FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Inserting LComponentID->'||LComponentID||'LMixtureID->'||LMixtureID); $end null;

                        IF NVL (LComponentID, 0) > 0  THEN
                            SELECT MixtureComponentID
                                INTO LMixtureComponentID
                                FROM VW_Mixture_Component
                                WHERE MixtureID = LMixtureID AND CompoundID = LComponentID;
                        ELSE
                            SELECT LXslTablesTransformed.extract('/MultiCompoundRegistryRecord/VW_Compound[1]/ROW/COMPOUNDID/text()').GetNumberVal()
                           -- SELECT ExtractValue (XmlType (LXmlRows), 'VW_BatchComponent/ROW/COMPOUNDID')
                              INTO LComponentID
                              FROM dual;

                            SELECT MixtureComponentID
                                INTO LMixtureComponentID
                                FROM VW_Mixture_Component
                                WHERE MixtureID = LMixtureID AND CompoundID = LComponentID;
                        END IF;

                        IF INSTR(LXmlRows,'<COMPOUNDID>')<>0 THEN
                          LXmlRows:=SUBSTR(LXmlRows,1,INSTR(LXmlRows,'<COMPOUNDID>')-1)||SUBSTR(LXmlRows,INSTR(LXmlRows,'</COMPOUNDID>')+13,LENGTH(LXmlRows));
                        END IF;
                        LXmlRows:=REPLACE(LXmlRows,'insert="yes"','');
                        SetKeyValue(LMixtureComponentID,LMixtureComponentIDTag,LMixtureComponentIDTagEnd);

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LXmlRows2='||LXmlRows||' LMixtureComponentID='||LMixtureComponentID||' LMixtureComponentIDTag='||LMixtureComponentIDTag||' LMixtureComponentIDTagEnd='||LMixtureComponentIDTagEnd); $end null;

                        InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);

                    END;

               WHEN 'VW_STRUCTURE' THEN
                BEGIN
                    SELECT extractValue(XmlType.CreateXml(LXmlRows),'VW_Structure/ROW/STRUCTUREID/text()')
                        INTO LStructureID
                        FROM dual;
                    IF LStructureID<>-1 THEN
                        SELECT MOLID_SEQ.NEXTVAL INTO LStructureID FROM DUAL;
                        SetKeyValue(LStructureID,LStructureIDTag,LStructureIdTagEnd);

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Inserting VW_STRUCTURE LStructuresList='||LStructuresList); $end null;
                        LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Inserting VW_STRUCTURE LStructureValue= '|| LStructureValue ); $end null;
                        LXmlRows:=Replace(LXmlRows,'<STRUCTURE>(RemovedStructure)</STRUCTURE>','');
                        InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                    ELSE
                      LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                    END IF;
                END;
            WHEN 'VW_REGISTRYNUMBER' THEN
                BEGIN
                    SELECT extractValue(XmlType.CreateXml(LXmlRows),'VW_RegistryNumber/ROW/REGID/text()')
                        INTO LRegIDAux
                        FROM dual;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Inserting LRegIDAux'||'->'||LRegIDAux); $end null;
                    IF LRegIDAux=0 THEN

                        SELECT SEQ_REG_NUMBERS.NEXTVAL INTO LRegID FROM DUAL;
                        SetKeyValue(LRegID,LRegIDTag,LRegIDTagEnd);
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Inserting LRegID'||'->'||LRegID); $end null;
                        SELECT  extractvalue(XmlType(LXmlRows),'VW_RegistryNumber/ROW/SEQUENCEID')
                          INTO LSequenceID
                          FROM dual;

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Inserting LSequenceID'||'->'||LSequenceID); $end null;

                        IF LSequenceID IS NOT NULL THEN
                          LRegNumber:=GetRegNumber(LSequenceID,LRootNumber,LSequenceNumber);
                        END IF;

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Updating LRegNumber'||'->'||LRegNumber); $end null;
                        SetKeyValue(LRegNumber,LRegNumberTag,LRegNumberTagEnd);
                        SetKeyValue(LRootNumber,LRootNumberTag,LRootNumberTagEnd);
                        SetKeyValue(LSequenceNumber,LSequenceNumberTag,LSequenceNumberTagEnd);
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Updating LXmlRows'||'->'||LXmlRows); $end null;

                        InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                    ELSE
                        LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Updating1 VW_REGISTRYNUMBER LRegIDAux->'||LRegIDAux); $end null;
                        SELECT CompoundID INTO LCompoundID
                            FROM VW_Compound WHERE RegID=LRegIDAux;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Updating VW_REGISTRYNUMBER LCompoundID->'||LCompoundID); $end null;

                    END IF;
                END;
            WHEN 'VW_COMPOUND_IDENTIFIER' THEN
                BEGIN
                    SetKeyValue(LRegID,LRegIDTag,LRegIDTagEnd);
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
            WHEN 'VW_COMPOUND_PROJECT' THEN
                BEGIN
                    SetKeyValue(LRegID,LRegIDTag,LRegIDTagEnd);
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
            WHEN 'VW_COMPOUND' THEN
                BEGIN
                    SELECT SEQ_COMPOUND_MOLECULE.NEXTVAL INTO LCompoundID FROM DUAL;

                    SELECT cscartridge.formula(LStructureValue,''),cscartridge.molweight(LStructureValue)
                                INTO LMolecularFormula,LFormulaWeight
                                FROM DUAL;

                    $if CompoundRegistry.Debuging $then InsertLog('UpdateSingleCompoundRegistry',' LMolecularFormula= '|| LMolecularFormula ); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateSingleCompoundRegistry',' LFormulaWeight= '|| LFormulaWeight ); $end null;

                    SetKeyValue(LMolecularFormula,LMolecularFormulaTag,LMolecularFormulaTagEnd);
                    SetKeyValue(LFormulaWeight,LFormulaWeightTag,LFormulaWeightTagEnd);
                    SetKeyValue(LCompoundID,LCompoundIDTag,LCompoundIDTagEnd);
                    SetKeyValue(LRegID,LRegIDTag,LRegIDTagEnd);
                    SetKeyValue(LStructureID,LStructureIDTag,LStructureIdTagEnd);
                    SetKeyValue(SYSDATE,'<DATECREATED>','</DATECREATED>');
                    SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
              WHEN 'VW_COMPOUND_FRAGMENT' THEN
                BEGIN
                    SELECT SEQ_COMPOUND_FRAGMENT.NEXTVAL INTO LCompoundFragmentID FROM DUAL;
                    SetKeyValue(LCompoundFragmentID,LCompoundFragmentIdTag,LCompoundFragmentIdTagEnd);
                    --SetKeyValue(LFragmentID,LFragmentIdTag,LFragmentIdTagEnd);
                    IF NVL(LCompoundID,0)<>0 THEN
                        SetKeyValue(LCompoundID,LCompoundIDTag,LCompoundIDTagEnd);
                    END IF;
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
              WHEN 'VW_MIXTURE_COMPONENT' THEN
                BEGIN
                    SELECT SEQ_MIXTURE_COMPONENT.NEXTVAL INTO LMixtureComponentID FROM DUAL;

                    SetKeyValue(LMixtureComponentID,LMixtureComponentIDTag,LMixtureComponentIDTagEnd);
                    SetKeyValue(LMixtureID,LMixtureIDTag,LMixtureIDTagEnd);
                    SetKeyValue(LCompoundID,LCompoundIDTag,LCompoundIDTagEnd);
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;
              WHEN 'VW_BATCHCOMPONENTFRAGMENT' THEN
                BEGIN
                    IF NVL(LCompoundFragmentID,0)<>0 THEN
                        SetKeyValue(LCompoundFragmentID,LBatchCompoundFragIdTag,LBatchCompoundFragIdTagEnd);
                    END IF;
                    IF NVL(LBatchComponentID,0)<>0 THEN
                        SetKeyValue(LBatchComponentID,LBatchCompFragIdTag,LBatchCompFragIdTagEnd);
                    END IF;
                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','VW_BATCHCOMPONENTFRAGMENT= '|| LXmlRows ); $end null;
                    InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LStructureID,LMixtureID,LMessage,LRowsInserted);
                END;

                ELSE  LMessage:=LMessage || ' "' || LTableName||'" table stranger.';
            END CASE;

            LXmlRows:=replace(LXmlRows,'insert="yes"','');
            LXmlRows:=replace(LXmlRows,'delete="yes"','');

            IF LRowsInserted>0 THEN
                LRowsProcessed:=LRowsProcessed + LRowsInserted;
                LSomeUpdate:=TRUE;
            END IF;

        ELSE
        -- Update

            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Processing update on ' || LTableName ); $end null;

            LinsCtx := DBMS_XMLSTORE.newContext(LTableName);

            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Before LFieldName:'||LTableName||'-'||LXmlRows); $end null;

            SELECT  XmlType(LXmlRows).extract(LTableName||'/ROW[1]/node()[1]').getClobVal()
                INTO LFieldName
                FROM dual;

            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','After LFieldName: LFieldName='||LFieldName); $end null;

            LUpdate:=FALSE;
            IF LFieldName IS NOT NULL THEN
                DBMS_XMLSTORE.clearUpdateColumnList(LinsCtx);

                CASE UPPER(LTableName)
                    WHEN 'VW_MIXTURE' THEN
                    BEGIN
                        SELECT extractvalue(XmlType(LXmlRows),'VW_Mixture/ROW/MIXTUREID')
                          INTO LMixtureID
                          FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','W_MIXTURE LMixtureID:'||LMixtureID); $end null;

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureAggregationValue= '||LStructureAggregationValue); $end null;
                        LStructureAggregationValue:=TakeOffAndGetClob(LStructureAggregationList,'Clob');
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureAggregationValue= '||LStructureAggregationValue); $end null;

                        SetKeyValue(SYSDATE,'<MODIFIED>','</MODIFIED>');

                    END;

                    WHEN 'VW_STRUCTURE' THEN
                        BEGIN
                            --LStructureValue:=XMLType(LFieldName).getCLOBVal();
                            SELECT  extractvalue(XmlType(LXmlRows),'VW_Structure/ROW[1]/node()[1]')
                            INTO LStructureID
                            FROM dual;

                            IF LStructureID<>-1 THEN
                                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','VW_TEMPORARYCOMPOUND LStructuresList='||LStructuresList); $end null;
                                LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry',' LStructureValue= '|| LStructureValue ); $end null;

                                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureID='||LStructureID||' LStructureValue='||LStructureValue); $end null;
                            ELSE
                                SELECT MOLID_SEQ.NEXTVAL INTO LStructureID FROM DUAL;
                                SetKeyValue(LStructureID,LStructureIDTag,LStructureIdTagEnd);

                                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureID<>-1 - VW_TEMPORARYCOMPOUND LStructuresList='||LStructuresList); $end null;
                                LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry',' LStructureValue= '|| LStructureValue ); $end null;

                                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureID='||LStructureID||' LStructureValue='||LStructureValue); $end null;
                                InsertData(LTableName,LXmlRows,LStructureValue,NULL,NULL,LStructureID,NULL,LMessage,LRowsInserted);
                            END IF;


                        END;
                    WHEN 'VW_COMPOUND' THEN
                        BEGIN
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateSingleCompoundRegistry','LStructureID='||LStructureID||' LStructureValue='||LStructureValue); $end null;

                            SELECT cscartridge.formula(LStructureValue,''),cscartridge.molweight(LStructureValue)
                                INTO LMolecularFormula,LFormulaWeight
                                FROM DUAL;

                            $if CompoundRegistry.Debuging $then InsertLog('UpdateSingleCompoundRegistry',' LMolecularFormula= '|| LMolecularFormula ); $end null;
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateSingleCompoundRegistry',' LFormulaWeight= '|| LFormulaWeight ); $end null;
                            SetKeyValue(LMolecularFormula,LMolecularFormulaTag,LMolecularFormulaTagEnd);
                            SetKeyValue(LFormulaWeight,LFormulaWeightTag,LFormulaWeightTagEnd);
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateSingleCompoundRegistry','VW_STRUCTURE LXmlRows= '|| LXmlRows ); $end null;
                            SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
                            SetKeyValue(LStructureID,LStructureIDTag,LStructureIdTagEnd);
                        END;
                    WHEN 'VW_REGISTRYNUMBER' THEN
                        BEGIN
                            SELECT extractValue(XmlType.CreateXml(LXmlRows),'VW_RegistryNumber/ROW/REGID/text()')
                                INTO LRegID
                                FROM dual;
                        END;
                    WHEN 'VW_BATCH' THEN
                        BEGIN
                            SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
                        END;
                    WHEN 'VW_BATCHCOMPONENT' THEN
                        BEGIN
                            SELECT ExtractValue (XmlType (LXmlRows), 'VW_BatchComponent/ROW/MIXTURECOMPONENTID')
                              INTO LComponentID
                              FROM dual;
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Updating LComponentID->'||LComponentID||'LMixtureID->'||LMixtureID); $end null;

                            IF NVL (LComponentID, 0) <> 0  THEN
                                SELECT MixtureComponentID
                                  INTO LMixtureComponentID
                                  FROM VW_Mixture_Component
                                 WHERE MixtureID = LMixtureID AND CompoundID = LComponentID
                                   AND ROWNUM < 2 ORDER BY MixtureComponentID;

                                SetKeyValue(LMixtureComponentID,LMixtureComponentIDTag,LMixtureComponentIDTagEnd);
                            END IF;
                        END;
                    ELSE  NULL;
                END CASE;

                LFieldName:=XMLType(LFieldName).getRootElement();
                DBMS_XMLSTORE.setKeyColumn(LinsCtx,LFieldName);

                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LFieldName->'||LFieldName); $end null;

                LIndexField:=1;
                LOOP
                    --Search each Table
                    LIndexField:=LIndexField+1;
                    SELECT  XmlType(LXmlRows).extract(LTableName||'/ROW[1]/node()['||LIndexField||']').getClobVal()
                        INTO LFieldToUpdate
                        FROM dual;

                    IF LFieldToUpdate IS NOT NULL THEN
                        LUpdate:=TRUE;
                        LFieldToUpdate:=XMLType(LFieldToUpdate).getRootElement();
                    END IF;

                EXIT WHEN LFieldToUpdate IS NULL;
                    DBMS_XMLSTORE.setUpdateColumn(LinsCtx, LFieldToUpdate);
                END LOOP;
            END IF;

            --Insert Rows and get count it inserted
            IF LUpdate THEN
                LSomeUpdate:=TRUE;
                 $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','updateXM LXmlRows->'||LXmlRows); $end null;

                LXmlRows:=Replace(LXmlRows,'<STRUCTURE>(RemovedStructure)</STRUCTURE>','');
                LRowsUpdated := DBMS_XMLSTORE.updateXML(LinsCtx, LXmlRows );
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','updating LRowsUpdated->'||LRowsUpdated); $end null;
                LRowsProcessed:=LRowsProcessed + LRowsUpdated;
                --Build Message Logs
                LMessage := LMessage || ' ' || cast(LRowsUpdated as string) || ' Row/s Updated on "' || LTableName || '".'||CHR(13);
            ELSE
                --Build Message Logs
                LMessage:=LMessage || ' 0 Row Updated on "'||LTableName||'".';
            END IF;

             --Close the Table Context
             DBMS_XMLSTORE.closeContext(LinsCtx);

            IF UPPER(LTableName)='VW_STRUCTURE' AND LStructureValue IS NOT NULL THEN
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LStructureValue:'||LStructureValue||' LStructureID:'||LStructureID); $end null;
                UPDATE VW_STRUCTURE
                    SET STRUCTURE=LStructureValue
                    WHERE STRUCTUREID=LStructureID;
            END IF;

            IF UPPER(LTableName)='VW_MIXTURE' AND LStructureAggregationValue IS NOT NULL THEN
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry',' LMixtureID:'||LMixtureID||' LStructureAggregationValue:'||LStructureAggregationValue); $end null;
                UPDATE VW_MIXTURE
                    SET StructureAggregation=LStructureAggregationValue
                    WHERE MixtureID=LMixtureID;
            END IF;

            IF UPPER(LTableName)='VW_FRAGMENT' AND LFragmentXmlValue IS NOT NULL THEN
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry',' LStructureID:'||LStructureID||' LFragmentXmlValue:'||LFragmentXmlValue); $end null;
                UPDATE VW_STRUCTURE
                    SET STRUCTURE=LFragmentXmlValue
                    WHERE StructureID=LStructureID;
            END IF;
        END IF;

    END LOOP;

    IF LSomeUpdate THEN
        IF LRegNumber IS NOT NULL AND LXMLRegNumberDuplicatedHidden IS NOT NULL THEN
            SaveRegNumberDuplicatedHidden(LRegNumber, LXMLRegNumberDuplicatedHidden);
        END IF;

        --Build Message Logs
        LMessage := LMessage || chr(10) || 'Total: ' || cast(LRowsProcessed as string) || ' Row/s Processed.'||CHR(13);
        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSomeUpdate->true'); $end null;
    ELSE
        --Build Message Logs
        LMessage := LMessage || chr(10) || 'Total: ' || cast(LRowsProcessed as string) || ' Row/s Processed.'||CHR(13);
        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','LSomeUpdate->false There aren''t fields/sections to update/insert.'); $end null;
        RAISE_APPLICATION_ERROR(eGenericException, 'There aren''t fields/sections to update/insert.');
    END IF;

    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Finished -->'||chr(10) ||LMessage); $end null;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        LMessage := LMessage || ' ' || cast(0 as string) || ' Row/s Updated on "' || LTableName || '".'|| chr(13) ||'Total: 0 Row/s Updated.';
        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegistry','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK||'Process Log:'||CHR(13)||LMessage);$end null;
        RAISE_APPLICATION_ERROR(eUpdateMultiCompoundRegistry,' '||DBMS_UTILITY.FORMAT_ERROR_STACK||'Process Log:'||CHR(13)||LMessage||CHR(13));
    END;
END;


PROCEDURE DeleteMultiCompoundRegistry(ARegNumber in VW_RegistryNumber.regnumber%type) IS
    CURSOR C_Components(ARegNumber VW_RegistryNumber.regnumber%type)  IS
      SELECT MC.CompoundID,MC.MixtureID
            FROM VW_Mixture_Component MC,VW_Mixture M,VW_RegistryNumber R
            WHERE MC.MixtureID=M.MixtureID AND M.RegID=R.RegID AND R.RegNumber=ARegNumber;

/*    CURSOR C_Batches(ARegNumber VW_RegistryNumber.regnumber%type)  IS
        SELECT B.BatchID,M.MixtureID
            FROM VW_Batch B,VW_RegistryNumber R, VW_Mixture M
            WHERE B.RegID=R.RegID AND R.RegNumber=ARegNumber AND M.RegID=R.RegID;*/
   LMessage                  CLOB:='';
   LExist Number;

BEGIN
    $if CompoundRegistry.Debuging $then InsertLog('DeleteMultiCompoundRegistry','Begining mixture deleted process. ARegNumber='||ARegNumber); $end null;

    LMessage:=chr(10) || 'Begining mixture deleted process.';

    SELECT Count(1)
        INTO LExist
        FROM VW_RegistryNumber R
        WHERE R.RegNumber=ARegNumber;

    IF LExist=0 THEN
         LMessage:=chr(10) || '0 Row/s found on "VW_Mixture_Component".';
        RAISE_APPLICATION_ERROR(eInvalidRegNum,'The Registry "'||ARegNumber||'" doesn''t exist.');
    END IF;

    FOR  R_Components IN C_Components(ARegNumber) LOOP
        LMessage:=LMessage||chr(10) || 'Deleting CompoundID='||R_Components.CompoundID;
        $if CompoundRegistry.Debuging $then InsertLog('DeleteMultiCompoundRegistry','Deleting R_Components.CompoundID='||R_Components.CompoundID||'Deleting R_Components.MixtureID='||R_Components.MixtureID); $end null;
        DeleteCompound(R_Components.CompoundID,R_Components.MixtureID,LMessage);
    END LOOP;

   /*  FOR  R_Batches IN C_Batches(ARegNumber) LOOP
        DeleteBatch(R_Batches.BatchID,R_Batches.MictureID,LMessage);
    END LOOP;*/

    DELETE VW_Mixture_Component
        WHERE MixtureID IN (SELECT M.MixtureID
                                FROM VW_RegistryNumber RN,VW_Mixture M
                                WHERE RN.RegID=M.RegID AND RN.RegNumber=ARegNumber);

    LMessage:=LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Mixture_Component".';

    DELETE VW_Mixture WHERE RegID IN (SELECT RegID FROM VW_RegistryNumber WHERE RegNumber=ARegNumber);

    LMessage:=LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Mixture".';

    DELETE VW_Batch_Project WHERE BatchID IN (SELECT B.BatchID FROM VW_Batch B,VW_RegistryNumber RN WHERE B.RegID=RN.RegID AND RN.RegNumber=ARegNumber);

    LMessage:=LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Batch_Project".';

    DELETE VW_Batch WHERE RegID IN (SELECT RegID FROM VW_RegistryNumber WHERE RegNumber=ARegNumber);

    LMessage:=LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Batch".';

    DELETE VW_RegistryNumber_Project WHERE RegID IN (SELECT RegID FROM VW_RegistryNumber WHERE RegNumber=ARegNumber);

    LMessage:=LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound_Project".';

    DELETE VW_RegistryNumber WHERE RegNumber=ARegNumber;

    LMessage:=LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_RegistryNumber".';

    $if CompoundRegistry.Debuging $then InsertLog('DeleteMultiCompoundRegistry','Finished -->'||chr(10) ||LMessage); $end null;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        $if CompoundRegistry.Debuging $then InsertLog('DeleteMultiCompoundRegistry','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK||'Process Log:'||CHR(13)||LMessage);$end null;
        RAISE_APPLICATION_ERROR(eDeleteMultiCompoundRegistry,CHR(13)||DBMS_UTILITY.FORMAT_ERROR_STACK||'Process Log:'||CHR(13)||LMessage||CHR(13));
    END;
END;

PROCEDURE RetrieveMultiCompoundRegList(AXmlRegNumbers in clob, AXmlCompoundList out NOCOPY clob) IS
       -- Autor: Fari
       -- Date:17-May-07
       -- Object:
       -- Pending:
      LMessage                  CLOB:='';
      LXml                      CLOB:='';
      LXmlList                  CLOB:='';
      LSectionList   CONSTANT   VARCHAR2(500):='Compound,Fragment,Batch,Mixture';
      LRegNumber                VW_RegistryNumber.RegNumber%type;
      LIndex                    Number;
      LAux                      CLOB:='';
      LIndexaUX                 Number;
BEGIN
  LIndex:=1;
  LOOP
      $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegList','AXmlRegIDs->'||AXmlRegNumbers); $end null;
      SELECT  extractValue(XmlType(AXmlRegNumbers),'REGISTRYLIST/node()['||LIndex||']/node()[1]')
        INTO LRegNumber
        FROM dual;
      $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegList','LRegNumber->'||LRegNumber); $end null;
  EXIT WHEN LRegNumber IS NULL;
      BEGIN
          RetrieveMultiCompoundRegistry(LRegNumber,LXml,LSectionList);

          LXmlList:=LXmlList||CHR(10)||LXml;
      EXCEPTION
         WHEN OTHERS THEN
         BEGIN
           IF INSTR(DBMS_UTILITY.FORMAT_ERROR_STACK,eNoRowsReturned)<>0 THEN NULL; --Though a Compound doesn't exist to get the others
           ELSE
              RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegList,DBMS_UTILITY.FORMAT_ERROR_STACK);
           END IF;
         END;
      END;
      LIndex:=LIndex+1;
  END LOOP;
  AXmlCompoundList:='<MultiCompoundRegistryRecordList>'||CHR(10)||LXmlList||CHR(10)||'</MultiCompoundRegistryRecordList>';
  $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegList','List->'||AXmlCompoundList); $end null;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegList, LMessage||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
    END;
END;

PROCEDURE CreateMultiCompoundRegTmp(AXml in CLOB,  ATempID out Number) IS
    /*
         Autor: Fari
         Date:09-apr-07
         Object: Insert a single compound record temporary
         Description: Look over a Xml searching each Table and insert the rows on it.
         Pending.
             Optimize repase of'&lt;' and &gt;'
     */

    LinsCtx                   DBMS_XMLSTORE.ctxType;
    LXmlTables                XmlType;
    LXslTablesTransformed     XmlType;
    LXmlCompReg               CLOB;
    LXmlRows                  CLOB;
    LXmlTypeRows              XmlType;
    LXmlSequenceType          XmlSequenceType;
    LXmlSequenceTypeField     XmlSequenceType;
    LXmlField                 CLOB;
    LIndex                    Number:=0;
    LRowsInserted             Number:=0;
    LTableName                CLOB;
    LMessage                  CLOB:='';

    LPosTagBegin              Number:=0;
    LPosTagEnd                Number:=0;
    LTagXmlFieldBegin         VARCHAR2(10):='<XMLFIELD>';
    LTagXmlFieldEnd           VARCHAR2(11):='</XMLFIELD>';

    LTempCompoundID                 Number:=0;
    LTempBatchID                    Number:=0;

    LStructureValue                 CLOB;
    LStructuresList                 CLOB;
    LFragmentXmlValue               CLOB;
    LBatchCompFragmentXMLValue      CLOB;
    LFragmentXmlList                CLOB;
    LBatchComponentFragmentXMLList  CLOB;
    LNormalizedStructureList        CLOB;
    LNormalizedStructureValue       CLOB;
    LStructureAggregationList       CLOB;
    LStructureAggregationValue      CLOB;

    LMolecularFormula               VW_TEMPORARYCOMPOUND.MOLECULARFORMULA%Type;
    LFormulaWeight                  VW_TEMPORARYCOMPOUND.FormulaWeight%Type;

    LStructureID                    VARCHAR2(8);

    LProjectsSequenceType           XmlSequenceType;
    LProjectName                    VW_PROJECT.Name%Type;
    LProjectdescription             VW_PROJECT.Description%Type;
    LProjectID                      VW_PROJECT.ProjectID%Type;
    LProjectsIndex                  Number;
    LIdentifiersSequenceType        XmlSequenceType;
    LIdentifierName                 VW_IdentifierType.Name%Type;
    LIdentifierdescription          VW_IdentifierType.Description%Type;
    LIdentifierID                   VW_IdentifierType.ID%Type;
    LIdentifiersIndex               Number;


    LXslTables XmlType:=XmlType.CreateXml('<?xml version="1.0" encoding="UTF-8"?>
    <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
        <xsl:template match="/MultiCompoundRegistryRecord">
            <xsl:variable name="VIdentifierList" select="IdentifierList"/>
            <xsl:variable name="VProjectList" select="ProjectList"/>
            <xsl:variable name="VProjectBatchList" select="BatchList/Batch/ProjectList"/>
            <xsl:variable name="VPropertyList" select="PropertyList"/>
            <xsl:variable name="VLESSTHANSIGN">&lt;</xsl:variable>
            <xsl:for-each select="BatchList/Batch">
                <xsl:variable name="VBatchList_Batch" select="."/>
                <VW_TemporaryBatch>
                    <ROW>
                        <TEMPBATCHID>
                            <xsl:value-of select="BatchID"/>
                        </TEMPBATCHID>
                        <BATCHNUMBER>
                            <xsl:value-of select="BatchNumber"/>
                        </BATCHNUMBER>
                        <DATECREATED>
                            <xsl:value-of select="DateCreated"/>
                        </DATECREATED>
                        <PERSONCREATED>
                            <xsl:value-of select="PersonCreated"/>
                        </PERSONCREATED>
                        <DATELASTMODIFIED>
                            <xsl:value-of select="DateLastModified"/>
                        </DATELASTMODIFIED>
                        <SEQUENCEID>
                            <xsl:value-of select="/MultiCompoundRegistryRecord/RegNumber/SequenceID"/>
                        </SEQUENCEID>
                        <STRUCTUREAGGREGATION>
                            <xsl:copy-of select="StructureAggregation"/>
                        </STRUCTUREAGGREGATION>
                        <xsl:for-each select="PropertyList/Property">
                            <xsl:variable name="V1" select="."/>
                            <xsl:for-each select="@name">
                                <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                                <xsl:choose>
                                    <xsl:when test="$V2 = ''DELIVERYDATE'' and $V1 != ''''">
                                        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:when>
                                    <xsl:when test="$V2 = ''DATEENTERED'' and $V1 != ''''">
                                        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:when>
                                    <xsl:otherwise>
                                        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:otherwise>
                                </xsl:choose>
                            </xsl:for-each>
                        </xsl:for-each>
                        <PROJECTXML>
                            <XMLFIELD>
                                <xsl:copy-of select="$VProjectList"/>
                            </XMLFIELD>
                        </PROJECTXML>
                        <PROJECTXMLBATCH>
                            <XMLFIELD>
                                <xsl:copy-of select="$VProjectBatchList"/>
                            </XMLFIELD>
                        </PROJECTXMLBATCH>
                        <IDENTIFIERXML>
                            <XMLFIELD>
                                <xsl:copy-of select="$VIdentifierList"/>
                            </XMLFIELD>
                        </IDENTIFIERXML>
                        <xsl:for-each select="$VPropertyList/Property">
                                <xsl:variable name="V1" select="."/>
                                <xsl:for-each select="@name">
                                    <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                                        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:for-each>
                        </xsl:for-each>
                    </ROW>
                </VW_TemporaryBatch>
            </xsl:for-each>
            <xsl:for-each select="ComponentList/Component">
                <xsl:variable name="VComponentIndex" select="ComponentIndex/."/>
                <xsl:for-each select="Compound">
                    <VW_TemporaryCompound>
                        <ROW>
                            <TEMPCOMPOUNDID>
                                0
                            </TEMPCOMPOUNDID>
                            <TEMPBATCHID>
                                0
                            </TEMPBATCHID>
                            <xsl:for-each select="RegNumber/RegID">
                                <REGID>
                                    <xsl:choose>
                                        <xsl:when test=".=''0''">
                                        </xsl:when>
                                        <xsl:otherwise>
                                            <xsl:value-of select="."/>
                                        </xsl:otherwise>
                                    </xsl:choose>
                                </REGID>
                            </xsl:for-each>
                            <xsl:for-each select="BaseFragment/Structure">
                                <BASE64_CDX>
                                    <xsl:value-of select="Structure"/>
                                </BASE64_CDX>
                                <STRUCTUREID>
                                    <xsl:value-of select="StructureID"/>
                                </STRUCTUREID>
                            </xsl:for-each>
                            <DATECREATED>
                                <xsl:value-of select="DateCreated"/>
                            </DATECREATED>
                            <PERSONCREATED>
                                <xsl:value-of select="PersonCreated"/>
                            </PERSONCREATED>
                            <DATELASTMODIFIED>
                                <xsl:value-of select="DateLastModified"/>
                            </DATELASTMODIFIED>
                            <MOLECULARFORMULA>" "</MOLECULARFORMULA>
                            <FORMULAWEIGHT>" "</FORMULAWEIGHT>
                            <SEQUENCEID>
                                <xsl:value-of select="RegNumber/SequenceID"/>
                            </SEQUENCEID>
                            <xsl:for-each select="PropertyList/Property">
                                <xsl:variable name="V1" select="."/>
                                <xsl:for-each select="@name">
                                    <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                                        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:for-each>
                            </xsl:for-each>
                            <PROJECTXML>
                                <XMLFIELD>
                                    <xsl:copy-of select="$VProjectList"/>
                                </XMLFIELD>
                            </PROJECTXML>
                            <xsl:for-each select="FragmentList">
                                <FRAGMENTXML>
                                    <XMLFIELD>
                                        <xsl:copy-of select="."/>
                                    </XMLFIELD>
                                </FRAGMENTXML>
                            </xsl:for-each>
                            <xsl:for-each select="BatchComponentFragmentList">
                                <BATCHCOMPONENTFRAGMENTXML>
                                    <XMLFIELD>
                                        <xsl:copy-of select="."/>
                                    </XMLFIELD>
                                </BATCHCOMPONENTFRAGMENTXML>
                            </xsl:for-each>
                            <xsl:for-each select="IdentifierList">
                                <IDENTIFIERXML>
                                    <XMLFIELD>
                                        <xsl:copy-of select="."/>
                                    </XMLFIELD>
                                </IDENTIFIERXML>
                            </xsl:for-each>
                            <xsl:for-each select="Structure/NormalizedStructure">
                                <NORMALIZEDSTRUCTURE>
                                    <xsl:copy-of select="."/>
                                </NORMALIZEDSTRUCTURE>
                            </xsl:for-each>
                            <xsl:for-each select="BaseFragment/Structure/UseNormalization">
                                <USENORMALIZATION>
                                    <xsl:value-of select="."/>
                                </USENORMALIZATION>
                            </xsl:for-each>
                            <xsl:for-each select="$VBatchList_Batch/BatchComponentList/BatchComponent[ComponentIndex=$VComponentIndex]">
                                <xsl:for-each select="PropertyList/Property">
                                    <xsl:variable name="V1" select="."/>
                                    <xsl:for-each select="@name">
                                        <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                                        <xsl:choose>
                                            <xsl:when test="$V2 = ''COMMENTS''">
                                                LESS_THAN_SIGN;BATCHCOMPONENTCOMMENTSGREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/BATCHCOMPONENTCOMMENTSGREATER_THAN_SIGN;
                                            </xsl:when>
                                            <xsl:otherwise>
                                                LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;
                                            </xsl:otherwise>
                                        </xsl:choose>
                                    </xsl:for-each>
                                </xsl:for-each>
                            </xsl:for-each>
                        </ROW>
                    </VW_TemporaryCompound>
                </xsl:for-each>
            </xsl:for-each>
        </xsl:template>
    </xsl:stylesheet>
');

BEGIN
    SetSessionParameter;

    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','AXml= '||AXml); $end null;

    LXmlCompReg:=AXml;

    -- Take Out the Structures because XmlType don't suport > 64k.
    --LFragmentXmlList:=TakeOffAndGetClobsList(LXmlCompReg,'<Fragment>','<FragmentList>');
    LFragmentXmlList:=TakeOffAndGetClobslist(LXmlCompReg,'<FragmentList');
    LBatchComponentFragmentXMLList:=TakeOffAndGetClobslist(LXmlCompReg,'<BatchComponentFragmentList');
    LStructuresList:=TakeOffAndGetClobslist(LXmlCompReg,'<Structure ','<Structure','<BaseFragment>');
    --LStructuresList:=TakeOffAndGetClobslist(LXmlCompReg,'<Structure ','<Structure',FALSE,TRUE);
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LStructuresListt= '||LStructuresList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXmlCompReg sin Structures2= '||LXmlCompReg); $end null;
    LNormalizedStructureList:=TakeOffAndGetClobslist(LXmlCompReg,'<NormalizedStructure','<Structure');
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LNormalizedStructureList= '||LNormalizedStructureList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXmlCompReg sin Structures3= '||LXmlCompReg); $end null;
    LStructureAggregationList:=TakeOffAndGetClobslist(LXmlCompReg,'<StructureAggregation');
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LStructureAggregationList= '||LStructureAggregationList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXmlCompReg sin Structures4= '||LXmlCompReg); $end null;

    -- Get the xml
    LXmlTables:=XmlType.CreateXML(LXmlCompReg);

    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXmlTables= '||LXmlTables.getClobVal()); $end null;

    -- Build a new formatted Xml
    LXslTablesTransformed:=LXmlTables.Transform(LXslTables);
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXslTablesTransformed= '||LXslTablesTransformed.getClobVal()); $end null;

    --Get ID
    LTempBatchID:=LXmlTables.Extract('node()[1]/ID/text()').getNumberVal();
    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LTempBatchID= '||LTempBatchID); $end null;

    --Look over Xml searching each Table and insert the rows of it.
    SELECT XMLSequence(LXslTablesTransformed.Extract('/node()'))
        INTO LXmlSequenceType
        FROM DUAL;

    FOR LIndex IN  LXmlSequenceType.FIRST..LXmlSequenceType.LAST LOOP
        --Search each Table
        LXmlTypeRows:=LXmlSequenceType(LIndex);
        LTableName:= LXmlTypeRows.GetRootElement();

        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LTableName= '||LTableName||CHR(10)||' LXmlRows= '||LXmlRows); $end null;

        --Customization for each View - Use of Sequences and parser for Strcutres
        CASE UPPER(LTableName)
            WHEN 'VW_TEMPORARYBATCH' THEN
                BEGIN
                    --Use of Sequences
                    IF NVL(LTempBatchID,0)=0 THEN
                      SELECT SEQ_TEMPORARY_BATCH.NEXTVAL INTO LTempBatchID FROM DUAL;
                    END IF;

                    ATempID:=LTempBatchID;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','VW_TEMPORARYBATCH LTempBatchID= '||LTempBatchID); $end null;

                    LStructureAggregationValue:=TakeOffAndGetClob(LStructureAggregationList,'Clob');

                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LStructureAggregationValue= '||LStructureAggregationValue); $end null;

                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/TEMPBATCHID/text()'     ,LTempBatchID
                                                 ,'/node()/ROW/DATECREATED/text()'     ,TO_CHAR(SYSDATE)
                                                 ,'/node()/ROW/DATELASTMODIFIED/text()',TO_CHAR(SYSDATE))
                        INTO LXmlTypeRows
                        FROM dual;
                    --Project List section
                    BEGIN
                        SELECT XMLSequence(LXmlTables.Extract('/node()/ProjectList/Project/ProjectID'))
                            INTO LProjectsSequenceType
                            FROM DUAL
                            WHERE ExistsNode(LXmlTables,'/node()/ProjectList/Project/ProjectID')=1;

                         $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LProjectsSequenceType= '||LProjectsSequenceType(1).getclobVal()); $end null;

                         FOR LProjectsIndex IN  LProjectsSequenceType.FIRST..LProjectsSequenceType.LAST LOOP
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LProjectsSequenceType= '||LProjectsSequenceType(LProjectsIndex).getclobVal()); $end null;
                            LProjectID:=LProjectsSequenceType(LProjectsIndex).Extract('ProjectID/text()').getNumberVal();
                            IF LProjectID IS NOT NULL THEN
                                SELECT Name,Description
                                    INTO LProjectName,LProjectDescription
                                    FROM VW_Project WHERE ProjectID=LProjectID;
                                 $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LProjectName= '||LProjectName); $end null;

                                SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/PROJECTXML/XMLFIELD/ProjectList/Project[ProjectID='||LProjectID||']/ProjectID'
                                                             ,XmlType('<ProjectID name="'||LProjectName||'" description="'||LProjectDescription||'">'||LProjectID||'</ProjectID>'))
                                    INTO LXmlTypeRows
                                    FROM dual;
                            END IF;
                         END LOOP;
                     EXCEPTION
                        WHEN NO_DATA_FOUND THEN NULL;
                     END;
                     --Project Batch List section
                    BEGIN
                        SELECT XMLSequence(LXmlTables.Extract('/node()/BatchList/Batch/ProjectList/Project/ProjectID'))
                            INTO LProjectsSequenceType
                            FROM DUAL
                            WHERE ExistsNode(LXmlTables,'/node()/BatchList/Batch/ProjectList/Project/ProjectID')=1;

                         $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LProjectsSequenceType (Batch)= '||LProjectsSequenceType(1).getclobVal()); $end null;

                         FOR LProjectsIndex IN  LProjectsSequenceType.FIRST..LProjectsSequenceType.LAST LOOP
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LProjectsSequenceType(Batch)= '||LProjectsSequenceType(LProjectsIndex).getclobVal()); $end null;
                            LProjectID:=LProjectsSequenceType(LProjectsIndex).Extract('ProjectID/text()').getNumberVal();
                            IF LProjectID IS NOT NULL THEN
                                SELECT Name,Description
                                    INTO LProjectName,LProjectDescription
                                    FROM VW_Project WHERE ProjectID=LProjectID;
                                 $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LProjectName (Batch)= '||LProjectName); $end null;

                                SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/PROJECTXMLBATCH/XMLFIELD/ProjectList/Project[ProjectID='||LProjectID||']/ProjectID'
                                                             ,XmlType('<ProjectID name="'||LProjectName||'" description="'||LProjectDescription||'">'||LProjectID||'</ProjectID>'))
                                    INTO LXmlTypeRows
                                    FROM dual;
                            END IF;
                         END LOOP;
                     EXCEPTION
                        WHEN NO_DATA_FOUND THEN NULL;
                     END;
                     --Identifier List section
                     BEGIN
                        SELECT XMLSequence(LXmlTables.Extract('/node()/IdentifierList/Identifier/IdentifierID'))
                            INTO LIdentifiersSequenceType
                            FROM DUAL
                            WHERE ExistsNode(LXmlTables,'/node()/IdentifierList/Identifier/IdentifierID')=1;

                         $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LIdentifiersSequenceType= '||LIdentifiersSequenceType(1).getclobVal()); $end null;

                         FOR LIdentifiersIndex IN  LIdentifiersSequenceType.FIRST..LIdentifiersSequenceType.LAST LOOP
                            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LIdentifiersSequenceType= '||LIdentifiersSequenceType(LIdentifiersIndex).getclobVal()); $end null;
                            LIdentifierID:=LIdentifiersSequenceType(LIdentifiersIndex).Extract('IdentifierID/text()').getNumberVal();
                            IF LIdentifierID IS NOT NULL THEN
                                SELECT Name,Description
                                    INTO LIdentifierName,LIdentifierDescription
                                    FROM VW_IdentifierType WHERE ID=LIdentifierID;
                                 $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LIdentifierName= '||LIdentifierName); $end null;

                                SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/IDENTIFIERXML/XMLFIELD/IdentifierList/Identifier[IdentifierID='||LIdentifierID||']/IdentifierID'
                                                             ,XmlType('<IdentifierID name="'||LIdentifierName||'" description="'||LIdentifierDescription||'">'||LIdentifierID||'</IdentifierID>'))
                                    INTO LXmlTypeRows
                                    FROM dual;
                            END IF;
                         END LOOP;
                     EXCEPTION
                        WHEN NO_DATA_FOUND THEN NULL;
                     END;
                END;
            WHEN 'VW_TEMPORARYCOMPOUND' THEN
                BEGIN
                    SELECT SEQ_TEMPORARY_COMPOUND.NEXTVAL INTO LTempCompoundID FROM DUAL;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LTempCompoundID= '||LTempCompoundID); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LStructuresList='||LStructuresList); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LFragmentXmlList='||LFragmentXmlList); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LBatchComponentFragmentXMLList='||LBatchComponentFragmentXMLList); $end null;

                    LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');

                    LStructureID:= LXmlTypeRows.extract('/VW_TemporaryCompound/ROW/STRUCTUREID/text()').getNumberVal();
                    IF LStructureID='-1' THEN
                        SELECT Structure
                            INTO LStructureValue
                            FROM VW_Structure
                            WHERE StructureID=-1;
                    END IF;

                    LFragmentXmlValue:='<FragmentList>'||TakeOffAndGetClob(LFragmentXmlList,'Clob')||'</FragmentList>';
                    LBatchCompFragmentXMLValue:='<BatchComponentFragmentList>'||TakeOffAndGetClob(LBatchComponentFragmentXMLList,'Clob')||'</BatchComponentFragmentList>';
                    LNormalizedStructureValue:=TakeOffAndGetClob(LNormalizedStructureList,'Clob');

                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LStructureValue= '|| LStructureValue ); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LFragmentXmlValue= '|| LFragmentXmlValue ); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LBatchCompFragmentXMLValue= '|| LBatchCompFragmentXMLValue ); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LNormalizedStructureValue= '|| LNormalizedStructureValue ); $end null;

                    IF LStructureValue IS NOT NULL THEN
                        SELECT cscartridge.formula(LStructureValue,''),cscartridge.molweight(LStructureValue)
                            INTO LMolecularFormula,LFormulaWeight
                            FROM DUAL;
                      $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LMolecularFormula= '|| LMolecularFormula||' LXmlRows:'||LXmlRows ); $end null;
                    END IF;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LXmlTypeRows.getClobVal1= '|| LXmlTypeRows.getClobVal ); $end null;
                    SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/TEMPCOMPOUNDID/text()'  ,LTempCompoundID
                                                 ,'/node()/ROW/TEMPBATCHID/text()'     ,LTempBatchID
                                                 ,'/node()/ROW/MOLECULARFORMULA/text()',LMolecularFormula
                                                 ,'/node()/ROW/FORMULAWEIGHT/text()'   ,LFormulaWeight
                                                 ,'/node()/ROW/DATECREATED/text()'     ,TO_CHAR(SYSDATE)
                                                 ,'/node()/ROW/DATELASTMODIFIED/text()',TO_CHAR(SYSDATE))
                        INTO LXmlTypeRows
                        FROM dual;
                    $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' LMolecularFormula='||LMolecularFormula||' LXmlTypeRows.getClobVal2= '|| LXmlTypeRows.getClobVal ); $end null;
                END;
            ELSE
                --Build Message Logs
                LMessage := LMessage || ' "' || LTableName || '" table stranger.'||CHR(13);

        END CASE;

        LXmlRows:=LXmlTypeRows.getClobVal;
        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXmlRows1= '||LXmlRows); $end null;

        --PropertyList fields: Replace '&lt;' and '&lt;'  by '<'' and '>''. I can't to do it using "XmlTransform"
        LXmlRows:=replace(replace(LXmlRows,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');

        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXmlRows2= '||LXmlRows); $end null;

        --XML fields: Replace '<' and '>'  by '&lt;' and '&lt;'' to the XML fields . Necesary to save a xml field using insertXML. I can't to do it using "XmlTransform".
        LPosTagBegin:=1;
        LOOP
            LPosTagBegin := INSTR(LXmlRows,LTagXmlFieldBegin,LPosTagBegin);
            LPosTagEnd   := INSTR(LXmlRows,LTagXmlFieldEnd,LPosTagBegin);

              $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LOOP LXmlRows='||LXmlRows||' LPosTagBegin='||LPosTagBegin||' LPosTagEnd='||LPosTagEnd); $end null;

        EXIT WHEN (LPosTagBegin=0) or (LPosTagEnd=0);
            LXmlField:= SUBSTR(LXmlRows,LPosTagBegin+LENGTH(LTagXmlFieldBegin),LPosTagEnd-(LPosTagBegin+LENGTH(LTagXmlFieldBegin)));

            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','LXmlField='||LXmlField||' '||LPosTagBegin||' '||LTagXmlFieldEnd||LPosTagEnd); $end null;

            LXmlField:= replace(replace(LXmlField,'<','&lt;') ,'>','&gt;');

            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' Before LXmlRows='||LXmlRows); $end null;
            LXmlRows:= SUBSTR(LXmlRows,1,LPosTagBegin-1)||LXmlField|| SUBSTR(LXmlRows,LPosTagEnd+LENGTH(LTagXmlFieldEnd),LENGTH(LXmlRows)) ;

            $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',' After LXmlRows='||LXmlRows); $end null;
        END LOOP;


        --Create the Table Context
        LinsCtx := DBMS_XMLSTORE.newContext(LTableName);
        DBMS_XMLSTORE.clearUpdateColumnList(LinsCtx);

        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp',LTableName||'->'||LXmlRows); $end null;

        SELECT XMLSequence(XmlType(LXmlRows).Extract('/node()/node()/node()'))
            INTO LXmlSequenceTypeField
            FROM DUAL;
        FOR LIndex IN  LXmlSequenceTypeField.FIRST..LXmlSequenceTypeField.LAST LOOP
            DBMS_XMLSTORE.SetupDateColumn (LinsCtx, UPPER(LXmlSequenceTypeField(LIndex).GetRootElement()));
        END LOOP;

        --Insert Rows and get count it inserted
        LRowsInserted := DBMS_XMLSTORE.insertXML(LinsCtx, LXmlRows );

        --Build Message Logs
        LMessage:=LMessage || ' ' || cast (LRowsInserted  as string) || ' Row/s Inserted on "'||LTableName||'".';

        --Close the Table Context
        DBMS_XMLSTORE.closeContext(LinsCtx);

        --When structure is more length than 4000 we can't use insertXML
       $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','BEGIN UPDATE VW_TEMPORARYCOMPOUN'); $end null;

        IF UPPER(LTableName)='VW_TEMPORARYCOMPOUND' THEN
            IF LBatchCompFragmentXMLValue IS NOT NULL THEN
                IF LStructureValue IS NOT NULL AND LFragmentXmlValue IS NOT NULL THEN
                    UPDATE VW_TEMPORARYCOMPOUND
                        SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXMLValue, NormalizedStructure=LNormalizedStructureValue
                        WHERE TempCompoundID=LTempCompoundID;
                ELSIF LStructureValue IS NOT NULL THEN
                    UPDATE VW_TEMPORARYCOMPOUND
                      SET BASE64_CDX=LStructureValue, BatchCompFragmentXML=LBatchCompFragmentXMLValue,NormalizedStructure=LNormalizedStructureValue
                      WHERE TempCompoundID=LTempCompoundID;
                ELSIF LFragmentXmlValue IS NOT NULL THEN
                    UPDATE VW_TEMPORARYCOMPOUND
                      SET FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXMLValue,NormalizedStructure=LNormalizedStructureValue
                      WHERE TempCompoundID=LTempCompoundID;
                END IF;
            ELSE
                IF LStructureValue IS NOT NULL AND LFragmentXmlValue IS NOT NULL THEN
                    UPDATE VW_TEMPORARYCOMPOUND
                        SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                        WHERE TempCompoundID=LTempCompoundID;
                ELSIF LStructureValue IS NOT NULL THEN
                    UPDATE VW_TEMPORARYCOMPOUND
                      SET BASE64_CDX=LStructureValue, NormalizedStructure=LNormalizedStructureValue
                      WHERE TempCompoundID=LTempCompoundID;
                ELSIF LFragmentXmlValue IS NOT NULL THEN
                    UPDATE VW_TEMPORARYCOMPOUND
                      SET FRAGMENTXML=LFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                      WHERE TempCompoundID=LTempCompoundID;
                END IF;
            END IF;
       ELSIF UPPER(LTableName)='VW_TEMPORARYBATCH' THEN
          IF LStructureAggregationValue IS NOT NULL THEN
            UPDATE VW_TEMPORARYBATCH
                SET StructureAggregation=LStructureAggregationValue
                WHERE TempBatchID=LTempBatchID;
          END IF;
       END IF;

       $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp', 'END UDAPTE VW_TEMPORARYCOMPOUN'||CHR(13)||LMessage); $end null;

    END LOOP;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        LMessage := LMessage || ' ' || cast(0 as string) || ' Row/s Inserted on "' || LTableName || '".';
        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegTmp','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK||'Process Log:'||CHR(13)||LMessage);$end null;
        RAISE_APPLICATION_ERROR(eCreateMultiCompoundRegTmp,CHR(13)||DBMS_UTILITY.FORMAT_ERROR_STACK||'Process Log:'||CHR(13)||LMessage||CHR(13));
    END;
END;

FUNCTION  GetFragmentXML( ATempCompundID  in Number) RETURN CLOB IS
    LFragmentXML  CLOB;

    CURSOR C_Fragments(ATempCompoundID in VW_TEMPORARYCOMPOUND.TempCompoundID%type) IS
        SELECT To_Clob('<Fragment><FragmentID>'||Extract(value(FragmentXML), '/FragmentID/text()').GetStringVal()||'</FragmentID><Code>'||Code||'</Code><Description>'||Description||'</Description><DateCreated>'||CREATED||'</DateCreated><DateLastModified>'||MODIFIED||'</DateLastModified><Structure><Structure molWeight="'||MOLWEIGHT||'" formula="'||FORMULA||'">'||STRUCTURE||'</Structure></Structure></Fragment>') FragmentXML
            FROM VW_FRAGMENT F,Table(SELECT XMLSequence(XmlType(TB.FragmentXML).Extract('/FragmentList/Fragment/FragmentID')) FROM VW_TEMPORARYCOMPOUND TB WHERE ATempCompoundID=TempCompoundID AND TB.FragmentXML IS NOT NULL) FragmentXML
            WHERE F.FragmentID(+)=Extract(value(FragmentXML), '/FragmentID/text()').GetStringVal();
BEGIN
    --** Get Fragment**

    LFragmentXML:='<FragmentList>';
    FOR R_Fragment IN C_Fragments(ATempCompundID) LOOP
        LFragmentXML:=LFragmentXML||R_Fragment.FragmentXML;
    END LOOP;
    LFragmentXML:=LFragmentXML||'</FragmentList>';

    RETURN LFragmentXML;
END;

PROCEDURE RetrieveMultiCompoundRegTmp( ATempID  in Number, AXml out NOCOPY clob) IS

       -- Autor: Fari
       -- Date:11-jun-077
       -- Object:
       -- Pending:
            --Optimize use Replace with XSLT


    LQryCtx                   DBMS_XMLGEN.ctxHandle;
    LXmlMixture               CLOB;
    LXmlComponent             CLOB;
    LXmlQuery                 CLOB;
    LXmlBatch                 CLOB;
    LXml                      CLOB;
    LSelect                   CLOB;
    LXmlBatchComponent        CLOB;

    LXmlTables                XmlType;
    LXslTablesTransformed     XmlType;
    LXmlResult                XmlType;
    LMessage                  CLOB:='';
    LStructuresList           CLOB;
    LNormalizedStructureList  CLOB;
    LStructureAggregationList CLOB;
    LFragmentXmlList          CLOB;

    LCoeObjectConfigField     XmlType;

    LIndex                    Number;
    LXmlRows                  CLOB;

    LMixtureFields            CLOB;
    LCompoundFields           CLOB;
    LBatchFields              CLOB;
    LBatchComponentFields     CLOB;

    LProjectXML               CLOB;
    LProjectXMLBatch          CLOB;
    LIdentifierXML            CLOB;


    LXslTables XmlType:=XmlType.CreateXml('
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
  <xsl:template match="/MultiCompoundRegistryRecord">
    <MultiCompoundRegistryRecord>
      <xsl:attribute name="SameBatchesIdentity"><xsl:value-of select="@SameBatchesIdentity"/></xsl:attribute>
      <ID>
        <xsl:for-each select="Batch/ROW/TEMPBATCHID">
          <xsl:value-of select="."/>
        </xsl:for-each>
      </ID>
      <DateCreated></DateCreated>
      <DateLastModified></DateLastModified>
      <PersonCreated></PersonCreated>
      <StructureAggregation>
          <xsl:for-each select="Batch/ROW/STRUCTUREAGGREGATION">
            <xsl:value-of select="."/>
          </xsl:for-each>
      </StructureAggregation>
      <RegNumber>
              <RegID></RegID>
              <RootNumber></RootNumber>
              <SequenceNumber></SequenceNumber>
              <RegNumber></RegNumber>
              <SequenceID>
                <xsl:for-each select="Batch/ROW/SEQUENCEID">
                     <xsl:value-of select="."/>
                </xsl:for-each>
              </SequenceID>
      </RegNumber>
      <xsl:for-each select="Batch/ROW[1]/IDENTIFIERXML">
            <xsl:value-of select="."/>
      </xsl:for-each>
      <PropertyList>
        <xsl:for-each select="Mixture/ROW[1]/PropertyList">
          <xsl:for-each select="node()">
             LESS_THAN_SIGN;Property name="<xsl:variable name="V3" select="name()"/><xsl:value-of select="name()"/>"GREATER_THAN_SIGN;<xsl:value-of select="."/>LESS_THAN_SIGN;/PropertyGREATER_THAN_SIGN;
          </xsl:for-each>
        </xsl:for-each>
      </PropertyList>
      <xsl:for-each select="Batch/ROW[1]/PROJECTXML">
          <xsl:value-of select="."/>
      </xsl:for-each>
      <ComponentList>
       <xsl:for-each select="Compound/ROW">
         <Component>
           <ID>
           </ID>
           <ComponentIndex>
             <xsl:for-each select="COMPONENTINDEX">
               <xsl:value-of select="."/>
             </xsl:for-each>
           </ComponentIndex>
           <Compound>
              <CompoundID>
                <xsl:for-each select="TEMPCOMPOUNDID">
                  <xsl:value-of select="."/>
                </xsl:for-each>
              </CompoundID>
              <DateCreated>
                <xsl:for-each select="DATECREATED">
                  <xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
                  <xsl:value-of select="."/>
                </xsl:for-each>
              </DateCreated>
              <PersonCreated>
                <xsl:for-each select="PERSONCREATED">
                  <xsl:value-of select="."/>
                </xsl:for-each>
              </PersonCreated>
              <DateLastModified>
                 <xsl:for-each select="DATELASTMODIFIED">
                   <xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
                   <xsl:value-of select="."/>
                 </xsl:for-each>
               </DateLastModified>
               <xsl:for-each select="PropertyList">
                 <PropertyList>
                  <xsl:for-each select="node()">
                    LESS_THAN_SIGN;Property name="<xsl:value-of select="name()"/>"GREATER_THAN_SIGN;<xsl:value-of select="."/>LESS_THAN_SIGN;/PropertyGREATER_THAN_SIGN;</xsl:for-each>
                 </PropertyList>
               </xsl:for-each>
               <RegNumber>
                 <RegID>
                    <xsl:for-each select="REGID">
                      <xsl:value-of select="."/>
                    </xsl:for-each>
                 </RegID>
                 <RootNumber>
                   <xsl:for-each select="ROOTNUMBER">
                    <xsl:value-of select="."/>
                   </xsl:for-each>
                 </RootNumber>
                 <SequenceNumber>
                   <xsl:for-each select="SEQUENCENUMBER">
                     <xsl:value-of select="."/>
                   </xsl:for-each>
                 </SequenceNumber>
                 <RegNumber>
                   <xsl:for-each select="REGNUMBER">
                    <xsl:value-of select="."/>
                   </xsl:for-each>
                 </RegNumber>
                 <SequenceID>
                   <xsl:for-each select="SEQUENCEID">
                     <xsl:value-of select="."/>
                   </xsl:for-each>
                 </SequenceID>
               </RegNumber>
              <BaseFragment>
                <Structure>
                  <StructureID>
                      <xsl:for-each select="STRUCTUREID">
                        <xsl:value-of select="."/>
                      </xsl:for-each>
                  </StructureID>
                  <StrucureFormat>
                    <xsl:for-each select="STRUCTUREFORMAT">
                      <xsl:value-of select="."/>
                    </xsl:for-each>
                  </StrucureFormat>

                  <xsl:variable name="VROW" select="."/>
                  <Structure>
                    <xsl:for-each select="STRUCTURE">
                      <xsl:for-each select="$VROW/FORMULAWEIGHT">
                        <xsl:attribute name="molWeight">
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                      </xsl:for-each>
                      <xsl:for-each select="$VROW/MOLECULARFORMULA">
                        <xsl:attribute name="formula">
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                      </xsl:for-each>

                      <xsl:value-of select="."/>
                    </xsl:for-each>
                  </Structure>
                  <NormalizedStructure>
                        <xsl:for-each select="NORMALIZEDSTRUCTURE">
                            <xsl:value-of select="."/>
                        </xsl:for-each>
                  </NormalizedStructure>
                  <UseNormalization>
                        <xsl:for-each select="USENORMALIZATION">
                            <xsl:value-of select="."/>
                        </xsl:for-each>
                  </UseNormalization>
                </Structure>
              </BaseFragment>
              <xsl:for-each select="FRAGMENTXML">
                <xsl:value-of select="."/>
              </xsl:for-each>
              <xsl:for-each select="IDENTIFIERXML">
                <xsl:value-of select="."/>
              </xsl:for-each>
            </Compound>
          </Component>
        </xsl:for-each>
       </ComponentList>
       <BatchList>
        <xsl:for-each select="Batch/ROW">
          <Batch>
            <BatchID>
              <xsl:for-each select="TEMPBATCHID">
                <xsl:value-of select="."/>
              </xsl:for-each>
            </BatchID>
            <BatchNumber>
              <xsl:for-each select="BATCHNUMBER">
                <xsl:value-of select="."/>
              </xsl:for-each>
            </BatchNumber>
            <DateCreated>
              <xsl:for-each select="DATECREATED">
                <xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
                <xsl:value-of select="."/>
              </xsl:for-each>
            </DateCreated>
            <xsl:variable name="VROW3" select="."/>
            <PersonCreated>
              <xsl:for-each select="PERSONCREATED">
                <xsl:for-each select="$VROW3/PERSONCREATEDDISPLAY">
                  <xsl:attribute name="displayName">
                    <xsl:value-of select="."/>
                  </xsl:attribute>
                </xsl:for-each>
                 <xsl:value-of select="."/>
              </xsl:for-each>
            </PersonCreated>
            <DateLastModified>
              <xsl:for-each select="DATELASTMODIFIED">
                <xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
                <xsl:value-of select="."/>
              </xsl:for-each>
            </DateLastModified>
            <xsl:for-each select="PROJECTXMLBATCH">
                <xsl:value-of select="."/>
            </xsl:for-each>
            <xsl:for-each select="PropertyList">
              <PropertyList><xsl:for-each select="node()">
    LESS_THAN_SIGN;Property name="<xsl:variable name="V3" select="name()"/><xsl:value-of select="name()"/>"GREATER_THAN_SIGN;<xsl:variable name="V4" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/><xsl:choose>
                                                    <xsl:when test="$V3 = ''DELIVERYDATE'' and . != ''''">
    <xsl:value-of select="$V4"/></xsl:when>
                                                    <xsl:when test="$V3  = ''DATEENTERED'' and . != ''''">
    <xsl:value-of select="$V4"/></xsl:when>
                                                    <xsl:otherwise><xsl:value-of select="."/></xsl:otherwise></xsl:choose>LESS_THAN_SIGN;/PropertyGREATER_THAN_SIGN;</xsl:for-each>
              </PropertyList>
            </xsl:for-each>
            <BatchComponentList>
              <xsl:for-each select="BatchComponent/ROW">
                 <BatchComponent>
                    <ID />
                    <BatchID>
                       <xsl:for-each select="TEMPBATCHID">
                          <xsl:value-of select="."/>
                       </xsl:for-each>
                    </BatchID>
                    <CompoundID>
                       <xsl:for-each select="TEMPCOMPOUNDID">
                          <xsl:value-of select="."/>
                       </xsl:for-each>
                    </CompoundID>
                    <ComponentIndex>
                       <xsl:for-each select="COMPONENTINDEX">
                          <xsl:value-of select="."/>
                       </xsl:for-each>
                    </ComponentIndex>
                    <xsl:for-each select="PropertyList">
                       <PropertyList>
                          <xsl:for-each select="node()">
                             LESS_THAN_SIGN;Property name="<xsl:variable name="V3" select="name()"/><xsl:value-of select="name()"/>"GREATER_THAN_SIGN;<xsl:value-of select="."/>LESS_THAN_SIGN;/PropertyGREATER_THAN_SIGN;
                          </xsl:for-each>
                       </PropertyList>
                    </xsl:for-each>
                    <xsl:for-each select="BATCHCOMPFRAGMENTXML">
                      <xsl:value-of select="."/>
                    </xsl:for-each>
                  </BatchComponent>
                </xsl:for-each>
             </BatchComponentList>
          </Batch>
       </xsl:for-each>
     </BatchList>
   </MultiCompoundRegistryRecord>
 </xsl:template>
</xsl:stylesheet>
');

    CURSOR C_Identifiers(ATempBatchID in VW_TemporaryBatch.TempBatchID%type) IS
        SELECT To_Clob('<Identifier><IdentifierID Name="'||Name||'" Description="'||Description||'" Active="'||Active||'">'||Extract(value(IdentifierXML), '/IdentifierID/text()').GetStringVal()||'</IdentifierID><InputText>'||Extract(value(IdentifierXML), '/Identifier/InputText/text()').GetStringVal()||'</InputText></Identifier>') IdentifierXML
            FROM VW_IdentifierType IT,Table(SELECT XMLSequence(XmlType(TB.IdentifierXML).Extract('/IdentifierList/Identifier/IdentifierID')) FROM VW_TemporaryBatch TB WHERE TempBatchID=ATempBatchID AND TB.IdentifierXML IS NOT NULL) IdentifierXML
            WHERE IT.ID(+)=Extract(value(IdentifierXML), '/IdentifierID/text()').GetStringVal();

    CURSOR C_Projects(ATempBatchID in VW_TemporaryBatch.TempBatchID%type) IS
        SELECT To_Clob('<Project><ProjectID Name="'||Name||'" Description="'||Description||'" Active="'||Active||'">'||Extract(value(ProjectXML), '/ProjectID/text()').GetStringVal()||'</ProjectID></Project>') ProjectXML
            FROM VW_Project P,Table(SELECT XMLSequence(XmlType(TB.ProjectXML).Extract('/ProjectList/Project/ProjectID')) FROM VW_TemporaryBatch TB WHERE TempBatchID=ATempBatchID AND TB.ProjectXML IS NOT NULL) ProjectXML
            WHERE P.ProjectID(+)=Extract(value(ProjectXML), '/ProjectID/text()').GetStringVal();

    CURSOR C_ProjectsBatch(ATempBatchID in VW_TemporaryBatch.TempBatchID%type) IS
        SELECT To_Clob('<Project><ProjectID Name="'||Name||'" Description="'||Description||'" Active="'||Active||'">'||Extract(value(ProjectXMLBatch), '/ProjectID/text()').GetStringVal()||'</ProjectID></Project>') ProjectXMLBatch
            FROM VW_Project P,Table(SELECT XMLSequence(XmlType(TB.ProjectXMLBatch).Extract('/ProjectList/Project/ProjectID')) FROM VW_TemporaryBatch TB WHERE TempBatchID=ATempBatchID AND TB.ProjectXMLBatch IS NOT NULL) ProjectXMLBatch
            WHERE P.ProjectID(+)=Extract(value(ProjectXMLBatch), '/ProjectID/text()').GetStringVal() AND ExistsNode(value(ProjectXMLBatch),'/ProjectID/text()')=1;

    PROCEDURE AddNullFields(AFields IN CLOB,AXml IN OUT NOCOPY CLOB,ATagSearch IN CLOB,ABeginXml IN CLOB) IS
        LPosBegin                 NUMBER;
        LPoslast                  NUMBER;
        LField                    VARCHAR2(100);
        LFields                   CLOB;

        LPosField                 NUMBER;

    BEGIN
        LFields:=AFields||',';
        LPosBegin:=0;
        LPoslast:=1;
        LOOP
            LPosBegin:=INSTR(LFields,',',LPoslast);
            LField:=UPPER(SUBSTR(LFields,LPoslast,LPosBegin-LPoslast));
            LPoslast:=LPosBegin+1;
        EXIT WHEN LField IS NULL;
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LField:'||LField||' AXml='||AXml||' INSTR(AXml,ABeginXml)='||INSTR(AXml,ABeginXml)); $end null;
            LPosField:=INSTR(AXml, '<'||LField||'>',INSTR(AXml,ABeginXml)+1);
            IF LPosField=0 OR LPosField > INSTR(AXml,ATagSearch) THEN
               AXml:=REPLACE(AXml,ATagSearch,' <'||LField||'/>'||CHR(10)||' '||ATagSearch);
            END IF;
        END LOOP;
    END;

BEGIN
    SetSessionParameter;
    --Get Query XML or Get empty template xml
    IF (NVL(ATempID,0)>0) THEN
        --**Get the PropertyList Fields from the XML field
        SELECT XmlTransform(XmlType.CreateXml(XML),XmlType.CreateXml('
          <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
             <xsl:template match="/MultiCompoundRegistryRecord">
                <xsl:for-each select="ComponentList/Component/Compound/PropertyList/Property">TC.<xsl:value-of select="@name"/>,</xsl:for-each>
             </xsl:template>
          </xsl:stylesheet>')).GetStringVal()
        INTO LCompoundFields
        FROM COEOBJECTCONFIG
        WHERE ID=2;

        --Get Compound**
        LSelect:='SELECT TC.TempCompoundID,
                    TC.TempBatchID,
                    TC.FORMULAWEIGHT, TC.MOLECULARFORMULA,TC.PERSONCREATED, TC.DATELASTMODIFIED, TC.DATECREATED,TC.SequenceID,
                    TC.BASE64_CDX STRUCTURE,
                    ''PropertyListBegin'' Aux,'||LCompoundFields||'''PropertyListEnd'' Aux,
                    CompoundRegistry.GetFragmentXML(TC.TempCompoundID) FRAGMENTXML,
                    TC.IDENTIFIERXML,
                    TC.REGID,
                    R.REGNUMBER,
                    DECODE(NVL(R.RegID,0),0,-TC.TempCompoundID,-C.COMPOUNDID) COMPONENTINDEX,
                    TC.NormalizedStructure,
                    TC.UseNormalization,
                    TC.StructureID
                 FROM VW_TEMPORARYCOMPOUND TC, VW_RegistryNumber R, VW_Compound C
                 WHERE TC.RegID = C.RegID(+) AND TC.RegID=R.RegID(+) AND TC.TempBatchID='||ATempID||' ORDER BY TC.TempCompoundID';

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LSelect='||LSelect); $end null;

        LQryCtx:=DBMS_XMLGEN.newContext(LSelect);

        LXmlQuery:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
        DBMS_XMLGEN.closeContext(LQryCtx);
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LXmlQuery:'|| LXmlQuery); $end null;
        LStructuresList:=TakeOffAndGetClobslist(LXmlQuery,'<STRUCTURE>',NULL,NULL,FALSE);
        LNormalizedStructureList:=TakeOffAndGetClobslist(LXmlQuery,'<NORMALIZEDSTRUCTURE>',NULL,NULL,FALSE);
        LFragmentXMLList:=TakeOffAndGetClobslist(LXmlQuery,'<FRAGMENTXML>',NULL,NULL,FALSE);

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LStructuresList:'|| LStructuresList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LFragmentXMLList:'|| LFragmentXMLList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LXmlQuery SIN ESTRUCTURA:'|| LXmlQuery); $end null;

        LCompoundFields:=SUBSTR(LCompoundFields,4,LENGTH(LCompoundFields));
        LCompoundFields:=Replace(LCompoundFields,'TC.','');

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LCompoundFields II='||LCompoundFields); $end null;

        IF LXmlQuery IS NULL THEN
             RAISE_APPLICATION_ERROR(eGenericException,'No rows returned.'||CHR(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END IF;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','Replace compound LXmlQuery='||LXmlQuery); $end null;

        LIndex:=0;
        LOOP
            --Search each Compounds
            LIndex:=LIndex+1;
            SELECT XmlType(LXmlQuery).extract('/ROWSET/ROW['||LIndex||']').getClobVal()
              INTO LXmlRows
              FROM dual;
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','erplace compound  LXmlRows='||LXmlRows); $end null;

        EXIT WHEN LXmlRows IS NULL;
            LXmlRows:=Replace(LXmlRows,'<AUX>PropertyListBegin</AUX>','<PropertyList>');
            AddNullFields(LCompoundFields,LXmlRows,'<AUX>PropertyListEnd</AUX>','<COMPOUND>');
            LXmlRows:=Replace(LXmlRows,'<AUX>PropertyListEnd</AUX>','</PropertyList>');
            LXmlComponent:=LXmlComponent || LXmlRows;
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','compound LXmlComponent='||LXmlComponent); $end null;
        END LOOP;

        LXmlComponent:='<MultiCompoundRegistryRecord '||'SameBatchesIdentity="'||CoeDB.ConfigurationManager.RetrieveParameter('Registration','SameBatchesIdentity')||'"'||'> <Compound>'||LXmlComponent||'</Compound>';

        --**Get the PropertyList Fields from the XML field (Mixture)
        SELECT XmlTransform(XmlType.CreateXml(XML),XmlType.CreateXml('
            <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
              <xsl:template match="/MultiCompoundRegistryRecord">
                <xsl:for-each select="PropertyList/Property">
                    <xsl:value-of select="@name"/>,</xsl:for-each>
              </xsl:template>
            </xsl:stylesheet>')).GetStringVal()
          INTO LMixtureFields
          FROM COEOBJECTCONFIG
          WHERE ID=2;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LMixtureFields='||LMixtureFields); $end null;
        --**Get Mixture**
        LSelect:='SELECT
                      ''BatchPropertyListBegin'' Aux,'||LMixtureFields||'''BatchPropertyListEnd'' Aux
                  FROM VW_TEMPORARYBATCH
                  WHERE TempBatchID='||ATempID;

        LQryCtx:=DBMS_XMLGEN.newContext(LSelect);
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LSelect='||LSelect); $end null;
        LXmlMixture:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
        DBMS_XMLGEN.closeContext(LQryCtx);

        LXmlMixture:=Replace(LXmlMixture,'<ROWSET>','<Mixture>');
        LXmlMixture:=Replace(LXmlMixture,'<AUX>BatchPropertyListBegin</AUX>','<PropertyList>');
        AddNullFields(LMixtureFields,LXmlMixture,'<AUX>BatchPropertyListEnd</AUX>','<Mixture>');
        LXmlMixture:=Replace(LXmlMixture,'<AUX>BatchPropertyListEnd</AUX>','</PropertyList>');
        LXmlMixture:=Replace(LXmlMixture,'</ROWSET>','</Mixture>');
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LXmlMixture='||LXmlMixture); $end null;

        --**Get the PropertyList Fields from the XML field
        SELECT XmlTransform(XmlType.CreateXml(XML),XmlType.CreateXml('
            <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
              <xsl:template match="/MultiCompoundRegistryRecord">
                <xsl:for-each select="BatchList/Batch/PropertyList/Property">
                    <xsl:value-of select="@name"/>,</xsl:for-each>
              </xsl:template>
            </xsl:stylesheet>')).GetStringVal()
          INTO LBatchFields
          FROM COEOBJECTCONFIG
          WHERE ID=2;

         --**Get Identifier**
        LIdentifierXML:='<IdentifierList>';
        FOR R_Identifiers IN C_Identifiers(ATempID) LOOP
            LIdentifierXML:=LIdentifierXML||R_Identifiers.IdentifierXML;
        END LOOP;
        LIdentifierXML:=LIdentifierXML||'</IdentifierList>';

        --**Get Project**
        LProjectXML:='<ProjectList>';
        FOR R_Projects IN C_Projects(ATempID) LOOP
            LProjectXML:=LProjectXML||R_Projects.ProjectXML;
        END LOOP;
        LProjectXML:=LProjectXML||'</ProjectList>';

         --**Get Project Batch**
        LProjectXMLBatch:='<ProjectList>';
        FOR R_Projects IN C_ProjectsBatch(ATempID) LOOP
            LProjectXMLBatch:=LProjectXMLBatch||R_Projects.ProjectXMLBatch;
        END LOOP;
        LProjectXMLBatch:=LProjectXMLBatch||'</ProjectList>';

        --**Get Batch**
        LSelect:='SELECT
                      TempBatchID, BATCHNUMBER,PERSONCREATED, DATELASTMODIFIED, DATECREATED,SequenceID,
                      ''BatchPropertyListBegin'' Aux,'||LBatchFields||'''BatchPropertyListEnd'' Aux,
                      '''||LProjectXML||''' PROJECTXML,'''||LProjectXMLBatch||''' PROJECTXMLBATCH,'''||LIdentifierXML||''' IDENTIFIERXML,
                      STRUCTUREAGGREGATION
                  FROM VW_TEMPORARYBATCH
                  WHERE TempBatchID='||ATempID;
         $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LSelect='||LSelect); $end null;

        LQryCtx:=DBMS_XMLGEN.newContext(LSelect);
        LXmlBatch:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
        DBMS_XMLGEN.closeContext(LQryCtx);

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LXmlBatch='||LXmlBatch); $end null;
        LStructureAggregationList:=TakeOffAndGetClobslist(LXmlBatch,'<STRUCTUREAGGREGATION>',NULL,NULL,FALSE);


        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LStructureAggregationList='||LStructureAggregationList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LXmlBatch sin StructureAggregation='||LXmlBatch); $end null;


        IF LXmlBatch IS NULL THEN
             RAISE_APPLICATION_ERROR(eGenericException,'No rows returned.'||CHR(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END IF;

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LXmlBatch='||LXmlBatch); $end null;

        LXmlBatch:=Replace(LXmlBatch,'<ROWSET>','<Batch>');
        LXmlBatch:=Replace(LXmlBatch,'<AUX>BatchPropertyListBegin</AUX>','<PropertyList>');
        AddNullFields(LBatchFields,LXmlBatch,'<AUX>BatchPropertyListEnd</AUX>','<Batch>');
        LXmlBatch:=Replace(LXmlBatch,'<AUX>BatchPropertyListEnd</AUX>','</PropertyList>');
        LXmlBatch:=Replace(LXmlBatch,'</ROW>','');
        LXmlBatch:=Replace(LXmlBatch,'</ROWSET>','');

        --**Get the PropertyList Fields from the XML field
        SELECT XmlTransform(XmlType.CreateXml(XML),XmlType.CreateXml('
                <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                  <xsl:template match="/MultiCompoundRegistryRecord">
                    <xsl:for-each select="BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property">TC.<xsl:value-of select="@name"/>,</xsl:for-each>
                  </xsl:template>
                </xsl:stylesheet>')).GetStringVal()
          INTO LBatchComponentFields
          FROM COEOBJECTCONFIG
         WHERE ID=2;
        --take out the last character and add Alias to first field
        LBatchComponentFields:=Replace(LBatchComponentFields,'COMMENTS','BATCHCOMPONENTCOMMENTS COMMENTS');

        --**Get Batch Component**
        LSelect:='SELECT TC.TempCompoundID, TC.TempBatchID,TC.BatchCompFragmentXML,
                         DECODE(NVL(r.regid,0),0,-TC.TempCompoundID,-C.COMPOUNDID) COMPONENTINDEX,
                         ''BatchComponentPropertyListBegin'' Aux,'||LBatchComponentFields||'''BatchComponentPropertyListEnd'' Aux
                    FROM VW_TemporaryCompound TC, VW_RegistryNumber R, VW_Compound C
                   WHERE TC.RegID = C.RegID(+) AND TC.RegID = R.RegID(+)
                     AND TC.TempBatchID = '||ATempID||' ORDER BY TC.TempCompoundID';

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LSelect='||LSelect); $end null;

        LQryCtx:=DBMS_XMLGEN.newContext(LSelect);

        LXmlQuery:=Replace(DBMS_XMLGEN.getXML(LQryCtx),'<?xml version="1.0"?>','');
        DBMS_XMLGEN.closeContext(LQryCtx);

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp',' LXmlQuery1='||LXmlQuery); $end null;

        LBatchComponentFields:=SUBSTR(LBatchComponentFields,4,LENGTH(LBatchComponentFields));
        LBatchComponentFields:=Replace(LBatchComponentFields,'TC.','');
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LXmlQuery2='||LXmlQuery); $end null;
        LBatchComponentFields:=Replace(LBatchComponentFields,'BATCHCOMPONENTCOMMENTS COMMENTS','COMMENTS');
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LXmlQuery3='||LXmlQuery); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LXmlQuery4='||LXmlQuery); $end null;

        IF LXmlQuery IS NULL THEN
             RAISE_APPLICATION_ERROR(eGenericException,'No rows returned.'||CHR(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END IF;


        LIndex:=0;
        LOOP
            --Search each Compounds
            LIndex:=LIndex+1;
            SELECT XmlType(LXmlQuery).extract('/ROWSET/ROW['||LIndex||']').getClobVal()
              INTO LXmlRows
              FROM dual;
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','replace batchcompound  LXmlRows='||LXmlRows); $end null;

        EXIT WHEN LXmlRows IS NULL;
            LXmlRows:=Replace(LXmlRows,'<AUX>BatchComponentPropertyListBegin</AUX>','<PropertyList>');
            AddNullFields(LBatchComponentFields,LXmlRows,'<AUX>BatchComponentPropertyListEnd</AUX>','<BatchComponent>');
            LXmlRows:=Replace(LXmlRows,'<AUX>BatchComponentPropertyListEnd</AUX>','</PropertyList>');
            LXmlBatchComponent:=LXmlBatchComponent || LXmlRows;
            $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','batchcompound LXmlBatchComponent='||LXmlBatchComponent); $end null;
        END LOOP;

        LXmlBatchComponent:='<BatchComponent>'||LXmlBatchComponent||'</BatchComponent>';

        LXml:=LXmlComponent||LXmlMixture||LXmlBatch||LXmlBatchComponent||'</ROW></Batch></MultiCompoundRegistryRecord> ';

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','MultiCompoundRegTmp SIN TRANSFORMACION:'|| chr(10)||LXml); $end null;

        LXmlTables:=XmlType.CreateXml(LXml);

        -- Build a new formatted Xml
        SELECT XmlTransform(LXmlTables,LXslTables).GetClobVal()
          INTO AXml
          FROM DUAL;

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','AXml:'|| chr(10)||AXml); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LStructuresList:'|| chr(10)||LStructuresList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LNormalizedStructureList:'|| chr(10)||LNormalizedStructureList); $end null;

        SELECT XmlType.CreateXml(XML)
            INTO LCoeObjectConfigField
            FROM COEOBJECTCONFIG
            WHERE ID=2;

        LXmlResult:=XmlType(AXml);

        AddTags(LCoeObjectConfigField,LXmlResult,'AddIns',Null);
        AddTags(LCoeObjectConfigField,LXmlResult,'ValidationRuleList','name');

        AXml:= TakeOnAndGetXml(LXmlResult.GetClobVal(),'STRUCTURE',LStructuresList);
        AXml:= TakeOnAndGetXml(AXml,'NORMALIZEDSTRUCTURE',LNormalizedStructureList);
        AXml:= TakeOnAndGetXml(AXml,'STRUCTUREAGGREGATION',LStructureAggregationList);
        AXml:= TakeOnAndGetXml(AXml,'FRAGMENTXML',LFragmentXMLList );

        --Replace '&lt;' and '&gt;'  by '<'' and '>''. I can't to do it using "XmlTransform"
        AXml:=replace(replace(replace(AXml,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>'),'&quot;','"');
        AXml:=replace(replace(AXml,'&lt;','<') ,'&gt;','>');

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','AXml:'|| chr(10)||AXml); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LStructuresList:'|| chr(10)||LStructuresList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LNormalizedStructureList:'|| chr(10)||LNormalizedStructureList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LStructureAggregationList:'|| chr(10)||LStructureAggregationList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','LFragmentXMLList:'|| chr(10)||LFragmentXMLList); $end null;

        AXml:=replace(AXml,'<FRAGMENTXML>','');
        AXml:=replace(AXml,'</FRAGMENTXML>','');
        AXml:=replace(AXml,'<BATCHCOMPFRAGMENTXML>','');
        AXml:=replace(AXml,'</BATCHCOMPFRAGMENTXML>','');
        AXml:=replace(AXml,'<IDENTIFIERXML>','');
        AXml:=replace(AXml,'</IDENTIFIERXML>','');
        AXml:=replace(AXml,'<PROJECTXML>','');
        AXml:=replace(AXml,'</PROJECTXML>','');

        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','MultiCompoundRegistryRecord:'|| chr(10)||AXml); $end null;
    ELSE
        --Validate xml template with the CreateXml object and get it.
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','ATempID:'|| chr(10)||ATempID); $end null;
        SELECT XmlType.CreateXml(XML).GetClobVal()
          INTO AXml
          FROM COEOBJECTCONFIG
          WHERE ID=2;
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','COEOBJECTCONFIG:'|| chr(10)||AXml); $end null;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        $if CompoundRegistry.Debuging $then InsertLog('RetrieveMultiCompoundRegTmp','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK || CHR(10) || LMessage); $end null;
        RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegTmp, CHR(13)||DBMS_UTILITY.FORMAT_ERROR_STACK || CHR(10) || LMessage);
    END;
END;

PROCEDURE UpdateMultiCompoundRegTmp( AXml in clob) IS
     /*
        Autor: Fari
        Date:10-may-20077
        Object:
        Description:
        Pending.
    */
    LCtx                   DBMS_XMLSTORE.ctxType;
    LXmlTables                XmlType;
    LXslTablesTransformed     XmlType;
    LXmlCompReg               CLOB;
    LXmlRows                  CLOB;
    LXmlField                 CLOB;
    LFieldToUpdate            CLOB;

    LIndex                    Number:=0;
    LFieldIndex               Number:=0;
    LRowsUpdated              Number:=0;
    LRowsDeleted              Number:=0;
    LRowsInserted             Number:=0;
    LTableName                CLOB;
    LMessage                  CLOB:='';
    LUpdate                   BOOLEAN;
    LSomeUpdate               BOOLEAN;
    LKeyFieldName             CLOB;
    LSectionDelete            BOOLEAN;
    LSectionInsert            BOOLEAN;

    LPosTagBegin              Number:=0;
    LPosTagEnd                Number:=0;
    LTagXmlFieldBegin         VARCHAR2(10):='<XMLFIELD>';
    LTagXmlFieldEnd           VARCHAR2(11):='</XMLFIELD>';

    LMolecularFormula                VW_TEMPORARYCOMPOUND.MOLECULARFORMULA%Type;
    LMolecularFormulaTag    CONSTANT VARCHAR2(20):='<MOLECULARFORMULA>';
    LMolecularFormulaTagEnd CONSTANT VARCHAR2(20):='</MOLECULARFORMULA>';
    LFormulaWeight                   VW_TEMPORARYCOMPOUND.FormulaWeight%Type;
    LFormulaWeightTag       CONSTANT VARCHAR2(20):='<FORMULAWEIGHT>';
    LFormulaWeightTagEnd    CONSTANT VARCHAR2(20):='</FORMULAWEIGHT>';

    LTempCompoundID                   Number:=0;
    LTempCompoundIDTag      CONSTANT VARCHAR2(20):='<TEMPCOMPOUNDID>';
    LTempCompoundIDTagEnd   CONSTANT VARCHAR2(20):='</TEMPCOMPOUNDID>';

    LTempBatchID                     Number:=0;
    LTempBatchIDTag         CONSTANT VARCHAR2(15):='<TEMPBATCHID>';
    LTempBatchIDTagEnd      CONSTANT VARCHAR2(15):='</TEMPBATCHID>';

    LStructureValue                CLOB;
    LStructuresList                CLOB;
    LFragmentXmlValue              CLOB;
    LFragmentXmlList               CLOB;
    LBatchCompFragmentXmlList      CLOB;
    LBatchCompFragmentXmlValue     CLOB;

    LNormalizedStructureList       CLOB;
    LNormalizedStructureValue      CLOB;
    LStructureAggregationList      CLOB;
    LStructureAggregationValue     CLOB;

    LStructureID                   Number(8);

    LXslTables XmlType:=XmlType.CreateXml('<?xml version="1.0" encoding="UTF-8"?>
      <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
      <xsl:template match="/MultiCompoundRegistryRecord">
        <MultiCompoundRegistryRecord>
          <xsl:for-each select="BatchList">
            <VW_TEMPORARYBATCH>
              <ROW>
                <xsl:for-each select="Batch">
                  <xsl:for-each select="BatchID">
                    <TEMPBATCHID>
                      <xsl:value-of select="."/>
                    </TEMPBATCHID>
                  </xsl:for-each>
                  <xsl:for-each select="BatchNumber[@update=''yes'']">
                    <BATCHNUMBER>
                      <xsl:value-of select="."/>
                    </BATCHNUMBER>
                  </xsl:for-each>
                  <xsl:for-each select="DateCreated[@update=''yes'']">
                    <DATECREATED>
                       <xsl:value-of select="."/>
                    </DATECREATED>
                  </xsl:for-each>
                  <xsl:for-each select="PersonCreated[@update=''yes'']">
                    <PERSONCREATED>
                      <xsl:value-of select="."/>
                    </PERSONCREATED>
                  </xsl:for-each>
                  <xsl:for-each select="DateLastModified[@update=''yes'']">
                    <DATELASTMODIFIED>
                      <xsl:value-of select="."/>
                    </DATELASTMODIFIED>
                  </xsl:for-each>
                  <xsl:for-each select="StructureAggregation[@update=''yes'']">
                    <STRUCTUREAGGREGATION>
                      <xsl:copy-of select="." />
                    </STRUCTUREAGGREGATION>
                  </xsl:for-each>
                 <xsl:for-each select="PropertyList">
                   <xsl:for-each select="Property[@update=''yes'']">
                     <xsl:variable name="V1" select="."/>
                     <xsl:for-each select="@name">
                       <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                       <xsl:choose>
                         <xsl:when test="$V2 = ''DELIVERYDATE''">
        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:when>
                                                        <xsl:when test="$V2 = ''DATEENTERED''">
        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:when>
                                                            <xsl:otherwise>
        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:otherwise>
                       </xsl:choose>
                     </xsl:for-each>
                   </xsl:for-each>
                 </xsl:for-each>
                 <xsl:for-each select="ProjectList[@update=''yes'']">
                   <PROJECTXML>
                     <XMLFIELD>
                        <xsl:copy-of select="." />
                     </XMLFIELD>
                  </PROJECTXML>
                 </xsl:for-each>
               </xsl:for-each>
            </ROW>
          </VW_TEMPORARYBATCH>
        </xsl:for-each>
          <xsl:for-each select="ComponentList/Component">
          <xsl:variable name="VDelete" select="@delete"/>
          <xsl:variable name="VInsert" select="@insert"/>
           <xsl:variable name="VComponentIndex" select="ComponentIndex/."/>
           <xsl:for-each select="Compound">
            <VW_TEMPORARYCOMPOUND>
              <xsl:choose>
                <xsl:when test="$VDelete=''yes''">delete="yes"</xsl:when>
                <xsl:when test="$VInsert=''yes''">insert="yes"</xsl:when>
              </xsl:choose>
              <ROW>

                <xsl:for-each select="RegNumber/RegID[$VInsert=''yes'']">
                  <REGID>
                    <xsl:choose>
                      <xsl:when test=".=''0''">
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="."/>
                      </xsl:otherwise>
                    </xsl:choose>
                  </REGID>
                </xsl:for-each>

                <xsl:for-each select="CompoundID">
                  <TEMPCOMPOUNDID>
                    <xsl:value-of select="."/>
                  </TEMPCOMPOUNDID>
                </xsl:for-each>
                <xsl:for-each select="CompoundID[$VInsert=''yes'']">
                   <TEMPBATCHID>
                     <xsl:value-of select="."/>
                   </TEMPBATCHID>
                 </xsl:for-each>
                <xsl:for-each select="BaseFragment/Structure/Structure[@update=''yes'' or $VInsert=''yes'']">
                  <BASE64_CDX>
                    <xsl:value-of select="."/>
                  </BASE64_CDX>
                </xsl:for-each>
                <xsl:for-each select="BaseFragment/Structure[@insert=''yes'']">
                  <STRUCTUREID></STRUCTUREID>
                </xsl:for-each>
                <xsl:for-each select="DateCreated[@update=''yes'' or $VInsert=''yes'']">
                  <DATECREATED>
                    <xsl:value-of select="."/>
                  </DATECREATED>
                </xsl:for-each>
                <xsl:for-each select="PersonCreated[@update=''yes'' or $VInsert=''yes'']">
                  <PERSONCREATED>
                    <xsl:value-of select="."/>
                  </PERSONCREATED>
                </xsl:for-each>
                <xsl:for-each select="DateLastModified[@update=''yes'' or $VInsert=''yes'']">
                  <DATELASTMODIFIED>
                    <xsl:value-of select="."/>
                  </DATELASTMODIFIED>
                </xsl:for-each>
                <!--xsl:for-each select="BaseFragment/Structure/Structure[@update=''yes'' or $VInsert=''yes'']"-->
                    <MOLECULARFORMULA></MOLECULARFORMULA>
                    <FORMULAWEIGHT></FORMULAWEIGHT>
                <!--/xsl:for-each-->
                <xsl:for-each select="PropertyList">
                  <xsl:for-each select="Property[@update=''yes'' or $VInsert=''yes'']">
                     <xsl:variable name="V1" select="."/>
                     <xsl:for-each select="@name">
                       <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
        LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;</xsl:for-each>
                   </xsl:for-each>
                </xsl:for-each>
                <xsl:for-each select="ProjectList[@update=''yes'' or $VInsert=''yes'']">
                  <PROJECTXML>
                    <XMLFIELD>
                      <xsl:copy-of select="." />
                    </XMLFIELD>
                  </PROJECTXML>
                </xsl:for-each>
                <xsl:for-each select="FragmentList[@update=''yes'' or $VInsert=''yes'']">
                  <FRAGMENTXML>
                    <XMLFIELD>
                      <xsl:copy-of select="." />
                    </XMLFIELD>
                  </FRAGMENTXML>
                </xsl:for-each>
                <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponentFragmentList[@update=''yes'' or $VInsert=''yes'']">
                  <BATCHCOMPFRAGMENTXML>
                    <XMLFIELD>
                      <xsl:copy-of select="." />
                    </XMLFIELD>
                  </BATCHCOMPFRAGMENTXML>
                </xsl:for-each>
                <xsl:for-each select="IdentifierList[@update=''yes'' or $VInsert=''yes'']">
                  <IDENTIFIERXML>
                    <XMLFIELD>
                      <xsl:copy-of select="." />
                    </XMLFIELD>
                  </IDENTIFIERXML>
                </xsl:for-each>
                <xsl:for-each select="Structure/NormalizedStructure[@update=''yes'' or $VInsert=''yes'']">
                  <NORMALIZEDSTRUCTURE>
                      <xsl:copy-of select="." />
                  </NORMALIZEDSTRUCTURE>
                </xsl:for-each>
                <xsl:for-each select="BaseFragment/Structure/UseNormalization[@update=''yes'' or $VInsert=''yes'']">
                  <USENORMALIZATION>
                      <xsl:value-of select="."/>
                  </USENORMALIZATION>
                </xsl:for-each>
                <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex=$VComponentIndex]">
                  <xsl:for-each select="PropertyList/Property[@update=''yes'' or $VInsert=''yes'']">
                    <xsl:variable name="V1" select="."/>
                    <xsl:for-each select="@name">
                      <xsl:variable name="V2" select="translate(., ''abcdefghijklmnopqrstuvwxyz'', ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'')"/>
                      <xsl:choose>
                        <xsl:when test="$V2 = ''COMMENTS''">
                          LESS_THAN_SIGN;BATCHCOMPONENTCOMMENTSGREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/BATCHCOMPONENTCOMMENTSGREATER_THAN_SIGN;
                        </xsl:when>
                        <xsl:otherwise>
                          LESS_THAN_SIGN;<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;<xsl:value-of select="$V1"/>LESS_THAN_SIGN;/<xsl:value-of select="$V2"/>GREATER_THAN_SIGN;
                        </xsl:otherwise>
                      </xsl:choose>
                    </xsl:for-each>
                  </xsl:for-each>
                </xsl:for-each>
              </ROW>
            </VW_TEMPORARYCOMPOUND>
           </xsl:for-each>
          </xsl:for-each>
      </MultiCompoundRegistryRecord>
     </xsl:template>
    </xsl:stylesheet>');


    PROCEDURE SetKeyValue(AID VARCHAR2,AIDTag VARCHAR2,AIDTagEnd VARCHAR2) IS
        LPosTag                   Number:=0;
        LPosTagNull               Number:=0;
        LPosTagEnd                Number:=0;
    BEGIN
        LPosTag:=1;
        LOOP
            LPosTagNull := INSTR(LXmlRows,SUBSTR(AIDTag,1,LENGTH(AIDTag)-1)||'/>',LPosTag);
            IF LPosTagNull<>0 THEN
                LXmlRows:=REPLACE(LXmlRows,SUBSTR(AIDTag,1,LENGTH(AIDTag)-1)||'/>',AIDTag||AIDTagEnd);
            END IF;
           LPosTag := INSTR(LXmlRows,AIDTag,LPosTag);
        EXIT WHEN LPosTag=0;
            LPosTag  := LPosTag + LENGTH(AIDTag)- 1;
            LPosTagEnd := INSTR(LXmlRows,AIDTagEnd,LPosTag);
            LXmlRows:=SUBSTR(LXmlRows,1,LPosTag)||AID||SUBSTR(LXmlRows,LPosTagEnd,LENGTH(LXmlRows));
        END LOOP;
    END;

BEGIN
    SetSessionParameter;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' AXml='||AXml); $end null;

    LXmlCompReg:=AXml;

    LSomeUpdate:=False;
    -- Take Out the Structures because the nodes of XmlType don't suport a size grater 64k.
    LFragmentXmlList:=TakeOffAndGetClobslist(LXmlCompReg,'<FragmentList',NULL,NULL,TRUE,TRUE);
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlCompReg sin FragmentList= '||LXmlCompReg); $end null;
    LBatchCompFragmentXmlList:=TakeOffAndGetClobslist(LXmlCompReg,'<BatchComponentFragmentList',NULL,NULL,TRUE,TRUE);
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlCompReg sin BatchComponentFragmentList= '||LXmlCompReg); $end null;
    LStructuresList:=TakeOffAndGetClobslist(LXmlCompReg,'<Structure ','<Structure','<BaseFragment>',TRUE);

    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlCompReg sin BaseFragment= '||LXmlCompReg); $end null;
    --LStructuresList:=TakeOffAndGetClobslist(LXmlCompReg,'<Structure ','<Structure ',NULL,FALSE,TRUE);
    LNormalizedStructureList:=TakeOffAndGetClobslist(LXmlCompReg,'<NormalizedStructure',NULL,NULL,TRUE);
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlCompReg sin NormalizedStructure= '||LXmlCompReg); $end null;
    LStructureAggregationList:=TakeOffAndGetClobslist(LXmlCompReg,'<StructureAggregation',NULL,NULL,TRUE);
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlCompReg sin StructureAggregation= '||LXmlCompReg); $end null;

    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LFragmentXmlList= '||LFragmentXmlList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LBatchCompFragmentXmlList= '||LBatchCompFragmentXmlList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LStructuresListt= '||LStructuresList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LNormalizedStructureList= '||LNormalizedStructureList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LStructureAggregationList= '||LStructureAggregationList); $end null;
    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlCompReg sin Structures= '||LXmlCompReg); $end null;

    -- Get the xml
    LXmlTables:=XmlType.createXML(LXmlCompReg);

    -- Build a new formatted Xml
    SELECT XmlTransform(LXmlTables,LXslTables)
      INTO LXslTablesTransformed
      FROM DUAL;

    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LXslTablesTransformed='||LXslTablesTransformed.getClobVal()); $end null;

    --Look over Xml searching each Table and update the rows of it.
     LOOP
        --Search each Table
        LIndex:=LIndex+1;
        SELECT LXslTablesTransformed.extract('/MultiCompoundRegistryRecord/node()['||LIndex||']').getClobVal()
        INTO LXmlRows
        FROM dual;

     EXIT WHEN LXmlRows IS NULL;

        --Get Table Name. Remove  '<' and '>'
        LTableName:= substr(LXmlRows,2,INSTR(LXmlRows,'>')-2);

        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LTableName='||LTableName||' LXmlRows='||LXmlRows); $end null;

        --Replace '&lt;' and '&gt;'  by '<'' and '>''. I can't to do it using "XmlTransform"
        LXmlRows:=replace(replace(replace(LXmlRows,'&quot;','"'),'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');

        IF INSTR(LXmlRows,'delete="yes"')=0 THEN
            LSectionDelete:=FALSE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LSectionDelete->FALSE'); $end null;
        ELSE
        LSectionDelete:=TRUE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LSectionDelete->TRUE'); $end null;
        END IF;

        IF INSTR(LXmlRows,'insert="yes"')=0 THEN
            LSectionInsert:=FALSE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LSectionInsert->FALSE'); $end null;
        ELSE
            LSectionInsert:=TRUE;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LSectionInsert->TRUE'); $end null;
        END IF;

        IF LSectionInsert THEN
            LRowsInserted:=0;

            CASE UPPER(LTableName)
                WHEN 'VW_TEMPORARYCOMPOUND' THEN
                    BEGIN
                        SELECT SEQ_TEMPORARY_COMPOUND.NEXTVAL INTO LTempCompoundID FROM DUAL;
                        SetKeyValue(LTempCompoundID,LTempCompoundIDTag,LTempCompoundIDTagEnd);
                        SetKeyValue(LTempBatchID,LTempBatchIDTag,LTempBatchIDTagEnd);

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LTempCompoundID= '||LTempCompoundID); $end null;

                        --When the structure is more length than 4000 we can't use insertXML
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LTempBatchID= '||LTempBatchID); $end null;

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LStructuresList='||LStructuresList); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LFragmentXmlList='||LFragmentXmlList); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LBatchComponentFragmentList='||LBatchCompFragmentXmlList); $end null;
                        LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');

                        SELECT extractValue(XmlType.createXML(LXmlRows),'/VW_TemporaryCompound/ROW/STRUCTUREID')
                            INTO LStructureID
                            FROM dual;

                        IF LStructureID='-1' THEN
                            SELECT Structure
                                INTO LStructureValue
                                FROM VW_Structure
                                WHERE StructureID=-1;
                        END IF;

                        LFragmentXmlValue:='<FragmentList>'||TakeOffAndGetClob(LFragmentXmlList,'Clob')||'</FragmentList>';
                        LBatchCompFragmentXmlValue:='<BatchComponentFragmentList>'||TakeOffAndGetClob(LBatchCompFragmentXmlList,'Clob')||'</BatchComponentFragmentList>';
                        LNormalizedStructureValue:=TakeOffAndGetClob(LNormalizedStructureList,'Clob');

                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LStructureValue= '|| LStructureValue ); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LFragmentXmlValue= '|| LFragmentXmlValue ); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LBatchCompFragmentXmlValue= '|| LBatchCompFragmentXmlValue ); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LNormalizedStructureValue= '|| LNormalizedStructureValue ); $end null;


                        IF LStructureValue IS NOT NULL THEN
                          SELECT cscartridge.formula(LStructureValue,''),cscartridge.molweight(LStructureValue)
                            INTO LMolecularFormula,LFormulaWeight
                             FROM DUAL;
                          $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LMolecularFormula= '|| LMolecularFormula||' LXmlRows:'||LXmlRows ); $end null;
                          SetKeyValue(LMolecularFormula,LMolecularFormulaTag,LMolecularFormulaTagEnd);
                          SetKeyValue(LFormulaWeight,LFormulaWeightTag,LFormulaWeightTagEnd);
                        END IF;

                        SetKeyValue(SYSDATE,'<DATECREATED>','</DATECREATED>');
                        SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
                    END;
                ELSE  LMessage:=LMessage || ' "' || LTableName||'" table stranger.';
            END CASE;
            --Replace '<' and '>'  by '&lt;' and '&gt;'' to the XML fields . Necesary to save a xml field using insertXML. I can't to do it using "XmlTransform".
            LPosTagBegin:=1;
            LOOP
                LPosTagBegin := INSTR(LXmlRows,LTagXmlFieldBegin,LPosTagBegin);
                LPosTagEnd   := INSTR(LXmlRows,LTagXmlFieldEnd,LPosTagBegin);

                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LOOP LXmlRows='||LXmlRows||' LPosTagBegin='||LPosTagBegin||' LPosTagEnd='||LPosTagEnd); $end null;

            EXIT WHEN (LPosTagBegin=0) or (LPosTagEnd=0);
                LXmlField:= SUBSTR(LXmlRows,LPosTagBegin+LENGTH(LTagXmlFieldBegin),LPosTagEnd-(LPosTagBegin+LENGTH(LTagXmlFieldBegin)));

                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LXmlField='||LXmlField||' '||LPosTagBegin||' '||LTagXmlFieldEnd||LPosTagEnd); $end null;

                LXmlField:= replace(replace(LXmlField,'<','&lt;') ,'>','&gt;');

                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' Before LXmlRows='||LXmlRows); $end null;
                LXmlRows:= SUBSTR(LXmlRows,1,LPosTagBegin-1)||LXmlField|| SUBSTR(LXmlRows,LPosTagEnd+LENGTH(LTagXmlFieldEnd),LENGTH(LXmlRows)) ;

                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' After LXmlRows='||LXmlRows); $end null;
            END LOOP;
            --Create the Table Context
            LCtx := DBMS_XMLSTORE.newContext(LTableName);
            DBMS_XMLSTORE.clearUpdateColumnList(LCtx);

            LXmlRows:=replace(LXmlRows,'insert="yes"','');

            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName||'->'||LXmlRows); $end null;

            --Insert Rows and get count it inserted
            LRowsInserted := DBMS_XMLSTORE.insertXML(LCtx, LXmlRows );

            --Build Message Logs
            LMessage:=LMessage || ' ' || cast (LRowsInserted  as string) || ' Row/s Inserted on "'||LTableName||'".';

            --Close the Table Context
            DBMS_XMLSTORE.closeContext(LCtx);

            --When structure is more length than 4000 we can't use insertXML
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','BEGIN UPDATE VW_TEMPORARYCOMPOUND'); $end null;

            IF UPPER(LTableName)='VW_TEMPORARYCOMPOUND' THEN
                IF LBatchCompFragmentXmlValue IS NOT NULL THEN
                     IF LStructureValue IS NOT NULL AND LFragmentXmlValue IS NOT NULL THEN
                       UPDATE VW_TEMPORARYCOMPOUND
                           SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                           WHERE TempCompoundID=LTempCompoundID;
                     ELSE
                        IF LStructureValue IS NOT NULL THEN
                          UPDATE VW_TEMPORARYCOMPOUND
                            SET BASE64_CDX=LStructureValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue,NormalizedStructure=LNormalizedStructureValue
                            WHERE TempCompoundID=LTempCompoundID;
                        END IF;
                        IF LFragmentXmlValue IS NOT NULL THEN
                          UPDATE VW_TEMPORARYCOMPOUND
                            SET FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue,NormalizedStructure=LNormalizedStructureValue
                            WHERE TempCompoundID=LTempCompoundID;
                        END IF;
                    END IF;
                ELSE
                    IF LStructureValue IS NOT NULL AND LFragmentXmlValue IS NOT NULL THEN
                         UPDATE VW_TEMPORARYCOMPOUND
                             SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                             WHERE TempCompoundID=LTempCompoundID;
                    ELSE
                         IF LStructureValue IS NOT NULL THEN
                           UPDATE VW_TEMPORARYCOMPOUND
                             SET BASE64_CDX=LStructureValue, NormalizedStructure=LNormalizedStructureValue
                             WHERE TempCompoundID=LTempCompoundID;
                         END IF;
                         IF LFragmentXmlValue IS NOT NULL THEN
                           UPDATE VW_TEMPORARYCOMPOUND
                             SET FRAGMENTXML=LFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                             WHERE TempCompoundID=LTempCompoundID;
                         END IF;
                    END IF;
                END IF;
            END IF;

            IF (LRowsInserted>0) THEN
                LSomeUpdate:=True;
            END IF;
            LMessage:=LMessage ||  chr(10) || cast (LRowsInserted  as string) || ' Row/s Inserted on "'||LTableName||'".';
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName||'->'||LXmlRows|| chr(10) ||LMessage); $end null;
        ELSIF LSectionDelete THEN
            LRowsDeleted:=0;

            SELECT  XmlType(LXmlRows).extract('node()[1]/ROW[1]/node()[1]').getClobVal()
                INTO LKeyFieldName
                FROM dual;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LKeyFieldName='||LKeyFieldName); $end null;

            IF LKeyFieldName IS NOT NULL THEN
                LCtx := DBMS_XMLSTORE.newContext(LTableName);
                LKeyFieldName:=XMLType(LKeyFieldName).getRootElement();
                DBMS_XMLSTORE.setKeyColumn(LCtx,LKeyFieldName);
                LRowsDeleted := DBMS_XMLSTORE.deleteXML(LCtx, LXmlRows );
                DBMS_XMLSTORE.closeContext(LCtx);
                IF (LRowsDeleted>0) THEN
                    LSomeUpdate:=True;
                END IF;
            END IF;
            LMessage:=LMessage ||  chr(10) || cast (LRowsDeleted  as string) || ' Row/s Deleted on "'||LTableName||'".';
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName||'->'||LXmlRows|| chr(10) ||LMessage); $end null;
        ELSE
            LRowsUpdated:=0;

            SELECT  XmlType(LXmlRows).extract('node()[1]/ROW[1]/node()[1]').getClobVal()
                INTO LKeyFieldName
                FROM dual;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'LKeyFieldName='||LKeyFieldName); $end null;

            IF LKeyFieldName IS NOT NULL THEN

                --Replace '<' and '>'  by '&lt;' and '&gt;'' to the XML fields . Necesary to save a xml field using insertXML. I can't to do it using "XmlTransform".
                LPosTagBegin:=1;
                LOOP
                    LPosTagBegin := INSTR(LXmlRows,LTagXmlFieldBegin,LPosTagBegin);
                    LPosTagEnd   := INSTR(LXmlRows,LTagXmlFieldEnd,LPosTagBegin);
                EXIT WHEN (LPosTagBegin=0) or (LPosTagEnd=0);
                    LXmlField:= SUBSTR(LXmlRows,LPosTagBegin+LENGTH(LTagXmlFieldBegin),LPosTagEnd-(LPosTagBegin+LENGTH(LTagXmlFieldBegin)));

                    LXmlField:= replace(replace(LXmlField,'<','&lt;') ,'>','&gt;');
                    LXmlRows:= SUBSTR(LXmlRows,1,LPosTagBegin-1)||LXmlField|| SUBSTR(LXmlRows,LPosTagEnd+LENGTH(LTagXmlFieldEnd),LENGTH(LXmlRows)) ;
                END LOOP;

                --Build Message Logs
                LMessage:=LMessage || chr(10) || 'Processing '||LTableName|| ': ';

                --Create the Table Context
                LCtx := DBMS_XMLSTORE.newContext(LTableName);


                CASE UPPER(LTableName)
                    WHEN 'VW_TEMPORARYBATCH' THEN
                    BEGIN
                        SELECT  extractvalue(XmlType(LXmlRows),'node()[1]/ROW/node()[1]')
                            INTO LTempBatchID
                            FROM dual;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYBATCH LTempBatchID= '||LTempBatchID); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LStructureAggregationValue= '||LStructureAggregationValue); $end null;
                        LStructureAggregationValue:=TakeOffAndGetClob(LStructureAggregationList,'Clob');
                        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','LStructureAggregationValue= '||LStructureAggregationValue); $end null;
                    END;
                    WHEN 'VW_TEMPORARYCOMPOUND' THEN
                        BEGIN
                            SELECT  extractvalue(XmlType(LXmlRows),'node()[1]/ROW/node()[1]')
                                INTO LTempCompoundID
                                FROM dual;

                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LStructuresList='||LStructuresList); $end null;
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','VW_TEMPORARYCOMPOUND LFragmentXmlList='||LFragmentXmlList); $end null;
                            LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LStructureValue= '|| LStructureValue ); $end null;
                            LFragmentXmlValue:='<FragmentList>'||TakeOffAndGetClob(LFragmentXmlList,'Clob')||'</FragmentList>';
                            IF LFragmentXmlValue='<FragmentList></FragmentList>' THEN
                                LFragmentXmlValue:='';
                            END IF;
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LFragmentXmlValue= '|| LFragmentXmlValue ); $end null;
                            LBatchCompFragmentXmlValue:='<BatchComponentFragmentList>'||TakeOffAndGetClob(LBatchCompFragmentXmlList,'Clob')||'</BatchComponentFragmentList>';
                            IF LBatchCompFragmentXmlValue='<BatchComponentFragmentList></BatchComponentFragmentList>' THEN
                                LBatchCompFragmentXmlValue:='';
                            END IF;
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LBatchCompFragmentXmlValue= '|| LBatchCompFragmentXmlValue ); $end null;
                            LNormalizedStructureValue:=TakeOffAndGetClob(LNormalizedStructureList,'Clob');
                            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LNormalizedStructureValue= '|| LNormalizedStructureValue ); $end null;

                            IF LStructureValue IS NOT NULL THEN
                                SELECT cscartridge.formula(LStructureValue,''),cscartridge.molweight(LStructureValue)
                                    INTO LMolecularFormula,LFormulaWeight
                                    FROM DUAL;
                                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LMolecularFormula= '|| LMolecularFormula ); $end null;
                                SetKeyValue(LMolecularFormula,LMolecularFormulaTag,LMolecularFormulaTagEnd);
                                SetKeyValue(LFormulaWeight,LFormulaWeightTag,LFormulaWeightTagEnd);
                            ELSE
                                LXmlRows:=replace(replace(LXmlRows,'<MOLECULARFORMULA/>',''),'<FORMULAWEIGHT/>','');
                                LXmlRows:=replace(replace(LXmlRows,'<MOLECULARFORMULA></MOLECULARFORMULA>',''),'<FORMULAWEIGHT></FORMULAWEIGHT>','');
                            END IF;
                        END;
                    ELSE  NULL;
                END CASE;


                DBMS_XMLSTORE.clearUpdateColumnList(LCtx);
                LKeyFieldName:=XMLType(LKeyFieldName).getRootElement();
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' LKeyFieldName= '|| LKeyFieldName ); $end null;
                DBMS_XMLSTORE.setKeyColumn(LCtx,LKeyFieldName);
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',' after setKeyColumn LKeyFieldName= '|| LKeyFieldName ); $end null;
                LFieldIndex:=1;
                LUpdate:=FALSE;
                LOOP
                    LFieldIndex:=LFieldIndex+1;
                    SELECT  XmlType(LXmlRows).extract('node()[1]/ROW/node()['||LFieldIndex||']').getClobVal()
                    INTO LFieldToUpdate
                    FROM dual;

                    IF LFieldToUpdate IS NOT NULL THEN
                        LUpdate:=TRUE;
                        LFieldToUpdate:=XMLType(LFieldToUpdate).getRootElement();
                    END IF;

                    $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp'||' Line:'||$$plsql_line,'->'||LFieldToUpdate); $end null;

                EXIT WHEN LFieldToUpdate IS NULL;
                    DBMS_XMLSTORE.setUpdateColumn(LCtx, LFieldToUpdate);
                END LOOP;

                --Insert Rows and get count it inserted
                IF LUpdate THEN
                    LRowsUpdated := DBMS_XMLSTORE.updateXML(LCtx, LXmlRows );
                    LSomeUpdate:=TRUE;
                END IF;
                --Close the Table Context
                DBMS_XMLSTORE.closeContext(LCtx);

                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','UPDATE LTempCompoundID= '|| LTempCompoundID ); $end null;
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','UPDATE LStructureValue= '|| LStructureValue ); $end null;
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','UPDATE LFragmentXmlValue= '|| LFragmentXmlValue ); $end null;
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','UPDATE LBatchCompFragmentXmlValue= '|| LBatchCompFragmentXmlValue ); $end null;
                $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','UPDATE LNormalizedStructureValue= '|| LNormalizedStructureValue ); $end null;
                IF UPPER(LTableName)='VW_TEMPORARYCOMPOUND' THEN
                    IF  LBatchCompFragmentXmlValue IS NOT NULL THEN
                        IF LStructureValue IS NOT NULL THEN
                            IF LFragmentXmlValue IS NOT NULL THEN
                                IF LNormalizedStructureValue IS NOT NULL THEN
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                ELSE
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                END IF;
                            ELSE
                                IF LNormalizedStructureValue IS NOT NULL THEN
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue,NormalizedStructure=LNormalizedStructureValue,BatchCompFragmentXML=LBatchCompFragmentXmlValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                ELSE
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                END IF;
                            END IF;
                        ELSE
                            IF LFragmentXmlValue IS NOT NULL THEN
                                  IF LNormalizedStructureValue IS NOT NULL THEN
                                      LSomeUpdate:=True;
                                      UPDATE VW_TEMPORARYCOMPOUND
                                          SET FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                                          WHERE TempCompoundID=LTempCompoundID;
                                      LRowsUpdated:=SQL%ROWCOUNT;
                                  ELSE
                                      LSomeUpdate:=True;
                                      UPDATE VW_TEMPORARYCOMPOUND
                                          SET FRAGMENTXML=LFragmentXmlValue,BatchCompFragmentXML=LBatchCompFragmentXmlValue
                                          WHERE TempCompoundID=LTempCompoundID;
                                      LRowsUpdated:=SQL%ROWCOUNT;
                                  END IF;
                              ELSE
                                  IF LNormalizedStructureValue IS NOT NULL THEN
                                      LSomeUpdate:=True;
                                      UPDATE VW_TEMPORARYCOMPOUND
                                          SET NormalizedStructure=LNormalizedStructureValue,BatchCompFragmentXML=LBatchCompFragmentXmlValue
                                          WHERE TempCompoundID=LTempCompoundID;
                                      LRowsUpdated:=SQL%ROWCOUNT;
                                  END IF;
                              END IF;
                        END IF;
                   ELSE
                        IF LStructureValue IS NOT NULL THEN
                            IF LFragmentXmlValue IS NOT NULL THEN
                                IF LNormalizedStructureValue IS NOT NULL THEN
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                ELSE
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                END IF;
                            ELSE
                                IF LNormalizedStructureValue IS NOT NULL THEN
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue,NormalizedStructure=LNormalizedStructureValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                ELSE
                                    LSomeUpdate:=True;
                                    UPDATE VW_TEMPORARYCOMPOUND
                                        SET BASE64_CDX=LStructureValue
                                        WHERE TempCompoundID=LTempCompoundID;
                                    LRowsUpdated:=SQL%ROWCOUNT;
                                END IF;
                            END IF;
                        ELSE
                            IF LFragmentXmlValue IS NOT NULL THEN
                                  IF LNormalizedStructureValue IS NOT NULL THEN
                                      LSomeUpdate:=True;
                                      UPDATE VW_TEMPORARYCOMPOUND
                                          SET FRAGMENTXML=LFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue
                                          WHERE TempCompoundID=LTempCompoundID;
                                      LRowsUpdated:=SQL%ROWCOUNT;
                                  ELSE
                                      LSomeUpdate:=True;
                                      UPDATE VW_TEMPORARYCOMPOUND
                                          SET FRAGMENTXML=LFragmentXmlValue
                                          WHERE TempCompoundID=LTempCompoundID;
                                      LRowsUpdated:=SQL%ROWCOUNT;
                                  END IF;
                              ELSE
                                  IF LNormalizedStructureValue IS NOT NULL THEN
                                      LSomeUpdate:=True;
                                      UPDATE VW_TEMPORARYCOMPOUND
                                          SET NormalizedStructure=LNormalizedStructureValue
                                          WHERE TempCompoundID=LTempCompoundID;
                                      LRowsUpdated:=SQL%ROWCOUNT;
                                  END IF;
                              END IF;
                        END IF;
                     END IF;
                END IF;

                IF UPPER(LTableName)='VW_TEMPORARYBATCH' THEN
                    IF LStructureAggregationValue IS NOT NULL THEN
                        LSomeUpdate:=True;
                        UPDATE VW_TEMPORARYBATCH
                            SET StructureAggregation=LStructureAggregationValue
                            WHERE TempBatchID=LTempBatchID;
                        LRowsUpdated:=SQL%ROWCOUNT;
                  END IF;
                END IF;
            END IF;

            LMessage := LMessage || ' ' || cast(LRowsUpdated as string) || ' Row/s Updated on "' || LTableName || '".' || chr(13) ;
            $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp',LTableName || '->' || LXmlRows || chr(10) || LMessage); $end null;
       END IF;
    END LOOP;

    IF NOT LSomeUpdate THEN
        RAISE_APPLICATION_ERROR(eGenericException, 'There aren''t data to update.');
    END IF;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        LMessage := LMessage || ' ' || cast(LRowsUpdated as string) || ' Row/s Updated on "' || LTableName || '".';
        $if CompoundRegistry.Debuging $then InsertLog('UpdateMultiCompoundRegTmp','Error: '||DBMS_UTILITY.FORMAT_ERROR_STACK||'Process Log:'||CHR(13)||LMessage);$end null;
        RAISE_APPLICATION_ERROR(eUpdateMultiCompoundRegTmp,CHR(13)||DBMS_UTILITY.FORMAT_ERROR_STACK||'Process Log:'||CHR(13)||LMessage || chr(13));
    END;
END;


PROCEDURE DeleteMultiCompoundRegTmp( ATempID  in Number) IS
BEGIN

    $if CompoundRegistry.Debuging $then InsertLog('DeleteMultiCompoundRegTmp','Begin'); $end null;

    DELETE VW_TemporaryCompound WHERE TempBatchID=ATempID;

    DELETE VW_TemporaryBatch WHERE TempBatchID =ATempID;

    IF SQL%NOTFOUND THEN
        RAISE_APPLICATION_ERROR(eGenericException, 'TempID '||ATempID||' doesn''t exist. 0 Row Deleted on VW_TemporaryBatch.');
    END IF;

    $if CompoundRegistry.Debuging $then InsertLog('DeleteMultiCompoundRegTmp','End'); $end null;

EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eDeleteMultiCompoundRegTmp,' '||DBMS_UTILITY.FORMAT_ERROR_STACK||CHR(13));
    END;
END;

PROCEDURE RetrieveCompoundRegistryList(AXmlRegNumbers in clob, AXmlCompoundList out NOCOPY clob) IS
       -- Autor: Fari
       -- Date:17-May-07
       -- Object:
       -- Pending:
      LMessage                  CLOB:='';
      LXml                      CLOB:='';
      LXmlList                  CLOB:='';
      LSectionList   CONSTANT   VARCHAR2(500):='Compound,Identifier';
      LRegNumber                VW_RegistryNumber.RegNumber%type;
      LIndex                    Number;
      LAux                      CLOB:='';
      LIndexAux                 Number;
      LPosBegin                 Number;
      LPosEnd                   Number;
BEGIN
  LIndex:=1;
  LOOP
      $if CompoundRegistry.Debuging $then InsertLog('RetrieveCompoundRegistryList','AXmlRegNumbers->'||AXmlRegNumbers); $end null;
      SELECT  extractValue(XmlType(AXmlRegNumbers),'REGISTRYLIST/node()['||LIndex||']/node()[1]')
        INTO LRegNumber
        FROM dual;
      $if CompoundRegistry.Debuging $then InsertLog('RetrieveCompoundRegistryList','LRegNumber->'||LRegNumber); $end null;
  EXIT WHEN LRegNumber IS NULL;
      BEGIN
          RetrieveMultiCompoundRegistry(LRegNumber,LXml,LSectionList);

          LPosBegin:=Instr(LXml,'<MultiCompoundRegistryRecord');
          LPosEnd:=Instr(LXml,'>',LPosBegin);
          LXml:=Substr(LXml,1,LPosBegin-1)||Substr(LXml,LPosEnd-LPosBegin+2,length(LXml));
          LXml:=replace(LXml,'</MultiCompoundRegistryRecord>','');

          LXmlList:=LXmlList||CHR(10)||LXml;
      EXCEPTION
         WHEN OTHERS THEN
         BEGIN
           IF INSTR(DBMS_UTILITY.FORMAT_ERROR_STACK,eNoRowsReturned)<>0 THEN NULL; --Though a Compound doesn't exist to get the others
           ELSE
              RAISE_APPLICATION_ERROR(eRetrieveCompoundRegistryList,DBMS_UTILITY.FORMAT_ERROR_STACK);
           END IF;
         END;
      END;
      LIndex:=LIndex+1;
  END LOOP;
  AXmlCompoundList:='<CompoundList>'||CHR(10)||LXmlList||CHR(10)||'</CompoundList>';
  $if CompoundRegistry.Debuging $then InsertLog('RetrieveCompoundRegistryList','List->'||AXmlCompoundList); $end null;
EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eRetrieveCompoundRegistryList, LMessage||chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
    END;
END;

PROCEDURE MoveBatch(ABatchID in Number,ARegNumber in VW_RegistryNumber.RegNumber%type) IS
    LBatchCount Integer;
	LValidRegistry Integer;
    LTargetComponentCount Integer;
    LSourceComponentCount Integer;
    LTargetBatchID Integer;
    LRegID Integer;
BEGIN
    $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'ABatchID: ' || ABatchID||' ARegNumber: ' || ARegNumber); $end null;

    SELECT COUNT(1)
        INTO  LBatchCount
        FROM  VW_Batch
        WHERE RegID = (SELECT RegID FROM  VW_Batch WHERE BatchID=ABatchID);

	--Ulises 9-12-08: Check destiny RegNum before moving.
	SELECT COUNT(1)
		INTO LValidRegistry
		FROM VW_RegistryNumber
		WHERE REGNUMBER = ARegNumber;

    IF LBatchCount=0 THEN
        BEGIN
            Raise_Application_Error (eNoRowsReturned,'No rows returned. The batch was not moved.'||CHR(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    ELSIF LBatchCount=1 THEN
        BEGIN
            Raise_Application_Error (eOnlyOneBatch, 'The Batch''s Registry has an only Batch. The Batch was not moved.'||CHR(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
	ELSIF LValidRegistry=0 THEN
		BEGIN
			Raise_Application_Error (eInvalidRegNum,'The given RegNum doesn''t exist.'||CHR(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
		END;
    ELSE
        BEGIN
            SELECT RegID
                INTO LRegID
                FROM  VW_RegistryNumber
                WHERE RegNumber=ARegNumber;
            $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LRegID: ' || LRegID); $end null;

            SELECT Min(BatchID)
               INTO LTargetBatchID
               FROM VW_Batch
               WHERE RegID=LRegID AND BatchID<>ABatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LTargetBatchID: ' || LTargetBatchID); $end null;

            SELECT Count(1)
                INTO LTargetComponentCount
                FROM VW_BatchComponent
                WHERE BatchID=LTargetBatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LTargetComponentCount: ' || LTargetComponentCount); $end null;

            UPDATE VW_BatchComponent BC1
                SET MixtureComponentID=(SELECT BC2.MixtureComponentID
                                            FROM VW_BatchComponent BC2
                                            WHERE BatchID=LTargetBatchID AND BC1.OrderIndex=BC2.OrderIndex
                                            GROUP BY BC2.MixtureComponentID)
                WHERE BatchID=ABatchID;

            LSourceComponentCount:=SQL%ROWCOUNT;
            $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'LSourceComponentCount: ' || LSourceComponentCount); $end null;

            IF LSourceComponentCount<LTargetComponentCount THEN
                INSERT INTO VW_BatchComponent(ID,BATCHID,MIXTURECOMPONENTID,ORDERINDEX)
                    (SELECT (SELECT MAX(ID)+1 FROM VW_BatchComponent),ABatchID,MixtureComponentID,OrderIndex
                        FROM VW_BatchComponent
                        WHERE BatchID=LTargetBatchID AND OrderIndex>LSourceComponentCount);
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'INSERT INTO VW_BatchComponent SQL%ROWCOUNT : ' || SQL%ROWCOUNT); $end null;
            ELSIF LSourceComponentCount>LTargetComponentCount THEN
                DELETE VW_BatchComponent WHERE BatchID=ABatchID AND OrderIndex>LTargetComponentCount;
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'DELETE VW_BatchComponent SQL%ROWCOUNT : ' || SQL%ROWCOUNT); $end null;
            END IF;

            UPDATE VW_Batch SET REGID = LRegID
                WHERE BATCHID=ABatchID;

            $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'UPDATE VW_Batch SQL%ROWCOUNT : ' || SQL%ROWCOUNT); $end null;
        END;
    END IF;

EXCEPTION
    WHEN OTHERS THEN
        $if CompoundRegistry.Debuging $then InsertLog('MoveBatch',chr(10)||DBMS_UTILITY.FORMAT_ERROR_STACK); $end null;
        Raise_Application_Error (EGenericException, DBMS_UTILITY.Format_Error_Stack);
        Rollback;
END;

PROCEDURE DeleteBatch(ABatchID in Number) IS
    LBatchCount Integer;
BEGIN
    SELECT COUNT(1)
        INTO  LBatchCount
        FROM  VW_Batch
        WHERE REGID = (SELECT RegID FROM  VW_Batch WHERE BatchID=ABatchID);

    IF LBatchCount=0 THEN
        BEGIN
            Raise_Application_Error (eNoRowsReturned,'No rows returned. The batch was not moved.'||CHR(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    ELSIF LBatchCount=1 THEN
        BEGIN
            Raise_Application_Error (eOnlyOneBatch, 'The Batch''s Registry has an only Batch. The Batch was not deleted.'||CHR(10)||DBMS_UTILITY.FORMAT_ERROR_STACK);
        END;
    ELSE
        BEGIN
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'ABatchID: ' || ABatchID); $end null;
            DELETE VW_BATCH_PROJECT WHERE BATCHID=ABatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_BATCH_PROJECT SQL%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
            DELETE VW_BATCHCOMPONENT WHERE BATCHID=ABatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_BATCHCOMPONENT SQL%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
            DELETE VW_BATCH WHERE BATCHID=ABatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_BATCH SQL%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
        END;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        Raise_Application_Error (EGenericException, DBMS_UTILITY.Format_Error_Stack);
END;

FUNCTION ExtractRegNumber (AXmlError IN CLOB, ATagError IN VARCHAR2,AFilter IN VARCHAR2:=NULL) RETURN VARCHAR2 IS
    LTagBeginError   VARCHAR2 (255);
    LTagEndError     VARCHAR2 (255);
    LStartIndex      NUMBER;
    LEndIndex        NUMBER;
    LXmlDuplicates   CLOB;
    LPath            VARCHAR2 (100) := '';
    LRegnumber       VARCHAR2 (100) := '';
BEGIN
    -- Extract Reg List Duplicates
    LTagBeginError := '<' || ATagError || '>';
    LTagEndError := '</' || ATagError || '>';
    LStartIndex := INSTR (AXmlError, LTagBeginError);
    LEndIndex := INSTR (AXmlError, LTagEndError) + LENGTH (LTagEndError);
    LXmlDuplicates := SUBSTR (AXmlError, LStartIndex, LEndIndex - LStartIndex);

    IF ATagError = 'COMPOUNDLIST' THEN
        LPath := 'COMPOUNDLIST/COMPOUND[1]/';
    END IF;

    IF AFilter IS NOT NULL THEN
        LPath:=LPath || 'REGISTRYLIST/REGNUMBER['||AFilter||'][1]/node()[1]';
    ELSE
        LPath:=LPath || 'REGISTRYLIST/REGNUMBER[1]/node()[1]';
    END IF;

    $if CompoundRegistry.Debuging $then InsertLog ('ExtractRegNumber', 'LXmlDuplicates ' || LXmlDuplicates|| 'Path: '||LPath); $end null;

    SELECT EXTRACTVALUE (XMLTYPE.CreateXml (LXmlDuplicates), LPath)
      INTO LRegNumber
      FROM DUAL;

    RETURN LRegNumber;
END;

PROCEDURE AddBatch (AXml IN OUT NOCOPY CLOB, AXmlError IN CLOB, ATagError IN VARCHAR2,AFilter IN VARCHAR2:=NULL) IS
    LRegNumber        VARCHAR2 (50) := '';
    LXmlBatchList     CLOB;
    LXmlRows          CLOB;
    LCompoundID       NUMBER (8);
    LPosInitial       NUMBER        := 1;
    LPosTagEnd        NUMBER;
BEGIN

    LRegNumber := ExtractRegNumber (AXmlError, ATagError,AFilter);

    SELECT MC.CompoundID
        INTO LCompoundID
        FROM VW_RegistryNumber RN, VW_Mixture_RegNumber MR,VW_Mixture_Component MC
        WHERE rn.regid = MR.regid  AND MR.MIXTUREID=MC.MIXTUREID AND rn.regnumber = LRegNumber;

    RetrieveMultiCompoundRegistry (LRegNumber, LXmlRows);

    $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'LXmlRows: ' || LXmlRows); $end null;

    SELECT Extract(XmlType.CreateXml(LXmlRows), '/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[CompoundID='||LCompoundID||']/../..').GetClobVal()
         INTO LXmlBatchList
         FROM DUAL;
    $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'LXmlBatchList1: ' || LXmlBatchList); $end null;

    LXmlBatchList := REPLACE (LXmlBatchList, '<Batch>', '<Batch insert="yes">');
    LXmlBatchList := REPLACE (LXmlBatchList, '<BatchComponent>', '<BatchComponent insert="yes">');

    LPosInitial := INSTR (LXmlBatchList, '<BatchID>');
    IF LPosInitial>0 THEN
         LPosTagEnd := INSTR (LXmlBatchList, '</BatchID>', LPosInitial);
         LXmlBatchList := SUBSTR (LXmlBatchList, 1, LPosInitial + 8) || '0' ||SUBSTR (LXmlBatchList,LPosTagEnd, LENGTH (LXmlBatchList));
    END IF;
    $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'LXmlBatchList2: ' || LXmlBatchList); $end null;

    LPosTagEnd := INSTR (LXmlRows, '</BatchList>', -1);
    IF LPosTagEnd <> 0 THEN
        AXml := SUBSTR (LXmlRows, 1, LPosTagEnd - 1) || LXmlBatchList
             || SUBSTR (LXmlRows, LPosTagEnd, LENGTH (LXmlRows) - LPosTagEnd);
    END IF;

    $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'Returning AXml: ' || AXml); $end null;
EXCEPTION
    WHEN OTHERS THEN
        Raise_Application_Error (EGenericException, DBMS_UTILITY.Format_Error_Stack);
END;

PROCEDURE UseCompound (AXml IN OUT NOCOPY CLOB, AXmlError IN CLOB, ATagError IN VARCHAR2) IS
    LRegnumber         VARCHAR2 (100) := '';
    LXmlTrash          CLOB;
    LXmlCompoundList   CLOB;
    LXmlCompound       CLOB;
    LPosTagEnd         NUMBER;
BEGIN
    $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'AXml: ' || AXml || 'ATagError: ' || ATagError); $end null;
    LRegNumber := ExtractRegNumber (AXmlError, ATagError);
    $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'LRegNumber: ' || LRegNumber ); $end null;
    RetrieveCompoundRegistryList ('<REGISTRYLIST>' || LRegNumber || '</REGISTRYLIST>', LXmlCompoundList);
    $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'Compound List: ' || LXmlCompoundList); $end null;
    LXmlCompound := TakeOffAndGetClob (LXmlCompoundList, 'Compound');
    $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'Compound: ' || LXmlCompound); $end null;
    LXmlTrash := TakeOffAndGetClob (AXml, 'Compound');
    $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'Xml sin Compound: ' || AXml); $end null;
    LPosTagEnd := INSTR (AXml, '</Component>');

    IF LPosTagEnd <> 0 THEN
        AXml := SUBSTR (AXml, 1, LPosTagEnd - 1) || '<Compound>' || LXmlCompound || '</Compound>' || SUBSTR (AXml, LPosTagEnd, LENGTH (AXml) - LPosTagEnd + 1);
    END IF;

    $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'Xml con Compound: ' || AXml); $end null;
EXCEPTION
    WHEN OTHERS THEN
        Raise_Application_Error (EGenericException, DBMS_UTILITY.Format_Error_Stack);
END;

PROCEDURE LoadMultiCompoundRegRecord(ARegistryXml IN CLOB, ADuplicateAction IN CHAR, AAction OUT NOCOPY CHAR, ARegNumber OUT NOCOPY VW_RegistryNumber.RegNumber%TYPE, ARegistration IN CHAR := 'Y', ARegNumGeneration IN CHAR := 'Y', AConfigurationID IN Number := 1) IS
    LXmlRows       CLOB           := ARegistryXml;
    LTempId        NUMBER;
    LError         CLOB;
    LRegNumber     CLOB;
    LDiscard       CLOB;
    LMessage       CLOB;
BEGIN
    AAction := 'E';
    IF ARegistration = 'Y' THEN
        BEGIN
            $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ARegistration: ' || ARegistration); $end null;
            CreateMultiCompoundRegistry (LXmlRows, ARegNumber, LMessage, 'C', ARegNumGeneration, AConfigurationID);
            IF LMessage IS NULL THEN
                AAction := 'C';
                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'RegNumber: ' || LRegNumber); $end null;
            ELSE
                LError := LMessage;
                AAction := ADuplicateAction;
                IF ADuplicateAction = 'D' THEN
                   CreateMultiCompoundRegistry (LXmlRows, ARegNumber, LMessage, 'N', ARegNumGeneration);
                ELSIF ADuplicateAction = 'B' THEN
                    LRegNumber := LError; -- Copy so we won't destroy LError
                    LRegNumber := TakeOffAndGetClobsList(LRegNumber, 'REGNUMBER');  -- Get the RegNumbers
                    LRegNumber := TakeOffAndGetClob(LRegNumber, 'Clob');  -- Get only the first one
                    LDiscard := TakeOffAndGetClobsList(LRegNumber, 'SAMEFRAGMENT'); -- Discard
                    LDiscard := TakeOffAndGetClobsList(LRegNumber, 'SAMEEQUIVALENT'); -- Discard
                    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ADuplicateAction = ''B'''); $end null;
                    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'LError: ' || LError); $end null;
                    IF INSTR (LError, '<COMPOUNDLIST>') > 0 AND INSTR (LError, '</COMPOUNDLIST>') > 0 THEN
                        IF INSTR (LError, 'count="1"') > 0 THEN
                            $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'Adding Batch...LXmlRows: ' || LXmlRows); $end null;
                            AddBatch (LXmlRows, LError, 'COMPOUNDLIST','@count="1"');
                            $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'Updating... LXmlRows:'|| LXmlRows); $end null;
                            UpdateMultiCompoundRegistry (LXmlRows, LMessage, 'D');
                            ARegNumber := LRegNumber;
                            $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'After Update' || LRegNumber); $end null;
                        ELSE
                            UseCompound (LXmlRows, LError, 'COMPOUNDLIST');
                            CreateMultiCompoundRegistry (LXmlRows, ARegNumber, LMessage, 'D', ARegNumGeneration);
                            AAction := 'D';
                            $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'UseCompound (COMPOUNDLIST): ' || LRegNumber); $end null;
                        END IF;
                    ELSIF INSTR (LError, '<REGISTRYLIST>') > 0 AND INSTR (LError, '</REGISTRYLIST>') > 0 THEN
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'Before-AddBatch LXmlRows): ' || LXmlRows); $end null;
                        AddBatch (LXmlRows, LError, 'REGISTRYLIST');
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'After-AddBatch LXmlRows): ' || LXmlRows); $end null;
                        UpdateMultiCompoundRegistry (LXmlRows, LMessage, 'D');
                        ARegNumber := LRegNumber;
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'AddBatch (REGISTRYLIST): ' || LRegNumber); $end null;
                    ELSE
                        Raise_Application_Error (EGenericException, DBMS_UTILITY.Format_Error_Stack);
                    END IF;
                ELSIF ADuplicateAction = 'T' THEN
                    CreateMultiCompoundRegTmp (LXmlRows, LTempID);
                    ARegNumber := LTempID;
                    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'TempID: ' || LTempID); $end null;
                ELSIF ADuplicateAction = 'N' THEN
                    ARegNumber := '';
                    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'TempID: ' || LTempID); $end null;
                ELSE
                    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'DBMS_UTILITY.Format_Error_Stack ' || DBMS_UTILITY.Format_Error_Stack); $end null;
                    Raise_Application_Error (EGenericException, DBMS_UTILITY.Format_Error_Stack);
                END IF;
           END IF;
        END;

    ELSE
        CreateMultiCompoundRegTmp (LXmlRows, LTempID);
        ARegNumber := LTempID;
        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'TempID: ' || LTempID); $end null;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', ' DBMS_UTILITY.Format_Error_Stack: ' ||  DBMS_UTILITY.Format_Error_Stack); $end null;
        Raise_Application_Error (EGenericException, DBMS_UTILITY.Format_Error_Stack);
END;

PROCEDURE LoadMultiCompoundRegRecordList(ARegistryListXml  IN CLOB, ADuplicateAction  IN CHAR, ARegistration IN CHAR, ARegNumGeneration IN CHAR, AConfigurationID IN NUMBER, LogID IN NUMBER) IS
    -- returns to C# every 100 records, so LogID has to be passed in
    LATempID        VARCHAR2(8) := '0';
    LXmlRows        CLOB;
    LXmlCompReg     CLOB := ARegistryListXml;

    -- temporarily. should get from xml. need confirmation
    ADuplicateCheck CHAR := 'C';
    -- temporarily. should be returned from LoadMultiCompoundRegRecord
    IsDuplicate     NUMBER := 0;
    RegID           NUMBER := 1;
    BatchID         NUMBER := 1;
    LAction         CHAR;
    LRegNum         VARCHAR2(100) := '';
BEGIN
    LOOP
        BEGIN
            SAVEPOINT one_record;
            LXmlRows := TakeOffAndGetClob(LXmlCompReg,
                                          'MultiCompoundRegistryRecord');
            EXIT WHEN LXmlRows IS NULL;
            LXmlRows := '<MultiCompoundRegistryRecord>' || LXmlRows || '</MultiCompoundRegistryRecord>';
            SELECT EXTRACTVALUE(XMLTYPE.CreateXml(LXmlRows),
                                '/MultiCompoundRegistryRecord/ID[1]')
              INTO LATempID
              FROM DUAL;

            /* Commented out until the extra parameters are returned.
            LoadMultiCompoundRegRecord(LXmlRows, ADuplicateAction, ARegistration, ARegNumGeneration, AConfigurationID, ADuplicateCheck, IsDuplicate, RegID, BatchID);
            */
            LoadMultiCompoundRegRecord(LXmlRows, ADuplicateAction, LAction, LRegNum, ARegistration, ARegNumGeneration, AConfigurationID);

            -- Register Marked only, not DataLoader
            IF LATempID <> '0' THEN
                DeleteMultiCompoundRegTmp(LATempID);
            END IF;

            -- WJC This area may need to be rethought
            -- We have LRegNum but not RegID
            -- Need to map LAction into IsDuplicate
            -- Don't have BatchID
            LogBULKREGISTRATION(LogID, LATempID,  LAction, LRegNum, BatchID, 'Succeed');

        EXCEPTION
            WHEN OTHERS THEN
                ROLLBACK TO one_record;
                LogBULKREGISTRATION(LogID, LATempID, LAction, LRegNum, BatchID, SUBSTR (DBMS_UTILITY.Format_Error_Stack, 1, 500));
        END;
        COMMIT;
    END LOOP;
EXCEPTION
    WHEN OTHERS THEN
        Raise_Application_Error(eGenericException, DBMS_UTILITY.Format_Error_Stack);
END;

PROCEDURE RetrieveTempIDList(Ahitlistid IN Number, Aid OUT NOCOPY CLOB) IS
    Cursor temp_get_hitlistid(m_Id Number) Is
      Select Id
        From COEDB.COESAVEDHITLIST
       Where HITLISTID = m_Id;

    LTempIdList CLOB := '';

BEGIN
    For Lrow in temp_get_hitlistid(Ahitlistid)
    Loop
      LTempIdList := LTempIdList || Lrow.Id || ',';
    END LOOP;
    Aid := Rtrim(LTempIdList, ',');
EXCEPTION
    WHEN OTHERS THEN
        RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegList, DBMS_UTILITY.FORMAT_ERROR_STACK);
END;

PROCEDURE LogBULKREGITRATIONID(ALOG_ID OUT NUMBER, ADUPLICATE_ACTION IN Varchar2, AUSER_ID IN Varchar2) IS
    LALOG_ID NUMBER;
BEGIN
    INSERT INTO LOG_BULKREGISTRATION_ID
      (DUPLICATE_ACTION, USER_ID, DATETIME_STAMP)
    VALUES
      (ADUPLICATE_ACTION, AUSER_ID, sysdate)
    returning LOG_ID INTO LALOG_ID;

    ALOG_ID := LALOG_ID;
    Commit;
EXCEPTION
    WHEN OTHERS THEN
      Raise_Application_Error(eGenericException,
                              DBMS_UTILITY.Format_Error_Stack);
END;


PROCEDURE LogBULKREGISTRATION(LogID IN NUMBER, LATempID IN VARCHAR2, AAction IN char, RegNumber IN VARCHAR2, BatchID IN NUMBER, Result IN VARCHAR2) IS
BEGIN

    INSERT INTO LOG_BULKREGISTRATION
            (log_id, temp_id, action, reg_number, batch_number,
             comments
            )
     VALUES (LogID, LATempID, AAction, RegNumber, BatchID,
             Result
            );
END;

-- Gets a property list from the object definition in COEOBJECTCONFIG
-- APath is the path between MultiCompoundRegistryRecord and PropertyList eg. 'BatchList/Batch'
-- ARemoveTrailingComma will remove the trailing comma if TRUE
-- ATerm controls how each term is expanded
FUNCTION GetPropertyList(APath IN CLOB, ARemoveTrailingComma IN BOOLEAN := FALSE, ATerm IN CLOB := '<xsl:value-of select="@name"/>,') RETURN CLOB IS
  LReturn CLOB;
  LXml    CLOB;
  LXsl    CLOB := '
    <xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
      <xsl:template match="/MultiCompoundRegistryRecord">
       <xsl:for-each select="@PATH/PropertyList/Property">@TERM</xsl:for-each>
      </xsl:template>
    </xsl:stylesheet>
    ';
BEGIN
    -- Prepare XSL
    LXsl := Replace(LXsl, '@PATH', APath); -- eg. BatchList/Batch
    LXsl := Replace(LXsl, '@TERM', ATerm); -- eg. DECODE(<xsl:value-of select="@name"/>, NULL, '' '', <xsl:value-of select="@name"/>) <xsl:value-of select="@name"/>,
    -- Get XML
    SELECT XmlType.CreateXml(XML).GetClobVal()
      INTO LXml
      FROM COEOBJECTCONFIG
      WHERE ID=2;
    -- Transform
    SELECT XmlTransform(XmlType.CreateXml(LXml), XmlType.CreateXml(LXsl)).GetClobVal()
      INTO LReturn
      FROM DUAL;
    -- Remove trailing comma is requested
    IF ARemoveTrailingComma AND Length(LReturn) > 0 THEN
        LReturn := Rtrim(LReturn, ',');
    END IF;
    --
    RETURN LReturn;
END;

PROCEDURE RetrieveBatchCommon( AID IN NUMBER, AXml OUT NOCOPY CLOB, AIsTemp IN BOOLEAN) IS
    -- vary based on AIsTemp
    LDebugProcedure               CLOB;
    LBatchID                      CLOB;
    LCompoundID                   CLOB;
    LID                           CLOB;
    LMixtureComponentID           CLOB;
    LPersonRegistered             CLOB;
    LViewBatch                    CLOB;
    LViewCompound                 CLOB;
    -- result XML
    LXmlBatchComponentList        CLOB;
    LXmlBatch                     CLOB;
    -- SELECT phrases
    LBatchProperties              CLOB;
    LBatchComponentProperties     CLOB;
    -- for queries
    LQryCtx                       DBMS_XMLGEN.ctxHandle;
    LSelect                       CLOB;
    -- for XML Transforms
    LXslBatch                     CLOB := '
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/BATCH">
    <Batch>
      <BatchID>
        <xsl:for-each select="BATCHID">
          <xsl:value-of select="."/>
        </xsl:for-each>
      </BatchID>
      <BatchNumber>
        <xsl:for-each select="BATCHNUMBER">
          <xsl:value-of select="."/>
        </xsl:for-each>
      </BatchNumber>
      <DateCreated>
        <xsl:for-each select="DATECREATED">
          <xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
          <xsl:value-of select="."/>
        </xsl:for-each>
      </DateCreated>
      <xsl:variable name="VBATCH" select="."/>
      <PersonCreated>
        <xsl:for-each select="PERSONCREATED">
          <xsl:for-each select="$VBATCH/PERSONCREATEDDISPLAY">
            <xsl:attribute name="displayName"><xsl:value-of select="."/></xsl:attribute>
          </xsl:for-each>
          <xsl:value-of select="."/>
        </xsl:for-each>
      </PersonCreated>
      <PersonRegistered>
        <xsl:for-each select="PERSONREGISTERED">
          <xsl:for-each select="$VBATCH/PERSONREGISTEREDDISPLAY">
            <xsl:attribute name="displayName"><xsl:value-of select="."/></xsl:attribute>
          </xsl:for-each>
          <xsl:value-of select="."/>
        </xsl:for-each>
      </PersonRegistered>
      <DateLastModified>
        <xsl:for-each select="DATELASTMODIFIED">
          <xsl:variable name="V2" select="translate(., ''ABCDEFGHIJKLMNOPQRSTUVWXYZ'', ''abcdefghijklmnopqrstuvwxyz'')"/>
          <xsl:value-of select="."/>
        </xsl:for-each>
      </DateLastModified>
      <xsl:for-each select="PROPERTYLIST">
        <PropertyList>
          <xsl:for-each select="node()">
            LESS_THAN_SIGN;Property name="<xsl:variable name="V3" select="name()"/>
            <xsl:value-of select="name()"/>"GREATER_THAN_SIGN;<xsl:value-of select="."/>LESS_THAN_SIGN;/PropertyGREATER_THAN_SIGN;
          </xsl:for-each>
        </PropertyList>
      </xsl:for-each>
    </Batch>
  </xsl:template>
</xsl:stylesheet>
';
    LXslBatchComponentList        CLOB := '
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/BATCHCOMPONENTLIST">
    <BatchComponentList>
      <xsl:for-each select="BATCHCOMPONENT">
        <BatchComponent>
          <ID>
            <xsl:for-each select="ID">
              <xsl:value-of select="."/>
            </xsl:for-each>
          </ID>
          <BatchID>
            <xsl:for-each select="BATCHID">
              <xsl:value-of select="."/>
            </xsl:for-each>
          </BatchID>
          <MixtureComponentID>
            <xsl:for-each select="MIXTURECOMPONENTID">
              <xsl:value-of select="."/>
            </xsl:for-each>
          </MixtureComponentID>
          <CompoundID>
            <xsl:for-each select="COMPOUNDID">
              <xsl:value-of select="."/>
            </xsl:for-each>
          </CompoundID>
          <ComponentIndex>
            <xsl:for-each select="COMPONENTINDEX">
              <xsl:value-of select="."/>
            </xsl:for-each>
          </ComponentIndex>
          <xsl:for-each select="PROPERTYLIST">
            <PropertyList>
              <xsl:for-each select="node()">
                LESS_THAN_SIGN;Property name="<xsl:variable name="V3" select="name()"/>
                <xsl:value-of select="name()"/>"GREATER_THAN_SIGN;<xsl:value-of select="."/>LESS_THAN_SIGN;/PropertyGREATER_THAN_SIGN;
              </xsl:for-each>
            </PropertyList>
          </xsl:for-each>
        </BatchComponent>
      </xsl:for-each>
    </BatchComponentList>
  </xsl:template>
</xsl:stylesheet>
';
BEGIN
    -- Set variables based on temp versus perm
    IF AIsTemp THEN LDebugProcedure := 'RetrieveBatchCommon(temp)'; ELSE LDebugProcedure := 'RetrieveBatchCommon(perm)'; END IF;
    IF AIsTemp THEN LBatchID := 'TEMPBATCHID'; ELSE LBatchID := 'BATCHID'; END IF;
    IF AIsTemp THEN LCompoundID := 'TEMPCOMPOUNDID'; ELSE LCompoundID := '0'; END IF;
    IF AIsTemp THEN LID := '0'; ELSE LID := 'ID'; END IF;
    IF AIsTemp THEN LPersonRegistered := ''''''; ELSE LPersonRegistered := 'PERSONREGISTERED'; END IF;
    IF AIsTemp THEN LMixtureComponentID := '0'; ELSE LMixtureComponentID := 'MIXTURECOMPONENTID'; END IF;
    IF AIsTemp THEN LViewBatch := 'VW_TEMPORARYBATCH'; ELSE LViewBatch := 'VW_BATCH'; END IF;
    IF AIsTemp THEN LViewCompound := 'VW_TEMPORARYCOMPOUND'; ELSE LViewCompound := 'VW_BATCHCOMPONENT'; END IF;

    --
    -- Start Prepare SELECT phrases for PropertyLists
    --
    LBatchProperties := GetPropertyList('BatchList/Batch', FALSE, 'DECODE(<xsl:value-of select="@name"/>, NULL, '' '', <xsl:value-of select="@name"/>) <xsl:value-of select="@name"/>,');
    LBatchProperties := Replace(LBatchProperties, '&apos;', '''');
    LBatchProperties := Replace(LBatchProperties, Chr(10), '');
    LBatchComponentProperties := GetPropertyList('BatchList/Batch/BatchComponentList/BatchComponent', FALSE, 'DECODE(<xsl:value-of select="@name"/>, NULL, '' '', <xsl:value-of select="@name"/>) <xsl:value-of select="@name"/>,');
    LBatchComponentProperties := Replace(LBatchComponentProperties, '&apos;', '''');
    LBatchComponentProperties := Replace(LBatchComponentProperties, Chr(10), '');
    --
    -- End Prepare SELECT phrases for PropertyLists
    --

    --
    -- Start Batch (without BatchComponentList)
    --

    -- Fetch XML
    LSelect :=
      'SELECT ' ||
      LBatchID || ' AS BATCHID, ' ||
      'BATCHNUMBER, ' ||
      'DATECREATED, ' ||
      'PERSONCREATED, ' ||
      LPersonRegistered || ' AS PERSONREGISTERED, ' ||
      'DATELASTMODIFIED, ' ||
      '''BatchPropertyListBegin'' Aux,' || LBatchProperties || '''BatchPropertyListEnd'' Aux ' ||
      'FROM ' || LViewBatch || ' ' ||
      'WHERE ' || LBatchID || '=' || AID
      ;
    $if CompoundRegistry.Debuging $then InsertLog(LDebugProcedure,' LSelect(' || LViewBatch || ')=' || LSelect); $end null;

    LQryCtx := DBMS_XMLGEN.newContext(LSelect);

    LXmlBatch := Replace(DBMS_XMLGEN.getXML(LQryCtx), '<?xml version="1.0"?>', '');
    DBMS_XMLGEN.closeContext(LQryCtx);

    $if CompoundRegistry.Debuging $then InsertLog(LDebugProcedure,' LXmlBatch=' || LXmlBatch); $end null;

    IF LXmlBatch IS NULL THEN
        RAISE_APPLICATION_ERROR(eGenericException, 'No rows returned.' || CHR(10) || DBMS_UTILITY.FORMAT_ERROR_STACK);
    END IF;

    -- Manipulate XML
    LXmlBatch := Replace(LXmlBatch, '<ROWSET>', '');
    LXmlBatch := Replace(LXmlBatch, '<ROW>', '<BATCH>');
    LXmlBatch := Replace(LXmlBatch, '<PROJECTXML>', '');
    LXmlBatch := Replace(LXmlBatch, '</PROJECTXML>', '');
    LXmlBatch := Replace(LXmlBatch, '<AUX>BatchPropertyListBegin</AUX>', '<PROPERTYLIST>');
    LXmlBatch := Replace(LXmlBatch, '> <', '><');
    LXmlBatch := Replace(LXmlBatch, '<AUX>BatchPropertyListEnd</AUX>', '</PROPERTYLIST>');
    LXmlBatch := Replace(LXmlBatch, '</ROW>', '</BATCH>');
    LXmlBatch := Replace(LXmlBatch, '</ROWSET>', '');
    LXmlBatch := Trim(Chr(10) from LXmlBatch);

    -- Replace entities
    -- WJC PROJECTXML has entities but probably shouldn't
    LXmlBatch := Replace(LXmlBatch, '&quot;', '"');
    LXmlBatch := Replace(Replace(LXmlBatch, 'LESS_THAN_SIGN;', '<'), '&lt;', '<');
    LXmlBatch := Replace(Replace(LXmlBatch, 'GREATER_THAN_SIGN;', '>'), '&gt;', '>');

    -- Transform
    SELECT XmlTransform(XmlType.CreateXml(LXmlBatch), XmlType.CreateXml(LXslBatch)).GetClobVal() INTO LXmlBatch FROM DUAL;

    -- Replace entities
    -- WJC we're really only expecting LESS_THAN_SIGN and GREATER_THAN_SIGN at this point
    LXmlBatch := Replace(LXmlBatch, '&quot;', '"');
    LXmlBatch := Replace(Replace(LXmlBatch, 'LESS_THAN_SIGN;', '<'), '&lt;', '<');
    LXmlBatch := Replace(Replace(LXmlBatch, 'GREATER_THAN_SIGN;', '>'), '&gt;', '>');

    --
    -- End Batch (without BatchComponentList)
    --

    --
    -- Start BatchComponentList
    --

    -- Fetch XML
    LSelect :=
      'SELECT ' ||
      LID || ' AS ID, ' ||
      LBatchID || ' AS BATCHID, ' ||
      LMixtureComponentID || ' AS MixtureComponentID, ' ||
      LCompoundID || ' AS COMPOUNDID, ' ||
      '-' || LCompoundID || ' AS COMPONENTINDEX, ' ||
      '''PropertyListBegin'' Aux,' || LBatchComponentProperties || '''PropertyListEnd'' Aux ' ||
      'FROM ' || LViewCompound || ' ' ||
      'WHERE ' || LBatchID || '=' || AID
      ;

    $if CompoundRegistry.Debuging $then InsertLog(LDebugProcedure,' LSelect(' || LViewCompound || ')=' || LSelect); $end null;

    LQryCtx := DBMS_XMLGEN.newContext(LSelect);

    LXmlBatchComponentList := Replace(DBMS_XMLGEN.getXML(LQryCtx), '<?xml version="1.0"?>', '');
    DBMS_XMLGEN.closeContext(LQryCtx);

    $if CompoundRegistry.Debuging $then InsertLog(LDebugProcedure,' LXmlBatchComponentList=' || LXmlBatchComponentList); $end null;

    IF LXmlBatchComponentList IS NULL THEN
        RAISE_APPLICATION_ERROR(eGenericException, 'No rows returned.' || Chr(10) || DBMS_UTILITY.FORMAT_ERROR_STACK);
    END IF;

    -- Manipulate XML
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '<ROWSET>', '<BATCHCOMPONENTLIST>');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '<ROW>', '<BATCHCOMPONENT>');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '<AUX>PropertyListBegin</AUX>', '<PROPERTYLIST>');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '> <', '><');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '<AUX>PropertyListEnd</AUX>', '</PROPERTYLIST>');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '<BATCHCOMPFRAGMENTXML>', '');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '</BATCHCOMPFRAGMENTXML>', '');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '</ROW>', '</BATCHCOMPONENT>');
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '</ROWSET>', '</BATCHCOMPONENTLIST>');

    -- Replace entities
    -- WJC BATCHCOMPFRAGMENTXML has entities but probably shouldn't
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '&quot;', '"');
    LXmlBatchComponentList := Replace(Replace(LXmlBatchComponentList, 'LESS_THAN_SIGN;', '<'), '&lt;', '<');
    LXmlBatchComponentList := Replace(Replace(LXmlBatchComponentList, 'GREATER_THAN_SIGN;', '>'), '&gt;', '>');

    -- Transform
    SELECT XmlTransform(XmlType.CreateXml(LXmlBatchComponentList), XmlType.CreateXml(LXslBatchComponentList)).GetClobVal() INTO LXmlBatchComponentList FROM DUAL;

    -- Replace entities
    -- WJC we're really only expecting LESS_THAN_SIGN and GREATER_THAN_SIGN at this point
    LXmlBatchComponentList := Replace(LXmlBatchComponentList, '&quot;', '"');
    LXmlBatchComponentList := Replace(Replace(LXmlBatchComponentList, 'LESS_THAN_SIGN;', '<'), '&lt;', '<');
    LXmlBatchComponentList := Replace(Replace(LXmlBatchComponentList, 'GREATER_THAN_SIGN;', '>'), '&gt;', '>');

    --
    -- End BatchComponentList
    --

    -- Final assembly of complete Batch
    AXml := Replace(LXmlBatch, '</Batch>', LXmlBatchComponentList || '</Batch>');

    -- Quick cleanup
    -- WJC debugging only ?
    AXml := Replace(AXml, Chr(10), '');
    AXml := Replace(AXml, '>                ', '>');
    AXml := Replace(AXml, '>        ', '>');
    AXml := Replace(AXml, '>    ', '>');
    AXml := Replace(AXml, '>  ', '>');
    AXml := Replace(AXml, '> ', '>');
    AXml := Replace(AXml, '><', '>' || Chr(10) || '<');
    AXml := Replace(AXml, '>' || Chr(10) || '</', '></');

    $if CompoundRegistry.Debuging $then InsertLog(LDebugProcedure,'Batch:'|| chr(10)||AXml); $end null;

    RETURN;
END;

PROCEDURE RetrieveBatch( AID IN NUMBER, AXml OUT NOCOPY CLOB) IS
BEGIN
    RetrieveBatchCommon(AID, AXml, FALSE);
    RETURN;
END;

PROCEDURE RetrieveBatchTmp( ATempID IN NUMBER, AXml OUT NOCOPY CLOB) IS
BEGIN
    RetrieveBatchCommon(ATempID, AXml, TRUE);
    RETURN;
END;

PROCEDURE UpdateBatchCommon( AXml IN CLOB, AIsTemp IN BOOLEAN) IS
    -- vary based on AIsTemp
    LDebugProcedure               CLOB;
    LBatchID                      CLOB;
    LViewBatch                    CLOB;
    -- for XML Transforms
    LXslBatch                     CLOB := '
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
 <xsl:template match="/">
  UPDATE {ViewBatch}
  SET
  <xsl:for-each select="Batch/PropertyList/Property[@update=''yes'']"><xsl:value-of select="@name"/>=''<xsl:value-of select="."/>'',</xsl:for-each>{WHERE}
  {BatchID}=<xsl:value-of select="Batch/BatchID"/>
 </xsl:template>
</xsl:stylesheet>
';
    -- for queries
    LUpdate                       VARCHAR2(4000);
BEGIN
    -- Set variables based on temp versus perm
    IF AIsTemp THEN LDebugProcedure := 'UpdateBatchCommon(temp)'; ELSE LDebugProcedure := 'UpdateBatchCommon(perm)'; END IF;
    IF AIsTemp THEN LBatchID := 'TEMPBATCHID'; ELSE LBatchID := 'BATCHID'; END IF;
    IF AIsTemp THEN LViewBatch := 'VW_TEMPORARYBATCH'; ELSE LViewBatch := 'VW_BATCH'; END IF;

    -- Set XSL based on temp versus perm
    LXslBatch := Replace(LXslBatch, '{BatchID}', LBatchID);
    LXslBatch := Replace(LXslBatch, '{ViewBatch}', LViewBatch);

    -- Transform XML into SQL
    SELECT XmlTransform(XmlType.CreateXml(AXml), XmlType.CreateXml(LXslBatch)).GetClobVal() INTO LUpdate FROM DUAL;

    -- Fix apostrophes
    LUpdate := Replace(LUpdate, '&apos;', '''');
    -- Remove trailing comma in the SET clause
    LUpdate := Replace(LUpdate, ',{WHERE}', ' WHERE');

    EXECUTE IMMEDIATE LUpdate;

    RETURN;
END;

PROCEDURE UpdateBatch( AXml IN CLOB) IS
BEGIN
    UpdateBatchCommon(AXml, FALSE);
    RETURN;
END;

PROCEDURE UpdateBatchTmp( AXml IN CLOB) IS
BEGIN
    UpdateBatchCommon(AXml, TRUE);
    RETURN;
END;

END;
/