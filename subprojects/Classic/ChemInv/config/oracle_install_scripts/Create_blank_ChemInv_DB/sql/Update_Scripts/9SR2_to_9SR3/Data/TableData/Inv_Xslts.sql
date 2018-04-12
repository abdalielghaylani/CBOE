
DECLARE
l_clob clob;
BEGIN

l_clob := '<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<PLATES>
			<xsl:for-each select="/MERGEDOC/REFORMAT_MAP/TARGET_PLATE">
				<PLATE>
					<xsl:if test="boolean(./@LOCATION_ID_FK) = false">
						<xsl:attribute name="LOCATION_ID_FK">1000</xsl:attribute>
					</xsl:if>
					<xsl:if test="boolean(./@PLATE_TYPE_ID_FK) = false">
						<xsl:attribute name="PLATE_TYPE_ID_FK">5</xsl:attribute>
					</xsl:if>
					<xsl:if test="boolean(./@STATUS_ID_FK) = false">
						<xsl:attribute name="STATUS_ID_FK">5</xsl:attribute>
					</xsl:if>
					<xsl:call-template name="CopyAttributes">
						<xsl:with-param name="dataNode" select="."/>
						<xsl:with-param name="exceptionList" select="''ROWS COLS PLATENUM''"/>
					</xsl:call-template>
					<xsl:for-each select="./ROW">
						<xsl:call-template name="CreatePlateRow">
							<xsl:with-param name="dataNode" select="."/>
						</xsl:call-template>
					</xsl:for-each>
				</PLATE>
			</xsl:for-each>
		</PLATES>
	</xsl:template>
	<xsl:template name="CreatePlateRow">
		<xsl:param name="dataNode"/>
		<ROW>
			<xsl:call-template name="CopyAttributes">
				<xsl:with-param name="dataNode" select="$dataNode"/>
				<xsl:with-param name="exceptionList"/>
			</xsl:call-template>
			<xsl:for-each select="$dataNode/COL">
				<xsl:call-template name="CreatePlateCol">
					<xsl:with-param name="dataNode" select="."/>
				</xsl:call-template>
			</xsl:for-each>
		</ROW>
	</xsl:template>
	<xsl:template name="CreatePlateCol">
		<xsl:param name="dataNode"/>
		<COL>
			<xsl:call-template name="CopyAttributes">
				<xsl:with-param name="dataNode" select="$dataNode"/>
				<xsl:with-param name="exceptionList"/>
			</xsl:call-template>
			<xsl:for-each select="$dataNode/SOURCE">
				<xsl:call-template name="CreateWell">
					<xsl:with-param name="dataNode" select="."/>
				</xsl:call-template>
			</xsl:for-each>
		</COL>
	</xsl:template>
	<xsl:template name="CreateWell">
		<xsl:param name="dataNode"/>
		<xsl:variable name="plateNum">
			<xsl:value-of select="$dataNode/@PLATENUM"/>
		</xsl:variable>
		<xsl:variable name="rowID">
			<xsl:value-of select="$dataNode/@ROWID"/>
		</xsl:variable>
		<xsl:variable name="colID">
			<xsl:value-of select="$dataNode/@COLID"/>
		</xsl:variable>
		<xsl:variable name="plateIDFK">
			<xsl:value-of select="/MERGEDOC/REFORMAT_MAP/SOURCE_PLATE[@PLATENUM=$plateNum]/@PLATE_ID_FK"/>
		</xsl:variable>
		<xsl:if test="count(/MERGEDOC/ROWSET/ROW[child::PLATE_ID_FK=$plateIDFK and child::ROW_INDEX=$rowID and child::COL_INDEX=$colID]) = 0">
			<xsl:variable name="sourceWellNode" select="/MERGEDOC/ROWSET/ROW[child::PLATE_ID_FK=$plateIDFK and child::ROW_INDEX=$rowID and child::COL_INDEX=$colID]"/>
			<WELL>
				<xsl:for-each select="$sourceWellNode/*[contains(''ROW_INDEX COL_INDEX'',local-name())=false]">
					<xsl:attribute name="{name(.)}"><xsl:value-of select="."/></xsl:attribute>
				</xsl:for-each>
				<xsl:attribute name="GRID_POSITION_ID_FK"><xsl:value-of select="$dataNode/ancestor::COL/@GRID_POSITION_ID_FK"/></xsl:attribute>
			</WELL>
		</xsl:if>
		<xsl:if test="count(/MERGEDOC/ROWSET/ROW[child::PLATE_ID_FK=$plateIDFK and child::ROW_INDEX=$rowID and child::COL_INDEX=$colID]) ' || chr(38) || 'gt; 0">
			<xsl:for-each select="/MERGEDOC/ROWSET/ROW[child::PLATE_ID_FK=$plateIDFK and child::ROW_INDEX=$rowID and child::COL_INDEX=$colID]">
				<WELL>
					<xsl:for-each select="./*[contains(''ROW_INDEX COL_INDEX'',local-name())=false]">
						<xsl:attribute name="{name(.)}"><xsl:value-of select="."/></xsl:attribute>
					</xsl:for-each>
					<xsl:attribute name="GRID_POSITION_ID_FK"><xsl:value-of select="$dataNode/ancestor::COL/@GRID_POSITION_ID_FK"/></xsl:attribute>
				</WELL>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
	<xsl:template name="FilterFields">
		<xsl:param name="currFieldNode"/>
		<xsl:param name="updateList"/>
		<xsl:variable name="currFieldName" select="name($currFieldNode)"/>
		<xsl:variable name="currFieldValue" select="$currFieldNode"/>
		<xsl:choose>
			<xsl:when test="string-length($updateList) = 0">
				<xsl:choose>
					<xsl:when test="contains(''PLATE_ID_FK,ROW_INDEX,COL_INDEX'',$currFieldName)">
						<xsl:value-of select="$updateList"/>
					</xsl:when>
					<xsl:when test="contains(''WELL_ID'',$currFieldName)">
						<xsl:value-of select="concat(''PARENT_WELL_ID_FK='',$currFieldValue)"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="concat($currFieldName,''='',$currFieldValue)"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="contains(''PLATE_ID_FK,ROW_INDEX,COL_INDEX'',$currFieldName)">
						<xsl:value-of select="$updateList"/>
					</xsl:when>
					<xsl:when test="contains(''WELL_ID'',$currFieldName)">
						<xsl:value-of select="concat($updateList,'','',''PARENT_WELL_ID_FK='',$currFieldValue)"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="concat($updateList,'','',$currFieldName,''='',$currFieldValue)"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="FilterFieldValue">
		<xsl:param name="currFieldName"/>
		<xsl:param name="newColumnList"/>
		<xsl:param name="dataNode"/>
		<xsl:param name="valueList"/>
		<xsl:param name="position"/>
		<xsl:choose>
			<xsl:when test="contains($newColumnList,$currFieldName)">
				<xsl:choose>
					<xsl:when test="$position = 1">
						<xsl:value-of select="$dataNode/*[$position]"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="concat($valueList,'','',$dataNode/*[$position])"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$valueList"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="CopyAttributes">
		<xsl:param name="dataNode"/>
		<xsl:param name="exceptionList"/>
		<xsl:for-each select="$dataNode/@*[contains($exceptionList,local-name())=false]">
			<xsl:attribute name="{name(.)}"><xsl:value-of select="."/></xsl:attribute>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>
';
Insert into inv_xslts  values (1, l_clob, 'Create Target Plates');

l_clob := '<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<ROWSET>
			<xsl:for-each select="/PLATES/PLATE">
				<ROW>
					<xsl:call-template name="Attributes2Elements">
						<xsl:with-param name="dataNode" select="."/>
					</xsl:call-template>
				</ROW>
			</xsl:for-each>
		</ROWSET>
	</xsl:template>
	<xsl:template name="Attributes2Elements">
		<xsl:param name="dataNode"/>
		<xsl:for-each select="$dataNode/@*">
			<xsl:element name="{name(.)}">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>
';
Insert into inv_xslts  values (2, l_clob, 'Create Plates');

l_clob := '<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<ROWSET>
			<!-- non-mixture wells -->
			<xsl:for-each select="//COL[count(WELL)=1]/WELL">
				<ROW>
					<xsl:call-template name="Attributes2Elements">
						<xsl:with-param name="dataNode" select="."/>
					</xsl:call-template>
				</ROW>
			</xsl:for-each>
			<!-- only create one well to represent a mixture well -->
			<xsl:for-each select="//COL[count(WELL)>1]/WELL[1]">
				<ROW>
					<xsl:call-template name="MixtureAttributes2Elements">
						<xsl:with-param name="dataNode" select="."/>
					</xsl:call-template>
				</ROW>
			</xsl:for-each>
		</ROWSET>
	</xsl:template>
	<xsl:template name="Attributes2Elements">
		<xsl:param name="dataNode"/>
		<xsl:if test="boolean($dataNode/@COMPOUND_ID_FK) = false">
			<xsl:element name="COMPOUND_ID_FK"></xsl:element>
		</xsl:if>
		<xsl:for-each select="$dataNode/@*"> 
			<xsl:element name="{name(.)}">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:for-each>
<!--
		<xsl:for-each select="$dataNode/@*[contains(''PLATE_ID_FK'',local-name()) = false]"> 
			<xsl:element name="{name(.)}">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:for-each>
		<xsl:element name="PLATE_ID_FK">
			<xsl:value-of select="$dataNode/ancestor::PLATE/@PLATE_ID"/>
		</xsl:element>
-->	
	</xsl:template>
	<xsl:template name="MixtureAttributes2Elements">
		<xsl:param name="dataNode"/>
		<xsl:for-each select="$dataNode/@*[contains(''PARENT_WELL_ID_FK'',local-name()) = false and contains(''COMPOUND_ID_FK'',local-name()) = false]">
			<xsl:element name="{name(.)}">
				<xsl:value-of select="."/>
			</xsl:element>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>
';
Insert into inv_xslts  values (3, l_clob, 'Create Wells');

l_clob := '<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<REFORMAT_MAP>
			<xsl:variable name="rowCount" select="/ROWSET/ROW[@num=''1'']/ROW_COUNT"/>
			<xsl:variable name="colCount" select="/ROWSET/ROW[@num=''1'']/COL_COUNT"/>
			<SOURCE_PLATE PLATENUM="1">
				<xsl:attribute name="ROWS"><xsl:value-of select="$rowCount"/></xsl:attribute>
				<xsl:attribute name="COLS"><xsl:value-of select="$colCount"/></xsl:attribute>
				<xsl:attribute name="PLATE_FORMAT_ID_FK"><xsl:value-of select="/ROWSET/ROW[@num=''1'']/PLATE_FORMAT_ID"/></xsl:attribute>
			</SOURCE_PLATE>
			<TARGET_PLATE PLATENUM="1">
				<xsl:attribute name="ROWS"><xsl:value-of select="$rowCount"/></xsl:attribute>
				<xsl:attribute name="COLS"><xsl:value-of select="$colCount"/></xsl:attribute>
				<xsl:attribute name="PLATE_FORMAT_ID_FK"><xsl:value-of select="/ROWSET/ROW[@num=''1'']/PLATE_FORMAT_ID"/></xsl:attribute>
				<xsl:call-template name="CreateRow">
					<xsl:with-param name="dataNode" select="/ROWSET/ROW[@num=''1'']"/>
					<xsl:with-param name="rowNum" select="1"/>
					<xsl:with-param name="rowsLeft" select="$rowCount"/>
					<xsl:with-param name="rowCount" select="$rowCount"/>
					<xsl:with-param name="colCount" select="$colCount"/>
				</xsl:call-template>
			</TARGET_PLATE>
		</REFORMAT_MAP>
	</xsl:template>
	<xsl:template name="CreateRow">
		<xsl:param name="dataNode"/>
		<xsl:param name="rowNum"/>
		<xsl:param name="rowsLeft"/>
		<xsl:param name="rowCount"/>
		<xsl:param name="colCount"/>
		<ROW>
			<xsl:attribute name="ID"><xsl:value-of select="$rowNum"/></xsl:attribute>
			<xsl:call-template name="CreateCol">
				<xsl:with-param name="dataNode" select="$dataNode"/>
				<xsl:with-param name="colNum" select="1"/>
				<xsl:with-param name="colsLeft" select="$colCount"/>
			</xsl:call-template>
		</ROW>
		<xsl:if test="($rowsLeft - 1) > 0">
			<xsl:call-template name="CreateRow">
				<xsl:with-param name="dataNode" select="$dataNode/following-sibling::*[position() = $colCount]"/>
				<xsl:with-param name="rowNum" select="$rowNum + 1"/>
				<xsl:with-param name="rowsLeft" select="$rowsLeft - 1"/>
				<xsl:with-param name="rowCount" select="$rowCount"/>
				<xsl:with-param name="colCount" select="$colCount"/>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<xsl:template name="CreateCol">
		<xsl:param name="dataNode"/>
		<xsl:param name="colNum"/>
		<xsl:param name="colsLeft"/>
		<COL>
			<xsl:attribute name="ID"><xsl:value-of select="$colNum"/></xsl:attribute>
			<xsl:attribute name="GRID_POSITION_ID_FK"><xsl:value-of select="$dataNode/GRID_POSITION_ID"/></xsl:attribute>
			<SOURCE PLATENUM="1">
				<xsl:attribute name="ROWID"><xsl:value-of select="$dataNode/ROW_INDEX"/></xsl:attribute>
				<xsl:attribute name="COLID"><xsl:value-of select="$dataNode/COL_INDEX"/></xsl:attribute>
			</SOURCE>
		</COL>
		<xsl:if test="($colsLeft -1) > 0">
			<xsl:call-template name="CreateCol">
				<xsl:with-param name="dataNode" select="$dataNode/following-sibling::*[1]"/>
				<xsl:with-param name="colNum" select="$colNum+1"/>
				<xsl:with-param name="colsLeft" select="$colsLeft - 1"/>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>
';
Insert into inv_xslts  values (4, l_clob, 'Create Daugtering Map');

commit;

END;

/

