prompt 
prompt Starting "pkg_CompoundRegistry_body.sql"...
prompt

create or replace PACKAGE BODY             "COMPOUNDREGISTRY" IS
--query to find slow procedures in package
--select * from (
-- select  l.id, l.logdate,  (lead(logdate) over (order by id)) as next_date,
----to_number(substr(l.logprocedure,1,9)) as time1, 
---- (lead(to_number(substr(l.logprocedure,1,9))) over (order by id)) as time_next,
---- (lead(to_number(substr(l.logprocedure,1,9))) over (order by id))-to_number(substr(l.logprocedure,1,9)) as diff_time,
--round (( (lead(logdate) over (order by id)) - logdate )* 24 * 3600,2) as diff_date_sec,
-- to_char(substr(l.logprocedure,instr(l.logprocedure,'COMPOUNDREGISTRY.',1)+17,
-- case when instr(l.logprocedure,'_ended',1)=0 then instr(l.logprocedure,'_started',1) else instr(l.logprocedure,'_ended',1) end
-- -(instr(l.logprocedure,'COMPOUNDREGISTRY.',1)+17)
--)) as proc_name,run_seq,to_char(L.LOGPROCEDURE) as LOGPROCEDURE
-- from regdb.log l 
--where to_char(substr(l.logcomment,1,10)) in ('start','end')
-- )  where diff_date_sec >=1
-- ;

--Methods to *potentially* delete:
--  AddAttribPickList and all references
--  GetPropertyListFields and all references
--  GetFragmentXML

  /** XML Declaration constant */
  cXmlDecoration Constant Varchar2(30) := '<?xml version="1.0"?>';
  /** Special section list constant, signifying a 'write-only' action */
  cSectionListEmpty Constant Varchar(4) := 'NONE';

   LEnableRLS BOOLEAN;
  log_run_seq number;
  cSubmittedStatus Constant Number := 1;
  cApprovedStatus Constant Number := 2;
  cRegisteredStatus Constant Number := 3;
  cLockedStatus Constant Number := 4;

  /** System settings xml doc for Registration */
  VSystemSettings XMLTYPE;
  /** System setting: true if Mixtures have been enabled */
  vMixturesEnabled VARCHAR2(200);
  /** System setting: True if Approvals have been enabled */
  vApprovalsEnabled VARCHAR2(200);
  /** System setting: padding for 'batch number' */
  vBatchNumberPad INTEGER;
  /** System setting: true if each batch's components must have identical fragments */
  vSameBatchesIdentity VARCHAR2(200);
  /** System setting: false if bypassing duplicate checking */
  vDupCheckingEnabled VARCHAR2(200);
  /** System setting: list of key=value pairs used by the CsCartridge*/
  vDupCheckingSettingList VARCHAR2(2000);
  /** System setting: true if Oracle's row-level-security is enabled */
  vActiveRLS VARCHAR2(200);
  /** System setting: true if allow unregistered Compounds in Mixtures */
  vAllowUnregistCompInMix VARCHAR2(200);
  /** System setting: true if is able use of prefixes in batches */
  vEnableUseBatchPrefixes VARCHAR2(200);
  /** XML template for new registration objects */
  vRegObjectTemplate XMLTYPE;

  -- custom property lists; also, lists of those that are picklists
  vMixtureFields CLOB;
  vMixturePicklistFields CLOB;
  vCompoundFields CLOB;
  vCompoundPicklistFields CLOB;
  vStructureFields CLOB;
  vStructurePicklistFields CLOB;
  vBatchFields CLOB;
  vBatchPicklistFields CLOB;
  vBatchComponentFields CLOB;
  vBatchComponentPicklistFields CLOB;

  -- complete PropertyList from CoeObjectConfig
  vPropsMixture XmlType;
  vPropsMixtureCsv clob;
  vPropsComponent XmlType;
  vPropsComponentCsv clob;
  vPropsStructure XmlType;
  vPropsStructureCsv clob;
  vPropsBatch XmlType;
  vPropsBatchCsv clob;
  vPropsBatchComponent XmlType;
  vPropsBatchComponentCsv clob;

  -- XSLTs
  vXslCreateMcrr XmlType;
  vXslRetrieveMcrr XmlType;
  vXslUpdateMcrr XmlType;

  vXslCreateMcrrTemp XmlType;
  vXslRetrieveMcrrTemp XmlType;
  vXslUpdateMcrrTemp XmlType;

  -- Forward declarations: functions
  FUNCTION TakeOffAndGetClob(AXml IN OUT NOCOPY Clob,ABeginTag VARCHAR2) RETURN CLOB;
  FUNCTION XslMcrrFetch RETURN XMLTYPE;
  FUNCTION XslMcrrTempFetch RETURN XMLTYPE;
  FUNCTION XslMcrrCreate RETURN XMLTYPE;
  FUNCTION XslMcrrTempCreate RETURN XMLTYPE;
  FUNCTION XslMcrrUpdate RETURN XMLTYPE;
  FUNCTION XslMcrrTempUpdate RETURN XMLTYPE;
  FUNCTION CanRegister(ATempID in VW_TemporaryBatch.tempbatchid%type) RETURN VARCHAR2;
  FUNCTION CanDeleteTemp(ATempID in VW_TemporaryBatch.tempbatchid%type) RETURN VARCHAR2;
  FUNCTION GetApprovalsEnabled RETURN VARCHAR2;
  FUNCTION CheckBatchPrefixForRegister(ATempID in VW_TemporaryBatch.tempbatchid%type) RETURN VARCHAR2;
  -- Forward declarations: procedures
  PROCEDURE TraceWrite(AMethodName Varchar2, APlSqlLine Integer, ALogComment CLOB);
  PROCEDURE GetBatchIDListByMixtureId(p_mixid IN NUMBER, p_batchId_list OUT tNumericIdList);
  PROCEDURE UpdateBatchRegNumbers(p_batchId_list IN TNumericIdList);

  /**
  Retrieves the xml metadata describing the property lists available for each
  domain object that supports one.
  -->author jed
  -->since January 2011
  -->return an xml document (of xmltype)
  */
  FUNCTION GetRegObjectTemplate RETURN XMLTYPE IS
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
     dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetRegObjectTemplate_started', $$plsql_line,'start' );
    IF (VRegObjectTemplate IS NULL) THEN
      SELECT XmlType.CreateXml(XML) INTO VRegObjectTemplate
      FROM COEOBJECTCONFIG WHERE ID=2;
      TraceWrite('GetRegObjectTemplate', $$plsql_line, 'COEOBJECTCONFIG from table');
    ELSE
      TraceWrite('GetRegObjectTemplate', $$plsql_line, 'COEOBJECTCONFIG from cache');
    END IF;
  TraceWrite( act||'GetRegObjectTemplate_ended', $$plsql_line,'end' );
    RETURN VRegObjectTemplate;
 end;

 /** function to add necessary attribs for batches. For improvment of performance*/
 function add_attrib(iFieldName VARCHAR2,iFieldList clob,iAlias VARCHAR2:=null) return varchar2
  is
    var1 clob ;
    l_FieldList clob :=replace(upper(iFieldList),' ','');
    l_FieldName VARCHAR2(32000) :=','||replace(upper(iFieldName),' ','')||':';
    l_var VARCHAR2(32000) ;
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
     dbms_application_info.read_module(mod1, act); 
     TraceWrite( act||'add_attrib_started', $$plsql_line,'start' );
    if l_FieldList is not null then
    l_FieldList  := case when ',' = substr(l_FieldList,1,1) then '' else ',' end || l_FieldList;
    end if;
    if l_FieldList like '%'||l_FieldName||'%' and l_FieldList is not null then
      l_var := substr(l_FieldList,instr(l_FieldList,l_FieldName)+length(l_FieldName));
      l_var := substr(l_var,1,instr(l_var,',',1,1)-1);
      begin
        select case when Ext_Display_Col is not null and Ext_Table  is not null and Ext_ID_Col is not null then
          'select '|| Ext_Display_Col || ' FROM ' || Ext_Table  ||' WHERE ' || Ext_ID_Col || ' = '||iAlias||iFieldName
                 else null end into var1
        from VW_PickListDomain pd where pd.id=to_number(l_var);
      exception when NO_DATA_FOUND then
        var1:=null;
      end;
      if var1 is not null then
        var1 := q'[ case when ]'||iFieldName||q'[ is not null then xmlelement("]'||iFieldName||
        q'[", XMLATTRIBUTES(']'||to_char(l_var)||q'[' "pickListDomainID",(]'||var1
        ||q'[)]'||q'[  "pickListDisplayValue" ),]'||iFieldName||q'[) else  xmlelement("]'||iFieldName||q'[",]'||iFieldName||q'[) end]';

       var1 := var1;
       else
         var1 := q'[ xmlelement("]'||iFieldName||q'[",]'||iFieldName||q'[)]';
      end if;
    else
      var1 := q'[ xmlelement("]'||iFieldName||q'[",]'||iFieldName||q'[)]';
    end if;
    TraceWrite( act||'add_attrib_ended', $$plsql_line,'end' );
    return var1;
  end;


  /**
  Provides the fields and the picklist-dependent fields as CSVs, depsnding on the object
  level specified.
  -->author jed
  -->since January 2011
  -->param
  */
  PROCEDURE GetPropertyListFields(
    ALevel IN Varchar2
    , APrototypeXml XMLTYPE
    , AFields OUT CLOB
    , APicklistFields OUT CLOB
  ) IS
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
     dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetPropertyListFields_started', $$plsql_line,'start' );

    case ALevel
      when 'Mixture' then
        begin
          if (VMixtureFields is null) then
            select
            XmlTransform(APrototypeXml, XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/MultiCompoundRegistryRecord">
                  <xsl:for-each select="PropertyList/Property">
                    <xsl:value-of select="@name"/>,</xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')
            ).GetStringVal(),
            XmlTransform(APrototypeXml, XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/MultiCompoundRegistryRecord">
                  <xsl:for-each select="PropertyList/Property[@pickListDomainID!='''']"><xsl:value-of select="@name"/>:<xsl:value-of select="@pickListDomainID"/>, </xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')
            ).GetStringVal()
            into VMixtureFields, VMixturePicklistFields
            from dual;

            VMixtureFields := SUBSTR(VMixtureFields, 1, LENGTH(VMixtureFields)-1);
          end if;
          AFields := VMixtureFields;
          APicklistFields := VMixturePicklistFields;
        end;
      when 'Compound' then
        begin
          if (VCompoundFields is null) then
            select
            XmlTransform(APrototypeXml, XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/MultiCompoundRegistryRecord">
                  <xsl:for-each select="ComponentList/Component/Compound/PropertyList/Property">
                    <xsl:value-of select="@name"/>,</xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')).GetStringVal(),
            XmlTransform(APrototypeXml, XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/MultiCompoundRegistryRecord">
                  <xsl:for-each select="ComponentList/Component/Compound/PropertyList/Property[@pickListDomainID!='''']"><xsl:value-of select="@name"/>:<xsl:value-of select="@pickListDomainID"/>, </xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')).GetStringVal()
            into VCompoundFields, VCompoundPicklistFields
            from dual;

            VCompoundFields := SUBSTR(VCompoundFields, 1, LENGTH(VCompoundFields)-1);
          end if;
          AFields := VCompoundFields;
          APicklistFields := VCompoundPicklistFields;
        end;
      when 'Structure' then
        begin
          if (VStructureFields is null) then
            select
            XmlTransform(APrototypeXml, XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/MultiCompoundRegistryRecord">
                  <xsl:for-each select="ComponentList/Component/Compound/BaseFragment/Structure/PropertyList/Property">
                    <xsl:value-of select="@name"/>,</xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')).GetStringVal(),
            XmlTransform(APrototypeXml, XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/MultiCompoundRegistryRecord">
                  <xsl:for-each select="ComponentList/Component/Compound/BaseFragment/Structure/PropertyList/Property[@pickListDomainID!='''']"><xsl:value-of select="@name"/>:<xsl:value-of select="@pickListDomainID"/>, </xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')).GetStringVal()
            into VStructureFields, VStructurePicklistFields
            from dual;

            VStructureFields := SUBSTR(VStructureFields, 1, LENGTH(VStructureFields)-1);
        end if;
          AFields := VStructureFields;
          APicklistFields := VStructurePicklistFields;
        end;
      when 'Batch' then
        begin
          if (VBatchFields is null) then
            select
            XmlTransform(APrototypeXml, XmlType.CreateXml('
            <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
              <xsl:template match="/MultiCompoundRegistryRecord">
                <xsl:for-each select="BatchList/Batch/PropertyList/Property">
                  <xsl:value-of select="@name"/>,</xsl:for-each>
              </xsl:template>
            </xsl:stylesheet>')).GetStringVal(),
            XmlTransform(APrototypeXml, XmlType.CreateXml('
            <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
              <xsl:template match="/MultiCompoundRegistryRecord">
                <xsl:for-each select="BatchList/Batch/PropertyList/Property[@pickListDomainID!='''']"><xsl:value-of select="@name"/>:<xsl:value-of select="@pickListDomainID"/>, </xsl:for-each>
              </xsl:template>
            </xsl:stylesheet>')).GetStringVal()
            into VBatchFields, VBatchPicklistFields
            from dual;

            VBatchFields := SUBSTR(VBatchFields, 1, LENGTH(VBatchFields)-1);
          end if;
          AFields := VBatchFields;
          APicklistFields := VBatchPicklistFields;
        end;
      when 'BatchComponent' then
        begin
          if (VBatchComponentFields is null) then
            select
            XmlTransform(APrototypeXml, XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/MultiCompoundRegistryRecord">
                  <xsl:for-each select="BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property">
                    <xsl:value-of select="@name"/>,</xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')).GetStringVal(),
            XmlTransform(APrototypeXml, XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/MultiCompoundRegistryRecord">
                  <xsl:for-each select="BatchList/Batch/BatchComponentList/BatchComponent/PropertyList/Property[@pickListDomainID!='''']"><xsl:value-of select="@name"/>:<xsl:value-of select="@pickListDomainID"/>, </xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')).GetStringVal()
            into VBatchComponentFields, VBatchComponentPicklistFields
            from dual;

            VBatchComponentFields := SUBSTR(VBatchComponentFields, 1, LENGTH(VBatchComponentFields)-1);
          end if;
          AFields := VBatchComponentFields;
          APicklistFields := VBatchComponentPicklistFields;
        end;
    end case;

  TraceWrite( act||'GetPropertyListFields_ended', $$plsql_line,'end' );
  end;

  /**
  Given an 'owner' designation and its related DB identifier, returns a
  data-filled PropertyList element.
  -->author jed
  -->since January 2011
  -->param pPropertyListType
  -->param pParentIdentifier
  -->param pPrototypeXml
  */
  FUNCTION GetPropertyList(
    pPropertyListType IN varchar2
    , pParentIdentifier IN number
    , pPrototypeXml IN XmlType
  ) RETURN XMLTYPE
  IS
    v_ctx dbms_xmlgen.ctxHandle;
    -- we will ALWAYS override the value for v_propertyFields, so this is just CLOB initialization
    v_propertyFields CLOB := '*';
    v_dynamicSql varchar2(4000);
    v_queryOutput CLOB;
    v_propertyListMetadata XMLTYPE;
    v_type varchar2(200) := upper(pPropertyListType);
    v_node XMLTYPE;
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetPropertyList_started', $$plsql_line,'start' );

    -- try to use an existing CSV of PropertyList fields
    case
      when (v_type = 'MIXTURE' or v_type = 'TEMPMIXTURE') then
        begin
          if (VPropsMixture is null) then
            select extract(pPrototypeXml, '/MultiCompoundRegistryRecord/PropertyList')
            into VPropsMixture from dual;
          end if;
          v_propertyListMetadata := VPropsMixture;
          v_propertyFields := vPropsMixtureCsv;
        end;

      when (v_type = 'COMPONENT' or v_type = 'TEMPCOMPONENT') then
        begin
          if (VPropsComponent is null) then
            select extract(pPrototypeXml, '/MultiCompoundRegistryRecord/ComponentList/Component/Compound/PropertyList')
            into VPropsComponent from dual;
          end if;
          v_propertyListMetadata := VPropsComponent;
          v_propertyFields := vPropsComponentCsv;
        end;

      when (v_type = 'STRUCTURE' or v_type = 'TEMPSTRUCTURE') then
        begin
          if (VPropsStructure is null) then
            select extract(pPrototypeXml, '/MultiCompoundRegistryRecord/ComponentList/Component/Compound/BaseFragment/Structure/PropertyList')
            into VPropsStructure from dual;
          end if;
          v_propertyListMetadata := VPropsStructure;
          v_propertyFields := vPropsStructureCsv;
        end;

      when (v_type = 'BATCH' or v_type = 'TEMPBATCH') then
        begin
          if (VPropsBatch is null) then
            select extract(pPrototypeXml, '/MultiCompoundRegistryRecord/BatchList/Batch/PropertyList')
            into VPropsBatch from dual;
          end if;
          v_propertyListMetadata := VPropsBatch;
          v_propertyFields := vPropsBatchCsv;
        end;

      when (v_type = 'BATCHCOMPONENT' or v_type = 'TEMPBATCHCOMPONENT') then
        begin
          if (VPropsBatchComponent is null) then
            select extract(pPrototypeXml, '/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/PropertyList')
            into VPropsBatchComponent from dual;
          end if;
          v_propertyListMetadata := VPropsBatchComponent;
          v_propertyFields := vPropsBatchComponentCsv;
        end;
      else NULL;
    end case;

    TraceWrite('GetFilledPropertyList_Metadata', $$plsql_line, v_propertyListMetadata.getClobVal());

    if (v_propertyFields is null) then
      -- lazy-load the appropriate custom field list
      select xmlquery(
        'fn:string-join(//PropertyList/Property/@name,",")'
        passing v_propertyListMetadata
        returning content
      ).getStringVal() cols into v_propertyFields from dual;

      -- save the custom field list for re-use in the session
      case
        when (v_type = 'MIXTURE' or v_type = 'TEMPMIXTURE') then begin vPropsMixtureCsv := v_propertyFields; end;
        when (v_type = 'COMPONENT' or v_type = 'TEMPCOMPONENT') then begin vPropsComponentCsv := v_propertyFields; end;
        when (v_type = 'STRUCTURE' or v_type = 'TEMPSTRUCTURE') then begin vPropsStructureCsv := v_propertyFields; end;
        when (v_type = 'BATCH' or v_type = 'TEMPBATCH') then begin vPropsBatchCsv := v_propertyFields; end;
        when (v_type = 'BATCHCOMPONENT' or v_type = 'TEMPBATCHCOMPONENT') then begin vPropsBatchComponentCsv := v_propertyFields; end;
      end case;
    end if;
    --If no properties configured in a section v_dynamicSql will need to select row with 'null' column
    if (v_propertyFields is null) then
      v_propertyFields :='null';
    end if;
    -- extract the actual data
    v_dynamicSql := 'select '
      || v_propertyFields
      ||
      case v_type
        when 'MIXTURE' then ' from vw_mixture v where v.regid ='
        when 'COMPONENT' then ' from vw_compound v where v.regid ='
        when 'STRUCTURE' then ' from vw_structure_drawing v where v.structureid = '
        when 'BATCH' then ' from vw_batch v where v.batchid ='
        when 'BATCHCOMPONENT' then ' from vw_batchcomponent v where v.id ='
        when 'TEMPMIXTURE' then ' from vw_temporarybatch v where v.tempbatchid ='
        when 'TEMPCOMPONENT' then ' from vw_temporarycompound v where v.tempcompoundid ='
        when 'TEMPSTRUCTURE' then ' from vw_temporarycompound v where v.tempcompoundid ='
        when 'TEMPBATCH' then ' from vw_temporarybatch v where v.tempbatchid ='
        when 'TEMPBATCHCOMPONENT' then ' from vw_temporarycompound v where v.tempcompoundid ='
      end
      || ' :bindvar';

    TraceWrite('GetFilledPropertyList_SQL', $$plsql_line, v_dynamicSql);

    v_ctx := dbms_xmlgen.newContext(v_dynamicSql);
    dbms_xmlgen.setBindValue(v_ctx, 'bindvar', to_char(pParentIdentifier));
    v_queryOutput := dbms_xmlgen.getxml(v_ctx);
    dbms_xmlgen.closeContext(v_ctx);

    v_queryOutput := replace(v_queryOutput, cXmlDecoration, null);
    TraceWrite('GetFilledPropertyList_Output', $$plsql_line, v_queryOutput);

    -- join the metadata and the actual data on the property names
    select
      xmlelement("PropertyList",
        xmlagg(
          case
            when existsNode(metaData."markup", '/Property/text()') = 1 then
              updateXML(metaData."markup", '/Property/text()', propInfo."value")
            when trim(propInfo."value") is null then
              metaData."markup"
            else
              appendChildXML( metaData."markup", '/Property', extract(propInfo."value", '//text()') )
          end
        )
      )
    into v_node
    from (
      xmltable(
        'for $i in /PropertyList return $i/node()'
        passing
          v_propertyListMetadata
        columns
          "name" varchar2(200) path '@name',
          "markup" xmltype path '.'
        )
    ) metaData
    left outer join (
      select props.* from
      xmltable(
          'for $i in /ROWSET/ROW/* return
            element Field {
              element Name {name($i)},
              element Value {$i}
              }'
          passing XMLTYPE(v_queryOutput)
          columns
            "name" varchar2(200) path '/Field/Name/text()',
            "value" xmltype path '/Field/Value/*'
        ) props
    ) propInfo on propInfo."name" = metaData."name";

    TraceWrite('GetFilledPropertyList', $$plsql_line, v_node.getClobVal());

    TraceWrite( act||'GetPropertyList_ended', $$plsql_line,'end' );
    RETURN v_node;
  end;

  FUNCTION GetFilledPropertyList(
    APropertyListType IN varchar2
    , AParentIdentifier IN number
  ) RETURN CLOB IS
    v_xbuf xmltype;
    v_buf clob;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetFilledPropertyList_started', $$plsql_line,'start' );
--    v_xbuf := GetPropertyList(APropertyListType, AParentIdentifier, GetRegObjectTemplate);
   v_xbuf := GetPropertyList(APropertyListType, AParentIdentifier, VRegObjectTemplate);
    if (v_xbuf is not null) then
      v_buf := v_xbuf.getClobVal();
    end if;
    v_buf := replace(v_buf, cXmlDecoration, null);
    TraceWrite('GetFilledPropertyList', $$plsql_line, v_buf);
  TraceWrite( act||'GetFilledPropertyList_ended', $$plsql_line,'end' );
    RETURN v_buf;
  end;

  /**
  Retrieves the xml metadata describing the Registration system settings.
  -->author jed
  -->since January 2011
  -->return an xml document (of xmltype)
  */
  FUNCTION GetSystemSettings RETURN XMLTYPE IS
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetSystemSettings_started', $$plsql_line,'start' );
    IF (VSystemSettings IS NULL) THEN
      SELECT c.configurationxml INTO VSystemSettings
      FROM coedb.coeconfiguration c
      WHERE UPPER(c.description) = 'REGISTRATION';
    END IF;
    TraceWrite( act||'GetSystemSettings_ended', $$plsql_line,'end' );
    RETURN VSystemSettings;
  end;

  /**
  Utility for providing a value for a Registration configuration setting
  -->author jed
  -->since December 2010
  -->return an integer value
  */
  FUNCTION GetMixturesEnabled RETURN VARCHAR2 IS
    LValue CLOB;
    LSettings XMLTYPE;
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN dbms_application_info.read_module(mod1, act); TraceWrite( act||'_started', $$plsql_line,'start' );
    IF (vMixturesEnabled is NULL) THEN
      LSettings := VSystemSettings;
      SELECT Extract(
        LSettings,
        'Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="EnableMixtures"]/@value'
      ).GetClobVal() INTO LValue
      FROM DUAl;
      vMixturesEnabled := to_char(LValue);
    END IF;
    RETURN vMixturesEnabled;
  TraceWrite( act||'_ended', $$plsql_line,'end' );
  end;

  /**
  Utility for providing a value for a Registration configuration setting
  -->author fari
  -->since December 2012
  -->return an integer value
  */
  FUNCTION GetEnableUseBatchPrefixes RETURN VARCHAR2 IS
    LValue CLOB;
    LSettings XMLTYPE;
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetEnableUseBatchPrefixes_started', $$plsql_line,'start' );
    IF (vEnableUseBatchPrefixes is NULL) THEN
      LSettings := VSystemSettings;
      SELECT Extract(
        LSettings,
        'Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="EnableUseBatchPrefixes"]/@value'
      ).GetClobVal() INTO LValue
      FROM DUAl;
      vEnableUseBatchPrefixes := to_char(LValue);
    END IF;
    RETURN vEnableUseBatchPrefixes;
  TraceWrite( act||'GetEnableUseBatchPrefixes_ended', $$plsql_line,'end' );
  end;

 /**
  Utility for providing a value for a Registration configuration setting
  -->
  -->
  -->
  */
  FUNCTION GetBatchPrefixValue(
    p_batchid number)

    RETURN VARCHAR2 IS
    v_batch_prefix_setting varchar(100):=null;
    v_batch_prefix_value varchar(100):='';
    v_execute_sql varchar(200):='';
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetBatchPrefixValue_started', $$plsql_line,'start' );
    v_batch_prefix_setting := GetEnableUseBatchPrefixes;

    IF upper(v_batch_prefix_setting) ='TRUE' THEN
         --v_execute_sql :='select p.picklistvalue   from vw_batch vb,picklist p where vb.batch_prefix=p.id and vb.batchid=' || to_char(p_batchid)|| 'into v_batch_prefix_value';
         --execute immediate v_execute_sql;
         execute immediate 'select p.picklistvalue from vw_batch vb,picklist p where vb.batch_prefix=p.id and vb.batchid='||to_char(p_batchid) into v_batch_prefix_value ;

    END IF;
    TraceWrite( act||'GetBatchPrefixValue_ended', $$plsql_line,'end' );
    RETURN v_batch_prefix_value;
     EXCEPTION
       WHEN OTHERS THEN NULL; -- The batch  prefix value can be null so no row selected exception can be thrown
        v_batch_prefix_value :='';
        RETURN v_batch_prefix_value;
     END;

  FUNCTION CheckBatchPrefixForRegister(ATempID in VW_TemporaryBatch.tempbatchid%type) RETURN VARCHAR2 IS
    v_batch_prefix_setting varchar(100):=null;
    v_batch_prefix_value varchar(100):='TRUE';
    v_execute_sql varchar(200):='';
    mod1 varchar2(100);
	act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'CheckBatchPrefixForRegister_started', $$plsql_line,'start' );
    v_batch_prefix_setting := GetEnableUseBatchPrefixes;

    IF upper(v_batch_prefix_setting) ='TRUE' THEN
         execute immediate 'select vb.batch_prefix from VW_TEMPORARYBATCH vb where vb.tempbatchid='||to_char(ATempID) into v_batch_prefix_value ;
    END IF;
    IF (v_batch_prefix_value is null)THEN
       v_batch_prefix_value :='FALSE';
    ELSE
       v_batch_prefix_value :='TRUE';
    END IF;
    TraceWrite( act||'CheckBatchPrefixForRegister_ended', $$plsql_line,'end' );
    RETURN v_batch_prefix_value;
     EXCEPTION
       WHEN OTHERS THEN NULL; -- The batch  prefix value can be null so no row selected exception can be thrown
        v_batch_prefix_value :='';
        RETURN v_batch_prefix_value;
     END;
  FUNCTION GetSaltAndSuffixValue(
    p_batchid number)

    RETURN VARCHAR2 IS
    v_batch_prefix_setting varchar(100):=null;
    v_salt_suffix_value varchar(100):='';
    v_execute_sql varchar(200):='';
    mod1 varchar2(100);
	act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetSaltAndSuffixValue_started', $$plsql_line,'start' );
    v_batch_prefix_setting := GetEnableUseBatchPrefixes;

    IF upper(v_batch_prefix_setting) ='TRUE' THEN
         execute immediate 'select vb.saltandbatchsuffix  from vw_batch vb where vb.batchid='||to_char(p_batchid) into v_salt_suffix_value;
    END IF;
    TraceWrite( act||'GetSaltAndSuffixValue_ended', $$plsql_line,'end' );
    RETURN v_salt_suffix_value;
    EXCEPTION
      WHEN OTHERS THEN NULL;-- There may be some scenarion in which saltandbatchsuffix field doesnot set for the batches table.
      v_salt_suffix_value :='';
      RETURN v_salt_suffix_value;
  END;
  /**
  Assembles the structure-matching parameters into a comma-separated values string
  -->author jed
  -->since January 2011
  -->return a CSV-string of key/value pairs the CsCartridge uses for structure-matching
  */
  FUNCTION GetStructureDupCheckSettings RETURN VARCHAR2 IS
    LValue VARCHAR2(2000);
    LSettings XMLTYPE;
    mod1 varchar2(100);
	act varchar2(100);
   BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetStructureDupCheckSettings_started', $$plsql_line,'start' );
    IF (VDupCheckingSettingList is NULL) THEN
      LSettings := VSystemSettings;
      SELECT XmlTransform(
        Extract(
          LSettings,
          'Registration/applicationSettings/groups/add[@name="DUPLICATE_CHECKING"]/settings'
        )
        , XmlType.CreateXml('
          <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
            <xsl:template match="/settings">
              <xsl:for-each select="add">
                <xsl:choose>
                  <xsl:when test="@name!= ''''">
                    <xsl:value-of select="@name"/>=<xsl:value-of select="@value"/>,</xsl:when>
                </xsl:choose>
              </xsl:for-each>
            </xsl:template>
          </xsl:stylesheet>')
      ).GetStringVal() INTO LValue FROM DUAL;

      TRaceWrite('GetStructureDupCheckSettings', $$plsql_line, LValue);

      VDupCheckingSettingList := SUBSTR(LValue, 1, LENGTH(LValue)-1);
    END IF;
    TraceWrite( act||'GetStructureDupCheckSettings_ended', $$plsql_line,'end' );
    RETURN VDupCheckingSettingList;
  END;

  /**
  Get the value of "SameBatchesIdentity" from the "Application Settings" of Registration.
  -->author Fari
  -->since March 2010
  */
  FUNCTION GetSameBatchesIdentity RETURN VARCHAR2 IS
    LValue CLOB;
    LSettings XMLTYPE;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetSameBatchesIdentity_started', $$plsql_line,'start' );
    IF (VSameBatchesIdentity is NULL) THEN
      LSettings := VSystemSettings;
      SELECT Extract(
        LSettings,
        'Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="SameBatchesIdentity"]/@value'
      ).GetStringVal() INTO LValue
      FROM DUAl;
      VSameBatchesIdentity := upper(to_char(LValue));
    END IF;
    TraceWrite( act||'GetSameBatchesIdentity_ended', $$plsql_line,'end' );
    RETURN VSameBatchesIdentity;
  END;

  /**
  Get the value of "CheckDuplication" from the "Application Settings" of Registration.
  "CheckDuplication" setting enables validation of duplication of structures and mixtures with identical compound before of the registration." allowedValues="True|False" isAdmin="False"/>                    <add name="FragmentsUsed
  -->author Fari
  -->since July 2010
  */
  FUNCTION GetDuplicateCheckEnable RETURN VARCHAR2 IS
    LValue CLOB;
    LSettings XMLTYPE;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetDuplicateCheckEnable_started', $$plsql_line,'start' );
    IF (VDupCheckingEnabled is NULL) THEN
      LSettings := VSystemSettings;
      SELECT Extract(
        LSettings,
        'Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="CheckDuplication"]/@value'
      ).GetClobVal() INTO LValue
      FROM DUAl;
      VDupCheckingEnabled := to_char(LValue);
    END IF;
    TraceWrite( act||'GetDuplicateCheckEnable_ended', $$plsql_line,'end' );
    RETURN VDupCheckingEnabled;
  END;

  /**
  Get the value of "ActiveRLS" from the "Application Settings" of Registration.
  -->author Fari
  -->since September 2010
  */
  FUNCTION GetActiveRLS RETURN VARCHAR2 IS
    LValue CLOB;
    LSettings XMLTYPE;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetActiveRLS_started', $$plsql_line,'start' );
    IF (VActiveRLS is NULL) THEN
      LSettings := VSystemSettings;
      SELECT Extract(
        LSettings,
        'Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="ActiveRLS"]/@value'
      ).GetStringVal() INTO LValue
      FROM DUAl;
      VActiveRLS := to_char(LValue);
    END IF;
    TraceWrite( act||'GetActiveRLS_ended', $$plsql_line,'end' );
    RETURN VActiveRLS;
  END;

  /**
  Get the value of "AllowUnregisteredComponentsInMixtures" from the "Application Settings" of Registration.
  "AllowUnregisteredComponentsInMixtures" setting enables validation of Unregistered Components In Mixtures
  -->author fari
  -->since September 2011
  -->return an xml document (of xmltype)
  */
  FUNCTION GetAllowUnregistCompInMix RETURN VARCHAR2 IS
    LValue CLOB;
    LSettings XMLTYPE;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetAllowUnregistCompInMix_started', $$plsql_line,'start' );
    IF (VAllowUnregistCompInMix is NULL) THEN
      LSettings := VSystemSettings;
      SELECT Extract(
        LSettings,
        'Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="AllowUnregisteredComponentsInMixtures"]/@value'
      ).GetStringVal() INTO LValue
      FROM DUAl;
      VAllowUnregistCompInMix := to_char(LValue);
    END IF;
    TraceWrite( act||'GetAllowUnregistCompInMix_ended', $$plsql_line,'end' );
    RETURN VAllowUnregistCompInMix;
  END;

  /**
  Utility to help build an error list that retains the innermost error stack.
  -->param AErrorMessage the text to prepend to the exception backtrace
  -->param AErrorStack   the text (backtrace) to append to the current exception message, if any exists
  -->return a CLOB containing complete error details
  */
  FUNCTION AppendError(AErrorMessage CLOB DEFAULT NULL, AErrorStack CLOB DEFAULT NULL)
  RETURN CLOB AS
    LNewErr CLOB := empty_clob();
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'AppendError_started', $$plsql_line,'start' );
    --tack on message to exception stacktrace
    IF AErrorMessage IS NOT NULL THEN
      LNewErr := AErrorMessage || chr(10) || AErrorStack;
    ELSE
      LNewErr := AErrorStack;
    END IF;

    --tack this derived onto the current exception error messge
    IF SQLCODE <> 0 THEN
      LNewErr := SQLERRM || chr(10) || LNewErr;
    END IF;
    TraceWrite( act||'AppendError_ended', $$plsql_line,'end' );
    RETURN LNewErr;
  END;

  /**
  Utility for generating a universal xml wrapper for actions taken on one or more registry records.
  -->author jed
  -->since December 2009
  -->param AMessage   the text to be used as the 'message' attribute of the Response element
  -->param AError     describes why an action was not carried out
  -->param AResult    an xml representation of one or more objects resulting from an action
  -->return           a CLOB in the format '<Response message=""><Error></Error><Result></Result></Response>'
  */
  FUNCTION CreateRegistrationResponse(
    AMessage VARCHAR2
    , AError CLOB
    , AResult CLOB
    , ARegId Varchar2 := '0'
    , ABatchNumber Varchar2 := '0'
  ) RETURN CLOB
  AS
 LResponse CLOB := empty_clob();
    LCleanError CLOB := CASE WHEN AError IS NULL THEN NULL ELSE REGEXP_REPLACE(AError, cXmlDecoration, '') END;
    LCleanResult CLOB := CASE WHEN AResult IS NULL THEN NULL ELSE REGEXP_REPLACE(AResult, cXmlDecoration, '') END;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'CreateRegistrationResponse_started', $$plsql_line,'start' );
    LResponse :=
    '<Response message="' || AMessage ||  '">'
      || '<Error>' || LCleanError || '</Error>'
      || '<Result>' || LCleanResult || '</Result>'
      || '<CustomFields>'
      || '<RegId>' || ARegId || '</RegId>'
      || '<BatchNumber>' || ABatchNumber || '</BatchNumber>'
      || '</CustomFields>'
      || '</Response>';
     TraceWrite( act||'CreateRegistrationResponse_ended', $$plsql_line,'end' );
    RETURN LResponse;
  END;

  /**
  Dissembles the message, error and result properties from create/update output.
  Create and Update events return an xml (CLOB) value from "CreateRegistrationResponse" to provide
  the caller with details. This will parse the result for internal callers that require details.
  -->author jed
  -->since December 2009
  -->param AResponseXml 'Response' input as an xml string
  -->param AMessage     extracted message output
  -->param AError       extracted error output
  -->param AResult      extracted object(s) output
  */
  PROCEDURE ExtractRegistrationResponse(
    AResponseXml IN CLOB
    , AMessage OUT CLOB
    , AError OUT CLOB
    , AResult OUT CLOB
  ) IS
    LResponse CLOB := AResponseXml;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'ExtractRegistrationResponse_started', $$plsql_line,'start' );
    --use string extraction mechanisms to get these values
    AError := TakeOffAndGetClob(LResponse, 'Error');
    AResult := TakeOffAndGetClob(LResponse, 'Result');

    if AResponseXml is null then
        TraceWrite('ExtractRegistrationResponse', $$plsql_line, 'Return because AResponseXml is null');
        TraceWrite( act||'ExtractRegistrationResponse_ended', $$plsql_line,'end' );
        return;
    end if;

    --using xmltype here is 64k-safe because LResponse is so small now
    SELECT EXTRACT(XMLTYPE(LResponse), '/Response/@message').GetClobVal()
    INTO AMessage FROM dual;
    TraceWrite( act||'ExtractRegistrationResponse_ended', $$plsql_line,'end' );
  END;

  PROCEDURE InsertLog(ALogProcedure CLOB,ALogComment CLOB) IS
      PRAGMA AUTONOMOUS_TRANSACTION;
  BEGIN
    $if CompoundRegistry.TraceEnabled $then
      INSERT INTO LOG
        (LogProcedure,LogComment,run_seq)
      VALUES
        (to_char(systimestamp, 'ss.FF') || ' - ' || $$plsql_unit||'.'||ALogProcedure,ALogComment,log_run_seq);
      COMMIT;
    $end null;
  EXCEPTION
      WHEN OTHERS THEN NULL; --If logs don't work then don't stop
  END;

  /**
  Conditionally generates a log entry with a line number reported by the caller,
  based on the TraceEnabled package constant. (There is significant performance
  cost, so be sure this is only set to True while performing workflow tracing.)
  This is a wrapper method for the original InsertLog overload.
  -->author jed
  -->since January 2011
  -->param AMethodName the name of the calling method
  -->param APlSqlLine the line number of the calling code
  -->param ALogComment the log text/content
  */
  PROCEDURE TraceWrite(AMethodName Varchar2, APlSqlLine Integer, ALogComment CLOB)
  IS
    PRAGMA AUTONOMOUS_TRANSACTION;
  BEGIN
    $if CompoundRegistry.TraceEnabled $then
      InsertLog(
        AMethodName || ' line ' || to_char(APlSqlLine)
        , ALogComment
      );
    $end NULL;
  END;

  /**
  Sets NLS parameters for data format and numeric characters on the current session.
  -->author M. Fariello
  */
  PROCEDURE SetSessionParameter IS
      PRAGMA AUTONOMOUS_TRANSACTION;
  BEGIN
      DBMS_SESSION.set_nls('NLS_DATE_FORMAT','''YYYY-MM-DD HH12:Mi:SS AM''');
      DBMS_SESSION.set_nls('NLS_NUMERIC_CHARACTERS', '''.,''');
      EXECUTE IMMEDIATE 'ALTER SESSION SET CURSOR_SHARING  = FORCE';
      COMMIT; --It is necesary to finished the Autonomous-Transaction
  EXCEPTION
      WHEN OTHERS THEN
      BEGIN
          RAISE_APPLICATION_ERROR(eSetSessionParameter, AppendError('SetSessionParameter', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
      END;
  END;

  /**
  Procedure to be invoked after a registration has been performed but before the
  resulting XML is returned to the caller.
  -->author jed
  -->since December 2010
  -->param ARegID Registration ID (private key) assigned to the registration
  */
  PROCEDURE OnRegistrationInsert(ARegID NUMBER) IS
    LProc varchar2(1000) := upper('AfterRegInsert');
    LProcex varchar2(1100) := 'begin ' || LProc || '(:regid); end;';
    LExists number := 0;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'OnRegistrationInsert_started', $$plsql_line,'start' );
    --ensure the event-handling procedure exists before attempting to invoke it
    SELECT COUNT(*) INTO LExists
    FROM user_objects obj
    WHERE obj.object_type IN ('PROCEDURE')
      AND obj.object_name = LProc;
    BEGIN
      IF (LExists = 1) THEN
        --Uncomment the line below for tracing
        -- InsertLog('OnRegistrationInsert', LProcex || ' using ' || to_char(ARegID));
        EXECUTE IMMEDIATE LProcex USING IN ARegID;
      END IF;
    END;
    TraceWrite( act||'OnRegistrationInsert_ended', $$plsql_line,'end' );
  END;

  /**
  Procedure to be invoked after a registration has been performed but before the
  resulting XML is returned to the caller. Discovers the Reg ID and then calls
  the other overload of the same function.
  -->author jed
  -->since December 2010
  -->param ARegNum 'Registry Number' (public key) assigned to the registration
  */
  PROCEDURE OnRegistrationInsert(ARegNum VARCHAR2) IS
    LRegID number;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'OnRegistrationInsert2_started', $$plsql_line,'start' );
    SELECT rn.reg_id INTO LRegId
    FROM reg_numbers rn
    WHERE rn.reg_number = ARegNum;

    OnRegistrationInsert(LRegId);
    TraceWrite( act||'OnRegistrationInsert2_ended', $$plsql_line,'end' );
  END;

  /**
  Procedure to be invoked after a registration has been performed but before the
  resulting XML is returned to the caller.
  -->author jed
  -->since December 2010
  -->param ARegID Registration ID (private key) assigned to the registration
  */
  PROCEDURE OnRegistrationUpdate(ARegID NUMBER) IS
    LProc varchar2(1000) := upper('AfterRegUpdate');
    LProcex varchar2(1100) := 'begin ' || LProc || '(:regid); end;';
    LExists number := 0;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'OnRegistrationUpdate_started', $$plsql_line,'start' );
    --ensure the event-handling procedure exists before attempting to invoke it
    SELECT COUNT(*) INTO LExists
    FROM all_objects obj
    WHERE obj.object_type IN ('PROCEDURE')
      AND obj.object_name = LProc;
    BEGIN
      IF (LExists = 1) THEN
        --Uncomment the line below for tracing
        -- InsertLog('OnRegistrationUpdate', LProcex || ' using ' || to_char(ARegID));
        EXECUTE IMMEDIATE LProcex USING IN ARegID;
      END IF;
    END;
    TraceWrite( act||'OnRegistrationUpdate_ended', $$plsql_line,'end' );
  END;

  /**
  Procedure to be invoked after a registration has been updated but before the
  resulting XML is returned to the caller. Discovers the Reg ID and then calls
  the other overload of the same function.
  -->author jed
  -->since December 2010
  -->param ARegNum 'Registry Number' (public key) assigned to the registration
  */
  PROCEDURE OnRegistrationUpdate(ARegNum VARCHAR2) IS
    LRegID number;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'OnRegistrationUpdate2_started', $$plsql_line,'start' );
    SELECT rn.reg_id INTO LRegId
    FROM reg_numbers rn
    WHERE rn.reg_number = ARegNum;

    OnRegistrationUpdate(LRegId);
    TraceWrite( act||'OnRegistrationUpdate2_ended', $$plsql_line,'end' );
  END;

  /**
  Go over the Field's tag of the properties and add the attributes of picklist
  -->author Fari
  -->since December 2010
  -->param AFields   list of field name with picklist
  -->param AXml      xml with the property and property's value
  -->param ABeginXml tag parent to begin the search
  */
  PROCEDURE AddAttribPickList(
    AFields IN CLOB
    , AXml IN OUT NOCOPY CLOB
    , ABeginXml IN CLOB
  ) IS
    LPosBegin                 NUMBER;
    LPosDot                   NUMBER;
    LPoslast                  NUMBER;
    LField                    VARCHAR2(100);
    LFieldTag                 VARCHAR2(100);
    LFieldTagEnd              VARCHAR2(100);
    LpickListDomainID               VARCHAR2(100);
    LFields                   CLOB;

    LPosField                 NUMBER;
    LPosFieldEnd              NUMBER;

    LExt_Table                VW_PickListDomain.Ext_Table%Type;
    LExt_ID_Col               VW_PickListDomain.Ext_ID_Col%Type;
    LExt_Display_Col          VW_PickListDomain.Ext_Display_Col%Type;
    LPickListDisplayValue     CLOB;
    LFieldValue               VARCHAR2(100);
    LSelect                   VARCHAR2(4000);
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'AddAttribPickList_started', $$plsql_line,'start' );
    LFields := AFields || ',';
    LPosBegin := 0;
    LPoslast := 1;

    TraceWrite('AddAttribPickList_OriginalXml', $$plsql_line, AXml);

    LOOP
      LPosBegin := INSTR(LFields,',', LPoslast);
      LPosDot := INSTR(LFields,':', LPoslast);
      LField := TRIM( UPPER(SUBSTR(LFields, LPoslast, LPosDot-LPoslast)) );
    EXIT WHEN LField IS NULL;

      LpickListDomainID := TRIM( SUBSTR(LFields, LPosDot+1, LPosBegin-LPosDot-1) );
      LPoslast := LPosBegin + 1;

      LFieldTag := '<'||LField||'>';
      LFieldTagEnd := '</'||LField||'>';
      LPosField := INSTR( AXml, LFieldTag, INSTR(AXml, ABeginXml)+1 );

      TraceWrite('AddAttribPickList_Field', $$plsql_line, LField);
      TraceWrite('AddAttribPickList_DomainID', $$plsql_line, LPickListDomainID);
      TraceWrite('AddAttribPickList_Position', $$plsql_line, to_char(LPosField));

      IF LPosField <> 0 THEN
        IF NVL(LpickListDomainID,0)<>0 THEN
          SELECT Ext_Table,Ext_ID_Col,Ext_Display_Col
          INTO LExt_Table,LExt_ID_Col,LExt_Display_Col
          FROM VW_PickListDomain PLM
          WHERE PLM.ID = LPickListDomainID;

          LPosFieldEnd := INSTR(AXml,LFieldTagEnd,INSTR(AXml,ABeginXml)+1);
          LFieldValue := SUBSTR(AXml,LPosField+Length(LFieldTag),LPosFieldEnd-(LPosField+Length(LFieldTag))) ;

          IF LFieldValue IS NOT NULL AND LExt_Display_Col IS NOT NULL AND LExt_Table IS NOT NULL AND LExt_ID_Col IS NOT NULL THEN
             LSelect :=
               'SELECT ' || LExt_Display_Col || ' FROM ' || LExt_Table  ||' WHERE ' || LExt_ID_Col || ' = :LFieldValue';

             TraceWrite('AddAttribPickList_Select', $$plsql_line, LSelect);
             BEGIN
                EXECUTE IMMEDIATE LSelect INTO LPickListDisplayValue
                USING LFieldValue;
             EXCEPTION
                WHEN OTHERS THEN
                BEGIN
                  TraceWrite('AddAttribPickList_Error', $$plsql_line, DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
                END;
             END;

             IF LPickListDisplayValue IS NOT NULL THEN
               AXml := REPLACE(AXml,LFieldTag,'<'||LField||' pickListDomainID="'||LPickListDomainID||'" pickListDisplayValue="'||LPickListDisplayValue||'">');
             END IF;

          END IF;
        END IF;
      END IF;
    END LOOP;

    TraceWrite('AddAttribPickList_ModifiedXml', $$plsql_line, AXml);
     TraceWrite( act||'AddAttribPickList_ended', $$plsql_line,'end' );

  END;

  FUNCTION GetIsBatchEditable(ARegNumber in VW_RegistryNumber.RegNumber%type, pBatchNumber in VW_Batch.BatchNumber%type) RETURN VARCHAR2 IS
    LIsBatchEditable VARCHAR2(5):='False';
--    LEnableRLS BOOLEAN;
    LLevelRLS VARCHAR2(50);
    LUserProjectID INTEGER;
    LProjectID VW_Project.ProjectID%type;
    TYPE T_RegProjectAssigned IS REF CURSOR;
    C_RegProjectAssigned  T_RegProjectAssigned;

    CURSOR C_UserProjects IS
      SELECT ProjectID
      FROM  VW_Project
      ORDER BY ProjectID;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetIsBatchEditable_started', $$plsql_line,'start' );

    LLevelRLS := vActiveRLS; --RegistrationRLS.GetLevelRLS;

    IF upper(LLevelRLS)!='OFF' THEN
      TraceWrite('GetIsBatchEditable', $$plsql_line, 'LLevelRLS->'||LLevelRLS);

--      LEnableRLS := RegistrationRLS.GEnableRLS;
      IF LEnableRLS THEN
        RegistrationRLS.GEnableRLS := False;
      END IF;

      IF LLevelRLS='Registry Level Projects' THEN
          OPEN C_RegProjectAssigned FOR
            SELECT Distinct RNP.ProjectID
            FROM VW_RegistryNumber RN, VW_RegistryNumber_Project RNP
            WHERE RN.RegID = RNP.RegID AND RN.RegNumber=ARegNumber
            ORDER BY RNP.ProjectID;
      ELSE
          OPEN C_RegProjectAssigned FOR
            SELECT Distinct BP.ProjectID
            FROM VW_Batch B,VW_Batch_Project BP
            WHERE BP.BatchID=B.BatchID AND B.BatchNumber=pBatchNumber
            ORDER BY BP.ProjectID;
      END IF;

      RegistrationRLS.GEnableRLS := True;

      OPEN C_UserProjects;
      LOOP
        FETCH C_RegProjectAssigned INTO LProjectID;
        TraceWrite('GetIsBatchEditable', $$plsql_line,'LProjectID->'||LProjectID);
        IF C_RegProjectAssigned%NOTFOUND THEN
          InsertLog('GetIsBatchEditable','C_RegProjectAssigned%NOTFOUND');
          LIsBatchEditable:='True';
          EXIT;
        END IF;

        LOOP
          FETCH C_UserProjects INTO LUserProjectID;
          IF C_UserProjects%NOTFOUND THEN
            TraceWrite('GetIsBatchEditable', $$plsql_line, 'C_UserProjects%NOTFOUND ');
            EXIT;
          END IF;

          TraceWrite('GetIsBatchEditable', $$plsql_line,'LUserProjectID:'||LUserProjectID);
        EXIT WHEN LUserProjectID=LProjectID;
        END LOOP;

      EXIT WHEN  C_UserProjects%NOTFOUND;
      END LOOP;
      CLOSE C_UserProjects;
      CLOSE C_RegProjectAssigned;

      RegistrationRLS.GEnableRLS:=LEnableRLS;
    ELSE
      LIsBatchEditable := 'True';
    END IF;

    InsertLog('GetIsBatchEditable','LIsBatchEditable->'||LIsBatchEditable);
    TraceWrite( act||'GetIsBatchEditable_ended', $$plsql_line,'end' );
    RETURN LIsBatchEditable;

  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        IF LEnableRLS IS NOT NULL THEN
            RegistrationRLS.GEnableRLS:=LEnableRLS;
        END IF;

        RAISE_APPLICATION_ERROR(eGenericException, AppendError('GetIsBatchEditable', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
  END;


  /**
  Define and get the status "Editable". of un registry
  -->author Fari
  -->since November 2010
  */
  FUNCTION GetIsEditable(ARegNumber in VW_RegistryNumber.RegNumber%type) RETURN VARCHAR2 IS
    LIsEditable VARCHAR2(5):='False';
--    LEnableRLS BOOLEAN;
    LLevelRLS VARCHAR2(50);
    LUserProjectID INTEGER;
    LProjectID VW_Project.ProjectID%type;
    TYPE T_RegProjectAssigned IS REF CURSOR;
    C_RegProjectAssigned  T_RegProjectAssigned;

    CURSOR C_UserProjects IS
      SELECT ProjectID
      FROM  VW_Project
      ORDER BY ProjectID;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetIsEditable_started', $$plsql_line,'start' );

    LLevelRLS := vActiveRLS; --RegistrationRLS.GetLevelRLS;

    IF upper(LLevelRLS)!='OFF' THEN
      TraceWrite('GetIsEditable', $$plsql_line, 'LLevelRLS->'||LLevelRLS);

--      LEnableRLS := RegistrationRLS.GEnableRLS;
      IF LEnableRLS THEN
        RegistrationRLS.GEnableRLS := False;
      END IF;

      IF LLevelRLS='Registry Level Projects' THEN
        OPEN C_RegProjectAssigned FOR
        SELECT Distinct RNP_StructureUsed.ProjectID
        FROM  VW_RegistryNumber RN, VW_RegistryNumber_Project RNP,VW_Mixture_RegNumber MR,VW_Mixture_Component MC,VW_Compound C,
            VW_Compound C_StructureUsed,VW_Mixture_Component MC_StructureUsed,VW_Mixture_RegNumber MR_StructureUsed, VW_RegistryNumber_Project RNP_StructureUsed
        WHERE RN.RegID = RNP.RegID AND RN.RegNumber=ARegNumber AND C.CompoundID=MC.CompoundID AND MR.RegID=RN.RegID AND MC.MixtureID=MR.MixtureID AND
            C_StructureUsed.StructureID=C.StructureID AND MC_StructureUsed.CompoundID=C_StructureUsed.CompoundID AND MR_StructureUsed.MixtureID = MC_StructureUsed.MixtureID AND MR_StructureUsed.RegID = RNP_StructureUsed.RegID AND MR_StructureUsed.RegID = RN.RegID
        ORDER BY RNP_StructureUsed.ProjectID;
      ELSE
        OPEN C_RegProjectAssigned FOR
        SELECT Distinct BP_StructureUsed.ProjectID
        FROM VW_RegistryNumber RN,VW_Batch B,VW_Batch_Project BP,VW_Mixture_RegNumber MR,VW_Mixture_Component MC,VW_Compound C,
            VW_Compound C_StructureUsed,VW_Mixture_Component MC_StructureUsed,VW_Mixture_RegNumber MR_StructureUsed,VW_Batch B_StructureUsed,VW_Batch_Project BP_StructureUsed
        WHERE RN.RegID = B.RegID AND BP.BatchID=B.BatchID AND RN.RegNumber=ARegNumber AND MR.RegID=RN.RegID AND MC.MixtureID=MR.MixtureID AND C.CompoundID=MC.CompoundID AND
            C_StructureUsed.StructureID=C.StructureID AND MC_StructureUsed.CompoundID=C_StructureUsed.CompoundID AND MR_StructureUsed.MixtureID = MC_StructureUsed.MixtureID AND MR_StructureUsed.RegID = B_StructureUsed.RegID AND BP_StructureUsed.BatchID=B_StructureUsed.BatchID AND MR_StructureUsed.RegID = RN.RegID
        ORDER BY BP_StructureUsed.ProjectID;
      END IF;

      RegistrationRLS.GEnableRLS := True;

      OPEN C_UserProjects;
      LOOP
        FETCH C_RegProjectAssigned INTO LProjectID;
        TraceWrite('GetIsEditable', $$plsql_line,'LProjectID->'||LProjectID);
        IF C_RegProjectAssigned%NOTFOUND THEN
          InsertLog('GetIsEditable','C_RegProjectAssigned%NOTFOUND');
          LIsEditable:='True';
          EXIT;
        END IF;

        LOOP
          FETCH C_UserProjects INTO LUserProjectID;
          IF C_UserProjects%NOTFOUND THEN
            TraceWrite('GetIsEditable', $$plsql_line, 'C_UserProjects%NOTFOUND ');
            EXIT;
          END IF;

          TraceWrite('GetIsEditable', $$plsql_line,'LUserProjectID:'||LUserProjectID);
        EXIT WHEN LUserProjectID=LProjectID;
        END LOOP;

      EXIT WHEN  C_UserProjects%NOTFOUND;
      END LOOP;
      CLOSE C_UserProjects;
      CLOSE C_RegProjectAssigned;

      RegistrationRLS.GEnableRLS:=LEnableRLS;
    ELSE
      LIsEditable := 'True';
    END IF;

    InsertLog('GetIsEditable','LIsEditable->'||LIsEditable);
    TraceWrite( act||'GetIsEditable_ended', $$plsql_line,'end' );
    RETURN LIsEditable;

  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        IF LEnableRLS IS NOT NULL THEN
            RegistrationRLS.GEnableRLS:=LEnableRLS;
        END IF;

        RAISE_APPLICATION_ERROR(eGenericException, AppendError('GetIsEditable', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
  END;

  /**
  Define and get the status "Editable". of un registry in the temporary context
  -->author Fari
  -->since November 2010
  -->param ATempID Identifier for a temporary Registration
  */
  FUNCTION GetIsEditableTmp(ATempID  in Number) RETURN VARCHAR2 IS
    LIsEditable VARCHAR2(5):='False';
--    LEnableRLS BOOLEAN;
    LLevelRLS VARCHAR2(50);
    LUserProjectID INTEGER;

    LProjectID VW_Project.ProjectID%type;

    TYPE T_RegProjectAssigned IS REF CURSOR;
    C_RegProjectAssigned   T_RegProjectAssigned;

    CURSOR C_UserProjects IS
        SELECT ProjectID
            FROM  VW_Project
            ORDER BY ProjectID;

    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetIsEditableTmp_started', $$plsql_line,'start' );

    LLevelRLS:=vActiveRLS; --RegistrationRLS.GetLevelRLS;

    IF upper(LLevelRLS)!='OFF' THEN
        InsertLog('GetIsEditable','LLevelRLS->'||LLevelRLS);

--        LEnableRLS:=RegistrationRLS.GEnableRLS;

        IF LEnableRLS THEN
            RegistrationRLS.GEnableRLS:=False;
        END IF;

        IF LLevelRLS='Registry Level Projects' THEN
            OPEN C_RegProjectAssigned FOR
                SELECT Distinct TRP.ProjectID
                    FROM  VW_TemporaryRegnumbersProject TRP
                    WHERE TRP.TempBatchID=ATempID
                    ORDER BY TRP.ProjectID;

        ELSE
            OPEN C_RegProjectAssigned FOR
                SELECT Distinct TBP.ProjectID
                    FROM  VW_TemporaryBatchProject TBP
                    WHERE TBP.TempBatchID=ATempID
                    ORDER BY TBP.ProjectID;
        END IF;

        RegistrationRLS.GEnableRLS:=True;

        OPEN C_UserProjects;

        LOOP

            FETCH C_RegProjectAssigned INTO LProjectID;
            InsertLog('GetIsEditableTmp','LProjectID->'||LProjectID);

            IF C_RegProjectAssigned%NOTFOUND THEN
                InsertLog('GetIsEditableTmp','C_RegProjectAssigned%NOTFOUND');
                LIsEditable:='True';
                EXIT;
            END IF;

            LOOP

                FETCH C_UserProjects INTO LUserProjectID;

                IF C_UserProjects%NOTFOUND THEN
                   InsertLog('GetIsEditableTmp','C_UserProjects%NOTFOUND ');
                   EXIT;
                END IF;


                InsertLog('GetIsEditableTmp','LUserProjectID:'||LUserProjectID);

                EXIT WHEN LUserProjectID=LProjectID;


            END LOOP;

            EXIT WHEN  C_UserProjects%NOTFOUND;

        END LOOP;


        CLOSE C_UserProjects;

        CLOSE C_RegProjectAssigned;

        RegistrationRLS.GEnableRLS:=LEnableRLS;
    ELSE
        LIsEditable:='True';
    END IF;

    InsertLog('GetIsEditableTmp','LIsEditable->'||LIsEditable);
    TraceWrite( act||'GetIsEditableTmp_ended', $$plsql_line,'end' );
    RETURN LIsEditable;

  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        IF LEnableRLS IS NOT NULL THEN
            RegistrationRLS.GEnableRLS:=LEnableRLS;
        END IF;

        RAISE_APPLICATION_ERROR(eGenericException, AppendError('GetIsEditableTmp', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
  END;

  /* Editability of all batches determines the deleteability of the registry record when RLS-Batch turned on */
  FUNCTION GetIsRegistryDeleteable(ARegNumber in VW_RegistryNumber.RegNumber%type) RETURN VARCHAR2 IS
    LIsRegsitryDeleteable VARCHAR2(5):='True';
    LIsBatchEditable VARCHAR2(5):='True';
--    LEnableRLS BOOLEAN;
    LLevelRLS VARCHAR2(50);
    LBatchNumber INTEGER;
    TYPE T_RegBatches IS REF CURSOR;
    C_RegBatches  T_RegBatches;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetIsRegistryDeleteable_started', $$plsql_line,'start' );

    LLevelRLS := vActiveRLS; --RegistrationRLS.GetLevelRLS;
--    LEnableRLS := RegistrationRLS.GEnableRLS;

    IF upper(LLevelRLS)!='OFF' THEN
      TraceWrite('GetIsRegistryDeleteable', $$plsql_line, 'LLevelRLS->'||LLevelRLS);
      IF LLevelRLS='Batch Level Projects' THEN

        IF LEnableRLS THEN
            RegistrationRLS.GEnableRLS := False;
        END IF;

        OPEN C_RegBatches FOR
        SELECT B.BatchNumber
        FROM VW_RegistryNumber RN, VW_Batch B
        WHERE RN.RegNumber=ARegNumber AND RN.RegID = B.RegID;

          RegistrationRLS.GEnableRLS:=LEnableRLS;

        LOOP
          FETCH C_RegBatches INTO LBatchNumber;
          IF C_RegBatches%NOTFOUND THEN
            TraceWrite('GetIsRegistryDeleteable', $$plsql_line, 'C_RegBatches%NOTFOUND ');
            EXIT;
          END IF;
          TraceWrite('GetIsRegistryDeleteable', $$plsql_line,'LBatchNumber:'||LBatchNumber);
          LIsBatchEditable := GetIsBatchEditable(ARegNumber, LBatchNumber);
          IF UPPER(LIsBatchEditable) = 'FALSE' THEN -- if any of the batches are not editable, then the registry record is not deleteable
              EXIT;
          END IF;
          TraceWrite('GetIsRegistryDeleteable', $$plsql_line,'LIsBatchEditable:'||LIsBatchEditable);
        END LOOP;
          LIsRegsitryDeleteable := LIsBatchEditable;
      END IF;
    ELSE
      LIsRegsitryDeleteable := 'True';
    END IF;
    InsertLog('GetIsRegistryDeleteable','LIsRegsitryDeleteable->'||LIsRegsitryDeleteable);
    TraceWrite( act||'GetIsRegistryDeleteable_ended', $$plsql_line,'end' );
    RETURN LIsRegsitryDeleteable;

  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        IF LEnableRLS IS NOT NULL THEN
            RegistrationRLS.GEnableRLS:=LEnableRLS;
        END IF;
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('GetIsRegistryDeleteable', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
  END;

  FUNCTION GetCanPropogateComponentEdits(PCompoundID in VW_COMPOUND.COMPOUNDID%type) RETURN VARCHAR2 IS
    LCanPropogateComponentEdits VARCHAR2(5):='False';
--    LEnableRLS BOOLEAN;
    LLevelRLS VARCHAR2(50);
    LUserProjectID INTEGER;
    LProjectID VW_Project.ProjectID%type;
    TYPE T_RegProjectAssigned IS REF CURSOR;
    C_RegProjectAssigned  T_RegProjectAssigned;

    CURSOR C_UserProjects IS
      SELECT ProjectID
      FROM  VW_Project
      ORDER BY ProjectID;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetCanPropogateComponentEdits_started', $$plsql_line,'start' );

    LLevelRLS := vActiveRLS;

    IF upper(LLevelRLS)!='OFF' THEN
      TraceWrite('GetCanPropogateComponentEdits', $$plsql_line, 'LLevelRLS->'||LLevelRLS);

--      LEnableRLS := RegistrationRLS.GEnableRLS;
      IF LEnableRLS THEN
        RegistrationRLS.GEnableRLS := False;
      END IF;

      IF LLevelRLS='Registry Level Projects' THEN
        OPEN C_RegProjectAssigned FOR
        SELECT Distinct RNP_CompoundUsed.ProjectID
        FROM  VW_RegistryNumber RN, VW_RegistryNumber_Project RNP,VW_Mixture M,VW_Mixture_RegNumber MR,VW_Mixture_Component MC,VW_Compound C,
            VW_Mixture_Component MC_CompoundUsed,VW_Mixture_RegNumber MR_CompoundUsed,VW_RegistryNumber_Project RNP_CompoundUsed,
            VW_Compound C_StructureUsed,VW_Mixture_Component MC_StructureUsed,VW_Mixture_RegNumber MR_StructureUsed, VW_RegistryNumber_Project RNP_StructureUsed
        WHERE RN.RegID = RNP.RegID AND RN.RegID=M.RegID AND M.MixtureID=MC.MixtureID AND C.CompoundID=MC.CompoundID AND C.CompoundID=PCompoundID AND
            MR.RegID=RN.RegID AND MC.MixtureID=MR.MixtureID AND MC_CompoundUsed.CompoundID=MC.CompoundID AND MR_CompoundUsed.MixtureID=MC_CompoundUsed.MixtureID AND MR_CompoundUsed.RegID = RNP_CompoundUsed.RegID AND MR_CompoundUsed.RegID = RN.RegID AND
            C_StructureUsed.StructureID=C.StructureID AND MC_StructureUsed.CompoundID=C_StructureUsed.CompoundID AND MR_StructureUsed.MixtureID = MC_StructureUsed.MixtureID AND MR_StructureUsed.RegID = RNP_StructureUsed.RegID AND MR_StructureUsed.RegID = RN.RegID
        ORDER BY RNP_CompoundUsed.ProjectID;
      ELSE
        OPEN C_RegProjectAssigned FOR
        SELECT Distinct BP_CompoundUsed.ProjectID
        FROM VW_RegistryNumber RN,VW_Batch B,VW_Batch_Project BP,VW_Mixture M,VW_Mixture_RegNumber MR,VW_Mixture_Component MC,VW_Compound C,
            VW_Mixture_Component MC_CompoundUsed,VW_Mixture_RegNumber MR_CompoundUsed,VW_Batch B_CompoundUsed,VW_Batch_Project BP_CompoundUsed,
            VW_Compound C_StructureUsed,VW_Mixture_Component MC_StructureUsed,VW_Mixture_RegNumber MR_StructureUsed,VW_Batch B_StructureUsed,VW_Batch_Project BP_StructureUsed
        WHERE RN.RegID = B.RegID AND BP.BatchID=B.BatchID AND RN.RegID=M.RegID AND M.MixtureID=MC.MixtureID AND C.CompoundID=MC.CompoundID AND C.CompoundID=PCompoundID AND
            MR.RegID=RN.RegID AND MC.MixtureID=MR.MixtureID AND MC_CompoundUsed.CompoundID=MC.CompoundID AND MR_CompoundUsed.MixtureID=MC_CompoundUsed.MixtureID AND MR_CompoundUsed.RegID = B_CompoundUsed.RegID AND BP_CompoundUsed.BatchID=B_CompoundUsed.BatchID AND MR_CompoundUsed.RegID = RN.RegID AND
            C_StructureUsed.StructureID=C.StructureID AND MC_StructureUsed.CompoundID=C_StructureUsed.CompoundID AND MR_StructureUsed.MixtureID = MC_StructureUsed.MixtureID AND MR_StructureUsed.RegID = B_StructureUsed.RegID AND BP_StructureUsed.BatchID=B_StructureUsed.BatchID AND MR_StructureUsed.RegID = RN.RegID
        ORDER BY BP_CompoundUsed.ProjectID;
      END IF;

      RegistrationRLS.GEnableRLS := True;
      OPEN C_UserProjects;
      LOOP
        FETCH C_RegProjectAssigned INTO LProjectID;
        TraceWrite('GetCanPropogateComponentEdits', $$plsql_line,'LProjectID->'||LProjectID);
        IF C_RegProjectAssigned%NOTFOUND THEN
          InsertLog('GetCanPropogateComponentEdits','C_RegProjectAssigned%NOTFOUND');
          LCanPropogateComponentEdits:='True';
          EXIT;
        END IF;

        LOOP
          FETCH C_UserProjects INTO LUserProjectID;
          IF C_UserProjects%NOTFOUND THEN
            TraceWrite('GetCanPropogateComponentEdits', $$plsql_line, 'C_UserProjects%NOTFOUND ');
            EXIT;
          END IF;

          TraceWrite('GetCanPropogateComponentEdits', $$plsql_line,'LUserProjectID:'||LUserProjectID);
        EXIT WHEN LUserProjectID=LProjectID;
        END LOOP;

      EXIT WHEN  C_UserProjects%NOTFOUND;
      END LOOP;
      CLOSE C_UserProjects;
      CLOSE C_RegProjectAssigned;

      RegistrationRLS.GEnableRLS:=LEnableRLS;
    ELSE
      LCanPropogateComponentEdits := 'True';
    END IF;

    InsertLog('GetCanPropogateComponentEdits','LCanPropogateComponentEdits->'||LCanPropogateComponentEdits);
    TraceWrite( act||'GetCanPropogateComponentEdits_ended', $$plsql_line,'end' );
    RETURN LCanPropogateComponentEdits;

  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        IF LEnableRLS IS NOT NULL THEN
            RegistrationRLS.GEnableRLS:=LEnableRLS;
        END IF;

        RAISE_APPLICATION_ERROR(eGenericException, AppendError('GetCanPropogateComponentEdits', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
  END;

  FUNCTION GetCanPropogateStructureEdits(PStructureId in vw_structure.structureid%type) RETURN VARCHAR2 IS
    LCanPropogateStructureEdits VARCHAR2(5):='False';
--    LEnableRLS BOOLEAN;
    LLevelRLS VARCHAR2(50);
    LUserProjectID INTEGER;
    LProjectID VW_Project.ProjectID%type;
    TYPE T_RegProjectAssigned IS REF CURSOR;
    C_RegProjectAssigned  T_RegProjectAssigned;

    CURSOR C_UserProjects IS
      SELECT ProjectID
      FROM  VW_Project
      ORDER BY ProjectID;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetCanPropogateStructureEdits_started', $$plsql_line,'start' );

    LLevelRLS := vActiveRLS;

    IF upper(LLevelRLS)!='OFF' THEN
      TraceWrite('GetCanPropogateStructureEdits', $$plsql_line, 'LLevelRLS->'||LLevelRLS);

--      LEnableRLS := RegistrationRLS.GEnableRLS;
      IF LEnableRLS THEN
        RegistrationRLS.GEnableRLS := False;
      END IF;

      IF LLevelRLS='Registry Level Projects' THEN
        OPEN C_RegProjectAssigned FOR
        SELECT Distinct RNP_StructureUsed.ProjectID
        FROM  VW_RegistryNumber RN, VW_RegistryNumber_Project RNP,VW_Mixture M,VW_Mixture_RegNumber MR,VW_Mixture_Component MC,VW_Compound C,VW_STRUCTURE S,
            VW_Mixture_Component MC_CompoundUsed,VW_Mixture_RegNumber MR_CompoundUsed,VW_RegistryNumber_Project RNP_CompoundUsed,
            VW_Compound C_StructureUsed,VW_Mixture_Component MC_StructureUsed,VW_Mixture_RegNumber MR_StructureUsed, VW_RegistryNumber_Project RNP_StructureUsed
        WHERE RN.RegID = RNP.RegID AND RN.RegID=M.RegID AND M.MixtureID=MC.MixtureID AND C.CompoundID=MC.CompoundID AND C.STRUCTUREID=S.STRUCTUREID AND S.STRUCTUREID=PStructureId AND
            MR.RegID=RN.RegID AND MC.MixtureID=MR.MixtureID AND MC_CompoundUsed.CompoundID=MC.CompoundID AND MR_CompoundUsed.MixtureID=MC_CompoundUsed.MixtureID AND MR_CompoundUsed.RegID = RNP_CompoundUsed.RegID AND MR_CompoundUsed.RegID = RN.RegID AND
            C_StructureUsed.StructureID=C.StructureID AND MC_StructureUsed.CompoundID=C_StructureUsed.CompoundID AND MR_StructureUsed.MixtureID = MC_StructureUsed.MixtureID AND MR_StructureUsed.RegID = RNP_StructureUsed.RegID AND MR_StructureUsed.RegID = RN.RegID
        ORDER BY RNP_StructureUsed.ProjectID;
      ELSE
        OPEN C_RegProjectAssigned FOR
        SELECT Distinct BP_StructureUsed.ProjectID
        FROM VW_RegistryNumber RN,VW_Batch B,VW_Batch_Project BP,VW_Mixture M,VW_Mixture_RegNumber MR,VW_Mixture_Component MC,VW_Compound C,VW_STRUCTURE S,
            VW_Mixture_Component MC_CompoundUsed,VW_Mixture_RegNumber MR_CompoundUsed,VW_Batch B_CompoundUsed,VW_Batch_Project BP_CompoundUsed,
            VW_Compound C_StructureUsed,VW_Mixture_Component MC_StructureUsed,VW_Mixture_RegNumber MR_StructureUsed,VW_Batch B_StructureUsed,VW_Batch_Project BP_StructureUsed
        WHERE RN.RegID = B.RegID AND BP.BatchID=B.BatchID AND RN.RegID=M.RegID AND M.MixtureID=MC.MixtureID AND C.CompoundID=MC.CompoundID AND C.STRUCTUREID=S.STRUCTUREID AND S.STRUCTUREID=PStructureId AND
            MR.RegID=RN.RegID AND MC.MixtureID=MR.MixtureID AND MC_CompoundUsed.CompoundID=MC.CompoundID AND MR_CompoundUsed.MixtureID=MC_CompoundUsed.MixtureID AND MR_CompoundUsed.RegID = B_CompoundUsed.RegID AND BP_CompoundUsed.BatchID=B_CompoundUsed.BatchID AND MR_CompoundUsed.RegID = RN.RegID AND
            C_StructureUsed.StructureID=C.StructureID AND MC_StructureUsed.CompoundID=C_StructureUsed.CompoundID AND MR_StructureUsed.MixtureID = MC_StructureUsed.MixtureID AND MR_StructureUsed.RegID = B_StructureUsed.RegID AND BP_StructureUsed.BatchID=B_StructureUsed.BatchID AND MR_StructureUsed.RegID = RN.RegID
        ORDER BY BP_StructureUsed.ProjectID;
      END IF;

      RegistrationRLS.GEnableRLS := True;
      OPEN C_UserProjects;
      LOOP
        FETCH C_RegProjectAssigned INTO LProjectID;
        TraceWrite('GetCanPropogateStructureEdits', $$plsql_line,'LProjectID->'||LProjectID);
        IF C_RegProjectAssigned%NOTFOUND THEN
          InsertLog('GetCanPropogateStructureEdits','C_RegProjectAssigned%NOTFOUND');
          LCanPropogateStructureEdits:='True';
          EXIT;
        END IF;

        LOOP
          FETCH C_UserProjects INTO LUserProjectID;
          IF C_UserProjects%NOTFOUND THEN
            TraceWrite('GetCanPropogateStructureEdits', $$plsql_line, 'C_UserProjects%NOTFOUND ');
            EXIT;
          END IF;

          TraceWrite('GetCanPropogateStructureEdits', $$plsql_line,'LUserProjectID:'||LUserProjectID);
        EXIT WHEN LUserProjectID=LProjectID;
        END LOOP;

      EXIT WHEN  C_UserProjects%NOTFOUND;
      END LOOP;
      CLOSE C_UserProjects;
      CLOSE C_RegProjectAssigned;

      RegistrationRLS.GEnableRLS:=LEnableRLS;
    ELSE
      LCanPropogateStructureEdits := 'True';
    END IF;

    InsertLog('GetCanPropogateStructureEdits','LCanPropogateStructureEdits->'||LCanPropogateStructureEdits);
    TraceWrite( act||'GetCanPropogateStructureEdits_ended', $$plsql_line,'end' );
    RETURN LCanPropogateStructureEdits;

  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        IF LEnableRLS IS NOT NULL THEN
            RegistrationRLS.GEnableRLS:=LEnableRLS;
        END IF;

        RAISE_APPLICATION_ERROR(eGenericException, AppendError('GetCanPropogateStructureEdits', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
  END;

  /**
  Save a log with the Compound Registry Number of the Structures Duplciated by a new strcuture or by a structure updated.
  Tale Name: DUPLICATED
  Description: When a structure of a compound is saved or updated a new row is added in this table for each structure similar existing in the DB .

  Column Name          Description
  ID                   Row Single identify
  REGNUMBER            Registry Number of the Compound added or updated. It is not the Registry Number of the Registry (mixture)
  REGNUMBERDUPLICATED  It is Registry Number of the Compound existing in the DB whose structure is similary.
                       It is not the Registry Number of the Registry (mixture)
  PERSONID             Person who registered the Registry
  CREATED              Duplication Date
  -->author Fari
  -->since June 2010
  -->param AXMLRegNumberDuplicated Each Registry Number Duplciated
  -->param APersonID               Person that added or upadated the compound strcuture
  */
  PROCEDURE SaveRegNumbersDuplicated(AXMLRegNumberDuplicated XmlType,APersonID VARCHAR2)IS
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'SaveRegNumbersDuplicated_started', $$plsql_line,'start' );
    TraceWrite(
      'SaveRegNumbersDuplicated', $$plsql_line
      , 'AXMLRegNumberDuplicated->'||AXMLRegNumberDuplicated.GetStringVal()
    );

    INSERT INTO VW_Duplicates(RegNumber ,RegNumberDuplicated,PersonID,Created)
        (SELECT extract(value(RegNumberDuplicated), '//REGNUMBER/@NEW').GetStringVal(),extract(value(RegNumberDuplicated), '//REGNUMBER/text()').GetStringVal(),APersonID,SYSDATE
         FROM Table(XMLSequence(Extract(AXMLRegNumberDuplicated, '/ROWSET/REGNUMBER'))) RegNumberDuplicated);
         TraceWrite( act||'SaveRegNumbersDuplicated_ended', $$plsql_line,'end' );
  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('SaveRegNumbersDuplicated', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
  END;

  PROCEDURE AddTags(AXmlSource IN XmlType,AXmlTarget IN OUT NOCOPY XmlType,APathSource IN Varchar2,AAttributeSource IN Varchar2) IS
    LIndex               Number;
    LNodesCount          Number;
    LIndexTarget         Number;
    LNodeName            Varchar2(300);
    LNodePath            Varchar2(4000);

    LDOMDocumentSource   DBMS_XMLDom.DOMDocument;
    LSourceNode          DBMS_XMLDom.DOMNode;
    LSourceParentNode    DBMS_XMLDom.DOMNode;

    LNodeListSource      DBMS_XMLDom.DOMNodelist;

    LNodeListTarget      DBMS_XMLDom.DOMNodelist;

    LDOMDocumentTarget   DBMS_XMLDom.DOMDocument;
    LNodeTarget          DBMS_XMLDom.DOMNode;

    LAttrs               DBMS_XMLDom.DOMNamedNodeMap;
    LAttr                DBMS_XMLDom.DOMNode;
    LIndexAttr           Number;
    LXPath               Varchar2(1000);
    LAttrName            Varchar2(300);

    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'AddTags_started', $$plsql_line,'start' );
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
    DBMS_XMLDom.freeDocument(LDOMDocumentSource);
    DBMS_XMLDom.freeDocument(LDOMDocumentTarget);
    TraceWrite( act||'AddTags_ended', $$plsql_line,'end' );
  END;

  /*
  Performs the compound-to-fragment matching algorithm.
  */
  FUNCTION ValidateCompoundFragment(
    ACompoundID VW_Compound.CompoundID%Type
    , AXMLCompound XmlType
    , AXMLFragmentEquivalent XmlType
  ) RETURN CLOB IS
    LqryCtx                DBMS_XMLGEN.ctxHandle;
    LFragmentsIdsValue     VARCHAR2(32000);
    LQuery                 VARCHAR2(32000);
    LFragmentCount         INTEGER;
    LPosOld                INTEGER;
    LPos                   INTEGER;
    LSameFragment          VARCHAR2(32000);
    LSameEquivalent        VARCHAR2(32000);
    LResult                VARCHAR2(32000);

    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'ValidateCompoundFragment_started', $$plsql_line,'start' );

    TraceWrite('ValidateCompFrag_0_Compound', $$plsql_line, 'ACompoundID = ' || ACompoundID);

    -- Get a csv string of fragment IDs for this compound.
    SELECT XmlTransform(
      extract(AXMLCompound,'/Component/Compound/FragmentList/Fragment/FragmentID')
      , XmlType.CreateXml('
        <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
          <xsl:template match="/FragmentID">
            <xsl:for-each select="."><xsl:value-of select="."/>,</xsl:for-each>
          </xsl:template>
        </xsl:stylesheet>')
    ).GetClobVal()
    INTO LFragmentsIdsValue FROM dual;

    IF LFragmentsIdsValue IS NOT NULL THEN
      -- strip the trailing comma
      LFragmentsIdsValue := SUBSTR(LFragmentsIdsValue, 1, Length(LFragmentsIdsValue) - 1);

      TraceWrite('ValidateCompFrag_1_FragmentIDs', $$plsql_line, 'LFragmentsIdsValue = ' || LFragmentsIdsValue);

      LFragmentCount := 1;
      LPosOld        := 0;

      -- loop the fragment IDs list
      LOOP
        LPos := NVL(INSTR(LFragmentsIdsValue, ',', LPosOld + 1), 0);
      EXIT WHEN LPos = 0;
        LPosOld        := LPos;
        LFragmentCount := LFragmentCount + 1;
      END LOOP;

      LQuery :=
        'SELECT 1
        FROM VW_Compound_Fragment CF,VW_BatchComponentFragment BCF, VW_BatchComponent BC
        WHERE CF.CompoundID = :varCompoundID
          AND CF.ID=BCF.CompoundFragmentID AND BCF.BatchComponentID=BC.ID
          AND CF.FragmentID IN (' || LFragmentsIdsValue || ')
        GROUP BY CF.CompoundID,BC.BatchID
        HAVING COUNT(1) = :varFragmentCount
          AND :varFragmentCount = (
            SELECT Count(1)
            FROM VW_Compound_Fragment CF1,VW_BatchComponentFragment BCF1, VW_BatchComponent BC1
            WHERE CF.CompoundID=CF1.CompoundID
              AND BC.BatchID=BC1.BatchID
              AND CF1.ID=BCF1.CompoundFragmentID
              AND BCF1.BatchComponentID=BC1.ID
          )';

      TraceWrite('ValidateCompFrag_2_FragmentQuery', $$plsql_line, LQuery);

      LQryCtx := dbms_xmlgen.newContext(LQuery);

      dbms_xmlgen.setBindValue(LQryCtx, 'varCompoundID', to_char(ACompoundID));
      dbms_xmlgen.setBindValue(LQryCtx, 'varFragmentCount', to_char(LFragmentCount));
      dbms_xmlgen.setMaxRows(LqryCtx, 3);
      dbms_xmlgen.setRowTag(LqryCtx, '');
      LSameFragment := dbms_xmlgen.getXML(LqryCtx);
      dbms_xmlgen.closeContext(LqryCtx);

      TraceWrite('ValidateCompFrag_3_CompoundSameFrags', $$plsql_line, 'LSameFragment ' || LSameFragment);

      -- If another compound has the same fragments, does it also have the same equivalents?
      IF LSameFragment IS NOT NULL THEN
         LQuery := 'SELECT 1 FROM
            (SELECT FragmentID,Equivalent
                FROM VW_Compound_Fragment CF,VW_BatchComponentFragment BCF, VW_BatchComponent BC
                WHERE CF.CompoundID=''' || ACompoundID ||
              ''' AND CF.ID=BCF.CompoundFragmentID AND BCF.BatchComponentID=BC.ID AND
                      CF.FragmentID IN (' ||
              LFragmentsIdsValue || ')
                ) Fragments WHERE Equivalent<>ExtractValue(Xmltype(''<Head>' ||
              AXMLFragmentEquivalent.GetClobVal ||
              '</Head>''),''/Head/BatchComponentFragmentList[1]/BatchComponentFragment[FragmentID=''||FragmentID||'']/Equivalents'')';

        TraceWrite('ValidateCompFrag_4_FragmentEqsQuery', $$plsql_line, LQuery);

        LQryCtx := DBMS_XMLGEN.newContext(LQuery);
        DBMS_XMLGEN.setMaxRows(LqryCtx, 3);
        DBMS_XMLGEN.setRowTag(LqryCtx, '');
        LSameEquivalent := DBMS_XMLGEN.getXML(LqryCtx);
        DBMS_XMLGEN.closeContext(LqryCtx);

        TraceWrite('ValidateCompFrag_5_CompoundSameFragEqs', $$plsql_line, 'LSameEquivalent ' || LSameEquivalent);

        IF LSameEquivalent IS NOT NULL THEN
          LResult := 'SAMEFRAGMENT="True" SAMEEQUIVALENT="False"';
        ELSE
          LResult := 'SAMEFRAGMENT="True" SAMEEQUIVALENT="True"';
        END IF;
      ELSE
        LResult := 'SAMEFRAGMENT="False" SAMEEQUIVALENT="False"';
      END IF;
    ELSE
      -- If there's no 'master' compound-fragment list
      LQuery :=
        'SELECT 1 FROM VW_Compound_Fragment CF,VW_BatchComponentFragment BCF, VW_BatchComponent BC
        WHERE CF.CompoundID = ''' || ACompoundID ||''' AND CF.ID=BCF.CompoundFragmentID AND BCF.BatchComponentID=BC.ID';

      TraceWrite('ValidateCompFrag_6_FragmentsQuery', $$plsql_line, LQuery);

      LQryCtx := DBMS_XMLGEN.newContext(LQuery);
      DBMS_XMLGEN.setMaxRows(LqryCtx, 3);
      DBMS_XMLGEN.setRowTag(LqryCtx, null);
      LSameFragment := DBMS_XMLGEN.getXML(LqryCtx);
      DBMS_XMLGEN.closeContext(LqryCtx);

      TraceWrite('ValidateCompFrag_7_CompoundSameFrags', $$plsql_line, 'LSameFragment ' || LSameFragment);

      IF LSameFragment IS NULL THEN
          LResult := 'SAMEFRAGMENT="True" SAMEEQUIVALENT="True"';
      ELSE
          LResult := 'SAMEFRAGMENT="False" SAMEEQUIVALENT="False"';
      END IF;
    END IF;

    TraceWrite('ValidateCompFrag_8_Result', $$plsql_line, 'LResult = ' || LResult);
    TraceWrite( act||'ValidateCompoundFragment_ended', $$plsql_line,'end' );
    RETURN LResult;

  END;

  /**
  Determines if a 'compound' (a chemical structure) is unique in the system. If not,
  additional information is gathered about the uniqueness of any associated component
  fragments for the consumer to enforce the 'SameBatchesIdentity' rules.
  -->author Fari (re-factored by Jeff D.)
  -->param AStructure the string representation of a chemical structure
  -->param AStructureIDToValidate
  -->param AConfigurationID defaults to 1 (currently unused JAN 2011)
  -->param AXMLCompound can hold the xml representation of a compound (currently unused JAN 2011)
  -->param AXMLFragmentEquivalent can hold the fragment information for a component
  -->param AREGNumber optional, to avoid to determine Reg_number, which we updating now, as duplicate of itself. Bug CBOE-8400. Works only for UPPER(vAllowUnregistCompInMix)='TRUE'
  */
  FUNCTION ValidateCompoundMulti(
    AStructure CLOB
    , AStructureIDToValidate Number := NULL
    , AConfigurationID Number := 1
    , AXMLCompound XmlType
    , AXMLFragmentEquivalent XmlType
    , AREGNumber varchar2 :=null
  ) RETURN CLOB IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    LResult              CLOB;
    LDuplicateCount      Number;
    LParameters          Varchar2(1000);
    LCompoundID          VW_Compound.CompoundID%Type;
    LCount               NUMBER := 0;
    LFormulaWeight       VW_TEMPORARYCOMPOUND.FormulaWeight%Type;
    LFragmentsData       VARCHAR2(4000);
    LResultXMLType       XMLType;
    LScanCtx             cscartridge.moleculeindexmethods;
    CURSOR C_RegNumbers(ACoumpoundID in VW_Compound.CompoundID%type) IS
      SELECT RegNumber
      FROM VW_RegistryNumber RN,VW_Mixture M,VW_Mixture_Component MC
      WHERE RN.RegID=M.RegID AND M.MixtureID=MC.MixtureID AND MC.CompoundID=ACoumpoundID
	  --CBOE-8421
      and not exists (select 1 from VW_REGISTRYNUMBER  rn where M.RegID=rn.regid and regnumber = AREGNumber)
	  --CBOE-8421.End
      ORDER BY MC.CompoundID;
    cursor C_REGNUMBERSONLYSINGLEREGISTRY(ACOUMPOUNDID in VW_COMPOUND.COMPOUNDID%type) is
      select REGNUMBER
      FROM (SELECT REGNUMBER, COUNT(*) OVER  (partition by MC.MIXTUREID) as cnt
      FROM
       VW_REGISTRYNUMBER RN,VW_MIXTURE M,VW_MIXTURE_COMPONENT MC where RN.REGID=
        M.REGID AND M.MIXTUREID=MC.MIXTUREID AND MC.COMPOUNDID=ACoumpoundID
        ) where CNT = 1;
    mod1 varchar2(100);
    act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'ValidateCompoundMulti_started', $$plsql_line,'start' );
    TraceWrite('ValidateMcrr_0.1_ComponentMatches', $$plsql_line, AStructure);
    TraceWrite('ValidateMcrr_0.2_StructureID', $$plsql_line, to_char(AStructureIDToValidate));

    -- This is the most definitive way of saying "it's a chemical drawing"
    LFormulaWeight := CSCartridge.MolWeightC(AStructure, null, LScanCtx, 0);

    -- bypass non-structural entities (like bitmaps)
    if LFormulaWeight != 0 then
      -- a csv string of the "key=value" setting pairs
      LParameters := GetStructureDupCheckSettings;

      -- find structure-based matches
      LResult := RegistryDuplicateCheck.FindComponentDuplicates('S', LParameters, AStructure);
      -- you have to either commit or rollback the cartridge's autonomous transaction
      commit;
      TraceWrite('ValidateMcrr_1_StructureMatches', $$plsql_line, LResult);

      -- convert to xml and use that xml for further information
      LResultXMLType := xmltype(LResult);
      select Extract(LResultXMLType, '/RegistryList/@uniqueComps').getNumberVal()
      into LDuplicateCount from dual;
      TraceWrite('ValidateMcrr_2_DuplicateComponents', $$plsql_line, to_char(LDuplicateCount));

      -- re-use this string
      LResult := null;
      dbms_lob.createtemporary(LResult,true);
      -- put together a list of component matches
      -- be sure to exclude the potantial match with 'self' for update scenarios
      if (LDuplicateCount is not null and LDuplicateCount > 0) then
        for rec in (
          select distinct "compoundid"
          from xmltable(
            '/RegistryList/Registration'
            passing LResultXMLType
            columns
              "compoundid" number path 'Component/@id',
              "structureid" number path 'Component/Structure/@id'
          ) components
          where "structureid" <> NVL(ASTRUCTUREIDTOVALIDATE, 0)
          order by components."compoundid"
        )
        loop
          LCompoundID := rec."compoundid";

          if (LCompoundID is not null) then
            -- Get a count of how many registrations that component is found in
            select count (*) into lcount
            from vw_mixture_component mc
            where mc.compoundid = lcompoundid
              and (
                select count (*)
                from vw_mixture m, vw_mixture_component mcc
                where m.mixtureid = mc.mixtureid and m.mixtureid = mcc.mixtureid
              ) = 1;

            -- We're only matching the ocmpound's *structure*, but we're fetching additional
            -- information to empower the caller regarding 'SameBatchesIdentity' system setting.
            LFragmentsData := ValidateCompoundFragment(LCompoundID,AXMLCompound,AXMLFragmentEquivalent);

            -- 'LFragmentsData' is an attribute string (this="someValue")!
            vAllowUnregistCompInMix:=GetAllowUnregistCompInMix;
            if UPPER(vAllowUnregistCompInMix)='TRUE' THEN
                for R_RegNumbers IN C_RegNumbers(LCompoundID) loop
                      dbms_lob.append(LResult,to_clob('<REGNUMBER count="' || LCount
                      || '" CompoundID="' || LCompoundID
                      || '" '||LFragmentsData || '>'
                      || R_RegNumbers.RegNumber || '</REGNUMBER>'));
                end loop;
            else
                for R_RegNumbersOnlySingleRegistry IN C_RegNumbersOnlySingleRegistry(LCompoundID) loop
                  dbms_lob.append(LResult,to_clob('<REGNUMBER count="' || LCount
                    || '" CompoundID="' || LCompoundID
                    || '" '||LFragmentsData || '>'
                    || R_RegNumbersOnlySingleRegistry.RegNumber || '</REGNUMBER>'));
                end loop;
            end if;
          end if;

        end loop;
      end if;
      -- wrap the results in a 'list' tag
      LResult := '<REGISTRYLIST>' || LResult || '</REGISTRYLIST>';
    end if;

    TraceWrite('ValidateMcrr', $$plsql_line, LResult);
    TraceWrite( act||'ValidateCompoundMulti_ended', $$plsql_line,'end' );
    RETURN LResult;
  END;

  /**
  Gets the Compound Registry Numbers that duplicate the structure.
  -->author Fari
  -->since June 2010
  -->param ARegNumber              Compund Registry Number of the structure to evaluate
  -->param AStructure              Strcuture to validate
  -->param ARegIDToValidate        Identify the RegID of the Compound of the Strcuture,It should not be taked into account in the comparation
  -->param LXMLRegNumberDuplicated List of the Registry Numbers Duplicated
  */
  PROCEDURE VerifyAndAddDuplicateToSave(
    ARegNumber VW_RegistryNumber.RegNumber%Type
    , AStructure CLOB
    , ARegIDToValidate Number := NULL
    , LXMLRegNumberDuplicated IN OUT NOCOPY XMLType
  ) IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    LqryCtx              DBMS_XMLGEN.ctxHandle;
    LResult              CLOB;
    LResultXML           CLOB;
    LDuplicateCount      Number;
    LParameters          Varchar2(1000);
    LFormulaWeight       VW_TEMPORARYCOMPOUND.FormulaWeight%Type;
    LRLSState            Boolean;
    LScanCtx             cscartridge.moleculeindexmethods;
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'VerifyAndAddDuplicateToSave_started', $$plsql_line,'start' );

    InsertLog('VerifyAndAddDuplicateToSave','Begin');

    LFormulaWeight := CSCartridge.MolWeightC(AStructure, null, LScanCtx, 0);

    --Duplicate Validation
    IF (LFormulaWeight != 0) THEN

        $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','AStructureXML to validate->'||AStructure); $end null;

        INSERT INTO CsCartridge.TempQueries (Query, Id) VALUES (AStructure, 0);

        LParameters := GetStructureDupCheckSettings;
        LRLSState := RegistrationRLS.GetStateRLS;

        IF LRLSState THEN
          RegistrationRLS.SetEnableRLS(False);
        END IF;

        IF NVL(ARegIDToValidate,0)<>0 THEN
            LQryCtx := DBMS_XMLGEN.newContext('
              SELECT RN.REGNUMBER
                  FROM VW_Compound C, Structures S,VW_RegistryNumber RN
                  WHERE RN.RegID=C.RegID and C.RegID<>'||ARegIDToValidate||' AND C.StructureId = S.cpd_internal_id AND
                        CsCartridge.MoleculeContains(S.base64_cdx, ''select query from cscartridge.tempqueries where id = 0'','''','''||LParameters||''') = 1
                  ORDER BY C.RegID');
        ELSE
            LQryCtx := DBMS_XMLGEN.newContext('
                SELECT RN.REGNUMBER
                    FROM  VW_Compound C, Structures S,VW_RegistryNumber RN
                    WHERE RN.RegID=C.RegID and C.StructureId = S.cpd_internal_id AND
                          CsCartridge.MoleculeContains(S.base64_cdx, ''select query from cscartridge.tempqueries where id = 0'','''','''||LParameters||''') = 1
                    ORDER BY C.RegID');
        END IF;


        DBMS_XMLGEN.setMaxRows(LqryCtx,30);
        DBMS_XMLGEN.setRowTag(LqryCtx, '');
        LResultXML := DBMS_XMLGEN.getXML(LqryCtx);
        DBMS_XMLGEN.closeContext(LqryCtx);

        IF LRLSState THEN
          RegistrationRLS.SetEnableRLS(LRLSState);
        END IF;

        LResult := replace(LResultXML,chr(10),'');
        COMMIT;

        $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','LDuplicateCount->'||LDuplicateCount||' LResult->'||LResult); $end null;
        $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','AStructureXML to validate->'||AStructure); $end null;


        IF LResultXML IS NOT NULL THEN  --Save in VW_Duplciated
            $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','1LResultXML->'||LResultXML); $end null;
            $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','LResult->'||LResult); $end null;

            IF LXMLRegNumberDuplicated IS NOT NULL THEN
                $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','ARegNumber='||ARegNumber); $end null;
                LResultXML:=REPLACE(LResultXML,'<REGNUMBER>','<REGNUMBER NEW='''||ARegNumber||'''>');
                LResultXML:=REPLACE(LResultXML,'</ROWSET>','');
                SELECT extract(LXMLRegNumberDuplicated,'//ROWSET/REGNUMBER').GetClobVal()
                    INTO LResult
                    FROM DUAL;
                $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','LResult||LResultXML||''</ROWSET>''->'||LResult||LResultXML||'</ROWSET>'); $end null;
                LXMLRegNumberDuplicated:=XmlType.CreateXml(LResultXML||LResult||'</ROWSET>');
                $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','LXMLRegNumberDuplicated->'||LXMLRegNumberDuplicated.GetClobVal()); $end null;
            ELSE
                $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','ARegNumber='||ARegNumber); $end null;
                LResultXML:=REPLACE(LResultXML,'<REGNUMBER>','<REGNUMBER NEW='''||ARegNumber||'''>');
                LXMLRegNumberDuplicated:=XmlType.CreateXml(LResultXML);
                $if CompoundRegistry.Debuging $then InsertLog('VerifyAndAddDuplicateToSave','LXMLRegNumberDuplicated->'||LXMLRegNumberDuplicated.GetClobVal()); $end null;
            END IF;
        END IF;
    END IF;
    TraceWrite( act||'VerifyAndAddDuplicateToSave_ended', $$plsql_line,'end' );
  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eCompoundValidation, AppendError('Error validating the compound.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
  END;

  /**
  Retrieve a XML with the list of Registry Number Duplciated. The argument identify the Registry Number appraise.
  -->author Fari
  -->since  June 2010
  -->param  ARegNumber              Compund Registry Number of the structure to evaluate
  -->return List of Registry Number Duplciated. For example CompoundRegistry.GetDuplicatedList('C/000048') gets:
              <DuplicateList RegNumber="C/000048">
                      <RegNumber PersonID="94" DuplicateDate="2010-06-08 01:15:40">C/000046/RegNumber>
                      <RegNumber PersonID="94" DuplicateDate="2010-06-08 01:15:350>C/000047</RegNumber>
                      <RegNumber PersonID="94" DuplicateDate="2010-06-08 01:15:55">C/000049</RegNumber>
              </DuplicateList>
  */
  FUNCTION GetDuplicatedList(ARegNumber IN VW_RegistryNumber.RegNumber%type) RETURN CLOB IS

    LListDuplicated CLOB;

    CURSOR C_RegNumberDuplicated(ARegNumber IN VW_RegistryNumber.RegNumber%type) IS
        SELECT RegNumberDuplicated,PersonID,Created
            FROM VW_Duplicates
            WHERE RegNumber=ARegNumber
        UNION ALL
        SELECT RegNumber,PersonID,Created
            FROM VW_Duplicates
            WHERE RegNumberDuplicated=ARegNumber
        ORDER BY 1;
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetDuplicatedList_started', $$plsql_line,'start' );
    SetSessionParameter;

    LListDuplicated:='<DuplicateList RegNumber="'||ARegNumber||'">';
    FOR R_RegNumberDuplicated IN C_RegNumberDuplicated(ARegNumber) LOOP
        LListDuplicated:=LListDuplicated||'<RegNumber PersonID="'||R_RegNumberDuplicated.PersonID||'" DuplicateDate="'||R_RegNumberDuplicated.Created||'">'||R_RegNumberDuplicated.RegNumberDuplicated||'</RegNumber>';
    END LOOP;

    LListDuplicated:=LListDuplicated||'</DuplicateList>';
    TraceWrite( act||'GetDuplicatedList_ended', $$plsql_line,'end' );
    RETURN LListDuplicated;
  END;

  FUNCTION TakeOffAndGetClob(AXml IN OUT NOCOPY Clob,ABeginTag VARCHAR2) RETURN CLOB IS
    LValue           CLOB;
    LTagBegin        Number;
    LTagEnd          Number;
    LEndTag          VARCHAR2(255);
    LBeginTag        VARCHAR2(255);
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'TakeOffAndGetClob_started', $$plsql_line,'start' );

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
    TraceWrite( act||'TakeOffAndGetClob_ended', $$plsql_line,'end' );
    RETURN LValue;
  END;

  /**
  Helper procedure to trim the element name
  -->param ATag   string representation of an xml element
  -->return       the element name ignoring any provided '<', '>', or spaces
  */
  FUNCTION TrimElement(ATag VARCHAR2) RETURN VARCHAR2 IS
    LTag  VARCHAR2(255);
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'TrimElement_started', $$plsql_line,'start' );
    -- Get the element name ignoring any provided '<', '>', or spaces
    LTag := Trim(ATag);
    LTag := Ltrim(Ltrim(LTag, '<'));
    LTag := Rtrim(Rtrim(LTag, '>'));
    TraceWrite( act||'TrimElement_ended', $$plsql_line,'end' );
    RETURN LTag;
  END;

  /**
  Helper procedure for locating tags in XML.
  ALft and ARht will be the positions of the '<' and '>' respectively. Both are 0 if the element is not found.
  -->param AXml
  -->param ATag
  -->param ALft
  -->param ARht
  -->param AStart
  -->return       a string representation of the element 'begin' tag
  */
  FUNCTION LocateElement(AXml IN CLOB, ATag VARCHAR2, ALft OUT NUMBER, ARht OUT NUMBER, AStart IN NUMBER := 1) RETURN VARCHAR2 IS
    LTag  VARCHAR2(255);
    LTemp VARCHAR2(255);
    LLft  NUMBER;
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
  dbms_application_info.read_module(mod1, act); TraceWrite( act||'LocateElement_started', $$plsql_line,'start' );
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
          TraceWrite( act||'LocateElement_ended', $$plsql_line,'end' );
          RETURN NULL; -- Unable to locate the tag
        END IF;
        LTemp := Substr(AXml, LLft, 1 + Length(LTag) + 1);
        EXIT WHEN (LTemp = '<' || LTag || '>');
        EXIT WHEN (LTemp = '<' || LTag || ' ');
        LLft := LLft + 1;
    END LOOP;
    -- Locate the right end of the element
    ARht := NVL(Instr(AXml, '>', LLft), 0);
    TraceWrite( act||'LocateElement_ended', $$plsql_line,'end' );
    IF ARht = 0 THEN
        RETURN NULL; -- Unable to locate the matching '>' (should not happen)
    END IF;
    ALft := LLft; -- Located the right end so now we can safely return the left end
    RETURN Substr(AXml, ALft, ARht + 1 - ALft);
  END;

  /**
  Helper procedure for locating a matching end element
  This is not trivial if elements with the same tag name are nested
  AStart must point to the '<' of the begin element
  -->param AXml
  -->param AStart
  -->param ALft
  -->param ARht
  -->return       a string representation of the element 'end' tag
  */
  FUNCTION LocateMatchingElement(AXml IN CLOB, AStart IN NUMBER, ALft OUT NUMBER, ARht OUT NUMBER) RETURN VARCHAR2 IS
    LDepth        NUMBER := 0;
    LTag          VARCHAR2(255);
    LBegin        VARCHAR2(255);
    LBeginLft     NUMBER;
    LBeginRht     NUMBER;
    LEnd          VARCHAR2(255);
    LEndLft       NUMBER;
    LEndRht       NUMBER;
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'LocateMatchingElement_started', $$plsql_line,'start' );
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
    TraceWrite( act||'LocateMatchingElement_ended', $$plsql_line,'end' );
    RETURN Substr(AXml, ALft, ARht + 1 - ALft);
  END;

  /**
  Excises a complete xml element (as a CLOB) from a parent document; caller determines
  if it is a 'cut' or 'copy' operation. (?)
  -->param AXml
  -->param ABeginTag
  -->param ABeginTagParent
  -->param ABeginTagGranParent  defaults to NULL
  -->param ASourceDeleteTagName defaults to True
  -->param AUpdateVerify        defaults to False
  -->return                     a string representation of an element
  */
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
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'TakeOffAndGetClobsList_started', $$plsql_line,'start' );
    LSearchPos := 1;
    LOOP
        -- Locate grandparent if any
        IF ABeginTagGranParent IS NOT NULL THEN
            LElement := LocateElement(AXml, ABeginTagGranParent, LElementLft, LElementRht, LSearchPos);
            EXIT WHEN LElementLft = 0; -- Unable to locate the grandparentubstr(LElement, Length(LElement) - 1, 1) = '/'; -- Grandparent is empty
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
    LReturn := '<ClobList>' || LReturn || '</ClobList>';
    TraceWrite( act||'TakeOffAndGetClobsList_ended', $$plsql_line,'end' );
    RETURN LReturn;
  END;

  FUNCTION TakeOnAndGetXml(
    AXml IN Clob
    , ATagName VARCHAR
    , AStructuresList IN OUT NOCOPY Clob
  ) RETURN Clob IS
    LValue             CLOB;
    LStructureValue    CLOB;
    LStructuresList    CLOB;
    LTagBegin          Number;
    LValueStr          Varchar2(255);
    LStructureTagBegin Number;
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'TakeOnAndGetXml_started', $$plsql_line,'start' );
    LValue := AXml;
    LStructuresList := AStructuresList;
    LValueStr := '(Removed'||ATagName||')';
--    TraceWrite('TakeOnAndGetXml - AXml' , $$plsql_line, AXml);
--    TraceWrite('TakeOnAndGetXml - ' || ATagName, $$plsql_line, ATagName);
--    TraceWrite('TakeOnAndGetXml - AStructuresList', $$plsql_line, AStructuresList);
    LOOP
      LTagBegin := INSTR(LValue,LValueStr);
    EXIT WHEN (LTagBegin=0);
      LStructureValue := TakeOffAndGetClob(LStructuresList, 'Clob');
      LStructureTagBegin := INSTR(LValue, LValueStr);
      IF LStructureTagBegin <> 0 THEN
        LValue := SUBSTR(LValue, 1, LStructureTagBegin-1)
          || LStructureValue
          || SUBSTR(LValue, LStructureTagBegin+LENGTH(LValueStr), LENGTH(LValue));
      END IF;
    END LOOP;

--    TraceWrite('LValueStr_end - ' || LValueStr, $$plsql_line, LValue);
    TraceWrite( act||'TakeOnAndGetXml_ended', $$plsql_line,'end' );
    RETURN LValue;
  END;

  FUNCTION TakeOnAndGetXml_new(
    AXml IN Clob
    , ATagName VARCHAR
    , AStructuresList IN OUT NOCOPY Clob
  ) RETURN Clob IS
    LValue             CLOB;
    LStructureValue    CLOB;
    LStructuresList    CLOB;
    LTagBegin          Number;
    LValueStr          Varchar2(255);
    LStructureTagBegin Number;
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'TakeOnAndGetXml_started', $$plsql_line,'start' );
    LValue := AXml;
    LStructuresList := AStructuresList;
    TraceWrite('TakeOnAndGetXml - AXml' , $$plsql_line, AXml);
    TraceWrite('TakeOnAndGetXml - ' || ATagName, $$plsql_line, ATagName);
    TraceWrite('TakeOnAndGetXml - AStructuresList', $$plsql_line, AStructuresList);
--    LValueStr := '<'||ATagName||'>'||'(Removed' || ATagName || ')'||'</'||ATagName||'>';
    LValueStr := '(Removed' || ATagName || ')';
    LOOP
      LTagBegin := INSTR(LValue,LValueStr);
    EXIT WHEN (LTagBegin=0);
      LStructureValue := TakeOffAndGetClob(LStructuresList, 'Clob');
      LStructureTagBegin := INSTR(LValue, LValueStr);
      IF LStructureTagBegin <> 0 THEN
        LValue := SUBSTR(LValue, 1, LStructureTagBegin-1)
          || LStructureValue
          || SUBSTR(LValue, LStructureTagBegin+LENGTH(LValueStr), LENGTH(LValue));
      END IF;
    END LOOP;

    TraceWrite('LValueStr - ' || LValueStr, $$plsql_line, LValue);
    TraceWrite( act||'TakeOnAndGetXml_ended', $$plsql_line,'end' );
    RETURN LValue;
  END;

  FUNCTION GenerateFragmentSuffixForBatch (
    p_batchId IN number
    , p_fragment_delimiter IN varchar2 default '*'
    , p_fragment_attribute IN varchar2 default 'code'
  )
  RETURN VARCHAR2 IS
    v_result varchar2(1000);
    v_batchXml xmltype;
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GenerateFragmentSuffixForBatch_started', $$plsql_line,'start' );

    if (p_fragment_attribute is null) then
      return null;
    end if;

    select
      xmlelement( "fragments",
        xmlagg(
            xmlelement( "fragment",
              xmlattributes(
                b.batchid as "batId",
                b.batchnumber as "batNum",
                bcf.orderindex as "order",
                f.code as "code",
                f.fragmentid as "id",
                f.formula as "formula",
                f.description as "desc"
              )
           )
        )
      ) into v_batchXml
    from vw_batch b
      left outer join vw_batchcomponent bc on bc.batchid = b.batchid
      left outer join vw_batchcomponentfragment bcf on bcf.batchcomponentid = bc.id
      left outer join vw_compound_fragment cf on cf.id = bcf.compoundfragmentid
      left outer join vw_fragment f on f.fragmentid = cf.fragmentid
    where b.batchid = p_batchId
    order by b.batchid, b.batchnumber, bcf.orderindex asc;

    -- 1. Oracle 10g can't handle non-literal string values in a string-join call, so using xslt instead
    -- 2. GetStringVal() will return NULL if the output is strictly numeric (ex. one fragment using 'id' attribute)
    select XmlTransform(v_batchXml, XmlType.CreateXml('
      <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
        <xsl:output method="text" indent="no"/>
        <xsl:template match="//fragments">
          <xsl:for-each select="fragment/@' || p_fragment_attribute || '">
            <xsl:value-of select="normalize-space(.)"/><xsl:if test="not(position() = last())"><xsl:text>' || p_fragment_delimiter || '</xsl:text></xsl:if>
          </xsl:for-each>
        </xsl:template>
      </xsl:stylesheet>')
    ).GetClobVal() into v_result from dual;
    TraceWrite( act||'GenerateFragmentSuffixForBatch_ended', $$plsql_line,'end' );
    RETURN v_result;
  END;

  /**
  Generates a full registration number for a registration record.
  -->param ASequenceID
  -->param ASequenceNumber
  -->param AXmlTables
  -->param AIncSequence         defaults to 'Y'
  -->return                     a registration number
  */
  FUNCTION GetRegNumber(
    ASequenceID in VW_SEQUENCE.SequenceId%Type
    , ASequenceNumber out VW_REGISTRYNUMBER.SequenceNumber%Type
    , AXmlTables IN XmlType
    , AIncSequence IN Char:='Y'
  ) RETURN VW_REGISTRYNUMBER.RegNumber%Type IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    LRegNumber VW_REGISTRYNUMBER.RegNumber%Type;
    v_batch_prefix_setting varchar(50);
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetRegNumber_started', $$plsql_line,'start' );
    v_batch_prefix_setting := upper(GetEnableUseBatchPrefixes);
    IF AIncSequence='Y' THEN
      SELECT
         case when type='R' then decode(v_batch_prefix_setting,'TRUE','', Prefix||PrefixDelimiter)||lpad(NVL(NextInSequence,1),RegNumberLength,'0')
      else
         Prefix||PrefixDelimiter||lpad(NVL(NextInSequence,1),RegNumberLength,'0')
      end

      ,NVL(NextInSequence,1)
      INTO LRegNumber,ASequenceNumber
      FROM VW_SEQUENCE
      WHERE SequenceID=ASequenceID
      FOR UPDATE;

      UPDATE VW_SEQUENCE
      SET NextInSequence=NVL(NextInSequence,1)+1
      WHERE SequenceID=ASequenceID;

      COMMIT;
    ELSE
     /* SELECT decode(v_batch_prefix_setting,'TRUE','', Prefix||PrefixDelimiter)||lpad(NVL(NextInSequence,1),RegNumberLength,'0'),NVL(NextInSequence,1)
      INTO LRegNumber,ASequenceNumber
      FROM VW_SEQUENCE
      WHERE SequenceID=ASequenceID;*/
      SELECT
         case when type='R' then decode(v_batch_prefix_setting,'TRUE','', Prefix||PrefixDelimiter)||lpad(NVL(NextInSequence,1),RegNumberLength,'0')
      else
         Prefix||PrefixDelimiter||lpad(NVL(NextInSequence,1),RegNumberLength,'0')
      end

      ,NVL(NextInSequence,1)
      INTO LRegNumber,ASequenceNumber
      FROM VW_SEQUENCE
      WHERE SequenceID=ASequenceID;

    END IF;
     TraceWrite( act||'GetRegNumber_ended', $$plsql_line,'end' );
    RETURN LRegNumber;
  END;

  -- TODO: determine the logic for this procedure
  FUNCTION GenerateRegistryRegNumber(
    p_mixtureId vw_mixture.mixtureid%type
  ) RETURN VARCHAR2
  IS
    v_regnum vw_registrynumber.regnumber%type;
    v_sequenceId vw_registrynumber.sequenceid%type;
    v_autonumber vw_registrynumber.sequencenumber%type;
    v_prefix vw_sequence.prefix%type;
    v_prefix_separator vw_sequence.prefixdelimiter%type;
    v_reg_autonum_length vw_sequence.regnumberlength%type;
    v_reg_autonum_next vw_sequence.nextinsequence%type;
    v_suffix_separator vw_sequence.suffixdelimiter%type;
    -- "v_root" isn't YET a true value of the record
    v_root vw_registrynumber.regnumber%type;
    v_first_batchId vw_batch.batchid%type;
    v_suffix_value varchar2(200);
    v_sbi_setting varchar2(10);
    v_mixture_setting varchar2(10);
    v_output_value vw_registrynumber.regnumber%type;

    v_component_count integer;
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GenerateRegistryRegNumber_started', $$plsql_line,'start' );

    select
      mrn.regnumber
      , rn.sequenceid
      , rn.sequencenumber
      , s.prefix
      , s.prefixdelimiter
      , s.regnumberlength
      , s.nextinsequence
      , s.suffixdelimiter
    into
      v_regnum
      , v_sequenceId
      , v_autonumber
      , v_prefix
      , v_prefix_separator
      , v_reg_autonum_length
      , v_reg_autonum_next
      , v_suffix_separator
    from vw_mixture_regnumber mrn
      inner join vw_registrynumber rn on rn.regid = mrn.regid
      inner join vw_sequence s on s.sequenceid = rn.sequenceid
    where mrn.mixtureid = p_mixtureId;
    --for update;

    --TODO: Add the root_number to the registration table and business object

    if (v_root is not null) then
      -- the registration uses another registration's root number
      -- when fragments are maintained at the compound level,
      --   we simply need to add the custom suffix to the RegNum

      null;
    else
      -- this is a new registration
      -- increment the registry-level autonumber
      v_reg_autonum_next := NVL(v_reg_autonum_next, 0) + 1;

      -- (re)create the root number
      v_root := v_prefix
        || case when v_prefix is null then null else v_prefix_separator end
        || LPad(v_reg_autonum_next, v_reg_autonum_length, '0');

      -- apply the edit
      update vw_sequence s
      set s.nextinsequence = v_reg_autonum_next
      where s.sequenceid = v_sequenceId;
    end if;

    v_sbi_setting := VSameBatchesIdentity;
    v_mixture_setting := upper(GetMixturesEnabled);
    vAllowUnregistCompInMix:=upper(GetAllowUnregistCompInMix);
    if ( v_sbi_setting = upper('true') OR (v_mixture_setting = upper('true') AND vAllowUnregistCompInMix=upper('true'))) then
      v_output_value := v_root;
    else
      select batchId
      into v_first_batchId
      from (
        select
          b.batchid
        from vw_batch b
          inner join vw_batchcomponent bc on bc.batchid = b.batchid
          inner join vw_mixture_component mc on mc.mixturecomponentid = bc.mixturecomponentid
          inner join vw_mixture m on m.mixtureid = mc.mixtureid
        where m.mixtureid = p_mixtureId
        order by b.batchnumber ASC
      ) b
      where rownum = 1;

      -- fetch the suffix
      v_suffix_value := GenerateFragmentSuffixForBatch(v_first_batchId);

      -- apply the suffix
      v_output_value := v_root || v_suffix_separator || v_suffix_value;
    end if;
    TraceWrite( act||'GenerateRegistryRegNumber_ended', $$plsql_line,'end' );
    RETURN v_output_value;
  END;

  /**
  Using a Batch's associated Registration number and fragments, a system
  setting for the batch number padding, and metadata about the CS Sequence
  associated with the Registration, generates the 'FullRegNum' value for an
  individual batch. NOTE: use the sxpecial value "-1" for the batch number
  padding in order to bypass the padding altogether.
  -->author jed
  -->since April 2011
  -->param p_batchId IN the individual batch's identifier
  -->param p_batch_number_padding IN the system setting controlling the batch number length
  -->return a string representing the FullRegNum for the batch
  */
  FUNCTION GenerateBatchRegNumber(
    p_batchId vw_batch.batchid%type
  ) RETURN VARCHAR2
  IS
    v_root_regnum vw_registrynumber.regnumber%type;
    v_batch_number vw_batch.batchnumber%type;
    v_sequenceId vw_sequence.sequenceid%type;
    v_prefix_delimiter vw_sequence.PREFIXDELIMITER%type;
    v_frag_suffix_type vw_sequence.saltsuffixtype%type;
    v_frag_suffix_delimiter vw_sequence.suffixdelimiter%type;
    v_batch_delimiter vw_sequence.batchdelimiter%type;
    v_batchnum_length vw_sequence.batchnumlength%type;

    v_suffix_value varchar2(50) := null;
    v_output_value vw_batch.fullregnumber%type;
    v_frag_attribute varchar2(100);

    v_component_count number := 0;
    v_batch_prefix_value varchar(100);
    v_saltandsuffix_text varchar2(100);
     v_mixture_setting varchar2(10);
    v_sbi_setting varchar2(255);
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GenerateBatchRegNumber_started', $$plsql_line,'start' );

    --first, determine if this is a batch of a 'mixture' or a batch of a 'compound'
    select count(*) into v_component_count
    from mixture_component where mixtrureid in (
      select mix_internal_id from mixtures where regid in (
      select reg_internal_id from batches where batch_internal_id = p_batchId
      )
    );

    v_sbi_setting := VSameBatchesIdentity;

    if ( v_component_count = 1 ) then
       -- get all the necessary data and sequence metadata
      select
        mrn.regnumber
        , b.batchnumber
        , s.sequenceid
        , s.saltsuffixtype
        , s.suffixdelimiter
        , s.batchdelimiter
        , s.batchnumlength
        , s.prefixdelimiter
      into
        v_root_regnum
        , v_batch_number
        , v_sequenceId
        , v_frag_suffix_type
        , v_frag_suffix_delimiter
        , v_batch_delimiter
        , v_batchnum_length
        , v_prefix_delimiter
      from vw_batch b
        inner join vw_mixture_batchcomponent mbc on mbc.batchid = b.batchid
        inner join vw_mixture_regnumber mrn on mrn.mixtureid = mbc.mixtureid
        inner join vw_registrynumber rn on rn.regid = mrn.regid
        inner join vw_sequence s on s.sequenceid = rn.sequenceid
      where b.batchid = p_batchId;

      --> determine which attribute to use
      select
        case upper(v_frag_suffix_type)
          when 'GETSALTCODE' then 'code'
          when 'GETSALTDESCRIPTION' then 'desc'
          when 'GETSALTFORMULA' then 'formula'
          else lower(v_frag_suffix_type)
        end case
      into v_frag_attribute from dual;

      if  v_sbi_setting = upper('false') then
        v_suffix_value := GenerateFragmentSuffixForBatch(p_batchId, '*', v_frag_attribute);
      else
        v_suffix_value := null;
      end if;
    else
      -- get all the necessary data and sequence metadata
      select
        mrn.regnumber
        , b.batchnumber
        , s.sequenceid
        , s.saltsuffixtype
        , s.suffixdelimiter
        , s.batchdelimiter
        , s.batchnumlength
        , s.prefixdelimiter
      into
        v_root_regnum
        , v_batch_number
        , v_sequenceId
        , v_frag_suffix_type
        , v_frag_suffix_delimiter
        , v_batch_delimiter
        , v_batchnum_length
        , v_prefix_delimiter
      from vw_batch b
        inner join vw_mixture_batch mb on mb.batchid = b.batchid
        inner join vw_mixture_regnumber mrn on mrn.mixtureid = mb.mixtureid
        inner join vw_registrynumber rn on rn.regid = mrn.regid
        inner join vw_sequence s on s.sequenceid = rn.sequenceid
      where b.batchid = p_batchId;

      --> determine which attribute to use
      select
        case upper(v_frag_suffix_type)
          when 'GETSALTCODE' then 'code'
          when 'GETSALTDESCRIPTION' then 'desc'
          when 'GETSALTFORMULA' then 'formula'
          else lower(v_frag_suffix_type)
        end case
      into v_frag_attribute from dual;


      v_sbi_setting := VSameBatchesIdentity;
      v_mixture_setting := upper(GetMixturesEnabled);
      vAllowUnregistCompInMix:=upper(GetAllowUnregistCompInMix);
      if ( v_sbi_setting = upper('false') OR (v_mixture_setting = upper('true') AND vAllowUnregistCompInMix=upper('false'))) then
        v_suffix_value := GenerateFragmentSuffixForBatch(p_batchId, '*', v_frag_attribute);
      else
        v_suffix_value := null;
      end if;

    end if;

    -- provide a starting point: the mixture's RegNumber
    v_output_value := v_root_regnum;

    --Apply the Salt Suffix part if Batch perfix slected and RegNum less than 10000
    v_saltandsuffix_text :=GetSaltAndSuffixValue(p_batchId);
    v_batch_prefix_value :=GetBatchPrefixValue(p_batchId);
    --> apply the fragment suffix
    if v_suffix_value is NOT NULL then
      v_output_value :=
        SUBSTR( v_output_value || v_frag_suffix_delimiter || v_suffix_value, 1, 50 );
    end if;

    --> combine the parts of the 'full registration number' into a single string
    if (v_batchnum_length = -1) then
      v_output_value := SUBSTR(
        ( v_output_value || v_batch_delimiter || v_batch_number ),1 ,50
      );
    else
      v_output_value := SUBSTR(
        ( v_output_value || v_batch_delimiter || LPad( v_batch_number, v_batchnum_length, '0' ) ),1 ,50
      );
    end if;

    if v_saltandsuffix_text is not null then
      if instr(upper(v_saltandsuffix_text) ,'BATCH 1') >0 then
          v_saltandsuffix_text :='';
      end if;
      v_output_value := SUBSTR(v_root_regnum||v_saltandsuffix_text,1 ,50);
    end if;

    if v_batch_prefix_value is not null then
     v_output_value := SUBSTR(v_batch_Prefix_value||v_prefix_delimiter||v_output_value ,1 ,50);
    end if;

   -- v_mixture_setting := upper(GetBatchPrefixEnabled);
    TraceWrite( act||'GenerateBatchRegNumber_ended', $$plsql_line,'end' );
    RETURN v_output_value;
  END;

  /*
  A catch-all 'insert' routine for records with structures.
  Here the 'set' is an update, not an insert (the core insert has already been performed).
  */
  PROCEDURE InsertData(
    ATableName IN CLOB
    , AXmlRows IN CLOB
    , AStructureValue IN CLOB
    , AStructureAggregationValue IN CLOB
    , AFragmentXmlValue IN CLOB
    , ANormalizedStructureValue IN CLOB
    , ACompoundID IN Number
    , AStructureID IN Number
    , AMixtureID IN Number
    , AMessage IN OUT NOCOPY CLOB
    , ARowsInserted IN OUT Number
  ) IS
    LinsCtx       DBMS_XMLSTORE.ctxType;
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'InsertData_started', $$plsql_line,'start' );
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
            SET STRUCTURE = AStructureValue
            WHERE STRUCTUREID = AStructureID;
          END IF;
        END;

      WHEN 'VW_MIXTURE' THEN
        BEGIN
          IF AStructureAggregationValue IS NOT NULL THEN
           $if CompoundRegistry.Debuging $then InsertLog('InsertData',' LMixtureID:'||AMixtureID||' LStructureAggregationValue:'||AStructureAggregationValue); $end null;
            UPDATE VW_MIXTURE
            SET StructureAggregation = AStructureAggregationValue
            WHERE MixtureID = AMixtureID;
          END IF;
        END;

      WHEN 'VW_FRAGMENT' THEN
        BEGIN
          IF UPPER(ATableName)='VW_FRAGMENT' AND AFragmentXmlValue IS NOT NULL THEN
            $if CompoundRegistry.Debuging $then InsertLog('InsertData',' LStructureID:'||AStructureID||' LFragmentXmlValue:'||AFragmentXmlValue); $end null;
            UPDATE VW_STRUCTURE
            SET STRUCTURE = AFragmentXmlValue
            WHERE StructureID = AStructureID;
          END IF;
        END;

      WHEN 'VW_COMPOUND' THEN
        BEGIN
          IF ANormalizedStructureValue IS NOT NULL THEN
            $if CompoundRegistry.Debuging $then InsertLog('InsertData',' ACompoundID:'||ACompoundID||' ANormalizedStructureValue:'||ANormalizedStructureValue); $end null;
            UPDATE VW_COMPOUND
            SET NORMALIZEDSTRUCTURE = ANormalizedStructureValue
            WHERE COMPOUNDID = ACompoundID;
          END IF;
        END;

      ELSE NULL;
    END CASE;
    TraceWrite( act||'InsertData_ended', $$plsql_line,'end' );
  END;

  /**
  Deletes a structure from the

  */
  PROCEDURE DeleteStructure(AStrcutureIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
    LCountStructure Number;
    LMsg varchar2(255) := ' row(s) deleted on ';
--    LEnableRLS BOOLEAN;
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'DeleteStructure_started', $$plsql_line,'start' );
    -- all structures have positive StructureID
    IF NVL(AStrcutureIDToDelete,0) > 0 THEN
--        LEnableRLS := RegistrationRLS.GEnableRLS;
        IF LEnableRLS THEN
          RegistrationRLS.GEnableRLS := False;
        END IF;
      SELECT  NVL(Count(1),0)
      INTO  LCountStructure
      FROM  VW_Compound
      WHERE StructureID=AStrcutureIDToDelete;
        RegistrationRLS.GEnableRLS:=LEnableRLS;

      -- Don't delete structures still in use by a compound (delete the compound first)
      IF LCountStructure = 0 THEN
        -- eliminate the child rows first
        DELETE VW_Structure_Identifier si WHERE si.StructureID = AStrcutureIDToDelete;
        AMessage := AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || LMsg || '"VW_Structure_Identifier".';

        -- then eliminate the parent row
        DELETE VW_Structure s WHERE s.structureid = AStrcutureIDToDelete;
        AMessage := AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || LMsg || '"VW_Structure".';
      END IF;
    END IF;
    TraceWrite( act||'DeleteStructure_ended', $$plsql_line,'end' );
  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        IF LEnableRLS IS NOT NULL THEN
            RegistrationRLS.GEnableRLS:=LEnableRLS;
        END IF;
        TraceWrite( act||'DeleteStructure_ended', $$plsql_line,'end' );
    END;
  END;

  PROCEDURE DeleteStructureIdentifier(AStructureIdentifierIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'DeleteStructureIdentifier_started', $$plsql_line,'start' );
    DELETE VW_Structure_Identifier WHERE ID = AStructureIdentifierIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Structure_Identifier".';
    TraceWrite( act||'DeleteStructureIdentifier_ended', $$plsql_line,'end' );
  END;

  PROCEDURE DeleteCompound(ACompoundIDToDelete IN Number, AMixtureID IN Number,AMessage IN OUT NOCOPY Varchar2) IS
    LCountMixture Number;
    LCountStructure Number;
    LStructureID Number;
    LRegID Number;
    LMsg varchar2(255) := ' row(s) deleted on ';
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'DeleteCompound_started', $$plsql_line,'start' );
    DELETE VW_BatchComponentFragment WHERE BatchComponentID IN (SELECT ID FROM VW_BatchComponent WHERE MixtureComponentID IN (SELECT MixtureComponentID FROM VW_Mixture_Component WHERE CompoundID=ACompoundIDToDelete and MixtureID=AMixtureID));
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || LMsg || '"VW_BatchComponentFragment".';

    DELETE VW_BatchComponent WHERE MixtureComponentID IN (SELECT MixtureComponentID FROM VW_Mixture_Component WHERE CompoundID=ACompoundIDToDelete and MixtureID=AMixtureID);
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || LMsg || '"VW_BatchComponent".';

    --Delete the compound
    SELECT  NVL(Count(1),0)  --take into account that DELETE VW_Mixture_Component was just done
    INTO  LCountMixture
    FROM  VW_Mixture_Component
    WHERE CompoundID=ACompoundIDToDelete AND MixtureID!=AMixtureID;

    IF LCountMixture=0 THEN
      SELECT RegID
      INTO LRegID
      FROM VW_Compound
      WHERE CompoundID=ACompoundIDToDelete;

        DELETE VW_Compound_Identifier WHERE RegID = LRegID;
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound_Identifier".';

        DELETE VW_Duplicates WHERE RegNumber IN (SELECT RN.RegNumber FROM VW_Compound C,VW_RegistryNumber RN WHERE C.CompoundID=ACompoundIDToDelete AND RN.RegID=C.RegID);
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Duplicates".';

        DELETE VW_Duplicates WHERE RegNumberDuplicated IN (SELECT RN.RegNumber FROM VW_Compound C,VW_RegistryNumber RN WHERE C.CompoundID=ACompoundIDToDelete AND RN.RegID=C.RegID);
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Duplicates".';

        DELETE VW_RegistryNumber WHERE RegID = LRegID;
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_RegistryNumber".';

        DELETE VW_Compound_Fragment WHERE CompoundID=ACompoundIDToDelete;
        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound_Fragment".';

        -- We have to be careful to AVOID DELETING the internal-use structures (-1, -2 and -3)
        BEGIN
          SELECT StructureID
          INTO LStructureID
          FROM VW_Compound
          WHERE CompoundID = ACompoundIDToDelete;
        EXCEPTION
          WHEN NO_DATA_FOUND THEN
            LStructureID := 0;
        END;

        DELETE VW_Mixture_Component WHERE MixtureID=AMixtureID AND CompoundID=ACompoundIDToDelete;
        AMessage := AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Mixture_Component".';

        DELETE VW_Compound WHERE CompoundID=ACompoundIDToDelete;
        AMessage := AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound".';

        IF LStructureID > 0 THEN
          DeleteStructure(LStructureID, AMessage);
        END IF;
    ELSE
      DELETE VW_Mixture_Component WHERE MixtureID=AMixtureID AND CompoundID=ACompoundIDToDelete;
      AMessage := AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Mixture_Component".';
    END IF;
    TraceWrite( act||'DeleteCompound_ended', $$plsql_line,'end' );
  END;

  PROCEDURE DeleteFragment(ACompoundfragmentIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'DeleteFragment_started', $$plsql_line,'start' );
    DELETE VW_BatchComponentFragment WHERE CompoundFragmentID=ACompoundfragmentIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_BatchComponentFragment".';
    DELETE VW_Compound_Fragment WHERE ID=ACompoundfragmentIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound_Fragment".';
    TraceWrite( act||'DeleteFragment_ended', $$plsql_line,'end' );
  END;

  PROCEDURE DeleteIdentifier(ACompoundIdentifierIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'DeleteIdentifier_started', $$plsql_line,'start' );
    DELETE VW_Compound_Identifier WHERE ID=ACompoundIdentifierIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound_Identifier".';
    TraceWrite( act||'DeleteIdentifier_ended', $$plsql_line,'end' );
  END;

  PROCEDURE DeleteRegistryNumberProject(ARegistryProjectIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'DeleteRegistryNumberProject_started', $$plsql_line,'start' );
    DELETE VW_RegistryNumber_Project WHERE ID=ARegistryProjectIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_RegistryNumber_Project".';
    TraceWrite( act||'DeleteRegistryNumberProject_ended', $$plsql_line,'end' );
  END;

  PROCEDURE DeleteBatchProject(ABatchProjectIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'DeleteBatchProject_started', $$plsql_line,'start' );
    DELETE VW_Batch_Project WHERE ID=ABatchProjectIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Batch_Project".';
    TraceWrite( act||'DeleteBatchProject_ended', $$plsql_line,'end' );
  END;

  PROCEDURE DeleteBatchIdentifier(ABatchIdentifierIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
  mod1 varchar2(100);
  act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'DeleteBatchIdentifier_started', $$plsql_line,'start' );
    DELETE VW_BatchIdentifier WHERE ID=ABatchIdentifierIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_BatchIdentifier".';
    TraceWrite( act||'DeleteBatchIdentifier_ended', $$plsql_line,'end' );
  END;

  PROCEDURE DeleteBatchComponentFragment(ABatchCompFragIDToDelete IN Number, AMessage IN OUT NOCOPY Varchar2) IS
    LCompoundID          VW_Compound.CompoundID%Type;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'DeleteBatchComponentFragment_started', $$plsql_line,'start' );
-- We have to be careful to AVOID no data found error in case of SBI True
    BEGIN
      SELECT CF.CompoundID
        INTO LCompoundID
        FROM  VW_BatchComponentFragment BCF, VW_Compound_Fragment CF
        WHERE BCF.ID=ABatchCompFragIDToDelete AND BCF.CompOundFragmentID=CF.ID;
    EXCEPTION
      WHEN NO_DATA_FOUND THEN
        LCompoundID := 0;
    END;

    DELETE VW_BatchComponentFragment WHERE ID=ABatchCompFragIDToDelete;
    AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_BatchComponentFragment".';

    IF LCompoundID > 0 THEN
        DELETE VW_Compound_Fragment
            WHERE CompoundID=LCompoundID AND
                  ID NOT IN (SELECT CompoundFragmentID
                                FROM VW_BatchComponentFragment BCF, VW_Compound_Fragment CF
                                WHERE BCF.CompOundFragmentID=CF.ID AND CF.CompoundID=LCompoundID  );

        AMessage:=AMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || ' Row/s Deleted on "VW_Compound_Fragment".';
    END IF;
    TraceWrite( act||'DeleteBatchComponentFragment_ended', $$plsql_line,'end' );
  END;

  /*
  Autor: Fari
  Object: Validate Duplicated Mixture.
  Description: The ValidateMixture function validates if there are others Registries with the Compounds same.
               If there are others Registries with the Compounds same then it returns a list with the RegNumbers duplicates for
               than the application shows the regnumbers to the users. ValidateMixture also calls to ValidateCompoundFragment.
               This function identifies if the compounds have the same fragments and if the fragments are equivalent.
  */
  FUNCTION ValidateMixture(
    ARegIDs IN CLOB
    , ADuplicateCount OUT Number
    , AMixtureID IN Varchar2 := '0'
    , ACompoundIdsValueDeleting IN Varchar2 := NULL
    , AXmlTables XmlType
  ) RETURN CLOB IS
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

  mod1 varchar2(100); act varchar2(100);
  BEGIN
   dbms_application_info.read_module(mod1, act); TraceWrite( act||'ValidateMixture_started', $$plsql_line,'start' );

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
          WHERE  R.RegID=M.RegID AND M.MixtureID=MC.MixtureID AND C.CompoundID=MC.CompoundID AND '||'M.MixtureID<>'||AMixtureID;
    IF ARegIDs IS NOT NULL THEN
        LQuery:=LQuery||' AND C.RegID IN ('||ARegIDs||') ';
    ELSE
        LQuery:=LQuery||' AND 1=0';
    END IF;
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
    TraceWrite( act||'ValidateMixture_ended', $$plsql_line,'end' );
    RETURN LResult;
  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        IF C_RegNumbers%ISOPEN THEN
            CLOSE C_RegNumbers;
        END IF;
        RAISE_APPLICATION_ERROR(eCompoundValidation, AppendError('Error validating the mixture.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
  END;

  /**
  This method throws an exception if the 'SameBatchesIdentity' system setting is set to
  'true' but batches have different fragments for each component.
  There are three flaws here:
  (1) Each fragments equivalents should also be equal, not just the FragmentID
  (2) If the fragment list for any given component is simply in a different order
      from batch to batch, this will fail to detect that.
  (3) We're going to the database to do work that should have been done by the caller.
  */
  PROCEDURE ValidateIdentityBetweenBatches(AXmlTables IN XmlType) IS
    LCIndex Number := 0;
    LComponentIndex Number;
    LBIndex Number := 0;
    LFragmentsIdsValue Varchar2(4000);
    LFragmentsIdsValueLast Varchar2(4000);
  mod1 varchar2(100); act varchar2(100);
  BEGIN
   dbms_application_info.read_module(mod1, act); TraceWrite( act||'ValidateIdentityBetweenBatches_started', $$plsql_line,'start' );
    IF VSameBatchesIdentity='True' THEN
            TraceWrite( 'ValidateIdentityBetweenBatches_loop0', $$plsql_line,AXmlTables.getclobval() );
      LOOP
        LCIndex := LCIndex + 1;
            TraceWrite( 'ValidateIdentityBetweenBatches_loop1', $$plsql_line,to_char(LCIndex) );

        SELECT extractValue(
          AXmlTables
          ,'/MultiCompoundRegistryRecord/ComponentList/Component[' || LCIndex || ']/ComponentIndex'
        ) INTO LComponentIndex FROM dual;

            TraceWrite( 'ValidateIdentityBetweenBatches_loop2', $$plsql_line,to_char(LComponentIndex) );
        LFragmentsIdsValueLast := NULL;
      EXIT WHEN LComponentIndex IS NULL;
        LOOP
          LBIndex := LBIndex + 1;
        EXIT WHEN AXmlTables.ExistsNode(
          '/MultiCompoundRegistryRecord/BatchList/Batch[' || LBIndex || ']/BatchComponentList/BatchComponent[ComponentIndex='||LComponentIndex||']'
        ) = 0; --LFragmentsIdsValue IS NULL

          SELECT XmlTransform(
            extract(AXmlTables,'/MultiCompoundRegistryRecord/BatchList/Batch[' || LBIndex || ']/BatchComponentList/BatchComponent[ComponentIndex='||LComponentIndex||']/BatchComponentFragmentList/BatchComponentFragment/FragmentID')
            , XmlType.CreateXml('
              <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
                <xsl:template match="/FragmentID">
                  <xsl:for-each select="."><xsl:value-of select="."/>,</xsl:for-each>
                </xsl:template>
              </xsl:stylesheet>')
            ).GetClobVal() INTO LFragmentsIdsValue FROM dual;

            TraceWrite( 'ValidateIdentityBetweenBatches_loop3', $$plsql_line,to_char(LFragmentsIdsValue) );
            select wm_concat(FragmentIdsSorted) into LFragmentsIdsValue from
            (select regexp_substr(LFragmentsIdsValue,'[^,]+', 1, level) as FragmentIdsSorted from dual
            connect by regexp_substr(LFragmentsIdsValue, '[^,]+', 1, level) is not null  order by FragmentIdsSorted);

            IF LBIndex > 1 THEN
              IF NVL(LFragmentsIdsValueLast, 0) <> NVL(LFragmentsIdsValue, 0) THEN
                RAISE_APPLICATION_ERROR(
                  eSameIdentityBetweenBatches,
                  AppendError('Error validating the compound '||ABS(LComponentIndex)||'. The comopund should have the same identity of fragments between batches. (The "SameBatchesIdentity" flag is set in "true")'));
              END IF;
            END IF;

            LFragmentsIdsValueLast := LFragmentsIdsValue;
        END LOOP;
      END LOOP;
    END IF;
    TraceWrite( act||'ValidateIdentityBetweenBatches_ended', $$plsql_line,'end' );
  END;

  FUNCTION ValidateFragment(
    ARegIDs IN CLOB
    , ADuplicateCount OUT Number
    , AMixtureID IN Varchar2 := '0'
    , ACompoundIdsValueDeleting IN Varchar2 := NULL
  ) RETURN CLOB IS
        LqryCtx DBMS_XMLGEN.ctxHandle;
        LResult CLOB;
        LComponentCount NUMBER;
        LPos NUMBER;
        LPosOld NUMBER;
        LQuery Varchar2(3000);
  mod1 varchar2(100); act varchar2(100);
  BEGIN
  dbms_application_info.read_module(mod1, act); TraceWrite( act||'ValidateFragment_started', $$plsql_line,'start' );

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
    TraceWrite( act||'ValidateFragment_ended', $$plsql_line,'end' );
    IF ADuplicateCount>0 THEN
        LResult:=replace(LResult,cXmlDecoration,'');
        LResult:=replace(LResult,'ROWSET','REGISTRYLIST');
        RETURN LResult;
    ELSE
        RETURN '';
    END IF;

  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        RAISE_APPLICATION_ERROR(eCompoundValidation, AppendError('Error validating the fragment.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
    TraceWrite( act||'ValidateFragment_ended', $$plsql_line,'end' );
    RETURN '';
  END;

  /*
  Why do this? If the caller specifies -1, -2 or -3, simply use that StructureID value
  and ignore the corresponding structure value.
  */
  FUNCTION  ValidateWildcardStructure(AStructureValue CLOB) RETURN Boolean IS
    PRAGMA AUTONOMOUS_TRANSACTION;
    LInternalID Integer;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'ValidateWildcardStructure_started', $$plsql_line,'start' );
    IF (AStructureValue IS NULL) THEN
      RETURN True;
    END IF;

    INSERT INTO CsCartridge.TempQueries (Query, Id) VALUES (AStructureValue, 0);

    SELECT CPD_INTERNAL_ID INTO LInternalID
    FROM structures
    WHERE cscartridge.containscdx.molcontains(
      BASE64_CDX
      , 'SELECT Query FROM CSCartridge.TempQueries WHERE ID = 0'
      , ''
      , 'IDENTITY=YES'
    ) = 1 AND Drawing_Type = 1;

    COMMIT;
      TraceWrite( act||'ValidateWildcardStructure_ended', $$plsql_line,'end' );
    RETURN (LInternalID <> -1);

  EXCEPTION
    WHEN NO_DATA_FOUND THEN
      BEGIN
        COMMIT;
        TraceWrite( act||'ValidateWildcardStructure_ended', $$plsql_line,'end' );
        RETURN True;
      END;
    WHEN OTHERS THEN
      BEGIN
        COMMIT;
        RAISE_APPLICATION_ERROR(eWildcardValidation, AppendError('Error validating Wildcard Structure.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
      END;
  END;

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
  PROCEDURE CanCreateMultiCompoundRegistry(
    AXml IN CLOB
    , AMessage OUT CLOB
    , ADuplicateCheck Char := 'C'
    , AConfigurationID Number := 1
  ) AS
    LRegNumber VW_RegistryNumber.RegNumber%type;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
  dbms_application_info.read_module(mod1, act); TraceWrite( act||'CanCreateMultiCompoundRegistry_started', $$plsql_line,'start' );
    IF Upper(ADuplicateCheck) = 'C' THEN
      CreateMultiCompoundRegistry(AXml, LRegNumber, AMessage, 'V');
    ELSIF Upper(ADuplicateCheck) = 'M' THEN
      CreateMultiCompoundRegistry(AXml, LRegNumber, AMessage, 'L');
    END IF;
    TraceWrite( act||'CanCreateMultiCompoundRegistry_ended', $$plsql_line,'end' );
  END;

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
  PROCEDURE CreateMultiCompoundRegistry(
    AXml IN CLOB
    , ARegNumber OUT NOCOPY VW_RegistryNumber.RegNumber%type
    , AMessage OUT CLOB
    , ADuplicateCheck Char := 'C'
    , ARegNumGeneration IN CHAR := 'Y'
    , AConfigurationID Number := 1
    , ASectionsList IN Varchar2 := NULL
    , ASetBatchNumber IN Number := 1
  ) IS
    LXmlTables                XmlType;
    LXslTablesTransformed     XmlType;
    LXmlCompReg               CLOB;
    LXmlRows                  CLOB;
    LXmlTypeRows              XmlType;
    LXmlSequenceType          XmlSequenceType;

    LIndex                    Number:=0;
    LRowsInserted             Number:=0;
    LTableName                CLOB;
    LBriefMessage             CLOB;
    LMessage                  CLOB:='';

    LStructureValue           CLOB;
    LRegNumberRegID           Number:=0;
    LDuplicatedCompoundID     Number;
    LDuplicatedStructures     CLOB;
    LListDulicatesCompound    CLOB;
    LDuplicateComponentCount  Number:=0;

    LStructuresList            CLOB;
    LStructuresToValidateList  CLOB;
    LFragmentXmlValue          CLOB;
    LNormalizedStructureList   CLOB;
    LNormalizedStructureValue  CLOB;
    LStructureAggregationList  CLOB;
    LStructureAggregationValue CLOB;
    LXMLRegistryRecord         CLOB;

    LXMLCompound                  XmlType;
    LXMLFragmentEquivalent        XmlType;
    LXMLRegNumberDuplicated       XmlType;

    LRegDBIdsValue         Varchar2(4000);
    LDuplicatedMixtureCount   Number;
    LDuplicatedMixtureRegIds  Varchar2(4000);
    LDuplicatedAuxStructureID Number:=0;

    LRegID                      Number:=0;
    LNewRegID                   Number:=0;
    LBatchID                    Number:=0;
    LCompoundID                 Number:=0;
    LFragmentID                 Number:=0;
    LStructureID                Number:=0;
    LMixtureID                  Number:=0;
    LBatchNumber                Number:=0;
    LMixtureComponentID         Number:=0;
    LBatchComponentID           Number:=0;
    LCompoundFragmentID         Number:=0;
    LCreateNewStructure        Boolean:=false;
    LRegNumber                  VW_REGISTRYNUMBER.RegNumber%Type;
    LSequenceNumber             VW_REGISTRYNUMBER.SequenceNumber%Type;
    LFullRegNumber              VW_BATCH.FullRegNumber%Type;

    LTempID                        VW_TEMPORARYBATCH.TEMPBATCHID%TYPE;

    LSequenceID                   Number:=0;
    LProcessingMixture            Varchar2(1);

    LRegIDAux                      Number:=0;
    LExistentComponentIndex        Number:=0;

    LXslTables XmlType := XslMcrrCreate;
    LNewBatchList tNumericIdList;
     mod1 varchar2(100);
     act varchar2(100);

    /** Verifies that pre-existing registration numbers match a supported format. */
    PROCEDURE ValidateRegNumber(
      ARegNumber VW_RegistryNumber.RegNumber%type
      , ASequenceID VW_SEQUENCE.SequenceId%Type
      , AProcessingMixture IN VARCHAR2
      , ASequenceNumber IN OUT VW_REGISTRYNUMBER.SequenceNumber%Type
    ) IS
      LRegNumber        VW_RegistryNumber.RegNumber%type;
      LSequenceNumber   VW_REGISTRYNUMBER.SequenceNumber%Type;
      LRegNumberLength  VW_SEQUENCE.RegNumberLength%Type;
    mod1 varchar2(100); act varchar2(100);
    BEGIN
      dbms_application_info.read_module(mod1, act); TraceWrite( act||'ValidateRegNumber_started', $$plsql_line,'start' );
      SELECT RegNumberLength
      INTO LRegNumberLength
      FROM VW_SEQUENCE
      WHERE SequenceID=ASequenceID;

      IF AProcessingMixture='Y' THEN
        LRegNumber := GetRegNumber(ASequenceID,LSequenceNumber,NULL,'N');
        IF SUBSTR(LRegNumber,1,LENGTH(LRegNumber)-LRegNumberLength)<>NVL(SUBSTR(ARegNumber,1,LENGTH(ARegNumber)-LRegNumberLength),' ') THEN
          RAISE_APPLICATION_ERROR(eCompoundValidation,
            AppendError('Registry Number is incorrect: "'||ARegNumber||'"'));
        END IF;
        IF NVL(ASequenceNumber,0) = 0 THEN
          ASequenceNumber := SUBSTR(ARegNumber,-LRegNumberLength,LRegNumberLength);
        END IF;
      END IF;
    TraceWrite( act||'ValidateRegNumber_ended', $$plsql_line,'end' );
    EXCEPTION
        WHEN OTHERS THEN
        BEGIN
          TraceWrite('CreateMXRR.ValidateRegNumber', $$plsql_line, DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
          RAISE_APPLICATION_ERROR(eRegNumberValidation, AppendError('Error validating the Registry Number.', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
        END;
    END;


  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'CreateMultiCompoundRegistry_started', $$plsql_line,'start' );
    SetSessionParameter;

    if AXml is null then
        TraceWrite('CreateMultiCompoundRegistry', $$plsql_line, 'Return because AXml is null');
        return;
    end if;

    LXmlCompReg := AXml;
    TraceWrite('CreateMXRR_0_InputXml', $$plsql_line, AXml);

    -- Take Out the Structures because XmlType don't suport > 64k.
    LStructuresList := TakeOffAndGetClobsList(LXmlCompReg,'<Structure ','Structure','<BaseFragment>',NULL,FALSE);
    LStructuresToValidateList := LStructuresList;
    LNormalizedStructureList := TakeOffAndGetClobslist(LXmlCompReg,'<NormalizedStructure','<Structure>');
    LStructureAggregationList := TakeOffAndGetClobsList(LXmlCompReg,'<StructureAggregation');

    -- Convert the remaining string to XMLTYPE
    LXmlTables := XmlType.createXML(LXmlCompReg);
    TraceWrite('CreateMXRR_1_StructureStrippedXml', $$plsql_line, LXmlTables.GetClobVal());

    -- Test the SameBatchesIdentity setting
    -- !! throws exception if it fails
    ValidateIdentityBetweenBatches(LXmlTables);
    TraceWrite('CreateMXRR_2_SameBatchesIdentityPass', $$plsql_line, null);

    SELECT EXTRACTVALUE(LXmlTables, '/MultiCompoundRegistryRecord/ID') into LTempID from dual;

    -- Perform duplicate-checking
    IF UPPER(GetDuplicateCheckEnable)='TRUE' THEN
      TraceWrite('CreateMXRR_DuplicateCheckSetting=TRUE', $$plsql_line, null);

      -- First at the compound level
      IF Upper(ADuplicateCheck)='C' OR Upper(ADuplicateCheck)='V' THEN
        -- Get the XML for each component
        LOOP
          LIndex := LIndex + 1;
          SELECT extract(
            LXmlTables
            , '/MultiCompoundRegistryRecord/ComponentList/Component[' || LIndex || ']'
          ) INTO LXMLCompound FROM dual;
        EXIT WHEN LXMLCompound IS NULL;
          SELECT
            extractValue(LXMLCompound, '/Component/Compound/RegNumber/RegID')
            , extractValue(LXMLCompound,'/Component/Compound/BaseFragment/Structure/StructureID')
          INTO LRegNumberRegID, LDuplicatedAuxStructureID FROM dual;

          -- Extract the compound's structure value
          LStructureValue := TakeOffAndGetClob(LStructuresToValidateList, 'Clob');

          -- Bypass matching for wildcard (ID < 0) and new (ID = 0) structures
          IF NVL(LRegNumberRegID, 0) = 0 AND NVL(LDuplicatedAuxStructureID, 0) >= 0 THEN
            SELECT
              extractValue(LXMLCompound, '/Component/Compound/CompoundID')
              , extractValue(LXMLCompound, '/Component/ComponentIndex')
            INTO LDuplicatedCompoundID, LExistentComponentIndex FROM dual;

            -- Get the XML for the compound's BatchComponentFragmentList
            IF LDuplicatedCompoundID IS NOT NULL THEN
              SELECT extract(
                LXmlTables
                ,'/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex='||LExistentComponentIndex||']/BatchComponentFragmentList'
              ) INTO LXMLFragmentEquivalent FROM dual;

              -- Perform 'compound' (structure AND fragments) duplicate-checking
              --  "Is there already a compound with this structure and these associated fragments?"
              LDuplicatedStructures := ValidateCompoundMulti(
                LStructureValue, NULL, AConfigurationID, LXMLCompound, LXMLFragmentEquivalent
              );

              TraceWrite('CreateMXRR_AFTER_ValidateCompoundMulti', $$plsql_line, LDuplicatedStructures);

              IF LDuplicatedStructures IS NOT NULL
                AND LDuplicatedStructures <> '<REGISTRYLIST></REGISTRYLIST>' THEN

                LListDulicatesCompound := LListDulicatesCompound||'<COMPOUND>'||'<TEMPCOMPOUNDID>'||LDuplicatedCompoundID||'</TEMPCOMPOUNDID>'||LDuplicatedStructures||'</COMPOUND>';
                LDuplicateComponentCount := LDuplicateComponentCount + 1;
              END IF;

            END IF;
          END IF;
        END LOOP;

        -- If compound-matches were found, create a response-XML and return
        IF LListDulicatesCompound IS NOT NULL THEN
          LListDulicatesCompound := '<COMPOUNDLIST>' || LListDulicatesCompound || '</COMPOUNDLIST>';
          IF (LDuplicateComponentCount = 1) THEN
            AMessage := CreateRegistrationResponse('1 duplicated component.', LListDulicatesCompound, NULL);
          ELSE
            AMessage := CreateRegistrationResponse(to_char(LDuplicateComponentCount) || ' duplicated components.', LListDulicatesCompound, NULL);
          END IF;
          RETURN;
        END IF;
      END IF;

      -- Second at the mixture level
      IF (Upper(ADuplicateCheck)='M') OR (Upper(ADuplicateCheck)='C') OR (Upper(ADuplicateCheck)='V') OR (Upper(ADuplicateCheck)='L')THEN
        SELECT XmlTransform(
          extract(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component/Compound/RegNumber/RegID')
          ,XmlType.CreateXml(
            '<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
            <xsl:template match="/RegID">
            <xsl:for-each select=".">
            <xsl:value-of select="."/>,</xsl:for-each>
            </xsl:template>
            </xsl:stylesheet>')
        ).GetClobVal() INTO LRegDBIdsValue FROM dual;
        LRegDBIdsValue := SUBSTR(LRegDBIdsValue,1,Length(LRegDBIdsValue)-1);

        -- Perform 'mixture' (all compounds) duplicate-checking
        --  "Is there already a mixture with these same components?"
        LDuplicatedMixtureRegIds := ValidateMixture(
          LRegDBIdsValue, LDuplicatedMixtureCount, '0', null, LXmlTables
        );

        -- If mixture-matches were found, create a response-XML and return
        IF LDuplicatedMixtureRegIds IS NOT NULL THEN
          IF (LDuplicatedMixtureCount = 1) THEN
            AMessage := CreateRegistrationResponse('1 duplicated mixture.', LDuplicatedMixtureRegIds, NULL);
          ELSE
            AMessage := CreateRegistrationResponse(to_char( LDuplicatedMixtureCount ) || ' duplicated mixtures.', LDuplicatedMixtureRegIds, NULL);
          END IF;
          RETURN;
        END IF;

      END IF;

    END IF;

    -- What do these mean? (I've never heard of them). -- jed
    IF (Upper(ADuplicateCheck)='V') OR (Upper(ADuplicateCheck)='L') THEN
      RETURN;
    END IF;

    LBriefMessage := 'Compound Validation OK';
    LMessage := LMessage || LBriefMessage ||CHR(13);

    -- Transform the raw xml input into an Oracle DB-friendly format.
    LXslTablesTransformed := LXmlTables.Transform(LXslTables);

    TraceWrite('CreateMXRR_3_TransformedXml', $$plsql_line, LXslTablesTransformed.getClobVal());

    -- Look over Xml searching each Table and insert the rows of it.
    SELECT XMLSequence(LXslTablesTransformed.Extract('/node()'))
    INTO LXmlSequenceType
    FROM DUAL;

    LStructureValue := null;
    LProcessingMixture := 'Y';

    FOR LIndex IN LXmlSequenceType.FIRST..LXmlSequenceType.LAST LOOP
      --Search each Table
      LXmlTypeRows := LXmlSequenceType(LIndex);
      LTableName := LXmlTypeRows.GetRootElement();

      --Build Message Logs
      LMessage := LMessage || chr(10) || 'Processing ' || LTableName || ': ';

      --Customization for each View - Use of Sequences
      CASE UPPER(LTableName)

        WHEN 'VW_STRUCTURE' THEN
          BEGIN
            LStructureID := LXmlTypeRows.extract('VW_Structure/ROW/STRUCTUREID/text()').getNumberVal();

            IF ( NVL(LStructureID, 0) <= 0 ) THEN --StructureID <=0 means a new structure, rather than existing structure from "Use Structure" duplicate option

              SELECT MOLID_SEQ.NEXTVAL INTO LStructureID FROM DUAL;
              LCreateNewStructure:=true;
              LStructureValue := TakeOffAndGetClob(LStructuresList, 'Clob');

              SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/STRUCTUREID/text()',LStructureID)
              INTO LXmlTypeRows FROM dual;

              LXmlRows := LXmlTypeRows.getClobVal();
              TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
              InsertData(
                LTableName, LXmlRows, LStructureValue
                , LStructureAggregationValue, LFragmentXmlValue, LNormalizedStructureValue
                , LCompoundID, LStructureID, LMixtureID, LMessage, LRowsInserted
              );

              IF Upper(ADuplicateCheck)='N' OR ( NOT (Upper(ADuplicateCheck)='N') AND RegistrationRLS.GetStateRLS ) THEN
                IF NVL(LRegIDAux,0) = 0 THEN
                  IF ValidateWildcardStructure(LStructureValue) THEN
                    VerifyAndAddDuplicateToSave(LRegNumber,LStructureValue, NULL,LXMLRegNumberDuplicated);
                  END IF;
                END IF;
              END IF;
            ELSE
                LStructureValue := TakeOffAndGetClob(LStructuresList, 'Clob');
            END IF;
          END;

        WHEN 'VW_STRUCTURE_IDENTIFIER' THEN
          BEGIN
            IF LCreateNewStructure THEN --create new structure identifiers only if we are creating a new STRUCTURE
                SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/STRUCTUREID/text()', LStructureID)
                INTO LXmlTypeRows FROM dual;

                LXmlRows := LXmlTypeRows.getClobVal();
                TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
                InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
            ELSE -- "Use Component" or "Use Structure"
                NULL;
            END IF;
          END;

        WHEN 'VW_REGISTRYNUMBER' THEN
          BEGIN
            IF LProcessingMixture='Y' THEN
              LRegID := LXmlTypeRows.extract('VW_RegistryNumber/ROW/REGID/text()').getNumberVal();

              IF (LRegID = 0) THEN
                SELECT SEQ_REG_NUMBERS.NEXTVAL INTO LRegID FROM DUAL;

                IF ARegNumGeneration = 'Y' THEN
                  LRegNumber:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/REGNUMBER/text()').GetStringVal();

                  LSequenceID:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/SEQUENCEID/text()').getNumberVal();
                  IF LRegNumber='null' THEN
                    IF LSequenceID IS NOT NULL THEN
                      LRegNumber:=GetRegNumber(LSequenceID,LSequenceNumber,LXmlTables);
                    END IF;
                  ELSE
                    LSequenceNumber := LXmlTypeRows.extract('VW_RegistryNumber/ROW/SEQUENCENUMBER/text()').GetStringVal();
                    --JED ValidateRegNumber(LRegNumber,LSequenceID,LProcessingMixture,LSequenceNumber);
                  END IF;

                  SELECT UpdateXML(
                    LXmlTypeRows
                    , '/node()/ROW/REGID/text()', LRegID
                    , '/node()/ROW/REGNUMBER/text()', LRegNumber
                    , '/node()/ROW/SEQUENCENUMBER/text()', LSequenceNumber
                    , '/node()/ROW/DATECREATED/text()', TO_CHAR(SYSDATE)
                  ) INTO LXmlTypeRows FROM dual;

                  IF ARegNumber IS NULL THEN  --The first regid
                    ARegNumber := LRegNumber;
                  END IF;

                ELSE
                  LRegNumber := LXmlTypeRows.extract('VW_RegistryNumber/ROW/REGNUMBER/text()').GetStringVal();
                  LSequenceID := LXmlTypeRows.extract('VW_RegistryNumber/ROW/SEQUENCEID/text()').getNumberVal();
                  --JED ValidateRegNumber(LRegNumber,LSequenceID,LProcessingMixture,LSequenceNumber);

                  SELECT UpdateXML(
                    LXmlTypeRows
                    , '/node()/ROW/REGID/text()', LRegID
                  ) INTO LXmlTypeRows FROM dual;

                  IF ARegNumber IS NULL THEN  --The first regid
                    ARegNumber:=LRegNumber;
                  END IF;
                END IF;

                LXmlRows := LXmlTypeRows.getClobVal();
                TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
                InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
              END IF;
            ELSE

              LRegIDAux:= LXmlTypeRows.extract('VW_RegistryNumber/ROW/REGID/text()').getNumberVal();

              IF LRegIDAux=0 THEN
                SELECT SEQ_REG_NUMBERS.NEXTVAL INTO LRegID FROM DUAL;
                LSequenceID := LXmlTypeRows.extract('VW_RegistryNumber/ROW/SEQUENCEID/text()').getNumberVal();

                IF LSequenceID IS NOT NULL THEN
                  LRegNumber := GetRegNumber(LSequenceID,LSequenceNumber,LXmlTables);
                END IF;

                SELECT UpdateXML(LXmlTypeRows
                  , '/node()/ROW/REGID/text()', LRegID
                  , '/node()/ROW/REGNUMBER/text()', LRegNumber
                  , '/node()/ROW/SEQUENCENUMBER/text()', LSequenceNumber
                  , '/node()/ROW/DATECREATED/text()', TO_CHAR(SYSDATE)
                ) INTO LXmlTypeRows FROM dual;

                IF ARegNumber IS NULL THEN  --The first regid
                  ARegNumber:=LRegNumber;
                END IF;

                LXmlRows:=LXmlTypeRows.getClobVal();
                TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
                InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
              ELSE
                SELECT CompoundID
                INTO LCompoundID
                FROM VW_Compound WHERE RegID = LRegIDAux;

                LStructureValue := TakeOffAndGetClob(LStructuresList,'Clob');
                LNormalizedStructureValue :=TakeOffAndGetClob(LNormalizedStructureList,'Clob');
              END IF;

            END IF;
          END;

        WHEN 'VW_COMPOUND_IDENTIFIER' THEN
          BEGIN
            SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/REGID/text()',LRegID)
            INTO LXmlTypeRows FROM dual;

            LXmlRows := LXmlTypeRows.getClobVal();
            TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
            InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
          END;

        WHEN 'VW_BATCH' THEN
          BEGIN
            SELECT SEQ_BATCHES.NEXTVAL INTO LBatchID FROM DUAL;

            SELECT decode(MAX(BatchNumber),null,ASetBatchNumber,MAX(BatchNumber)+1) INTO LBatchNumber
            FROM VW_Batch
            WHERE REGID=LRegID;

            -- new batches *should* require a FullRegNum unless it's from legacy data
            LFullRegNumber := LXmlTypeRows.extract('VW_Batch/ROW/FULLREGNUMBER/text()').GetStringVal();

            SELECT UpdateXML(
              LXmlTypeRows
              , '/node()/ROW/BATCHID/text()', LBatchID
              , '/node()/ROW/REGID/text()', LRegID
              , '/node()/ROW/BATCHNUMBER/text()', LBatchNumber
              , '/node()/ROW/FULLREGNUMBER/text()', LFullRegNumber
              , '/node()/ROW/DATECREATED/text()', TO_CHAR(SYSDATE)
              , '/node()/ROW/DATELASTMODIFIED/text()', TO_CHAR(SYSDATE)
            ) INTO LXmlTypeRows FROM dual;

            LXmlRows := LXmlTypeRows.getClobVal();
            TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
            InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);

            -- Add to the BatchID list for FullRegNum generation if we're NOT using a legacy value
            IF ( NVL(LFullRegNumber, 'null') ='null' )THEN
              LNewBatchList(LBatchNumber) := LBatchID;
            END IF;
          END;

        WHEN 'VW_BATCH_PROJECT' THEN
          BEGIN
            SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/BATCHID/text()', LBatchID)
            INTO LXmlTypeRows FROM dual;

            LXmlRows:=LXmlTypeRows.getClobVal();
            TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
            InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
          END;

        WHEN 'VW_BATCHIDENTIFIER' THEN
          BEGIN
            SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/BATCHID/text()', LBatchID)
            INTO LXmlTypeRows FROM dual;

            LXmlRows:=LXmlTypeRows.getClobVal();
            TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
            InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
          END;

        WHEN 'VW_REGISTRYNUMBER_PROJECT' THEN
          BEGIN
            SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/REGID/text()', LRegId)
            INTO LXmlTypeRows FROM dual;

            LXmlRows:=LXmlTypeRows.getClobVal();
            TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
            InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
          END;

        WHEN 'VW_COMPOUND' THEN
          BEGIN
            SELECT SEQ_COMPOUND_MOLECULE.NEXTVAL INTO LCompoundID FROM DUAL;
            SELECT UpdateXML(
              LXmlTypeRows
              , '/node()/ROW/COMPOUNDID/text()', LCompoundID
              , '/node()/ROW/REGID/text()', LRegID
              , '/node()/ROW/STRUCTUREID/text()', LStructureID
              , '/node()/ROW/DATECREATED/text()', TO_CHAR(SYSDATE)
              , '/node()/ROW/DATELASTMODIFIED/text()', TO_CHAR(SYSDATE)
            ) INTO LXmlTypeRows FROM dual;

            LNormalizedStructureValue :=TakeOffAndGetClob(LNormalizedStructureList,'Clob');

            LXmlRows:=LXmlTypeRows.getClobVal();
            TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
            InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
          END;

        WHEN 'VW_FRAGMENT' THEN
          BEGIN
            LFragmentID := LXmlTypeRows.extract('VW_Fragment/ROW/FRAGMENTID/text()').getNumberVal();
          END;

        WHEN 'VW_COMPOUND_FRAGMENT' THEN
          BEGIN
            SELECT Min(ID)
            INTO LCompoundFragmentID
            FROM VW_Compound_Fragment
            WHERE FragmentID=LFragmentID AND CompoundID=LCompoundID;

            IF NVL(LCompoundFragmentID,0)=0 THEN
              SELECT SEQ_COMPOUND_FRAGMENT.NEXTVAL INTO LCompoundFragmentID FROM DUAL;

              SELECT UpdateXML(
                LXmlTypeRows
                , '/node()/ROW/ID/text()', LCompoundFragmentID
                , '/node()/ROW/FRAGMENTID/text()', LFragmentID
                , '/node()/ROW/COMPOUNDID/text()', LCompoundID
              ) INTO LXmlTypeRows FROM dual;

              LXmlRows:=LXmlTypeRows.getClobVal();
              TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
              InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
            END IF;
          END;

        WHEN 'VW_MIXTURE' THEN
          BEGIN
            SELECT SEQ_MIXTURE.NEXTVAL INTO LMixtureID FROM DUAL;
            LStructureAggregationValue:=TakeOffAndGetClob(LStructureAggregationList,'Clob');

            SELECT UpdateXML(
              LXmlTypeRows
              , '/node()/ROW/MIXTUREID/text()', LMixtureID
              , '/node()/ROW/REGID/text()', LRegID
              , '/node()/ROW/CREATED/text()', TO_CHAR(SYSDATE)
              , '/node()/ROW/MODIFIED/text()', TO_CHAR(SYSDATE)
            ) INTO LXmlTypeRows FROM dual;

            LXmlRows:=LXmlTypeRows.getClobVal();
            TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
            InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);

            LProcessingMixture:='N';
          END;

        WHEN 'VW_MIXTURE_COMPONENT' THEN
          BEGIN
            SELECT SEQ_MIXTURE_COMPONENT.NEXTVAL INTO LMixtureComponentID FROM DUAL;
            SELECT UpdateXML(
              LXmlTypeRows
              , '/node()/ROW/MIXTURECOMPONENTID/text()', LMixtureComponentID
              , '/node()/ROW/MIXTUREID/text()', LMixtureID
              , '/node()/ROW/COMPOUNDID/text()', LCompoundID
            ) INTO LXmlTypeRows FROM dual;

            LXmlRows:=LXmlTypeRows.getClobVal();
            TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
            InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
          END;

        WHEN 'VW_BATCHCOMPONENT' THEN
          BEGIN
            SELECT SEQ_BATCHCOMPONENT.NEXTVAL INTO LBatchComponentID FROM DUAL;
            SELECT UpdateXML(
              LXmlTypeRows
              , '/node()/ROW/ID/text()', LBatchComponentID
              , '/node()/ROW/MIXTURECOMPONENTID/text()', LMixtureComponentID
              , '/node()/ROW/BATCHID/text()', LBatchId
            ) INTO LXmlTypeRows FROM dual;

            LXmlRows:=LXmlTypeRows.getClobVal();
            TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
            InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
          END;

        WHEN 'VW_BATCHCOMPONENTFRAGMENT' THEN
          BEGIN
            SELECT UpdateXML(
              LXmlTypeRows
              , '/node()/ROW/BATCHCOMPONENTID/text()', LBatchComponentID
              , '/node()/ROW/COMPOUNDFRAGMENTID/text()', LCompoundFragmentID
            ) INTO LXmlTypeRows FROM dual;

            LXmlRows:=LXmlTypeRows.getClobVal();
            TraceWrite('CreateMXRR_4_' || LTableName, $$plsql_line, LXmlRows);
            InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
          END;

        ELSE LMessage := LMessage || ' "' || LTableName || '" table unknown.' ||chr(13);

      END CASE;
    END LOOP;

    IF ARegNumber IS NOT NULL THEN

      -- Run the FullBatchNum generators!
      UpdateBatchRegNumbers(LNewBatchList);

      TraceWrite('CreateMXRR_5_RegNumberPre', $$plsql_line, ARegNumber);
      OnRegistrationInsert(ARegNumber);
      TraceWrite('CreateMXRR_6_RegNumberPost', $$plsql_line, ARegNumber);

      -- START PATCH
      -- Re-fetch the surrogate key value since the listener SPROCs may have modified it
      begin
        select rn.regnumber,rn.regid into ARegNumber,LNewRegID
        from vw_mixture m
          inner join vw_registrynumber rn on rn.regid = m.regid
        where m.mixtureid = LMixtureID;
      end;
      TraceWrite('CreateMXRR_5_RegNumber', $$plsql_line, ARegNumber);

      IF (LXMLRegNumberDuplicated is not null) THEN
        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'LXMLRegNumberDuplicated->'||LXMLRegNumberDuplicated.getClobVal()); $end NULL;
        IF LXslTablesTransformed.ExistsNode('VW_RegistryNumber[1]/ROW/PERSONREGISTERED/text()')=1 THEN
          SaveRegNumbersDuplicated(LXMLRegNumberDuplicated,LXslTablesTransformed.extract('VW_RegistryNumber[1]/ROW/PERSONREGISTERED/text()').getStringVal());
        ELSE
          SaveRegNumbersDuplicated(LXMLRegNumberDuplicated,NULL);
        END IF;
      END IF;

      IF (
        (ASectionsList is null or UPPER(ASectionsList) <> cSectionListEmpty)
        AND (ARegNumber is not null)
      ) THEN
        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'Before RetrieveMultiCompoundRegistry'); $end NULL;
        RetrieveMultiCompoundRegistry(ARegNumber, LXMLRegistryRecord);
        UpdateApprovedStatus(LTempID, cSubmittedStatus); -- Make the temporary record deletable
        $if CompoundRegistry.Debuging $then InsertLog('CreateMultiCompoundRegistry', 'LXMLRegistryRecord->'||LXMLRegistryRecord); $end NULL;
      END IF;
      -- END PATCH

      AMessage := CreateRegistrationResponse(LBriefMessage, NULL, LXMLRegistryRecord, LNewRegID, LBatchNumber);
    END IF;
    TraceWrite( act||'CreateMultiCompoundRegistry_ended', $$plsql_line,'end' );
  END;

  PROCEDURE AddNullFields(AFields IN CLOB, AXml IN OUT NOCOPY CLOB) IS
    LPosBegin NUMBER;
    LPoslast NUMBER;
    LField VARCHAR2(30);
    LFields CLOB;
    LPosField NUMBER;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'AddNullFields_started', $$plsql_line,'start' );
    LFields:=AFields||',';
    LPosBegin:=0;
    LPoslast:=1;

    LOOP
      LPosBegin:=INSTR(LFields,',',LPoslast);
      LField:=UPPER(SUBSTR(LFields,LPoslast,LPosBegin-LPoslast));
      LPoslast:=LPosBegin+1;
        EXIT WHEN LField IS NULL;
      LPosField:=INSTR(AXml, '<'||LField||'>');
      IF LPosField=0 THEN
        AXml:=REGEXP_REPLACE(AXml,'</ROW>',' <'||LField||'/>'||CHR(10)||' </ROW>');
      END IF;
    END LOOP;
    TraceWrite( act||'AddNullFields_ended', $$plsql_line,'end' );
  END;

  /**
  Fetches the raw xml for the 'mixture' level of a Chemical Registration.
  This includes the Property List, Project List, Identifier List and inherent properties.
  -->author jed
  -->since January, 2010
  -->param ARegNumber | IN | varchar2 | the external registry number of the registration
  -->param ARegID | OUT | reg_numbers.reg_id%type | the matching internal registry ID
  -->param APrototypeXml | IN | xmltype | the xml-based object definition for Registration
  -->return a clob containing the raw mixture node of a Registration xml
  */
  FUNCTION GetMixtureBlock(
    ARegNumber IN varchar2
    , AStructureAggregationList OUT clob
    , ARegID OUT reg_numbers.reg_id%type
    , APrototypeXml IN XmlType
  ) RETURN CLOB IS
    LQContext dbms_xmlgen.ctxHandle;
    LMixtureFields CLOB;
    LPickListFields CLOB;
    LXml CLOB;
    LResult CLOB;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
  dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetMixtureBlock_started', $$plsql_line,'start' );
    TraceWrite('GetMixtureBlock_0_Start', $$plsql_line, ARegNumber);

    LQContext := dbms_xmlgen.newContext(
      'select
        m.mixtureid, m.created, m.modified, m.statusid
        , m.personcreated, m.structureaggregation
        , r.regid, r.sequencenumber, r.regnumber, r.sequenceid
      from vw_mixture m
        inner join vw_registrynumber r on r.regid = m.regid
      where r.regnumber = :bindvar'
    );

    dbms_xmlgen.setBindValue(LQContext, 'bindvar', ARegNumber);
    LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQContext);

    --throw an error if we found no rows
    if LXml is null then
      if RegistrationRLS.GetStateRLS then
        RAISE_APPLICATION_ERROR(
          eInvalidRegNum
          , AppendError('The Registry "'||ARegNumber||'" doesn''t exist or isen''t available.')
        );
      else
        RAISE_APPLICATION_ERROR(
          eInvalidRegNum
          , AppendError('The Registry "'||ARegNumber||'" doesn''t exist.')
        );
      end if;
    end if;

    --remove the structure aggregation as a separate clob
    AStructureAggregationList :=
      TakeOffAndGetClobslist(LXml, '<STRUCTUREAGGREGATION>', NULL, Null, FALSE);

    select extractvalue(XmlType(LXml),'ROWSET/ROW/REGID')
    into ARegID from DUAL;

    LXml := replace(LXml,'</ROWSET>','');
    LXml := replace(LXml,'<ROWSET>','');

    TraceWrite('GetMixtureBlock_1_Data', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- PropertyList
    if (CompoundRegistry.FillPropertyTemplate = TRUE) then
      LXml := GetPropertyList('MIXTURE', ARegID, APrototypeXml).getClobVal();
    else
      GetPropertyListFields('Mixture', APrototypeXml, LMixtureFields, LPickListFields);

      IF (LMixtureFields IS NOT NULL) THEN
        LQContext := dbms_xmlgen.newContext(
          'select ' || LMixtureFields || '
          from vw_mixture m
            inner join vw_registrynumber r on r.regid = m.regid
          where m.regid = :bindvar'
        );

        dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ARegID));
        LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
        dbms_xmlgen.closeContext(LQContext);
        AddAttribPickList(LPickListFields, LXml, '<ROW>');
        LXml := replace(LXml, 'ROWSET', 'PropertyList');
        AddNullFields(LMixtureFields, LXml);
      END IF;
    end if;

    TraceWrite('GetMixtureBlock_2_PropertyList', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- IdentifierList
    LQContext := dbms_xmlgen.newContext(
      'select
        ci.id, ci.type, ci.regid, ci.value
        , i.description description, i.name name, i.active
      from vw_compound_identifier ci
        left outer join vw_identifiertype i on i.id = ci.type
      where ci.regid = :bindvar order by ci.orderindex'
    );

    dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ARegID));
    LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQContext);
    LXml := replace(LXml,'ROWSET','IdentifierList');

    TraceWrite('GetMixtureBlock_3_IdentifierList', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- ProjectList
    LQContext := dbms_xmlgen.newContext(
      'select
        rp.id, rp.projectid
        , p.description , p.name, p.active
      from vw_registrynumber_project rp
        inner join vw_project p on p.projectid = rp.projectid
      where rp.regid = :bindvar order by rp.orderindex'
    );

    dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ARegID));
    LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQContext);
    LXml := replace(LXml,'ROWSET','ProjectList');

    TraceWrite('GetMixtureBlock_4_ProjectList', $$plsql_line, LXml);
    LResult := LResult || LXml;
    LResult := '<Mixture>' || LResult || '</Mixture>';

    TraceWrite('GetMixtureBlock', $$plsql_line, LResult);
    TraceWrite( act||'GetMixtureBlock_ended', $$plsql_line,'end' );
    RETURN LResult;
  END;


  FUNCTION GetMixtureBlock_new(
    ARegNumber IN varchar2
    , AStructureAggregationList OUT clob
    , ARegID OUT reg_numbers.reg_id%type
    , APrototypeXml IN XmlType
  ) RETURN CLOB IS
    LQContext dbms_xmlgen.ctxHandle;
    LMixtureFields CLOB;
    LPickListFields CLOB;
    LXml CLOB;
    LResult CLOB;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetMixtureBlock_new_started', $$plsql_line,'start' );
    TraceWrite('GetMixtureBlock_new__0_Start', $$plsql_line, ARegNumber);

    LQContext := dbms_xmlgen.newContext(
      'select
        m.mixtureid, m.created, m.modified, m.statusid
        , m.personcreated, '|| q'['(Removed]' || 'STRUCTUREAGGREGATION' || q'[)']' ||' as structureaggregation
        , r.regid, r.sequencenumber, r.regnumber, r.sequenceid
      from vw_mixture m
        inner join vw_registrynumber r on r.regid = m.regid
      where r.regnumber = :bindvar'
    );

    dbms_xmlgen.setBindValue(LQContext, 'bindvar', ARegNumber);
    LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQContext);
    TraceWrite('GetMixtureBlock_new_02', $$plsql_line, LXml);

    --throw an error if we found no rows
    if LXml is null then
      if RegistrationRLS.GetStateRLS then
        RAISE_APPLICATION_ERROR(
          eInvalidRegNum
          , AppendError('The Registry "'||ARegNumber||'" doesn''t exist or isn''t available.')
        );
      else
        RAISE_APPLICATION_ERROR(
          eInvalidRegNum
          , AppendError('The Registry "'||ARegNumber||'" doesn''t exist.')
        );
      end if;
    end if;

    --remove the structure aggregation as a separate clob
    select to_clob('<Clob>') ||
  m.structureaggregation
  || '</Clob>'
  ,r.regid  into AStructureAggregationList, ARegID
    from vw_mixture m, vw_registrynumber r where  r.regid = m.regid and  r.regnumber = ARegNumber;
--    AStructureAggregationList := TakeOffAndGetClobslist(LXml, '<STRUCTUREAGGREGATION>', NULL, Null, FALSE);

    LXml := replace(LXml,'</ROWSET>','');
    LXml := replace(LXml,'<ROWSET>','');

    TraceWrite('GetMixtureBlock_new_1_Data', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- PropertyList
    if (CompoundRegistry.FillPropertyTemplate = TRUE) then
      LXml := GetPropertyList('MIXTURE', ARegID, APrototypeXml).getClobVal();
    else
      GetPropertyListFields('Mixture', APrototypeXml, LMixtureFields, LPickListFields);

      IF (LMixtureFields IS NOT NULL) THEN
        LQContext := dbms_xmlgen.newContext(
          'select ' || LMixtureFields || '
          from vw_mixture m
            inner join vw_registrynumber r on r.regid = m.regid
          where m.regid = :bindvar'
        );

        dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ARegID));
        LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
        dbms_xmlgen.closeContext(LQContext);
        AddAttribPickList(LPickListFields, LXml, '<ROW>');
        LXml := replace(LXml, 'ROWSET', 'PropertyList');
        AddNullFields(LMixtureFields, LXml);
      END IF;
    end if;

    TraceWrite('GetMixtureBlock_new_2_PropertyList', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- IdentifierList
    LQContext := dbms_xmlgen.newContext(
      'select
        ci.id, ci.type, ci.regid, ci.value
        , i.description description, i.name name, i.active
      from vw_compound_identifier ci
        left outer join vw_identifiertype i on i.id = ci.type
      where ci.regid = :bindvar order by ci.orderindex'
    );

    dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ARegID));
    LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQContext);
    LXml := replace(LXml,'ROWSET','IdentifierList');

    TraceWrite('GetMixtureBlock_new_3_IdentifierList', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- ProjectList
    LQContext := dbms_xmlgen.newContext(
      'select
        rp.id, rp.projectid
        , p.description , p.name, p.active
      from vw_registrynumber_project rp
        inner join vw_project p on p.projectid = rp.projectid
      where rp.regid = :bindvar order by rp.orderindex'
    );

    dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ARegID));
    LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQContext);
    LXml := replace(LXml,'ROWSET','ProjectList');

    TraceWrite('GetMixtureBlock_new_4_ProjectList', $$plsql_line, LXml);
    LResult := LResult || LXml;
    LResult := '<Mixture>' || LResult || '</Mixture>';

    TraceWrite('GetMixtureBlock_new', $$plsql_line, LResult);
    TraceWrite( act||'GetMixtureBlock_new_ended', $$plsql_line,'end' );
    RETURN LResult;
  END;

  /**
  Fetches the raw xml for the 'structure' of one Compound.
  This includes the Property List, Identifier List and inherent properties.
  -->author jed
  -->since January, 2010
  -->param ARegID the internal registry ID for a compound
  -->param ANormalizedStructureList a structure-extraction list (for xml brevity)
  -->param APrototypeXml the xml-based object definition for Registration
  -->return a clob containing the raw structure node of a Compound xml
  */
  FUNCTION GetStructureBlock(
    ARegID IN reg_numbers.reg_id%type
    , ANormalizedStructureList IN OUT clob
    , APrototypeXml IN XmlType DEFAULT NULL
  ) RETURN CLOB IS
    LQContext dbms_xmlgen.ctxHandle;
    LStructureFields CLOB;
    LPickListFields CLOB;
    LPrototypeXml XmlType := APrototypeXml;
    LXml CLOB;
    LResult CLOB;
    LStructureId vw_structure.structureid%type;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetStructureBlock_started', $$plsql_line,'start' );
    if (LPrototypeXml is null) then
      LPrototypeXml := VRegObjectTemplate;
    end if;

    TraceWrite('GetStructureBlock_0_Start', $$plsql_line, LXml);

    LQContext := dbms_xmlgen.newContext(
      'select
        s.structureid
        , s.structureformat
        , s.structure
        , s.drawingtype
        , nvl(c.formulaweight,cscartridge.molweight(s.structure)) formulaweight
        , nvl(c.molecularformula,cscartridge.formula(s.structure,'''')) molecularformula
        , c.regid
        , c.normalizedstructure
        , c.usenormalization
      from vw_compound c
        left outer join VW_Structure_Drawing s on c.structureid = s.structureid
      where c.regid = :bindvar'
    );

    dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ARegID));
    LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQContext);

    IF LXml IS NULL THEN
      RAISE_APPLICATION_ERROR(eNoRowsReturned, AppendError('No Structure rows returned.'));
    END IF;

    ANormalizedStructureList := ANormalizedStructureList
      || TakeOffAndGetClobslist(LXml, '<NORMALIZEDSTRUCTURE>', NULL, NULL, FALSE);
    ANormalizedStructureList :=
      '<ClobList>' || REPLACE(
        REPLACE(ANormalizedStructureList,'<ClobList>',''),'</ClobList>','')||'</ClobList>';

    LXml := replace(LXml, '</ROWSET>', '');
    LXml := replace(LXml, '<ROWSET>', '');
    SELECT STRUCTUREID INTO LStructureId
    FROM VW_Compound
    WHERE RegID=ARegID;
    LXml := replace(LXml, '</ROW>', '<CanPropogateStructureEdits>' || GetCanPropogateStructureEdits(LStructureId) || '</CanPropogateStructureEdits>');
    LXml := LXml || '</ROW>';

    LResult := LXml;
    TraceWrite('GetStructureBlock_1_Data', $$plsql_line, LXml);

    -- PropertyList
    if (CompoundRegistry.FillPropertyTemplate = TRUE) then
      select extractValue(xmltype(LXml), '/ROW/STRUCTUREID/text()')
      into LStructureId from dual;
      LXml := GetPropertyList('STRUCTURE', LStructureId, APrototypeXml).getClobVal();
    else
      GetPropertyListFields('Structure', LPrototypeXml, LStructureFields, LPickListFields);

      IF LStructureFields IS NOT NULL THEN
        LQContext := dbms_xmlgen.newContext(
          'SELECT ' || LStructureFields || '
           FROM VW_Compound C,VW_RegistryNumber R,VW_Structure_Drawing S
           WHERE C.RegID(+)=R.RegID AND S.StructureID(+)=C.StructureID AND C.RegID = :bindvar'
        );

        dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ARegID));
        LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
        dbms_xmlgen.closeContext(LQContext);
        AddAttribPickList(LPickListFields, LXml, '<ROW>');
        LXml := replace(LXml,'ROWSET','PropertyList');
        AddNullFields(LStructureFields,LXml);
      END IF;
    end if;

    TraceWrite('GetStructureBlock_2', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- IdentifierList
    LQContext := dbms_xmlgen.newContext(
      'select
        si.id, si.type, si.structureid, si.value
        , i.description description, i.name name, i.active
      from structure_identifier si
        inner join vw_compound c on c.structureid = si.structureid
        left outer join vw_identifiertype i on i.id = si.type
      where c.regid = :bindvar order by si.orderindex'
    );

    dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ARegID));
    LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQContext);
    LXml := replace(LXml,'ROWSET','IdentifierList');

    TraceWrite('GetStructureBlock_3_IdentifierList', $$plsql_line, LXml);
    LResult := LResult || LXml;

    LResult := '<Structure>' || LResult || '</Structure>';
    TraceWrite('GetStructureBlock', $$plsql_line, LResult);
    TraceWrite( act||'GetStructureBlock_ended', $$plsql_line,'end' );
    RETURN LResult;
  END;

  FUNCTION GetStructureBlock_new(
    ARegID IN reg_numbers.reg_id%type
    , ANormalizedStructureList IN OUT clob
    , APrototypeXml IN XmlType DEFAULT NULL
  ) RETURN CLOB IS
    LQContext dbms_xmlgen.ctxHandle;
    LStructureFields CLOB;
    LPickListFields CLOB;
    LPrototypeXml XmlType := APrototypeXml;
    LXml CLOB;
    LResult CLOB;
    LStructureId vw_structure.structureid%type;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
  dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetStructureBlock_new_started', $$plsql_line,'start' );
    if (LPrototypeXml is null) then
      LPrototypeXml := VRegObjectTemplate;
    end if;

    TraceWrite('GetStructureBlock_new_0_Start', $$plsql_line, LXml);

    LQContext := dbms_xmlgen.newContext(
      'select
        s.structureid
        , s.structureformat
        , s.structure
        , s.drawingtype
        , nvl(c.formulaweight,cscartridge.molweight(s.structure)) formulaweight
        , nvl(c.molecularformula,cscartridge.formula(s.structure,'''')) molecularformula
        , c.regid
        , '|| q'['(Removed]' || 'NORMALIZEDSTRUCTURE' || q'[)']' ||' as normalizedstructure
        , c.usenormalization
      from vw_compound c
        left outer join VW_Structure_Drawing s on c.structureid = s.structureid
      where c.regid = :bindvar'
    );

    dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ARegID));
    LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQContext);

    IF LXml IS NULL THEN
      RAISE_APPLICATION_ERROR(eNoRowsReturned, AppendError('No Structure rows returned.'));
    END IF;

    select ANormalizedStructureList ||
    '<Clob>' ||
    normalizedstructure
    || '</Clob>'
    into ANormalizedStructureList from vw_compound c
      where c.regid = ARegID;

--    ANormalizedStructureList := ANormalizedStructureList
--      || TakeOffAndGetClobslist(LXml, '<NORMALIZEDSTRUCTURE>', NULL, NULL, FALSE);
    ANormalizedStructureList :=
      '<ClobList>' || REPLACE(
        REPLACE(ANormalizedStructureList,'<ClobList>',''),'</ClobList>','')||'</ClobList>';

    LXml := replace(LXml, '</ROWSET>', '');
    LXml := replace(LXml, '<ROWSET>', '');
    SELECT STRUCTUREID INTO LStructureId
    FROM VW_Compound
    WHERE RegID=ARegID;
    LXml := replace(LXml, '</ROW>', '<CanPropogateStructureEdits>' || GetCanPropogateStructureEdits(LStructureId) || '</CanPropogateStructureEdits>');
    LXml := LXml || '</ROW>';

    LResult := LXml;
    TraceWrite('GetStructureBlock_new_1_Data', $$plsql_line, LXml);

    -- PropertyList
    if (CompoundRegistry.FillPropertyTemplate = TRUE) then
--      select extractValue(xmltype(LXml), '/ROW/STRUCTUREID/text()')
--      into LStructureId from dual;

      LXml := GetPropertyList('STRUCTURE', LStructureId, APrototypeXml).getClobVal();
    else
      GetPropertyListFields('Structure', LPrototypeXml, LStructureFields, LPickListFields);

      IF LStructureFields IS NOT NULL THEN
        LQContext := dbms_xmlgen.newContext(
          'SELECT ' || LStructureFields || '
           FROM VW_Compound C,VW_RegistryNumber R,VW_Structure_Drawing S
           WHERE C.RegID(+)=R.RegID AND S.StructureID(+)=C.StructureID AND C.RegID = :bindvar'
        );

        dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ARegID));
        LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
        dbms_xmlgen.closeContext(LQContext);
        AddAttribPickList(LPickListFields, LXml, '<ROW>');
        LXml := replace(LXml,'ROWSET','PropertyList');
        AddNullFields(LStructureFields,LXml);
      END IF;
    end if;

    TraceWrite('GetStructureBlock_new_2', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- IdentifierList
    LQContext := dbms_xmlgen.newContext(
      'select
        si.id, si.type, si.structureid, si.value
        , i.description description, i.name name, i.active
      from structure_identifier si
        inner join vw_compound c on c.structureid = si.structureid
        left outer join vw_identifiertype i on i.id = si.type
      where c.regid = :bindvar order by si.orderindex'
    );

    dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ARegID));
    LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQContext);
    LXml := replace(LXml,'ROWSET','IdentifierList');

    TraceWrite('GetStructureBlock_new_3_IdentifierList', $$plsql_line, LXml);
    LResult := LResult || LXml;

    LResult := '<Structure>' || LResult || '</Structure>';
    TraceWrite('GetStructureBlock_new', $$plsql_line, LResult);
    TraceWrite( act||'GetStructureBlock_new_ended', $$plsql_line,'end' );
    RETURN LResult;
  END;

  /**
  Fetches the raw xml for the 'structure' of one Compound.
  This includes the Property List, Identifier List and inherent properties.
  -->author jed
  -->since January, 2010
  -->param ARegID the internal registry ID for a compound
  -->param ANormalizedStructureList a structure-extraction list (for xml brevity)
  -->param APrototypeXml the xml-based object definition for Registration
  -->param ASectionsList a list of nodes to exclude during xml-fetching
  -->return a clob containing one raw Compound node of a Registration xml
  */
  FUNCTION GetCompoundBlock(
    ARegID IN reg_numbers.reg_id%type
    , ANormalizedStructureList IN OUT clob
    , APrototypeXml IN XmlType
    , ASectionsList in Varchar2 := NULL
  ) RETURN CLOB IS
    LQryCtx DBMS_XMLGEN.ctxHandle;
    LCompoundFields CLOB;
    LPickListFields CLOB;
    LXml CLOB;
    LResult CLOB;
    LCompoundID VW_COMPOUND.COMPOUNDID%type;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetCompoundBlock_started', $$plsql_line,'start' );
    TraceWrite('GetCompoundBlock_0_Start', $$plsql_line, 'ARegID ' || to_char(ARegID));

    LQryCtx := DBMS_XMLGEN.newContext(
      'select
        c.compoundid
        , c.datecreated
        , c.personcreated
        , c.personapproved
        , c.personregistered
        , c.datelastmodified
        , r.regid, r.sequencenumber, r.regnumber, r.sequenceid, -c.compoundid componentindex
        , c.tag
      from vw_compound c
        left outer join vw_registrynumber r on r.regid = c.regid
      where c.regid = :bindvar'
    );

    dbms_xmlgen.setBindValue(LQryCtx, 'bindvar', to_char(ARegID));
    LXml := replace(dbms_xmlgen.getXML(LQryCtx), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQryCtx);

    IF LXml IS NULL THEN
      RAISE_APPLICATION_ERROR(eNoRowsReturned, AppendError('No Compound rows returned.'));
    END IF;

    LXml := replace(LXml, '</ROWSET>', '');
    LXml := replace(LXml, '<ROWSET>', '');

    SELECT CompoundID INTO LCompoundID
    FROM VW_Compound
    WHERE RegID=ARegID;
    LXml := replace(LXml, '</ROW>', '<CanPropogateComponentEdits>' || GetCanPropogateComponentEdits(LCompoundID) || '</CanPropogateComponentEdits>');
    LXml := LXml || '</ROW>';

    TraceWrite('GetCompoundBlock_1_Data', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- Structure block
    LXml := GetStructureBlock(ARegID, ANormalizedStructureList, APrototypeXml);
    TraceWrite('GetCompoundBlock_2_Structure', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- PropertyList
    if (CompoundRegistry.FillPropertyTemplate = TRUE) then
      LXml := GetPropertyList('COMPONENT', ARegID, APrototypeXml).getClobVal();
    else
      GetPropertyListFields('Compound', APrototypeXml, LCompoundFields, LPickListFields);

      IF LCompoundFields IS NOT NULL THEN
        LQryCtx:=DBMS_XMLGEN.newContext(
          'select ' || LCompoundFields || '
          from vw_compound c
            left outer join vw_registrynumber r on r.regid = c.regid
            left outer join VW_Structure_Drawing s on s.structureid = c.structureid
          where c.regid = :bindvar'
        );

        dbms_xmlgen.setBindValue(LQryCtx, 'bindvar', to_char(ARegID));
        LXml := replace(dbms_xmlgen.getXML(LQryCtx), cXmlDecoration, null);
        dbms_xmlgen.closeContext(LQryCtx);
        AddAttribPickList(LPickListFields, LXml, '<ROW>');
        LXml := replace(LXml, 'ROWSET', 'PropertyList');
        AddNullFields(LCompoundFields,LXml);
      END IF;
    end if;

    TraceWrite('GetCompoundBlock_3_PropertyList', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- IdentifierList
    IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Identifier'), 0) > 0 ) THEN
      LQryCtx:=DBMS_XMLGEN.newContext(
        'select
          ci.id, ci.type, ci.regid, ci.value
          , i.description description, i.name name, i.active
        from vw_compound_identifier ci
          left outer join vw_identifiertype i on i.id = ci.type
        where ci.regid = :bindvar order by ci.orderindex'
      );

      dbms_xmlgen.setBindValue(LQryCtx, 'bindvar', to_char(ARegID));
      LXml := replace(dbms_xmlgen.getXML(LQryCtx), cXmlDecoration, null);
      dbms_xmlgen.closeContext(LQryCtx);
      LXml := replace(LXml, 'ROWSET', 'IdentifierList');

      TraceWrite('GetCompoundBlock_4_Identifiers', $$plsql_line, LXml);
      LResult := LResult || LXml;
    END IF;

    -- FragmentList
    IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Fragment'), 0) > 0 ) THEN
      LQryCtx:=DBMS_XMLGEN.newContext(
        'select
          f.fragmentid, f.code, f.description
          , f.fragmenttypeid, ft.description typedescription
          , f.molweight, f.formula, f.molweight, f.formula
          , f.created
          , f.modified
          , f.structureformat, f.structure
          , cf.equivalents, cf.id
        from vw_fragment f
          inner join vw_compound_fragment cf on cf.fragmentid = f.fragmentid
          inner join vw_compound c on c.compoundid = cf.compoundid
          inner join vw_fragmenttype ft on ft.id = f.fragmenttypeid
        where c.regid = :bindvar'
      );

      dbms_xmlgen.setBindValue(LQryCtx, 'bindvar', to_char(ARegID));
      LXml := replace(dbms_xmlgen.getXML(LQryCtx), cXmlDecoration, null);
      dbms_xmlgen.closeContext(LQryCtx);
      LXml := Replace(LXml, 'ROWSET', 'FragmentList');

      TraceWrite('GetCompoundBlock_5_FragmentList', $$plsql_line, LXml);
      LResult := LResult || LXml;
    END IF;

    -- fin
    LResult := '<Compound>' || LResult || '</Compound>';
    TraceWrite('GetCompoundBlock', $$plsql_line, LResult);
    TraceWrite( act||'GetCompoundBlock_ended', $$plsql_line,'end' );
    RETURN LResult;
  END;

  FUNCTION GetCompoundBlock_new(
    ARegID IN reg_numbers.reg_id%type
    , ANormalizedStructureList IN OUT clob
    , APrototypeXml IN XmlType
    , ASectionsList in Varchar2 := NULL
  ) RETURN CLOB IS
    LQryCtx DBMS_XMLGEN.ctxHandle;
    LCompoundFields CLOB;
    LPickListFields CLOB;
    LXml CLOB;
    LResult CLOB;
    LCompoundID VW_COMPOUND.COMPOUNDID%type;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
  dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetCompoundBlock_new_started', $$plsql_line,'start' );
    TraceWrite('GetCompoundBlock_new0_Start', $$plsql_line, 'ARegID ' || to_char(ARegID));

    LQryCtx := DBMS_XMLGEN.newContext(
      'select
        c.compoundid
        , c.datecreated
        , c.personcreated
        , c.personapproved
        , c.personregistered
        , c.datelastmodified
        , r.regid, r.sequencenumber, r.regnumber, r.sequenceid, -c.compoundid componentindex
        , c.tag
      from vw_compound c
        left outer join vw_registrynumber r on r.regid = c.regid
      where c.regid = :bindvar'
    );

    dbms_xmlgen.setBindValue(LQryCtx, 'bindvar', to_char(ARegID));
    LXml := replace(dbms_xmlgen.getXML(LQryCtx), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQryCtx);

    IF LXml IS NULL THEN
      RAISE_APPLICATION_ERROR(eNoRowsReturned, AppendError('No Compound rows returned.'));
    END IF;

    LXml := replace(LXml, '</ROWSET>', '');
    LXml := replace(LXml, '<ROWSET>', '');

    SELECT CompoundID INTO LCompoundID
    FROM VW_Compound
    WHERE RegID=ARegID;
    LXml := replace(LXml, '</ROW>', '<CanPropogateComponentEdits>' || GetCanPropogateComponentEdits(LCompoundID) || '</CanPropogateComponentEdits>');
    LXml := LXml || '</ROW>';

    TraceWrite('GetCompoundBlock_new1_Data', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- Structure block
    LXml := GetStructureBlock_new(ARegID, ANormalizedStructureList, APrototypeXml);
    TraceWrite('GetCompoundBlock_new2_Structure', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- PropertyList
    if (CompoundRegistry.FillPropertyTemplate = TRUE) then
      LXml := GetPropertyList('COMPONENT', ARegID, APrototypeXml).getClobVal();
    else
      GetPropertyListFields('Compound', APrototypeXml, LCompoundFields, LPickListFields);

      IF LCompoundFields IS NOT NULL THEN
        LQryCtx:=DBMS_XMLGEN.newContext(
          'select ' || LCompoundFields || '
          from vw_compound c
            left outer join vw_registrynumber r on r.regid = c.regid
            left outer join VW_Structure_Drawing s on s.structureid = c.structureid
          where c.regid = :bindvar'
        );

        dbms_xmlgen.setBindValue(LQryCtx, 'bindvar', to_char(ARegID));
        LXml := replace(dbms_xmlgen.getXML(LQryCtx), cXmlDecoration, null);
        dbms_xmlgen.closeContext(LQryCtx);
        AddAttribPickList(LPickListFields, LXml, '<ROW>');
        LXml := replace(LXml, 'ROWSET', 'PropertyList');
        AddNullFields(LCompoundFields,LXml);
      END IF;
    end if;

    TraceWrite('GetCompoundBlock_new3_PropertyList', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- IdentifierList
    IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Identifier'), 0) > 0 ) THEN
      LQryCtx:=DBMS_XMLGEN.newContext(
        'select
          ci.id, ci.type, ci.regid, ci.value
          , i.description description, i.name name, i.active
        from vw_compound_identifier ci
          left outer join vw_identifiertype i on i.id = ci.type
        where ci.regid = :bindvar order by ci.orderindex'
      );

      dbms_xmlgen.setBindValue(LQryCtx, 'bindvar', to_char(ARegID));
      LXml := replace(dbms_xmlgen.getXML(LQryCtx), cXmlDecoration, null);
      dbms_xmlgen.closeContext(LQryCtx);
      LXml := replace(LXml, 'ROWSET', 'IdentifierList');

      TraceWrite('GetCompoundBlock_new4_Identifiers', $$plsql_line, LXml);
      LResult := LResult || LXml;
    END IF;

    -- FragmentList
    IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Fragment'), 0) > 0 ) THEN
      LQryCtx:=DBMS_XMLGEN.newContext(
        'select
          f.fragmentid, f.code, f.description
          , f.fragmenttypeid, ft.description typedescription
          , f.molweight, f.formula, f.molweight, f.formula
          , f.created
          , f.modified
          , f.structureformat, f.structure
          , cf.equivalents, cf.id
        from vw_fragment f
          inner join vw_compound_fragment cf on cf.fragmentid = f.fragmentid
          inner join vw_compound c on c.compoundid = cf.compoundid
          inner join vw_fragmenttype ft on ft.id = f.fragmenttypeid
        where c.regid = :bindvar'
      );

      dbms_xmlgen.setBindValue(LQryCtx, 'bindvar', to_char(ARegID));
      LXml := replace(dbms_xmlgen.getXML(LQryCtx), cXmlDecoration, null);
      dbms_xmlgen.closeContext(LQryCtx);
      LXml := Replace(LXml, 'ROWSET', 'FragmentList');

      TraceWrite('GetCompoundBlock_new5_FragmentList', $$plsql_line, LXml);
      LResult := LResult || LXml;
    END IF;

    -- fin
    LResult := '<Compound>' || LResult || '</Compound>';
    TraceWrite('GetCompoundBlock', $$plsql_line, LResult);
    TraceWrite( act||'GetCompoundBlock_new_ended', $$plsql_line,'end' );
    RETURN LResult;
  END;

  /**
  Fetches the raw xml for one 'batch component' of a Batch.
  This includes the Property List, Identifier List and inherent properties.
  -->author jed
  -->since January, 2010
  -->param ABatchComponentID an internal batchcomponent ID
  -->param APrototypeXml the xml-based object definition for Registration
  -->return a clob containing one raw BatchComponent node of a Registration xml
  */
  FUNCTION GetBatchComponentBlock(
    ABatchComponentID IN vw_batchcomponent.id%type
    , APrototypeXml IN XmlType
  ) RETURN CLOB IS
    LQryCtx DBMS_XMLGEN.ctxHandle;
    LBatchComponentFields CLOB;
    LPickListFields CLOB;
    LXml CLOB;
    LResult CLOB;

    CURSOR C_BatchComponentFragmentIDs(ABatchComponetID BatchComponentFragment.BatchComponentID%Type) IS
    select bcf.id
    from vw_batchcomponentfragment bcf
    where bcf.batchcomponentid = ABatchComponetID
    order by bcf.orderindex;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
  dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetBatchComponentBlock_started', $$plsql_line,'start' );

    TraceWrite('GetBatchComponentBlock_0_Start', $$plsql_line, 'ABatchComponentID ' || to_char(ABatchComponentID));

    LQryCtx := DBMS_XMLGEN.newContext(
      'select
        p.id, p.batchid
      , m.compoundid, m.mixturecomponentid, -m.compoundid componentindex, r.regnumber
      from vw_batchcomponent p
        inner join vw_mixture_component m on m.mixturecomponentid = p.mixturecomponentid
        inner join vw_compound c on c.compoundid = m.compoundid
        inner join vw_registrynumber r on r.regid = c.regid
      where p.id = :bindvar'
    );

    dbms_xmlgen.setBindValue(LQryCtx, 'bindvar', to_char(ABatchComponentID));
    LXml := replace(dbms_xmlgen.getXML(LQryCtx), cXmlDecoration, '');
    dbms_xmlgen.closeContext(LQryCtx);
    LXml := replace(LXml,'<ROWSET>', null);
    LXml := replace(LXml,'</ROWSET>', null);

    TraceWrite('GetBatchComponentBlock_1_Data', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- PropertyList
    if (CompoundRegistry.FillPropertyTemplate = TRUE) then
      LXml := GetPropertyList('BATCHCOMPONENT', ABatchComponentID, APrototypeXml).getClobVal();
    else
      GetPropertyListFields('BatchComponent', APrototypeXml, LBatchComponentFields, LPickListFields);

      IF LBatchComponentFields IS NOT NULL THEN
        LQryCtx := DBMS_XMLGEN.newContext(
          'SELECT ' || LBatchComponentFields || '
           FROM VW_BatchComponent B
           WHERE B.ID = :bindvar'
        );

        dbms_xmlgen.setBindValue(LQryCtx, 'bindvar', to_char(ABatchComponentID));
        LXml := replace(dbms_xmlgen.getXML(LQryCtx), cXmlDecoration, '');
        dbms_xmlgen.closeContext(LQryCtx);
        AddAttribPickList(LBatchComponentFields, LXml, '<ROW>');
        LXml := replace(LXml, 'ROWSET', 'PropertyList');
        AddNullFields(LBatchComponentFields, LXml);
      END IF;
    end if;

    TraceWrite('GetBatchComponentBlock_2_PropertyList', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- BatchComponentFragment
    FOR R_BatchComponentFragmentIDs in C_BatchComponentFragmentIDs(ABatchComponentID) LOOP
      LQryCtx:=DBMS_XMLGEN.newContext(
        'select
           cf.fragmentid, bcf.compoundfragmentid, bcf.equivalent, bcf.id
         from vw_batchcomponentfragment bcf
           inner join vw_compound_fragment cf on cf.id = bcf.compoundfragmentid
         where bcf.id = :bindvar'
      );

      dbms_xmlgen.setBindValue(LQryCtx, 'bindvar', to_char(R_BatchComponentFragmentIDs.ID));
      LXml := replace(dbms_xmlgen.getXML(LQryCtx), cXmlDecoration, '');
      dbms_xmlgen.closeContext(LQryCtx);
      LXml := replace(LXml, 'ROWSET', 'BatchComponentFragment');
      LXml := replace(LXml, '</ROWSET>', null);

      TraceWrite('GetBatchComponentFragmentBlock', $$plsql_line, LXml);
      LResult := LResult || LXml;
    END LOOP;

    LResult := '<BatchComponent>' || LResult ||' </BatchComponent>' ;
    TraceWrite('GetBatchComponentBlock', $$plsql_line, LResult);
    TraceWrite( act||'GetBatchComponentBlock_ended', $$plsql_line,'end' );
    RETURN LResult;
  END;

  /**
  Fetches the raw xml for one 'batch' of a Registry xml.
  This includes the Property List, Project List, Identifier List and inherent properties.
  -->author jed
  -->since January, 2010
  -->param ABatchID the internal ID for an individual batch
  -->param APrototypeXml the xml-based object definition for Registration
  -->param ASectionsList a list of nodes to exclude during xml-fetching
  -->return a clob containing one raw Batch node of a Registration xml
  */
  FUNCTION GetBatchBlock(
    ARegNumber IN VW_RegistryNumber.RegNumber%type
    , ABatchID IN vw_batch.batchid%type
    , APrototypeXml IN XmlType
    , ASectionsList in Varchar2 := NULL
  ) RETURN CLOB IS
    LQContext dbms_xmlgen.ctxHandle;
    LBatchFields CLOB;
    LPickListFields CLOB;
    LXml CLOB;
    LResult CLOB;
    LBatchNumber NUMBER := 0;
    CURSOR C_BatchComponentIDs(ABatchID BatchComponent.batchid%Type) IS
    select p.id
    from vw_batchcomponent p
    where p.batchid = ABatchID
    order by p.orderindex;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetBatchBlock_started', $$plsql_line,'start' );
    TraceWrite('GetBatchBlock_0_Start', $$plsql_line, 'ABatchID ' || to_char(ABatchID));

    LQContext := dbms_xmlgen.newContext(
      'select
        b.batchid, b.fullregnumber, b.batchnumber, b.statusid
        , b.datecreated, b.personcreated, b.personapproved
        , b.personregistered, b.datelastmodified
        , pc.user_id personcreateddisplay
        , pr.user_id personregistereddisplay
        , pa.user_id personapproveddisplay
      from vw_batch b
        left outer join cs_security.people pc on pc.person_id = b.personcreated
        left outer join cs_security.people pr on pr.person_id = b.personregistered
        left outer join cs_security.people pa on pa.person_id = b.personapproved
      where b.batchid = :bindvar'
    );

    dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ABatchID));
    LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQContext);
    LXml := replace(LXml, '<ROWSET>', '');
    LXml := replace(LXml, '</ROWSET>', '');
    SELECT BatchNumber INTO LBatchNumber
    FROM VW_Batch
    WHERE BatchID=ABatchID;
    LXml := replace(LXml, '</ROW>', '<IsBatchEditable>' || GetIsBatchEditable(ARegNumber, LBatchNumber) || '</IsBatchEditable>');
    LXml := LXml || '</ROW>';

    TraceWrite('GetBatchBlock_1_Data', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- PropertyList
    if (CompoundRegistry.FillPropertyTemplate = TRUE) then
      LXml := GetPropertyList('BATCH', ABatchID, APrototypeXml).getClobVal();
    else
      GetPropertyListFields('Batch', APrototypeXml, LBatchFields, LPickListFields);

      if LBatchFields is not null then
        LQContext := dbms_xmlgen.newContext(
          'SELECT ' || LBatchFields || '
           FROM VW_Batch B
           WHERE B.BatchID = :bindvar'
        );

        dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ABatchID));
        LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
        dbms_xmlgen.closeContext(LQContext);
        TraceWrite('GetBatchBlock_1.5_PropertyPickListFields', $$plsql_line, LPickListFields);
        AddAttribPickList(LPickListFields, LXml, '<ROW>');

        LXml := replace(LXml,'ROWSET','PropertyList');
        AddNullFields(LBatchFields, LXml);
      end if;
    end if;

    TraceWrite('GetBatchBlock_2_PropertyList', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- IdentifierList
    LQContext := dbms_xmlgen.newContext(
      'select
        bi.id, bi.type, bi.value
        , p.description, p.active, p.name
      from vw_batchidentifier bi
        inner join vw_identifiertype p on p.id = bi.type
      where bi.batchid = :bindvar'
    );

    dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ABatchID));
    LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQContext);
    LXml := replace(LXml, 'ROWSET', 'IdentifierList');

    TraceWrite('GetBatchBlock_3_IdentifierList', $$plsql_line, LXml);
    LResult := LResult || LXml;

    --ProjectList
    LQContext := dbms_xmlgen.newContext(
      'select
        bp.id, bp.projectid
      , p.description, p.active, p.name
      from vw_batch_project bp
        inner join vw_project p on p.projectid = bp.projectid
      where bp.batchid = :bindvar'
    );

    dbms_xmlgen.setBindValue(LQContext, 'bindvar', to_char(ABatchID));
    LXml := replace(dbms_xmlgen.getXML(LQContext), cXmlDecoration, null);
    dbms_xmlgen.closeContext(LQContext);
    LXml := replace(LXml, 'ROWSET', 'ProjectList');

    TraceWrite('GetBatchBlock_4_ProjectList', $$plsql_line, LXml);
    LResult := LResult || LXml;

    -- Batch Components
    FOR R_BatchComponentIDs in C_BatchComponentIDs(ABatchID) LOOP
      LXml := GetBatchComponentBlock(R_BatchComponentIDs.Id, APrototypeXml);
      LResult := LResult || LXml;
    END LOOP;

    -- fin
    LResult := '<Batch>' || LResult || '</Batch>';
    TraceWrite('GetBatchBlock', $$plsql_line, LResult);
    TraceWrite( act||'GetBatchBlock_ended', $$plsql_line,'end' );
    RETURN LResult;
  END;

  /**
  Retrieves all the data associated with a single mixture registration via the Batch ID from
  one of its batches.
  -->author JED
  -->since 01-JUN-2011
  -->param ABatchID the BatchID of a batch from the registry record being sought
  -->param AXml the xml-string representation of the RegistryRecord object
  */
  PROCEDURE GetRegistrationByBatch(
    ABatchID in vw_batch.batchid%type
    , AXml out NOCOPY clob
  ) IS
    v_regnumber vw_registrynumber.regnumber%type;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
  dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetRegistrationByBatch_started '||ABatchID, $$plsql_line,'start' );

    select rn.reg_number into v_regnumber
    from reg_numbers rn
      inner join vw_mixture m on m.regid = rn.reg_id
      inner join vw_batch b on b.regid = m.regid
    where b.batchid = ABatchID;

    RetrieveMultiCompoundRegistry(v_regnumber, AXml);
    TraceWrite( act||'GetRegistrationByBatch_ended', $$plsql_line,'end' );
  END;

  /**
  Retrieves all the data associated with a single mixture registration via the Batch Registration Number from
  one of its batches.
  -->author JED
  -->since 01-JUN-2011
  -->param ABatchRegNumber the FullRegNum of a batch from the registry record being sought
  -->param AXml the xml-string representation of the RegistryRecord object
  */
  PROCEDURE GetRegistrationByBatch(
    ABatchRegNumber in vw_batch.fullregnumber%type
    , AXml out NOCOPY clob
  ) IS
    v_regnumber vw_registrynumber.regnumber%type;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
  dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetRegistrationByBatch_started '||ABatchRegNumber, $$plsql_line,'start' );

    select rn.reg_number into v_regnumber
    from reg_numbers rn
      inner join vw_mixture m on m.regid = rn.reg_id
      inner join vw_batch b on b.regid = m.regid
    where b.fullregnumber = ABatchRegNumber;

    RetrieveMultiCompoundRegistry(v_regnumber, AXml);
    TraceWrite( act||'GetRegistrationByBatch_ended', $$plsql_line,'end' );
  END;

  /**
  Go over the Field's tag of the properties and add the attributes of picklist; customized for batch & batchcomponent picklists
  -->author munnikrishnan
  -->since March 2014
  -->param AFields   list of field name with picklist
  -->param AXml      xml with the property and property's value
  -->param ABeginXml tag parent to begin the search
  */
  PROCEDURE AddAttribPickListForBatch(
    AFields IN CLOB
    , AXml IN OUT NOCOPY CLOB
    , ABeginXml IN CLOB
  ) IS
    LPosBegin                 NUMBER;
    LPosDot                   NUMBER;
    LPoslast                  NUMBER;
    LField                    VARCHAR2(100);
    LFieldTag                 VARCHAR2(100);
    LFieldTagEnd              VARCHAR2(100);
    LpickListDomainID               VARCHAR2(100);
    LFields                   CLOB;
    LPosField                 NUMBER;
    LPosFieldEnd              NUMBER;
    LExt_Table                VW_PickListDomain.Ext_Table%Type;
    LExt_ID_Col               VW_PickListDomain.Ext_ID_Col%Type;
    LExt_Display_Col          VW_PickListDomain.Ext_Display_Col%Type;
    LPickListDisplayValue     CLOB;
    LFieldValue               VARCHAR2(100);
    LSelect                   VARCHAR2(4000);
    ATempXml                          CLOB;
    AFinalXml                          CLOB;
    AEndXml                              CLOB;
    LXmlPosLast                     NUMBER;
    LXmlPosEndTag             NUMBER;
    LXmlCopyRestPos                 NUMBER;

  mod1 varchar2(100); act varchar2(100);
  BEGIN
  dbms_application_info.read_module(mod1, act); TraceWrite( act||'AddAttribPickListForBatch_started', $$plsql_line,'start' );
    AFinalXml := NULL;
  LFields := AFields || ',';
    AEndXml := '</' || SUBSTR(ABeginXml, 2, LENGTH(ABeginXml)-1);
  LXmlCopyRestPos := 0;
    LXmlPosLast := 1;
    LOOP
        LXmlPosEndTag := INSTR(AXml, AEndXml, LXmlPosLast);
    IF LXmlPosEndTag = 0 THEN
      LXmlCopyRestPos := LXmlPosLast;
    END IF;
        ATempXml := SUBSTR(AXml, LXmlPosLast, LXmlPosEndTag-LXmlPosLast+1);
        LXmlPosLast := LXmlPosEndTag + 1;
        EXIT WHEN LXmlPosEndTag=0;
        LPosBegin := 0;
        LPoslast := 1;

        LOOP
          LPosBegin := INSTR(LFields,',', LPoslast);
          LPosDot := INSTR(LFields,':', LPoslast);
          LField := TRIM( UPPER(SUBSTR(LFields, LPoslast, LPosDot-LPoslast)) );
        EXIT WHEN LField IS NULL;

          LpickListDomainID := TRIM( SUBSTR(LFields, LPosDot+1, LPosBegin-LPosDot-1) );
          LPoslast := LPosBegin + 1;

          LFieldTag := '<'||LField||'>';
          LFieldTagEnd := '</'||LField||'>';
          LPosField := INSTR( ATempXml, LFieldTag, INSTR(ATempXml, ABeginXml)+1 );

--          TraceWrite('AddAttribPickListForBatch_ATempXml', $$plsql_line, ABeginXml||' ->'|| ATempXml);
          TraceWrite('AddAttribPickListForBatch_Field', $$plsql_line, LField);
          TraceWrite('AddAttribPickListForBatch_DomainID', $$plsql_line, LPickListDomainID);
          TraceWrite('AddAttribPickListForBatch_Position', $$plsql_line, to_char(LPosField));

          IF LPosField <> 0 THEN
            IF NVL(LpickListDomainID,0)<>0 THEN
              SELECT Ext_Table,Ext_ID_Col,Ext_Display_Col
              INTO LExt_Table,LExt_ID_Col,LExt_Display_Col
              FROM VW_PickListDomain PLM
              WHERE PLM.ID = LPickListDomainID;

              LPosFieldEnd := INSTR(ATempXml,LFieldTagEnd,INSTR(ATempXml,ABeginXml)+1);
              LFieldValue := SUBSTR(ATempXml,LPosField+Length(LFieldTag),LPosFieldEnd-(LPosField+Length(LFieldTag))) ;

              IF LFieldValue IS NOT NULL AND LExt_Display_Col IS NOT NULL AND LExt_Table IS NOT NULL AND LExt_ID_Col IS NOT NULL THEN
                 LSelect :=
                   'SELECT ' || LExt_Display_Col || ' FROM ' || LExt_Table  ||' WHERE ' || LExt_ID_Col || ' = :LFieldValue';

                 TraceWrite('AddAttribPickListForBatch_Select', $$plsql_line, LSelect||'; LFieldValue ='||LFieldValue);
                 BEGIN
                    EXECUTE IMMEDIATE LSelect INTO LPickListDisplayValue
                    USING LFieldValue;
                 EXCEPTION
                    WHEN OTHERS THEN
                    BEGIN
                      TraceWrite('AddAttribPickListForBatch_Error', $$plsql_line, DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
                      LPickListDisplayValue:=null;
                    END;
                 END;

                 IF LPickListDisplayValue IS NOT NULL THEN
                   ATempXml := REPLACE(ATempXml,LFieldTag,'<'||LField||' pickListDomainID="'||LPickListDomainID||'" pickListDisplayValue="'||LPickListDisplayValue||'">');
                 END IF;

              END IF;
            END IF;
          END IF;
        END LOOP;
    AFinalXml := AFinalXml || ATempXml;
    END LOOP;
    IF AFinalXml IS NOT NULL THEN
        AXml := AFinalXml || TRIM(SUBSTR(AXml,LXmlCopyRestPos,LENGTH(AXml)-LXmlCopyRestPos+1));
    END IF;
  TraceWrite('AddAttribPickListForBatch_ModifiedXml', $$plsql_line, AXml);
  TraceWrite( act||'AddAttribPickListForBatch_ended', $$plsql_line,'end' );
  END;


  FUNCTION SplitClob (p_clob_in CLOB, p_delim VARCHAR2) RETURN t_array
  AS
    v_buffer VARCHAR2(500);
    v_len NUMBER;
    v_offset NUMBER := 1;
    v_delim_pos NUMBER;
    v_amount NUMBER;
    v_result_array t_array := t_array();
  mod1 varchar2(100); act varchar2(100);
  BEGIN
  dbms_application_info.read_module(mod1, act); TraceWrite( act||'SplitClob_started', $$plsql_line,'start' );
    IF p_clob_in IS NOT NULL THEN
      v_len := dbms_lob.getlength(p_clob_in);

      WHILE v_offset < v_len
      LOOP
        v_delim_pos := instr(p_clob_in, p_delim, v_offset);

        IF v_delim_pos = 0 THEN
          v_amount := v_len - v_offset + 1;
        ELSE
          v_amount := v_delim_pos - v_offset;
        END IF;

        dbms_lob.read(p_clob_in, v_amount, v_offset, v_buffer);
        v_offset := v_offset + v_amount + 1;

        v_result_array.EXTEND;
        v_result_array(v_result_array.LAST) := v_buffer;
      END LOOP;
    END IF;
    TraceWrite( act||'SplitClob_ended', $$plsql_line,'end' );
    RETURN v_result_array;
  END;

  /**
  Retrieves all the data associated with a single mixture registration via its
  unique, public-facing 'RegNumber'.
  -->author Fari
  -->param ARegNumber the Registration Number created during import or insert
  -->param AXml the xml-string representation of the RegistryRecord object
  -->param ASectionsList a csv-list of section names to include in the returned string
  */

    PROCEDURE RetrieveMultiCompoundRegistry(
    ARegNumber in VW_RegistryNumber.RegNumber%type
    , AXml out NOCOPY clob
    , ASectionsList in Varchar2 := NULL
  ) IS
    LResult                   CLOB;
    LXml                      CLOB;
    LXmlTables                XmlType;
    LXmlResult                XmlType;
    LStructuresList           CLOB;
    LNormalizedStructureList  CLOB;
    LStructureAggregationList CLOB;
    LRegID                    reg_numbers.reg_id%type;
    LCompundRegID             reg_numbers.reg_id%type;
    LCoeObjectConfigField     XmlType;
    LXslt XmlType := XslMcrrFetch;
    v_propertyListMetadata XMLTYPE;
    -- we will ALWAYS override the value for v_propertyFields, so this is just CLOB initialization
    v_propertyFields CLOB := '*';
    v_sqlFields1 varchar2(4000);
    v_sqlFields2 varchar2(4000);
    v_sqlFields3 varchar2(4000);
    v_sqlBatchPropList varchar2(4000);
    v_sqlBatchIdentifierList varchar2(4000);
    v_sqlBatchProjectList varchar2(4000);
    v_sqlBatchComponentPropList varchar2(4000);
    v_sqlBatchComponentFragment varchar2(4000);
    v_array                   t_array;
    v_LXml  XMLType;
    v_ctx dbms_xmlgen.ctxHandle;
    v_queryOutput CLOB;
    v_node XMLTYPE;
    LBatchFields CLOB;
    LBatchPickListFields CLOB;
    LBatchComponentFields CLOB;
    LBatchComponentPickListFields CLOB;
    lstrIsBatchEditable varchar2(80);
    CURSOR C_CompoundRegIDs(ARegID in VW_RegistryNumber.regid%type) IS
      SELECT C.RegID
      FROM VW_Mixture M,VW_Mixture_Component MC, VW_Compound C
      WHERE M.MixtureID=MC.MixtureID AND C.CompoundID=MC.CompoundID AND M.RegID=ARegID ORDER BY C.RegID;

  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'RetrieveMultiCompoundRegistry_started', $$plsql_line,'start' );
    SetSessionParameter;

    --Get Query or Get empty template xml
    IF (ARegNumber IS NOT NULL) THEN
      LCoeObjectConfigField := VRegObjectTemplate;

      --Build a string containing the Mixture-level block
      IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Mixture'), 0) > 1 ) THEN
        LResult :=
          '<MultiCompoundRegistryRecord '
          ||'SameBatchesIdentity="'||VSameBatchesIdentity
          ||'" ActiveRLS="'||vActiveRLS
          ||'" IsEditable="'||GetIsEditable(ARegNumber)
          ||'" IsRegistryDeleteable="'||GetIsRegistryDeleteable(ARegNumber)
          ||'" TypeRegistryRecord="Mixture">';

        LXml := GetMixtureBlock_new(ARegNumber, LStructureAggregationList, LRegID, LCoeObjectConfigField);
--        LXml := GetMixtureBlock(ARegNumber, LStructureAggregationList, LRegID, LCoeObjectConfigField);
        LResult := LResult || LXml;
      ELSE
        LResult:='<MultiCompoundRegistryRecord TypeRegistryRecord="WithoutMixture">';
      END IF;
      TraceWrite( act||'RetrieveMultiCompoundRegistry_GetMixtureBlock_LXml', $$plsql_line,LXml );
      --Continue building the string with all the Compound-level blocks
      IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Compound'), 0) > 0) THEN
        IF (ASectionsList IS NULL) OR INSTR(ASectionsList,'Mixture') <> 0 THEN
          FOR R_CompoundRegIDs IN C_CompoundRegIDs(LRegID) LOOP
            LXml := GetCompoundBlock_new(R_CompoundRegIDs.RegID, LNormalizedStructureList, LCoeObjectConfigField, ASectionsList);
--            LXml := GetCompoundBlock(R_CompoundRegIDs.RegID, LNormalizedStructureList, LCoeObjectConfigField, ASectionsList);
            LResult := LResult || LXml;
          END LOOP;
        ELSE
          SELECT RegID INTO LCompundRegID
          FROM VW_RegistryNumber
          WHERE RegNumber = ARegNumber;

          LXml := GetCompoundBlock_new(LCompundRegID, LNormalizedStructureList, LCoeObjectConfigField, ASectionsList);
          LResult := LResult || LXml;
        END IF;

        LStructuresList := TakeOffAndGetClobslist(LResult, '<STRUCTURE>', NULL, Null, FALSE);
      END IF;

      TraceWrite('RetrieveMultiCompoundRegistry_GetCompoundBlock_LXml', $$plsql_line,LNormalizedStructureList );
      --Continue building the string with all the Batch-level blocks
      IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Batch'), 0) > 0 ) THEN
        -- PropertyList
        if (CompoundRegistry.FillPropertyTemplate = FALSE) then
          GetPropertyListFields('Batch', LCoeObjectConfigField, LBatchFields, LBatchPickListFields);
--          TraceWrite('GetPropertyListFields->LBatchFields', $$plsql_line, LBatchFields);
--          TraceWrite('GetPropertyListFields->LBatchPickListFields', $$plsql_line, LBatchPickListFields);
          GetPropertyListFields('BatchComponent', LCoeObjectConfigField, LBatchComponentFields, LBatchComponentPickListFields);
--          TraceWrite('GetPropertyListFields->LBatchComponentFields', $$plsql_line, LBatchComponentFields);
--          TraceWrite('GetPropertyListFields->LBatchComponentPickListFields', $$plsql_line, LBatchComponentPickListFields);
        end if;

    IF upper(vActiveRLS)='OFF' THEN
      lstrIsBatchEditable := q'['True']';
    else
       lstrIsBatchEditable := ' CompoundRegistry.GetIsBatchEditable(''' || ARegNumber || ''', b.batchnumber)';
    END IF;

        v_sqlFields1 :=
          'SELECT XMLELEMENT("ROWSET", XMLAGG(XMLELEMENT("ROW"
          , XMLELEMENT("BATCHID", b.batchid)
          , XMLELEMENT("FULLREGNUMBER", b.fullregnumber)
          , XMLELEMENT("BATCHNUMBER", b.batchnumber)
          , XMLELEMENT("STATUSID", b.statusid)
          , XMLELEMENT("DATECREATED", b.datecreated)
          , XMLELEMENT("PERSONCREATED", b.personcreated)
          , XMLELEMENT("PERSONAPPROVED", b.personapproved)
          , XMLELEMENT("PERSONREGISTERED", b.personregistered)
          , XMLELEMENT("DATELASTMODIFIED", b.datelastmodified)
          , XMLELEMENT("PERSONCREATEDDISPLAY", pc.user_id)
          , XMLELEMENT("PERSONREGISTEREDDISPLAY", pr.user_id)
          , XMLELEMENT("PERSONAPPROVEDDISPLAY", pa.user_id)
          , XMLELEMENT("IsBatchEditable",'||lstrIsBatchEditable||')';
          v_sqlBatchPropList := ', (SELECT XMLELEMENT("BATCHPROPERTYLIST", XMLAGG(XMLELEMENT("BATCHPROPERTYLIST_ROW", ';
          if (CompoundRegistry.FillPropertyTemplate = TRUE) then
            v_sqlBatchPropList := v_sqlBatchPropList || 'CompoundRegistry.GetFilledPropertyList(''BATCH'', b.batchid)';
          else
            v_array := SplitClob(LBatchFields, ',');
            FOR v_i IN v_array.first..v_array.last
            LOOP
--              if v_i <> 1 then
--                v_sqlBatchPropList := v_sqlBatchPropList ;
--              end if;
--              v_sqlBatchPropList := v_sqlBatchPropList || 'XMLELEMENT("' || UPPER(v_array(v_i)) || '", ' || v_array(v_i) || ')';
              v_sqlBatchPropList := v_sqlBatchPropList || add_attrib( UPPER(v_array(v_i)),LBatchPickListFields,'b.') || ',';
                     TraceWrite('v_sqlBatchPropList', $$plsql_line,'v_array(v_i)->' ||v_array(v_i)||'-> '||v_sqlBatchPropList);
            END LOOP;
            v_sqlBatchPropList := rtrim(v_sqlBatchPropList,',') || '))) from vw_batch v where v.batchid = b.batchid)';
          end if;
          v_sqlBatchIdentifierList := ', (SELECT XMLELEMENT("BATCHIDENTIFIERLIST", XMLAGG(XMLELEMENT("BATCHIDENTIFIERLIST_ROW"
          , XMLELEMENT("ID", bi.id)
          , XMLELEMENT("TYPE", bi.type)
          , XMLELEMENT("VALUE", bi.value)
          , XMLELEMENT("DESCRIPTION", p.description)
          , XMLELEMENT("ACTIVE", p.active)
          , XMLELEMENT("NAME", p.name)
          ))) from vw_batchidentifier bi inner join vw_identifiertype p on p.id = bi.type where bi.batchid = b.batchid)';
          v_sqlBatchProjectList := ', (SELECT XMLELEMENT("BATCHPROJECTLIST", XMLAGG(XMLELEMENT("BATCHPROJECTLIST_ROW"
          , XMLELEMENT("ID", bp.id)
          , XMLELEMENT("PROJECTID", bp.projectid)
          , XMLELEMENT("DESCRIPTION", p.description)
          , XMLELEMENT("ACTIVE", p.active)
          , XMLELEMENT("NAME", p.name)
          ))) from vw_batch_project bp inner join vw_project p on p.projectid = bp.projectid where bp.batchid = b.batchid)';
          v_sqlFields2 := ', (SELECT XMLELEMENT("BATCHCOMPONENT", XMLAGG(XMLELEMENT("BATCHCOMPONENT_ROW"
          , XMLELEMENT("ID", p.id)
          , XMLELEMENT("BATCHID", p.batchid)
          , XMLELEMENT("COMPOUNDID", m.compoundid)
          , XMLELEMENT("MIXTURECOMPONENTID", m.mixturecomponentid)
          , XMLELEMENT("COMPONENTINDEX", -m.compoundid)
          , XMLELEMENT("REGNUMBER", r.regnumber)';
                v_sqlBatchComponentPropList := ', (SELECT XMLELEMENT("BATCHCOMPONENTPROPERTYLIST", XMLAGG(XMLELEMENT("BATCHCOMPONENTPROPERTYLIST_ROW", ';
                if (CompoundRegistry.FillPropertyTemplate = TRUE) then
                  v_sqlBatchComponentPropList := 'CompoundRegistry.GetFilledPropertyList(''BATCHCOMPONENT'', p.ID)';
                else
                  v_array := SplitClob(LBatchComponentFields, ',');
                  FOR v_i IN v_array.first..v_array.last
                  LOOP
--                    if v_i <> 1 then
--                      v_sqlBatchComponentPropList := v_sqlBatchComponentPropList;
--                    end if;
--                    v_sqlBatchComponentPropList := v_sqlBatchComponentPropList || 'XMLELEMENT("' || UPPER(v_array(v_i)) || '", ' || v_array(v_i) || ')';
                    v_sqlBatchComponentPropList := v_sqlBatchComponentPropList ||  add_attrib( UPPER(v_array(v_i)),LBatchComponentPickListFields,'b.') || ',';
                     TraceWrite('v_sqlBatchComponentPropList', $$plsql_line,'v_array(v_i)->' ||v_array(v_i)||'-> '|| v_sqlBatchComponentPropList);
                  END LOOP;
                  v_sqlBatchComponentPropList := rtrim(v_sqlBatchComponentPropList,',') || '))) from VW_BatchComponent B WHERE B.ID = p.ID)';
                end if;
                v_sqlBatchComponentFragment := ', (SELECT XMLELEMENT("BATCHCOMPONENTFRAGMENT", XMLAGG(XMLELEMENT("BATCHCOMPONENTFRAGMENT_ROW"
                  , XMLELEMENT("FRAGMENTID", cf.fragmentid)
                  , XMLELEMENT("COMPOUNDFRAGMENTID", bcf.compoundfragmentid)
                  , XMLELEMENT("EQUIVALENT", bcf.equivalent)
                  , XMLELEMENT("ID", bcf.id)
                  ))) from vw_batchcomponentfragment bcf inner join vw_compound_fragment cf on cf.id = bcf.compoundfragmentid where bcf.batchcomponentid = p.ID))';
                v_sqlFields3 := ' order by p.orderindex)) from vw_batchcomponent p inner join vw_mixture_component m on m.mixturecomponentid = p.mixturecomponentid
                    inner join vw_compound c on c.compoundid = m.compoundid
                    inner join vw_registrynumber r on r.regid = c.regid
                  where p.batchid = b.batchid)
          ) order by b.batchnumber)) from vw_batch b
            inner join vw_registrynumber r on r.regid = b.regid
            left outer join cs_security.people pc on pc.person_id = b.personcreated
            left outer join cs_security.people pr on pr.person_id = b.personregistered
            left outer join cs_security.people pa on pa.person_id = b.personapproved
          where r.regnumber= ''' || ARegNumber || '''';
        TraceWrite('RetrieveMcrrSQL', $$plsql_line, v_sqlFields1 || v_sqlBatchPropList || v_sqlBatchIdentifierList || v_sqlBatchProjectList || v_sqlFields2 || v_sqlBatchComponentPropList || v_sqlBatchComponentFragment || v_sqlFields3);
        execute immediate v_sqlFields1 || v_sqlBatchPropList || v_sqlBatchIdentifierList || v_sqlBatchProjectList || v_sqlFields2 || v_sqlBatchComponentPropList || v_sqlBatchComponentFragment || v_sqlFields3 into v_LXml;-- using to_char(ARegNumber);
        LXml := v_LXml.getClobVal();
--        LXml := REGEXP_REPLACE(LXml, cXmlDecoration, null);
        TraceWrite('RetrieveMcrrRawFetch', $$plsql_line, LXml);
----        select replace(replace(REPLACE(LXml, cXmlDecoration, null), '<ROWSET>', ''), '</ROWSET>', '') into LXml from dual;
--        LXml := REGEXP_REPLACE(LXml, '<ROWSET>', '');
--        LXml := REGEXP_REPLACE(LXml, '</ROWSET>', '');
--        AddAttribPickListForBatch(LBatchPickListFields, LXml, '<BATCHPROPERTYLIST_ROW>');
--        AddAttribPickListForBatch(LBatchComponentPickListFields, LXml, '<BATCHCOMPONENTPROPERTYLIST_ROW>');
--        TraceWrite('After AddAttribPickListForBatch', $$plsql_line, LXml);
/*        LXml := REGEXP_REPLACE(LXml, '<BATCHPROPERTYLIST_ROW>', '<ROW>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHPROPERTYLIST_ROW>', '</ROW>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHIDENTIFIERLIST_ROW>', '<ROW>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHIDENTIFIERLIST_ROW>', '</ROW>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHPROJECTLIST_ROW>', '<ROW>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHPROJECTLIST_ROW>', '</ROW>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHCOMPONENT_ROW>', '<ROW>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHCOMPONENT_ROW>', '</BATCHCOMPONENT><BATCHCOMPONENT>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHCOMPONENT></ROW>', '</BATCHCOMPONENT></ROW></Batch><Batch>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHIDENTIFIERLIST>', '<IdentifierList>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHIDENTIFIERLIST>', '</IdentifierList>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHPROJECTLIST>', '<ProjectList>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHPROJECTLIST>', '</ProjectList>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHPROPERTYLIST>', '</ROW>'||CHR(10)||'<PropertyList>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHPROPERTYLIST>', '</PropertyList>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHCOMPONENTPROPERTYLIST>', '</ROW><PropertyList>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHCOMPONENTPROPERTYLIST>', '</PropertyList>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHCOMPONENTPROPERTYLIST_ROW>', '<ROW>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHCOMPONENTPROPERTYLIST_ROW>', '</ROW>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHCOMPONENT>', '<BatchComponent>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHCOMPONENT>', '</BatchComponent>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHCOMPONENTFRAGMENT_ROW>', '<ROW>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHCOMPONENTFRAGMENT_ROW>', '</ROW>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHCOMPONENTFRAGMENT>', '<BatchComponentFragment>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHCOMPONENTFRAGMENT>', '</BatchComponentFragment>');
        LXml := REGEXP_REPLACE(LXml, '</ROW></Batch>', '</Batch>');
        LXml := REGEXP_REPLACE(LXml, '</BatchComponent><BatchComponent></BatchComponent>', '</BatchComponent>');*/

        select LResult || '<Batch>' || replace(replace(replace(
        replace(replace(replace(
        replace(replace(replace(
        replace(replace(replace(

        replace(replace(replace(
        replace(replace(replace(
        replace(replace(replace(
        replace(replace(replace(
        replace(replace(replace(replace(replace(REPLACE(LXml, cXmlDecoration, null), '<ROWSET>', ''), '</ROWSET>', ''),'<BATCHPROPERTYLIST_ROW>', '<ROW>'), '</BATCHPROPERTYLIST_ROW>', '</ROW>'), '<BATCHIDENTIFIERLIST_ROW>', '<ROW>')
        , '</BATCHIDENTIFIERLIST_ROW>', '</ROW>'), '<BATCHPROJECTLIST_ROW>', '<ROW>'), '</BATCHPROJECTLIST_ROW>', '</ROW>')
        , '<BATCHCOMPONENT_ROW>', '<ROW>'), '</BATCHCOMPONENT_ROW>', '</BATCHCOMPONENT><BATCHCOMPONENT>'), '</BATCHCOMPONENT></ROW>', '</BATCHCOMPONENT></ROW></Batch><Batch>')
        , '<BATCHIDENTIFIERLIST>', '<IdentifierList>'), '</BATCHIDENTIFIERLIST>', '</IdentifierList>'), '<BATCHPROJECTLIST>', '<ProjectList>')
        , '</BATCHPROJECTLIST>', '</ProjectList>'), '<BATCHPROPERTYLIST>', '</ROW>'||CHR(10)||'<PropertyList>'), '</BATCHPROPERTYLIST>', '</PropertyList>')

        , '<BATCHCOMPONENTPROPERTYLIST>', '</ROW><PropertyList>'), '</BATCHCOMPONENTPROPERTYLIST>', '</PropertyList>'), '<BATCHCOMPONENTPROPERTYLIST_ROW>', '<ROW>')
, '</BATCHCOMPONENTPROPERTYLIST_ROW>', '</ROW>'), '<BATCHCOMPONENT>', '<BatchComponent>'), '</BATCHCOMPONENT>', '</BatchComponent>')
, '<BATCHCOMPONENTFRAGMENT_ROW>', '<ROW>'), '</BATCHCOMPONENTFRAGMENT_ROW>', '</ROW>'), '<BATCHCOMPONENTFRAGMENT>', '<BatchComponentFragment>')
, '</BATCHCOMPONENTFRAGMENT>', '</BatchComponentFragment>'), '</ROW></Batch>', '</Batch>'), '</BatchComponent><BatchComponent></BatchComponent>', '</BatchComponent>')
        into LResult
        from dual;

        TraceWrite('GetBatchBlock', $$plsql_line, LResult);
      END IF;

      select REPLACE(LResult || '</MultiCompoundRegistryRecord>', '<Batch></MultiCompoundRegistryRecord>', '</MultiCompoundRegistryRecord>')
      into LResult from dual;
      TraceWrite('RetrieveMcrr_0_RawFetch', $$plsql_line, LResult);

      SELECT xml_n, (xml_n).GetClobVal() into LXmlResult, AXml from
      (SELECT
      XmlTransform(Xmltype(LResult), LXslt)
      as xml_n
        FROM DUAL);
      TraceWrite('RetrieveMcrr_1_Transformed', $$plsql_line, AXml);


      AddTags(LCoeObjectConfigField, LXmlResult, 'AddIns',Null);
      AddTags(LCoeObjectConfigField, LXmlResult, 'ValidationRuleList','name');

      AXml:= TakeOnAndGetXml(LXmlResult.GetClobVal(), 'STRUCTURE', LStructuresList);
      AXml:= TakeOnAndGetXml(AXml, 'STRUCTUREAGGREGATION', LStructureAggregationList);
      AXml:= TakeOnAndGetXml(AXml, 'NORMALIZEDSTRUCTURE', LNormalizedStructureList);

    ELSE
      --Validate xml template with the CreateXml object and get it.
      AXml := VRegObjectTemplate.GetClobVal();
      AXml:=
        '<MultiCompoundRegistryRecord ModuleName="''" SameBatchesIdentity="' || VSameBatchesIdentity || '" ActiveRLS="'||vActiveRLS||'" '
        ||Substr(AXml,29,Length(AXml));
    END IF;

    TraceWrite('RetrieveMcrr', $$plsql_line, AXml);
    TraceWrite( act||'RetrieveMultiCompoundRegistry_ended', $$plsql_line,'end' );
  END;


  PROCEDURE RetrieveMultiCompRegistry_old(
    ARegNumber in VW_RegistryNumber.RegNumber%type
    , AXml out NOCOPY clob
    , ASectionsList in Varchar2 := NULL
  ) IS
    LResult                   CLOB;
    LXml                      CLOB;
    LXmlTables                XmlType;
    LXmlResult                XmlType;
    LStructuresList           CLOB;
    LNormalizedStructureList  CLOB;
    LStructureAggregationList CLOB;
    LRegID                    reg_numbers.reg_id%type;
    LCompundRegID             reg_numbers.reg_id%type;
    LCoeObjectConfigField     XmlType;
    LXslt XmlType := XslMcrrFetch;
    v_propertyListMetadata XMLTYPE;
    -- we will ALWAYS override the value for v_propertyFields, so this is just CLOB initialization
    v_propertyFields CLOB := '*';
    v_sqlFields1 varchar2(4000);
    v_sqlFields2 varchar2(4000);
    v_sqlFields3 varchar2(4000);
    v_sqlBatchPropList varchar2(4000);
    v_sqlBatchIdentifierList varchar2(4000);
    v_sqlBatchProjectList varchar2(4000);
    v_sqlBatchComponentPropList varchar2(4000);
    v_sqlBatchComponentFragment varchar2(4000);
    v_array                   t_array;
    v_LXml  XMLType;
    v_ctx dbms_xmlgen.ctxHandle;
    v_queryOutput CLOB;
    v_node XMLTYPE;
    LBatchFields CLOB;
    LBatchPickListFields CLOB;
    LBatchComponentFields CLOB;
    LBatchComponentPickListFields CLOB;
    lstrIsBatchEditable varchar2(80);
    CURSOR C_CompoundRegIDs(ARegID in VW_RegistryNumber.regid%type) IS
      SELECT C.RegID
      FROM VW_Mixture M,VW_Mixture_Component MC, VW_Compound C
      WHERE M.MixtureID=MC.MixtureID AND C.CompoundID=MC.CompoundID AND M.RegID=ARegID ORDER BY C.RegID;

  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'RetrieveMultiCompoundRegistry_started', $$plsql_line,'start' );
    SetSessionParameter;

    --Get Query or Get empty template xml
    IF (ARegNumber IS NOT NULL) THEN
      LCoeObjectConfigField := VRegObjectTemplate;

      --Build a string containing the Mixture-level block
      IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Mixture'), 0) > 1 ) THEN
        LResult :=
          '<MultiCompoundRegistryRecord '
          ||'SameBatchesIdentity="'||VSameBatchesIdentity
          ||'" ActiveRLS="'||vActiveRLS
          ||'" IsEditable="'||GetIsEditable(ARegNumber)
          ||'" IsRegistryDeleteable="'||GetIsRegistryDeleteable(ARegNumber)
          ||'" TypeRegistryRecord="Mixture">';

        LXml := GetMixtureBlock(ARegNumber, LStructureAggregationList, LRegID, LCoeObjectConfigField);
        LResult := LResult || LXml;
      ELSE
        LResult:='<MultiCompoundRegistryRecord TypeRegistryRecord="WithoutMixture">';
      END IF;

      --Continue building the string with all the Compound-level blocks
      IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Compound'), 0) > 0) THEN
        IF (ASectionsList IS NULL) OR INSTR(ASectionsList,'Mixture') <> 0 THEN
          FOR R_CompoundRegIDs IN C_CompoundRegIDs(LRegID) LOOP
            LXml := GetCompoundBlock(R_CompoundRegIDs.RegID, LNormalizedStructureList, LCoeObjectConfigField, ASectionsList);
            LResult := LResult || LXml;
          END LOOP;
        ELSE
          SELECT RegID INTO LCompundRegID
          FROM VW_RegistryNumber
          WHERE RegNumber = ARegNumber;

          LXml := GetCompoundBlock(LCompundRegID, LNormalizedStructureList, LCoeObjectConfigField, ASectionsList);
          LResult := LResult || LXml;
        END IF;

        LStructuresList := TakeOffAndGetClobslist(LResult, '<STRUCTURE>', NULL, Null, FALSE);
      END IF;

      --Continue building the string with all the Batch-level blocks
      IF ( ASectionsList IS NULL ) OR ( NVL(INSTR(ASectionsList,'Batch'), 0) > 0 ) THEN
        -- PropertyList
        if (CompoundRegistry.FillPropertyTemplate = FALSE) then
          GetPropertyListFields('Batch', LCoeObjectConfigField, LBatchFields, LBatchPickListFields);
          TraceWrite('GetPropertyListFields->LBatchFields', $$plsql_line, LBatchFields);
          TraceWrite('GetPropertyListFields->LBatchPickListFields', $$plsql_line, LBatchPickListFields);
          GetPropertyListFields('BatchComponent', LCoeObjectConfigField, LBatchComponentFields, LBatchComponentPickListFields);
          TraceWrite('GetPropertyListFields->LBatchComponentFields', $$plsql_line, LBatchComponentFields);
          TraceWrite('GetPropertyListFields->LBatchComponentPickListFields', $$plsql_line, LBatchComponentPickListFields);
        end if;

    IF upper(vActiveRLS)='OFF' THEN
      lstrIsBatchEditable := q'['True']';
    else
       lstrIsBatchEditable := ' CompoundRegistry.GetIsBatchEditable(''' || ARegNumber || ''', b.batchnumber)';
    END IF;

        v_sqlFields1 :=
          'SELECT XMLELEMENT("ROWSET", XMLAGG(XMLELEMENT("ROW"
          , XMLELEMENT("BATCHID", b.batchid)
          , XMLELEMENT("FULLREGNUMBER", b.fullregnumber)
          , XMLELEMENT("BATCHNUMBER", b.batchnumber)
          , XMLELEMENT("STATUSID", b.statusid)
          , XMLELEMENT("DATECREATED", b.datecreated)
          , XMLELEMENT("PERSONCREATED", b.personcreated)
          , XMLELEMENT("PERSONAPPROVED", b.personapproved)
          , XMLELEMENT("PERSONREGISTERED", b.personregistered)
          , XMLELEMENT("DATELASTMODIFIED", b.datelastmodified)
          , XMLELEMENT("PERSONCREATEDDISPLAY", pc.user_id)
          , XMLELEMENT("PERSONREGISTEREDDISPLAY", pr.user_id)
          , XMLELEMENT("PERSONAPPROVEDDISPLAY", pa.user_id)
          , XMLELEMENT("IsBatchEditable",'||lstrIsBatchEditable||')';
          v_sqlBatchPropList := ', (SELECT XMLELEMENT("BATCHPROPERTYLIST", XMLAGG(XMLELEMENT("BATCHPROPERTYLIST_ROW", ';
          if (CompoundRegistry.FillPropertyTemplate = TRUE) then
            v_sqlBatchPropList := v_sqlBatchPropList || 'CompoundRegistry.GetFilledPropertyList(''BATCH'', b.batchid)';
          else
            v_array := SplitClob(LBatchFields, ',');
            FOR v_i IN v_array.first..v_array.last
            LOOP
              if v_i <> 1 then
                v_sqlBatchPropList := v_sqlBatchPropList || ',';
              end if;
              v_sqlBatchPropList := v_sqlBatchPropList || 'XMLELEMENT("' || UPPER(v_array(v_i)) || '", ' || v_array(v_i) || ')';
            END LOOP;
            v_sqlBatchPropList := v_sqlBatchPropList || '))) from vw_batch v where v.batchid = b.batchid)';
          end if;
          v_sqlBatchIdentifierList := ', (SELECT XMLELEMENT("BATCHIDENTIFIERLIST", XMLAGG(XMLELEMENT("BATCHIDENTIFIERLIST_ROW"
          , XMLELEMENT("ID", bi.id)
          , XMLELEMENT("TYPE", bi.type)
          , XMLELEMENT("VALUE", bi.value)
          , XMLELEMENT("DESCRIPTION", p.description)
          , XMLELEMENT("ACTIVE", p.active)
          , XMLELEMENT("NAME", p.name)
          ))) from vw_batchidentifier bi inner join vw_identifiertype p on p.id = bi.type where bi.batchid = b.batchid)';
          v_sqlBatchProjectList := ', (SELECT XMLELEMENT("BATCHPROJECTLIST", XMLAGG(XMLELEMENT("BATCHPROJECTLIST_ROW"
          , XMLELEMENT("ID", bp.id)
          , XMLELEMENT("PROJECTID", bp.projectid)
          , XMLELEMENT("DESCRIPTION", p.description)
          , XMLELEMENT("ACTIVE", p.active)
          , XMLELEMENT("NAME", p.name)
          ))) from vw_batch_project bp inner join vw_project p on p.projectid = bp.projectid where bp.batchid = b.batchid)';
          v_sqlFields2 := ', (SELECT XMLELEMENT("BATCHCOMPONENT", XMLAGG(XMLELEMENT("BATCHCOMPONENT_ROW"
          , XMLELEMENT("ID", p.id)
          , XMLELEMENT("BATCHID", p.batchid)
          , XMLELEMENT("COMPOUNDID", m.compoundid)
          , XMLELEMENT("MIXTURECOMPONENTID", m.mixturecomponentid)
          , XMLELEMENT("COMPONENTINDEX", -m.compoundid)
          , XMLELEMENT("REGNUMBER", r.regnumber)';
                v_sqlBatchComponentPropList := ', (SELECT XMLELEMENT("BATCHCOMPONENTPROPERTYLIST", XMLAGG(XMLELEMENT("BATCHCOMPONENTPROPERTYLIST_ROW", ';
                if (CompoundRegistry.FillPropertyTemplate = TRUE) then
                  v_sqlBatchComponentPropList := 'CompoundRegistry.GetFilledPropertyList(''BATCHCOMPONENT'', p.ID)';
                else
                  v_array := SplitClob(LBatchComponentFields, ',');
                  FOR v_i IN v_array.first..v_array.last
                  LOOP
                    if v_i <> 1 then
                      v_sqlBatchComponentPropList := v_sqlBatchComponentPropList || ',';
                    end if;
                    v_sqlBatchComponentPropList := v_sqlBatchComponentPropList || 'XMLELEMENT("' || UPPER(v_array(v_i)) || '", ' || v_array(v_i) || ')';
                  END LOOP;
                  v_sqlBatchComponentPropList := v_sqlBatchComponentPropList || '))) from VW_BatchComponent B WHERE B.ID = p.ID)';
                end if;
                v_sqlBatchComponentFragment := ', (SELECT XMLELEMENT("BATCHCOMPONENTFRAGMENT", XMLAGG(XMLELEMENT("BATCHCOMPONENTFRAGMENT_ROW"
                  , XMLELEMENT("FRAGMENTID", cf.fragmentid)
                  , XMLELEMENT("COMPOUNDFRAGMENTID", bcf.compoundfragmentid)
                  , XMLELEMENT("EQUIVALENT", bcf.equivalent)
                  , XMLELEMENT("ID", bcf.id)
                  ))) from vw_batchcomponentfragment bcf inner join vw_compound_fragment cf on cf.id = bcf.compoundfragmentid where bcf.batchcomponentid = p.ID))';
                v_sqlFields3 := ' order by p.orderindex)) from vw_batchcomponent p inner join vw_mixture_component m on m.mixturecomponentid = p.mixturecomponentid
                    inner join vw_compound c on c.compoundid = m.compoundid
                    inner join vw_registrynumber r on r.regid = c.regid
                  where p.batchid = b.batchid)
          ) order by b.batchnumber)) from vw_batch b
            inner join vw_registrynumber r on r.regid = b.regid
            left outer join cs_security.people pc on pc.person_id = b.personcreated
            left outer join cs_security.people pr on pr.person_id = b.personregistered
            left outer join cs_security.people pa on pa.person_id = b.personapproved
          where r.regnumber= ''' || ARegNumber || '''';
        TraceWrite('RetrieveMcrrSQL', $$plsql_line, v_sqlFields1 || v_sqlBatchPropList || v_sqlBatchIdentifierList || v_sqlBatchProjectList || v_sqlFields2 || v_sqlBatchComponentPropList || v_sqlBatchComponentFragment || v_sqlFields3);
        execute immediate v_sqlFields1 || v_sqlBatchPropList || v_sqlBatchIdentifierList || v_sqlBatchProjectList || v_sqlFields2 || v_sqlBatchComponentPropList || v_sqlBatchComponentFragment || v_sqlFields3 into v_LXml;-- using to_char(ARegNumber);
        LXml := v_LXml.getClobVal();
        LXml := REGEXP_REPLACE(LXml, cXmlDecoration, null);
        TraceWrite('RetrieveMcrrRawFetch', $$plsql_line, LXml);
        LXml := REGEXP_REPLACE(LXml, '<ROWSET>', '');
        LXml := REGEXP_REPLACE(LXml, '</ROWSET>', '');
        AddAttribPickListForBatch(LBatchPickListFields, LXml, '<BATCHPROPERTYLIST_ROW>');
        AddAttribPickListForBatch(LBatchComponentPickListFields, LXml, '<BATCHCOMPONENTPROPERTYLIST_ROW>');
        TraceWrite('After AddAttribPickListForBatch', $$plsql_line, LXml);
/*        LXml := REGEXP_REPLACE(LXml, '<BATCHPROPERTYLIST_ROW>', '<ROW>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHPROPERTYLIST_ROW>', '</ROW>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHIDENTIFIERLIST_ROW>', '<ROW>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHIDENTIFIERLIST_ROW>', '</ROW>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHPROJECTLIST_ROW>', '<ROW>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHPROJECTLIST_ROW>', '</ROW>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHCOMPONENT_ROW>', '<ROW>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHCOMPONENT_ROW>', '</BATCHCOMPONENT><BATCHCOMPONENT>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHCOMPONENT></ROW>', '</BATCHCOMPONENT></ROW></Batch><Batch>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHIDENTIFIERLIST>', '<IdentifierList>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHIDENTIFIERLIST>', '</IdentifierList>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHPROJECTLIST>', '<ProjectList>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHPROJECTLIST>', '</ProjectList>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHPROPERTYLIST>', '</ROW>'||CHR(10)||'<PropertyList>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHPROPERTYLIST>', '</PropertyList>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHCOMPONENTPROPERTYLIST>', '</ROW><PropertyList>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHCOMPONENTPROPERTYLIST>', '</PropertyList>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHCOMPONENTPROPERTYLIST_ROW>', '<ROW>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHCOMPONENTPROPERTYLIST_ROW>', '</ROW>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHCOMPONENT>', '<BatchComponent>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHCOMPONENT>', '</BatchComponent>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHCOMPONENTFRAGMENT_ROW>', '<ROW>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHCOMPONENTFRAGMENT_ROW>', '</ROW>');
        LXml := REGEXP_REPLACE(LXml, '<BATCHCOMPONENTFRAGMENT>', '<BatchComponentFragment>');
        LXml := REGEXP_REPLACE(LXml, '</BATCHCOMPONENTFRAGMENT>', '</BatchComponentFragment>');
        LXml := REGEXP_REPLACE(LXml, '</ROW></Batch>', '</Batch>');
        LXml := REGEXP_REPLACE(LXml, '</BatchComponent><BatchComponent></BatchComponent>', '</BatchComponent>');*/

        select replace(replace(replace(
        replace(replace(replace(
        replace(replace(replace(
        replace(replace(replace(

        replace(replace(replace(
        replace(replace(replace(
        replace(replace(replace(
        replace(replace(replace(
        replace(replace(replace(LXml,'<BATCHPROPERTYLIST_ROW>', '<ROW>'), '</BATCHPROPERTYLIST_ROW>', '</ROW>'), '<BATCHIDENTIFIERLIST_ROW>', '<ROW>')
        , '</BATCHIDENTIFIERLIST_ROW>', '</ROW>'), '<BATCHPROJECTLIST_ROW>', '<ROW>'), '</BATCHPROJECTLIST_ROW>', '</ROW>')
        , '<BATCHCOMPONENT_ROW>', '<ROW>'), '</BATCHCOMPONENT_ROW>', '</BATCHCOMPONENT><BATCHCOMPONENT>'), '</BATCHCOMPONENT></ROW>', '</BATCHCOMPONENT></ROW></Batch><Batch>')
        , '<BATCHIDENTIFIERLIST>', '<IdentifierList>'), '</BATCHIDENTIFIERLIST>', '</IdentifierList>'), '<BATCHPROJECTLIST>', '<ProjectList>')
        , '</BATCHPROJECTLIST>', '</ProjectList>'), '<BATCHPROPERTYLIST>', '</ROW>'||CHR(10)||'<PropertyList>'), '</BATCHPROPERTYLIST>', '</PropertyList>')

        , '<BATCHCOMPONENTPROPERTYLIST>', '</ROW><PropertyList>'), '</BATCHCOMPONENTPROPERTYLIST>', '</PropertyList>'), '<BATCHCOMPONENTPROPERTYLIST_ROW>', '<ROW>')
, '</BATCHCOMPONENTPROPERTYLIST_ROW>', '</ROW>'), '<BATCHCOMPONENT>', '<BatchComponent>'), '</BATCHCOMPONENT>', '</BatchComponent>')
, '<BATCHCOMPONENTFRAGMENT_ROW>', '<ROW>'), '</BATCHCOMPONENTFRAGMENT_ROW>', '</ROW>'), '<BATCHCOMPONENTFRAGMENT>', '<BatchComponentFragment>')
, '</BATCHCOMPONENTFRAGMENT>', '</BatchComponentFragment>'), '</ROW></Batch>', '</Batch>'), '</BatchComponent><BatchComponent></BatchComponent>', '</BatchComponent>')
        into LXml
        from dual;



        LResult := LResult || '<Batch>' || LXml;
        TraceWrite('GetBatchBlock', $$plsql_line, LResult);
      END IF;

      LResult := LResult || '</MultiCompoundRegistryRecord>';
      LResult := REGEXP_REPLACE(LResult, '<Batch></MultiCompoundRegistryRecord>', '</MultiCompoundRegistryRecord>');
      TraceWrite('RetrieveMcrr_0_RawFetch', $$plsql_line, LResult);
      LXmlTables := XmlType.CreateXml(LResult);

      --Transform to consumer-friendly xml
      SELECT XmlTransform(LXmlTables, LXslt).GetClobVal()
      INTO AXml FROM DUAL;
      TraceWrite('RetrieveMcrr_1_Transformed', $$plsql_line, AXml);

      LXmlResult := XmlType(AXml);
      AddTags(LCoeObjectConfigField, LXmlResult, 'AddIns',Null);
      AddTags(LCoeObjectConfigField, LXmlResult, 'ValidationRuleList','name');

      AXml:= TakeOnAndGetXml(LXmlResult.GetClobVal(), 'STRUCTURE', LStructuresList);
      AXml:= TakeOnAndGetXml(AXml, 'STRUCTUREAGGREGATION', LStructureAggregationList);
      AXml:= TakeOnAndGetXml(AXml, 'NORMALIZEDSTRUCTURE', LNormalizedStructureList);

    ELSE
      --Validate xml template with the CreateXml object and get it.
      AXml := VRegObjectTemplate.GetClobVal();
      AXml:=
        '<MultiCompoundRegistryRecord ModuleName="''" SameBatchesIdentity="' || VSameBatchesIdentity || '" ActiveRLS="'||vActiveRLS||'" '
        ||Substr(AXml,29,Length(AXml));
    END IF;

    TraceWrite('RetrieveMcrr', $$plsql_line, AXml);
    TraceWrite( act||'RetrieveMultiCompoundRegistry_ended', $$plsql_line,'end' );
  END;

  -- This SPROC has a total of 100 local vars and constants.
  /**
  Look over a Xml searching each Table and insert the rows on it.
  -->author Fari
  -->since 07-Mar-07
  */
  PROCEDURE UpdateMultiCompoundRegistry(
    AXml in CLOB
    , AMessage OUT CLOB
    , ADuplicateCheck Char:='C'
    , AConfigurationID Number:=1
    , ASectionsList Varchar2 := NULL
  ) IS
    LInsert varchar2(10);
    LDelete varchar2(10);

    LinsCtx                     DBMS_XMLSTORE.ctxType;
    LXslTablesTransformed       XmlType;
    LXmlCompReg CLOB := AXml;
    LXmlRows                    CLOB;
    LXmlTables xmltype;
    LXmlTypeRows                XmlType;
    LFieldToUpdate              CLOB;
    LStructureUpdating          CLOB;
    LRowsUpdated                Number:=0;
    LRowsProcessed              Number:=0;

    LIndex                      Number:=0;
    LIndexField                 Number:=0;
    LRowsInserted               Number:=0;
    LTableName                  CLOB;
    LFieldName                  CLOB;
    LBriefMessage               CLOB;
    LMessage                    Clob := NULL;
    LUpdate                     boolean := FALSE;
    LSomeUpdate                 boolean := FALSE;
    LSectionInsert              boolean;
    LSectionDelete              boolean;

    LRegID                      Number:=0;
    LRegIDTag CONSTANT          VARCHAR2(10):='<REGID>';
    LRegIDTagEnd CONSTANT       VARCHAR2(10):='</REGID>';

    LBatchID                    Number:=0;
    LBatchIDTag CONSTANT        VARCHAR2(10):='<BATCHID>';
    LBatchIDTagEnd CONSTANT     VARCHAR2(10):='</BATCHID>';

    LCompoundID                 Number:=0;
    LCompoundIDAux              Number:=0;
    LCompoundIDTemp             Number:=0;
    LCompoundIDTag CONSTANT     VARCHAR2(15):='<COMPOUNDID>';
    LCompoundIDTagEnd CONSTANT  VARCHAR2(15):='</COMPOUNDID>';

    LFragmentID                 Number:=0;

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
    LStructureDrawingType               Number:=0;
    LStructureIDTag CONSTANT            VARCHAR2(15):='<STRUCTUREID>';
    LStructureIDTagEnd CONSTANT         VARCHAR2(15):='</STRUCTUREID>';

    LMixtureComponentID                 Number:=0;
    LMixtureComponentIDTag CONSTANT     VARCHAR2(25):='<MIXTURECOMPONENTID>';
    LMixtureComponentIDTagEnd CONSTANT  VARCHAR2(25):='</MIXTURECOMPONENTID>';

    LBatchNumber                     Number:=0;
    LBatchNumberTag CONSTANT         VARCHAR2(15):='<BATCHNUMBER>';
    LBatchNumberTagEnd CONSTANT      VARCHAR2(15):='</BATCHNUMBER>';

    LRegNumber                       VW_REGISTRYNUMBER.RegNumber%Type;
    LRegNumberAux                    VW_REGISTRYNUMBER.RegNumber%Type;
    LRegNumberTag CONSTANT           VARCHAR2(15):='<REGNUMBER>';
    LRegNumberTagEnd CONSTANT        VARCHAR2(15):='</REGNUMBER>';

    LFullRegNumber                   VW_REGISTRYNUMBER.RegNumber%Type;
    LFullRegNumberTag CONSTANT       VARCHAR2(20):='<FULLREGNUMBER>';
    LFullRegNumberTagEnd CONSTANT VARCHAR2(20):='</FULLREGNUMBER>';

    LSequenceNumber                  VW_REGISTRYNUMBER.SequenceNumber%Type;
    LSequenceNumberTag CONSTANT      VARCHAR2(20):='<SEQUENCENUMBER>';
    LSequenceNumberTagEnd CONSTANT   VARCHAR2(20):='</SEQUENCENUMBER>';

    LMixtureRegID                    Number := 0;
    LMixtureID                       Number := 0;
    LMixtureIDTag CONSTANT           VARCHAR2(15):='<MIXTUREID>';
    LMixtureIDTagEnd CONSTANT        VARCHAR2(15):='</MIXTUREID>';

    LComponentID                     Number := 0;
    Larr_idc                                number;
    Larr_idc1                                number;
    Larr_idv                                number;
    source_cursor                       INTEGER;

    Lclobtable                           T_CLOB_ARRAY;
    Lvartable                           T_VAR_ARRAY;
    Lclobtable1                           T_CLOB_ARRAY;
    Lvartable1                           T_VAR_ARRAY;

    LStructureValue                  CLOB;
    LStructuresList                  CLOB;
    LStructuresToValidateList        CLOB;
    LFragmentXmlValue                CLOB;
    LNormalizedStructureList         CLOB;
    LNormalizedStructureValue        CLOB;
    LStructureAggregationList        CLOB;
    LStructureAggregationValue       CLOB;
    LXMLRegistryRecord               CLOB;

    LDuplicatedCompoundID            Number;
    LDuplicatedStructures            CLOB;
    LListDulicatesCompound           CLOB;
    LDuplicateComponentCount         Number:=0;
    LStructureIDToValidate           Number;
    LRegIdsValue                     Varchar2(4000);
    LDuplicatedMixtureRegIds         Varchar2(4000);
    LDuplicatedMixtureCount          Number;
    LMixtureIDAux                    Varchar2(20);
    LXMLCompound                     XmlType;
    LXMLFragmentEquivalent           XmlType;
    LXMLRegNumberDuplicated          XmlType;
    LIDTodelete                      Number;
    LExistentComponentIndex          Number:=0;
    LSequenceID                      Number:=0;
    LRegIDAux                        Number:=0;
    LDuplicatedAuxStructureID        Number:=0;
    LStructureIDToUpdate             Number;
    LRLSState                        Boolean;
    LMixtureSetting                  Varchar2(5);
    LCountComponent                  Number;
    LXslTables xmltype := XslMcrrUpdate;
    lstmt                            Varchar2(500);
    lstmt2                           CLOB;
    lstmt3                           CLOB;
    lstmt4                           CLOB;
    ldata_type                       Varchar2(100);
    lvTableName                      Varchar2(100);
    lout                             number;
    LNewBatchList tNumericIdList;
    mod1 varchar2(100); act varchar2(100);

    --TODO: Why not just updatexml?!?
    PROCEDURE SetKeyValue(AID VARCHAR2, AIDTag VARCHAR2, AIDTagEnd VARCHAR2) IS
      LPosTag number := 0;
      LPosTagNull number := 0;
      LPosTagEnd number := 0;
    mod1 varchar2(100); act varchar2(100);
    BEGIN
      dbms_application_info.read_module(mod1, act); TraceWrite( act||'SetKeyValue_started', $$plsql_line,'start' );
      LPosTag := 1;
      LOOP
        LPosTagNull := INSTR(LXmlRows,SUBSTR(AIDTag,1,LENGTH(AIDTag)-1)||'/>',LPosTag);
        IF LPosTagNull<>0 THEN
            LXmlRows:=REPLACE(LXmlRows,SUBSTR(AIDTag,1,LENGTH(AIDTag)-1)||'/>',AIDTag||AIDTagEnd);
        END IF;
        LPosTag := INSTR(LXmlRows,AIDTag,LPosTag);
      EXIT WHEN LPosTag = 0;
        LPosTag := LPosTag + LENGTH(AIDTag)- 1;
        LPosTagEnd := INSTR(LXmlRows,AIDTagEnd,LPosTag);
        LXmlRows := SUBSTR(LXmlRows,1,LPosTag)||AID||SUBSTR(LXmlRows,LPosTagEnd,LENGTH(LXmlRows));
        TraceWrite('UpdateMcrr_SetKeyValue_' || AIDTag, $$plsql_line, LXmlRows);
      END LOOP;
      TraceWrite( act||'SetKeyValue_ended', $$plsql_line,'end' );
    END;

  begin
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'UpdateMultiCompoundRegistry_started', $$plsql_line,'start' );
    SetSessionParameter;

    TraceWrite('UpdateMcrr_0_AXml', $$plsql_line, AXml);

    -- Take Out the Structures because the nodes of XmlType don't suport a size grater 64k.
    --LFragmentXmlList:=TakeOffAndGetClobsList(LXmlCompReg,'<Structure ','Structure','<Fragment>');
    LStructuresList:=TakeOffAndGetClobsList(LXmlCompReg,'<Structure ','Structure','<BaseFragment>',NULL,FALSE);
    LStructuresToValidateList:=LStructuresList;
    LNormalizedStructureList:=TakeOffAndGetClobslist(LXmlCompReg,'<NormalizedStructure',NULL,NULL,TRUE,FALSE);
    LStructureAggregationList:=TakeOffAndGetClobslist(LXmlCompReg,'<StructureAggregation',NULL,NULL,TRUE,TRUE);

    -- Convert the remaining clob to XmlType
    LXmlTables := XmlType.createXML(LXmlCompReg);

    -- START PATCH
    -- Get the reg number for retrieval of the record as part of the output message
    SELECT
      extractValue(LXmlTables,'/MultiCompoundRegistryRecord/RegNumber/RegNumber')
      , extractValue(LXmlTables, '/MultiCompoundRegistryRecord/ID')
    INTO LRegNumber, LMixtureIDAux FROM dual;
    -- END PATCH

    -- Ensure that the batches accomodate the SameBatchesIdentify flag
    ValidateIdentityBetweenBatches(LXmlTables);

    IF ( UPPER(GetDuplicateCheckEnable) = 'TRUE' ) THEN
      IF ( Upper(ADuplicateCheck)='C' ) THEN
        -- Validate Components Strcuture
        LIndex := 0;
        LOOP
          -- Extract each component
          LIndex := LIndex+1;
          SELECT extract(
            LXmlTables
            , '/MultiCompoundRegistryRecord/ComponentList/Component['||LIndex||']'
          ) INTO LXMLCompound FROM dual;
        EXIT WHEN LXMLCompound IS NULL;
          -- bypass duplicate-checking if structure is to be deleted
          SELECT extract(
            LXMLCompound
            , '/Component/Compound/BaseFragment/Structure/Structure[@update="yes" or @insert="yes"]/text()'
          ).getClobVal() INTO LStructureUpdating FROM dual;

          LStructureValue := TakeOffAndGetClob(LStructuresToValidateList,'Clob');

          IF LStructureValue IS NOT NULL AND INSTR(UPPER(LXMLCompound.getClobVal()),'DELETE="YES"')=0 AND LStructureUpdating IS NOT NULL THEN
            SELECT extractValue(
              LXMLCompound
              , '/Component/Compound/BaseFragment/Structure/StructureID'
            ) INTO LDuplicatedAuxStructureID FROM dual;

            IF NVL(LDuplicatedAuxStructureID,0)>=0 THEN
              IF ValidateWildcardStructure(LStructureValue) THEN
                Select
                  extractValue(LXMLCompound, '/Component/Compound/BaseFragment/Structure/StructureID')
                  , extractValue(LXMLCompound,'/Component/ComponentIndex')
                INTO LStructureIDToValidate,LExistentComponentIndex FROM dual;

                SELECT extract(
                  LXmlTables
                  ,'/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex='||LExistentComponentIndex||']/BatchComponentFragmentList'
                ) INTO LXMLFragmentEquivalent FROM dual;

                LDuplicatedStructures:=ValidateCompoundMulti(LStructureValue,LStructureIDToValidate, AConfigurationID, LXMLCompound,LXMLFragmentEquivalent, LRegNumber);

                IF LDuplicatedStructures IS NOT NULL AND LDuplicatedStructures<>'<REGISTRYLIST></REGISTRYLIST>'THEN
                  SELECT extractValue(LXMLCompound,'/Component/Compound/CompoundID')
                  INTO LDuplicatedCompoundID FROM dual;

                  LListDulicatesCompound:=LListDulicatesCompound||'<COMPOUND>'||'<TEMPCOMPOUNDID>'||LDuplicatedCompoundID||'</TEMPCOMPOUNDID>'||LDuplicatedStructures||'</COMPOUND>';
                  LDuplicateComponentCount:=LDuplicateComponentCount+1;
                END IF;

              END IF;
            END IF;
          END IF;
        END LOOP;

        IF LListDulicatesCompound IS NOT NULL THEN
          LListDulicatesCompound:='<COMPOUNDLIST>'||LListDulicatesCompound||'</COMPOUNDLIST>';
          IF LDuplicateComponentCount=1 THEN
            BEGIN
              AMessage := CreateRegistrationResponse('1 duplicated component.', LListDulicatesCompound, NULL);
            END;
            RETURN;
          ELSE
            BEGIN
              AMessage := CreateRegistrationResponse(to_char(LDuplicateComponentCount) || ' duplicated components.', LListDulicatesCompound, NULL);
            END;
            RETURN;
          END IF;
        END IF;
      END IF;

      IF (Upper(ADuplicateCheck)='M') OR (Upper(ADuplicateCheck)='C') THEN
        SELECT XmlTransform(
          extract(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[((@delete!=''yes'') or (string-length(@delete)=0))]/Compound/RegNumber/RegID'),XmlType.CreateXml('
          <xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
            <xsl:template match="/RegID">
              <xsl:for-each select="."><xsl:value-of select="."/>,</xsl:for-each>
            </xsl:template>
          </xsl:stylesheet>')
        ).GetClobVal()
        INTO LRegIdsValue FROM dual;

        LRegIdsValue:=SUBSTR(LRegIdsValue,1,Length(LRegIdsValue)-1);

        LDuplicatedMixtureRegIds := ValidateMixture(LRegIdsValue,LDuplicatedMixtureCount,LMixtureIDAux,null,LXmlTables);

        IF LDuplicatedMixtureRegIds IS NOT NULL THEN
          IF LDuplicatedMixtureCount > 1 THEN
            BEGIN
              AMessage := CreateRegistrationResponse(to_char( LDuplicatedMixtureCount ) || ' duplicated mixtures.', LDuplicatedMixtureRegIds, NULL);
            END;
            RETURN;
          ELSE
            BEGIN
              AMessage := CreateRegistrationResponse('1 duplicated mixture.', LDuplicatedMixtureRegIds, NULL);
            END;
            RETURN;
          END IF;
        END IF;

      END IF;
    END IF; -- end of duplicate-checking block

    LBriefMessage := 'Compound Validation OK';
    LMessage := LMessage || LBriefMessage ||CHR(13);

    --Build a new formatted Xml
    SELECT XmlTransform(LXmlTables,LXslTables)
    INTO LXslTablesTransformed FROM DUAL;

    TraceWrite('UpdateMcrr_LXslTablesTransformed', $$plsql_line, LXslTablesTransformed.getClobVal());
    LStructureValue := null;

    LIndex := 0;
    --Look over Xml searching each Table and update the rows of it.

    for cur_main in (with a as
           (select LXslTablesTransformed as xml  FROM dual)
           select z.tt.getClobVal() as clob_var, z.tt as xml_var, z.tt.getRootElement() as table_name,
           z.cur_insert,
           z.cur_delete
           from a, xmltable('for $i in /MultiCompoundRegistryRecord/node() where $i != "" return $i'--'for $i in /MultiCompoundRegistryRecord/node() where $i/@insert != "" or $i/@delete != "" return $i'
           passing (a.xml)
                columns tt   XMLtype path '/*'
                ,cur_insert clob path '/node()/@insert'
                ,cur_delete clob path '/node()/@delete'
           ) z )
    LOOP
      --Search each Table
--      LIndex := LIndex+1;
--      SELECT LXslTablesTransformed.extract('/MultiCompoundRegistryRecord/node()['||LIndex||']').getClobVal()
--      INTO LXmlRows FROM dual;
     LXmlRows:=cur_main.clob_var;
      TraceWrite('UpdateMcrr_Loop(' || to_char(LIndex) || ')', $$plsql_line, LXmlRows);

--      LXmlTypeRows := XmlType.CreateXml(LXmlRows);
--    EXIT WHEN LXmlRows IS NULL;
      LXmlTypeRows := cur_main.xml_var;

      -- get the table name
--      select t."RootElementName" into LTableName from  xmltable(
--          'for $i in /node()[1] return
--            element RetVal {
--              element RootElementName {name($i)}
--              }'
--          passing LXmlTypeRows
--          columns
--            "RootElementName" varchar2(50) path '/RetVal/RootElementName/text()'
--        ) t;
      LTableName:=cur_main.table_name;

--      select
--        extract(LXmlTypeRows, '/node()/@insert').getClobVal()
--        , extract(LXmlTypeRows, '/node()/@delete').getClobVal()
--      into LInsert, LDelete
--      from dual;
      LInsert:=cur_main.cur_insert;
      LDelete:=cur_main.cur_delete;
      IF (LInsert is not null) THEN
        LSectionInsert:=TRUE;
        TraceWrite('UpdateMcrr_insert="yes"', $$plsql_line, LXmlRows);
        TraceWrite('LMixtureID: ', $$plsql_line, to_char(LMixtureID));

      ELSE
        LSectionInsert:=FALSE;
      END IF;

      IF (LDelete is not null) THEN
        LSectionDelete:=TRUE;
        TraceWrite('UpdateMcrr_delete="yes"', $$plsql_line, LXmlRows);
      ELSE
        LSectionDelete:=FALSE;
      END IF;

      IF LSectionDelete THEN
        CASE UPPER(LTableName)

          WHEN 'VW_COMPOUND' THEN
            BEGIN
              SELECT extractvalue(XmlType(LXmlRows),'VW_Compound/ROW/COMPOUNDID')
              INTO LIDTodelete FROM dual;

              DeleteCompound(LIDTodelete, LMixtureID, LMessage);
            END;

          WHEN 'VW_COMPOUND_FRAGMENT' THEN
            BEGIN
              LMixtureSetting := UPPER(GetMixturesEnabled);
              IF LMixtureSetting = 'TRUE' THEN
                SELECT COUNT(1)
                    INTO LCountComponent
                    FROM VW_Mixture_Component MC
                    WHERE MC.MixtureID=LMixtureID;
                IF LCountComponent=1 THEN
                  SELECT extractvalue(XmlType(LXmlRows),'VW_Compound_Fragment/ROW/ID')
                  INTO LIDTodelete FROM dual;

                  DeleteFragment(LIDTodelete, LMessage);
                END IF;
              ELSE
                SELECT extractvalue(XmlType(LXmlRows),'VW_Compound_Fragment/ROW/ID')
                  INTO LIDTodelete FROM dual;

                DeleteFragment(LIDTodelete, LMessage);
              END IF;
            END;


          WHEN 'VW_COMPOUND_IDENTIFIER' THEN
            BEGIN
              SELECT extractvalue(XmlType(LXmlRows),'VW_Compound_Identifier/ROW/ID')
              INTO LIDTodelete FROM dual;

              DeleteIdentifier(LIDTodelete, LMessage);
            END;

          WHEN 'VW_REGISTRYNUMBER_PROJECT' THEN
            BEGIN
            SELECT extractvalue(XmlType(LXmlRows),'VW_RegistryNumber_Project/ROW/ID')
            INTO LIDTodelete FROM dual;

            DeleteRegistryNumberProject(LIDTodelete, LMessage);
          END;

          WHEN 'VW_BATCHIDENTIFIER' THEN
            BEGIN
              SELECT extractvalue(XmlType(LXmlRows),'VW_BatchIdentifier/ROW/ID')
              INTO LIDTodelete FROM dual;

              DeleteBatchIdentifier(LIDTodelete, LMessage);
            END;

          WHEN 'VW_BATCH_PROJECT' THEN
            BEGIN
              SELECT extractvalue(XmlType(LXmlRows),'VW_Batch_Project/ROW/ID')
              INTO LIDTodelete FROM dual;

              DeleteBatchProject(LIDTodelete,LMessage);
            END;
          WHEN 'VW_BATCHCOMPONENTFRAGMENT' THEN
            BEGIN
              LMixtureSetting := UPPER(GetMixturesEnabled);
              IF LMixtureSetting = 'TRUE' THEN
                SELECT COUNT(1)
                    INTO LCountComponent
                    FROM VW_Mixture_Component MC
                    WHERE MC.MixtureID=LMixtureID;
                IF LCountComponent>1 THEN
                  SELECT extractvalue(XmlType(LXmlRows),'VW_BatchComponentFragment/ROW/ID')
                  INTO LIDTodelete FROM dual;

                  DeleteBatchComponentFragment(LIDTodelete,LMessage);
                END IF;
              ELSE
                  SELECT extractvalue(XmlType(LXmlRows),'VW_BatchComponentFragment/ROW/ID')
                  INTO LIDTodelete FROM dual;
                  DeleteBatchComponentFragment(LIDTodelete, LMessage);
              END IF;
            END;
          WHEN 'VW_STRUCTURE' THEN
            BEGIN
                SELECT extractvalue(XmlType(LXmlRows),'VW_Structure/ROW/STRUCTUREID')
                INTO LIDTodelete FROM dual;

                DeleteStructure(LIDTodelete,LMessage);
            END;

          WHEN 'VW_STRUCTURE_IDENTIFIER' THEN
            BEGIN
              SELECT extractvalue(XmlType(LXmlRows),'VW_Structure_Identifier/ROW/ID')
              INTO LIDTodelete FROM dual;

              DeleteStructureIdentifier(LIDTodelete, LMessage);
            END;

          ELSE  LMessage := LMessage || ' "' || LTableName || '" table stranger.' ||CHR(13);

        END CASE;
      ELSIF LSectionInsert THEN
        --Customization for each View - Use of Sequences
        CASE UPPER(LTableName)
          WHEN 'VW_BATCH' THEN
            BEGIN
              -- create a new PK for the Batch and inject its value into the xml
              SELECT SEQ_BATCHES.NEXTVAL INTO LBatchID FROM DUAL;
              SetKeyValue(LBatchID, LBatchIdTag, LBatchIdTagEnd);

              SELECT extractvalue(XmlType(LXmlRows),'VW_Batch/ROW[1]/REGID')
              INTO LMixtureRegID FROM dual;

              LRLSState := RegistrationRLS.GetStateRLS;
              IF LRLSState THEN
                RegistrationRLS.SetEnableRLS(False);
              END IF;

              SELECT NVL(MAX(BatchNumber),0)+1
              INTO LBatchNumber
              FROM VW_Batch
              WHERE REGID = LMixtureRegID;

              IF LRLSState THEN
                RegistrationRLS.SetEnableRLS(LRLSState);
              END IF;

              TraceWrite('UpdateMCRR_ERROR', $$plsql_line, LXmlRows);
              SELECT extractvalue(XmlType(LXmlRows),'VW_Batch/ROW/FULLREGNUMBER')
              INTO LFullRegNumber FROM dual;

              IF ( NVL(LFullRegNumber, 'null') = 'null') THEN
                LNewBatchList(LBatchNumber) := LBatchID;
              END IF;

              SetKeyValue(LBatchNumber,LBatchNumberTag,LBatchNumberTagEnd);
              SetKeyValue(SYSDATE,'<DATECREATED>','</DATECREATED>');
              SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
              InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
            END;

          WHEN 'VW_BATCH_PROJECT' THEN
            BEGIN
              IF NVL(LBatchID,0)<>0 THEN
                SetKeyValue(LBatchID,LBatchIdTag,LBatchIdTagEnd);
              END IF;
              InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
            END;

          WHEN 'VW_BATCHIDENTIFIER' THEN
            BEGIN
              IF NVL(LBatchID,0)<>0 THEN
                SetKeyValue(LBatchID,LBatchIdTag,LBatchIdTagEnd);
              END IF;

              InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
            END;

          WHEN 'VW_BATCHCOMPONENT' THEN
            BEGIN
              SELECT SEQ_BATCHCOMPONENT.NEXTVAL INTO LBatchComponentID FROM DUAL;
              SetKeyValue(LBatchComponentID,LBatchComponentIdTag,LBatchComponentIdTagEnd);
        TraceWrite('LBatchComponentID: ', $$plsql_line, to_char(LBatchComponentID));

              IF NVL(LBatchId,0)<>0 THEN
                SetKeyValue(LBatchId,LBatchIdTag,LBatchIdTagEnd);
              END IF;

              SELECT ExtractValue (XmlType (LXmlRows), 'VW_BatchComponent/ROW/COMPOUNDID')
              INTO LComponentID FROM dual;
        TraceWrite('LComponentID: ', $$plsql_line, to_char(LComponentID));


              IF NVL (LComponentID, 0) > 0  THEN
                SELECT MixtureComponentID
                INTO LMixtureComponentID
                FROM VW_Mixture_Component
                WHERE MixtureID = LMixtureID AND CompoundID = LComponentID;
        TraceWrite('LMixtureComponentID: ', $$plsql_line, to_char(LMixtureComponentID));

              ELSE
                IF NVL (LMixtureComponentID, 0) <= 0 THEN
                  SELECT LXslTablesTransformed.extract('/MultiCompoundRegistryRecord/VW_Compound[1]/ROW/COMPOUNDID/text()').GetNumberVal()
                  INTO LComponentID FROM dual;

                  SELECT MixtureComponentID
                  INTO LMixtureComponentID
                  FROM VW_Mixture_Component
                  WHERE MixtureID = LMixtureID AND CompoundID = LComponentID;
                END IF;
              END IF;

              IF INSTR(LXmlRows,'<COMPOUNDID>')<>0 THEN
                LXmlRows:=SUBSTR(LXmlRows,1,INSTR(LXmlRows,'<COMPOUNDID>')-1)||SUBSTR(LXmlRows,INSTR(LXmlRows,'</COMPOUNDID>')+13,LENGTH(LXmlRows));
              END IF;
        TraceWrite('LXmlRows: ', $$plsql_line, LXmlRows);


              --LXmlRows:=REPLACE(LXmlRows,'insert="yes"','');
              SetKeyValue(LMixtureComponentID,LMixtureComponentIDTag,LMixtureComponentIDTagEnd);
        TraceWrite('SetKeyValue_after: ', $$plsql_line, LXmlRows);

              InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);

            END;

          WHEN 'VW_STRUCTURE' THEN
            BEGIN
              SELECT extractValue(XmlType.CreateXml(LXmlRows),'VW_Structure/ROW/STRUCTUREID/text()')
              INTO LStructureID FROM dual;

              IF ( NVL(LStructureID, 0) <= 0 ) THEN --StructureID <=0 means a new structure, rather than existing structure from "Use Structure" duplicate option
                SELECT MOLID_SEQ.NEXTVAL INTO LStructureID FROM DUAL;
                SetKeyValue(LStructureID, LStructureIDTag, LStructureIdTagEnd);

                LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                LXmlRows:=Replace(LXmlRows,'<STRUCTURE>(RemovedStructure)</STRUCTURE>','');

                InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);

                IF Upper(ADuplicateCheck)='N' OR ( NOT (Upper(ADuplicateCheck)='N') AND RegistrationRLS.GetStateRLS )  THEN
                  IF NVL(LRegIDAux,0) = 0 THEN
                    IF ValidateWildcardStructure(LStructureValue) THEN
                      VerifyAndAddDuplicateToSave(LRegNumber,LStructureValue, NULL,LXMLRegNumberDuplicated);
                    END IF;
                  END IF;
                END IF;
              ELSE
                LStructureValue := TakeOffAndGetClob(LStructuresList, 'Clob');
              END IF;
            END;

          WHEN 'VW_STRUCTURE_IDENTIFIER' THEN
            BEGIN
              TraceWrite('UpdateMcrr_VW_STRUCTURE_IDENTIFIER', $$plsql_line, to_char(LStructureID));

              if NVL(LStructureID, 0) <> 0 then
                SetKeyValue(LStructureID, LStructureIDTag, LStructureIDTagEnd);
              end if;

              TraceWrite('UpdateMcrr_VW_STRUCTURE_IDENTIFIER', $$plsql_line, LXmlRows);

              InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
            END;

          WHEN 'VW_REGISTRYNUMBER' THEN
            BEGIN
              SELECT extractValue(XmlType.CreateXml(LXmlRows),'VW_RegistryNumber/ROW/REGID/text()')
              INTO LRegIDAux FROM dual;

              IF LRegIDAux = 0 THEN
                SELECT SEQ_REG_NUMBERS.NEXTVAL INTO LRegID FROM DUAL;
                SetKeyValue(LRegID,LRegIDTag,LRegIDTagEnd);

                SELECT  extractvalue(XmlType(LXmlRows),'VW_RegistryNumber/ROW/SEQUENCEID')
                INTO LSequenceID FROM dual;

                IF LSequenceID IS NOT NULL THEN
                  LRegNumber:=GetRegNumber(LSequenceID,LSequenceNumber,LXmlTables);
                END IF;

                SetKeyValue(LRegNumber,LRegNumberTag,LRegNumberTagEnd);
                SetKeyValue(LSequenceNumber,LSequenceNumberTag,LSequenceNumberTagEnd);
                SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');

                InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
              ELSE
                LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                SELECT CompoundID INTO LCompoundID
                FROM VW_Compound WHERE RegID=LRegIDAux;
              END IF;
            END;

          WHEN 'VW_COMPOUND_IDENTIFIER' THEN
            BEGIN
              SetKeyValue(LRegID,LRegIDTag,LRegIDTagEnd);
              InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
            END;

          WHEN 'VW_COMPOUND_PROJECT' THEN
            BEGIN
              SetKeyValue(LRegID,LRegIDTag,LRegIDTagEnd);
              InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
            END;

          WHEN 'VW_REGISTRYNUMBER_PROJECT' THEN
            BEGIN
              SetKeyValue(LBatchID,LBatchIdTag,LBatchIdTagEnd);
              InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
            END;

          WHEN 'VW_COMPOUND' THEN
            BEGIN
              SELECT SEQ_COMPOUND_MOLECULE.NEXTVAL INTO LCompoundID FROM DUAL;

              SetKeyValue(LCompoundID,LCompoundIDTag,LCompoundIDTagEnd);
              SetKeyValue(LRegID,LRegIDTag,LRegIDTagEnd);
              SetKeyValue(LStructureID,LStructureIDTag,LStructureIdTagEnd);
              SetKeyValue(SYSDATE,'<DATECREATED>','</DATECREATED>');
              SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');

              LNormalizedStructureValue := TakeOffAndGetClob(LNormalizedStructureList,'Clob');

              InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
            END;

          WHEN 'VW_COMPOUND_FRAGMENT' THEN
            BEGIN
              SELECT SEQ_COMPOUND_FRAGMENT.NEXTVAL INTO LCompoundFragmentID FROM DUAL;
              SetKeyValue(LCompoundFragmentID, LCompoundFragmentIdTag, LCompoundFragmentIdTagEnd);
              IF NVL(LCompoundID,0)<>0 THEN
                SetKeyValue(LCompoundID,LCompoundIDTag,LCompoundIDTagEnd);
              END IF;
              InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
            END;

          WHEN 'VW_MIXTURE_COMPONENT' THEN
            BEGIN
              SELECT SEQ_MIXTURE_COMPONENT.NEXTVAL INTO LMixtureComponentID FROM DUAL;

              SetKeyValue(LMixtureComponentID,LMixtureComponentIDTag,LMixtureComponentIDTagEnd);
              SetKeyValue(LMixtureID,LMixtureIDTag,LMixtureIDTagEnd);
              SetKeyValue(LCompoundID,LCompoundIDTag,LCompoundIDTagEnd);
              InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
            END;

          WHEN 'VW_BATCHCOMPONENTFRAGMENT' THEN
            TraceWrite('UpdateMcrr_insert="yes"', $$plsql_line, LXmlRows);
            BEGIN
              SELECT
                ExtractValue(LXmlTypeRows, 'VW_BatchComponentFragment/ROW/@FragmentID')
                , ExtractValue(LXmlTypeRows, 'VW_BatchComponentFragment/ROW/@CompoundID')
              INTO LFragmentID,LCompoundIDAux
              FROM DUAL;

              IF NVL(LCompoundIDAux,0)>0 THEN
                LCompoundID:=LCompoundIDAux;
              END IF;

              IF NVL(LFragmentID,0)<>0 AND NVL(LCompoundID,0)<>0 THEN
                SELECT MIN(ID)
                INTO LCompoundFragmentID
                FROM VW_Compound_Fragment
                WHERE CompoundID = LCompoundID
                  And FragmentID = LFragmentID;
              END IF;

              IF NVL(LCompoundFragmentID,0)<>0 THEN
                SetKeyValue(LCompoundFragmentID,LBatchCompoundFragIdTag,LBatchCompoundFragIdTagEnd);
              END IF;

              IF NVL(LBatchComponentID,0)<>0 THEN
                SetKeyValue(LBatchComponentID,LBatchCompFragIdTag,LBatchCompFragIdTagEnd);
              END IF;

              InsertData(LTableName,LXmlRows,LStructureValue,LStructureAggregationValue,LFragmentXmlValue,LNormalizedStructureValue,LCompoundID,LStructureID,LMixtureID,LMessage,LRowsInserted);
            END;

          ELSE  LMessage:=LMessage || ' "' || LTableName||'" table stranger.';
        END CASE;

        IF LRowsInserted > 0 THEN
          LRowsProcessed := LRowsProcessed + LRowsInserted;
          LSomeUpdate := TRUE;
        END IF;

      ELSE
      -- Update
        LinsCtx := DBMS_XMLSTORE.newContext(LTableName);

        SELECT XmlType(LXmlRows).extract(LTableName||'/ROW[1]/node()[1]').getClobVal()
        INTO LFieldName FROM dual;

        LUpdate := FALSE;
        IF LFieldName IS NOT NULL THEN
          DBMS_XMLSTORE.clearUpdateColumnList(LinsCtx);

          CASE UPPER(LTableName)
            WHEN 'VW_MIXTURE' THEN
              BEGIN
                SELECT extractvalue(XmlType(LXmlRows),'VW_Mixture/ROW/MIXTUREID')
                INTO LMixtureID FROM dual;

                LStructureAggregationValue := TakeOffAndGetClob(LStructureAggregationList, 'Clob');
                SetKeyValue(SYSDATE,'<MODIFIED>','</MODIFIED>');
              END;

            WHEN 'VW_STRUCTURE' THEN
              BEGIN
                SELECT extractvalue(XmlType(LXmlRows),'VW_Structure/ROW[1]/node()[1]')
                INTO LStructureID FROM dual;
                SELECT extractvalue(XmlType(LXmlRows),'VW_Structure/ROW[1]/DRAWINGTYPE')
                INTO LStructureDrawingType FROM dual;
                TraceWrite('UpdateMcrr_LStructureID', $$plsql_line, to_char(LStructureID));

                LStructureValue := TakeOffAndGetClob(LStructuresList, 'Clob');
                IF NVL(LRegID, 0) <> 0 AND LStructureDrawingType > 0 THEN
                  SELECT RegNumber
                  INTO LRegNumberAux
                  FROM VW_RegistryNumber
                  WHERE RegID = LRegID;

                  DELETE VW_Duplicates WHERE RegNumber = LRegNumberAux;
                  DELETE VW_Duplicates WHERE RegNumberDuplicated = LRegNumberAux;

                  IF ValidateWildcardStructure(LStructureValue) THEN
                    VerifyAndAddDuplicateToSave(LRegNumberAux,LStructureValue, LRegID,LXMLRegNumberDuplicated);
                  END IF;
                END IF;
              END;

            WHEN 'VW_COMPOUND' THEN
              BEGIN
                SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
                SetKeyValue(LStructureID,LStructureIDTag,LStructureIdTagEnd);

                LNormalizedStructureValue :=TakeOffAndGetClob(LNormalizedStructureList,'Clob');

                IF XmlType(LXmlRows).ExistsNode('VW_Compound/ROW/STRUCTUREID')=1 THEN
                  SELECT StructureID
                  INTO LStructureIDToUpdate
                  FROM VW_Compound
                  WHERE CompoundID = XmlType.CreateXml(LXmlRows).extract(
                    'VW_Compound/ROW/COMPOUNDID/text()'
                  ).getStringVal();
                END IF;
              END;

            WHEN 'VW_REGISTRYNUMBER' THEN
              BEGIN
                SELECT extractValue(XmlType.CreateXml(LXmlRows),'VW_RegistryNumber/ROW/REGID/text()')
                INTO LRegID FROM dual;
              END;

            WHEN 'VW_BATCH' THEN
              BEGIN
                IF XmlType(LXmlRows).ExistsNode('VW_Batch/ROW/FULLREGNUMBER') = 1 THEN
                  SELECT
                    extractValue(LXmlTables,'/MultiCompoundRegistryRecord/RegNumber/RegNumber')
                    , extractValue(LXmlTables,'/MultiCompoundRegistryRecord/RegNumber/SequenceID')
                    , extractvalue(XmlType(LXmlRows),'VW_Batch/ROW/BATCHNUMBER')
                    , extractvalue(XmlType(LXmlRows),'VW_Batch/ROW/BATCHID')
                  INTO LFullRegNumber,LSequenceID,LBatchNumber,LBatchID
                  FROM dual;
                END IF;

                SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
              END;

            WHEN 'VW_BATCHCOMPONENT' THEN
              BEGIN
                SELECT ExtractValue (XmlType (LXmlRows), 'VW_BatchComponent/ROW/MIXTURECOMPONENTID')
                INTO LComponentID FROM dual;

                IF NVL (LComponentID, 0) <> 0  THEN
                  SELECT MixtureComponentID
                  INTO LMixtureComponentID
                  FROM VW_Mixture_Component
                  WHERE MixtureID = LMixtureID AND CompoundID = LComponentID
                    AND ROWNUM < 2 ORDER BY MixtureComponentID;

                  SetKeyValue(LMixtureComponentID,LMixtureComponentIDTag,LMixtureComponentIDTagEnd);
                END IF;
              END;
          ELSE NULL;
        END CASE;

        LFieldName := XMLType(LFieldName).getRootElement();
        DBMS_XMLSTORE.setKeyColumn(LinsCtx, LFieldName);

        TraceWrite('UpdateMcrr_LFieldName', $$plsql_line, LFieldName);

        LIndexField := 1;
        /* LOOP
          --Search each Table

TraceWrite('UpdateMcrr', $$plsql_line, XmlType(LXmlRows).extract(LTableName||'/ROW[1]').GetClobval());

          LIndexField := LIndexField + 1;
          SELECT XmlType(LXmlRows).extract(LTableName||'/ROW[1]/node()['||LIndexField||']').getClobVal()
          INTO LFieldToUpdate FROM dual;

TraceWrite('UpdateMcrr', $$plsql_line, LFieldToUpdate);

          IF LFieldToUpdate IS NOT NULL THEN
            LUpdate := TRUE;
            LFieldToUpdate := XMLType(LFieldToUpdate).getRootElement();
          END IF;

TraceWrite('UpdateMcrr_LFieldToUpdate', $$plsql_line, LFieldToUpdate);

          EXIT WHEN LFieldToUpdate IS NULL;
            DBMS_XMLSTORE.setUpdateColumn(LinsCtx, LFieldToUpdate);
          END LOOP;*/
          lstmt:='for $i in '||LTableName||'/ROW[1]/* return $i';
          -- check, do we need update table or not
              lstmt2 := 'select  count(1) from '||LTableName||' a where ';
              lstmt3 := '';
              lstmt4 := ' where ';

              LIndexField:= 0;
              lvTableName := to_char(LTableName);
              Larr_idc:=-1;
              Larr_idv:=0;
              Larr_idc1:=-1;
              
         for cur_upd in (with a as
           (select XmlType(LXmlRows) as xml  FROM dual)
           select
           --z.tt.getRootElement() as FieldToUpdate
           z.tt as FieldToUpdate
           , 
           -- if value is structure, we take it from variable
           case when UPPER(lvTableName)='VW_STRUCTURE' and upper((z.tt))='STRUCTURE' then empty_clob()
                  when UPPER(lvTableName)='VW_MIXTURE' and upper((z.tt))='STRUCTUREAGGREGATION' then empty_clob()
                  when UPPER(lvTableName)='VW_FRAGMENT' and upper((z.tt))='STRUCTURE' then empty_clob()
                  when UPPER(lvTableName)='VW_COMPOUND' and upper((z.tt))='NORMALIZEDSTRUCTURE' then empty_clob()
                else
                  z.val
             end as val
           from a, xmltable(lstmt
           passing (a.xml)
                columns tt   varchar2(100) path './local-name()'--'/node()'
                , val clob path '/*'
           ) z ) loop
              LIndexField := LIndexField + 1;
              select data_type into ldata_type from user_tab_cols where table_name = upper(lvTableName) and column_name = upper(cur_upd.FieldToUpdate);


              case ldata_type
                when 'NUMBER'
                      then
                      begin
                        lstmt2 := lstmt2 || ' a.'|| case when dbms_lob.getlength(cur_upd.val)=0 then cur_upd.FieldToUpdate || ' is null  '
                                                             else cur_upd.FieldToUpdate || '=' || to_char(cur_upd.val) end || ' and ';
                        if LIndexField != 1 then -- we don't need update ID of table
                           lstmt3 := lstmt3 || ', a.'||cur_upd.FieldToUpdate||'='||nvl(to_char(cur_upd.val),'null');
                        else -- we need to do update only using primary ID of table
--                           lstmt4 := lstmt4 || ' a.'|| case when dbms_lob.getlength(cur_upd.val)=0 then cur_upd.FieldToUpdate || ' is null '
--                                                                else cur_upd.FieldToUpdate || '=' || to_char(cur_upd.val) end;
                         -- here cur_upd.val must be not null, if it is null it is error
                           lstmt4 := lstmt4 || ' a.'|| cur_upd.FieldToUpdate || '=' || to_char(cur_upd.val);
                        end if;

                      end;
                when 'CLOB'
                      then
                      begin
                        lstmt2 := lstmt2 ||
                                 case when (UPPER(lvTableName)='VW_STRUCTURE' and upper(cur_upd.FieldToUpdate)='STRUCTURE')
                                  or ( UPPER(lvTableName)='VW_MIXTURE' and upper(cur_upd.FieldToUpdate)='STRUCTUREAGGREGATION')
                                  or (UPPER(lvTableName)='VW_FRAGMENT' and upper(cur_upd.FieldToUpdate)='STRUCTURE')
                                  or (UPPER(lvTableName)='VW_COMPOUND' and upper(cur_upd.FieldToUpdate)='NORMALIZEDSTRUCTURE')
                                    then ''-- it will be updated in any case at the end procedure
                                   else
                                      ' nvl(dbms_lob.compare(a.'||cur_upd.FieldToUpdate||', :'||( cur_upd.FieldToUpdate)|| '),1) != 0 ' || ' and '
                                   end;
                        lstmt3 := lstmt3 ||
                         case when (UPPER(lvTableName)='VW_STRUCTURE' and upper(cur_upd.FieldToUpdate)='STRUCTURE')
                                  or ( UPPER(lvTableName)='VW_MIXTURE' and upper(cur_upd.FieldToUpdate)='STRUCTUREAGGREGATION')
                                  or (UPPER(lvTableName)='VW_FRAGMENT' and upper(cur_upd.FieldToUpdate)='STRUCTURE')
                                  or (UPPER(lvTableName)='VW_COMPOUND' and upper(cur_upd.FieldToUpdate)='NORMALIZEDSTRUCTURE')
                                    then ''-- it will be updated in any case at the end procedure
                                   else
                               ', a.'||cur_upd.FieldToUpdate||'='||  case when dbms_lob.getlength(cur_upd.val)=0 then q'[empty_clob()]'
                                                                                       else
                                                                                           ':' || cur_upd.FieldToUpdate
                                                                                       end
                         end;

                                 if  ( (UPPER(lvTableName)='VW_STRUCTURE' and upper(cur_upd.FieldToUpdate)='STRUCTURE')
                                  or ( UPPER(lvTableName)='VW_MIXTURE' and upper(cur_upd.FieldToUpdate)='STRUCTUREAGGREGATION')
                                  or (UPPER(lvTableName)='VW_FRAGMENT' and upper(cur_upd.FieldToUpdate)='STRUCTURE')
                                  or (UPPER(lvTableName)='VW_COMPOUND' and upper(cur_upd.FieldToUpdate)='NORMALIZEDSTRUCTURE')) then
                                    null;
                                 else
                                 if dbms_lob.getlength(cur_upd.val) =0 then
                                               Larr_idc:=Larr_idc+1;
                                               Lclobtable(Larr_idc):=cur_upd.val;
                                               Lvartable(Larr_idc):=cur_upd.FieldToUpdate;
                                    end if;
                                    Larr_idc1:=Larr_idc1+1;
                                    Lclobtable1(Larr_idc1):=cur_upd.val;
                                    Lvartable1(Larr_idc1):=cur_upd.FieldToUpdate;
                                 end if;





                        --lstmt4 := lstmt4 || ' and nvl(dbms_lob.compare('||cur_upd.FieldToUpdate||', '||q'[']'||to_char(cur_upd.val)||q'[']'||'),1) != 0';
                      end;
                when 'NCHAR'
                      then
                      begin
                        lstmt2 := lstmt2 || ' a.'||case when dbms_lob.getlength(cur_upd.val)=0 then cur_upd.FieldToUpdate || ' is null '
                                                            else cur_upd.FieldToUpdate||'='||q'[q'[]'||to_char(cur_upd.val)||']'||q'[']' end || ' and ';
                        lstmt3 := lstmt3 || ', a.'||cur_upd.FieldToUpdate||'='||q'[q'[]'||to_char(cur_upd.val)||']'||q'[']';
                        --lstmt4 := lstmt4 || ' and a.'||cur_upd.FieldToUpdate||'='||q'[']'||to_char(cur_upd.val)||q'[']';
                      end;
                when 'DATE'
                      then
                      begin
                       if lvTableName||'.'||cur_upd.FieldToUpdate in ('VW_Compound.DATELASTMODIFIED','VW_Compound.ORDER_DATE','VW_Compound.RECEIVED_DATE','VW_Mixture.MODIFIED','VW_Batch.CREATION_DATE','VW_Batch.DATELASTMODIFIED') then
                         null;
                       else
                          lstmt2 := lstmt2 || ' a.' ||case when dbms_lob.getlength(cur_upd.val)=0 then cur_upd.FieldToUpdate || ' is null '
                                                               else cur_upd.FieldToUpdate||'='||q'[to_date(']' ||to_char(cur_upd.val)||q'[','yyyy-mm-dd hh:mi:ss AM')]' end || ' and ';
                       end if;
                        lstmt3 := lstmt3 || ', a.' ||cur_upd.FieldToUpdate||'='|| case when dbms_lob.getlength(cur_upd.val)=0 then q'[null]'
                                                                                       else  q'[to_date(']' ||to_char(cur_upd.val)||q'[','yyyy-mm-dd hh:mi:ss AM')]' end;
                        --lstmt4 := lstmt4 || ' and a.' ||cur_upd.FieldToUpdate||'='||q'[to_date(']' ||to_char(cur_upd.val)||q'[','yyyy-mm-dd hh:mi:ss AM')]';
                      end;
                when 'VARCHAR2'
                      then
                      begin
                        lstmt2 := lstmt2 ||case when dbms_lob.getlength(cur_upd.val)=0 then  ' a.'||cur_upd.FieldToUpdate || ' is null '
                                                            else cur_upd.FieldToUpdate||'= to_char(:'|| cur_upd.FieldToUpdate ||')'  end || ' and ';
                        lstmt3 := lstmt3 || ', a.'||cur_upd.FieldToUpdate||'=to_char(:'|| cur_upd.FieldToUpdate ||')';
                        --lstmt4 := lstmt4 || ' and a.'||cur_upd.FieldToUpdate||'='||q'[']'||to_char(cur_upd.val)||q'[']';
                         if dbms_lob.getlength(cur_upd.val) !=0 then
                            Larr_idc:=Larr_idc+1;
                            Lclobtable(Larr_idc):=cur_upd.val;
                            Lvartable(Larr_idc):=cur_upd.FieldToUpdate;
                         end if;

                            Larr_idc1:=Larr_idc1+1;
                            Lclobtable1(Larr_idc1):=cur_upd.val;
                            Lvartable1(Larr_idc1):=cur_upd.FieldToUpdate;

                      end;
                else lstmt2 := lstmt2 || '';
                     lstmt3 := lstmt3 || '';
                     --lstmt4 := lstmt4 || '';
              end case;
          end loop;

           lstmt2 :=rtrim(lstmt2,' and ');
           
          TraceWrite('UpdateMcrr_lstmt2', $$plsql_line, 'lstmt2-> '||lstmt2);
        begin
          source_cursor := dbms_sql.open_cursor;
         DBMS_SQL.PARSE(source_cursor,
            lstmt2,
              DBMS_SQL.NATIVE);


          DBMS_SQL.DEFINE_COLUMN(source_cursor, 1, lout);

        IF Larr_idc>-1 THEN
          for i in 0..Larr_idc    loop
              DBMS_SQL.BIND_VARIABLE(source_cursor, Lvartable(i), Lclobtable(i));
          end loop;
        END IF;

          Larr_idv:= DBMS_SQL.EXECUTE(source_cursor);
          if dbms_sql.fetch_rows(source_cursor) > 0 then
             dbms_sql.column_value(source_cursor, 1, Lout);
          end if;
           DBMS_SQL.CLOSE_CURSOR(source_cursor);

          EXCEPTION
         WHEN OTHERS THEN
              TraceWrite('UpdateMcrr_lstmt2_err', $$plsql_line, 'lstmt2-> '||SQLERRM);
              DBMS_SQL.CLOSE_CURSOR(source_cursor);

       RAISE;
     end;

           lstmt3 := case
           -- if nothing set to update
           when nvl(length(trim(both ' ' from  substr(lstmt3,2) )),0)=0 then 'select 1 from dual'
           -- if some columns set for updating
           else 'update '||LTableName||' a set '||substr(lstmt3,2)||lstmt4
           end  ;
           lstmt3 := case
           -- if nothing set to update
           when lstmt3='update VW_Mixture_Component a set  a.MIXTUREID=0, a.COMPOUNDID=0 where  a.MIXTURECOMPONENTID=0'
            then  'select 1 from dual' else lstmt3 end;



           case lout when 0 then
              LUpdate:= true;
           else
              LUpdate:= false;
           end case;
         -- End. check, do we need update table or not
--        if LUpdate  then
--         for cur_upd in (with a as
--           (select XmlType(LXmlRows) as xml  FROM dual)
--           select z.tt.getRootElement() as FieldToUpdate
--           from a, xmltable(lstmt
--           passing (a.xml)
--                columns tt   XMLtype path '/*'
--           ) z ) loop
--              LUpdate := TRUE;
--              LFieldToUpdate:=cur_upd.FieldToUpdate;
--              DBMS_XMLSTORE.setUpdateColumn(LinsCtx, LFieldToUpdate);
--              TraceWrite('UpdateMcrr_LFieldToUpdate', $$plsql_line, LFieldToUpdate||' '||XmlType(LXmlRows).extract(LTableName||'/ROW[1]').GetClobval());
--          end loop;
--         end if;
        END IF;

        --Insert Rows and get count it inserted
        IF LUpdate THEN
          LSomeUpdate := TRUE;
--          LRowsUpdated := DBMS_XMLSTORE.updateXML( LinsCtx, LXmlRows );
          lstmt3:= Replace(lstmt3, '(RemovedStructure)','');
          if  lstmt3 != 'select 1 from dual' then
          TraceWrite('UpdateMcrr_lstmt3', $$plsql_line, 'lstmt3-> '||lstmt3);

             begin
                  source_cursor := dbms_sql.open_cursor;

                 DBMS_SQL.PARSE(source_cursor,   lstmt3,  DBMS_SQL.NATIVE);

                IF Larr_idc1>-1 THEN
                  for i in 0..Larr_idc1    loop

                      DBMS_SQL.BIND_VARIABLE(source_cursor, Lvartable1(i), Lclobtable1(i));
                  end loop;
                END IF;

                  Larr_idv:= DBMS_SQL.EXECUTE(source_cursor);


                   DBMS_SQL.CLOSE_CURSOR(source_cursor);

                  EXCEPTION
                 WHEN OTHERS THEN
                      TraceWrite('UpdateMcrr_lstmt3_err', $$plsql_line, 'lstmt3-> '||SQLERRM);
                      DBMS_SQL.CLOSE_CURSOR(source_cursor);

               RAISE;
             end;
          end if;

          LRowsProcessed := LRowsProcessed + LRowsUpdated;
          --Build Message Logs
          LMessage := LMessage || ' ' || cast(LRowsUpdated as string) || ' Row/s Updated on "' || LTableName || '".'||CHR(13);
        ELSE
          --Build Message Logs
          LMessage:=LMessage || ' 0 Row Updated on "'||LTableName||'".';
        END IF;

        --Close the Table Context
        DBMS_XMLSTORE.closeContext(LinsCtx);

        IF UPPER(LTableName)='VW_STRUCTURE' AND LStructureValue IS NOT NULL THEN
          UPDATE VW_STRUCTURE
          SET STRUCTURE = LStructureValue
          WHERE STRUCTUREID = LStructureID and dbms_lob.compare(STRUCTURE, LStructureValue)!=0;
        END IF;

        IF UPPER(LTableName)='VW_MIXTURE' AND LStructureAggregationValue IS NOT NULL THEN
          UPDATE VW_MIXTURE
          SET StructureAggregation = LStructureAggregationValue
          WHERE MixtureID = LMixtureID  and dbms_lob.compare(StructureAggregation, LStructureAggregationValue)!=0;
        END IF;

        IF UPPER(LTableName)='VW_FRAGMENT' AND LFragmentXmlValue IS NOT NULL THEN
          LFragmentID := XmlType(LXmlRows).extract('VW_Fragment/ROW/FRAGMENTID/text()').getNumberVal();

          UPDATE VW_FRAGMENT
          SET STRUCTURE = LFragmentXmlValue
          WHERE FRAGMENTID = LFragmentID  and dbms_lob.compare(STRUCTURE, LFragmentXmlValue)!=0;
        END IF;

        IF UPPER(LTableName)='VW_COMPOUND' THEN
          IF LNormalizedStructureValue IS NOT NULL THEN
            SELECT extractvalue(XmlType(LXmlRows),'VW_Compound/ROW[1]/node()[1]')
            INTO LCompoundIDTemp
            FROM dual;

            UPDATE VW_COMPOUND
            SET NORMALIZEDSTRUCTURE = LNormalizedStructureValue
            WHERE COMPOUNDID = LCompoundIDTemp  and dbms_lob.compare(NORMALIZEDSTRUCTURE, LNormalizedStructureValue)!=0;

            IF LStructureIDToUpdate IS NOT NULL THEN
              DeleteStructure(LStructureIDToUpdate,LMessage);
            END IF;
          END IF;

          IF LStructureIDToUpdate IS NOT NULL THEN
            DeleteStructure(LStructureIDToUpdate,LMessage);
          END IF;
        END IF;
      END IF;

    END LOOP;

    LXmlRows := Replace(LXmlRows, '<STRUCTURE>(RemovedStructure)</STRUCTURE>','');

    IF LSomeUpdate THEN
      --Build Message Logs
      LBriefMessage := 'Total: ' || cast(LRowsProcessed as string) || ' Row/s Processed.';
      LMessage := LMessage || chr(10) || LBriefMessage ||CHR(13);

      IF LXMLRegNumberDuplicated IS NOT NULL THEN
        IF LXslTablesTransformed.ExistsNode('VW_RegistryNumber[1]/ROW/PERSONREGISTERED/text()')=1 THEN
          SaveRegNumbersDuplicated(LXMLRegNumberDuplicated,LXmlTables.extract('/MultiCompoundRegistryRecord/PersonCreated/text()').getStringVal());
        ELSE
          SaveRegNumbersDuplicated(LXMLRegNumberDuplicated,NULL);
        END IF;
      END IF;

      IF LRegNumber IS NOT NULL THEN

        -- Run the FullBatchNum generators!
        --GetBatchIDListByMixtureId(LMixtureIDAux, LNewBatchList);
        UpdateBatchRegNumbers(LNewBatchList);

        -- notify listener SPROCs
        OnRegistrationUpdate(LRegNumber);

        -- Re-fetch the surrogate key value since the listener SPROCs may have modified it
        begin
          select rn.regnumber into LRegNumber
          from vw_mixture m
            join vw_registrynumber rn on rn.regid = m.regid
          where m.mixtureid = LMixtureIDAux;
        end;

        IF (
          (ASectionsList is null or UPPER(ASectionsList) <> cSectionListEmpty)
          AND (LRegNumber is not null)
        ) THEN
          RetrieveMultiCompoundRegistry(LRegNumber, LXMLRegistryRecord, ASectionsList);
        END IF;

        AMessage := CreateRegistrationResponse(LBriefMessage, NULL, LXMLRegistryRecord);
      END IF;
    ELSE
      --Build Message Logs
      LMessage := LMessage || chr(10) || 'Total: ' || cast(LRowsProcessed as string) || ' Row/s Processed.'||CHR(13);
      RAISE_APPLICATION_ERROR(eGenericException, AppendError('No fields/sections to update/insert.'));
    END IF;
    TraceWrite( act||'UpdateMultiCompoundRegistry_ended', $$plsql_line,'end' );
  END;

  /**
  Deletes a registration from the repository. Will not delete structures that are shared
  by other compounds.
  -->author Fari
  -->param ARegNumber the surrogate key for a Registration
  */
  PROCEDURE DeleteMultiCompoundRegistry(ARegNumber in VW_RegistryNumber.regnumber%type) IS
    LMessage CLOB;
    LRegID Number;
    LIsNotEditableDeleting EXCEPTION;
    LMsg varchar2(255) := ' row(s) deleted on ';
    LRegStatus number(1);
--    LEnableRLS BOOLEAN;
    LLevelRLS VARCHAR2(50);
    CURSOR C_Components(ARegNumber VW_RegistryNumber.regnumber%type)  IS
      SELECT MC.CompoundID,MC.MixtureID
      FROM VW_Mixture_Component MC,VW_Mixture M,VW_RegistryNumber R
      WHERE MC.MixtureID=M.MixtureID AND M.RegID=R.RegID AND R.RegNumber=ARegNumber;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'DeleteMultiCompoundRegistry_started', $$plsql_line,'start' );

    TraceWrite('DeleteMcrr_0_RegNumber', $$plsql_line, ARegNumber);
    IF GetIsEditable(ARegNumber)='False' OR UPPER(GetIsRegistryDeleteable(ARegNumber))='FALSE' then
      RAISE LIsNotEditableDeleting ;
    END IF;

    LMessage := chr(10) || 'Begining mixture deleted process.';
    BEGIN
      SELECT RegID
      INTO LRegID
      FROM VW_RegistryNumber R
      WHERE R.RegNumber=ARegNumber;
      SELECT STATUSID
      INTO LRegStatus
      FROM VW_MIXTURE
      WHERE REGID=LRegID;
    EXCEPTION
      WHEN NO_DATA_FOUND THEN
        LMessage:=chr(10) || '0 Row/s found on "VW_RegistryNumber".';
        RAISE_APPLICATION_ERROR(eInvalidRegNum,
        AppendError('The Registry "'||ARegNumber||'" does not exist.'));
    END;
    IF LRegStatus = 4 Then
     RAISE LIsNotEditableDeleting ;
    END IF;
    FOR R_Components IN C_Components(ARegNumber) LOOP
      LMessage := LMessage||chr(10) || 'Deleting CompoundID='||R_Components.CompoundID;
      DeleteCompound(R_Components.CompoundID,R_Components.MixtureID, LMessage);
      TraceWrite('DeleteMcrr_VW_Mixture_Component(' || R_Components.CompoundID || ')', $$plsql_line, LMessage);
    END LOOP;

    DELETE VW_Mixture_Component
    WHERE MixtureID IN (
      SELECT M.MixtureID
      FROM VW_RegistryNumber RN,VW_Mixture M
      WHERE RN.RegID=M.RegID AND RN.RegNumber=ARegNumber
    );
    LMessage := LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || LMsg || '"VW_Mixture_Component".';
    TraceWrite('DeleteMcrr_VW_Mixture_Component', $$plsql_line, LMessage);

    DELETE VW_Compound_Identifier WHERE RegID = LRegID;
    LMessage := LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || LMsg || '"VW_Compound_Identifie".';
    TraceWrite('DeleteMcrr_VW_Compound_Identifier', $$plsql_line, LMessage);

    DELETE VW_Batch_Project WHERE BatchID IN (SELECT B.BatchID FROM VW_Batch B WHERE B.RegID = LRegID );
    LMessage := LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || LMsg || '"VW_Batch_Project".';
    TraceWrite('DeleteMcrr_VW_Batch_Project', $$plsql_line, LMessage);

    LLevelRLS := vActiveRLS;
    IF upper(LLevelRLS) != 'OFF' THEN
--      LEnableRLS := RegistrationRLS.GEnableRLS;
      IF LEnableRLS THEN
        RegistrationRLS.GEnableRLS := False;
      END IF;
    END IF;

    DELETE VW_BatchIdentifier WHERE BatchID IN (SELECT B.BatchID FROM VW_Batch B WHERE B.RegID = LRegID);
    LMessage := LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || LMsg || '"VW_BatchIdentifier".';
    TraceWrite('DeleteMcrr_VW_BatchIdentifier', $$plsql_line, LMessage);

    IF LEnableRLS IS NOT NULL THEN
        RegistrationRLS.GEnableRLS:=LEnableRLS;
    END IF;

    DELETE VW_Batch WHERE RegID = LRegID;
    LMessage := LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || LMsg || '"VW_Batch".';
    TraceWrite('DeleteMcrr_VW_Batch', $$plsql_line, LMessage);

    DELETE VW_Mixture WHERE RegID = LRegID;
    LMessage := LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || LMsg || '"VW_Mixture".';
    TraceWrite('DeleteMcrr_VW_Mixture', $$plsql_line, LMessage);

    DELETE VW_RegistryNumber_Project WHERE RegID = LRegID;
    LMessage := LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || LMsg || '"VW_RegistryNumber_Project".';
    TraceWrite('DeleteMcrr_VW_RegistryNumber_Project', $$plsql_line, LMessage);

    DELETE VW_RegistryNumber WHERE  RegID = LRegID;
    LMessage := LMessage || chr(10) ||  cast (SQL%ROWCOUNT  as string) || LMsg || '"VW_RegistryNumber".';
    TraceWrite('DeleteMcrr_VW_RegistryNumber', $$plsql_line, LMessage);

    TraceWrite('DeleteMcrr', $$plsql_line, 'Completed deletion of RegNum ' || ARegNumber || '(RegID ' || to_char(LRegID) || ')');
    TraceWrite( act||'DeleteMultiCompoundRegistry_ended', $$plsql_line,'end' );
  EXCEPTION
    WHEN LIsNotEditableDeleting THEN
      RAISE_APPLICATION_ERROR(eIsNotEditableDeleting, 'You are not authorized to delete this record. (Registry # '||ARegNumber||')');
    WHEN OTHERS THEN
    BEGIN
      IF LEnableRLS IS NOT NULL THEN
          RegistrationRLS.GEnableRLS:=LEnableRLS;
      END IF;
      LMsg := DBMS_UTILITY.FORMAT_ERROR_STACK || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
      TraceWrite('DeleteMcrr_ERROR', $$plsql_line, LMsg);
      TraceWrite( act||'DeleteMultiCompoundRegistry_ended', $$plsql_line,'end' );
      RAISE_APPLICATION_ERROR(
        eDeleteMultiCompoundRegistry, AppendError('DeleteMultiCompoundRegistry', LMsg)
      );
    END;
  END;

  
  PROCEDURE RetrieveMultiCompRegList_new(
    AXmlRegNumbers in clob
    , AXmlCompoundList out NOCOPY clob
  ) IS
    LXml                      CLOB:='';
    LXmlList                  CLOB:='';
    LSectionList   CONSTANT   VARCHAR2(500):='Compound,Fragment,Batch,Mixture,Identifier';
    LRegNumber                VW_RegistryNumber.RegNumber%type;
    LIndex                    Number;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'RetrieveMultiCompoundRegList_new_started', $$plsql_line,'start' );
     TraceWrite( act||'RetrieveMultiCompoundRegList_new_AXmlRegNumbers', $$plsql_line, AXmlRegNumbers );
    LIndex:=1;
    for cur_ret in (with a as
           (select  XmlType(AXmlRegNumbers) as xml  FROM dual)
           select reg_number from (select z.tt as reg_number
           from a, xmltable('/REGISTRYLIST/node()/node()[1]'
           passing (a.xml)
                columns tt   varchar2(20) path '/text()'
           ) z) where reg_number is not null)
    LOOP
--      SELECT  extractValue(XmlType(AXmlRegNumbers),'REGISTRYLIST/node()['||LIndex||']/node()[1]')
--      INTO LRegNumber
--      FROM dual;
--    EXIT WHEN LRegNumber IS NULL;
      LRegNumber:=cur_ret.reg_number;
      BEGIN
        RetrieveMultiCompoundRegistry(LRegNumber, LXml, LSectionList);
        LXmlList := LXmlList || CHR(10) || LXml;
      EXCEPTION
         WHEN OTHERS THEN
         BEGIN
          IF INSTR(DBMS_UTILITY.format_error_stack,eNoRowsReturned)<>0 THEN
            NULL; --Though a Compound doesn't exist to get the others
          ELSE
           RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegList,DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
          END IF;
         END;
      END;
--      LIndex := LIndex+1;
    END LOOP;

    AXmlCompoundList:='<MultiCompoundRegistryRecordList>'||CHR(10)||LXmlList||CHR(10)||'</MultiCompoundRegistryRecordList>';
    TraceWrite( act||'RetrieveMultiCompoundRegList_new_ended', $$plsql_line,'end' );
  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
      TraceWrite( act||'RetrieveMultiCompoundRegList_new_ended', $$plsql_line,'end' );
      RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegList, AppendError('RetrieveMultiCompoundRegList', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
  END;


  PROCEDURE RetrieveMultiCompoundRegList(
    AXmlRegNumbers in clob
    , AXmlCompoundList out NOCOPY clob
  ) IS
    LXml                      CLOB:='';
    LXmlList                  CLOB:='';
    LSectionList   CONSTANT   VARCHAR2(500):='Compound,Fragment,Batch,Mixture,Identifier';
    LRegNumber                VW_RegistryNumber.RegNumber%type;
    LIndex                    Number;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'RetrieveMultiCompoundRegList_started', $$plsql_line,'start' );
     TraceWrite( act||'RetrieveMultiCompoundRegList_AXmlRegNumbers', $$plsql_line, AXmlRegNumbers );
    LIndex:=1;
    for cur_ret in (with a as
           (select  XmlType(AXmlRegNumbers) as xml  FROM dual)
           select reg_number from (select z.tt as reg_number
           from a, xmltable('/REGISTRYLIST/node()/node()[1]'
           passing (a.xml)
                columns tt   varchar2(20) path '/text()'
           ) z) where reg_number is not null)
    LOOP
--      SELECT  extractValue(XmlType(AXmlRegNumbers),'REGISTRYLIST/node()['||LIndex||']/node()[1]')
--      INTO LRegNumber
--      FROM dual;
--    EXIT WHEN LRegNumber IS NULL;
      LRegNumber:=cur_ret.reg_number;
      BEGIN
        RetrieveMultiCompoundRegistry(LRegNumber, LXml, LSectionList);
        LXmlList := LXmlList || CHR(10) || LXml;
      EXCEPTION
         WHEN OTHERS THEN
         BEGIN
          IF INSTR(DBMS_UTILITY.format_error_stack,eNoRowsReturned)<>0 THEN
            NULL; --Though a Compound doesn't exist to get the others
          ELSE
           RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegList,DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
          END IF;
         END;
      END;
--      LIndex := LIndex+1;
    END LOOP;

    AXmlCompoundList:='<MultiCompoundRegistryRecordList>'||CHR(10)||LXmlList||CHR(10)||'</MultiCompoundRegistryRecordList>';
     TraceWrite( act||'RetrieveMultiCompoundRegList_AXmlCompoundList', $$plsql_line, AXmlCompoundList );
    TraceWrite( act||'RetrieveMultiCompoundRegList_ended', $$plsql_line,'end' );
  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
      TraceWrite( act||'RetrieveMultiCompoundRegList_ended', $$plsql_line,'end' );
      RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegList, AppendError('RetrieveMultiCompoundRegList', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
  END;

  /**
  Insert a single compound record temporary.
  Description: Look over a Xml searching each Table and insert the rows on it.
  -->author Fari
  -->since 09-apr-07
  */
  PROCEDURE CreateTemporaryRegistration(
    AXml in CLOB
    , ATempID out Number
    , AMessage OUT CLOB
    , ASectionsList IN Varchar2 := NULL
  ) IS

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
    LXMLRegistryRecord              CLOB;

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

    LXslTables XmlType := XslMcrrTempCreate;

  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'CreateTemporaryRegistration_started', $$plsql_line,'start' );
    SetSessionParameter;

    TraceWrite('CreateMcrrTmp_0_RawXml', $$plsql_line, AXml);
    LXmlCompReg := AXml;

    -- Take Out the Structures because XmlType don't suport > 64k in any given node
    LFragmentXmlList := TakeOffAndGetClobslist(LXmlCompReg,'<FragmentList');
    LBatchComponentFragmentXMLList := TakeOffAndGetClobslist(LXmlCompReg,'<BatchComponentFragmentList');
    LStructuresList := TakeOffAndGetClobslist(LXmlCompReg,'<Structure ','<Structure','<BaseFragment>');
    LNormalizedStructureList := TakeOffAndGetClobslist(LXmlCompReg,'<NormalizedStructure','<Structure');
    LStructureAggregationList := TakeOffAndGetClobslist(LXmlCompReg,'<StructureAggregation');

    TraceWrite('CreateMcrrTmp_1_StrippedXml', $$plsql_line, AXml);

    -- Get the xml
    LXmlTables := XmlType.CreateXML(LXmlCompReg);

    -- Build a new formatted Xml
    LXslTablesTransformed := LXmlTables.Transform(LXslTables);

    TraceWrite('CreateMcrrTmp_2_Transformed', $$plsql_line, LXslTablesTransformed.getClobVal());

    --Get ID
    LTempBatchID := LXmlTables.Extract('node()[1]/ID/text()').getNumberVal();

    --Look over Xml searching each Table and insert the rows of it.
    SELECT XMLSequence(LXslTablesTransformed.Extract('/node()'))
    INTO LXmlSequenceType FROM DUAL;

    FOR LIndex IN  LXmlSequenceType.FIRST..LXmlSequenceType.LAST LOOP
      --Search each Table
      LXmlTypeRows := LXmlSequenceType(LIndex);
      LTableName:= LXmlTypeRows.GetRootElement();

      --Customization for each View - Use of Sequences and parser for Strcutres
      CASE UPPER(LTableName)
        WHEN 'VW_TEMPORARYBATCH' THEN
          BEGIN
            --Use of Sequences
            IF NVL(LTempBatchID,0)=0 THEN
              SELECT SEQ_TEMPORARY_BATCH.NEXTVAL INTO LTempBatchID FROM DUAL;
            END IF;
            ATempID := LTempBatchID;

            -- Pull the aggregate from the structures list
            LStructureAggregationValue := TakeOffAndGetClob(LStructureAggregationList,'Clob');

            SELECT UpdateXML(LXmlTypeRows
              ,'/node()/ROW/TEMPBATCHID/text()', LTempBatchID
              ,'/node()/ROW/DATECREATED/text()', TO_CHAR(SYSDATE)
              ,'/node()/ROW/DATELASTMODIFIED/text()', TO_CHAR(SYSDATE)
            ) INTO LXmlTypeRows FROM dual;

            --Project List section
            BEGIN
              SELECT XMLSequence(LXmlTables.Extract('/node()/ProjectList/Project/ProjectID'))
              INTO LProjectsSequenceType FROM DUAL
              WHERE ExistsNode(LXmlTables,'/node()/ProjectList/Project/ProjectID')=1;

              FOR LProjectsIndex IN  LProjectsSequenceType.FIRST..LProjectsSequenceType.LAST LOOP
                LProjectID := LProjectsSequenceType(LProjectsIndex).Extract('ProjectID/text()').getNumberVal();
                IF LProjectID IS NOT NULL THEN
                  SELECT Name,Description
                  INTO LProjectName,LProjectDescription
                  FROM VW_Project WHERE ProjectID=LProjectID;

                  SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/PROJECTXML/XMLFIELD/ProjectList/Project[ProjectID='||LProjectID||']/ProjectID'
                  ,XmlType('<ProjectID name="'||LProjectName||'" description="'||LProjectDescription||'">'||LProjectID||'</ProjectID>'))
                  INTO LXmlTypeRows FROM dual;
                END IF;
              END LOOP;
            EXCEPTION
              WHEN NO_DATA_FOUND THEN NULL;
            END;

             --Project Batch List section
            BEGIN
              SELECT XMLSequence(LXmlTables.Extract('/node()/BatchList/Batch/ProjectList/Project/ProjectID'))
              INTO LProjectsSequenceType FROM DUAL
              WHERE ExistsNode(LXmlTables,'/node()/BatchList/Batch/ProjectList/Project/ProjectID')=1;

              FOR LProjectsIndex IN  LProjectsSequenceType.FIRST..LProjectsSequenceType.LAST LOOP
                LProjectID := LProjectsSequenceType(LProjectsIndex).Extract('ProjectID/text()').getNumberVal();
                IF LProjectID IS NOT NULL THEN
                  SELECT Name,Description
                  INTO LProjectName,LProjectDescription
                  FROM VW_Project WHERE ProjectID=LProjectID;

                  SELECT UpdateXML(LXmlTypeRows
                    ,'/node()/ROW/PROJECTXMLBATCH/XMLFIELD/ProjectList/Project[ProjectID='||LProjectID||']/ProjectID'
                    ,XmlType('<ProjectID name="'||LProjectName||'" description="'||LProjectDescription||'">'||LProjectID||'</ProjectID>')
                  )
                  INTO LXmlTypeRows FROM dual;
                END IF;
              END LOOP;
            EXCEPTION
              WHEN NO_DATA_FOUND THEN NULL;
            END;

            --Identifier List section
            BEGIN
              SELECT XMLSequence(LXmlTables.Extract('/node()/IdentifierList/Identifier/IdentifierID'))
              INTO LIdentifiersSequenceType FROM DUAL
              WHERE ExistsNode(LXmlTables,'/node()/IdentifierList/Identifier/IdentifierID')=1;

              FOR LIdentifiersIndex IN  LIdentifiersSequenceType.FIRST..LIdentifiersSequenceType.LAST LOOP
                LIdentifierID := LIdentifiersSequenceType(LIdentifiersIndex).Extract('IdentifierID/text()').getNumberVal();
                IF LIdentifierID IS NOT NULL THEN
                  SELECT Name,Description
                  INTO LIdentifierName,LIdentifierDescription
                  FROM VW_IdentifierType WHERE ID=LIdentifierID;

                  SELECT UpdateXML(
                    LXmlTypeRows
                    , '/node()/ROW/IDENTIFIERXML/XMLFIELD/IdentifierList/Identifier[IdentifierID='||LIdentifierID||']/IdentifierID'
                    , XmlType('<IdentifierID name="'||LIdentifierName||'" description="'||LIdentifierDescription||'">'||LIdentifierID||'</IdentifierID>')
                  )
                  INTO LXmlTypeRows FROM dual;
                END IF;
              END LOOP;
            EXCEPTION
              WHEN NO_DATA_FOUND THEN NULL;
            END;

            --Identifier Batch List section
            BEGIN
              SELECT XMLSequence(LXmlTables.Extract('/node()/BatchList/Batch/IdentifierList/Identifier/IdentifierID'))
              INTO LIdentifiersSequenceType FROM DUAL
              WHERE ExistsNode(LXmlTables,'//node()/BatchList/Batch/IdentifierList/Identifier/IdentifierID')=1;

              FOR LIdentifiersIndex IN  LIdentifiersSequenceType.FIRST..LIdentifiersSequenceType.LAST LOOP
                LIdentifierID:=LIdentifiersSequenceType(LIdentifiersIndex).Extract('IdentifierID/text()').getNumberVal();
                IF LIdentifierID IS NOT NULL THEN
                  SELECT Name,Description
                  INTO LIdentifierName,LIdentifierDescription
                  FROM VW_IdentifierType WHERE ID=LIdentifierID;

                  SELECT UpdateXML(
                    LXmlTypeRows
                    , '/node()/ROW/IDENTIFIERXMLBATCH/XMLFIELD/IdentifierList/Identifier[IdentifierID='||LIdentifierID||']/IdentifierID'
                    ,XmlType('<IdentifierID name="'||LIdentifierName||'" description="'||LIdentifierDescription||'">'||LIdentifierID||'</IdentifierID>')
                  )
                  INTO LXmlTypeRows FROM dual;
                END IF;
              END LOOP;
            EXCEPTION
              WHEN NO_DATA_FOUND THEN NULL;
            END;
          END;

        WHEN 'VW_TEMPORARYCOMPOUND' THEN
          BEGIN
            SELECT SEQ_TEMPORARY_COMPOUND.NEXTVAL INTO LTempCompoundID FROM DUAL;

            LStructureValue := TakeOffAndGetClob(LStructuresList,'Clob');
            LStructureID := LXmlTypeRows.extract('/VW_TemporaryCompound/ROW/STRUCTUREID/text()').getNumberVal();

            IF LStructureID < 0 THEN
              SELECT Structure
              INTO LStructureValue
              FROM VW_Structure_Drawing
              WHERE StructureID = LStructureID;
            END IF;

            LFragmentXmlValue := '<FragmentList>'||TakeOffAndGetClob(LFragmentXmlList,'Clob')||'</FragmentList>';
            LBatchCompFragmentXMLValue := '<BatchComponentFragmentList>'||TakeOffAndGetClob(LBatchComponentFragmentXMLList,'Clob')||'</BatchComponentFragmentList>';
            LNormalizedStructureValue := TakeOffAndGetClob(LNormalizedStructureList,'Clob');

            SELECT UpdateXML(
              LXmlTypeRows
              , '/node()/ROW/TEMPCOMPOUNDID/text()', LTempCompoundID
              , '/node()/ROW/TEMPBATCHID/text()', LTempBatchID
              , '/node()/ROW/DATECREATED/text()', TO_CHAR(SYSDATE)
              , '/node()/ROW/DATELASTMODIFIED/text()', TO_CHAR(SYSDATE))
            INTO LXmlTypeRows FROM dual;
          END;

        WHEN 'VW_TEMPORARYBATCHPROJECT' THEN
          BEGIN
            SELECT UpdateXML(LXmlTypeRows, '/node()/ROW/TEMPBATCHID/text()', LTempBatchID)
            INTO LXmlTypeRows FROM dual;
          END;

        WHEN 'VW_TEMPORARYREGNUMBERSPROJECT' THEN
          BEGIN
            SELECT UpdateXML(LXmlTypeRows,'/node()/ROW/TEMPBATCHID/text()', LTempBatchID)
            INTO LXmlTypeRows FROM dual;
          END;

        ELSE
          --Build Message Logs
          LMessage := LMessage || ' "' || LTableName || '" table is unknown.'||CHR(13);

        END CASE;

      LXmlRows := LXmlTypeRows.getClobVal;

      --PropertyList fields: Replace '&lt;' and '&lt;'  by '<'' and '>''. I can't to do it using "XmlTransform"
      LXmlRows:=regexp_replace(regexp_replace(LXmlRows,'LESS_THAN_SIGN;','<') ,'GREATER_THAN_SIGN;','>');

      --this will probably be made unnecessary by the xslt process
      LPosTagBegin:=1;
      LOOP
        LPosTagBegin := INSTR(LXmlRows,LTagXmlFieldBegin,LPosTagBegin);
        LPosTagEnd := INSTR(LXmlRows,LTagXmlFieldEnd,LPosTagBegin);
      EXIT WHEN (LPosTagBegin=0) or (LPosTagEnd=0);
        LXmlField := SUBSTR(LXmlRows,LPosTagBegin+LENGTH(LTagXmlFieldBegin),LPosTagEnd-(LPosTagBegin+LENGTH(LTagXmlFieldBegin)));
        LXmlField := replace(replace(LXmlField,'<','&lt;') ,'>','&gt;');
        LXmlRows := SUBSTR(LXmlRows,1,LPosTagBegin-1)||LXmlField|| SUBSTR(LXmlRows,LPosTagEnd+LENGTH(LTagXmlFieldEnd),LENGTH(LXmlRows)) ;
      END LOOP;

      --Create the Table Context
      LinsCtx := DBMS_XMLSTORE.newContext(LTableName);
      DBMS_XMLSTORE.clearUpdateColumnList(LinsCtx);

      SELECT XMLSequence(XmlType(LXmlRows).Extract('/node()/node()/node()'))
      INTO LXmlSequenceTypeField FROM DUAL;

      FOR LIndex IN  LXmlSequenceTypeField.FIRST..LXmlSequenceTypeField.LAST LOOP
        DBMS_XMLSTORE.SetupDateColumn (LinsCtx, UPPER(LXmlSequenceTypeField(LIndex).GetRootElement()));
      END LOOP;

      TraceWrite('CreateMcrrTmp_3_BeforeInsert', $$plsql_line, LXmlRows);

      --Insert Rows and get count it inserted
      LRowsInserted := DBMS_XMLSTORE.insertXML(LinsCtx, LXmlRows );

      TraceWrite('CreateMcrrTmp_31_BeforeInsert', $$plsql_line, LXmlRows);
      --Build Message Logs
      LMessage:=LMessage || ' ' || cast (LRowsInserted  as string) || ' Row/s Inserted on "'||LTableName||'".';

      --Close the Table Context
      DBMS_XMLSTORE.closeContext(LinsCtx);

      --Stripped structures and fragment lists must be updated separately in Oracle 10gR2
      IF UPPER(LTableName)='VW_TEMPORARYCOMPOUND' THEN
        IF LBatchCompFragmentXMLValue IS NOT NULL THEN
          IF LStructureValue IS NOT NULL AND LFragmentXmlValue IS NOT NULL THEN
            UPDATE VW_TEMPORARYCOMPOUND
            SET BASE64_CDX = LStructureValue
              , FRAGMENTXML = LFragmentXmlValue
              , BatchCompFragmentXML = LBatchCompFragmentXMLValue
              , NormalizedStructure = LNormalizedStructureValue
            WHERE TempCompoundID = LTempCompoundID;
          ELSIF LStructureValue IS NOT NULL THEN
            UPDATE VW_TEMPORARYCOMPOUND
            SET BASE64_CDX = LStructureValue
              , BatchCompFragmentXML = LBatchCompFragmentXMLValue
              , NormalizedStructure = LNormalizedStructureValue
            WHERE TempCompoundID = LTempCompoundID;
          ELSIF LFragmentXmlValue IS NOT NULL THEN
            UPDATE VW_TEMPORARYCOMPOUND
            SET FRAGMENTXML = LFragmentXmlValue
              , BatchCompFragmentXML = LBatchCompFragmentXMLValue
              , NormalizedStructure = LNormalizedStructureValue
            WHERE TempCompoundID=LTempCompoundID;
          END IF;
        ELSE
          IF LStructureValue IS NOT NULL AND LFragmentXmlValue IS NOT NULL THEN
            UPDATE VW_TEMPORARYCOMPOUND
            SET BASE64_CDX = LStructureValue
              , FRAGMENTXML = LFragmentXmlValue
              , NormalizedStructure = LNormalizedStructureValue
            WHERE TempCompoundID = LTempCompoundID;
          ELSIF LStructureValue IS NOT NULL THEN
            UPDATE VW_TEMPORARYCOMPOUND
            SET BASE64_CDX=LStructureValue
              , NormalizedStructure = LNormalizedStructureValue
            WHERE TempCompoundID = LTempCompoundID;
          ELSIF LFragmentXmlValue IS NOT NULL THEN
            UPDATE VW_TEMPORARYCOMPOUND
            SET FRAGMENTXML = LFragmentXmlValue
              , NormalizedStructure = LNormalizedStructureValue
            WHERE TempCompoundID = LTempCompoundID;
          END IF;
        END IF;

      ELSIF UPPER(LTableName)='VW_TEMPORARYBATCH' THEN
        IF LStructureAggregationValue IS NOT NULL THEN
          UPDATE VW_TEMPORARYBATCH
          SET StructureAggregation = LStructureAggregationValue
          WHERE TempBatchID=LTempBatchID;
        END IF;
      END IF;
    END LOOP;

    --conditionally fetch the new record into the output message
    IF ATempID is not NULL THEN
      UpdateApprovedStatus(ATempID, cSubmittedStatus);
      IF ASectionsList IS NULL OR UPPER(ASectionsList) <> cSectionListEmpty THEN
        RetrieveTemporaryRegistration(ATempID, LXMLRegistryRecord);
      END IF;
      AMessage := CreateRegistrationResponse('Temporary registration created successfully.', NULL, LXMLRegistryRecord);
    END IF;
    TraceWrite( act||'CreateTemporaryRegistration_ended', $$plsql_line,'end' );
  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
      LMessage := LMessage || ' ' || cast(0 as string) || ' Row/s Inserted on "' || LTableName || '".';
        TraceWrite('CreateMcrrTmp_ERROR', $$plsql_line, DBMS_UTILITY.FORMAT_ERROR_STACK || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
        TraceWrite( act||'CreateTemporaryRegistration_ended', $$plsql_line,'end' );
      RAISE_APPLICATION_ERROR(eCreateMultiCompoundRegTmp,CHR(13)||DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE||'Process Log:'||CHR(13)||LMessage||CHR(13));
    END;
  END;

  FUNCTION GetFragmentXML( ATempCompundID  in Number) RETURN CLOB IS
    LFragmentXML  CLOB;

    CURSOR C_Fragments(ATempCompoundID in VW_TEMPORARYCOMPOUND.TempCompoundID%type) IS
        SELECT To_Clob('<Fragment><FragmentID>'||Extract(value(FragmentXML), '/FragmentID/text()').GetStringVal()||'</FragmentID><Code>'||Code||'</Code><Description>'||Description||'</Description><DateCreated>'||CREATED||'</DateCreated><DateLastModified>'||MODIFIED||'</DateLastModified><Structure><Structure molWeight="'||MOLWEIGHT||'" formula="'||FORMULA||'">'||STRUCTURE||'</Structure></Structure></Fragment>') FragmentXML
            FROM VW_FRAGMENT F,Table(SELECT XMLSequence(XmlType(TB.FragmentXML).Extract('/FragmentList/Fragment/FragmentID')) FROM VW_TEMPORARYCOMPOUND TB WHERE ATempCompoundID=TempCompoundID AND TB.FragmentXML IS NOT NULL) FragmentXML
            WHERE F.FragmentID(+)=Extract(value(FragmentXML), '/FragmentID/text()').GetStringVal();
  mod1 varchar2(100); act varchar2(100);
  BEGIN
  dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetFragmentXML_started', $$plsql_line,'start' );
    --** Get Fragment**

    LFragmentXML:='<FragmentList>';
    FOR R_Fragment IN C_Fragments(ATempCompundID) LOOP
        LFragmentXML:=LFragmentXML||R_Fragment.FragmentXML;
    END LOOP;
    LFragmentXML:=LFragmentXML||'</FragmentList>';
    TraceWrite( act||'GetFragmentXML_ended', $$plsql_line,'end' );
    RETURN LFragmentXML;
  END;

  FUNCTION GetIdentifierCompundXML( ATempCompundID  in Number) RETURN CLOB IS
    LIdentifierXML  CLOB;

  CURSOR C_Identifiers(ATempCompoundID in VW_TEMPORARYCOMPOUND.TempCompoundID%type) IS
    SELECT
      To_Clob(
        '<Identifier><IdentifierID '
        || 'Name="' || Name || '" '
        || 'Description="' || Description || '" '
        || 'Active="' ||Active || '">'
        || Extract(value(IdentifierXML), '/Identifier/IdentifierID/text()').GetStringVal()
        || '</IdentifierID><InputText>'
        || Extract(value(IdentifierXML), '/Identifier/InputText/text()').GetStringVal()
        || '</InputText></Identifier>'
      ) IdentifierXML
      FROM
        VW_IdentifierType IT, Table(
          SELECT XMLSequence(XmlType(TC.IdentifierXML).Extract('/IdentifierList/Identifier'))
          FROM VW_TemporaryCompound TC
          WHERE TempCompoundID=ATempCompoundID
            AND TC.IdentifierXML IS NOT NULL
        ) IdentifierXML
      WHERE IT.ID(+) =
        Extract(value(IdentifierXML), '/Identifier/IdentifierID/text()').GetStringVal()
        --ExtractValue(value(IdentifierXML), '/Identifier/IdentifierID')
        ;

      mod1 varchar2(100); act varchar2(100);
  BEGIN
  dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetIdentifierCompundXML_started', $$plsql_line,'start' );
    --** Get Fragment**

    LIdentifierXML:='<IdentifierList>';
    FOR R_Identifier IN C_Identifiers(ATempCompundID) LOOP
        LIdentifierXML:=LIdentifierXML||R_Identifier.IdentifierXML;
    END LOOP;
    LIdentifierXML:=LIdentifierXML||'</IdentifierList>';
    TraceWrite( act||'GetIdentifierCompundXML_ended', $$plsql_line,'end' );
    RETURN LIdentifierXML;
  END;

  /**
  Fetches an entire temporary Registration from the representative 'temp' table-set.
  -->author Jeff D.
  -->since January 2011
  -->param ATempID the primary key of the temporary registration
  -->param AXml the xml string representing the business object
  */
  PROCEDURE RetrieveTemporaryRegistration(
    ATempID in Number
    , AXml out NOCOPY clob
  ) IS
    v_regObjectDefinition xmltype := VRegObjectTemplate;
    v_ctx dbms_xmlgen.ctxHandle;
    v_sql varchar2(4000);
    v_buf clob;
    v_transformedBuf clob;
    v_xbuf xmltype;

    v_structures clob;
    v_normalizedStructures clob;
    v_aggregateStructure clob;

    v_sbiFlag varchar2(255) := VSameBatchesIdentity;
    v_rlsFlag varchar2(255) := vActiveRLS;
    v_isEditableFlag varchar(255);

    v_xsl xmltype := XslMcrrTempFetch;
    v_xpath varchar2(255);
    mod1 varchar2(100); act varchar2(100);

    /* TODO: move thnis function out if generally useful */
    FUNCTION getPropListFields(p_regObjectDef xmltype, p_xpath varchar2) RETURN CLOB IS
      v_csvfields clob;
    mod1 varchar2(100); act varchar2(100);
    BEGIN
      dbms_application_info.read_module(mod1, act); TraceWrite( act||'getPropListFields_started', $$plsql_line,'start' );
      select xmlquery(
        'fn:string-join(//PropertyList/Property/@name,",")'
        passing extract(p_regObjectDef, p_xpath)
        returning content
      ).getStringVal() cols into v_csvfields from dual;
      TraceWrite( act||'getPropListFields_ended', $$plsql_line,'end' );
      RETURN v_csvfields;
    END;

  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'RetrieveTemporaryRegistration_started', $$plsql_line,'start' );
    SetSessionParameter;

    TraceWrite('FetchTempMcrr_0' || to_char(ATempID), $$plsql_line, AXml);

    -- Return the object template, with necessary attributes
    if ( nvl(ATempID, 0) = 0 ) then
      AXml := v_regObjectDefinition.getClobVal();

      -- fetch and apply root node attribute values
      AXml := replace(
        AXml
        , '<MultiCompoundRegistryRecord>'
        , '<MultiCompoundRegistryRecord '
          || 'SameBatchesIdentity="' || v_sbiFlag || '" '
          || 'ModuleName="" '
          || 'ActiveRLS="' || v_rlsFlag || '">'
      );
      TraceWrite( act||'RetrieveTemporaryRegistration_ended', $$plsql_line,'end' );
      RETURN;
    end if;

    -- fetch root node attribute values
    v_isEditableFlag := GetIsEditableTmp(ATempID);

    -- define the sql statement that will fetch the raw xml, using cursors for subnodes
    v_sql :=
    'select
      tb.tempbatchid
      , tb.batchnumber
      , tb.personcreated
      , tb.personapproved
      , tb.SubmissionComments
      , tb.datelastmodified
      , tb.datecreated
      , tb.sequenceid
      , tb.statusid
      , tb.structureaggregation
      , CompoundRegistry.GetFilledPropertyList(''TEMPMIXTURE'', tb.tempbatchid) regproplist
      , tb.identifierxml as regidentifiers -- mixture identifiers
      , cursor (
          select
            trp.id, p.projectid, p.description, p.name, to_char(p.active) active
          from vw_temporaryregnumbersproject trp
            inner join vw_project p on p.projectid = trp.projectid
          where trp.tempbatchid = tb.tempbatchid
          --order by p.projectid
        ) regprojects -- mixture projects
      , CompoundRegistry.GetFilledPropertyList(''TEMPBATCH'', tb.tempbatchid) batchproplist
      , tb.identifierxmlbatch as batchidentifiers -- batch identifiers
      , cursor (
          select
            tbp.id, p.projectid, p.description, p.name, to_char(p.active) active
          from vw_temporarybatchproject tbp
            inner join vw_project p on p.projectid = tbp.projectid
          where tbp.tempbatchid = tb.tempbatchid
          --order by p.projectid
        ) batchprojects -- batch projects
      , cursor(
        select
          tc.tempcompoundid
          , tc.tempbatchid
          , case when tc.formulaweight is not null then tc.formulaweight
            else case
            when tc.normalizedstructure is null then
              cscartridge.molweight(tc.base64_cdx)
            else
              cscartridge.molweight(tc.normalizedstructure)
               end
            end as formulaweight
            , case when tc.molecularformula is not null then tc.molecularformula
            else case
            when tc.normalizedstructure is null then
               cscartridge.formula(tc.base64_cdx, null)
            else
               cscartridge.formula(tc.normalizedstructure, null)
               end
            end as molecularformula
          , tc.personcreated
          , tc.personapproved
          , tc.DateLastModified
          , tc.datecreated
          , tc.sequenceid
          , case
              when tc.drawingtype = 0 then tc.Base64_CDX
              else (
                select dt.default_drawing from drawing_type dt where dt.id = tc.drawingtype
              )
            end as structure
          , tc.drawingtype
          , CompoundRegistry.GetFilledPropertyList(''TEMPSTRUCTURE'', tc.tempcompoundid) structproplist
          , tc.fragmentxml -- compound fragments
          , tc.identifierxml -- compound identifiers
          , tc.structidentifierxml -- structure identifiers
          , case
              when tc.regid is null then 0 -- Related to CSBR-138689, the RegID for temp registry is null, this is a problem only in bulk registration
                    else tc.regid
                end as regid
          , (select regnumber from vw_registrynumber rnc where rnc.regid = tc.regid) as regnumber
          , tc.normalizedstructure
          , tc.usenormalization
          , tc.structureid
          , tc.tag
          , tc.BatchCompFragmentXML -- batch component fragments
          , CompoundRegistry.GetFilledPropertyList(''TEMPCOMPONENT'', tc.tempcompoundid) compproplist
          , CompoundRegistry.GetFilledPropertyList(''TEMPBATCHCOMPONENT'', tc.tempcompoundid) batchcompproplist
        from vw_temporarycompound tc
        where tc.tempbatchid = tb.tempbatchid
        --order by tc.tempcompoundid
      ) components
    from vw_temporarybatch tb
    where tb.tempbatchid = :tempid';

    TraceWrite('FetchTempMcrr_1_DynamicSql' || to_char(ATempID), $$plsql_line, v_sql);

    -- configure the xml context
    v_ctx := dbms_xmlgen.newContext(v_sql);
    dbms_xmlgen.setBindValue(v_ctx, 'tempid', to_char(ATempID));

    dbms_xmlgen.setNullHandling(v_ctx, 2);
    dbms_xmlgen.setRowSetTag(v_ctx, 'MIXTURE');
    dbms_xmlgen.setRowtag(v_ctx, null);

    -- execute and close the context
    v_buf := dbms_xmlgen.getXML(v_ctx);
    dbms_xmlgen.closeContext(v_ctx);

    TraceWrite('FetchTempMcrr_2_RawXml', $$plsql_line, v_buf);

    -- CSBR- if v_buf is null, XmlTransform that follows will raise an exception
    if v_buf is null then
        TraceWrite('RetrieveTemporaryRegistration', $$plsql_line, 'Return because v_buf is null');
        TraceWrite( act||'RetrieveTemporaryRegistration_ended', $$plsql_line,'end' );
        return;
    end if;

    -- clean up the results
    v_buf := dbms_xmlgen.convert(v_buf, 1);
    v_buf := regexp_replace(v_buf, '<?xml version="1.0"?>', null);

    -- strip off structures
    v_aggregateStructure := TakeOffAndGetClobslist(v_buf, '<STRUCTUREAGGREGATION>', NULL, NULL, FALSE);
    v_structures := TakeOffAndGetClobslist(v_buf, '<STRUCTURE>', NULL, NULL, FALSE);
    v_normalizedStructures := TakeOffAndGetClobslist(v_buf, '<NORMALIZEDSTRUCTURE>', NULL, NULL, FALSE);

    TraceWrite('FetchTempMcrr_3_StrippedStructureXml', $$plsql_line, v_buf);
    TraceWrite('FetchTempMcrr_3.25_StrippedStructureXml', $$plsql_line, v_xsl.getClobVal());

    -- transform the xml, using xslt parameters
    select XmlTransform(
      xmltype(v_buf)
      , v_xsl
      , 'sbiFlag="''' || v_sbiFlag || '''"  rlsFlag="''' || replace(v_rlsFlag, ' ', '&nbsp;') || '''"  isEditableFlag="''' || v_isEditableFlag || '''"'
    )
    into v_xbuf from dual;

    -- add the AddIns and validation rules nodes
    AddTags(v_regObjectDefinition, v_xbuf, 'AddIns', null);
    AddTags(v_regObjectDefinition, v_xbuf, 'ValidationRuleList', 'name');

    TraceWrite('FetchTempMcrr_3.5_SinTags', $$plsql_line, v_buf);

    -- enhance the xml with compound fragment details
    for t in (
      select * from XMLTable(
        'for $i in //FragmentList return $i/node()'
        passing v_xbuf
        columns
          "fragid" varchar2(200) path 'FragmentID/text()',
          "node" xmltype path '.'
      ) FragList
    )
    loop
      -- update the master document on the indexed 'Fragment' node
      -- DEBUG dbms_output.put_line(t."fragid");
      -- DEBUG dbms_output.put_line(t."node".getClobVal());
      v_xpath := '//FragmentList/Fragment[FragmentID/text() = ''' || to_char(t."fragid") || ''']';
      select updatexml(
        v_xbuf
        , v_xpath
        , xmlelement(
          "Fragment",
            xmlelement("FragmentID", f.fragmentid),
            xmlelement("Code", f.code),
            xmlelement("Description", f.description),
            xmlelement("DateCreated", f.created),
            xmlelement("DateLastModified", f.modified),
            xmlelement("Structure",
              xmlelement( "Structure",
                xmlattributes( f.molweight as "molWeight", f.formula as "formula" ),
                f.structure
              )
            )
          )
        ) into v_xbuf
      from vw_fragment f
      where f.fragmentid = t."fragid";
    end loop;

    -- enhance the xml with compound identifier details
    for t in (
      select * from XMLTable(
        'for $i in //IdentifierList return $i/node()'
        passing v_xbuf
        columns
          "IdentifierID" varchar2(200) path 'IdentifierID/text()',
          "node" xmltype path '.'
      ) IdentifierList
    )
    loop
      -- update the master document on the indexed 'Identifier' node
      v_xpath := '//IdentifierList/Identifier[IdentifierID/text() = ''' || to_char(t."IdentifierID") || ''']/IdentifierID';
      select updatexml(
        v_xbuf
        , v_xpath
        , xmlelement(
          "IdentifierID", xmlattributes( i.name as "Name",i.Active as "Active" ),i.id)
        ) into v_xbuf
      from vw_IdentifierType i
      where i.id = t."IdentifierID";
    end loop;


    -- add back the structures
    v_transformedBuf := v_xbuf.GetClobVal();

    TraceWrite('FetchTempMcrr_3_TransformedNoStructs', $$plsql_line, v_transformedBuf);

    v_transformedBuf := TakeOnAndGetXml(v_transformedBuf, 'STRUCTURE', v_structures);
    v_transformedBuf := TakeOnAndGetXml(v_transformedBuf, 'STRUCTUREAGGREGATION', v_aggregateStructure);
    v_transformedBuf := TakeOnAndGetXml(v_transformedBuf, 'NORMALIZEDSTRUCTURE', v_normalizedStructures);

    -- FIN
    v_transformedBuf := regexp_replace(v_transformedBuf,'&amp;nbsp;', ' ');
    AXml := v_transformedBuf;
    TraceWrite('FetchTempMcrr', $$plsql_line, AXml);
    TraceWrite( act||'RetrieveTemporaryRegistration_ended', $$plsql_line,'end' );
  END;

  /*
  Autor: Fari
  Date:10-may-20077
  Object:
  Description:
  Pending.
  */
  PROCEDURE UpdateTemporaryRegistration(
    AXml in clob
    , AMessage OUT CLOB
    , ASectionsList in Varchar2:=NULL
  ) IS
    LInsert varchar2(10);
    LDelete varchar2(10);

    LCtx                   DBMS_XMLSTORE.ctxType;
    LXmlTables                XmlType;
    LXslTablesTransformed     XmlType;
    LXmlCompReg               CLOB;
    LXmlRows                  CLOB;
    LXmlTypeRows              XmlType;
    LXmlField                 CLOB;
    LFieldToUpdate            CLOB;
    LSequenceID               Number(8);
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
    LID                       VARCHAR2(1000);

    LPosTagBegin              Number:=0;
    LPosTagEnd                Number:=0;
    LTagXmlFieldBegin         VARCHAR2(10):='<XMLFIELD>';
    LTagXmlFieldEnd           VARCHAR2(11):='</XMLFIELD>';

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
	LTempSequenceID				   Number(8); 	

    LProjectsSequenceType          XmlSequenceType;
    LProjectID                     VW_PROJECT.ProjectID%Type;
    LXmlSequenceTypeField          XmlSequenceType;

    LXslTables XmlType := XslMcrrTempUpdate;
    mod1 varchar2(100); act varchar2(100);

    PROCEDURE SetKeyValue(AID VARCHAR2,AIDTag VARCHAR2,AIDTagEnd VARCHAR2) IS
      LPosTag                   Number:=0;
      LPosTagNull               Number:=0;
      LPosTagEnd                Number:=0;
    mod1 varchar2(100); act varchar2(100);
    BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'SetKeyValue_started', $$plsql_line,'start' );
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
      TraceWrite( act||'SetKeyValue_ended', $$plsql_line,'end' );
    END;

  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'UpdateTemporaryRegistration_started', $$plsql_line,'start' );
    SetSessionParameter;

    TraceWrite('UpdateMcrrTemp_0_AXml', $$plsql_line, AXml);

    LXmlCompReg := AXml;
    LSomeUpdate := False;

    -- Take Out the Structures because the nodes of XmlType don't suport a size grater 64k.
    LFragmentXmlList := TakeOffAndGetClobslist(LXmlCompReg,'<FragmentList',NULL,NULL,TRUE,TRUE);
    LBatchCompFragmentXmlList := TakeOffAndGetClobslist(LXmlCompReg,'<BatchComponentFragmentList',NULL,NULL,TRUE,TRUE);
    LStructuresList := TakeOffAndGetClobslist(LXmlCompReg,'<Structure ','<Structure','<BaseFragment>',TRUE);
    LNormalizedStructureList := TakeOffAndGetClobslist(LXmlCompReg,'<NormalizedStructure',NULL,NULL,TRUE);
    LStructureAggregationList := TakeOffAndGetClobslist(LXmlCompReg,'<StructureAggregation',NULL,NULL,TRUE);

    -- Get the xml
    LXmlTables := XmlType.createXML(LXmlCompReg);

    -- Build a new formatted Xml
    SELECT XmlTransform(LXmlTables, LXslTables)
    INTO LXslTablesTransformed FROM DUAL;

    TraceWrite('UpdateMcrrTemp_1_TranformedNoStructuresXml', $$plsql_line, LXslTablesTransformed.getClobVal());

    --Look over Xml searching each Table and update the rows of it.
    LOOP
      --Search each Table
      LIndex := LIndex+1;
      SELECT LXslTablesTransformed.extract('/MultiCompoundRegistryRecord/node()['||LIndex||']').getClobVal()
      INTO LXmlRows FROM dual;

    EXIT WHEN LXmlRows IS NULL;
      TraceWrite('UpdateMcrrTemp_2_LXmlRows(' || to_char(LIndex) || ')', $$plsql_line, LXmlRows);

      LXmlTypeRows := XmlType(LXmlRows);

      -- get the table name
      select t."RootElementName" into LTableName from  xmltable(
          'for $i in /node()[1] return
            element RetVal {
              element RootElementName {name($i)}
              }'
          passing LXmlTypeRows
          columns
            "RootElementName" varchar2(50) path '/RetVal/RootElementName/text()'
        ) t;


      TraceWrite('UpdateMcrrTemp_2_TableName', $$plsql_line, LTableName);

      -- get the insert/delete CRUD attribute value
      select
        extract(LXmlTypeRows, '/node()/@insert').getClobVal()
        , extract(LXmlTypeRows, '/node()/@delete').getClobVal()
      into LInsert, LDelete
      from dual;

      IF (LInsert is not null) THEN
        LSectionInsert := TRUE;
        TraceWrite('UpdateMcrrTemp_insert="yes"', $$plsql_line, LXmlRows);
      ELSE
        LSectionInsert := FALSE;
      END IF;

      IF (LDelete is not null) THEN
        LSectionDelete := TRUE;
        TraceWrite('UpdateMcrr_delete="yes"', $$plsql_line, LXmlRows);
      ELSE
        LSectionDelete := FALSE;
      END IF;

      -- Only new components can be inserted on update of a temporary 'registration'
      IF LSectionInsert THEN
        LRowsInserted := 0;
		
		--Get the SequenceID for new component from xml
		SELECT extractValue(LXmlTables,'/MultiCompoundRegistryRecord/ComponentList/Component[@insert="yes"]/Compound[@insert="yes"]/RegNumber/SequenceID')      
        INTO LTempSequenceID FROM dual;
        TraceWrite('NewComponent_SequenceID', $$plsql_line, to_char(LTempSequenceID));

        CASE UPPER(LTableName)
          WHEN 'VW_TEMPORARYCOMPOUND' THEN
            BEGIN
              SELECT SEQ_TEMPORARY_COMPOUND.NEXTVAL INTO LTempCompoundID FROM DUAL;
              SetKeyValue(LTempCompoundID, LTempCompoundIDTag, LTempCompoundIDTagEnd);
              SetKeyValue(LTempBatchID, LTempBatchIDTag, LTempBatchIDTagEnd);

              LStructureValue := TakeOffAndGetClob(LStructuresList, 'Clob');

              SELECT extractValue(XmlType.createXML(LXmlRows),'/VW_TemporaryCompound/ROW/STRUCTUREID')
              INTO LStructureID FROM dual;

              IF LStructureID='-1' THEN
                SELECT Structure
                INTO LStructureValue
                FROM VW_Structure_Drawing
                WHERE DrawingType=1;
              END IF;

              LFragmentXmlValue := '<FragmentList>'||TakeOffAndGetClob(LFragmentXmlList,'Clob')||'</FragmentList>';
              LBatchCompFragmentXmlValue := '<BatchComponentFragmentList>'||TakeOffAndGetClob(LBatchCompFragmentXmlList,'Clob')||'</BatchComponentFragmentList>';
              LNormalizedStructureValue := TakeOffAndGetClob(LNormalizedStructureList,'Clob');

              IF NVL(LStructureID,0)<=-2 THEN
                SELECT Structure
                INTO LStructureValue
                FROM VW_Structure_Drawing
                WHERE DrawingType=abs(LStructureID);
              END IF;

              SetKeyValue(SYSDATE,'<DATECREATED>','</DATECREATED>');
              SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
            END;

          WHEN 'VW_TEMPORARYREGNUMBERSPROJECT' THEN
            BEGIN
              null;
            END;

          WHEN 'VW_TEMPORARYBATCHPROJECT' THEN
            BEGIN
              null;
            END;

          ELSE  LMessage:=LMessage || ' "' || LTableName||'" table stranger.';
        END CASE;

        LPosTagBegin:=1;
        LOOP
          LPosTagBegin := INSTR(LXmlRows,LTagXmlFieldBegin,LPosTagBegin);
          LPosTagEnd   := INSTR(LXmlRows,LTagXmlFieldEnd,LPosTagBegin);
        EXIT WHEN (LPosTagBegin=0) or (LPosTagEnd=0);
          LXmlField := SUBSTR(LXmlRows,LPosTagBegin+LENGTH(LTagXmlFieldBegin),LPosTagEnd-(LPosTagBegin+LENGTH(LTagXmlFieldBegin)));
          LXmlField := replace(replace(LXmlField,'<','&lt;') ,'>','&gt;');
          LXmlRows := SUBSTR(LXmlRows,1,LPosTagBegin-1)||LXmlField|| SUBSTR(LXmlRows,LPosTagEnd+LENGTH(LTagXmlFieldEnd),LENGTH(LXmlRows)) ;
        END LOOP;

        --Create the Table Context
        LCtx := DBMS_XMLSTORE.newContext(LTableName);
        DBMS_XMLSTORE.clearUpdateColumnList(LCtx);

        LXmlRows := replace(LXmlRows, 'insert="yes"', '');
        SELECT XMLSequence(XmlType(LXmlRows).Extract('/node()/node()/node()'))
          INTO LXmlSequenceTypeField FROM DUAL;

        FOR LIndex IN  LXmlSequenceTypeField.FIRST..LXmlSequenceTypeField.LAST LOOP
          DBMS_XMLSTORE.SetupDateColumn (LCtx, UPPER(LXmlSequenceTypeField(LIndex).GetRootElement()));
        END LOOP;

        --Insert Rows and get count it inserted
        LRowsInserted := DBMS_XMLSTORE.insertXML(LCtx, LXmlRows );

        --Build Message Logs
        LMessage:=LMessage || ' ' || cast (LRowsInserted  as string) || ' Row/s Inserted on "'||LTableName||'".';

        --Close the Table Context
        DBMS_XMLSTORE.closeContext(LCtx);

        IF UPPER(LTableName)='VW_TEMPORARYCOMPOUND' THEN
          IF LBatchCompFragmentXmlValue IS NOT NULL THEN
            IF LStructureValue IS NOT NULL AND LFragmentXmlValue IS NOT NULL THEN
              UPDATE VW_TEMPORARYCOMPOUND
              SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue, SequenceID=LTempSequenceID
              WHERE TempCompoundID=LTempCompoundID;

            ELSE
              IF LStructureValue IS NOT NULL THEN
                UPDATE VW_TEMPORARYCOMPOUND
                SET BASE64_CDX=LStructureValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue,NormalizedStructure=LNormalizedStructureValue, SequenceID=LTempSequenceID
                WHERE TempCompoundID=LTempCompoundID;
              END IF;

              IF LFragmentXmlValue IS NOT NULL THEN
                UPDATE VW_TEMPORARYCOMPOUND
                SET FRAGMENTXML=LFragmentXmlValue, BatchCompFragmentXML=LBatchCompFragmentXmlValue,NormalizedStructure=LNormalizedStructureValue, SequenceID=LTempSequenceID
                WHERE TempCompoundID=LTempCompoundID;
              END IF;
            END IF;

          ELSE
            IF LStructureValue IS NOT NULL AND LFragmentXmlValue IS NOT NULL THEN
              UPDATE VW_TEMPORARYCOMPOUND
              SET BASE64_CDX=LStructureValue,FRAGMENTXML=LFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue, SequenceID=LTempSequenceID
              WHERE TempCompoundID=LTempCompoundID;
            ELSE
              IF LStructureValue IS NOT NULL THEN
                UPDATE VW_TEMPORARYCOMPOUND
                SET BASE64_CDX=LStructureValue, NormalizedStructure=LNormalizedStructureValue,SequenceID=LTempSequenceID
                WHERE TempCompoundID=LTempCompoundID;
              END IF;

              IF LFragmentXmlValue IS NOT NULL THEN
                UPDATE VW_TEMPORARYCOMPOUND
                SET FRAGMENTXML=LFragmentXmlValue, NormalizedStructure=LNormalizedStructureValue,SequenceID=LTempSequenceID
                WHERE TempCompoundID=LTempCompoundID;
              END IF;
            END IF;
          END IF;
        END IF;

        IF (LRowsInserted>0) THEN
          LSomeUpdate:=True;
        END IF;
        LMessage:=LMessage ||  chr(10) || cast (LRowsInserted  as string) || ' Row/s Inserted on "'||LTableName||'".';

      -- but components and project reference records can be deleted during an update
      ELSIF LSectionDelete THEN

        LRowsDeleted := 0;
        CASE UPPER(LTableName)

          WHEN 'VW_TEMPORARYCOMPOUND' THEN
            BEGIN
              SELECT  XmlType(LXmlRows).extract('node()[1]/ROW[1]/node()[1]').getClobVal()
              INTO LKeyFieldName FROM dual;

              IF LKeyFieldName IS NOT NULL THEN
                LCtx := DBMS_XMLSTORE.newContext(LTableName);
                LKeyFieldName := XMLType(LKeyFieldName).getRootElement();
                DBMS_XMLSTORE.setKeyColumn(LCtx, LKeyFieldName);
                LRowsDeleted := DBMS_XMLSTORE.deleteXML(LCtx, LXmlRows );
                DBMS_XMLSTORE.closeContext(LCtx);
                IF (LRowsDeleted > 0) THEN
                  LSomeUpdate:=True;
                END IF;
              END IF;
              LMessage:=LMessage ||  chr(10) || cast (LRowsDeleted  as string) || ' Row/s Deleted on "'||LTableName||'".';
            END;

          WHEN 'VW_TEMPORARYREGNUMBERSPROJECT' THEN
            BEGIN
              SELECT  XmlType(LXmlRows).extract('node()[1]/ROW[1]/node()[1]/text()').getClobVal()
              INTO LID FROM dual;

              IF NVL(LID, 0) > 0 THEN
                DELETE VW_TemporaryRegNumbersProject v WHERE v.ID = LID;

                LRowsDeleted:=SQL%ROWCOUNT;
                IF (LRowsDeleted>0) THEN
                  LSomeUpdate:=True;
                END IF;
              END IF;
              LMessage:=LMessage ||  chr(10) || cast (LRowsDeleted  as string) || ' Tag/s Deleted on "'||LTableName||'".';
            END;

          WHEN 'VW_TEMPORARYBATCHPROJECT' THEN
            BEGIN
              SELECT  XmlType(LXmlRows).extract('node()[1]/ROW[1]/node()[1]/text()').getClobVal()
              INTO LID FROM dual;

              IF NVL(LID, 0) > 0 THEN
                DELETE VW_TemporaryBatchProject v WHERE v.ID = LID;

                LRowsDeleted:=SQL%ROWCOUNT;
                IF (LRowsDeleted>0) THEN
                  LSomeUpdate:=True;
                END IF;
              END IF;
              LMessage:=LMessage ||  chr(10) || cast (LRowsDeleted  as string) || ' Tag/s Deleted on "'||LTableName||'".';
            END;
        END CASE;
      -- all records are updatable during an update!
      ELSE

        LRowsUpdated := 0;

        SELECT  XmlType(LXmlRows).extract('node()[1]/ROW[1]/node()[1]').getClobVal()
        INTO LKeyFieldName FROM dual;

        IF LKeyFieldName IS NOT NULL THEN
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
                SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
                SELECT  extractvalue(XmlType(LXmlRows),'node()[1]/ROW/node()[1]')
                INTO LTempBatchID FROM dual;

                LStructureAggregationValue:=TakeOffAndGetClob(LStructureAggregationList,'Clob');

                BEGIN
                  SELECT XMLSequence(LXmlTypeRows.Extract('node()[1]/ROW/PROJECTXML/XMLFIELD/ProjectList/Project[@insert="yes"]/ProjectID'))
                  INTO LProjectsSequenceType
                  FROM DUAL
                  WHERE ExistsNode(LXmlTypeRows,'node()[1]/ROW/PROJECTXML/XMLFIELD/ProjectList/Project[@insert="yes"]/ProjectID')=1;

                  FOR LProjectsIndex IN  LProjectsSequenceType.FIRST..LProjectsSequenceType.LAST LOOP
                    LProjectID := LProjectsSequenceType(LProjectsIndex).Extract('ProjectID/text()').getNumberVal();
                    IF LProjectID IS NOT NULL THEN
                      INSERT INTO VW_TemporaryRegNumbersProject (TempBatchID, ProjectID)
                      VALUES (LTempBatchID,LProjectID);
                    END IF;
                  END LOOP;
                EXCEPTION
                  WHEN NO_DATA_FOUND THEN NULL;
                END;

                BEGIN
                  SELECT XMLSequence(LXmlTypeRows.Extract('node()[1]/ROW/PROJECTXMLBATCH/XMLFIELD/ProjectList/Project[@insert="yes"]/ProjectID'))
                  INTO LProjectsSequenceType
                  FROM DUAL
                  WHERE ExistsNode(LXmlTypeRows,'node()[1]/ROW/PROJECTXMLBATCH/XMLFIELD/ProjectList/Project[@insert="yes"]/ProjectID')=1;

                  FOR LProjectsIndex IN  LProjectsSequenceType.FIRST..LProjectsSequenceType.LAST LOOP
                    LProjectID:=LProjectsSequenceType(LProjectsIndex).Extract('ProjectID/text()').getNumberVal();
                    IF LProjectID IS NOT NULL THEN
                      INSERT INTO VW_TemporaryBatchProject (TempBatchID, ProjectID)
                      VALUES (LTempBatchID,LProjectID);
                    END IF;
                  END LOOP;
                EXCEPTION
                  WHEN NO_DATA_FOUND THEN NULL;
                END;
              END;

            WHEN 'VW_TEMPORARYCOMPOUND' THEN
              BEGIN
                SetKeyValue(SYSDATE,'<DATELASTMODIFIED>','</DATELASTMODIFIED>');
                SELECT  extractvalue(XmlType(LXmlRows),'node()[1]/ROW/node()[1]')
                INTO LTempCompoundID FROM dual;

                LStructureValue:=TakeOffAndGetClob(LStructuresList,'Clob');
                LFragmentXmlValue:='<FragmentList>'||TakeOffAndGetClob(LFragmentXmlList,'Clob')||'</FragmentList>';
                IF LFragmentXmlValue='<FragmentList></FragmentList>' THEN
                  LFragmentXmlValue:='';
                END IF;
                LBatchCompFragmentXmlValue:='<BatchComponentFragmentList>'||TakeOffAndGetClob(LBatchCompFragmentXmlList,'Clob')||'</BatchComponentFragmentList>';
                IF LBatchCompFragmentXmlValue='<BatchComponentFragmentList></BatchComponentFragmentList>' THEN
                  LBatchCompFragmentXmlValue:='';
                END IF;
                LNormalizedStructureValue:=TakeOffAndGetClob(LNormalizedStructureList,'Clob');
              END;
            ELSE
              NULL;
            END CASE;

          DBMS_XMLSTORE.clearUpdateColumnList(LCtx);
          LKeyFieldName := XMLType(LKeyFieldName).getRootElement();
          DBMS_XMLSTORE.setKeyColumn(LCtx,LKeyFieldName);

          LFieldIndex := 1;
          LUpdate := FALSE;

          LOOP
            LFieldIndex:=LFieldIndex+1;
            SELECT  XmlType(LXmlRows).extract('node()[1]/ROW/node()['||LFieldIndex||']').getClobVal()
            INTO LFieldToUpdate FROM dual;

            IF LFieldToUpdate IS NOT NULL THEN
              LUpdate:=TRUE;
              LFieldToUpdate:=XMLType(LFieldToUpdate).getRootElement();
            END IF;
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

          -- Delete fragment
          IF LFragmentXmlValue IS NOT NULL THEN
            LRowsDeleted := 0;
            LPosTagBegin := 1;
            LOOP
              LPosTagBegin := INSTR(UPPER(LFragmentXmlValue),'<FRAGMENT DELETE="YES"',LPosTagBegin);
              IF LPosTagBegin=0 THEN
                LPosTagBegin := INSTR(UPPER(LFragmentXmlValue),'<FRAGMENT DELETE=''YES''',LPosTagBegin);
              END IF;
            EXIT WHEN (LPosTagBegin=0);
              LPosTagEnd := INSTR(LFragmentXmlValue,'</Fragment>',LPosTagBegin);
              LFragmentXmlValue:= SUBSTR(LFragmentXmlValue,1,LPosTagBegin-1)||SUBSTR(LFragmentXmlValue,LPosTagEnd+LENGTH('</Fragment>'));
              LRowsDeleted:=LRowsDeleted+1;
              LMessage:=LMessage ||  chr(10) || cast (LRowsDeleted  as string) || ' Tag/s Deleted on FRAGMENTXML".';
            END LOOP;
          END IF;

          -- Delete batchcomponentfragment
          IF LBatchCompFragmentXmlValue IS NOT NULL THEN
            LRowsDeleted := 0;
            LPosTagBegin := 1;
            LOOP
              LPosTagBegin := INSTR(UPPER(LBatchCompFragmentXmlValue),'<BATCHCOMPONENTFRAGMENT DELETE="YES"',LPosTagBegin);
              IF LPosTagBegin=0 THEN
                LPosTagBegin := INSTR(UPPER(LBatchCompFragmentXmlValue),'<BATCHCOMPONENTFRAGMENT DELETE=''YES''',LPosTagBegin);
              END IF;
            EXIT WHEN (LPosTagBegin=0);
              LPosTagEnd := INSTR(LBatchCompFragmentXmlValue,'</BatchComponentFragment>',LPosTagBegin);
              LBatchCompFragmentXmlValue:= SUBSTR(LBatchCompFragmentXmlValue,1,LPosTagBegin-1)||SUBSTR(LBatchCompFragmentXmlValue,LPosTagEnd+LENGTH('</BatchComponentFragment>'));
              LRowsDeleted:=LRowsDeleted+1;
              LMessage:=LMessage ||  chr(10) || cast (LRowsDeleted  as string) || ' Tag/s Deleted on BATCHCOMPONENTFRAGMENTXML".';
            END LOOP;
          END IF;

          IF UPPER(LTableName) = 'VW_TEMPORARYCOMPOUND' THEN
            IF LBatchCompFragmentXmlValue IS NOT NULL THEN
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
              SELECT
              extractValue(LXmlTables,'/MultiCompoundRegistryRecord/RegNumber/SequenceID')
              INTO LSequenceID
              FROM dual;
              UPDATE VW_TEMPORARYBATCH
              SET StructureAggregation=LStructureAggregationValue,sequenceid = LSequenceID
              WHERE TempBatchID=LTempBatchID;

              LRowsUpdated:=SQL%ROWCOUNT;
            END IF;
          END IF;
        END IF;

        LMessage := LMessage || ' ' || cast(LRowsUpdated as string) || ' Row/s Updated on "' || LTableName || '".' || chr(13) ;
      END IF;
    END LOOP;
    IF LSomeUpdate THEN
      AMessage := null;
    TraceWrite( act||'UpdateTemporaryRegistration_ended', $$plsql_line,'end' );
    /* JED: We can only retrieve 'the' registration if there was only ONE record registration, lol!
      IF LRegNumber is not NULL then
        begin
          if UPPER(ASectionsList) <> cSectionListEmpty then
            RetrieveMultiCompoundRegistry(LRegNumber, LXMLRegistryRecord, ASectionsList);
          end if;
          AMessage := CreateRegistrationResponse(LBriefMessage, null, LXMLRegistryRecord);
        end;
      END IF;
    */
    ELSE
      RAISE_APPLICATION_ERROR(eGenericException, AppendError('No data to update.'));
    END IF;

  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
      LMessage := LMessage || ' ' || cast(LRowsUpdated as string) || ' Row/s Updated on "' || LTableName || '".';
      TraceWrite('UpdateMcrrTemp_ERROR', $$plsql_line, AppendError('UpdateTemporaryRegistration', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
      TraceWrite( act||'UpdateTemporaryRegistration_ended', $$plsql_line,'end' );
      RAISE_APPLICATION_ERROR(eUpdateMultiCompoundRegTmp, AppendError('UpdateTemporaryRegistration', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
  END;

  PROCEDURE DeleteTemporaryRegistration(ATempID  in Number) IS
    LIsNotEditableDeletingTmp EXCEPTION;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
  dbms_application_info.read_module(mod1, act); TraceWrite( act||'DeleteTemporaryRegistration_started', $$plsql_line,'start' );

    IF (GetIsEditableTmp (ATempID)='False' or CanDeleteTemp(ATempID)='False') then
      RAISE LIsNotEditableDeletingTmp;
    END IF;

--    insert into FOR_DELETE_TEMP_REC select ATempID FROM dual;
    DELETE VW_TemporaryCompound WHERE TempBatchID = ATempID;
    DELETE VW_TemporaryBatchProject WHERE TempBatchID = ATempID;
    DELETE VW_TemporaryRegNumbersProject WHERE TempBatchID = ATempID;
    DELETE VW_TemporaryBatch WHERE TempBatchID = ATempID;
    TraceWrite( act||'DeleteTemporaryRegistration_ended', $$plsql_line,'end' );
    -- JED: removed this error temporarily. Business objects are inadvertantly, but harmlessly,
    --      invoking this method after promoting a temporary record to permanent, despite the fact
    --      that NOT all temporary records come from the temp tables (they can also come from
    --      coedb.coegenericobject)
    -- Suggest we incorporate a readonly RegistryRecord property that indicates the SOURCE of the record
    -- so we can process it properly
    /*
    IF SQL%NOTFOUND THEN
        RAISE_APPLICATION_ERROR(eGenericException, 'TempID '||ATempID||' doesn''t exist. 0 Row Deleted on VW_TemporaryBatch.');
    END IF;
    */
  EXCEPTION
    WHEN LIsNotEditableDeletingTmp THEN
      RAISE_APPLICATION_ERROR(eIsNotEditableDeletingTmp, 'You are not authorized to delete this record. (Temporary Registry# '||ATempID||')');
    WHEN OTHERS THEN
    BEGIN
      RAISE_APPLICATION_ERROR(eDeleteMultiCompoundRegTmp, AppendError('DeleteMultiCompoundRegTmp', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
  END;

  /*
  Autor: Fari
  Date:17-May-07
  */
  PROCEDURE RetrieveCompoundRegistryList(AXmlRegNumbers in clob, AXmlCompoundList out NOCOPY clob) IS
    LXml                      CLOB:='';
    LXmlList                  CLOB:='';
    LSectionList   CONSTANT   VARCHAR2(500):='Compound,Identifier,Fragment';
    LRegNumber                VW_RegistryNumber.RegNumber%type;
    LIndex                    Number;
    LPosBegin                 Number;
    LPosEnd                   Number;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'RetrieveCompoundRegistryList_started', $$plsql_line,'start' );
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
          LXml:=regexp_replace(LXml,'</MultiCompoundRegistryRecord>','');

          LXmlList:=LXmlList||CHR(10)||LXml;
      EXCEPTION
         WHEN OTHERS THEN
         BEGIN
           IF INSTR(DBMS_UTILITY.format_error_stack,eNoRowsReturned)<>0 THEN NULL; --Though a Compound doesn't exist to get the others
           ELSE
                TraceWrite( act||'RetrieveCompoundRegistryList_ended', $$plsql_line,'end' );
              RAISE_APPLICATION_ERROR(eRetrieveCompoundRegistryList,DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
           END IF;
         END;
      END;
      LIndex:=LIndex+1;
    END LOOP;
    AXmlCompoundList:='<CompoundList>'||CHR(10)||LXmlList||CHR(10)||'</CompoundList>';
    $if CompoundRegistry.Debuging $then InsertLog('RetrieveCompoundRegistryList','List->'||AXmlCompoundList); $end null;
    TraceWrite( act||'RetrieveCompoundRegistryList_ended', $$plsql_line,'end' );
  EXCEPTION
    WHEN OTHERS THEN
    BEGIN
        TraceWrite( act||'RetrieveCompoundRegistryList_ended', $$plsql_line,'end' );
        RAISE_APPLICATION_ERROR(eRetrieveCompoundRegistryList, AppendError('RetrieveCompoundRegistryList', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
    END;
  END;

  /**
  Workhorse method for "ConvertHitlistTempsToPerm" method.
  Given a list of temporary registry IDs and duplicate-resolution instructions, fetch
  the records by ID, create permanent records, and eliminate the temporary records.
  -->author jed
  -->since December 2009
  -->param ATempIds           the collection of temporary registry identifiers to act upon
  -->param ADuplicateAction   indicator of duplication-resolution strategy to use
  -->param ADescription       processing note added by user
  -->param AUserID            user responsible for performing the action
  -->param AMessage           output of abbreviated responses (xml string) wrapped in a parent element
  -->param ARegistration      defaults to 'Y'
  -->param ARegNumGeneration  defaults to 'Y'
  -->param AConfigurationID   defaults to 1
  -->param ASectionsList      defaults to NULL
  */
  PROCEDURE ConvertTempRegRecordsToPerm(
    ATempIds IN tNumericIdList
    , ADuplicateAction IN CHAR
    , ADescription IN VARCHAR2
    , AUserID IN VARCHAR2
    , ALogID OUT NUMBER
    , AMessage OUT NOCOPY CLOB
    , ARegistration IN CHAR := 'Y'
    , ARegNumGeneration IN CHAR := 'Y'
    , AConfigurationID IN Number := 1
    , ASectionsList IN Varchar2 := NULL
  ) IS
    LRegistryResponses CLOB;
    -- very important to init "LRegistryResponseList" for concatenation
    LRegistryResponseList CLOB := empty_clob();
    LRegistryResponseItem CLOB;
    LErrorCount NUMBER := 0;
    Lthis_tempid NUMBER;
    Lthis_regxml CLOB;
    Lthis_action Char;
    Lthis_regnum VW_RegistryNumber.RegNumber%type;
    LExtractedMessage CLOB;
    LExtractedError CLOB;
    LExtractedResult CLOB;
    LIterator NUMBER;
    LCounter NUMBER := 0;
    LDuplicateResolutionAction CHAR := ADuplicateAction;
    LRegID VW_RegistryNumber.RegID%type;
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'ConvertTempRegRecordsToPerm_started', $$plsql_line,'start' );

    --by-pass erroneous 'Temporary' duplicate-resolution instructions
    IF (ADuplicateAction = 'T') THEN
      LDuplicateResolutionAction := 'N';
    END IF;

    --create the log header for the bulk-migration event
    LogBulkRegistrationId(ALogID, LDuplicateResolutionAction, AUserID, ADescription);

    --attempt the migration for each temporary registry record
    LIterator := ATempIds.FIRST;
    LOOP
    EXIT WHEN LIterator IS NULL;
      Lthis_tempid := ATempIds(LIterator);
      IF (GetIsEditableTmp(Lthis_tempid)='False') THEN
         LRegistryResponseItem := CreateRegistrationResponse('You are not authorized to register this Temporary Registry.', 'You are not authorized to register this Temporary Registry.', NULL);
         Lthis_action:='N';
      ELSIF(CanRegister(Lthis_tempid)='False') THEN
         LRegistryResponseItem := CreateRegistrationResponse('This record was not approved and thus cannot be registered.', 'This record was not approved and thus cannot be registered.', NULL);
         Lthis_action:='N';
      ELSIF (CheckBatchPrefixForRegister(Lthis_tempid)='FALSE') THEN
         LRegistryResponseItem := CreateRegistrationResponse('Batch Prefix is required for this record and thus cannot be registered.', 'Batch Prefix is required for this record and thus cannot be registered.', NULL);
         Lthis_action:='N';
      ELSE
        --fetch the record
        RetrieveTemporaryRegistration(Lthis_tempid, Lthis_regxml);
        --insert it to permanent
        BEGIN
          LoadMultiCompoundRegRecord(
            Lthis_regxml
            , LDuplicateResolutionAction
            , Lthis_action
            , LRegistryResponseItem
            , Lthis_regnum
            , ARegistration
            , ARegNumGeneration
            , AConfigurationID
            , ASectionsList
          );
        END;
      END IF;

      --determine errors
      IF LRegistryResponseItem IS NOT NULL THEN
        ExtractRegistrationResponse(LRegistryResponseItem, LExtractedMessage, LExtractedError, LExtractedResult);
        --create the log detail for this particular temp record
        IF LExtractedError IS NOT NULL THEN
            LErrorCount := LErrorCount + 1;
            LogBulkregistration(ALogID, Lthis_tempid, Lthis_action, NULL, NULL, LExtractedMessage);
        ELSE
            --delete the temp record
            LogBulkregistration(ALogID, NULL, Lthis_action, Lthis_regnum, NULL, LExtractedMessage);
            --Make the status be registered:
            select regid into LRegID from VW_RegistryNumber where regnumber=Lthis_regnum;
            UPDATE VW_MIXTURE SET STATUSID=cRegisteredStatus WHERE REGID = LRegID;
            UPDATE VW_BATCH SET STATUSID=cRegisteredStatus WHERE REGID = LRegID;

            UpdateApprovedStatus(Lthis_tempid, cSubmittedStatus); -- Make temporary record deletable

            DeleteTemporaryRegistration(Lthis_tempid);
        END IF;
        --TODO: as necessary, reconstruct the individual message items with focus on brevity
      END IF;

      --append to the message list
      IF LRegistryResponseList IS NULL THEN
        LRegistryResponseList := LRegistryResponseItem;
      ELSE
        LRegistryResponseList := LRegistryResponseList || LRegistryResponseItem;
      END IF;

      --maintain counters
      LCounter := LCounter + 1;
      LIterator := ATempIds.NEXT(LIterator);

      COMMIT;
    END LOOP;

    --fill and 'close' the messages container
    LRegistryResponses :=
      '<Responses message="'
      || to_char(LCounter) || ' records processed, with ' || to_char(LErrorCount)|| ' errors (unresolved duplicates)">'
      || LRegistryResponseList
      || '</Responses>';

    --set the messages container as the output
    AMessage := LRegistryResponses;
    TraceWrite( act||'ConvertTempRegRecordsToPerm_ended', $$plsql_line,'end' );
    RETURN;
  EXCEPTION
    WHEN OTHERS THEN
       TraceWrite( act||'ConvertTempRegRecordsToPerm_ended', $$plsql_line,'end' );
       RAISE_APPLICATION_ERROR(EGenericException, AppendError('ConvertTempRegRecordsToPerm', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
  END;

  /**
  Fetches a list of batch IDs based on a mixture ID.
  -->author jed
  -->since April 2011
  -->param p_mixid identifier of a mixture
  -->param p_batchId_list collection output of batch identifiers
  */
  PROCEDURE GetBatchIDListByMixtureId(
    p_mixid IN NUMBER
    , p_batchId_list OUT tNumericIdList
  )
  IS
    v_idList tNumericIdList;
    CURSOR CRecords(someID NUMBER) IS (
      select
        mrn.regnumber
        , mrn.mixtureid
        , mrn.regid
        , b.batchnumber
        , b.batchid
      from vw_batch b
        inner join vw_mixture_batchcomponent mbc on mbc.batchid = b.batchid
        inner join vw_mixture_regnumber mrn on mrn.mixtureid = mbc.mixtureid
      where mrn.mixtureid = someID
    );
    itemRow CRecords%rowtype;
    v_index integer:=0;
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetBatchIDListByMixtureId_started', $$plsql_line,'start' );

    TraceWrite('GetBatchIDListByMixtureId_MixtureID', $$plsql_line, to_char(p_mixid));
    FOR itemRow in CRecords(p_mixid) LOOP
      v_index:=v_index+1;
      v_idList( v_index) := itemRow.batchid;
      TraceWrite('GetBatchIDListByMixtureId_BatchID', $$plsql_line, to_char(itemRow.batchid));
    END LOOP;
    p_batchId_list := v_idList;
    TraceWrite( act||'GetBatchIDListByMixtureId_ended', $$plsql_line,'end' );
  END;

  /**
  Fetches a list of registration IDs based on a result-set identifier.
  (GUI-derived identifier collections - 'hits', often the result-set from a search - are
  saved so they can later be acted upon en masse.)
  -->author jed
  -->since December 2009
  -->param Ahitlistid identifier of a saved 'hit' list of temporary registrations
  -->param AId        collection output of temporary registry identifiers
  */
  PROCEDURE GetHitlistItemIdsFromHitlistId(
    Ahitlistid IN NUMBER
    , AId OUT tNumericIdList
  )
  IS
    LIdList tNumericIdList;
    LCounter NUMBER := 0;
    CURSOR CTempRecordList(someId NUMBER) IS
    SELECT item.id FROM COEDB.COESAVEDHITLIST item
    WHERE item.hitListId = someId;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetHitlistItemIdsFromHitlistId_started', $$plsql_line,'start' );
    FOR itemRow in CTempRecordList(Ahitlistid) LOOP
      LIdList(LCounter) := itemRow.id;
      LCounter := LCounter + 1;
    END LOOP;
    AId := LIdList;
    TraceWrite( act||'GetHitlistItemIdsFromHitlistId_ended', $$plsql_line,'end' );
  END;

  /**
  Registration records can be either 'temporary' (requiring review) or 'permanent'.
  This procedure finalizes the review process by bulk-promoting 'temporary' records
  to permanent status, using a list identifier as a source of temporary record IDs.
  -->author jed
  -->since December 2009
  -->see "ConvertHitlistTempsToPerm" method, which does the real work
  -->param Ahitlistid         identifier of a saved 'hit' list of temporary registrations
  -->param ADuplicateAction   indicator of duplication-resolution strategy to use
  -->param ADescription       processing note added by user
  -->param AUserID            user responsible for performing the action
  -->param AMessage           output of abbreviated responses (xml string) wrapped in a parent element
  -->param ARegistration      defaults to 'Y'
  -->param ARegNumGeneration  defaults to 'Y'
  -->param AConfigurationID   defaults to 1
  -->param ASectionsList      defaults to NULL
  */
  PROCEDURE ConvertHitlistTempsToPerm(
    Ahitlistid IN NUMBER
    , ADuplicateAction IN CHAR
    , ADescription IN VARCHAR2
    , AUserID IN VARCHAR2
    , ALogID OUT NUMBER
    , AMessage OUT NOCOPY CLOB
    , ARegistration IN CHAR := 'Y'
    , ARegNumGeneration IN CHAR := 'Y'
    , AConfigurationID IN NUMBER := 1
    , ASectionsList IN VARCHAR2 := NULL
  ) IS
    LIdList tNumericIdList;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'ConvertHitlistTempsToPerm_started', $$plsql_line,'start' );
    GetHitlistItemIdsFromHitlistId( Ahitlistid, LIdList );
    IF LIdList IS NOT NULL AND LIdList.COUNT > 0 THEN
      ConvertTempRegRecordsToPerm(
        LIdList
        , ADuplicateAction
        , ADescription
        , AUserID
        , ALogID
        , AMessage
        , ARegistration
        , ARegNumGeneration
        , AConfigurationID
        , ASectionsList
      );
    END IF;
    TraceWrite( act||'ConvertHitlistTempsToPerm_ended', $$plsql_line,'end' );
  END;

  PROCEDURE MoveBatch(ABatchID in Number,ARegNumber in VW_RegistryNumber.RegNumber%type) IS
    LBatchCount            Integer;
    LValidRegistry         Integer;
    LTargetComponentCount  Integer;
    LTargetBatchID         Integer;
    LTargetRegID           Integer;
    LTargeMixtureID        Integer;
    LBatchRegID            Integer;
    LCompoundCountBatch    Integer;
    LCompoundCountRegistry Integer;
    LSourceMixtureID       Integer;
    LMatchFragment         Integer;
    LErrorMessage          Varchar2(2000);
    LErrorCode             Integer;
    LSequenceID            Integer;
    LBatchNumber           VW_Batch.BatchNumber%Type;
    LFullregNumber         VW_Batch.FullRegNumber%Type;
    LXMLRegistryRecord     CLOB;
    LRLSState              Boolean;
    LBatchNumber_Present  Integer;
    CHEMINVDB2_EXISTS Integer;
    CHEMINV_INTEGRATED Integer:=0;
    InvCompounds_Updated Integer;

    LNewBatchList tNumericIdList;

    TYPE T_BatchComponent_Target IS
    RECORD (
      BatchID VW_BatchComponent.BatchID%Type
      , OrderIndex VW_BatchComponent.OrderIndex%Type
      , Percentage VW_BatchComponent.Percentage%Type
    );
    R_BatchComponent_Target T_BatchComponent_Target;

    CURSOR C_BatchComponent_Target IS
      SELECT BatchID, OrderIndex, Percentage
      FROM VW_BatchComponent
      WHERE BatchID = ABatchID
      ORDER BY MixtureComponentID;

    CURSOR C_Mixture_Component_Target IS
      SELECT MixtureComponentID
      FROM VW_Mixture_Component
      WHERE MixtureID = LTargeMixtureID
      ORDER BY MixtureComponentID ;

  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'MoveBatch_started', $$plsql_line,'start' );

    SELECT RegID INTO LBatchRegID FROM  VW_Batch WHERE BatchID=ABatchID;
    SELECT BatchNumber INTO LBatchNumber_Present FROM  VW_Batch WHERE BatchID=ABatchID;
    SELECT COUNT(1) INTO LBatchCount FROM VW_Batch WHERE RegID = LBatchRegID;

    --Ulises 9-12-08: Check destiny RegNum before moving.
    SELECT COUNT(1)INTO LValidRegistry FROM VW_RegistryNumber
    WHERE Upper(REGNUMBER) = Upper(ARegNumber);

    -- determine if the inventory schema exists
    SELECT COUNT(*) INTO CHEMINVDB2_EXISTS
    FROM ALL_TABLES WHERE owner = 'CHEMINVDB2' and ROWNUM = 1;
    -- Determine if integration is enabled (registers inv componds exist)
    IF CHEMINVDB2_EXISTS = 1 THEN
        EXECUTE IMMEDIATE 'BEGIN SELECT COUNT(*) into :1 from CHEMINVDB2.INV_COMPOUNDS where reg_id_fk is not null and rownum = 1; END;'
        USING OUT CHEMINV_INTEGRATED;
    END IF;


    -- exit with an error if any of these conditions are met
    IF LBatchCount = 0 THEN
      Raise_Application_Error (eNoRowsReturned, 'No rows returned. The batch was not moved.');
    ELSIF LBatchCount = 1 THEN
      Raise_Application_Error (eOnlyOneBatch, 'The Batch''s Registry has an only Batch. The Batch was not moved.');
    ELSIF LValidRegistry = 0 THEN
      Raise_Application_Error (eInvalidRegNum, 'The given RegNum does not exist.');
    END IF;

    -- otherwise, continue processing
    SELECT
      RN.RegID
      , MRN.MixtureID
      , RN.SequenceID
    INTO
      LTargetRegID
      , LTargeMixtureID
      , LSequenceID
    FROM VW_RegistryNumber RN,VW_Mixture_RegNumber MRN
    WHERE MRN.RegID=RN.RegID AND RN.RegNumber=ARegNumber;

    SELECT Count(1) INTO LCompoundCountRegistry
    FROM VW_Mixture_Component
    WHERE MixtureID=(SELECT MixtureID FROM VW_Mixture_RegNumber WHERE REGID=LTargetRegID);

    SELECT MixtureID
    INTO LSourceMixtureID
    FROM VW_Mixture_RegNumber WHERE REGID=LBatchRegID;

    SELECT Count(1)
    INTO LCompoundCountBatch
    FROM VW_Mixture_Component
    WHERE MixtureID=LSourceMixtureID;

    IF LCompoundCountRegistry = LCompoundCountBatch THEN

      SELECT Min(BatchID) INTO LTargetBatchID
      FROM VW_Batch
      WHERE RegID=LTargetRegID AND BatchID<>ABatchID;

      SELECT Count(1) INTO LTargetComponentCount
      FROM VW_BatchComponent
      WHERE BatchID=LTargetBatchID;

      SELECT RN.RegID, MRN.MixtureID
      INTO LTargetRegID, LTargeMixtureID
      FROM  VW_RegistryNumber RN,VW_Mixture_RegNumber MRN
      WHERE MRN.RegID=RN.RegID AND RN.RegNumber=ARegNumber;

      IF VSameBatchesIdentity='True' THEN

                SELECT Count(1)
                    INTO LMatchFragment
                    FROM
                        (SELECT CF_Source.FragmentID
                            FROM VW_Compound_Fragment CF_Source, VW_Mixture_Component MC_Source
                            WHERE  CF_Source.CompoundID=MC_Source.CompoundID AND MC_Source.MixtureID=LSourceMixtureID
                            GROUP BY CF_Source.FragmentID
                        MINUS
                        SELECT CF_Target.FragmentID
                            FROM VW_Compound_Fragment CF_Target, VW_Mixture_Component MC_Target
                            WHERE  CF_Target.CompoundID=MC_Target.CompoundID AND MC_Target.MixtureID=LTargeMixtureID
                            GROUP BY CF_Target.FragmentID);

                IF LMatchFragment<>0 THEN
                    LErrorMessage:='The Fragment List from source and target record is different. New fragment list was applied to the moved batch.';
                    LErrorCode:= eFragmentNotMatchIdentityTrue;
                END IF;

                DELETE VW_BatchComponentFragment BC
                    WHERE BatchComponentID IN (SELECT ID FROM VW_BatchComponent WHERE BatchID=ABatchID);
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'DELETE VW_BatchComponentFragmen: ' || SQL%ROWCOUNT); $end null;

                OPEN C_BatchComponent_Target;
                 $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'C_VW_BatchComponent: ' || C_BatchComponent_Target%ROWCOUNT); $end null;

                DELETE VW_BatchComponent BC1
                    WHERE BatchID=ABatchID;
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'DELETE VW_BatchComponent: ' || SQL%ROWCOUNT); $end null;
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'C_VW_BatchComponent: ' || C_BatchComponent_Target%ROWCOUNT); $end null;

                /*INSERT INTO VW_Compound_Fragment(ID,FragmentID,CompoundID)
                    (SELECT Seq_Compound_Fragment.NextVal,CF_Source.FragmentID, MC_Target.CompoundID
                        FROM VW_Compound_Fragment CF_Source ,VW_Mixture_Component MC_Source, VW_Mixture_Component MC_Target
                        WHERE CF_Source.CompoundID=MC_Source.CompoundID AND MC_Source.MixtureID=LSourceMixtureID AND MC_Target.MixtureID=LTargeMixtureID AND
                              CF_Source.FragmentID NOT IN (SELECT CF_Target.FragmentID
                                                        FROM VW_Compound_Fragment CF_Target,VW_Mixture_Component MC_Target
                                                        WHERE MC_Target.CompoundID=CF_Target.CompoundID AND MC_Target.MixtureID=LTargeMixtureID));*/
                 $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'INSERT INTO VW_Compound_Fragment: ' || SQL%ROWCOUNT); $end null;

                FOR R_Mixture_Component_Target IN C_Mixture_Component_Target LOOP
                    FETCH C_BatchComponent_Target INTO R_BatchComponent_Target;
                    INSERT INTO VW_BatchComponent(ID,BATCHID,MIXTURECOMPONENTID,ORDERINDEX,PERCENTAGE)
                        VALUES(SEQ_BatchComponent.NEXTVAL,R_BatchComponent_Target.BATCHID,R_Mixture_Component_Target.MIXTURECOMPONENTID,R_BatchComponent_Target.ORDERINDEX,R_BatchComponent_Target.PERCENTAGE);
                     $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'INSERT INTO VW_BatchComponent: ' || SQL%ROWCOUNT); $end null;
                 END LOOP;

                 CLOSE C_BatchComponent_Target;

                INSERT INTO VW_BatchComponentFragment(BATCHCOMPONENTID,COMPOUNDFRAGMENTID,EQUIVALENT,ORDERINDEX)
                    SELECT BC.ID,CF.ID,BCF_Target.Equivalent,1
                        FROM VW_Compound_Fragment CF, VW_Mixture_Component MC_Target,VW_BatchComponent BC,VW_BatchComponentFragment BCF_Target, VW_BatchComponent BC_Target
                        WHERE MC_Target.MixtureComponentID= BC.MixtureComponentID AND MC_Target.CompoundID=CF.CompoundID AND MC_Target.MixtureID=LTargeMixtureID AND
                        NOT EXISTS ( SELECT 1 FROM VW_BatchComponentFragment BCF1 WHERE BCF1.BatchComponentID=BC.ID AND BCF1.CompoundFragmentID=CF.ID) AND
                        BCF_Target.CompoundFragmentID=CF.ID AND BCF_Target.BatchComponentID=BC_Target.ID AND BC_Target.BatchID=LTargetBatchID AND MC_Target.MixtureComponentID=BC_Target.MixtureComponentID;

                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'INSERT INTO VW_BatchComponentFragment: ' || SQL%ROWCOUNT); $end null;

      ELSE

                SELECT Count(1)
                    INTO LMatchFragment
                    FROM
                        (SELECT CF_Source.FragmentID
                            FROM VW_Compound_Fragment CF_Source, VW_Mixture_Component MC_Source
                            WHERE  CF_Source.CompoundID=MC_Source.CompoundID AND MC_Source.MixtureID=LSourceMixtureID
                            GROUP BY CF_Source.FragmentID
                        MINUS
                        SELECT CF_Target.FragmentID
                            FROM VW_Compound_Fragment CF_Target, VW_Mixture_Component MC_Target
                            WHERE  CF_Target.CompoundID=MC_Target.CompoundID AND MC_Target.MixtureID=LTargeMixtureID
                            GROUP BY CF_Target.FragmentID);

                IF LMatchFragment<>0 THEN
                    LErrorMessage:='The Fragment List from source and target record is different. New fragment list was applied to the moved batch.';
                    LErrorCode:= eFragmentNotMatchIdentityTrue;
                END IF;

                INSERT INTO VW_Compound_Fragment(ID,FragmentID,CompoundID)
                    (SELECT Seq_Compound_Fragment.NextVal,CF_Source.FragmentID, MC_Target.CompoundID
                        FROM  VW_BatchComponentFragment BCF_Source, VW_BatchComponent BC_Source, VW_Compound_Fragment CF_Source ,VW_Mixture_Component MC_Source, VW_Mixture_Component MC_Target
                        WHERE BC_Source.BatchID=ABatchID AND BCF_Source.BatchComponentID=BC_Source.ID AND BCF_Source.CompoundfragmentID= CF_Source.ID AND CF_Source.CompoundID=MC_Source.CompoundID AND MC_Source.MixtureID=LSourceMixtureID AND
                              MC_Target.MixtureID=LTargeMixtureID AND
                              CF_Source.FragmentID NOT IN (SELECT CF_Target.FragmentID
                                                        FROM VW_Compound_Fragment CF_Target,VW_Mixture_Component MC_Target
                                                        WHERE MC_Target.CompoundID=CF_Target.CompoundID AND MC_Target.MixtureID=LTargeMixtureID));
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'INSERT INTO VW_Compound_Fragment: ' || SQL%ROWCOUNT); $end null;

                OPEN C_BatchComponent_Target;

                FOR R_Mixture_Component_Target IN C_Mixture_Component_Target LOOP
                    FETCH C_BatchComponent_Target INTO R_BatchComponent_Target;
                    INSERT INTO VW_BatchComponent(ID,BATCHID,MIXTURECOMPONENTID,ORDERINDEX,PERCENTAGE)
                        VALUES(SEQ_BatchComponent.NEXTVAL,R_BatchComponent_Target.BATCHID,R_Mixture_Component_Target.MIXTURECOMPONENTID,R_BatchComponent_Target.ORDERINDEX,R_BatchComponent_Target.PERCENTAGE);
                    $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'INSERT INTO VW_BatchComponent: ' || SQL%ROWCOUNT); $end null;
                END LOOP;

                CLOSE C_BatchComponent_Target;

                INSERT INTO VW_BatchComponentFragment(BATCHCOMPONENTID,COMPOUNDFRAGMENTID,EQUIVALENT,ORDERINDEX)
                    SELECT BC_Target.ID,CF_Target.ID,BCF_Source.Equivalent,BCF_Source.OrderIndex
                        FROM VW_Compound_Fragment CF_Target, VW_Mixture_Component MC_Target,VW_BatchComponent BC_Target,
                             VW_BatchComponentFragment BCF_Source, VW_BatchComponent BC_Source, VW_Compound_Fragment CF_Source ,VW_Mixture_Component MC_Source
                        WHERE BC_Target.BatchID=ABatchID AND MC_Target.MixtureComponentID= BC_Target.MixtureComponentID AND MC_Target.CompoundID=CF_Target.CompoundID AND MC_Target.MixtureID=LTargeMixtureID AND
                              CF_Target.FragmentID = CF_Source.FragmentID AND
                              BC_Source.BatchID=ABatchID AND BCF_Source.BatchComponentID=BC_Source.ID AND BCF_Source.CompoundfragmentID= CF_Source.ID AND CF_Source.CompoundID=MC_Source.CompoundID AND MC_Source.MixtureID=LSourceMixtureID;
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'INSERT INTO VW_BatchComponentFragment: ' || SQL%ROWCOUNT); $end null;

                DELETE VW_BatchComponentFragment BC
                    WHERE BatchComponentID IN (SELECT BC_Source.ID
                                    FROM  VW_BatchComponent BC_Source, VW_Mixture_Component MC_Source
                                    WHERE BC_Source.BatchID=ABatchID AND BC_Source.MixtureComponentID=MC_Source.MixtureComponentID AND MC_Source.MixtureID=LSourceMixtureID);
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'DELETE VW_BatchComponentFragmen: ' || SQL%ROWCOUNT); $end null;

                DELETE VW_BatchComponent BC
                    WHERE ID IN (SELECT BC_Source.ID
                                    FROM  VW_BatchComponent BC_Source, VW_Mixture_Component MC_Source
                                    WHERE BC_Source.BatchID=ABatchID AND BC_Source.MixtureComponentID=MC_Source.MixtureComponentID AND MC_Source.MixtureID=LSourceMixtureID) ;
                $if CompoundRegistry.Debuging $then InsertLog ('MoveBatch', 'DELETE VW_BatchComponent: ' || SQL%ROWCOUNT); $end null;

      END IF;

      LRLSState:=RegistrationRLS.GetStateRLS;
      IF LRLSState THEN
        RegistrationRLS.SetEnableRLS(False);
      END IF;

      SELECT NVL(MAX(BatchNumber),0)+1 INTO LBatchNumber
      FROM VW_Batch WHERE RegID=LTargetRegID;

      IF LRLSState THEN
        RegistrationRLS.SetEnableRLS(LRLSState);
      END IF;

      LFullRegNumber := ARegNumber;

      UPDATE VW_Batch
      SET RegID = LTargetRegID,BatchNumber=LBatchNumber
      WHERE BatchID=ABatchID;

      -- generate the list of batch IDs to have their FullRegNum values re-generated
      LNewBatchList(LBatchNumber) := ABatchID;

      -- run the FullBatchNum generators
      UpdateBatchRegNumbers(LNewBatchList);

      IF CHEMINVDB2_EXISTS = 1 THEN
        IF CHEMINV_INTEGRATED = 1 THEN
            EXECUTE IMMEDIATE
                  'BEGIN  :InvCompounds_Updated := CHEMINVDB2.COMPOUNDS.UPDATECOMPOUNDREGISTRYBATCH(:LBatchRegID,:LBatchNumber_Present,:LTargetRegID,:LBatchNumber); END;'
            USING OUT
              InvCompounds_Updated ,IN LBatchRegID, IN LBatchNumber_Present, IN LTargetRegID, IN LBatchNumber;
          END IF;
      END IF;

      IF LErrorMessage IS NOT NULL THEN
        LErrorMessage := 'Components do not match between registries. '||LErrorMessage||' The batch was moved.';
        COMMIT;
        TraceWrite( act||'MoveBatch_ended', $$plsql_line,'end' );
        Raise_Application_Error (LErrorCode,LErrorMessage);
      ELSE
        RETURN;
      END IF;

    END IF;

    IF LCompoundCountRegistry > LCompoundCountBatch THEN
      LErrorMessage := 'The target record has more components than the source. New batch component information should be manually adjusted on the target record.';
      LErrorCode := eMoreComponentsTarget;
    ELSIF LCompoundCountRegistry < LCompoundCountBatch THEN
      LErrorMessage := 'The source record has more components than the target.';
      LErrorCode := eMoreComponentsSource;
    END IF;

    SELECT Min(BatchID) INTO LTargetBatchID
    FROM VW_Batch WHERE RegID = LTargetRegID AND BatchID<>ABatchID;

    SELECT Count(1) INTO LTargetComponentCount
    FROM VW_BatchComponent WHERE BatchID=LTargetBatchID;

    SELECT RN.RegID, MRN.MixtureID
    INTO LTargetRegID, LTargeMixtureID
    FROM VW_RegistryNumber RN,VW_Mixture_RegNumber MRN
    WHERE MRN.RegID=RN.RegID AND RN.RegNumber = ARegNumber;

    DELETE VW_BatchComponentFragment BC
    WHERE BatchComponentID IN (
      SELECT ID FROM VW_BatchComponent WHERE BatchID = ABatchID
    );

    DELETE VW_BatchComponent BC1 WHERE BatchID = ABatchID;

    INSERT INTO VW_BatchComponent(
      ID, BATCHID, MIXTURECOMPONENTID, ORDERINDEX
    )(
      SELECT SEQ_BatchComponent.NEXTVAL,ABatchID,MixtureComponentID,OrderIndex
      FROM VW_BatchComponent
      WHERE BatchID=LTargetBatchID
    );

    LRLSState := RegistrationRLS.GetStateRLS;
    IF LRLSState THEN
      RegistrationRLS.SetEnableRLS(False);
    END IF;

    SELECT NVL(MAX(BatchNumber),0)+1 INTO LBatchNumber FROM VW_Batch WHERE RegID=LTargetRegID;

    IF LRLSState THEN
      RegistrationRLS.SetEnableRLS(LRLSState);
    END IF;

    LFullRegNumber := ARegNumber;

    UPDATE VW_Batch
    SET RegID = LTargetRegID, BatchNumber = LBatchNumber
    WHERE BatchID=ABatchID;

    -- generate the list of batch IDs to have their FullRegNum values re-generated
    LNewBatchList(LBatchNumber) := ABatchID;

    -- run the FullBatchNum generators
    UpdateBatchRegNumbers(LNewBatchList);

    -- update inventory as necessary
    If CHEMINVDB2_EXISTS = 1 THEN
      IF CHEMINV_INTEGRATED = 1 THEN
          EXECUTE IMMEDIATE
            'BEGIN  :InvCompounds_Updated := CHEMINVDB2.COMPOUNDS.UPDATECOMPOUNDREGISTRYBATCH(:LBatchRegID,:LBatchNumber_Present,:LTargetRegID,:LBatchNumber); END;'
          USING OUT
            InvCompounds_Updated ,IN LBatchRegID, IN LBatchNumber_Present, IN LTargetRegID, IN LBatchNumber;
      END IF;
    END IF;

    IF LErrorMessage IS NOT NULL THEN
      LErrorMessage:='Components do not match between registries. '||LErrorMessage||' The batch was moved.';
      COMMIT;
      TraceWrite( act||'MoveBatch_ended', $$plsql_line,'end' );
      Raise_Application_Error (LErrorCode, LErrorMessage);
    END IF;
    TraceWrite( act||'MoveBatch_ended', $$plsql_line,'end' );

  END;

  PROCEDURE DeleteBatch(ABatchID in Number) IS
    LBatchCount Integer;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'DeleteBatch_started', $$plsql_line,'start' );
    SELECT COUNT(1)
        INTO  LBatchCount
        FROM  VW_Batch
        WHERE REGID = (SELECT RegID FROM  VW_Batch WHERE BatchID=ABatchID);

    IF LBatchCount=0 THEN
        BEGIN
            Raise_Application_Error (eNoRowsReturned, AppendError('No rows returned. The batch was not moved.'));
        END;
    ELSIF LBatchCount=1 THEN
        BEGIN
            Raise_Application_Error (eOnlyOneBatchDeleting,
              AppendError('The Batch''s Registry has only one Batch. The Batch was not deleted.'));
        END;
    ELSE
        BEGIN
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'ABatchID: ' || ABatchID); $end null;
            DELETE VW_BATCH_PROJECT WHERE BATCHID=ABatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_BATCH_PROJECT SQL%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
            DELETE VW_BATCHCOMPONENTFRAGMENT WHERE BATCHCOMPONENTID IN (SELECT ID FROM VW_BATCHCOMPONENT WHERE BATCHID=ABatchID);
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_BATCH_BATCHCOMPONENTFRAGMENT%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
            DELETE VW_COMPOUND_FRAGMENT WHERE ID NOT IN (SELECT CompoundFragmentID FROM VW_BATCHCOMPONENTFRAGMENT);
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_COMPONENTFRAGMENT%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
            DELETE VW_BATCHCOMPONENT WHERE BATCHID=ABatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_BATCHCOMPONENT SQL%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
            DELETE VW_BATCHIDENTIFIER WHERE BATCHID=ABatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_BATCHIDENTIFIER SQL%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
            DELETE VW_BATCH WHERE BATCHID=ABatchID;
            $if CompoundRegistry.Debuging $then InsertLog ('DeleteBatch', 'VW_BATCH SQL%ROWCOUNT: ' || cast (SQL%ROWCOUNT  as string)); $end null;
        END;
    END IF;
    TraceWrite( act||'DeleteBatch_ended', $$plsql_line,'end' );
  EXCEPTION
    WHEN OTHERS THEN
        TraceWrite( act||'DeleteBatch_ended', $$plsql_line,'end' );
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('DeleteBatch', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
  END;

  FUNCTION ExtractRegNumber (AXmlError IN CLOB, ATagError IN VARCHAR2,AFilter IN VARCHAR2:=NULL) RETURN VARCHAR2 IS
    LTagBeginError   VARCHAR2 (255);
    LTagEndError     VARCHAR2 (255);
    LStartIndex      NUMBER;
    LEndIndex        NUMBER;
    LXmlDuplicates   CLOB;
    LPath            VARCHAR2 (100) := '';
    LRegnumber       VARCHAR2 (100) := '';
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'ExtractRegNumber_started', $$plsql_line,'start' );
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
      TraceWrite( act||'ExtractRegNumber_ended', $$plsql_line,'end' );

    RETURN LRegNumber;
  END;

  /**
  As part of the 'Add as new batch' duplicate-resolution strategy, performs a
  cut/paste operation on a COEBatch during the conversion of a new or temporary
  registration into an existing permanent one.
  -->author Fari
  -->since 17 March 2010
  -->param AXml       this has the new batch and return the new Registry with the new batch
  -->param AXmlError  this has Duplicated Reg List
  -->param ATagError  this has the tag for identify the Duplicated Reg
  -->param AFilter    this has a filter for identify the Duplicated Reg
  */
  PROCEDURE AddBatch (AXml IN OUT NOCOPY CLOB, AXmlError IN CLOB, ATagError IN VARCHAR2,AFilter IN VARCHAR2:=NULL) IS
    LRegNumber          VARCHAR2 (50) := '';
    LXml                          XMLTYPE;
    LXmlNewBatch                  XMLTYPE;
    LXmlNewFragment               XMLTYPE;
  --LXmlNewBatchComponentFragment XMLTYPE;
    LXmlRows                      CLOB;
   -- LXmlTypeRows                  XMLTYPE;
    LCompoundID         VW_Compound.CompoundID%TYPE;
    LTempBatchID        VW_Batch.BatchID%TYPE;
    LPersonRegistered   VW_RegistryNumber.PersonRegistered%TYPE;
    LMixtureComponentID VW_Mixture_Component.MixtureComponentID%TYPE;
    LIndex              Integer:=0;


    --Get Fragments isn't in target.
    CURSOR C_NewFragments(ARegNumber in VW_RegistryNumber.RegNumber%type,AXmlNewBatch XMLTYPE) IS
        SELECT FragmentID
            FROM (
                    SELECT  extract(value(FragmentIDs), '//text()').getNumberVal() FragmentID
                        FROM Table(XMLSequence(extract(AXmlNewBatch, '/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment/FragmentID'))) FragmentIDs
                    MINUS
                    SELECT CF.FragmentID FragmentID
                        FROM VW_Compound_Fragment CF, VW_RegistryNumber RN, VW_Mixture_RegNumber MR,VW_Mixture_Component MC
                       WHERE MC.CompoundID=CF.CompoundID AND RN.RegID = MR.RegID  AND MR.MixtureID=MC.MixtureID AND RN.RegNumber = ARegNumber
                );

    --Get Mixture's Componentes Fragments
    CURSOR C_Components(ARegNumber in VW_RegistryNumber.RegNumber%type) IS
        SELECT MC.CompoundID,MC.MixtureComponentID,RN.PersonRegistered
            INTO LCompoundID,LMixtureComponentID,LPersonRegistered
            FROM VW_RegistryNumber RN, VW_Mixture_RegNumber MR,VW_Mixture_Component MC
            WHERE RN.RegID = MR.RegID  AND MR.MixtureID=MC.MixtureID AND RN.RegNumber = ARegNumber;
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'AddBatch_started', $$plsql_line,'start' );

    --Get the registry record the compound was originally found in: AXmlError is a listing of such records
    LRegNumber := ExtractRegNumber (AXmlError, ATagError,AFilter);
    $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'LRegNumber:' || LRegNumber); $end null;

    --Get target Registry
    RetrieveMultiCompoundRegistry (LRegNumber, LXmlRows);
     $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'LXmlRows:' || LXmlRows); $end null;

    LXml:=XmlType.CreateXml(AXml);
    --LXmlTypeRows:=XmlType.CreateXml(LXmlRows);


    --Get the existing batch information as an xml document for easier manipulation
    SELECT  extract(LXml, '/MultiCompoundRegistryRecord/BatchList/Batch'),
            extractValue(LXml, '/MultiCompoundRegistryRecord/BatchList/Batch/BatchID')
        INTO
            LXmlNewBatch,
            LTempBatchID FROM DUAL;
    $if CompoundRegistry.Debuging $then InsertLog ('AddBatch','LXmlNewBatch 1:' || LXmlNewBatch.getClobVal()); $end null;

    --Replace the existing BatchComponentList data as necessary:
    SELECT updateXml(
            LXmlNewBatch,
            '/Batch/BatchID/text()', '0',
            '/Batch/BatchComponentList/BatchComponent/BatchID/text()', '0')
        INTO LXmlNewBatch FROM DUAL;

    FOR R_Components IN C_Components(LRegNumber) LOOP
        LIndex:=LIndex+1;
        SELECT updateXml(
                LXmlNewBatch,
                '/Batch/BatchComponentList/BatchComponent['||LIndex||']/CompoundID/text()', R_Components.CompoundID,
                '/Batch/BatchComponentList/BatchComponent['||LIndex||']/ComponentIndex/text()', -R_Components.CompoundID )
            INTO LXmlNewBatch FROM DUAL;
        SELECT insertChildXml(LXmlNewBatch, '/Batch/BatchComponentList/BatchComponent['||LIndex||']', 'MixtureComponentID', XMLType('<MixtureComponentID>'||R_Components.MixtureComponentID||'</MixtureComponentID>'))
            INTO LXmlNewBatch FROM dual;

        --BatchComponentFragment
       /* IF VSameBatchesIdentity='True' THEN
            SELECT  XmlType('<BatchComponentFragmentList>'||extract(LXmlTypeRows, '/MultiCompoundRegistryRecord/BatchList/Batch[1]/BatchComponentList/BatchComponent[CompoundID='||R_Components.CompoundID||']/BatchComponentFragmentList/BatchComponentFragment').getStringVal()||'</BatchComponentFragmentList>')
                INTO LXmlNewBatchComponentFragment FROM DUAL;
             $if CompoundRegistry.Debuging $then InsertLog ('AddBatch','LXmlNewBatch CON LXmlNewBatchComponentFragment:' || LXmlNewBatchComponentFragment.getClobVal()); $end null;
            SELECT updateXml(
                LXmlNewBatchComponentFragment,
                'BatchComponentFragmentList/BatchComponentFragment/ID/text()',0)
                INTO LXmlNewBatchComponentFragment FROM DUAL;
             SELECT insertChildXml(LXmlNewBatchComponentFragment, 'BatchComponentFragmentList/BatchComponentFragment', '@insert', 'yes')
                INTO LXmlNewBatchComponentFragment FROM dual;
             $if CompoundRegistry.Debuging $then InsertLog ('AddBatch','LXmlNewBatchComponentFragment:' || LXmlNewBatchComponentFragment.getClobVal()); $end null;
            SELECT insertChildXml(LXmlNewBatch, '/Batch/BatchComponentList/BatchComponent['||LIndex||']', 'BatchComponentFragmentList', LXmlNewBatchComponentFragment)
                INTO LXmlNewBatch FROM dual;

         END IF;*/
    END LOOP;

   $if CompoundRegistry.Debuging $then InsertLog ('AddBatch','LXmlNewBatch 2:' || LXmlNewBatch.getClobVal()); $end null;

    --Mark this batch as an insert
    SELECT insertChildXml(LXmlNewBatch, '/Batch', '@insert', 'yes')
        INTO LXmlNewBatch FROM dual;
    SELECT insertChildXml(LXmlNewBatch, '/Batch/BatchComponentList/BatchComponent', '@insert', 'yes')
        INTO LXmlNewBatch FROM dual;
    SELECT insertChildXml(LXmlNewBatch, '/Batch/ProjectList/Project', '@insert', 'yes')
        INTO LXmlNewBatch FROM dual;
    IF  LXmlNewBatch.existsNode('/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment[@insert="yes"]')=0 THEN
        SELECT insertChildXml(LXmlNewBatch, '/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment', '@insert', 'yes')
            INTO LXmlNewBatch FROM dual;
    END IF;
    $if CompoundRegistry.Debuging $then InsertLog ('AddBatch','LXmlNewBatch 3:' || LXmlNewBatch.getClobVal()); $end null;

    --This batch need more data
    IF LXmlNewBatch.ExistsNode('/Batch/FullRegNumber')=0 THEN
        SELECT insertChildXml(LXmlNewBatch, '/Batch', 'FullRegNumber', XMLType('<FullRegNumber></FullRegNumber>'))
            INTO LXmlNewBatch FROM dual;
    END IF;
    SELECT insertChildXml(LXmlNewBatch, '/Batch', 'TempBatchID', XMLType('<TempBatchID>'||LTempBatchID||'</TempBatchID>'))
        INTO LXmlNewBatch FROM dual;
    SELECT insertChildXml(LXmlNewBatch, '/Batch', 'PersonRegistered', XMLType('<PersonRegistered>'||LPersonRegistered||'</PersonRegistered>'))
        INTO LXmlNewBatch FROM dual;

    $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'LXmlNewBatch5:' || LXmlNewBatch.getClobVal()); $end null;

    IF not VSameBatchesIdentity='True' THEN
        --Add Frament isn't in target
        FOR R_NewFragments IN C_NewFragments(LRegNumber,LXmlNewBatch) LOOP
            SELECT extract(LXml, '/MultiCompoundRegistryRecord/ComponentList/Component/Compound/FragmentList/Fragment[FragmentID='||R_NewFragments.FragmentID||']')
                INTO LXmlNewFragment FROM DUAL;
             --Mark this Fragment as an insert
            SELECT insertChildXml(LXmlNewFragment, '/Fragment', '@insert', 'yes')
                INTO LXmlNewFragment FROM dual;
             $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'LXmlNewFragment:' || LXmlNewFragment.getClobVal()); $end null;
            SELECT insertChildXml(XmlType(LXmlRows), '/MultiCompoundRegistryRecord/ComponentList/Component/Compound/FragmentList','Fragment' , LXmlNewFragment).GetClobVal()
                INTO LXmlRows FROM dual;
             $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'LXmlRows with Fragment:' || LXmlRows); $end null;
         END LOOP;
    END IF;


    --Append this cloned batch to the pre-existing perm. reg. record
    SELECT insertChildXml(XmlType(LXmlRows), '/MultiCompoundRegistryRecord/BatchList','Batch' , LXmlNewBatch).GetClobVal()
        INTO AXml FROM dual;
    $if CompoundRegistry.Debuging $then InsertLog ('AddBatch', 'AXml:' || AXml); $end null;
    TraceWrite( act||'AddBatch_ended', $$plsql_line,'end' );

  EXCEPTION
    WHEN OTHERS THEN
        TraceWrite( act||'AddBatch_ended', $$plsql_line,'end' );
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('AddBatch', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
  END;

  PROCEDURE UseCompound (AXml IN OUT NOCOPY CLOB, AXmlError IN CLOB, ATagError IN VARCHAR2) IS
    LRegnumber         VARCHAR2 (100) := '';
    LXmlCompoundList   CLOB;
    LXmlCompound       CLOB;
    LPosTagEnd         NUMBER;
    LRegistryList      CLOB;
    LXmlType           XmlType;
    CURSOR C_RegNumbersCompound(AXmlError IN CLOB) IS
        SELECT RN.RegNumber ,extractvalue((value(CompoundIDs)),'COMPOUND/TEMPCOMPOUNDID') TempCompoundID
            FROM VW_RegistryNumber RN,VW_Compound C ,Table(XMLSequence(extract(XmlType(AXmlError),'COMPOUNDLIST/COMPOUND/REGISTRYLIST/REGNUMBER[@count="1"]/../..'))) CompoundIDs
            WHERE C.RegID=RN.RegID AND C.CompoundID =  extractvalue((value(CompoundIDs)),'COMPOUND/REGISTRYLIST/REGNUMBER/@CompoundID');
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'UseCompound_started', $$plsql_line,'start' );
    $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'AXmlError: ' || AXmlError || ' ATagError: ' || ATagError); $end null;
    $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'AXml: ' || AXml ); $end null;
    LXmlType:=XmlType(AXml);
    FOR R_RegNumbersCompound IN C_RegNumbersCompound(AXmlError) LOOP

        LRegistryList:='<REGISTRYLIST><REGNUMBER>' || R_RegNumbersCompound.RegNumber || '</REGNUMBER></REGISTRYLIST>';
        $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'LRegistryList: ' || LRegistryList ); $end null;
        RetrieveCompoundRegistryList (LRegistryList, LXmlCompoundList);
        $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'Compound List a: ' || LXmlCompoundList); $end null;
        LXmlCompound := '<Compound>'||TakeOffAndGetClob (LXmlCompoundList, 'Compound')||'</Compound>';
        $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'Compound List b: ' || LXmlCompoundList); $end null;
        $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'Compound: ' || LXmlCompound); $end null;

        SELECT UpdateXML(LXmlType,'/MultiCompoundRegistryRecord/ComponentList/Component/Compound[CompoundID='||R_RegNumbersCompound.TempCompoundID||']',LXmlCompound)
            INTO LXmlType
            FROM DUAL;
        $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'R_RegNumbersCompound.TempCompoundID: ' || R_RegNumbersCompound.TempCompoundID||' LXmlType.GetClobValue()'||LXmlType.GetClobVal()); $end null;

    END LOOP;

    AXml:=LXmlType.GetClobVal();

    $if CompoundRegistry.Debuging $then InsertLog ('UseCompound', 'Xml con Compound: ' || AXml); $end null;
    TraceWrite( act||'UseCompound_ended', $$plsql_line,'end' );
  EXCEPTION
    WHEN OTHERS THEN
        TraceWrite( act||'UseCompound_ended', $$plsql_line,'end' );
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('UseCompound', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
  END;

  PROCEDURE LoadMultiCompoundRegRecord(
    ARegistryXml IN CLOB
    , ADuplicateAction IN CHAR
    , AAction OUT NOCOPY CHAR
    , AMessage OUT NOCOPY CLOB
    , ARegNumber OUT NOCOPY VW_RegistryNumber.RegNumber%TYPE
    , ARegistration IN CHAR := 'Y'
    , ARegNumGeneration IN CHAR := 'Y'
    , AConfigurationID IN Number := 1
    , ASectionsList IN Varchar2:=NULL
    , ASetBatchNumber IN Number := 1
    ) IS
    LXmlRows       CLOB           := ARegistryXml;
    LTempId        NUMBER;
    LError         CLOB;
    LMessage       CLOB;
    LRegNumber     CLOB;
    LDiscard       CLOB;
    LXMLRegistryRecord   CLOB;
    LRegNumberToAddBatch CLOB;
    LNewRegId   number:=0;
    LBatchNumber number:=1;
    mod1 varchar2(100); act varchar2(100);

    FUNCTION ExistOneMixtureSameStructures(AXmlRows IN CLOB, AError IN CLOB, ARegNumberToAddBatch OUT VARCHAR2,AMessage OUT CLOB) RETURN BOOLEAN IS
        LCountCompound NUMBER;
        LCountCompoundDuplicated NUMBER;  --in same mixture
        mod1 varchar2(100); act varchar2(100);
      BEGIN
        dbms_application_info.read_module(mod1, act); TraceWrite( act||'ExistOneMixtureSameStructures_started', $$plsql_line,'start' );
        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneMixtureSameStructures', 'LXmlRows: ' || LXmlRows); $end null;
        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneMixtureSameStructures', 'AError: ' || AError); $end null;

        SELECT  COUNT(1)
            INTO LCountCompound
            FROM Table(XMLSequence(extract(XmlType(AXmlRows), 'MultiCompoundRegistryRecord/ComponentList/Component'))) Components;

        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneMixtureSameStructures', 'LCountCompound: ' || LCountCompound); $end null;

        BEGIN
            IF VSameBatchesIdentity='False' THEN
                SELECT  count(1) CountCompoundDuplicated,  extract(value(RegNumbers), '//text()').GetStringVal() RegNumbers
                    INTO LCountCompoundDuplicated,ARegNumberToAddBatch
                    FROM Table(XMLSequence(extract(XmlType(AError), 'COMPOUNDLIST/COMPOUND/REGISTRYLIST/REGNUMBER'))) RegNumbers
                    WHERE LCountCompound=(SELECT Count(1)
                                            FROM VW_RegistryNumber RN, VW_Mixture_RegNumber MR,VW_Mixture_Component MC
                                            WHERE  RN.RegID = MR.RegID  AND MR.MixtureID=MC.MixtureID AND RN.RegNumber = extract(value(RegNumbers), '//text()').GetStringVal() )
                    GROUP BY extract(value(RegNumbers), '//text()').GetStringVal();
            ELSE
                 SELECT  count(1) CountCompoundDuplicated,  extract(value(RegNumbers), '//text()').GetStringVal() RegNumbers
                    INTO LCountCompoundDuplicated,ARegNumberToAddBatch
                    FROM Table(XMLSequence(extract(XmlType(AError), 'COMPOUNDLIST/COMPOUND/REGISTRYLIST/REGNUMBER[@SAMEEQUIVALENT="True"]'))) RegNumbers
                    WHERE LCountCompound=(SELECT Count(1)
                                            FROM VW_RegistryNumber RN, VW_Mixture_RegNumber MR,VW_Mixture_Component MC
                                            WHERE  RN.RegID = MR.RegID  AND MR.MixtureID=MC.MixtureID AND RN.RegNumber = extract(value(RegNumbers), '//text()').GetStringVal() )
                    GROUP BY extract(value(RegNumbers), '//text()').GetStringVal();
            END IF;
        EXCEPTION
            WHEN TOO_MANY_ROWS THEN --then exist more than 1 mixture with some strcutres
                AMessage:='Conflicts with duplicated.';
                ARegNumberToAddBatch:=NULL;
            WHEN NO_DATA_FOUND THEN
                ARegNumberToAddBatch:=NULL;
        END;
        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneMixtureSameStructures', 'CountCompoundDuplicated: ' || LCountCompoundDuplicated); $end null;
        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneMixtureSameStructures', 'ARegNumberToAddBatch: ' || ARegNumberToAddBatch); $end null;
        TraceWrite( act||'ExistOneMixtureSameStructures_ended', $$plsql_line,'end' );

        IF ARegNumberToAddBatch IS NOT NULL THEN
            RETURN TRUE;
        ELSE
            RETURN FALSE;
        END IF;
    END;

    FUNCTION ExistOneStructureByDuplicated(AXmlRows IN CLOB, AError IN CLOB, AMessage OUT CLOB) RETURN BOOLEAN IS
        LExistOneStructureByDuplicated NUMBER;
        mod1 varchar2(100); act varchar2(100);
    BEGIN
        dbms_application_info.read_module(mod1, act); TraceWrite( act||'ExistOneStructureByDuplicated_started', $$plsql_line,'start' );
        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneStructureByDuplicated', 'LXmlRows: ' || LXmlRows); $end null;
        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneStructureByDuplicated', 'AError: ' || AError); $end null;

        IF VSameBatchesIdentity='False' THEN
            SELECT   Count(1)  --extractvalue((value(RegNumbers)),'COMPOUND/TEMPCOMPOUNDID') TempCompoundID,
                               --       (SELECT  Count(1)
                               --          FROM Table(XMLSequence(extract(value(RegNumbers), 'COMPOUND/REGISTRYLIST/REGNUMBER'))) RegNumbers) CountDuplicated
                INTO LExistOneStructureByDuplicated
                FROM Table(XMLSequence(extract(XmlType(AError), 'COMPOUNDLIST/COMPOUND'))) RegNumbers
                WHERE (SELECT  Count(1)
                          FROM Table(XMLSequence(extract(value(RegNumbers), 'COMPOUND/REGISTRYLIST/REGNUMBER'))) RegNumbers)>1;
        ELSE
            SELECT   Count(1)
                INTO LExistOneStructureByDuplicated
                FROM Table(XMLSequence(extract(XmlType(AError), 'COMPOUNDLIST/COMPOUND'))) RegNumbers
                WHERE (SELECT  Count(1)
                          FROM Table(XMLSequence(extract(value(RegNumbers), 'COMPOUND/REGISTRYLIST/REGNUMBER'))) RegNumbers)>1
                       OR ExtractValue(value(RegNumbers), 'COMPOUND/REGISTRYLIST/REGNUMBER[1]/@SAMEEQUIVALENT')='False';
        END IF;
        $if CompoundRegistry.Debuging $then InsertLog ('ExistOneStructureByDuplicated', 'LExistOneStructureByDuplicated=' || LExistOneStructureByDuplicated); $end null;
        TraceWrite( act||'ExistOneStructureByDuplicated_ended', $$plsql_line,'end' );

        IF LExistOneStructureByDuplicated=0 THEN
            RETURN TRUE;
        ELSE
            AMessage:='Conflicts with duplicated.';
            RETURN FALSE;
        END IF;
    END;

  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'LoadMultiCompoundRegRecord_started', $$plsql_line,'start' );
    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ARegistryXml: ' || ARegistryXml); $end null;
    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ADuplicateAction: ' || ADuplicateAction); $end null;
    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ARegistration: ' || ARegistration); $end null;
    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'AConfigurationID: ' || AConfigurationID); $end null;
    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ASectionsList: ' || ASectionsList); $end null;

    AAction := 'E';
    IF ARegistration = 'Y' THEN
        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ARegistration: ' || ARegistration); $end null;

        CreateMultiCompoundRegistry (LXmlRows, ARegNumber, AMessage, 'C', ARegNumGeneration, AConfigurationID, ASectionsList, ASetBatchNumber);

        --split the Response into its parts
        ExtractRegistrationResponse(AMessage, LMessage, LError, LDiscard);

        IF LError IS NULL THEN
            --the outgoing message (AMessage) parameter is already populated
            AAction := 'C';
            $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'RegNumber: ' || ARegNumber); $end null;
        ELSE
            --reset the outgoing message
            AMessage := null;
            AAction := ADuplicateAction;
            IF ADuplicateAction = 'D' THEN
                CreateMultiCompoundRegistry (LXmlRows, ARegNumber, AMessage, 'N', ARegNumGeneration, 1, ASectionsList, ASetBatchNumber);
            ELSIF ADuplicateAction = 'B' THEN
                LRegNumber := LError; -- Copy so we won't destroy LError
                LRegNumber := TakeOffAndGetClobsList(LRegNumber, 'REGNUMBER');  -- Get the RegNumbers
                LRegNumber := TakeOffAndGetClob(LRegNumber, 'Clob');  -- Get only the first one
                LDiscard := TakeOffAndGetClobsList(LRegNumber, 'SAMEFRAGMENT'); -- Discard
                LDiscard := TakeOffAndGetClobsList(LRegNumber, 'SAMEEQUIVALENT'); -- Discard
                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ADuplicateAction = ''B'''); $end null;
                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'LError: ' || LError); $end null;
                IF INSTR (LError, '<COMPOUNDLIST>') > 0 AND INSTR (LError, '</COMPOUNDLIST>') > 0 THEN
                    IF ExistOneMixtureSameStructures(LXmlRows,LError,LRegNumberToAddBatch,LMessage) THEN
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'Adding Batch...LXmlRows: ' || LXmlRows); $end null;
                        AddBatch (LXmlRows, LError, 'COMPOUNDLIST','. = "'||LRegNumberToAddBatch||'"');
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'Updating... LXmlRows:'|| LXmlRows); $end null;
                        UpdateMultiCompoundRegistry (LXmlRows, AMessage, 'D', 1, ASectionsList);
                        ARegNumber := LRegNumber;
                        -- Preserve the original message and conditionally return the new object
                        IF ARegNumber IS NOT NULL THEN
                            BEGIN
                                IF ASectionsList IS NULL OR UPPER(ASectionsList) <> cSectionListEmpty THEN
                                    RetrieveMultiCompoundRegistry(ARegNumber, LXMLRegistryRecord, ASectionsList);
                                END IF;
                                select rn.regid, max(b.batchnumber) into LNewRegId, LBatchNumber from vw_batch b, vw_registrynumber rn
                                  where b.regid=rn.regid and rn.regnumber=ARegNumber group by rn.regid;
                                AMessage := CreateRegistrationResponse(LMessage, NULL, LXMLRegistryRecord, LNewRegId, LBatchNumber);
                            end;
                        END IF;
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'After Update' || LRegNumber); $end null;
                    ELSIF ExistOneStructureByDuplicated(LXmlRows,LError,LMessage) THEN
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'Using Compund...LXmlRows: ' || LXmlRows); $end null;
                        UseCompound (LXmlRows, LError, 'COMPOUNDLIST');
                        CreateMultiCompoundRegistry (LXmlRows, ARegNumber, AMessage, 'D', ARegNumGeneration, ASetBatchNumber);
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'AMessage:' || AMessage); $end null;
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'ARegNumber:' || ARegNumber); $end null;
                        -- Preserve the original message and conditionally return the new object
                        IF ARegNumber IS NOT NULL THEN
                            BEGIN
                                IF ASectionsList IS NULL OR UPPER(ASectionsList) <> cSectionListEmpty THEN
                                    RetrieveMultiCompoundRegistry(ARegNumber, LXMLRegistryRecord, ASectionsList);
                                END IF;
                                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'LMessage:' || LMessage); $end null;
                                AMessage := CreateRegistrationResponse(LMessage, NULL, LXMLRegistryRecord);
                                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'AMessage:' || AMessage); $end null;
                            end;
                        END IF;
                        AAction := 'U';
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'UseCompound (COMPOUNDLIST): ' || LRegNumber); $end null;
                    ELSE
                        ARegNumber := '';
                        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'LError:' || LError); $end null;
                        AAction := 'N';
                        AMessage := CreateRegistrationResponse(LMessage, LError, NULL);
                    END IF;
                ELSIF INSTR (LError, '<REGISTRYLIST>') > 0 AND INSTR (LError, '</REGISTRYLIST>') > 0 THEN
                    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'Before-AddBatch LXmlRows): ' || LXmlRows); $end null;
                    AddBatch (LXmlRows, LError, 'REGISTRYLIST');
                    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'After-AddBatch LXmlRows): ' || LXmlRows); $end null;
                    UpdateMultiCompoundRegistry (LXmlRows, AMessage, 'D', 1, ASectionsList);
                    ARegNumber := LRegNumber;
                    -- Preserve the original message and conditionally return the new object
                    IF ARegNumber IS NOT NULL THEN
                        BEGIN
                            IF ASectionsList IS NULL OR UPPER(ASectionsList) <> cSectionListEmpty THEN
                                RetrieveMultiCompoundRegistry(ARegNumber, LXMLRegistryRecord, ASectionsList);
                            END IF;
                            AMessage := CreateRegistrationResponse(LMessage, NULL, LXMLRegistryRecord);
                        END;
                    END IF;
                    $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'AddBatch (REGISTRYLIST): ' || LRegNumber); $end null;
                ELSE
                    Raise_Application_Error (EGenericException,
                      AppendError('Duplciation errors exist but were incorrectly formatted.'));
                END IF;
            ELSIF ADuplicateAction = 'T' THEN
                CreateTemporaryRegistration (LXmlRows, LTempID, AMessage, ASectionsList);
                ARegNumber := LTempID; --Necessary in DataLoader Application
                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'TempID1: ' || LTempID); $end null;
            ELSIF ADuplicateAction = 'N' THEN
                ARegNumber := '0';
                AMessage := CreateRegistrationResponse('Duplicate found', LError, NULL);
                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'TempID2: ' || LTempID); $end null;
            ELSE
                $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE); $end null;
                Raise_Application_Error (EGenericException,
                  AppendError('Invalid duplicate-resolution instruction: "' || ADuplicateAction || '"'));
            END IF;
       END IF;
    ELSE
        CreateTemporaryRegistration (LXmlRows, LTempID, AMessage);
        ARegNumber := LTempID;
        IF ARegNumber IS NOT NULL THEN
            BEGIN
                IF ASectionsList IS NULL OR UPPER(ASectionsList) <> cSectionListEmpty THEN
                    RetrieveTemporaryRegistration(ARegNumber, LXMLRegistryRecord);
                END IF;
                AMessage := CreateRegistrationResponse(LMessage, NULL, LXMLRegistryRecord);
            END;
        END IF;
        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord', 'TempID3: ' || LTempID); $end null;
    END IF;
    TraceWrite( act||'LoadMultiCompoundRegRecord_ended', $$plsql_line,'end' );
  EXCEPTION
    WHEN OTHERS THEN
        $if CompoundRegistry.Debuging $then InsertLog ('LoadMultiCompoundRegRecord',  DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE); $end null;
           TraceWrite( act||'LoadMultiCompoundRegRecord_ended', $$plsql_line,'end' );
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('LoadMultiCompoundRegRecord', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
  END;

  PROCEDURE LoadMultiCompoundRegRecordList(
    ARegistryListXml IN CLOB
    , ADuplicateAction IN CHAR
    , ARegistration IN CHAR
    , ARegNumGeneration IN CHAR
    , AConfigurationID IN NUMBER
    , LogID IN NUMBER
  ) IS
    -- returns to C# every 100 records, so LogID has to be passed in
    LATempID        VARCHAR2(8) := '0';
    LXmlRows        CLOB;
    LXmlCompReg     CLOB := ARegistryListXml;
    AMessage        CLOB;
    -- temporarily. should get from xml. need confirmation
    ADuplicateCheck CHAR := 'C';
    BatchID         NUMBER := 1;
    LAction         CHAR;
    LRegNum         VARCHAR2(100) := '';
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'LoadMultiCompoundRegRecordList_started', $$plsql_line,'start' );
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
            LoadMultiCompoundRegRecord(LXmlRows, ADuplicateAction, LAction, AMessage, LRegNum, ARegistration, ARegNumGeneration, AConfigurationID);

            -- Register Marked only, not DataLoader
            IF LATempID <> '0' THEN
                DeleteTemporaryRegistration(LATempID);
            END IF;

            -- WJC This area may need to be rethought
            -- We have LRegNum but not RegID
            -- Need to map LAction into IsDuplicate
            -- Don't have BatchID
            LogBULKREGISTRATION(LogID, LATempID,  LAction, LRegNum, BatchID, 'Succeed');

        EXCEPTION
            WHEN OTHERS THEN
                ROLLBACK TO one_record;
                LogBULKREGISTRATION(LogID, LATempID, LAction, LRegNum, BatchID, SUBSTR (DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE, 1, 500));
        END;
        COMMIT;
    END LOOP;
    TraceWrite( act||'LoadMultiCompoundRegRecordList_ended', $$plsql_line,'end' );
  EXCEPTION
    WHEN OTHERS THEN
        TraceWrite( act||'LoadMultiCompoundRegRecordList_ended', $$plsql_line,'end' );
        RAISE_APPLICATION_ERROR(eGenericException, AppendError('LoadMultiCompoundRegRecordList', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
  END;

  PROCEDURE RetrieveTempIDList(Ahitlistid IN Number, Aid OUT NOCOPY CLOB) IS
    Cursor temp_get_hitlistid(m_Id Number) Is
      Select Id
        From COEDB.COESAVEDHITLIST
       Where HITLISTID = m_Id;

    LTempIdList CLOB := '';

    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'RetrieveTempIDList_started', $$plsql_line,'start' );
    For Lrow in temp_get_hitlistid(Ahitlistid)
    Loop
      LTempIdList := LTempIdList || Lrow.Id || ',';
    END LOOP;
    Aid := Rtrim(LTempIdList, ',');
    TraceWrite( act||'RetrieveTempIDList_ended', $$plsql_line,'end' );
  EXCEPTION
    WHEN OTHERS THEN
        TraceWrite( act||'RetrieveTempIDList_ended', $$plsql_line,'end' );
        RAISE_APPLICATION_ERROR(eRetrieveMultiCompoundRegList, AppendError('RetrieveTempIDList', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
  END;

  PROCEDURE LogBulkRegistrationId(
    ALogID OUT NUMBER
    , ADuplicateAction IN Varchar2
    , AUserID IN Varchar2
    , ADescription IN Varchar2 DEFAULT NULL
  ) IS
    LALOG_ID NUMBER;
  BEGIN
    INSERT INTO LOG_BULKREGISTRATION_ID
      (DUPLICATE_ACTION, DESCRIPTION, USER_ID, DATETIME_STAMP)
    VALUES
      (ADuplicateAction, ADescription, AUserID, sysdate)
    returning LOG_ID INTO LALOG_ID;

    ALogID := LALOG_ID;
  EXCEPTION
    WHEN OTHERS THEN
      RAISE_APPLICATION_ERROR(eGenericException, AppendError('LogBulkRegistrationId', DBMS_UTILITY.FORMAT_ERROR_STACK||DBMS_UTILITY.FORMAT_ERROR_BACKTRACE));
  END;

  PROCEDURE LogBulkRegistration(
    ALogID IN NUMBER
    , LATempID IN VARCHAR2
    , AAction IN char
    , RegNumber IN VARCHAR2
    , BatchID IN NUMBER
    , Result IN VARCHAR2
  ) IS
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'LogBulkRegistration_started', $$plsql_line,'start' );
    INSERT INTO LOG_BULKREGISTRATION
      (log_id, temp_id, action, reg_number, batch_number, comments)
    VALUES
      (ALogID, LATempID, AAction, RegNumber, BatchID, Result);
    TraceWrite( act||'LogBulkRegistration_ended', $$plsql_line,'end' );
  END;

  PROCEDURE RetrieveBatchCommon( AID IN NUMBER, AXml OUT NOCOPY CLOB, AIsTemp IN BOOLEAN) IS
      -- vary based on AIsTemp
      mod1 varchar2(100); act varchar2(100);
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
         <PersonApproved>
          <xsl:for-each select="PERSONAPPROVED">
            <xsl:for-each select="$VBATCH/PERSONAPPROVEDDISPLAY">
              <xsl:attribute name="displayName"><xsl:value-of select="."/></xsl:attribute>
            </xsl:for-each>
            <xsl:value-of select="."/>
          </xsl:for-each>
        </PersonApproved>
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

    /*
    Gets a property list from the object definition in COEOBJECTCONFIG
    -- APath is the path between MultiCompoundRegistryRecord and PropertyList eg. 'BatchList/Batch'
    -- ARemoveTrailingComma will remove the trailing comma if TRUE
    -- ATerm controls how each term is expanded
    */
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
      mod1 varchar2(100); act varchar2(100);
    BEGIN
      dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetPropertyList_started', $$plsql_line,'start' );
      -- Prepare XSL
      LXsl := Replace(LXsl, '@PATH', APath); -- eg. BatchList/Batch
      LXsl := Replace(LXsl, '@TERM', ATerm); -- eg. DECODE(<xsl:value-of select="@name"/>, NULL, '' '', <xsl:value-of select="@name"/>) <xsl:value-of select="@name"/>,
      -- Get XML
      LXml := VRegObjectTemplate.GetClobVal();
      -- Transform
      SELECT XmlTransform(XmlType.CreateXml(LXml), XmlType.CreateXml(LXsl)).GetClobVal()
        INTO LReturn
        FROM DUAL;
      -- Remove trailing comma is requested
      IF ARemoveTrailingComma AND Length(LReturn) > 0 THEN
          LReturn := Rtrim(LReturn, ',');
      END IF;
      --
      TraceWrite( act||'GetPropertyList_ended', $$plsql_line,'end' );
      RETURN LReturn;
    END;

    BEGIN
      dbms_application_info.read_module(mod1, act); TraceWrite( act||'RetrieveBatchCommon_started', $$plsql_line,'start' );
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
        'SELECT
          '||LBatchID||' as BATCHID
          , BATCHNUMBER
          , DATECREATED
          , PERSONCREATED
          , PERSONAPPROVED
          , '||LPersonRegistered||' as PERSONREGISTERED
          , DATELASTMODIFIED, '
          || '''BatchPropertyListBegin'' Aux,' || LBatchProperties || '''BatchPropertyListEnd'' Aux ' ||
        'FROM ' || LViewBatch || ' ' ||
        'WHERE '||LBatchID||' =:bindvar2';
      $if CompoundRegistry.Debuging $then InsertLog(LDebugProcedure,' LSelect(' || LViewBatch || ')=' || LSelect); $end null;


      LQryCtx := DBMS_XMLGEN.newContext(LSelect);

      dbms_xmlgen.setBindValue(LQryCtx, 'bindvar2', to_char(AID));

      LXmlBatch := Replace(DBMS_XMLGEN.getXML(LQryCtx), cXmlDecoration, '');
      DBMS_XMLGEN.closeContext(LQryCtx);

      $if CompoundRegistry.Debuging $then InsertLog(LDebugProcedure,' LXmlBatch=' || LXmlBatch); $end null;

      IF LXmlBatch IS NULL THEN
          RAISE_APPLICATION_ERROR(eGenericException, AppendError('No rows returned.'));
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
        'SELECT
          '||LID||' as ID
          , '||LBatchID||' as BATCHID
          , '||LMixtureComponentID||' as MixtureComponentID
          , '||LCompoundID||' as COMPOUNDID
          , -1* '||LCompoundID||' as COMPONENTINDEX, '
        || '''PropertyListBegin'' Aux,' || LBatchComponentProperties || '''PropertyListEnd'' Aux '
        || 'FROM ' || LViewCompound || ' '
        ||  'WHERE '||LBatchID||' =:bindvar2';

      LQryCtx := DBMS_XMLGEN.newContext(LSelect);
      dbms_xmlgen.setBindValue(LQryCtx, 'bindvar2', to_char(AID));

      LXmlBatchComponentList := Replace(DBMS_XMLGEN.getXML(LQryCtx), cXmlDecoration, '');
      DBMS_XMLGEN.closeContext(LQryCtx);

      $if CompoundRegistry.Debuging $then InsertLog(LDebugProcedure,' LXmlBatchComponentList=' || LXmlBatchComponentList); $end null;

      IF LXmlBatchComponentList IS NULL THEN
          RAISE_APPLICATION_ERROR(eGenericException, AppendError('No rows returned.'));
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
      TraceWrite( act||'GetPropertyList_ended', $$plsql_line,'end' );

      RETURN;
  END;

  PROCEDURE RetrieveBatch( AID IN NUMBER, AXml OUT NOCOPY CLOB) IS
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'RetrieveBatch_started', $$plsql_line,'start' );
    RetrieveBatchCommon(AID, AXml, FALSE);
    TraceWrite( act||'RetrieveBatch_ended', $$plsql_line,'end' );
  END;

  PROCEDURE RetrieveBatchTmp( ATempID IN NUMBER, AXml OUT NOCOPY CLOB) IS
  mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'RetrieveBatch_started', $$plsql_line,'start' );
    RetrieveBatchCommon(ATempID, AXml, TRUE);
    TraceWrite( act||'RetrieveBatch_ended', $$plsql_line,'end' );
  END;

  /* JED: This is not an optimal design - we can't use bind variables for the values */
  PROCEDURE UpdateBatchCommon( AXml IN CLOB, AIsTemp IN BOOLEAN) IS
    -- vary based on AIsTemp
    v_batchid_column varchar2(30);
    v_batch_table varchar2(30);
    v_batchId number;
    v_batch_xsl CLOB := '
      <xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
        <xsl:template match="/">
        UPDATE {v_batch_table} SET
        <xsl:for-each select="Batch/PropertyList/Property[@update=''yes'']">
          <xsl:value-of select="@name"/> = ''<xsl:value-of select="."/>'',</xsl:for-each>{WHERE}
        {v_batchid_column} = :batchid
        </xsl:template>
      </xsl:stylesheet>
    ';
    v_dyn_sql varchar2(4000);
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'UpdateBatchCommon_started', $$plsql_line,'start' );
    -- Ensure proper date format for auto-conversion from string
    BEGIN
        SetSessionParameter();
    END;

    --Get the underlying ID value
    select extractValue(xmltype(AXml), '/Batch/BatchID') into v_batchId from dual;

    -- Set variables based on temp versus perm
    IF AIsTemp THEN
      v_batchid_column := 'TEMPBATCHID';
      v_batch_table := 'VW_TEMPORARYBATCH';
    ELSE
      v_batchid_column := 'BATCHID';
      v_batch_table := 'VW_BATCH';
    END IF;

    -- Set XSL based on temp versus perm
    v_batch_xsl := Replace(v_batch_xsl, '{v_batch_table}', v_batch_table);
    v_batch_xsl := Replace(v_batch_xsl, '{v_batchid_column}', v_batchid_column);

    -- Transform XML into SQL
    SELECT XmlTransform(
      XmlType.CreateXml(AXml)
      , XmlType.CreateXml(v_batch_xsl)
    ).GetClobVal()
    INTO v_dyn_sql FROM DUAL;

    -- Fix apostrophes
    v_dyn_sql := Replace(v_dyn_sql, '&apos;', '''');
    -- Remove trailing comma in the SET clause
    v_dyn_sql := Replace(v_dyn_sql, ',{WHERE}', ' WHERE');

    EXECUTE IMMEDIATE v_dyn_sql USING v_batchId;
    TraceWrite( act||'UpdateBatchCommon_ended', $$plsql_line,'end' );
  END;

  PROCEDURE UpdateBatchRegNumbers(p_batchId_list IN TNumericIdList) IS
    v_index integer := 0;
    v_batchId vw_batch.batchid%type;
    v_batchRegNum vw_batch.fullregnumber%type;
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'UpdateBatchRegNumbers_started', $$plsql_line,'start' );

    TraceWrite('UpdateBatchRegNumbers_0_Start', $$plsql_line, null);

    IF ( p_batchId_list is null OR p_batchId_list.Count = 0 ) THEN
      RETURN;
    END IF;

    TraceWrite('UpdateBatchRegNumbers_1_Count', $$plsql_line, to_char(p_batchId_list.Count));

    FOR v_index IN p_batchId_list.FIRST..p_batchId_list.LAST LOOP
      v_batchId := p_batchId_list(v_index);
      TraceWrite('UpdateBatchRegNumbers_BatchID', $$plsql_line, to_char(v_batchId));

      BEGIN
        v_batchRegNum := GenerateBatchRegNumber(v_batchId);

        TraceWrite('UpdateBatchRegNumbers_BatchRegNum', $$plsql_line, v_batchRegNum);

        update vw_batch b
        set b.fullregnumber = v_batchRegNum
        where b.batchid = v_batchId;
      END;
    END LOOP;
    TraceWrite( act||'UpdateBatchRegNumbers_ended', $$plsql_line,'end' );

    RETURN;
  END;

  PROCEDURE UpdateBatch( AXml IN CLOB) IS
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'UpdateBatch_started', $$plsql_line,'start' );
    UpdateBatchCommon(AXml, FALSE);
    TraceWrite( act||'UpdateBatch_ended', $$plsql_line,'end' );
    RETURN;
  END;

  PROCEDURE UpdateBatchTmp( AXml IN CLOB) IS
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'UpdateBatchTmp_started', $$plsql_line,'start' );
    UpdateBatchCommon(AXml, TRUE);
    TraceWrite( act||'UpdateBatchTmp_ended', $$plsql_line,'end' );
    RETURN;
  END;

  /** XSL transform used to fetch a 'permanent' registry record
  -->author jed
  -->since January 2011
  -->return an XSLT for fetching permanent records
  */
  FUNCTION XslMcrrFetch RETURN XMLTYPE IS
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'XslMcrrFetch_started', $$plsql_line,'start' );
    if (vXslRetrieveMcrr is null) then
      if (CompoundRegistry.FillPropertyTemplate = false) then
vXslRetrieveMcrr := XmlType.CreateXml(
'<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:variable name="Vlower" select="''abcdefghijklmnopqrstuvwxyz''"/>
  <xsl:variable name="VUPPER" select="''ABCDEFGHIJKLMNOPQRSTUVWXYZ''"/>
  <xsl:template match="/MultiCompoundRegistryRecord">
    <MultiCompoundRegistryRecord>
      <xsl:attribute name="SameBatchesIdentity">
        <xsl:value-of select="@SameBatchesIdentity"/>
      </xsl:attribute>
      <xsl:attribute name="ActiveRLS">
        <xsl:value-of select="@ActiveRLS"/>
      </xsl:attribute>
      <xsl:attribute name="IsEditable">
        <xsl:value-of select="@IsEditable"/>
      </xsl:attribute>
      <xsl:attribute name="IsRegistryDeleteable">
        <xsl:value-of select="@IsRegistryDeleteable"/>
      </xsl:attribute>
      <xsl:variable name="VMultiCompoundRegistryRecord" select="."/>
      <xsl:variable name="VTypeRegistryRecord" select="@TypeRegistryRecord"/>
      <xsl:for-each select="Mixture/ROW">
        <ID>
          <xsl:value-of select="MIXTUREID"/>
        </ID>
        <DateCreated>
          <xsl:value-of select="CREATED"/>
        </DateCreated>
        <DateLastModified>
          <xsl:value-of select="MODIFIED"/>
        </DateLastModified>
        <PersonCreated>
          <xsl:value-of select="PERSONCREATED"/>
        </PersonCreated>
        <PersonApproved>
          <xsl:value-of select="PERSONAPPROVED"/>
        </PersonApproved>
        <StructureAggregation>
          <xsl:value-of select="STRUCTUREAGGREGATION"/>
        </StructureAggregation>
        <StatusID>
          <xsl:value-of select="STATUSID"/>
        </StatusID>
        <PropertyList>
          <xsl:for-each select="..">
            <xsl:apply-templates select="PropertyList"/>
          </xsl:for-each>
        </PropertyList>
        <RegNumber>
          <RegID>
            <xsl:value-of select="REGID"/>
          </RegID>
          <SequenceNumber>
            <xsl:value-of select="SEQUENCENUMBER"/>
          </SequenceNumber>
          <RegNumber>
            <xsl:value-of select="REGNUMBER"/>
          </RegNumber>
          <SequenceID>
            <xsl:value-of select="SEQUENCEID"/>
          </SequenceID>
        </RegNumber>
        <IdentifierList>
          <xsl:for-each select="../IdentifierList/ROW">
            <Identifier>
              <ID>
                <xsl:value-of select="ID"/>
              </ID>
              <IdentifierID>
                <xsl:for-each select="TYPE">
                  <xsl:attribute name="Description">
                    <xsl:value-of select="../DESCRIPTION"/>
                  </xsl:attribute>
                  <xsl:attribute name="Name">
                    <xsl:value-of select="../NAME"/>
                  </xsl:attribute>
                  <xsl:attribute name="Active">
                    <xsl:value-of select="../ACTIVE"/>
                  </xsl:attribute>
                  <xsl:value-of select="."/>
                </xsl:for-each>
              </IdentifierID>
              <InputText>
                <xsl:value-of select="VALUE"/>
              </InputText>
            </Identifier>
          </xsl:for-each>
        </IdentifierList>
        <ProjectList>
          <xsl:for-each select="../ProjectList/ROW">
            <Project>
              <ID>
                <xsl:value-of select="ID"/>
              </ID>
              <ProjectID>
                <xsl:attribute name="Description">
                  <xsl:value-of select="DESCRIPTION"/>
                </xsl:attribute>
                <xsl:attribute name="Name">
                  <xsl:value-of select="NAME"/>
                </xsl:attribute>
                <xsl:attribute name="Active">
                  <xsl:value-of select="ACTIVE"/>
                </xsl:attribute>
                <xsl:value-of select="PROJECTID"/>
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
                <xsl:value-of select="COMPONENTINDEX"/>
              </ComponentIndex>
              <Compound>
                <CompoundID>
                  <xsl:value-of select="COMPOUNDID"/>
                </CompoundID>
                <DateCreated>
                  <xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/>
                </DateCreated>
                <PersonCreated>
                  <xsl:value-of select="PERSONCREATED"/>
                </PersonCreated>
                 <PersonApproved>
                  <xsl:value-of select="PERSONAPPROVED"/>
                </PersonApproved>
                <PersonRegistered>
                  <xsl:value-of select="PERSONREGISTERED"/>
                </PersonRegistered>
                <DateLastModified>
                  <xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/>
                </DateLastModified>
                <Tag>
                  <xsl:value-of select="TAG"/>
                </Tag>
                <PropertyList>
                  <xsl:for-each select="..">
                    <xsl:apply-templates select="PropertyList"/>
                  </xsl:for-each>
                </PropertyList>
                <RegNumber>
                  <RegID>
                    <xsl:if test="string-length(REGID)!=0">
                      <xsl:value-of select="REGID"/>
                    </xsl:if>
                    <xsl:if test="string-length(REGID)=0">
                      <xsl:value-of select="''0''"/>
                    </xsl:if>
                  </RegID>
                  <SequenceNumber>
                    <xsl:value-of select="SEQUENCENUMBER"/>
                  </SequenceNumber>
                  <RegNumber>
                    <xsl:value-of select="REGNUMBER"/>
                  </RegNumber>
                  <SequenceID>
                    <xsl:value-of select="SEQUENCEID"/>
                  </SequenceID>
                </RegNumber>
                <CanPropogateComponentEdits>
                  <xsl:value-of select="CanPropogateComponentEdits"/>
                </CanPropogateComponentEdits>
                <BaseFragment>
                  <xsl:for-each select="../Structure/ROW">
                    <Structure>
                      <StructureID>
                        <xsl:value-of select="STRUCTUREID"/>
                      </StructureID>
                      <StructureFormat>
                        <xsl:value-of select="STRUCTUREFORMAT"/>
                      </StructureFormat>
                      <Structure>
                        <xsl:for-each select="STRUCTURE">
                          <xsl:attribute name="molWeight">
                            <xsl:value-of select="../FORMULAWEIGHT"/>
                          </xsl:attribute>
                          <xsl:attribute name="formula">
                            <xsl:value-of select="../MOLECULARFORMULA"/>
                          </xsl:attribute>
                          <xsl:value-of select="."/>
                        </xsl:for-each>
                      </Structure>
                      <NormalizedStructure>
                        <xsl:value-of select="NORMALIZEDSTRUCTURE"/>
                      </NormalizedStructure>
                      <UseNormalization>
                        <xsl:value-of select="USENORMALIZATION"/>
                      </UseNormalization>
                      <DrawingType>
                        <xsl:value-of select="DRAWINGTYPE"/>
                      </DrawingType>
                      <CanPropogateStructureEdits>
                          <xsl:value-of select="CanPropogateStructureEdits"/>
                      </CanPropogateStructureEdits>
                      <PropertyList>
                        <xsl:for-each select="..">
                          <xsl:apply-templates select="PropertyList"/>
                        </xsl:for-each>
                      </PropertyList>
                      <IdentifierList>
                        <xsl:for-each select="../IdentifierList/ROW">
                          <Identifier>
                            <ID>
                              <xsl:value-of select="ID"/>
                            </ID>
                            <IdentifierID>
                              <xsl:for-each select="TYPE">
                                <xsl:attribute name="Description">
                                  <xsl:value-of select="../DESCRIPTION"/>
                                </xsl:attribute>
                                <xsl:attribute name="Name">
                                  <xsl:value-of select="../NAME"/>
                                </xsl:attribute>
                                <xsl:attribute name="Active">
                                  <xsl:value-of select="../ACTIVE"/>
                                </xsl:attribute>
                                <xsl:value-of select="."/>
                              </xsl:for-each>
                            </IdentifierID>
                            <InputText>
                              <xsl:value-of select="VALUE"/>
                            </InputText>
                          </Identifier>
                        </xsl:for-each>
                      </IdentifierList>
                    </Structure>
                  </xsl:for-each>
                </BaseFragment>
                <FragmentList>
                  <xsl:for-each select="../FragmentList/ROW">
                    <Fragment>
                      <CompoundFragmentID>
                        <xsl:value-of select="ID"/>
                      </CompoundFragmentID>
                      <FragmentID>
                        <xsl:value-of select="FRAGMENTID"/>
                      </FragmentID>
                      <Code>
                        <xsl:value-of select="CODE"/>
                      </Code>
                      <Name>
                        <xsl:value-of select="DESCRIPTION"/>
                      </Name>
                      <FragmentTypeID>
                        <xsl:attribute name="lookupTable">
                          <xsl:value-of select="''FragmentType''"/>
                        </xsl:attribute>
                        <xsl:attribute name="lookupField">
                          <xsl:value-of select="''ID''"/>
                        </xsl:attribute>
                        <xsl:attribute name="displayField">
                          <xsl:value-of select="''Description''"/>
                        </xsl:attribute>
                        <xsl:attribute name="displayValue">
                          <xsl:value-of select="TYPEDESCRIPTION"/>
                        </xsl:attribute>
                        <xsl:for-each select="FRAGMENTTYPEID">
                          <xsl:value-of select="."/>
                        </xsl:for-each>
                      </FragmentTypeID>
                      <DateCreated>
                        <xsl:value-of select="translate(CREATED, $VUPPER, $Vlower)"/>
                      </DateCreated>
                      <DateLastModified>
                        <xsl:value-of select="translate(MODIFIED, $VUPPER, $Vlower)"/>
                      </DateLastModified>
                      <Equivalents>
                        <xsl:value-of select="EQUIVALENTS"/>
                      </Equivalents>
                      <Structure>
                        <StructureFormat>
                          <xsl:value-of select="STRUCTUREFORMAT"/>
                        </StructureFormat>
                        <xsl:variable name="VROW1" select="."/>
                        <Structure>
                          <xsl:for-each select="STRUCTURE">
                            <xsl:for-each select="$VROW1/MOLWEIGHT">
                              <xsl:attribute name="molWeight">
                                <xsl:value-of select="."/>
                              </xsl:attribute>
                            </xsl:for-each>
                            <xsl:for-each select="$VROW1/FORMULA">
                              <xsl:attribute name="formula">
                                <xsl:value-of select="."/>
                              </xsl:attribute>
                            </xsl:for-each>
                            <xsl:value-of select="."/>
                          </xsl:for-each>
                        </Structure>
                      </Structure>
                    </Fragment>
                  </xsl:for-each>
                </FragmentList>
                <IdentifierList>
                  <xsl:for-each select="../IdentifierList/ROW">
                    <Identifier>
                      <ID>
                        <xsl:value-of select="ID"/>
                      </ID>
                      <IdentifierID>
                        <xsl:for-each select="TYPE">
                          <xsl:attribute name="Description">
                            <xsl:value-of select="../DESCRIPTION"/>
                          </xsl:attribute>
                          <xsl:attribute name="Name">
                            <xsl:value-of select="../NAME"/>
                          </xsl:attribute>
                          <xsl:attribute name="Active">
                            <xsl:value-of select="../ACTIVE"/>
                          </xsl:attribute>
                          <xsl:value-of select="."/>
                        </xsl:for-each>
                      </IdentifierID>
                      <InputText>
                        <xsl:value-of select="VALUE"/>
                      </InputText>
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
              <xsl:value-of select="COMPOUNDID"/>
            </CompoundID>
            <DateCreated>
              <xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/>
            </DateCreated>
            <PersonCreated>
              <xsl:value-of select="PERSONCREATED"/>
            </PersonCreated>
            <PersonApproved>
              <xsl:value-of select="PERSONAPPROVED"/>
            </PersonApproved>
            <PersonRegistered>
              <xsl:value-of select="PERSONREGISTERED"/>
            </PersonRegistered>
            <Tag>
              <xsl:value-of select="TAG"/>
            </Tag>
            <DateLastModified>
              <xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/>
            </DateLastModified>
            <PropertyList>
              <xsl:for-each select="..">
                <xsl:apply-templates select="PropertyList"/>
              </xsl:for-each>
            </PropertyList>
            <RegNumber>
              <RegID>
                <xsl:value-of select="REGID"/>
              </RegID>
              <SequenceNumber>
                <xsl:value-of select="SEQUENCENUMBER"/>
              </SequenceNumber>
              <RegNumber>
                <xsl:value-of select="REGNUMBER"/>
              </RegNumber>
              <SequenceID>
                <xsl:value-of select="SEQUENCEID"/>
              </SequenceID>
            </RegNumber>
            <CanPropogateComponentEdits>
              <xsl:value-of select="CanPropogateComponentEdits"/>
            </CanPropogateComponentEdits>
            <BaseFragment>
              <xsl:for-each select="../Structure/ROW">
                <Structure>
                  <StructureID>
                    <xsl:value-of select="STRUCTUREID"/>
                  </StructureID>
                  <StructureFormat>
                    <xsl:value-of select="STRUCTUREFORMAT"/>
                  </StructureFormat>
                  <Structure>
                    <xsl:for-each select="STRUCTURE">
                      <xsl:attribute name="molWeight">
                        <xsl:value-of select="../FORMULAWEIGHT"/>
                      </xsl:attribute>
                      <xsl:attribute name="formula">
                        <xsl:value-of select="../MOLECULARFORMULA"/>
                      </xsl:attribute>
                      <xsl:value-of select="."/>
                    </xsl:for-each>
                  </Structure>
                  <NormalizedStructure>
                    <xsl:value-of select="NORMALIZEDSTRUCTURE"/>
                  </NormalizedStructure>
                  <UseNormalization>
                    <xsl:value-of select="USENORMALIZATION"/>
                  </UseNormalization>
                  <DrawingType>
                    <xsl:value-of select="DRAWINGTYPE"/>
                  </DrawingType>
                  <CanPropogateStructureEdits>
                      <xsl:value-of select="CanPropogateStructureEdits"/>
                  </CanPropogateStructureEdits>
                  <PropertyList>
                    <xsl:for-each select="..">
                      <xsl:apply-templates select="PropertyList"/>
                    </xsl:for-each>
                  </PropertyList>
                  <IdentifierList>
                    <xsl:for-each select="../IdentifierList/ROW">
                      <Identifier>
                        <ID>
                          <xsl:value-of select="ID"/>
                        </ID>
                        <IdentifierID>
                          <xsl:for-each select="TYPE">
                            <xsl:attribute name="Description">
                              <xsl:value-of select="../DESCRIPTION"/>
                            </xsl:attribute>
                            <xsl:attribute name="Name">
                              <xsl:value-of select="../NAME"/>
                            </xsl:attribute>
                            <xsl:attribute name="Active">
                              <xsl:value-of select="../ACTIVE"/>
                            </xsl:attribute>
                            <xsl:value-of select="."/>
                          </xsl:for-each>
                        </IdentifierID>
                        <InputText>
                          <xsl:value-of select="VALUE"/>
                        </InputText>
                      </Identifier>
                    </xsl:for-each>
                  </IdentifierList>
                </Structure>
              </xsl:for-each>
            </BaseFragment>
            <FragmentList>
              <xsl:for-each select="../FragmentList/ROW">
                <Fragment>
                  <CompoundFragmentID>
                    <xsl:value-of select="ID"/>
                  </CompoundFragmentID>
                  <FragmentID>
                    <xsl:value-of select="FRAGMENTID"/>
                  </FragmentID>
                  <Code>
                    <xsl:value-of select="CODE"/>
                  </Code>
                  <Name>
                    <xsl:value-of select="DESCRIPTION"/>
                  </Name>
                  <FragmentTypeID>
                    <xsl:attribute name="lookupTable">
                      <xsl:value-of select="''FragmentType''"/>
                    </xsl:attribute>
                    <xsl:attribute name="lookupField">
                      <xsl:value-of select="''ID''"/>
                    </xsl:attribute>
                    <xsl:attribute name="displayField">
                      <xsl:value-of select="''Description''"/>
                    </xsl:attribute>
                    <xsl:attribute name="displayValue">
                      <xsl:value-of select="TYPEDESCRIPTION"/>
                    </xsl:attribute>
                    <xsl:for-each select="FRAGMENTTYPEID">
                      <xsl:value-of select="."/>
                    </xsl:for-each>
                  </FragmentTypeID>
                  <DateCreated>
                    <xsl:value-of select="translate(CREATED, $VUPPER, $Vlower)"/>
                  </DateCreated>
                  <DateLastModified>
                    <xsl:value-of select="translate(MODIFIED, $VUPPER, $Vlower)"/>
                  </DateLastModified>
                  <Equivalents>
                    <xsl:value-of select="EQUIVALENTS"/>
                  </Equivalents>
                  <Structure>
                    <StructureFormat>
                      <xsl:value-of select="STRUCTUREFORMAT"/>
                    </StructureFormat>
                    <xsl:variable name="VROW1" select="."/>
                    <Structure>
                      <xsl:for-each select="STRUCTURE">
                        <xsl:for-each select="$VROW1/MOLWEIGHT">
                          <xsl:attribute name="molWeight">
                            <xsl:value-of select="."/>
                          </xsl:attribute>
                        </xsl:for-each>
                        <xsl:for-each select="$VROW1/FORMULA">
                          <xsl:attribute name="formula">
                            <xsl:value-of select="."/>
                          </xsl:attribute>
                        </xsl:for-each>
                        <xsl:value-of select="."/>
                      </xsl:for-each>
                    </Structure>
                  </Structure>
                </Fragment>
              </xsl:for-each>
            </FragmentList>
            <IdentifierList>
              <xsl:for-each select="../IdentifierList/ROW">
                <xsl:variable name="VROW3" select="."/>
                <Identifier>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <IdentifierID>
                    <xsl:for-each select="TYPE">
                      <xsl:for-each select="$VROW3/DESCRIPTION">
                        <xsl:attribute name="Description">
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                      </xsl:for-each>
                      <xsl:for-each select="$VROW3/NAME">
                        <xsl:attribute name="Name">
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                      </xsl:for-each>
                      <xsl:for-each select="$VROW3/ACTIVE">
                        <xsl:attribute name="Active">
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                      </xsl:for-each>
                      <xsl:value-of select="."/>
                    </xsl:for-each>
                  </IdentifierID>
                  <InputText>
                    <xsl:value-of select="VALUE"/>
                  </InputText>
                </Identifier>
              </xsl:for-each>
            </IdentifierList>
          </Compound>
        </xsl:for-each>
      </xsl:if>
      <BatchList>
        <xsl:for-each select="Batch/ROW">
          <Batch>
            <BatchID>
              <xsl:value-of select="BATCHID"/>
            </BatchID>
            <BatchNumber>
              <xsl:value-of select="BATCHNUMBER"/>
            </BatchNumber>
            <FullRegNumber>
              <xsl:value-of select="FULLREGNUMBER"/>
            </FullRegNumber>
            <DateCreated>
              <xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/>
            </DateCreated>
            <PersonCreated>
              <xsl:for-each select="PERSONCREATED">
                <xsl:attribute name="displayName">
                  <xsl:value-of select="../PERSONCREATEDDISPLAY"/>
                </xsl:attribute>
                <xsl:value-of select="."/>
              </xsl:for-each>
            </PersonCreated>
            <PersonApproved>
              <xsl:for-each select="PERSONAPPROVED">
                <xsl:attribute name="displayName">
                  <xsl:value-of select="../PERSONAPPROVEDDISPLAY"/>
                </xsl:attribute>
                <xsl:value-of select="."/>
              </xsl:for-each>
            </PersonApproved>
            <PersonRegistered>
              <xsl:for-each select="PERSONREGISTERED">
                <xsl:attribute name="displayName">
                  <xsl:value-of select="../PERSONREGISTEREDDISPLAY"/>
                </xsl:attribute>
                <xsl:value-of select="."/>
              </xsl:for-each>
            </PersonRegistered>
            <DateLastModified>
              <xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/>
            </DateLastModified>
            <StatusID>
              <xsl:value-of select="STATUSID"/>
            </StatusID>
              <IsBatchEditable>
              <xsl:value-of select="IsBatchEditable"/>
              </IsBatchEditable>
            <ProjectList>
              <xsl:for-each select="../ProjectList/ROW">
                <Project>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <ProjectID>
                    <xsl:attribute name="Description">
                      <xsl:value-of select="DESCRIPTION"/>
                    </xsl:attribute>
                    <xsl:attribute name="Name">
                      <xsl:value-of select="NAME"/>
                    </xsl:attribute>
                    <xsl:attribute name="Active">
                      <xsl:value-of select="ACTIVE"/>
                    </xsl:attribute>
                    <xsl:value-of select="PROJECTID"/>
                  </ProjectID>
                </Project>
              </xsl:for-each>
            </ProjectList>
            <IdentifierList>
              <xsl:for-each select="../IdentifierList/ROW">
                <Identifier>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <IdentifierID>
                    <xsl:attribute name="Description">
                      <xsl:value-of select="DESCRIPTION"/>
                    </xsl:attribute>
                    <xsl:attribute name="Name">
                      <xsl:value-of select="NAME"/>
                    </xsl:attribute>
                    <xsl:attribute name="Active">
                      <xsl:value-of select="ACTIVE"/>
                    </xsl:attribute>
                    <xsl:value-of select="TYPE"/>
                  </IdentifierID>
                  <InputText>
                    <xsl:value-of select="VALUE"/>
                  </InputText>
                </Identifier>
              </xsl:for-each>
            </IdentifierList>
            <PropertyList>
              <xsl:for-each select="..">
                <xsl:apply-templates select="PropertyList"/>
              </xsl:for-each>
            </PropertyList>
            <BatchComponentList>
              <xsl:for-each select="../BatchComponent/ROW">
                <BatchComponent>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <BatchID>
                    <xsl:value-of select="BATCHID"/>
                  </BatchID>
                  <CompoundID>
                    <xsl:value-of select="COMPOUNDID"/>
                  </CompoundID>
                  <MixtureComponentID>
                    <xsl:value-of select="MIXTURECOMPONENTID"/>
                  </MixtureComponentID>
                  <ComponentIndex>
                    <xsl:value-of select="COMPONENTINDEX"/>
                  </ComponentIndex>
                  <ComponentRegNum>
                    <xsl:value-of select="REGNUMBER"/>
                  </ComponentRegNum>
                  <PropertyList>
                    <xsl:for-each select="..">
                      <xsl:apply-templates select="PropertyList"/>
                    </xsl:for-each>
                  </PropertyList>
                  <BatchComponentFragmentList>
                    <xsl:for-each select="../BatchComponentFragment/ROW">
                      <BatchComponentFragment>
                        <ID>
                          <xsl:value-of select="ID"/>
                        </ID>
                        <CompoundFragmentID>
                          <xsl:value-of select="COMPOUNDFRAGMENTID"/>
                        </CompoundFragmentID>
                        <FragmentID>
                          <xsl:value-of select="FRAGMENTID"/>
                        </FragmentID>
                        <Equivalents>
                          <xsl:value-of select="EQUIVALENT"/>
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
  <xsl:template match="PropertyList">
    <xsl:for-each select="ROW">
      <xsl:for-each select="*">
        <Property>
          <xsl:attribute name="name">
            <xsl:value-of select="name()"/>
          </xsl:attribute>
          <xsl:if test="string-length(@pickListDomainID)!=0">
            <xsl:attribute name="pickListDomainID">
              <xsl:value-of select="@pickListDomainID"/>
            </xsl:attribute>
            <xsl:attribute name="pickListDisplayValue">
              <xsl:value-of select="@pickListDisplayValue"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:value-of select="."/>
        </Property>
      </xsl:for-each>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>'
);
      else
vXslRetrieveMcrr := XmlType.CreateXml(
'<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:variable name="Vlower" select="''abcdefghijklmnopqrstuvwxyz''"/>
  <xsl:variable name="VUPPER" select="''ABCDEFGHIJKLMNOPQRSTUVWXYZ''"/>
  <xsl:template match="/MultiCompoundRegistryRecord">
    <MultiCompoundRegistryRecord>
      <xsl:attribute name="SameBatchesIdentity">
        <xsl:value-of select="@SameBatchesIdentity"/>
      </xsl:attribute>
      <xsl:attribute name="ActiveRLS">
        <xsl:value-of select="@ActiveRLS"/>
      </xsl:attribute>
      <xsl:attribute name="IsEditable">
        <xsl:value-of select="@IsEditable"/>
      </xsl:attribute>
      <xsl:attribute name="IsRegistryDeleteable">
        <xsl:value-of select="@IsRegistryDeleteable"/>
      </xsl:attribute>
      <xsl:variable name="VMultiCompoundRegistryRecord" select="."/>
      <xsl:variable name="VTypeRegistryRecord" select="@TypeRegistryRecord"/>
      <xsl:for-each select="Mixture/ROW">
        <ID>
          <xsl:value-of select="MIXTUREID"/>
        </ID>
        <DateCreated>
          <xsl:value-of select="CREATED"/>
        </DateCreated>
        <DateLastModified>
          <xsl:value-of select="MODIFIED"/>
        </DateLastModified>
        <PersonCreated>
          <xsl:value-of select="PERSONCREATED"/>
        </PersonCreated>
        <PersonApproved>
          <xsl:value-of select="PERSONAPPROVED"/>
        </PersonApproved>
        <StructureAggregation>
          <xsl:value-of select="STRUCTUREAGGREGATION"/>
        </StructureAggregation>
        <xsl:copy-of select="../PropertyList"/>
        <RegNumber>
          <RegID>
            <xsl:value-of select="REGID"/>
          </RegID>
          <SequenceNumber>
            <xsl:value-of select="SEQUENCENUMBER"/>
          </SequenceNumber>
          <RegNumber>
            <xsl:value-of select="REGNUMBER"/>
          </RegNumber>
          <SequenceID>
            <xsl:value-of select="SEQUENCEID"/>
          </SequenceID>
        </RegNumber>
        <IdentifierList>
          <xsl:for-each select="../IdentifierList/ROW">
            <Identifier>
              <ID>
                <xsl:value-of select="ID"/>
              </ID>
              <IdentifierID>
                <xsl:for-each select="TYPE">
                  <xsl:attribute name="Description">
                    <xsl:value-of select="../DESCRIPTION"/>
                  </xsl:attribute>
                  <xsl:attribute name="Name">
                    <xsl:value-of select="../NAME"/>
                  </xsl:attribute>
                  <xsl:attribute name="Active">
                    <xsl:value-of select="../ACTIVE"/>
                  </xsl:attribute>
                  <xsl:value-of select="."/>
                </xsl:for-each>
              </IdentifierID>
              <InputText>
                <xsl:value-of select="VALUE"/>
              </InputText>
            </Identifier>
          </xsl:for-each>
        </IdentifierList>
        <ProjectList>
          <xsl:for-each select="../ProjectList/ROW">
            <Project>
              <ID>
                <xsl:value-of select="ID"/>
              </ID>
              <ProjectID>
                <xsl:attribute name="Description">
                  <xsl:value-of select="DESCRIPTION"/>
                </xsl:attribute>
                <xsl:attribute name="Name">
                  <xsl:value-of select="NAME"/>
                </xsl:attribute>
                <xsl:attribute name="Active">
                  <xsl:value-of select="ACTIVE"/>
                </xsl:attribute>
                <xsl:value-of select="PROJECTID"/>
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
                <xsl:value-of select="COMPONENTINDEX"/>
              </ComponentIndex>
              <Compound>
                <CompoundID>
                  <xsl:value-of select="COMPOUNDID"/>
                </CompoundID>
                <DateCreated>
                  <xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/>
                </DateCreated>
                <PersonCreated>
                  <xsl:value-of select="PERSONCREATED"/>
                </PersonCreated>
                <PersonApproved>
                  <xsl:value-of select="PERSONAPPROVED"/>
                </PersonApproved>
                <PersonRegistered>
                  <xsl:value-of select="PERSONREGISTERED"/>
                </PersonRegistered>
                <DateLastModified>
                  <xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/>
                </DateLastModified>
                <Tag>
                  <xsl:value-of select="TAG"/>
                </Tag>
                <xsl:copy-of select="../PropertyList"/>
                <RegNumber>
                  <RegID>
                    <xsl:if test="string-length(REGID)!=0">
                      <xsl:value-of select="REGID"/>
                    </xsl:if>
                    <xsl:if test="string-length(REGID)=0">
                      <xsl:value-of select="''0''"/>
                    </xsl:if>
                  </RegID>
                  <SequenceNumber>
                    <xsl:value-of select="SEQUENCENUMBER"/>
                  </SequenceNumber>
                  <RegNumber>
                    <xsl:value-of select="REGNUMBER"/>
                  </RegNumber>
                  <SequenceID>
                    <xsl:value-of select="SEQUENCEID"/>
                  </SequenceID>
                </RegNumber>
                <CanPropogateComponentEdits>
                  <xsl:value-of select="CanPropogateComponentEdits"/>
                </CanPropogateComponentEdits>
                <BaseFragment>
                  <xsl:for-each select="../Structure/ROW">
                    <Structure>
                      <StructureID>
                        <xsl:value-of select="STRUCTUREID"/>
                      </StructureID>
                      <StructureFormat>
                        <xsl:value-of select="STRUCTUREFORMAT"/>
                      </StructureFormat>
                      <Structure>
                        <xsl:for-each select="STRUCTURE">
                          <xsl:attribute name="molWeight">
                            <xsl:value-of select="../FORMULAWEIGHT"/>
                          </xsl:attribute>
                          <xsl:attribute name="formula">
                            <xsl:value-of select="../MOLECULARFORMULA"/>
                          </xsl:attribute>
                          <xsl:value-of select="."/>
                        </xsl:for-each>
                      </Structure>
                      <NormalizedStructure>
                        <xsl:value-of select="NORMALIZEDSTRUCTURE"/>
                      </NormalizedStructure>
                      <UseNormalization>
                        <xsl:value-of select="USENORMALIZATION"/>
                      </UseNormalization>
                      <CanPropogateStructureEdits>
                          <xsl:value-of select="CanPropogateStructureEdits"/>
                      </CanPropogateStructureEdits>
                      <xsl:copy-of select="../PropertyList"/>
                      <IdentifierList>
                        <xsl:for-each select="../IdentifierList/ROW">
                          <Identifier>
                            <ID>
                              <xsl:value-of select="ID"/>
                            </ID>
                            <IdentifierID>
                              <xsl:for-each select="TYPE">
                                <xsl:attribute name="Description">
                                  <xsl:value-of select="../DESCRIPTION"/>
                                </xsl:attribute>
                                <xsl:attribute name="Name">
                                  <xsl:value-of select="../NAME"/>
                                </xsl:attribute>
                                <xsl:attribute name="Active">
                                  <xsl:value-of select="../ACTIVE"/>
                                </xsl:attribute>
                                <xsl:value-of select="."/>
                              </xsl:for-each>
                            </IdentifierID>
                            <InputText>
                              <xsl:value-of select="VALUE"/>
                            </InputText>
                          </Identifier>
                        </xsl:for-each>
                      </IdentifierList>
                    </Structure>
                  </xsl:for-each>
                </BaseFragment>
                <FragmentList>
                  <xsl:for-each select="../FragmentList/ROW">
                    <Fragment>
                      <CompoundFragmentID>
                        <xsl:value-of select="ID"/>
                      </CompoundFragmentID>
                      <FragmentID>
                        <xsl:value-of select="FRAGMENTID"/>
                      </FragmentID>
                      <Code>
                        <xsl:value-of select="CODE"/>
                      </Code>
                      <Name>
                        <xsl:value-of select="DESCRIPTION"/>
                      </Name>
                      <FragmentTypeID>
                        <xsl:attribute name="lookupTable">
                          <xsl:value-of select="''FragmentType''"/>
                        </xsl:attribute>
                        <xsl:attribute name="lookupField">
                          <xsl:value-of select="''ID''"/>
                        </xsl:attribute>
                        <xsl:attribute name="displayField">
                          <xsl:value-of select="''Description''"/>
                        </xsl:attribute>
                        <xsl:attribute name="displayValue">
                          <xsl:value-of select="TYPEDESCRIPTION"/>
                        </xsl:attribute>
                        <xsl:for-each select="FRAGMENTTYPEID">
                          <xsl:value-of select="."/>
                        </xsl:for-each>
                      </FragmentTypeID>
                      <DateCreated>
                        <xsl:value-of select="translate(CREATED, $VUPPER, $Vlower)"/>
                      </DateCreated>
                      <DateLastModified>
                        <xsl:value-of select="translate(MODIFIED, $VUPPER, $Vlower)"/>
                      </DateLastModified>
                      <Equivalents>
                        <xsl:value-of select="EQUIVALENTS"/>
                      </Equivalents>
                      <Structure>
                        <StructureFormat>
                          <xsl:value-of select="STRUCTUREFORMAT"/>
                        </StructureFormat>
                        <xsl:variable name="VROW1" select="."/>
                        <Structure>
                          <xsl:for-each select="STRUCTURE">
                            <xsl:for-each select="$VROW1/MOLWEIGHT">
                              <xsl:attribute name="molWeight">
                                <xsl:value-of select="."/>
                              </xsl:attribute>
                            </xsl:for-each>
                            <xsl:for-each select="$VROW1/FORMULA">
                              <xsl:attribute name="formula">
                                <xsl:value-of select="."/>
                              </xsl:attribute>
                            </xsl:for-each>
                            <xsl:value-of select="."/>
                          </xsl:for-each>
                        </Structure>
                      </Structure>
                    </Fragment>
                  </xsl:for-each>
                </FragmentList>
                <IdentifierList>
                  <xsl:for-each select="../IdentifierList/ROW">
                    <Identifier>
                      <ID>
                        <xsl:value-of select="ID"/>
                      </ID>
                      <IdentifierID>
                        <xsl:for-each select="TYPE">
                          <xsl:attribute name="Description">
                            <xsl:value-of select="../DESCRIPTION"/>
                          </xsl:attribute>
                          <xsl:attribute name="Name">
                            <xsl:value-of select="../NAME"/>
                          </xsl:attribute>
                          <xsl:attribute name="Active">
                            <xsl:value-of select="../ACTIVE"/>
                          </xsl:attribute>
                          <xsl:value-of select="."/>
                        </xsl:for-each>
                      </IdentifierID>
                      <InputText>
                        <xsl:value-of select="VALUE"/>
                      </InputText>
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
              <xsl:value-of select="COMPOUNDID"/>
            </CompoundID>
            <DateCreated>
              <xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/>
            </DateCreated>
            <PersonCreated>
              <xsl:value-of select="PERSONCREATED"/>
            </PersonCreated>
             <PersonApproved>
              <xsl:value-of select="PERSONAPPROVED"/>
            </PersonApproved>
            <PersonRegistered>
              <xsl:value-of select="PERSONREGISTERED"/>
            </PersonRegistered>
            <Tag>
              <xsl:value-of select="TAG"/>
            </Tag>
            <DateLastModified>
              <xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/>
            </DateLastModified>
            <xsl:copy-of select="../PropertyList"/>
            <RegNumber>
              <RegID>
                <xsl:value-of select="REGID"/>
              </RegID>
              <SequenceNumber>
                <xsl:value-of select="SEQUENCENUMBER"/>
              </SequenceNumber>
              <RegNumber>
                <xsl:value-of select="REGNUMBER"/>
              </RegNumber>
              <SequenceID>
                <xsl:value-of select="SEQUENCEID"/>
              </SequenceID>
            </RegNumber>
            <CanPropogateComponentEdits>
              <xsl:value-of select="CanPropogateComponentEdits"/>
            </CanPropogateComponentEdits>
            <BaseFragment>
              <Structure>
                <StructureID>
                  <xsl:value-of select="STRUCTUREID"/>
                </StructureID>
                <StructureFormat>
                  <xsl:value-of select="STRUCTUREFORMAT"/>
                </StructureFormat>
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
                  <xsl:copy-of select="../PropertyList"/>
                  <IdentifierList>
                    <xsl:for-each select="../IdentifierList/ROW">
                      <Identifier>
                        <ID>
                          <xsl:value-of select="ID"/>
                        </ID>
                        <IdentifierID>
                          <xsl:for-each select="TYPE">
                            <xsl:attribute name="Description">
                              <xsl:value-of select="../DESCRIPTION"/>
                            </xsl:attribute>
                            <xsl:attribute name="Name">
                              <xsl:value-of select="../NAME"/>
                            </xsl:attribute>
                            <xsl:attribute name="Active">
                              <xsl:value-of select="../ACTIVE"/>
                            </xsl:attribute>
                            <xsl:value-of select="."/>
                          </xsl:for-each>
                        </IdentifierID>
                        <InputText>
                          <xsl:value-of select="VALUE"/>
                        </InputText>
                      </Identifier>
                    </xsl:for-each>
                  </IdentifierList>
                </Structure>
                <NormalizedStructure>
                  <xsl:value-of select="NORMALIZEDSTRUCTURE"/>
                </NormalizedStructure>
                <UseNormalization>
                  <xsl:value-of select="USENORMALIZATION"/>
                </UseNormalization>
                <CanPropogateStructureEdits>
                     <xsl:value-of select="CanPropogateStructureEdits"/>
                </CanPropogateStructureEdits>
              </Structure>
              <xsl:copy-of select="../PropertyList"/>
            </BaseFragment>
            <FragmentList>
              <xsl:for-each select="Fragment/ROW">
                <Fragment>
                  <CompoundFragmentID>
                    <xsl:value-of select="ID"/>
                  </CompoundFragmentID>
                  <FragmentID>
                    <xsl:value-of select="FRAGMENTID"/>
                  </FragmentID>
                  <Code>
                    <xsl:value-of select="CODE"/>
                  </Code>
                  <Name>
                    <xsl:value-of select="DESCRIPTION"/>
                  </Name>
                  <FragmentTypeID>
                    <xsl:attribute name="lookupTable">
                      <xsl:value-of select="''FragmentType''"/>
                    </xsl:attribute>
                    <xsl:attribute name="lookupField">
                      <xsl:value-of select="''ID''"/>
                    </xsl:attribute>
                    <xsl:attribute name="displayField">
                      <xsl:value-of select="''Description''"/>
                    </xsl:attribute>
                    <xsl:attribute name="displayValue">
                      <xsl:value-of select="TYPEDESCRIPTION"/>
                    </xsl:attribute>
                    <xsl:for-each select="FRAGMENTTYPEID">
                      <xsl:value-of select="."/>
                    </xsl:for-each>
                  </FragmentTypeID>
                  <DateCreated>
                    <xsl:value-of select="translate(CREATED, $VUPPER, $Vlower)"/>
                  </DateCreated>
                  <DateLastModified>
                    <xsl:value-of select="translate(MODIFIED, $VUPPER, $Vlower)"/>
                  </DateLastModified>
                  <Equivalents>
                    <xsl:value-of select="EQUIVALENTS"/>
                  </Equivalents>
                  <Structure>
                    <StructureFormat>
                      <xsl:value-of select="STRUCTUREFORMAT"/>
                    </StructureFormat>
                    <xsl:variable name="VROW1" select="."/>
                    <Structure>
                      <xsl:for-each select="STRUCTURE">
                        <xsl:for-each select="$VROW1/MOLWEIGHT">
                          <xsl:attribute name="molWeight">
                            <xsl:value-of select="."/>
                          </xsl:attribute>
                        </xsl:for-each>
                        <xsl:for-each select="$VROW1/FORMULA">
                          <xsl:attribute name="formula">
                            <xsl:value-of select="."/>
                          </xsl:attribute>
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
                    <xsl:value-of select="ID"/>
                  </ID>
                  <IdentifierID>
                    <xsl:for-each select="TYPE">
                      <xsl:for-each select="$VROW3/DESCRIPTION">
                        <xsl:attribute name="Description">
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                      </xsl:for-each>
                      <xsl:for-each select="$VROW3/NAME">
                        <xsl:attribute name="Name">
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                      </xsl:for-each>
                      <xsl:for-each select="$VROW3/ACTIVE">
                        <xsl:attribute name="Active">
                          <xsl:value-of select="."/>
                        </xsl:attribute>
                      </xsl:for-each>
                      <xsl:value-of select="."/>
                    </xsl:for-each>
                  </IdentifierID>
                  <InputText>
                    <xsl:value-of select="VALUE"/>
                  </InputText>
                </Identifier>
              </xsl:for-each>
            </IdentifierList>
          </Compound>
        </xsl:for-each>
      </xsl:if>
      <BatchList>
        <xsl:for-each select="Batch/ROW">
          <Batch>
            <BatchID>
              <xsl:value-of select="BATCHID"/>
            </BatchID>
            <BatchNumber>
              <xsl:value-of select="BATCHNUMBER"/>
            </BatchNumber>
            <FullRegNumber>
              <xsl:value-of select="FULLREGNUMBER"/>
            </FullRegNumber>
            <DateCreated>
              <xsl:value-of select="translate(DATECREATED, $VUPPER, $Vlower)"/>
            </DateCreated>
            <PersonCreated>
              <xsl:for-each select="PERSONCREATED">
                <xsl:attribute name="displayName">
                  <xsl:value-of select="../PERSONCREATEDDISPLAY"/>
                </xsl:attribute>
                <xsl:value-of select="."/>
              </xsl:for-each>
            </PersonCreated>
            <PersonApproved>
              <xsl:for-each select="PERSONAPPROVED">
                <xsl:attribute name="displayName">
                  <xsl:value-of select="../PERSONAPPROVEDDISPLAY"/>
                </xsl:attribute>
                <xsl:value-of select="."/>
              </xsl:for-each>
            </PersonApproved>
            <PersonRegistered>
              <xsl:for-each select="PERSONREGISTERED">
                <xsl:attribute name="displayName">
                  <xsl:value-of select="../PERSONREGISTEREDDISPLAY"/>
                </xsl:attribute>
                <xsl:value-of select="."/>
              </xsl:for-each>
            </PersonRegistered>
            <DateLastModified>
              <xsl:value-of select="translate(DATELASTMODIFIED, $VUPPER, $Vlower)"/>
            </DateLastModified>
            <StatusID>
              <xsl:value-of select="STATUSID"/>
            </StatusID>
              <IsBatchEditable>
              <xsl:value-of select="IsBatchEditable"/>
              </IsBatchEditable>
            <ProjectList>
              <xsl:for-each select="../ProjectList/ROW">
                <Project>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <ProjectID>
                    <xsl:attribute name="Description">
                      <xsl:value-of select="DESCRIPTION"/>
                    </xsl:attribute>
                    <xsl:attribute name="Name">
                      <xsl:value-of select="NAME"/>
                    </xsl:attribute>
                    <xsl:attribute name="Active">
                      <xsl:value-of select="ACTIVE"/>
                    </xsl:attribute>
                    <xsl:value-of select="PROJECTID"/>
                  </ProjectID>
                </Project>
              </xsl:for-each>
            </ProjectList>
            <IdentifierList>
              <xsl:for-each select="../IdentifierList/ROW">
                <Identifier>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <IdentifierID>
                    <xsl:attribute name="Description">
                      <xsl:value-of select="DESCRIPTION"/>
                    </xsl:attribute>
                    <xsl:attribute name="Name">
                      <xsl:value-of select="NAME"/>
                    </xsl:attribute>
                    <xsl:attribute name="Active">
                      <xsl:value-of select="ACTIVE"/>
                    </xsl:attribute>
                    <xsl:value-of select="TYPE"/>
                  </IdentifierID>
                  <InputText>
                    <xsl:value-of select="VALUE"/>
                  </InputText>
                </Identifier>
              </xsl:for-each>
            </IdentifierList>
            <xsl:copy-of select="../PropertyList"/>
            <BatchComponentList>
              <xsl:for-each select="../BatchComponent/ROW">
                <BatchComponent>
                  <ID>
                    <xsl:value-of select="ID"/>
                  </ID>
                  <BatchID>
                    <xsl:value-of select="BATCHID"/>
                  </BatchID>
                  <CompoundID>
                    <xsl:value-of select="COMPOUNDID"/>
                  </CompoundID>
                  <MixtureComponentID>
                    <xsl:value-of select="MIXTURECOMPONENTID"/>
                  </MixtureComponentID>
                  <ComponentIndex>
                    <xsl:value-of select="COMPONENTINDEX"/>
                  </ComponentIndex>
                  <xsl:copy-of select="../PropertyList"/>
                  <BatchComponentFragmentList>
                    <xsl:for-each select="../BatchComponentFragment/ROW">
                      <BatchComponentFragment>
                        <ID>
                          <xsl:value-of select="ID"/>
                        </ID>
                        <CompoundFragmentID>
                          <xsl:value-of select="COMPOUNDFRAGMENTID"/>
                        </CompoundFragmentID>
                        <FragmentID>
                          <xsl:value-of select="FRAGMENTID"/>
                        </FragmentID>
                        <Equivalents>
                          <xsl:value-of select="EQUIVALENT"/>
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
</xsl:stylesheet>'
);
      end if;
    end if;
    TraceWrite( act||'XslMcrrFetch_ended', $$plsql_line,'end' );

    RETURN VXslRetrieveMCRR;
  END;

  /** XSL transform used to create a 'permanent' registry record
  -->author jed
  -->since January 2011
  -->return an XSLT for creating permanent records
  */
  FUNCTION XslMcrrCreate RETURN XmlType IS
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'XslMcrrCreate_started', $$plsql_line,'start' );
    if (VXslCreateMCRR is null) then
VXslCreateMCRR := XmlType.CreateXml(
'<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:variable name="Vlower" select="''abcdefghijklmnopqrstuvwxyz''"/>
  <xsl:variable name="VUPPER" select="''ABCDEFGHIJKLMNOPQRSTUVWXYZ''"/>
  <xsl:template match="/MultiCompoundRegistryRecord">
    <xsl:for-each select="RegNumber">
      <xsl:variable name="VRegNumber" select="RegNumber"/>
      <VW_RegistryNumber>
        <ROW>
          <REGID>0</REGID>
          <SEQUENCENUMBER>
            <xsl:value-of select="SequenceNumber"/>
          </SEQUENCENUMBER>
          <REGNUMBER>
            <xsl:choose>
              <xsl:when test="string-length($VRegNumber) != 0">
                <xsl:value-of select="$VRegNumber"/>
              </xsl:when>
              <xsl:otherwise>null</xsl:otherwise>
            </xsl:choose>
          </REGNUMBER>
          <SEQUENCEID>
            <xsl:value-of select="SequenceID"/>
          </SEQUENCEID>
          <DATECREATED>
            <xsl:value-of select="/MultiCompoundRegistryRecord/DateCreated"/>
          </DATECREATED>
          <PERSONREGISTERED>
            <xsl:value-of select="/MultiCompoundRegistryRecord/PersonCreated"/>
          </PERSONREGISTERED>
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
         <PERSONAPPROVED>
          <xsl:value-of select="PersonApproved"/>
        </PERSONAPPROVED>
        <MODIFIED>
          <xsl:value-of select="DateLastModified"/>
        </MODIFIED>
        <STATUSID>
          <xsl:value-of select="StatusID"/>
        </STATUSID>
        <xsl:for-each select="PropertyList/Property">
          <xsl:variable name="eValue" select="."/>
          <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
          <xsl:element name="{$eName}">
               <xsl:value-of select="substring($eValue,string-length(substring-before($eValue,substring(translate($eValue, '' '', ''''),1,1))) +1) "/>
          </xsl:element>
        </xsl:for-each>
      </ROW>
    </VW_Mixture>
    <xsl:for-each select="ProjectList/Project">
      <xsl:if test="ProjectID!=''''">
        <VW_RegistryNumber_Project>
          <ROW>
            <PROJECTID>
              <xsl:value-of select="ProjectID"/>
            </PROJECTID>
            <REGID>0</REGID>
            <ORDERINDEX>
              <xsl:value-of select="OrderIndex"/>
            </ORDERINDEX>
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
            <xsl:value-of select="InputText"/>
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
          <FULLREGNUMBER>
            <xsl:choose>
              <xsl:when test="FullRegNumber!=''''">
                <xsl:value-of select="FullRegNumber"/>
              </xsl:when>
              <xsl:otherwise>null</xsl:otherwise>
            </xsl:choose>
          </FULLREGNUMBER>
          <DATECREATED>
            <xsl:value-of select="DateCreated"/>
          </DATECREATED>
          <PERSONCREATED>
            <xsl:value-of select="PersonCreated"/>
          </PERSONCREATED>
           <PERSONAPPROVED>
            <xsl:value-of select="PersonApproved"/>
          </PERSONAPPROVED>
          <PERSONREGISTERED>
            <xsl:value-of select="PersonRegistered"/>
          </PERSONREGISTERED>
          <DATELASTMODIFIED>
            <xsl:value-of select="DateLastModified"/>
          </DATELASTMODIFIED>
          <STATUSID>
            <xsl:value-of select="StatusID"/>
          </STATUSID>
          <REGID>0</REGID>
          <TEMPBATCHID>
            <xsl:value-of select="BatchID"/>
          </TEMPBATCHID>
          <xsl:for-each select="PropertyList/Property">
            <xsl:variable name="eValue" select="text()"/>
            <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
            <!-- conditionally create the element -->
            <xsl:choose>
              <xsl:when test="$eName = ''DELIVERYDATE'' and string-length($eValue) != 0">
                <xsl:element name="{$eName}">
                  <xsl:value-of select="$eValue"/>
                </xsl:element>
              </xsl:when>
              <xsl:when test="$eName = ''DATEENTERED'' and string-length($eValue) != 0">
                <xsl:element name="{$eName}">
                  <xsl:value-of select="$eValue"/>
                </xsl:element>
              </xsl:when>
              <xsl:otherwise>
                <xsl:element name="{$eName}">
                  <xsl:value-of select="normalize-space($eValue)"/>
                </xsl:element>
              </xsl:otherwise>
            </xsl:choose>
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
      <xsl:for-each select="IdentifierList/Identifier">
        <VW_BatchIdentifier>
          <ROW>
            <ID>0</ID>
            <TYPE>
              <xsl:value-of select="IdentifierID"/>
            </TYPE>
            <VALUE>
              <xsl:value-of select="InputText"/>
            </VALUE>
            <BATCHID>0</BATCHID>
            <ORDERINDEX>
              <xsl:value-of select="OrderIndex"/>
            </ORDERINDEX>
          </ROW>
        </VW_BatchIdentifier>
      </xsl:for-each>
    </xsl:for-each>
    <xsl:for-each select="ComponentList/Component">
      <xsl:variable name="VComponentIndex" select="ComponentIndex/."/>
      <xsl:for-each select="Compound">
        <xsl:variable name="VCompound" select="."/>
        <xsl:for-each select="RegNumber">
          <VW_RegistryNumber>
            <ROW>
              <REGID>
                <xsl:value-of select="RegID"/>
              </REGID>
              <!-- this is an odd construct: " " -->
              <SEQUENCENUMBER>
                <xsl:value-of select="SequenceNumber"/>
              </SEQUENCENUMBER>
              <REGNUMBER>" "</REGNUMBER>
              <SEQUENCEID>
                <xsl:value-of select="SequenceID"/>
              </SEQUENCEID>
              <DATECREATED>
                <xsl:value-of select="$VCompound/DateCreated"/>
              </DATECREATED>
              <PERSONREGISTERED>
                <xsl:value-of select="$VCompound/PersonRegistered"/>
              </PERSONREGISTERED>
            </ROW>
          </VW_RegistryNumber>
        </xsl:for-each>
        <xsl:choose>
          <xsl:when test="RegNumber/RegID=''0''">
            <xsl:for-each select="BaseFragment/Structure">
              <VW_Structure>
                <ROW>
                  <STRUCTUREID>
                    <xsl:value-of select="StructureID"/>
                  </STRUCTUREID>
                  <STRUCTUREFORMAT>
                    <xsl:value-of select="StructureFormat"/>
                  </STRUCTUREFORMAT>
                  <DRAWINGTYPE>
                    <xsl:value-of select="DrawingType"/>
                  </DRAWINGTYPE>
                  <xsl:for-each select="PropertyList/Property">
                    <xsl:variable name="eValue" select="."/>
                    <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
                    <xsl:element name="{$eName}">
                      <xsl:value-of select="normalize-space($eValue)"/>
                    </xsl:element>
                  </xsl:for-each>
                </ROW>
              </VW_Structure>
            </xsl:for-each>
            <xsl:for-each select="BaseFragment/Structure/IdentifierList/Identifier">
              <VW_Structure_Identifier>
                <ROW>
                  <ID>0</ID>
                  <TYPE>
                    <xsl:value-of select="IdentifierID"/>
                  </TYPE>
                  <VALUE>
                    <xsl:value-of select="InputText"/>
                  </VALUE>
                  <STRUCTUREID>0</STRUCTUREID>
                  <ORDERINDEX>
                    <xsl:value-of select="OrderIndex"/>
                  </ORDERINDEX>
                </ROW>
              </VW_Structure_Identifier>
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
                 <PERSONAPPROVED>
                  <xsl:value-of select="PersonApproved"/>
                </PERSONAPPROVED>
                <PERSONREGISTERED>
                  <xsl:value-of select="PersonRegistered"/>
                </PERSONREGISTERED>
                <DATELASTMODIFIED>
                  <xsl:value-of select="DateLastModified"/>
                </DATELASTMODIFIED>
                <TAG>
                  <xsl:value-of select="Tag"/>
                </TAG>
                <REGID>0</REGID>
                <STRUCTUREID>0</STRUCTUREID>
                <xsl:for-each select="BaseFragment/Structure">
                  <USENORMALIZATION>
                    <xsl:value-of select="UseNormalization"/>
                  </USENORMALIZATION>
                  <!-- JED: Why does this not also pull the "normalized structire" element? -->
                </xsl:for-each>
                <xsl:for-each select="PropertyList/Property">
                  <xsl:variable name="eValue" select="."/>
                  <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
                  <xsl:element name="{$eName}">
                    <xsl:value-of select="normalize-space($eValue)"/>
                  </xsl:element>
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
                  <ORDERINDEX>
                    <xsl:value-of select="OrderIndex"/>
                  </ORDERINDEX>
                  <xsl:for-each select="PropertyList/Property">
                    <xsl:variable name="eValue" select="."/>
                    <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
                    <xsl:element name="{$eName}">
                      <xsl:value-of select="normalize-space($eValue)"/>
                    </xsl:element>
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
              <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[$VComponentIndex=ComponentIndex]">
                <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment[FragmentID=$VFragmentID]">
                  <VW_BatchComponentFragment>
                    <ROW>
                      <BATCHCOMPONENTID>0</BATCHCOMPONENTID>
                      <COMPOUNDFRAGMENTID>0</COMPOUNDFRAGMENTID>
                      <EQUIVALENT>
                        <xsl:value-of select="Equivalents"/>
                      </EQUIVALENT>
                      <ORDERINDEX>
                        <xsl:value-of select="OrderIndex"/>
                      </ORDERINDEX>
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
                    <xsl:value-of select="IdentifierID"/>
                  </TYPE>
                  <VALUE>
                    <xsl:value-of select="InputText"/>
                  </VALUE>
                  <REGID>0</REGID>
                  <ID>0</ID>
                  <ORDERINDEX>
                    <xsl:value-of select="OrderIndex"/>
                  </ORDERINDEX>
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
                  <ORDERINDEX>
                    <xsl:value-of select="OrderIndex"/>
                  </ORDERINDEX>
                  <xsl:for-each select="PropertyList/Property">
                    <xsl:variable name="eValue" select="."/>
                    <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
                    <xsl:element name="{$eName}">
                      <xsl:value-of select="normalize-space($eValue)"/>
                    </xsl:element>
                  </xsl:for-each>
                </ROW>
              </VW_BatchComponent>
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
                      <ORDERINDEX>
                        <xsl:value-of select="OrderIndex"/>
                      </ORDERINDEX>
                    </ROW>
                  </VW_BatchComponentFragment>
                </xsl:for-each>
              </xsl:for-each>
            </xsl:for-each>
          </xsl:when>
        </xsl:choose>
      </xsl:for-each>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>'
);
    end if;

    TraceWrite( act||'XslMcrrCreate_ended', $$plsql_line,'end' );
    RETURN VXslCreateMCRR;
  END;

  /** XSL transform used to update a 'permanent' registry record
  -->author jed
  -->since January 2011
  -->return an XSLT for updating permanent records
  */
  FUNCTION XslMcrrUpdate RETURN XMLTYPE IS
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'XslMcrrUpdate_started', $$plsql_line,'start' );
    if (VXslUpdateMCRR is null) then

VXslUpdateMCRR := XmlType.CreateXml('
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:variable name="Vlower" select="''abcdefghijklmnopqrstuvwxyz''"/>
  <xsl:variable name="VUPPER" select="''ABCDEFGHIJKLMNOPQRSTUVWXYZ''"/>
  <xsl:template match="/MultiCompoundRegistryRecord">
    <MultiCompoundRegistryRecord>
      <xsl:variable name="VMixtureID" select="ID" />
      <xsl:variable name="VMixtureRegID" select="RegNumber/RegID/." />
      <!-- MIXTURE -->
      <VW_Mixture>
        <xsl:element name="ROW">
          <xsl:element name="MIXTUREID">
            <xsl:value-of select="ID" />
          </xsl:element>
          <xsl:if test="DateCreated/@update=''yes''">
            <xsl:element name="CREATED">
              <xsl:value-of select="DateCreated" />
            </xsl:element>
          </xsl:if>
          <xsl:if test="PersonCreated/@update=''yes''">
            <xsl:element name="PERSONCREATED">
              <xsl:value-of select="PersonCreated" />
            </xsl:element>
          </xsl:if>
           <xsl:if test="PersonApproved/@update=''yes''">
            <xsl:element name="PERSONAPPROVED">
              <xsl:value-of select="PersonApproved" />
            </xsl:element>
          </xsl:if>
          <xsl:element name="MODIFIED">
            <xsl:value-of select="DateLastModified" />
          </xsl:element>
          <xsl:if test="StructureAggregation/@update=''yes''">
            <xsl:element name="STRUCTUREAGGREGATION">
              <xsl:value-of select="StructureAggregation" />
            </xsl:element>
          </xsl:if>
          <xsl:if test="StatusID/@update=''yes''">
            <xsl:element name="STATUSID">
              <xsl:value-of select="StatusID" />
            </xsl:element>
          </xsl:if>
          <xsl:for-each select="PropertyList/Property[@update=''yes'' or @insert=''yes'']">
            <xsl:variable name="eValue" select="."/>
            <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
              <xsl:value-of select="substring($eValue,string-length(substring-before($eValue,substring(translate($eValue, '' '', ''''),1,1))) +1) "/>
            </xsl:element>
          </xsl:for-each>
        </xsl:element>
      </VW_Mixture>
      <!-- MIXTURE RegNumbers -->
      <VW_RegistryNumber>
        <xsl:for-each select="RegNumber">
          <xsl:element name="ROW">
            <xsl:element name="REGID">
              <xsl:value-of select="RegID" />
            </xsl:element>
            <xsl:if test="SequenceNumber/@update=''yes''">
              <xsl:element name="SEQUENCENUMBER">
                <xsl:value-of select="SequenceNumber" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="RegNumber/@update=''yes''">
              <xsl:element name="REGNUMBER">
                <xsl:value-of select="RegNumber" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="SequenceID/@update=''yes''">
              <xsl:element name="SEQUENCEID">
                <xsl:value-of select="SequenceID" />
              </xsl:element>
            </xsl:if>
          </xsl:element>
        </xsl:for-each>
      </VW_RegistryNumber>
      <!-- MIXTURE Projects -->
      <xsl:for-each select="ProjectList/Project">
        <xsl:variable name="VDeleteProject" select="@delete" />
        <xsl:variable name="VInsertProject" select="@insert" />
        <VW_RegistryNumber_Project>
          <xsl:choose>
            <xsl:when test="$VDeleteProject=''yes''">
              <xsl:attribute name="delete">
                <xsl:value-of select="@delete"/>
              </xsl:attribute>
            </xsl:when>
            <xsl:when test="$VInsertProject=''yes''">
              <xsl:attribute name="insert">
                <xsl:value-of select="@insert"/>
              </xsl:attribute>
            </xsl:when>
          </xsl:choose>
          <xsl:element name="ROW">
            <xsl:element name="ID">
              <xsl:value-of select="ID" />
            </xsl:element>
            <xsl:if test="ProjectID/@update=''yes'' or $VInsertProject=''yes''">
              <xsl:element name="PROJECTID">
                <xsl:value-of select="ProjectID" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="$VInsertProject=''yes''">
              <xsl:element name="REGID">
                <xsl:value-of select="$VMixtureRegID" />
              </xsl:element>
            </xsl:if>
          </xsl:element>
        </VW_RegistryNumber_Project>
      </xsl:for-each>
      <!-- MIXTURE Identifiers -->
      <xsl:for-each select="IdentifierList/Identifier">
        <xsl:variable name="VDeleteIdentifier" select="@delete" />
        <xsl:variable name="VInsertIdentifier" select="@insert" />
        <VW_Compound_Identifier>
          <xsl:choose>
            <xsl:when test="$VDeleteIdentifier=''yes''">
              <xsl:attribute name="delete">
                <xsl:value-of select="@delete"/>
              </xsl:attribute>
            </xsl:when>
            <xsl:when test="$VInsertIdentifier=''yes''">
              <xsl:attribute name="insert">
                <xsl:value-of select="@insert"/>
              </xsl:attribute>
            </xsl:when>
          </xsl:choose>
          <xsl:element name="ROW">
            <xsl:element name="ID">
              <xsl:value-of select="ID" />
            </xsl:element>
            <xsl:if test="IdentifierID/@update=''yes'' or $VInsertIdentifier=''yes''">
              <xsl:element name="TYPE">
                <xsl:value-of select="IdentifierID" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="InputText/@update=''yes'' or $VInsertIdentifier=''yes''">
              <xsl:element name="VALUE">
                <xsl:value-of select="InputText" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="$VInsertIdentifier=''yes''">
              <xsl:element name="REGID">
                <xsl:value-of select="$VMixtureRegID" />
              </xsl:element>
            </xsl:if>
          </xsl:element>
        </VW_Compound_Identifier>
      </xsl:for-each>
      ' || '
      <!-- COMPONENTS -->
      <xsl:for-each select="ComponentList/Component">
        <xsl:variable name="VComponentIndex" select="ComponentIndex/." />
        <xsl:variable name="VDeleteComponent" select="@delete" />
        <xsl:variable name="VInsertComponent" select="@insert" />
        <xsl:variable name="VCompoundID" select="Compound/CompoundID" />
        <!-- COMPOUND -->
        <xsl:for-each select="Compound">
          <xsl:variable name="VRegID" select="RegNumber/RegID" />
          <xsl:variable name="VStructID" select="BaseFragment/Structure/StructureID" />
          <xsl:variable name="VCompound" select="." />
          <!-- COMPOUND RegNumber -->
          <xsl:for-each select="RegNumber">
            <VW_RegistryNumber>
              <xsl:choose>
                <xsl:when test="$VInsertComponent=''yes''">
                  <xsl:attribute name="insert">
                    <xsl:value-of select="$VInsertComponent"/>
                  </xsl:attribute>
                </xsl:when>
              </xsl:choose>
              <xsl:element name="ROW">
                <xsl:element name="REGID">
                  <xsl:value-of select="RegID" />
                </xsl:element>
                <xsl:if test="SequenceNumber/@update=''yes'' or $VInsertComponent=''yes''">
                  <xsl:element name="SEQUENCENUMBER">
                    <xsl:value-of select="SequenceNumber" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="RegNumber/@update=''yes'' or $VInsertComponent=''yes''">
                  <xsl:element name="REGNUMBER">
                    <xsl:value-of select="RegNumber" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="SequenceID/@update=''yes'' or $VInsertComponent=''yes''">
                  <xsl:element name="SEQUENCEID">
                    <xsl:value-of select="SequenceID" />
                  </xsl:element>
                </xsl:if>
                <!-- Do we really want to alter the ''creation date'' of the record? -->
                <xsl:if test="DateCreated/@update=''yes'' or $VInsertComponent=''yes''">
                  <xsl:element name="DATECREATED">
                    <xsl:value-of select="DateCreated" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="$VCompound/PersonRegistered/@update=''yes'' or $VInsertComponent=''yes''">
                  <xsl:element name="PERSONREGISTERED">
                    <xsl:value-of select="$VCompound/PersonRegistered" />
                  </xsl:element>
                </xsl:if>
              </xsl:element>
            </VW_RegistryNumber>
          </xsl:for-each>

          <xsl:if test="$VRegID=''0'' and $VInsertComponent=''yes'' or $VDeleteComponent=''yes'' or string-length($VInsertComponent)=0 ">
            <xsl:if test="$VDeleteComponent!=''yes'' or string-length($VDeleteComponent)=0 ">
              <xsl:variable name="VInsertStructure" select="BaseFragment/Structure/@insert"/>
              <!-- STRUCTURE -->
              <VW_Structure>
                <xsl:if test="$VInsertComponent=''yes'' or $VInsertStructure=''yes'' or BaseFragment/Structure/StructureID=''0''">
                  <xsl:attribute name="insert">yes</xsl:attribute>
                </xsl:if>
                <xsl:for-each select="BaseFragment/Structure">
                  <xsl:element name="ROW">
                    <xsl:element name="STRUCTUREID">
                      <xsl:value-of select="StructureID" />
                    </xsl:element>
                    <xsl:if test="StructureFormat/@update=''yes'' or $VInsertComponent=''yes'' or @insert=''yes'' or StructureID=0 or StructureID=-1 or StructureID=-2 or StructureID=-3">
                      <xsl:element name="STRUCTUREFORMAT">
                        <xsl:value-of select="StructureFormat" />
                      </xsl:element>
                    </xsl:if>
                    <xsl:if test="DrawingType/@update=''yes'' or $VInsertComponent=''yes'' or @insert=''yes'' or StructureID=0 or StructureID=-1 or StructureID=-2 or StructureID=-3">
                      <xsl:element name="DRAWINGTYPE">
                        <xsl:value-of select="DrawingType" />
                      </xsl:element>
                    </xsl:if>
                    <xsl:if test="Structure/@update=''yes'' or $VInsertComponent=''yes'' or @insert=''yes'' or StructureID=0 or StructureID=-1 or StructureID=-2 or StructureID=-3">
                      <xsl:element name="STRUCTURE">
                        <xsl:value-of select="Structure" />
                      </xsl:element>
                    </xsl:if>
                    <xsl:for-each select="PropertyList/Property[@update=''yes'' or @insert=''yes'']">
                      <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                        <xsl:value-of select="normalize-space(.)"/>
                      </xsl:element>
                    </xsl:for-each>
                  </xsl:element>
                </xsl:for-each>
              </VW_Structure>
              <!-- STRUCTURE Identifiers -->
              <xsl:for-each select="BaseFragment/Structure/IdentifierList/Identifier">
                <xsl:variable name="VDeleteIdentifier" select="@delete" />
                <xsl:variable name="VInsertIdentifier" select="@insert" />
                <VW_Structure_Identifier>
                  <xsl:choose>
                    <xsl:when test="$VDeleteIdentifier=''yes''">
                      <xsl:attribute name="delete">
                        <xsl:value-of select="@delete"/>
                      </xsl:attribute>
                    </xsl:when>
                    <xsl:when test="$VInsertIdentifier=''yes'' or $VInsertStructure=''yes''">
                      <xsl:attribute name="insert">
                        <xsl:value-of select="@insert"/>
                      </xsl:attribute>
                    </xsl:when>
                  </xsl:choose>
                  <xsl:element name="ROW">
                    <xsl:element name="ID">
                      <xsl:value-of select="ID" />
                    </xsl:element>
                    <xsl:if test="IdentifierID/@update=''yes'' or $VInsertComponent=''yes'' or $VInsertIdentifier=''yes''">
                      <xsl:element name="TYPE">
                        <xsl:value-of select="IdentifierID" />
                      </xsl:element>
                    </xsl:if>
                    <xsl:if test="InputText/@update=''yes'' or $VInsertComponent=''yes'' or $VInsertIdentifier=''yes''">
                      <xsl:element name="VALUE">
                        <xsl:value-of select="InputText" />
                      </xsl:element>
                    </xsl:if>
                    <xsl:if test="$VInsertIdentifier=''yes''">
                      <xsl:element name="STRUCTUREID">
                        <xsl:value-of select="$VStructID" />
                      </xsl:element>
                    </xsl:if>
                  </xsl:element>
                </VW_Structure_Identifier>
              </xsl:for-each>
            </xsl:if>

            <!-- COMPOUND data -->
            <VW_Compound>
              <xsl:if test="$VDeleteComponent=''yes''">
                <xsl:attribute name="delete">yes</xsl:attribute>
              </xsl:if>
              <xsl:if test="$VInsertComponent=''yes''">
                <xsl:attribute name="insert">yes</xsl:attribute>
              </xsl:if>
              <xsl:element name="ROW">
                <xsl:element name="COMPOUNDID">
                  <xsl:value-of select="CompoundID" />
                </xsl:element>
                <xsl:if test="DateCreated/@update=''yes'' or $VInsertComponent=''yes''">
                  <xsl:element name="DATECREATED">
                    <xsl:value-of select="DateCreated" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="PersonCreated/@update=''yes'' or $VInsertComponent=''yes''">
                  <xsl:element name="PERSONCREATED">
                    <xsl:value-of select="PersonCreated" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="PersonApproved/@update=''yes'' or $VInsertComponent=''yes''">
                  <xsl:element name="PERSONAPPROVED">
                    <xsl:value-of select="PersonApproved" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="PersonRegistered/@update=''yes'' or $VInsertComponent=''yes''">
                  <xsl:element name="PERSONREGISTERED">
                    <xsl:value-of select="PersonRegistered" />
                  </xsl:element>
                </xsl:if>
                <xsl:element name="DATELASTMODIFIED">
                  <xsl:value-of select="DateLastModified" />
                </xsl:element>
                <xsl:if test="Tag/@update=''yes'' or $VInsertComponent=''yes''">
                  <xsl:element name="TAG">
                    <xsl:value-of select="Tag" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="$VInsertComponent=''yes''">
                  <xsl:element name="REGID">
                    <xsl:value-of select="RegID" />
                  </xsl:element>
                </xsl:if>
                <!-- COMPOUND structure data -->
                <xsl:for-each select="BaseFragment">
                  <xsl:for-each select="Structure">
                    <xsl:if test="@update=''yes'' or @insert=''yes'' or StructureID/@update=''yes'' or StructureID&lt;0 or $VInsertComponent=''yes''">
                      <xsl:element name="STRUCTUREID">
                        <xsl:value-of select="StructureID" />
                      </xsl:element>
                    </xsl:if>
                    <xsl:if test="UseNormalization/@update=''yes'' or $VInsertComponent=''yes''">
                      <xsl:element name="USENORMALIZATION">
                        <xsl:value-of select="UseNormalization" />
                      </xsl:element>
                    </xsl:if>
                  </xsl:for-each>
                </xsl:for-each>
                <xsl:for-each select="PropertyList/Property[@update=''yes'' or @insert=''yes'']">
                  <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                    <xsl:value-of select="normalize-space(.)"/>
                  </xsl:element>
                </xsl:for-each>
              </xsl:element>
            </VW_Compound>
            <!-- COMPOUND fragments -->
            <xsl:for-each select="FragmentList/Fragment">
              <xsl:variable name="VInsertFragment" select="@insert" />
              <xsl:variable name="VDeleteFragment" select="@delete" />
              <VW_Compound_Fragment>
                <xsl:choose>
                  <xsl:when test="CompoundFragmentID/@delete=''yes'' or $VDeleteFragment=''yes''">
                    <xsl:attribute name="delete">
                      <xsl:value-of select="@delete"/>
                    </xsl:attribute>
                  </xsl:when>
                  <xsl:when test="$VInsertFragment=''yes'' or $VInsertComponent=''yes''">
                    <xsl:attribute name="insert">
                      <xsl:value-of select="@insert"/>
                    </xsl:attribute>
                  </xsl:when>
                </xsl:choose>
                <xsl:element name="ROW">
                  <xsl:element name="ID">
                    <xsl:value-of select="CompoundFragmentID" />
                  </xsl:element>
                  <xsl:if test="$VInsertFragment=''yes'' or $VInsertComponent=''yes''">
                    <xsl:element name="COMPOUNDID">
                      <xsl:value-of select="$VCompoundID" />
                    </xsl:element>
                  </xsl:if>
                  <xsl:if test="FragmentID/@update=''yes'' or $VInsertFragment=''yes'' or $VInsertComponent=''yes''">
                    <xsl:element name="FRAGMENTID">
                      <xsl:value-of select="FragmentID" />
                    </xsl:element>
                  </xsl:if>
                </xsl:element>
              </VW_Compound_Fragment>
            </xsl:for-each>
            <!-- COMPOUND Identifiers -->
            <xsl:for-each select="IdentifierList/Identifier">
              <xsl:variable name="VDeleteIdentifier" select="@delete" />
              <xsl:variable name="VInsertIdentifier" select="@insert" />
              <VW_Compound_Identifier>
                <xsl:choose>
                  <xsl:when test="$VDeleteIdentifier=''yes''">
                    <xsl:attribute name="delete">
                      <xsl:value-of select="@delete"/>
                    </xsl:attribute>
                  </xsl:when>
                  <xsl:when test="$VInsertIdentifier=''yes'' or $VInsertComponent=''yes''">
                    <xsl:attribute name="insert">
                      <xsl:value-of select="@insert"/>
                    </xsl:attribute>
                  </xsl:when>
                </xsl:choose>
                <xsl:element name="ROW">
                  <xsl:element name="ID">
                    <xsl:value-of select="ID" />
                  </xsl:element>
                  <xsl:if test="IdentifierID/@update=''yes'' or $VInsertComponent=''yes'' or $VInsertIdentifier=''yes''">
                    <xsl:element name="TYPE">
                      <xsl:value-of select="IdentifierID" />
                    </xsl:element>
                  </xsl:if>
                  <xsl:if test="InputText/@update=''yes'' or $VInsertComponent=''yes'' or $VInsertIdentifier=''yes''">
                    <xsl:element name="VALUE">
                      <xsl:value-of select="InputText" />
                    </xsl:element>
                  </xsl:if>
                  <xsl:if test="$VCompound/RegNumber/RegID/@update=''yes'' or $VInsertComponent=''yes'' or $VInsertIdentifier=''yes''">
                    <xsl:element name="REGID">
                      <xsl:value-of select="$VCompound/RegNumber/RegID" />
                    </xsl:element>
                  </xsl:if>
                </xsl:element>
              </VW_Compound_Identifier>
            </xsl:for-each>
          </xsl:if>
          <!-- MIXTURE component linking information -->
          <VW_Mixture_Component>
            <xsl:if test="$VInsertComponent=''yes''">
              <xsl:attribute name="insert">yes</xsl:attribute>
            </xsl:if>
            <xsl:element name="ROW">
              <xsl:element name="MIXTURECOMPONENTID">0</xsl:element>
              <xsl:element name="MIXTUREID">0</xsl:element>
              <xsl:element name="COMPOUNDID">0</xsl:element>
            </xsl:element>
          </VW_Mixture_Component>
          <!-- BATCH components -->
          <xsl:if test="$VDeleteComponent!=''yes'' or string-length($VDeleteComponent)=0">
            <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[((ComponentIndex=$VComponentIndex) and ((@delete!=''yes'') or (string-length(@delete)=0))) ]">
              <VW_BatchComponent>
                <xsl:if test="$VInsertComponent=''yes''">
                  <xsl:attribute name="insert">yes</xsl:attribute>
                </xsl:if>
                <xsl:element name="ROW">
                  <xsl:element name="ID">
                    <xsl:value-of select="ID"/>
                  </xsl:element>
                  <xsl:if test="CompoundID/@update=''yes'' or $VInsertComponent=''yes''">
                    <xsl:element name="MIXTURECOMPONENTID">
                      <xsl:value-of select="CompoundID"/>
                    </xsl:element>
                  </xsl:if>
                  <xsl:if test="BatchID/@update=''yes'' or $VInsertComponent=''yes''">
                    <xsl:element name="BATCHID">
                      <xsl:value-of select="BatchID"/>
                    </xsl:element>
                  </xsl:if>
                  <xsl:if test="OrderIndex/@update=''yes'' or $VInsertComponent=''yes''">
                    <xsl:element name="ORDERINDEX">
                      <xsl:value-of select="OrderIndex"/>
                    </xsl:element>
                  </xsl:if>
                  <xsl:for-each select="PropertyList/Property[@update=''yes'' or @insert=''yes'']">
                    <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                      <xsl:value-of select="normalize-space(.)"/>
                    </xsl:element>
                  </xsl:for-each>
                </xsl:element>
              </VW_BatchComponent>
            </xsl:for-each>
            <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[((ComponentIndex=$VComponentIndex) and ((@delete!=''yes'') or (string-length(@delete)=0))) ]/BatchComponentFragmentList/BatchComponentFragment">
              <VW_BatchComponentFragment>
                <xsl:choose>
                  <xsl:when test="Equivalents/@update=''yes''">
                    <xsl:attribute name="update">yes</xsl:attribute>
                  </xsl:when>
                  <xsl:when test="$VInsertComponent=''yes''">
                    <xsl:attribute name="insert">yes</xsl:attribute>
                  </xsl:when>
                </xsl:choose>
                <xsl:element name="ROW">
                  <xsl:if test="$VInsertComponent=''yes''">
                    <xsl:attribute name="FragmentID">
                      <xsl:value-of select="FragmentID" />
                    </xsl:attribute>
                    <xsl:attribute name="CompoundID">
                      <xsl:value-of select="../../CompoundID" />
                    </xsl:attribute>
                  </xsl:if>
                  <!-- only need the ID for deletes and updates -->
                  <xsl:if test="Equivalents/@update=''yes'' or $VInsertComponent=''yes''">
                    <xsl:element name="ID">
                      <xsl:value-of select="ID" />
                    </xsl:element>
                  </xsl:if>
                  <xsl:if test="$VInsertComponent=''yes''">
                    <xsl:element name="BATCHCOMPONENTID">
                      <xsl:value-of select="../../ID"/>
                    </xsl:element>
                    <xsl:element name="COMPOUNDFRAGMENTID">0</xsl:element>
                  </xsl:if>
                  <xsl:if test="Equivalents/@update=''yes'' or $VInsertComponent=''yes''">
                    <xsl:element name="EQUIVALENT">
                      <xsl:value-of select="Equivalents"/>
                    </xsl:element>
                  </xsl:if>
                  <xsl:if test="OrderIndex/@update=''yes'' or $VInsertComponent=''yes''">
                    <xsl:element name="ORDERINDEX">
                      <xsl:value-of select="OrderIndex"/>
                    </xsl:element>
                  </xsl:if>
                </xsl:element>
              </VW_BatchComponentFragment>
            </xsl:for-each>
          </xsl:if>
        </xsl:for-each>
      </xsl:for-each>
      <!-- BATCH list -->
      <xsl:for-each select="BatchList/Batch">
        <xsl:variable name="VBatch" select="." />
        <xsl:variable name="VInsertBatch" select="@insert" />
        <xsl:variable name="VUpdateTable">
          <!-- TODO: there''s no sense to this...why use @insert=''yes'' in both cases? -->
          <xsl:if test="BatchID!=0 and @insert=''yes''">
            <xsl:value-of select="''yes''" />
          </xsl:if>
        </xsl:variable>
        <!-- BATCH data -->
        <VW_Batch>
          <xsl:if test="$VInsertBatch=''yes''">
            <xsl:attribute name="insert">yes</xsl:attribute>
          </xsl:if>
          <xsl:element name="ROW">
            <xsl:element name="BATCHID">
              <xsl:value-of select="BatchID" />
            </xsl:element>
            <xsl:if test="BatchNumber/@update=''yes'' or $VInsertBatch=''yes''">
              <xsl:element name="BATCHNUMBER">
                <xsl:value-of select="BatchNumber" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="FullRegNumber/@update=''yes'' or $VInsertBatch=''yes''">
              <xsl:element name="FULLREGNUMBER">
                <xsl:choose>
                  <xsl:when test="FullRegNumber!=''''">
                    <xsl:value-of select="FullRegNumber" />
                  </xsl:when>
                  <xsl:otherwise>null</xsl:otherwise>
                </xsl:choose>
              </xsl:element>
            </xsl:if>
            <!-- TODO: unravel THIS tongue-twister of a conditional.
            Is the intention to force an update if the fragments have been changed becuase of the ''salt suffix'' concept?
            <xsl:if test="(string-length(BatchComponentList/BatchComponent/BatchComponentFragmentList/BatchComponentFragment[@delete=''yes''])>0 or string-length(/MultiCompoundRegistryRecord/ComponentList/Component/Compound/FragmentList/Fragment/FragmentID[@update=''yes''])>0) and string-length(FullRegNumber[@update=''yes'' or $VInsertBatch=''yes''])=0">
              <xsl:element name="FULLREGNUMBER">
                <xsl:value-of select="FullRegNumber" />
              </xsl:element>
              <xsl:element name="BATCHNUMBER">
                <xsl:value-of select="BatchNumber" />
              </xsl:element>
            </xsl:if>
            -->
            <xsl:if test="DateCreated/@update=''yes'' or $VInsertBatch=''yes''">
              <xsl:element name="DATECREATED">
                <xsl:value-of select="DateCreated" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="PersonCreated/@update=''yes'' or $VInsertBatch=''yes''">
              <xsl:element name="PERSONCREATED">
                <xsl:value-of select="PersonCreated" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="PersonApproved/@update=''yes'' or $VInsertBatch=''yes''">
              <xsl:element name="PERSONAPPROVED">
                <xsl:value-of select="PersonApproved" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="PersonRegistered/@update=''yes'' or $VInsertBatch=''yes''">
              <xsl:element name="PERSONREGISTERED">
                <xsl:value-of select="PersonRegistered" />
              </xsl:element>
            </xsl:if>
            <xsl:element name="DATELASTMODIFIED">
              <xsl:value-of select="DateLastModified" />
            </xsl:element>
            <xsl:if test="StatusID/@update=''yes'' or $VInsertBatch=''yes''">
              <xsl:element name="STATUSID">
                <xsl:value-of select="StatusID" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="$VInsertBatch=''yes'' or $VUpdateTable=''yes''">
              <xsl:element name="REGID">
                <xsl:value-of select="$VMixtureRegID" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="TempBatchID/@update=''yes'' or $VInsertBatch=''yes''">
              <xsl:element name="TEMPBATCHID">
                <xsl:value-of select="TempBatchID" />
              </xsl:element>
            </xsl:if>
            <xsl:for-each select="PropertyList/Property[@update=''yes'' or @insert=''yes'' or $VInsertBatch=''yes'']">
              <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                <xsl:value-of select="normalize-space(.)"/>
              </xsl:element>
            </xsl:for-each>
          </xsl:element>
        </VW_Batch>
        <!-- TODO: re-factor -->
        <xsl:choose>
          <xsl:when test="string-length($VUpdateTable)>0">
            <xsl:for-each select="$VBatch/BatchComponentList/BatchComponent">
              <VW_BatchComponent>
                <xsl:element name="ROW">
                  <xsl:element name="ID">0</xsl:element>
                  <xsl:element name="BATCHID">
                    <xsl:value-of select="$VBatch/BatchID" />
                  </xsl:element>
                  <xsl:element name="MIXTURECOMPONENTID">
                    <xsl:value-of select="MixtureComponentID" />
                  </xsl:element>
                  <xsl:for-each select="PropertyList/Property[@update=''yes'' or @insert=''yes'']">
                    <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                      <xsl:value-of select="normalize-space(.)"/>
                    </xsl:element>
                  </xsl:for-each>
                </xsl:element>
              </VW_BatchComponent>
            </xsl:for-each>
          </xsl:when>
          <xsl:when test="$VInsertBatch=''yes''">
            <xsl:for-each select="BatchComponentList/BatchComponent[(@insert=''yes'')]">
              <VW_BatchComponent>
                <xsl:attribute name="insert">yes</xsl:attribute>
                <xsl:element name="ROW">
                  <xsl:element name="ID">0</xsl:element>
                  <xsl:element name="MIXTURECOMPONENTID">
                    <xsl:value-of select="MixtureComponentID" />
                  </xsl:element>
                  <xsl:element name="COMPOUNDID">
                    <xsl:value-of select="CompoundID" />
                  </xsl:element>
                  <xsl:element name="BATCHID">0</xsl:element>
                  <xsl:element name="ORDERINDEX">
                    <xsl:value-of select="OrderIndex" />
                  </xsl:element>
                  <xsl:for-each select="PropertyList/Property[@update=''yes'' or @insert=''yes'' or $VInsertBatch=''yes'']">
                    <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                      <xsl:value-of select="normalize-space(.)"/>
                    </xsl:element>
                  </xsl:for-each>
                </xsl:element>
              </VW_BatchComponent>
            </xsl:for-each>
          </xsl:when>
        </xsl:choose>
        <!-- TODO: re-factor -->
        <xsl:for-each select="BatchComponentList/BatchComponent">
          <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment[(@insert=''yes'')]">
            <VW_BatchComponentFragment>
              <xsl:attribute name="insert">yes</xsl:attribute>
              <xsl:element name="ROW">
                <xsl:attribute name="FragmentID">
                  <xsl:value-of select="FragmentID" />
                </xsl:attribute>
                <xsl:attribute name="CompoundID">
                  <xsl:value-of select="../../CompoundID" />
                </xsl:attribute>
                <xsl:element name="BATCHCOMPONENTID">
                  <xsl:value-of select="../../ID" />
                </xsl:element>
                <xsl:element name="COMPOUNDFRAGMENTID">
                  <xsl:value-of select="ID" />
                </xsl:element>
                <xsl:element name="EQUIVALENT">
                  <xsl:value-of select="Equivalents" />
                </xsl:element>
                <xsl:element name="ORDERINDEX">
                  <xsl:value-of select="OrderIndex" />
                </xsl:element>
              </xsl:element>
            </VW_BatchComponentFragment>
          </xsl:for-each>
        </xsl:for-each>
        <!-- TODO: re-factor -->
        <xsl:for-each select="BatchComponentList/BatchComponent">
          <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment[(@delete=''yes'')]">
            <VW_BatchComponentFragment>
              <xsl:attribute name="delete">yes</xsl:attribute>
              <xsl:element name="ROW">
                <xsl:element name="ID">
                  <xsl:value-of select="ID"/>
                </xsl:element>
              </xsl:element>
            </VW_BatchComponentFragment>
          </xsl:for-each>
        </xsl:for-each>
        <xsl:for-each select="BatchComponentList/BatchComponent[(@insert=''yes'') and (BatchComponentFragmentList/BatchComponentFragment/@insert!=''yes'' or string-length(BatchComponentFragmentList/BatchComponentFragment/@insert)=0)]">
          <xsl:variable name="VComponentIndex1" select="ComponentIndex" />
          <xsl:choose>
            <xsl:when test="/MultiCompoundRegistryRecord/ComponentList/Component[@insert!=''yes'' or string-length(@insert)=0]/ComponentIndex=$VComponentIndex1">
              <xsl:for-each select="BatchComponentFragmentList/BatchComponentFragment">
                <VW_BatchComponentFragment>
                  <xsl:attribute name="insert">yes</xsl:attribute>
                  <xsl:element name="ROW">
                    <xsl:attribute name="FragmentID">
                      <xsl:value-of select="FragmentID" />
                    </xsl:attribute>
                    <xsl:attribute name="CompoundID">
                      <xsl:value-of select="../../CompoundID" />
                    </xsl:attribute>
                    <xsl:element name="BATCHCOMPONENTID">
                      <xsl:value-of select="../../ID" />
                    </xsl:element>
                    <xsl:element name="COMPOUNDFRAGMENTID">
                      <xsl:value-of select="ID" />
                    </xsl:element>
                    <xsl:element name="EQUIVALENT">
                      <xsl:value-of select="Equivalents" />
                    </xsl:element>
                    <xsl:element name="ORDERINDEX">
                      <xsl:value-of select="OrderIndex" />
                    </xsl:element>
                  </xsl:element>
                </VW_BatchComponentFragment>
              </xsl:for-each>
            </xsl:when>
          </xsl:choose>
        </xsl:for-each>
        <!-- BATCH projects -->
        <xsl:for-each select="ProjectList/Project">
          <xsl:variable name="VDeleteProject" select="@delete" />
          <xsl:variable name="VInsertProject" select="@insert" />
          <VW_Batch_Project>
            <xsl:choose>
              <xsl:when test="$VDeleteProject=''yes''">
                <xsl:attribute name="delete">
                  <xsl:value-of select="@delete"/>
                </xsl:attribute>
              </xsl:when>
              <xsl:when test="$VInsertProject=''yes'' or $VInsertBatch=''yes''">
                <xsl:attribute name="insert">yes</xsl:attribute>
              </xsl:when>
            </xsl:choose>
            <xsl:element name="ROW">
              <xsl:element name="ID">
                <xsl:value-of select="ID" />
              </xsl:element>
              <xsl:if test="ProjectID/@update=''yes'' or $VInsertBatch=''yes'' or $VInsertProject=''yes''">
                <xsl:element name="PROJECTID">
                  <xsl:value-of select="ProjectID" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="$VInsertProject=''yes'' or $VInsertBatch=''yes''">
                <xsl:element name="BATCHID">
                  <xsl:value-of select="$VBatch/BatchID" />
                </xsl:element>
              </xsl:if>
            </xsl:element>
          </VW_Batch_Project>
        </xsl:for-each>
        <!-- BATCH identifiers -->
        <xsl:for-each select="IdentifierList/Identifier">
          <xsl:variable name="VDeleteIdentifier" select="@delete" />
          <xsl:variable name="VInsertIdentifier" select="@insert" />
          <VW_BatchIdentifier>
            <xsl:choose>
              <xsl:when test="$VDeleteIdentifier=''yes''">
                <xsl:attribute name="delete">
                  <xsl:value-of select="@delete"/>
                </xsl:attribute>
              </xsl:when>
              <xsl:when test="$VInsertIdentifier=''yes'' or $VInsertBatch=''yes''">
                <xsl:attribute name="insert">yes</xsl:attribute>
              </xsl:when>
            </xsl:choose>
            <xsl:element name="ROW">
              <xsl:element name="ID">
                <xsl:value-of select="ID" />
              </xsl:element>
              <xsl:if test="IdentifierID/@update=''yes'' or $VInsertBatch=''yes'' or $VInsertIdentifier=''yes''">
                <xsl:element name="TYPE">
                  <xsl:value-of select="IdentifierID" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="InputText/@update=''yes'' or $VInsertBatch=''yes'' or $VInsertIdentifier=''yes''">
                <xsl:element name="VALUE">
                  <xsl:value-of select="InputText" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="$VInsertIdentifier=''yes'' or $VInsertBatch=''yes''">
                <xsl:element name="BATCHID">
                  <xsl:value-of select="$VBatch/BatchID" />
                </xsl:element>
              </xsl:if>
            </xsl:element>
          </VW_BatchIdentifier>
        </xsl:for-each>
      </xsl:for-each>
    </MultiCompoundRegistryRecord>
  </xsl:template>
</xsl:stylesheet>
');
    end if;

    TraceWrite( act||'XslMcrrUpdate_ended', $$plsql_line,'end' );
    RETURN VXslUpdateMCRR;
  end;

  FUNCTION XslMcrrTempFetch RETURN XMLTYPE IS
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'XslMcrrTempFetch_started', $$plsql_line,'start' );
    if (vXslRetrieveMcrrTemp is null) then
      vXslRetrieveMcrrTemp:= XmlType.CreateXml('
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="sbiFlag" select="defaultSBI"/>
  <xsl:param name="rlsFlag" select="defaultRLS"/>
  <xsl:param name="isEditableFlag" select="defaultIsEditable"/>
  <xsl:template match="/MIXTURE">
    <xsl:element name="MultiCompoundRegistryRecord">
      <xsl:attribute name="SameBatchesIdentity">
        <xsl:value-of select="$sbiFlag"/>
      </xsl:attribute>
      <xsl:attribute name="ActiveRLS">
        <xsl:value-of select="$rlsFlag"/>
      </xsl:attribute>
      <xsl:attribute name="IsEditable">
        <xsl:value-of select="$isEditableFlag"/>
      </xsl:attribute>
      <xsl:element name="ID">
        <xsl:value-of select="TEMPBATCHID"/>
      </xsl:element>
      <xsl:element name="StatusID">
        <xsl:value-of select="STATUSID"/>
      </xsl:element>
      <xsl:element name="DateCreated">
        <xsl:value-of select="DATECREATED"/>
      </xsl:element>
      <xsl:element name="DateLastModified">
        <xsl:value-of select="DATELASTMODIFIED"/>
      </xsl:element>
      <xsl:element name="SubmissionComments">
        <xsl:value-of select="SUBMISSIONCOMMENTS"/>
      </xsl:element>
      <xsl:element name="PersonCreated">
        <xsl:value-of select="PERSONCREATED"/>
      </xsl:element>
       <xsl:element name="PersonApproved">
        <xsl:value-of select="PERSONAPPROVED"/>
      </xsl:element>
      <xsl:element name="StructureAggregation">
        <xsl:value-of select="STRUCTUREAGGREGATION"/>
      </xsl:element>
      <xsl:element name="RegNumber">
        <xsl:element name="RegID">0</xsl:element>
        <xsl:element name="SequenceNumber"/>
        <xsl:element name="RegNumber"/>
        <xsl:element name="SequenceID">
          <xsl:value-of select="SEQUENCEID"/>
        </xsl:element>
      </xsl:element>
      <xsl:copy-of select="REGIDENTIFIERS/IdentifierList"/>
      <xsl:copy-of select="REGPROPLIST/PropertyList"/>
      <xsl:element name="ProjectList">
        <xsl:for-each select="REGPROJECTS/REGPROJECTS_ROW">
          <xsl:element name="Project">
            <xsl:element name="ID">
              <xsl:value-of select="ID"/>
            </xsl:element>
            <xsl:element name="ProjectID">
              <xsl:attribute name="Description">
                <xsl:value-of select="DESCRIPTION"/>
              </xsl:attribute>
              <xsl:attribute name="Name">
                <xsl:value-of select="NAME"/>
              </xsl:attribute>
              <xsl:attribute name="Active">
                <xsl:value-of select="ACTIVE"/>
              </xsl:attribute>
              <xsl:value-of select="PROJECTID"/>
            </xsl:element>
          </xsl:element>
        </xsl:for-each>
      </xsl:element>
      <!-- ComponentList -->
      <xsl:element name="ComponentList">
        <xsl:for-each select="COMPONENTS/COMPONENTS_ROW">
          <xsl:element name="Component">
            <xsl:element name="ID"/>
            <xsl:element name="ComponentIndex">
              <xsl:value-of select="concat(''-'', TEMPCOMPOUNDID)"/>
            </xsl:element>
            <xsl:element name="Compound">
              <xsl:element name="CompoundID">
                <xsl:value-of select="TEMPCOMPOUNDID"/>
              </xsl:element>
              <xsl:element name="DateCreated">
                <xsl:value-of select="DATECREATED"/>
              </xsl:element>
              <xsl:element name="PersonCreated">
                <xsl:value-of select="PERSONCREATED"/>
              </xsl:element>
              <xsl:element name="PersonApproved">
                <xsl:value-of select="PERSONAPPROVED"/>
              </xsl:element>
              <xsl:element name="DateLastModified">
                <xsl:value-of select="DATELASTMODIFIED"/>
              </xsl:element>
              <xsl:element name="Tag"/>
              <xsl:copy-of select="COMPPROPLIST/PropertyList"/>
              <xsl:element name="RegNumber">
                <xsl:element name="RegID">
                  <xsl:value-of select="REGID"/>
                </xsl:element>
                <xsl:element name="RegNumber">
                  <xsl:value-of select="REGNUMBER"/>
                </xsl:element>
                <xsl:element name="SequenceID">
                  <xsl:value-of select="SEQUENCEID"/>
                </xsl:element>
                <xsl:element name="SequenceNumber"/>
              </xsl:element>
              <xsl:element name="BaseFragment">
                <xsl:element name="Structure">
                  <xsl:element name="StructureID">
                    <xsl:value-of select="STRUCTUREID"/>
                  </xsl:element>
                  <xsl:element name="StructureFormat"/>
                  <xsl:element name="Structure">
                    <xsl:attribute name="molWeight">
                      <xsl:value-of select="FORMULAWEIGHT"/>
                    </xsl:attribute>
                    <xsl:attribute name="formula">
                      <xsl:value-of select="MOLECULARFORMULA"/>
                    </xsl:attribute>
                    <xsl:value-of select="STRUCTURE"/>
                  </xsl:element>
                  <xsl:element name="DrawingType">
                    <xsl:value-of select="DRAWINGTYPE"/>
                  </xsl:element>
                  <xsl:element name="NormalizedStructure">
                    <xsl:value-of select="NORMALIZEDSTRUCTURE"/>
                  </xsl:element>
                  <xsl:element name="UseNormalization">
                    <xsl:value-of select="USENORMALIZATION"/>
                  </xsl:element>
                  <xsl:copy-of select="STRUCTPROPLIST/PropertyList"/>
                  <xsl:copy-of select="STRUCTIDENTIFIERXML/IdentifierList"/>
                </xsl:element>
              </xsl:element>
              <xsl:copy-of select="FRAGMENTXML/FragmentList"/>
              <xsl:copy-of select="IDENTIFIERXML/IdentifierList"/>
            </xsl:element>
          </xsl:element>
        </xsl:for-each>
      </xsl:element>
      <!-- BatchList -->
      <xsl:element name="BatchList">
        <xsl:element name="Batch">
          <xsl:element name="BatchID">
            <xsl:value-of select="TEMPBATCHID"/>
          </xsl:element>
          <xsl:element name="BatchNumber">
            <xsl:value-of select="BATCHNUMBER"/>
          </xsl:element>
          <xsl:element name="DateCreated">
            <xsl:value-of select="DATECREATED"/>
          </xsl:element>
          <xsl:element name="PersonCreated">
            <xsl:value-of select="PERSONCREATED"/>
          </xsl:element>
          <xsl:element name="PersonApproved">
            <xsl:value-of select="PERSONAPPROVED"/>
          </xsl:element>
          <xsl:element name="DateLastModified">
            <xsl:value-of select="DATELASTMODIFIED"/>
          </xsl:element>
          <xsl:element name="StatusID">
            <xsl:value-of select="STATUSID"/>
          </xsl:element>
          <xsl:element name="ProjectList">
            <xsl:for-each select="BATCHPROJECTS/BATCHPROJECTS_ROW">
              <xsl:element name="Project">
                <xsl:element name="ID">
                  <xsl:value-of select="ID"/>
                </xsl:element>
                <xsl:element name="ProjectID">
                  <xsl:attribute name="Description">
                    <xsl:value-of select="DESCRIPTION"/>
                  </xsl:attribute>
                  <xsl:attribute name="Name">
                    <xsl:value-of select="NAME"/>
                  </xsl:attribute>
                  <xsl:attribute name="Active">
                    <xsl:value-of select="ACTIVE"/>
                  </xsl:attribute>
                  <xsl:value-of select="PROJECTID"/>
                </xsl:element>
              </xsl:element>
            </xsl:for-each>
          </xsl:element>
          <xsl:copy-of select="BATCHIDENTIFIERS/IdentifierList"/>
          <xsl:copy-of select="BATCHPROPLIST/PropertyList"/>
          <xsl:element name="BatchComponentList">
            <xsl:for-each select="COMPONENTS/COMPONENTS_ROW">
              <xsl:element name="BatchComponent">
                <xsl:element name="ID"/>
                <xsl:element name="BatchID">
                  <xsl:value-of select="TEMPBATCHID"/>
                </xsl:element>
                <xsl:element name="CompoundID">
                  <xsl:value-of select="TEMPCOMPOUNDID"/>
                </xsl:element>
                <xsl:element name="ComponentIndex">
                  <xsl:value-of select="concat(''-'', TEMPCOMPOUNDID)"/>
                </xsl:element>
                <xsl:element name="ComponentRegNum">
                  <xsl:value-of select="REGNUMBER"/>
                </xsl:element>
                <xsl:copy-of select="BATCHCOMPPROPLIST/PropertyList"/>
                <xsl:copy-of select="BATCHCOMPFRAGMENTXML/BatchComponentFragmentList"/>
              </xsl:element>
            </xsl:for-each>
          </xsl:element>
        </xsl:element>
      </xsl:element>
    </xsl:element>
  </xsl:template>
</xsl:stylesheet>
');
    end if;

    TraceWrite( act||'XslMcrrTempFetch_ended', $$plsql_line,'end' );
    RETURN vXslRetrieveMcrrTemp;
  END;

  /** XSL transform used to create a 'temporary' registry record
  -->author jed
  -->since January 2011
  -->return an XSLT for creating 'temporary' records
  */
  FUNCTION XslMcrrTempCreate RETURN XMLTYPE IS
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'XslMcrrTempCreate_started', $$plsql_line,'start' );
    if (VXslCreateMCRRTemp is null) then
VXslCreateMCRRTemp := XmlType.CreateXml('
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:variable name="Vlower" select="''abcdefghijklmnopqrstuvwxyz''"/>
  <xsl:variable name="VUPPER" select="''ABCDEFGHIJKLMNOPQRSTUVWXYZ''"/>
  <xsl:template match="/MultiCompoundRegistryRecord">
    <xsl:variable name="VProjectList" select="ProjectList"/>
    <xsl:variable name="VProjectBatchList" select="BatchList/Batch/ProjectList"/>
    <xsl:variable name="VIdentifierList" select="IdentifierList"/>
    <xsl:variable name="VIdentifierBatchList" select="BatchList/Batch/IdentifierList"/>
    <xsl:variable name="VPropertyList" select="PropertyList"/>
    <xsl:variable name="VBatchList_Batch" select="BatchList/Batch"/>

    <xsl:for-each select="BatchList/Batch">
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
          <PERSONAPPROVED>
            <xsl:value-of select="PersonApproved"/>
          </PERSONAPPROVED>
          <SUBMISSIONCOMMENTS>
            <xsl:value-of select="/MultiCompoundRegistryRecord/SubmissionComments"/>
          </SUBMISSIONCOMMENTS>
          <DATELASTMODIFIED>
            <xsl:value-of select="DateLastModified"/>
          </DATELASTMODIFIED>
          <SEQUENCEID>
            <xsl:value-of select="/MultiCompoundRegistryRecord/RegNumber/SequenceID"/>
          </SEQUENCEID>
          <STATUSID>
            <xsl:value-of select="StatusID"/>
          </STATUSID>
          <STRUCTUREAGGREGATION>
            <xsl:copy-of select="StructureAggregation"/>
          </STRUCTUREAGGREGATION>
          <xsl:for-each select="PropertyList/Property">
            <!-- get the element value -->
            <xsl:variable name="eValue" select="."/>
            <!-- get the element name -->
            <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
            <!-- conditionally create the element -->
            <xsl:choose>
              <xsl:when test="$eName = ''DELIVERYDATE'' and string-length($eValue) != 0">
                <xsl:element name="{$eName}">
                  <xsl:value-of select="$eValue"/>
                </xsl:element>
              </xsl:when>
              <xsl:when test="$eName = ''DATEENTERED'' and string-length($eValue) != 0">
                <xsl:element name="{$eName}">
                  <xsl:value-of select="$eValue"/>
                </xsl:element>
              </xsl:when>
              <xsl:otherwise>
                <xsl:element name="{$eName}">
                  <xsl:value-of select="$eValue"/>
                </xsl:element>
              </xsl:otherwise>
            </xsl:choose>
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
          <IDENTIFIERXMLBATCH>
            <XMLFIELD>
              <xsl:copy-of select="$VIdentifierBatchList"/>
            </XMLFIELD>
          </IDENTIFIERXMLBATCH>
          <xsl:for-each select="$VPropertyList/Property">
            <!-- get the element value -->
            <xsl:variable name="eValue" select="."/>
            <!-- get the element name -->
            <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
            <!-- create the element -->
            <xsl:element name="{$eName}">
              <xsl:value-of select="$eValue"/>
            </xsl:element>
          </xsl:for-each>
        </ROW>
      </VW_TemporaryBatch>

      <xsl:for-each select="ProjectList/Project">
        <VW_TemporaryBatchProject>
          <ROW>
            <TEMPBATCHID>0</TEMPBATCHID>
            <PROJECTID>
              <xsl:value-of select="ProjectID"/>
            </PROJECTID>
          </ROW>
        </VW_TemporaryBatchProject>
      </xsl:for-each>

    </xsl:for-each>
    <xsl:for-each select="ComponentList/Component">
      <xsl:variable name="VComponentIndex" select="ComponentIndex/."/>
      <xsl:for-each select="Compound">
        <VW_TemporaryCompound>
          <ROW>
            <TEMPCOMPOUNDID>0</TEMPCOMPOUNDID>
            <TEMPBATCHID>0</TEMPBATCHID>
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
              <DRAWINGTYPE>
                <xsl:value-of select="DrawingType"/>
              </DRAWINGTYPE>
              <STRUCTUREID>
                <xsl:value-of select="StructureID"/>
              </STRUCTUREID>
              <xsl:for-each select="PropertyList/Property">
                <!-- get the element value -->
                <xsl:variable name="eValue" select="."/>
                <!-- get the element name -->
                <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
                <!-- create the element -->
                <xsl:element name="{$eName}">
                  <xsl:value-of select="$eValue"/>
                </xsl:element>
              </xsl:for-each>
              <STRUCTIDENTIFIERXML>
                <XMLFIELD>
                  <xsl:copy-of select="IdentifierList"/>
                </XMLFIELD>
              </STRUCTIDENTIFIERXML>
            </xsl:for-each>
            <DATECREATED>
              <xsl:value-of select="DateCreated"/>
            </DATECREATED>
            <PERSONCREATED>
              <xsl:value-of select="PersonCreated"/>
            </PERSONCREATED>
            <PERSONAPPROVED>
              <xsl:value-of select="PersonApproved"/>
            </PERSONAPPROVED>
            <DATELASTMODIFIED>
              <xsl:value-of select="DateLastModified"/>
            </DATELASTMODIFIED>
            <SEQUENCEID>
              <xsl:value-of select="RegNumber/SequenceID"/>
            </SEQUENCEID>
            <TAG>
              <xsl:value-of select="Tag"/>
            </TAG>
            <xsl:for-each select="PropertyList/Property">
              <!-- get the element value -->
              <xsl:variable name="eValue" select="."/>
              <!-- get the element name -->
              <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
              <!-- create the element -->
              <xsl:element name="{$eName}">
                <xsl:value-of select="$eValue"/>
              </xsl:element>
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
                <!-- get the element value -->
                <xsl:variable name="eValue" select="."/>
                <!-- get the element name -->
                <xsl:variable name="eName" select="translate(@name, $Vlower, $VUPPER)"/>
                <!-- conditionally create the element -->
                <xsl:choose>
                  <xsl:when test="$eName = ''COMMENTS''">
                    <xsl:element name="BATCHCOMPONENTCOMMENTS">
                      <xsl:value-of select="$eValue"/>
                    </xsl:element>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:element name="{$eName}">
                      <xsl:value-of select="$eValue"/>
                    </xsl:element>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:for-each>
            </xsl:for-each>
          </ROW>
        </VW_TemporaryCompound>
      </xsl:for-each>
    </xsl:for-each>

    <xsl:for-each select="ProjectList/Project">
      <VW_TemporaryRegNumbersProject>
        <ROW>
          <TEMPBATCHID>0</TEMPBATCHID>
          <PROJECTID>
            <xsl:value-of select="ProjectID"/>
          </PROJECTID>
        </ROW>
      </VW_TemporaryRegNumbersProject>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>'
);
    end if;

    TraceWrite( act||'XslMcrrTempCreate_ended', $$plsql_line,'end' );
    RETURN VXslCreateMCRRTemp;
  END;

  FUNCTION XslMcrrTempUpdate RETURN XMLTYPE IS
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'XslMcrrTempUpdate_started', $$plsql_line,'start' );
    if (VXslUpdateMCRRTemp is null) then

VXslUpdateMCRRTemp := XmlType.CreateXml('
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>
  <xsl:variable name="Vlower" select="''abcdefghijklmnopqrstuvwxyz''"/>
  <xsl:variable name="VUPPER" select="''ABCDEFGHIJKLMNOPQRSTUVWXYZ''"/>
  <xsl:template match="/MultiCompoundRegistryRecord">
    <MultiCompoundRegistryRecord>
      <xsl:variable name="VMixture" select="."/>
      <!-- Mixture projects -->
      <!-- TODO: handle inserts and updates for this table -->

      <xsl:for-each select="ProjectList/Project">
        <xsl:variable name="VDeleteProject" select="@delete" />
        <xsl:variable name="VInsertProject" select="@insert" />
        <VW_TemporaryRegNumbersProject>
          <xsl:choose>
            <xsl:when test="$VDeleteProject=''yes''">
              <xsl:attribute name="delete">
                <xsl:value-of select="@delete"/>
              </xsl:attribute>
            </xsl:when>
            <xsl:when test="$VInsertProject=''yes''">
              <xsl:attribute name="insert">
                <xsl:value-of select="@insert"/>
              </xsl:attribute>
            </xsl:when>
          </xsl:choose>
          <xsl:element name="ROW">
            <xsl:element name="ID">
              <xsl:value-of select="ID" />
            </xsl:element>
            <xsl:if test="$VInsertProject=''yes''">
              <xsl:element name="TEMPBATCHID">
                <xsl:value-of select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchID" />
              </xsl:element>
            </xsl:if>
            <xsl:if test="ProjectID/@update=''yes'' or $VInsertProject=''yes''">
              <xsl:element name="PROJECTID">
                <xsl:value-of select="ProjectID" />
              </xsl:element>
            </xsl:if>
          </xsl:element>
        </VW_TemporaryRegNumbersProject>
      </xsl:for-each>

      <xsl:for-each select="BatchList">
        <xsl:for-each select="Batch">
          <!-- Batch projects -->
          <xsl:for-each select="ProjectList/Project">
            <xsl:variable name="VDeleteProject" select="@delete" />
            <xsl:variable name="VInsertProject" select="@insert" />
            <VW_TemporaryBatchProject>
              <xsl:choose>
                <xsl:when test="$VDeleteProject=''yes''">
                  <xsl:attribute name="delete">
                    <xsl:value-of select="@delete"/>
                  </xsl:attribute>
                </xsl:when>
                <xsl:when test="$VInsertProject=''yes''">
                  <xsl:attribute name="insert">
                    <xsl:value-of select="@insert"/>
                  </xsl:attribute>
                </xsl:when>
              </xsl:choose>
              <xsl:element name="ROW">
                <xsl:element name="ID">
                  <xsl:value-of select="ID" />
                </xsl:element>
                <xsl:if test="$VInsertProject=''yes''">
                  <xsl:element name="TEMPBATCHID">
                    <xsl:value-of select="../../BatchID" />
                  </xsl:element>
                </xsl:if>
                <xsl:if test="ProjectID/@update=''yes'' or $VInsertProject=''yes''">
                  <xsl:element name="PROJECTID">
                    <xsl:value-of select="ProjectID" />
                  </xsl:element>
                </xsl:if>
              </xsl:element>
            </VW_TemporaryBatchProject>
          </xsl:for-each>
          <VW_TemporaryBatch>
            <xsl:element name="ROW">
              <xsl:element name="TEMPBATCHID">
                <xsl:value-of select="BatchID" />
              </xsl:element>
              <xsl:if test="BatchNumber/@update=''yes''">
                <xsl:element name="BATCHNUMBER">
                  <xsl:value-of select="BatchNumber" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="DateCreated/@update=''yes''">
                <xsl:element name="DATECREATED">
                  <xsl:value-of select="DateCreated" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="PersonCreated/@update=''yes''">
                <xsl:element name="PERSONCREATED">
                  <xsl:value-of select="PersonCreated" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="PersonApproved/@update=''yes''">
                <xsl:element name="PERSONAPPROVED">
                  <xsl:value-of select="PersonApproved" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="$VMixture/SubmissionComments/@update=''yes''">
                <xsl:element name="SUBMISSIONCOMMENTS">
                  <xsl:value-of select="$VMixture/SubmissionComments" />
                </xsl:element>
              </xsl:if>
              <xsl:element name="DATELASTMODIFIED">
                <xsl:value-of select="DateLastModified" />
              </xsl:element>
              <xsl:if test="StructureAggregation/@update=''yes''">
                <xsl:element name="STRUCTUREAGGREGATION">
                  <xsl:value-of select="StructureAggregation" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="StatusID/@update=''yes''">
                <xsl:element name="STATUSID">
                  <xsl:value-of select="StatusID" />
                </xsl:element>
              </xsl:if>
              <!-- Batch PropertyList -->
              <xsl:for-each select="PropertyList/Property[@update=''yes'' or @insert=''yes'']">
                <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                  <xsl:value-of select="normalize-space(.)"/>
                </xsl:element>
              </xsl:for-each>
              <!-- Mixture PropertyList -->
              <xsl:for-each select="$VMixture/PropertyList/Property[@update=''yes'' or @insert=''yes'']">
                <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                  <xsl:value-of select="normalize-space(.)"/>
                </xsl:element>
              </xsl:for-each>
              <!-- Mixture IdentifierList -->
              <xsl:if test="count($VMixture/IdentifierList/Identifier[@*]) > 0">
                <xsl:element name="IDENTIFIERXML">
                  <xsl:element name="XMLFIELD">
                    <xsl:element name="IdentifierList">
                      <xsl:for-each select="$VMixture/IdentifierList/Identifier[not(@delete)]">
                        <xsl:element name="Identifier">
                          <xsl:copy-of select="node()"/>
                        </xsl:element>
                      </xsl:for-each>
                    </xsl:element>
                  </xsl:element>
                </xsl:element>
              </xsl:if>
              <!-- Batch IdentifierList -->
              <xsl:if test="count(IdentifierList/Identifier[@*]) > 0">
                <xsl:element name="IDENTIFIERXMLBATCH">
                  <xsl:element name="XMLFIELD">
                    <xsl:element name="IdentifierList">
                      <xsl:for-each select="IdentifierList/Identifier[not(@delete)]">
                        <xsl:element name="Identifier">
                          <xsl:copy-of select="node()"/>
                        </xsl:element>
                      </xsl:for-each>
                    </xsl:element>
                  </xsl:element>
                </xsl:element>
              </xsl:if>
            </xsl:element>
          </VW_TemporaryBatch>
        </xsl:for-each>
      </xsl:for-each>

      <xsl:for-each select="ComponentList/Component">
        <xsl:variable name="VDelete" select="@delete"/>
        <xsl:variable name="VInsert" select="@insert"/>
        <xsl:variable name="VComponentIndex" select="ComponentIndex/."/>
        <xsl:for-each select="Compound">
          <VW_TemporaryCompound>
            <xsl:if test="$VDelete=''yes''">
              <xsl:attribute name="delete">yes</xsl:attribute>
            </xsl:if>
            <xsl:if test="$VInsert=''yes''">
              <xsl:attribute name="insert">yes</xsl:attribute>
            </xsl:if>
            <xsl:element name="ROW">
              <xsl:for-each select="RegNumber/RegID[$VInsert=''yes'']">
                <xsl:element name="REGID">
                  <xsl:if test=".!=''0''">
                    <xsl:value-of select="."/>
                  </xsl:if>
                </xsl:element>
              </xsl:for-each>
              <xsl:element name="TEMPCOMPOUNDID">
                <xsl:value-of select="CompoundID"/>
              </xsl:element>
              <xsl:if test="CompoundID/@insert=''yes'' or $VInsert=''yes''">
                <xsl:element name="TEMPBATCHID">
                  <xsl:value-of select="CompoundID"/>
                </xsl:element>
              </xsl:if>
              <!-- ? is this the correct logic ? -->
              <xsl:if test="BaseFragment/Structure/Structure/@update=''yes'' or $VInsert=''yes''">
                <xsl:element name="BASE64_CDX">
                  <xsl:value-of select="BaseFragment/Structure/Structure"/>
                </xsl:element>
              </xsl:if>
              <xsl:if test="BaseFragment/Structure/DrawingType/@update=''yes'' or $VInsert=''yes'' or @insert=''yes''">
                <xsl:element name="DRAWINGTYPE">
                  <xsl:value-of select="BaseFragment/Structure/DrawingType" />
                </xsl:element>
              </xsl:if>
              <!-- ? is this the correct logic ? -->
              <xsl:if test="BaseFragment/Structure/@update=''yes'' or $VInsert=''yes''">
                <xsl:element name="STRUCTUREID">
                  <xsl:value-of select="BaseFragment/Structure/StructureID"/>
                </xsl:element>
              </xsl:if>
              <xsl:if test="DateCreated/@update=''yes'' or $VInsert=''yes''">
                <xsl:element name="DATECREATED">
                  <xsl:value-of select="DateCreated"/>
                </xsl:element>
              </xsl:if>
              <xsl:if test="PersonCreated/@update=''yes'' or $VInsert=''yes''">
                <xsl:element name="PERSONCREATED">
                  <xsl:value-of select="PersonCreated"/>
                </xsl:element>
              </xsl:if>
               <xsl:if test="PersonApproved/@update=''yes'' or $VInsert=''yes''">
                <xsl:element name="PERSONAPPROVED">
                  <xsl:value-of select="PersonApproved"/>
                </xsl:element>
              </xsl:if>
              <xsl:if test="DateLastModified/@update=''yes'' or $VInsert=''yes''">
                <xsl:element name="DATELASTMODIFIED">
                  <xsl:value-of select="DateLastModified"/>
                </xsl:element>
              </xsl:if>
              <xsl:if test="Tag/@update=''yes'' or $VInsert=''yes''">
                <xsl:element name="TAG">
                  <xsl:value-of select="Tag"/>
                </xsl:element>
              </xsl:if>
              <!-- Compound PropertyList -->
              <xsl:for-each select="PropertyList/Property[@update=''yes'' or $VInsert=''yes'']">
                <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                  <xsl:value-of select="normalize-space(.)"/>
                </xsl:element>
              </xsl:for-each>
              <!-- Structure PropertyList -->
              <xsl:for-each select="BaseFragment/Structure/PropertyList/Property[@update=''yes'' or $VInsert=''yes'']">
                <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                  <xsl:value-of select="normalize-space(.)"/>
                </xsl:element>
              </xsl:for-each>
              <!-- Compound fragment list -->
              <xsl:if test="count(FragmentList/Fragment[@*]) > 0 or $VInsert=''yes''">
                <xsl:element name="FRAGMENTXML">
                  <xsl:element name="XMLFIELD">
                    <xsl:element name="FragmentList">
                      <xsl:for-each select="FragmentList/Fragment[not(@delete)]">
                        <xsl:copy-of select="node()"/>
                      </xsl:for-each>
                    </xsl:element>
                  </xsl:element>
                </xsl:element>
              </xsl:if>
              <!-- BatchComponent fragment list -->
              <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent/BatchComponentFragmentList[@update=''yes'' or $VInsert=''yes'']">
                <xsl:element name="BATCHCOMPFRAGMENTXML">
                  <xsl:element name="XMLFIELD">
                    <xsl:copy-of select="." />
                  </xsl:element>
                </xsl:element>
              </xsl:for-each>
              <!-- Compound identifiers -->
              <xsl:if test="count(IdentifierList/Identifier[@*]) > 0 or $VInsert=''yes''">
                <xsl:element name="IDENTIFIERXML">
                  <xsl:element name="XMLFIELD">
                    <xsl:element name="IdentifierList">
                      <xsl:for-each select="IdentifierList/Identifier[not(@delete)]">
                        <xsl:element name="Identifier">
                          <xsl:copy-of select="node()"/>
                        </xsl:element>
                      </xsl:for-each>
                    </xsl:element>
                  </xsl:element>
                </xsl:element>
              </xsl:if>
              <!-- Structure identifiers : strip all attributes -->
              <xsl:if test="count(BaseFragment/Structure/IdentifierList/Identifier[@*]) > 0 or $VInsert=''yes''">
                <xsl:element name="STRUCTIDENTIFIERXML">
                  <xsl:element name="XMLFIELD">
                    <xsl:element name="IdentifierList">
                      <xsl:for-each select="BaseFragment/Structure/IdentifierList/Identifier[not(@delete)]">
                        <xsl:element name="Identifier">
                          <xsl:copy-of select="node()"/>
                        </xsl:element>
                      </xsl:for-each>
                    </xsl:element>
                  </xsl:element>
                </xsl:element>
              </xsl:if>
              <xsl:if test="BaseFragment/Structure/NormalizedStructure/@update=''yes'' or $VInsert=''yes''">
                <xsl:element name="NORMALIZEDSTRUCTURE">
                  <xsl:value-of select="BaseFragment/Structure/NormalizedStructure" />
                </xsl:element>
              </xsl:if>
              <xsl:if test="BaseFragment/Structure/UseNormalization/@update=''yes'' or $VInsert=''yes''">
                <xsl:element name="USENORMALIZATION">
                  <xsl:value-of select="BaseFragment/Structure/UseNormalization" />
                </xsl:element>
              </xsl:if>
              <!-- BatchComponent property list -->
              <xsl:for-each select="/MultiCompoundRegistryRecord/BatchList/Batch/BatchComponentList/BatchComponent[ComponentIndex=$VComponentIndex]">
                <xsl:for-each select="PropertyList/Property[@update=''yes'' or $VInsert=''yes'']">
                  <xsl:element name="{translate(@name, $Vlower, $VUPPER)}" xml:space="default">
                    <xsl:value-of select="normalize-space(.)"/>
                  </xsl:element>
                </xsl:for-each>
              </xsl:for-each>
            </xsl:element>
          </VW_TemporaryCompound>
        </xsl:for-each>
      </xsl:for-each>
    </MultiCompoundRegistryRecord>
  </xsl:template>
</xsl:stylesheet>
');
    end if;

    TraceWrite( act||'XslMcrrTempUpdate_ended', $$plsql_line,'end' );
    RETURN VXslUpdateMCRRTemp;
  END;

 PROCEDURE UpdateApprovedStatus(ATempID NUMBER, AStatusID IN NUMBER) IS
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'UpdateApprovedStatus_started', $$plsql_line,'start' );
    IF(AStatusID = cSubmittedStatus OR AStatusID =cApprovedStatus) THEN
        UPDATE VW_TEMPORARYBATCH SET STATUSID=AStatusID WHERE TEMPBATCHID=ATempID;
    ELSIF (AStatusID = cRegisteredStatus OR AStatusID = cLockedStatus) THEN
        UPDATE VW_MIXTURE SET STATUSID=AStatusID WHERE REGID=ATempID;
    END IF;
    TraceWrite( act||'UpdateApprovedStatus_ended', $$plsql_line,'end' );
  END;

  PROCEDURE SetApprovePerson(ATempID NUMBER, AStatusID IN NUMBER, APersonID IN NUMBER) IS
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'SetApprovePerson_started', $$plsql_line,'start' );
    IF(AStatusID = cApprovedStatus) THEN
      UPDATE VW_TEMPORARYBATCH SET PERSONAPPROVED=APersonID WHERE TEMPBATCHID=ATempID;
      UPDATE VW_TEMPORARYCOMPOUND SET PERSONAPPROVED=APersonID WHERE TEMPBATCHID=ATempID;
    ELSE
          UPDATE VW_TEMPORARYCOMPOUND SET PERSONAPPROVED=null WHERE TEMPBATCHID=ATempID;
        UPDATE VW_TEMPORARYBATCH SET PERSONAPPROVED=null WHERE TEMPBATCHID=ATempID;
    END IF;
    TraceWrite( act||'SetApprovePerson_ended', $$plsql_line,'end' );
  END;

PROCEDURE UpdateLockedStatus(APermIDList VARCHAR2, AStatusID IN NUMBER) IS
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'UpdateLockedStatus_started', $$plsql_line,'start' );
    execute immediate  'UPDATE VW_MIXTURE SET STATUSID=' || AStatusID ||' WHERE REGID IN (' || APermIDList ||')';
        INSERT INTO LOG  (LogProcedure,LogComment,run_seq)
           VALUES
        ('UpdateLockedStatus',AStatusID ||'  ' || APermIDList,log_run_seq);
    TraceWrite( act||'UpdateLockedStatus_ended', $$plsql_line,'end' );
  END;

PROCEDURE  GetCompoundLockStatus(ACompoundid NUMBER,AStatusID OUT NOCOPY NUMBER) IS
      mod1 varchar2(100); act varchar2(100);
    BEGIN
      dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetCompoundLockStatus_started', $$plsql_line,'start' );
      select to_char(statusid) into AStatusID  from mixtures where mix_internal_id =(
         select mixtrureid from mixture_component  A where compoundid = ACompoundid and mixtrureid in (
          select mixtrureid from mixture_component  group by mixtrureid having count(mixtrureid)<2)
       );
        --AStatusID := '4';
        TraceWrite( act||'GetCompoundLockStatus_ended', $$plsql_line,'end' );
    EXCEPTION
      WHEN OTHERS THEN NULL; --  no row selected exception can be thrown
        AStatusID :=0;
        TraceWrite( act||'_ended', $$plsql_line,'end' );
 END;

PROCEDURE GetLockedRegsiteryList (vRegNumbers IN varchar2 , O_RS OUT CURSOR_TYPE)
  IS
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetLockedRegsiteryList_started', $$plsql_line,'start' );
   OPEN O_RS for  'select mx.statusid , reg.reg_number from mixtures mx,reg_numbers reg where mx.regid = reg.reg_id and
                reg.reg_number in  ('|| vRegNumbers ||') and mx.statusid =4';
    TraceWrite( act||'GetLockedRegsiteryList_ended', $$plsql_line,'end' );
  END;

  FUNCTION CanRegister(ATempID in VW_TemporaryBatch.tempbatchid%type) RETURN VARCHAR2 IS
    vResult varchar2(200);
    mod1 varchar2(100); act varchar2(100);
    BEGIN
        dbms_application_info.read_module(mod1, act); TraceWrite( act||'CanRegister_started', $$plsql_line,'start' );
        IF(GetApprovalsEnabled() = 'True') THEN
            SELECT decode(statusid, cSubmittedStatus, 'False', cApprovedStatus, 'True', 'False') into vResult from VW_TemporaryBatch where tempbatchid = ATempID;
        ELSE
            vResult := 'True';
        END IF;
        TraceWrite( act||'CanRegister_ended', $$plsql_line,'end' );
        return vResult;
    END;

FUNCTION CanDeleteTemp(ATempID in VW_TemporaryBatch.tempbatchid%type) RETURN VARCHAR2 IS
    vResult varchar2(200);
      mod1 varchar2(100); act varchar2(100);
    BEGIN
       dbms_application_info.read_module(mod1, act); TraceWrite( act||'CanDeleteTemp_started', $$plsql_line,'start' );
        IF(GetApprovalsEnabled() = 'True') THEN
            SELECT decode(statusid, 2, 'False', 1, 'True', 'False') into vResult from VW_TemporaryBatch where tempbatchid = ATempID;
        ELSE
            vResult := 'True';
        END IF;
        TraceWrite( act||'CanDeleteTemp_ended', $$plsql_line,'end' );
        return vResult;
    END;

PROCEDURE SaveDuplicates(AXMLDuplicated IN clob,APersonID IN VARCHAR2:=null) IS
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'SaveDuplicates_started', $$plsql_line,'start' );
     SaveRegNumbersDuplicated(SYS.XMLTYPE(AXMLDuplicated),APersonID);
    TraceWrite( act||'SaveDuplicates_ended', $$plsql_line,'end' );
  END;
/**
  Utility for providing a value for a Registration configuration setting
  -->author frg
  -->since August 2011
  -->return True if enabled
  */
  FUNCTION GetApprovalsEnabled RETURN VARCHAR2 IS
    LValue CLOB;
    LSettings XMLTYPE;
    mod1 varchar2(100); act varchar2(100);
  BEGIN
    dbms_application_info.read_module(mod1, act); TraceWrite( act||'GetApprovalsEnabled_started', $$plsql_line,'start' );
    IF (vApprovalsEnabled is NULL) THEN
      LSettings := VSystemSettings;
      SELECT Extract(
        LSettings,
        'Registration/applicationSettings/groups/add[@name="REGADMIN"]/settings/add[@name="ApprovalsEnabled"]/@value'
      ).GetClobVal() INTO LValue
      FROM DUAl;
      vApprovalsEnabled := to_char(LValue);
    END IF;
    TraceWrite( act||'GetApprovalsEnabled_ended', $$plsql_line,'end' );
    RETURN vApprovalsEnabled;
  END;

begin
 VSystemSettings := GetSystemSettings;
 vActiveRLS:=GetActiveRLS;
 VRegObjectTemplate:=GetRegObjectTemplate;
 LEnableRLS := RegistrationRLS.GEnableRLS;
 VSameBatchesIdentity:=GetSameBatchesIdentity;
 select run_for_log_seq.nextval into log_run_seq from dual;
END;
/
show errors
