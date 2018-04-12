CREATE OR REPLACE
PACKAGE BODY XMLUtils
AS

FUNCTION transformXML(pXSLT IN CLOB, pDoc IN CLOB, pParser IN xmlparser.Parser, pEngine IN xslprocessor.Processor) 
RETURN CLOB
IS
--vEngine xslprocessor.Processor := xslprocessor.newProcessor;
--vParser xmlparser.Parser := xmlparser.newParser;

vXSLTDoc xmldom.DOMDocument;
vXSLT xslprocessor.Stylesheet;      
vDoc xmldom.DOMDocument;
vResult CLOB;
BEGIN
	xmlparser.parseClob(pParser, pXSLT);
	vXSLTDoc := xmlparser.getDocument(pParser);
	vXSLT := xslprocessor.newStyleSheet(vXSLTDoc, null);
	xmlparser.parseClob(pParser, pDoc);
	vDoc := xmlparser.getDocument(pParser);
   	DBMS_LOB.CREATETEMPORARY(vResult, FALSE, DBMS_LOB.CALL);
	xslprocessor.processXSL(pEngine, vXSLT, vDoc, vResult);

	RETURN vResult;                                                                    
	
	xmldom.freeDocument(vXSLTDoc);
	xmldom.freeDocument(vDoc);
	dbms_lob.freetemporary(vResult);
    --xmlparser.freeParser(vParser);
	--xslprocessor.freeProcessor(vEngine);

END;

FUNCTION transformXML(pXSLT IN CLOB, pDoc IN xmldom.DOMDocument, pParser IN xmlparser.Parser, pEngine IN xslprocessor.Processor) 
RETURN CLOB
IS
--vEngine xslprocessor.Processor := xslprocessor.newProcessor;
--vParser xmlparser.Parser := xmlparser.newParser;

vXSLTDoc xmldom.DOMDocument;
vXSLT xslprocessor.Stylesheet;      
vResult CLOB;
BEGIN
	xmlparser.parseClob(pParser, pXSLT);
	vXSLTDoc := xmlparser.getDocument(pParser);
	vXSLT := xslprocessor.newStyleSheet(vXSLTDoc, null);
   	DBMS_LOB.CREATETEMPORARY(vResult, FALSE, DBMS_LOB.CALL);
	xslprocessor.processXSL(pEngine, vXSLT, pDoc, vResult);

	RETURN vResult;                                                                    
	
	xmldom.freeDocument(vXSLTDoc);
	dbms_lob.freetemporary(vResult);
    --xmlparser.freeParser(vParser);
	--xslprocessor.freeProcessor(vEngine);

END;

FUNCTION CLOB2NODE(
	pParser xmlparser.Parser,
	pClob CLOB)
RETURN xmldom.DOMNode
IS
vDoc xmldom.DOMDocument;
vNode xmldom.DOMNode;
BEGIN
	vDoc := CLOB2DOC(pParser, pClob);
	vNode := xmldom.makeNode(vDoc);
	RETURN vNode;
END CLOB2NODE;


FUNCTION CLOB2DOC(
	pParser xmlparser.Parser,
	pClob CLOB)
RETURN xmldom.DOMDocument
IS
vDoc xmldom.DOMDocument;
BEGIN
	xmlparser.parseClob(pParser,pClob);
	vDoc := xmlparser.getDocument(pParser);
	RETURN vDoc;
END CLOB2DOC;

FUNCTION RemoveXMLElement(pParser xmlparser.Parser, pXML IN CLOB) 
RETURN CLOB                                          
IS
vQueryNode xmldom.DOMNode;
vRowsetClob CLOB;
vRowsetNode xmldom.DOMNode;
BEGIN
	vQueryNode := CLOB2NODE(pParser, pXML);
	vRowsetNode := xmldom.getLastChild(vQueryNode);
   	DBMS_LOB.CREATETEMPORARY(vRowsetClob, FALSE, DBMS_LOB.CALL);
	xmldom.writeToClob(vRowsetNode,vRowsetClob); 
	
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