CREATE OR REPLACE
PACKAGE XMLUtils
AS                 

FUNCTION transformXML(pXSLT IN CLOB, pDoc IN CLOB, pParser DBMS_xmlparser.Parser, pEngine DBMS_xslprocessor.Processor) RETURN CLOB;
FUNCTION transformXML(pXSLT IN CLOB, pDoc IN DBMS_xmldom.DOMDocument, pParser IN DBMS_xmlparser.Parser, pEngine IN DBMS_xslprocessor.Processor) RETURN CLOB;
FUNCTION CLOB2DOC(pParser DBMS_xmlparser.Parser,	pClob CLOB) RETURN DBMS_xmldom.DOMDocument;
FUNCTION CLOB2NODE(pParser DBMS_xmlparser.Parser,pClob CLOB) RETURN DBMS_xmldom.DOMNode;
FUNCTION RemoveXMLElement(pParser DBMS_xmlparser.Parser, pXML IN CLOB) RETURN CLOB;
FUNCTION MERGE_XML_CLOBS(pClob1 CLOB, pClob2 CLOB) RETURN CLOB;
 		
END;
/
show errors;