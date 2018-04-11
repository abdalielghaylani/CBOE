CREATE OR REPLACE
PACKAGE BODY       BIOSAR_UTILS as

TYPE t_categories IS TABLE OF VARCHAR2(50);

FUNCTION splitCategories(pCategories in BIOSARDB.DB_TABLE.CATEGORY%Type)
RETURN t_categories
IS
	startpos number := 1;
	delimpos number := 0;
	vCategories t_categories := t_categories();
	category varchar2(50);
BEGIN
	delimpos := INSTR(pCategories, ',', startpos, 1);
	WHILE (delimpos > 0) LOOP
		category := trim(substr(pCategories, startpos, delimpos-startpos));
		if category is not null then
			vCategories.extend;
			vCategories(vCategories.count) := category;
		end if;
		startpos := delimpos + 1;
		delimpos := INSTR(pCategories, ',', startpos, 1);
	END LOOP;

	category := trim(substr(pCategories, startpos));
	if category is not null then
		vCategories.extend;
		vCategories(vCategories.count) := category;
	end if;
	return vCategories;
END splitCategories;

FUNCTION getTableXml(TableID in BIOSARDB.DB_TABLE.TABLE_ID%Type,
					 pCategoryNames in t_categories)
RETURN XmlType
IS
	vCategoryNames BIOSARDB.DB_TABLE.CATEGORY%Type;
	vTagTable t_categories;
	Tags XmlSequenceType := XmlSequenceType();
	TableXml XmlType;
BEGIN
	IF pCategoryNames IS NOT NULL THEN
		SELECT t.category INTO vCategoryNames
		FROM BIOSARDB.DB_TABLE t
		WHERE t.table_id = TableId;

		vTagTable := splitCategories(vCategoryNames) MULTISET INTERSECT DISTINCT pCategoryNames;
		IF vTagTable.Count > 0 THEN
			FOR i in vTagTable.first .. vTagTable.last
			LOOP
				Tags.extend;
				SELECT xmlElement("tag", vTagTable(i)) INTO Tags(Tags.Count) FROM dual;
			END LOOP;
		END IF;
	END IF;

	SELECT xmlElement("table",
			     xmlAttributes(t.table_id as "id",
						  	t.table_short_name as "name",
						  	t.display_name as "alias",
						  	t.owner as "database",
						  	DECODE(t.is_view, 'Y', '1', 'N', '0') as "isView",
						  	t.base_column_id as "primaryKey"
						  	),(select xmlagg(xmlElement("fields",
							  						xmlAttributes(f.column_id as "id",
								 								  f.column_name as "name",
								 								  f.display_name as "alias",
								 								  '1' as "visible",
								 								  f.lookup_column_id as "lookupFieldId",
								 								  f.lookup_column_display as "lookupDisplayFieldId",
								 								  DECODE(f.lookup_sort_direct, 'ASC', 'ASCENDING',
								 								  							   'DESC', 'DESCENDING') as "lookupSortOrder",
								 								  DECODE(f.index_type_id, 0, 'UNKNOWN',
								 								  						  1, 'NONE',
								 								  						  2, 'CS_CARTRIDGE',
								 								  						  3, 'NONE',
								 								  						  4, 'CS_CARTRIDGE',
								 								  						     'NONE') as "indexType",
													 			  DECODE(f.content_type_id, 0, 	'UNKNOWN',
																							1, 	'IMAGE_GIF',
																							2,	'IMAGE_JPEG',
																							3,	'IMAGE_PNG',
																							4,	'IMAGE_X_WMF',
																							5,	'CHEMICAL_X_CDX',
																							6,	'CHEMICAL_X_MDL_MOLFILE',
																							7,	'CHEMICAL_X_CDX',
																							8,	'CHEMICAL_X_SMILES',
																							9,	'TEXT_XML',
																							10,	'TEXT_XML',
																							11,	'TEXT_HTML',
																							12,	'TEXT_PLAIN',
																							13,	'TEXT_RAW',
																							14,	'APPLICATION_MS_EXCEL',
																							15,	'APPLICATION_MS_MSWORD',
																							16,	'APPLICATION_PDF',
																							  	'NONE') as "mimeType",
								 								  DECODE(f.datatype, 'VARCHAR2', 'TEXT',
								 								  					 'CHAR', DECODE(f.length, 1, 'BOOLEAN', 'TEXT'),
								 								  					 'CLOB', 'TEXT',
								 								  					 'NUMBER', DECODE(f.scale, 0, 'INTEGER', 'REAL'),
								 								  					 'DATE', 'DATE',
								 								  					 'TIMESTAMP(0)', 'DATE',
								 								  					 			     'TEXT' ) as "dataType",
								 								  DECODE(SUBSTR(f.description, 1, 9), '-default-', '1',
								 								                                                   '0') as "isDefault")))
									from BIOSARDB.DB_COLUMN f
									where f.table_id = t.table_id
									and datatype in ('VARCHAR2', 'CHAR', 'CLOB', 'NUMBER', 'DATE', 'TIMESTAMP(0)','BOOLEAN')
									--and is_visible = 'Y'
									), XmlElement("tags", XmlConcat(Tags)))
	INTO TableXml
	FROM BIOSARDB.DB_TABLE t
	WHERE t.table_id = TableID;
	RETURN TableXml;
END getTableXml;


PROCEDURE createOrUpdateDVFromBioSAR(
									 DataviewName in COEDataview.name%Type,
									 DataviewDescription in COEDataview.description%Type,
									 IsPublic in COEDataview.is_public%Type,
									 BaseTableID in INTEGER,
									 DatabaseName in COEDataView.DataBase%Type,
									 CategoryName in BIOSARDB.DB_TABLE.CATEGORY%Type,
									 IncludeTags in BIOSARDB.DB_TABLE.CATEGORY%Type) as

	vCategoryNames t_categories;
	vIncludeTags t_categories;
	vAllTags t_categories;
	DVTables XmlSequenceType := XmlSequenceType();
	DVRelationships XmlSequenceType := XmlSequenceType();
	COEDataView XmlType;
	DataViewId COEDB.COEDataview.id%Type;
	TableIds t_table_ids := t_table_ids();
BEGIN
	vCategoryNames := splitCategories(CategoryName);
	vIncludeTags := splitCategories(IncludeTags);
	vAllTags := vCategoryNames MULTISET UNION DISTINCT vIncludeTags;
	-- Get base table
	DVTables.extend;
	DVTables(DVTables.count) := getTableXml(BaseTableID, vAllTags);
	TableIds.extend;
	TableIds(TableIds.count) := BaseTableID;
	-- Get child tables
	for rec in (SELECT ct.table_id, ct.category,
					   r.table_id parent_table_id, r.child_table_id, r.column_id parentKey, r.child_column_id childKey, r.join_type
				FROM BIOSARDB.DB_TABLE bt, BIOSARDB.DB_TABLE ct, BIOSARDB.DB_RELATIONSHIP r
				where r.child_table_id = ct.table_id
				and r.table_id = bt.table_id
				and ct.is_exposed = 'Y'
				and r.table_id = BaseTableID
				and ct.owner in ('BIODM','REGDB','CS_SECURITY')) LOOP
		IF splitCategories(rec.category) MULTISET INTERSECT vCategoryNames IS NOT EMPTY THEN
			DVTables.extend;
			DVTables(DVTables.count) := getTableXml(rec.table_id, vAllTags);
			TableIDs.extend;
			TableIds(TableIds.count) := rec.table_id;
			DVRelationships.extend;
			SELECT xmlElement("relationship",
							  xmlAttributes(rec.parent_table_id as "parent",
											rec.child_table_id as "child",
											rec.parentKey as "parentkey",
											rec.childKey as "childkey",
											rec.join_type as "jointype"))
			INTO DVRelationships(DVRelationships.count)
			FROM dual;
		END IF;
	END LOOP;
	-- Get lookup tables
	for rec in (SELECT bt.table_id
				FROM BIOSARDB.DB_TABLE bt
				WHERE  bt.table_id IN (select distinct c.lookup_table_id from BIOSARDB.DB_COLUMN c
									   where c.lookup_table_id is not null
									   and ( table_id = BaseTableID OR table_id in (SELECT ct.table_id
																		FROM BIOSARDB.DB_TABLE bt, BIOSARDB.DB_TABLE ct, BIOSARDB.DB_RELATIONSHIP r,
																		     THE (select cast(TableIds as t_table_ids) from dual) i
																		where r.child_table_id = ct.table_id
																		and r.table_id = bt.table_id
																		and ct.is_exposed = 'Y'
																		and r.table_id = BaseTableID
																		and ct.owner in ('BIODM','REGDB','CS_SECURITY')
																		and ct.table_id = i.column_value
																		)))) LOOP
		DVTables.extend;
		DVTables(DVTables.count) := getTableXml(rec.table_id, null);
	END LOOP;

	DBMS_OUTPUT.PUT_LINE('Concatenating ' || DVTables.Count || ' tables...');
	SELECT xmlroot(xmlelement("COEDataView",
		xmlAttributes('COE.COEDataView' as "xmlns",
					  DataviewName as "name",
			   		  DataviewDescription as "description",
			   		  'USE_SERVER_DATAVIEW' as "dataviewHandling",
			   		  DataViewId as "dataviewid",
			   		  BaseTableID as "basetable",
			   		  DatabaseName as "database"),
		xmlelement("tables", xmlconcat(DVTables)),
		xmlelement("relationships", xmlconcat(DVRelationships))
		), version '1.0') into COEDataView from dual;
	DBMS_OUTPUT.PUT_LINE('Done.');

	Select COEDB.COEDATAVIEW_SEQ.NextVal into DataViewId from dual;
	INSERT INTO COEDB.COEDATAVIEW
	(id, name, description, user_id, is_public, date_created, coedataview, database)
	VALUES
	(DataViewId, DataViewName, DataViewDescription, 'COEDB', IsPublic, sysdate, COEDataView, DatabaseName);
	/*
	MERGE INTO COEDB.COEDATAVIEW dv
    USING
		(SELECT	 DataViewId id, DataviewName name, DatabaseName database, DataviewDescription description, 'COEDB' user_id, IsPublic is_public, sysdate date_created, COEDataView coedataview FROM DUAL) rec
	ON (dv.id = rec.id)
	WHEN MATCHED THEN
	   UPDATE set dv.name = rec.name, dv.description = rec.description, dv.is_public = rec.is_public, dv.coedataview = rec.coedataview
	WHEN NOT MATCHED THEN
       INSERT (dv.id, dv.name, dv.description, dv.user_id, dv.is_public, dv.date_created, dv.coedataview, dv.database)
       VALUES (rec.id, rec.name, rec.description, rec.user_id, rec.is_public, rec.date_created, rec.coedataview, rec.database);
 	*/
END createOrUpdateDVFromBioSAR;

PROCEDURE createOrUpdateMasterFromBioSAR as

	DataViewId COEDataview.id%Type := 0;
	DataviewName COEDataview.name%Type := 'Master Dataview';
	DataviewDescription COEDataview.description%Type := 'Master Dataview';
	IsPublic COEDataview.is_public%Type := 1;
	BaseTableID INTEGER := -1;
	DatabaseName COEDataView.DataBase%Type := 'COEDB';
BEGIN
	MERGE INTO COEDB.COEDATAVIEW dv
    USING
		(SELECT	 DataViewId id, DataviewName name, DatabaseName "database", DataviewDescription description, 'COEDB' user_id, IsPublic is_public, sysdate date_created, 'COEDB' database,
	 	      xmlRoot(
	 	      xmlElement("COEDataView",
			   xmlAttributes('COE.COEDataView' as "xmlns",
			   				 DataviewName as "name",
			   				 DataviewDescription as "description",
			   				 'USE_SERVER_DATAVIEW' as "dataviewHandling",
			   				 DataViewId as "dataviewid",
			   				 BaseTableID as "basetable",
			   				 DatabaseName as "database"), xmlElement("tables", (SELECT xmlagg(
			    xmlElement("table",
			     xmlAttributes(t.table_id as "id",
						  	t.table_short_name as "name",
						  	t.display_name as "alias",
						  	t.owner as "database",
						  	DECODE(t.is_view, 'Y', '1', 'N', '0') as "isView",
						  	t.base_column_id as "primaryKey"
						  	),(select xmlagg(xmlElement("fields",
							  						xmlAttributes(f.column_id as "id",
								 								  f.column_name as "name",
								 								  f.display_name as "alias",
								 								  '1' as "visible",
								 								  f.lookup_column_id as "lookupFieldId",
								 								  f.lookup_column_display as "lookupDisplayFieldId",
								 								  DECODE(f.lookup_sort_direct, 'ASC', 'ASCENDING',
								 								  							   'DESC', 'DESCENDING') as "lookupSortOrder",
								 								  DECODE(f.index_type_id, 0, 'UNKNOWN',
								 								  						  1, 'NONE',
								 								  						  2, 'CS_CARTRIDGE',
								 								  						  3, 'NONE',
								 								  						  4, 'CS_CARTRIDGE',
								 								  						     'NONE') as "indexType",
													 			  DECODE(f.content_type_id, 0, 	'UNKNOWN',
																							1, 	'IMAGE_GIF',
																							2,	'IMAGE_JPEG',
																							3,	'IMAGE_PNG',
																							4,	'IMAGE_X_WMF',
																							5,	'CHEMICAL_X_CDX',
																							6,	'CHEMICAL_X_MDL_MOLFILE',
																							7,	'CHEMICAL_X_CDX',
																							8,	'CHEMICAL_X_SMILES',
																							9,	'TEXT_XML',
																							10,	'TEXT_XML',
																							11,	'TEXT_HTML',
																							12,	'TEXT_PLAIN',
																							13,	'TEXT_RAW',
																							14,	'APPLICATION_MS_EXCEL',
																							15,	'APPLICATION_MS_MSWORD',
																							16,	'APPLICATION_PDF',
																							  	'NONE') as "mimeType",
								 								  DECODE(f.datatype, 'VARCHAR2', 'TEXT',
								 								  					 'CHAR', DECODE(f.length, 1, 'BOOLEAN', 'TEXT'),
								 								  					 'CLOB', 'TEXT',
								 								  					 'NUMBER', DECODE(f.scale, 0, 'INTEGER', 'REAL'),
								 								  					 'DATE', 'DATE',
								 								  					 'TIMESTAMP(0)', 'DATE',
								 								  					 			     'TEXT' ) as "dataType",
								 								  DECODE(SUBSTR(f.description, 1, 9), '-default-', '1',
								 								                                                   '0') as "isDefault")))
									from BIOSARDB.DB_COLUMN f
									where f.table_id = t.table_id
									and datatype in ('VARCHAR2', 'CHAR', 'CLOB', 'NUMBER', 'DATE', 'TIMESTAMP(0)','BOOLEAN')
									--and is_visible = 'Y'
									) /* close subselect*/
									) /* close <table>*/
									) /* close xmlAgg on <table> elements*/
									FROM (SELECT table_id, owner, table_short_name, display_name, base_column_id, description, is_exposed, is_view
		 								  FROM BIOSARDB.DB_TABLE
		 								  WHERE  is_exposed = 'Y') t)
									) /* close <tables>*/, xmlElement("relationships",
														    (select xmlagg(nvl2(r.parent_table_id, xmlElement("relationship",
															        						xmlAttributes(r.parent_table_id as "parent",
															        			  						  r.child_table_id as "child",
															        			  						  r.parentKey as "parentkey",
															        			  						  r.childKey as "childkey",
															        			  						  r.join_type as "jointype"))
															        			  		 ,null))
															 from 		(SELECT r.table_id parent_table_id, r.child_table_id, r.column_id parentKey, r.child_column_id childKey, r.join_type
																		 FROM BIOSARDB.DB_TABLE pt, BIOSARDB.DB_TABLE ct, BIOSARDB.DB_RELATIONSHIP r
																		 where r.child_table_id = ct.table_id
																		 and r.table_id = pt.table_id
																		 and ct.is_exposed = 'Y'
																		 and pt.is_exposed = 'Y') r)
									)), version '1.0') as COEDataview
		FROM dual) rec
	ON (dv.id = rec.id)
	WHEN MATCHED THEN
	   UPDATE set dv.name = rec.name, dv.description = rec.description, dv.is_public = rec.is_public, dv.coedataview = rec.coedataview
	WHEN NOT MATCHED THEN
       INSERT (dv.id, dv.name, dv.description, dv.user_id, dv.is_public, dv.date_created, dv.coedataview, dv.database)
       VALUES (rec.id, rec.name, rec.description, rec.user_id, rec.is_public, rec.date_created, rec.coedataview, rec.database);

END createOrUpdateMasterFromBioSAR;

END BIOSAR_UTILS;
/

