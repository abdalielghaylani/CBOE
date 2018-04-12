CREATE OR REPLACE
PACKAGE BODY XMLUtils
AS

FUNCTION transformXML(pXSLT IN CLOB, pDoc IN CLOB, pParser IN DBMS_xmlparser.Parser, pEngine IN DBMS_xslprocessor.Processor) 
RETURN CLOB
IS
--vEngine DBMS_xslprocessor.Processor := DBMS_xslprocessor.newProcessor;
--vParser DBMS_xmlparser.Parser := DBMS_xmlparser.newParser;

vXSLTDoc DBMS_xmldom.DOMDocument;
vXSLT DBMS_xslprocessor.Stylesheet;      
vDoc DBMS_xmldom.DOMDocument;
vResult CLOB;
BEGIN
	DBMS_xmlparser.parseClob(pParser, pXSLT);
	vXSLTDoc := DBMS_xmlparser.getDocument(pParser);
	vXSLT := DBMS_xslprocessor.newStyleSheet(vXSLTDoc, null);
	DBMS_xmlparser.parseClob(pParser, pDoc);
	vDoc := DBMS_xmlparser.getDocument(pParser);
   	DBMS_LOB.CREATETEMPORARY(vResult, FALSE, DBMS_LOB.CALL);
	DBMS_xslprocessor.processXSL(pEngine, vXSLT, vDoc, vResult);

	RETURN vResult;                                                                    
	
	DBMS_xmldom.freeDocument(vXSLTDoc);
	DBMS_xmldom.freeDocument(vDoc);
	dbms_lob.freetemporary(vResult);
    --DBMS_xmlparser.freeParser(vParser);
	--DBMS_xslprocessor.freeProcessor(vEngine);

END;

FUNCTION transformXML(pXSLT IN CLOB, pDoc IN DBMS_xmldom.DOMDocument, pParser IN DBMS_xmlparser.Parser, pEngine IN DBMS_xslprocessor.Processor) 
RETURN CLOB
IS
--vEngine DBMS_xslprocessor.Processor := DBMS_xslprocessor.newProcessor;
--vParser DBMS_xmlparser.Parser := DBMS_xmlparser.newParser;

vXSLTDoc DBMS_xmldom.DOMDocument;
vXSLT DBMS_xslprocessor.Stylesheet;      
vResult CLOB;
BEGIN
	DBMS_xmlparser.parseClob(pParser, pXSLT);
	vXSLTDoc := DBMS_xmlparser.getDocument(pParser);
	vXSLT := DBMS_xslprocessor.newStyleSheet(vXSLTDoc, null);
   	DBMS_LOB.CREATETEMPORARY(vResult, FALSE, DBMS_LOB.CALL);
	DBMS_xslprocessor.processXSL(pEngine, vXSLT, pDoc, vResult);

	RETURN vResult;                                                                    
	
	DBMS_xmldom.freeDocument(vXSLTDoc);
	dbms_lob.freetemporary(vResult);
    --DBMS_xmlparser.freeParser(vParser);
	--DBMS_xslprocessor.freeProcessor(vEngine);

END;

FUNCTION CLOB2NODE(
	pParser DBMS_xmlparser.Parser,
	pClob CLOB)
RETURN DBMS_xmldom.DOMNode
IS
vDoc DBMS_xmldom.DOMDocument;
vNode DBMS_xmldom.DOMNode;
BEGIN
	vDoc := CLOB2DOC(pParser, pClob);
	vNode := DBMS_xmldom.makeNode(vDoc);
	RETURN vNode;
END CLOB2NODE;


FUNCTION CLOB2DOC(
	pParser DBMS_xmlparser.Parser,
	pClob CLOB)
RETURN DBMS_xmldom.DOMDocument
IS
vDoc DBMS_xmldom.DOMDocument;
BEGIN
	DBMS_xmlparser.parseClob(pParser,pClob);
	vDoc := DBMS_xmlparser.getDocument(pParser);
	RETURN vDoc;
END CLOB2DOC;

FUNCTION RemoveXMLElement(pParser DBMS_xmlparser.Parser, pXML IN CLOB) 
RETURN CLOB                                          
IS
vQueryNode DBMS_xmldom.DOMNode;
vRowsetClob CLOB;
vRowsetNode DBMS_xmldom.DOMNode;
BEGIN
	vQueryNode := CLOB2NODE(pParser, pXML);
	vRowsetNode := DBMS_xmldom.getLastChild(vQueryNode);
   	DBMS_LOB.CREATETEMPORARY(vRowsetClob, FALSE, DBMS_LOB.CALL);
	DBMS_xmldom.writeToClob(vRowsetNode,vRowsetClob); 
	
	RETURN vRowsetClob;
END RemoveXMLElement;

FUNCTION MERGE_XML_CLOBS(
   	pClob1 CLOB,
	pClob2 CLOB)
RETURN CLOB
IS
vMergeClobStart CLOB;
vMergeClobEnd CLOB;
vMergeClob CLOB;
BEGIN

    SELECT xmldoc into vMergeClobStart FROM inv_xmldocs WHERE name = 'Merge Doc Start';
    SELECT xmldoc into vMergeClobEnd FROM inv_xmldocs WHERE name = 'Merge Doc End';
   	DBMS_LOB.CREATETEMPORARY(vMergeClob, FALSE, DBMS_LOB.CALL);
	DBMS_LOB.APPEND(vMergeClob, vMergeClobStart);
	DBMS_LOB.APPEND(vMergeClob, pClob1);
	DBMS_LOB.APPEND(vMergeClob, pClob2);
	DBMS_LOB.APPEND(vMergeClob, vMergeClobEnd);
	RETURN vMergeClob;

END MERGE_XML_CLOBS;
END;
/
show errors;