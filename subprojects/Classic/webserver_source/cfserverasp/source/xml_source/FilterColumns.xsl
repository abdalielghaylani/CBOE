<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:s="uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882" xmlns:dt="uuid:C2F41010-65B3-11d1-A29F-00AA00C14882" xmlns:rs="urn:schemas-microsoft-com:rowset" xmlns:z="#RowsetSchema">
	<xsl:template match="/">
		<TABLE_ELEMENT>
			<xsl:attribute name="SORT_URL"><xsl:value-of select="/MERGEDOC/DOCUMENT/ASP_VARIABLES/VARIABLE[@NAME = 'sortURL']"/></xsl:attribute>
			<xsl:attribute name="BORDER"><xsl:value-of select="/MERGEDOC/DOCUMENT/DISPLAY/@BORDER"/></xsl:attribute>
			<xsl:attribute name="CLASS"><xsl:value-of select="/MERGEDOC/DOCUMENT/DISPLAY/@CLASS"/></xsl:attribute>
			<xsl:attribute name="REPEAT_HEADER"><xsl:value-of select="/MERGEDOC/DOCUMENT/DISPLAY/@REPEAT_HEADER"/></xsl:attribute>
			<xsl:for-each select="/MERGEDOC/xml/rs:data/z:row">
				<DATAROW>
					<xsl:attribute name="PAGEROWNUM"><xsl:value-of select="./@PAGEROWNUM"/></xsl:attribute>
					<xsl:if test="count(./preceding-sibling::*) = 0">
						<xsl:attribute name="IS_FIRST">true</xsl:attribute>
					</xsl:if>
					<xsl:call-template name="Create_Data_Row">
						<xsl:with-param name="dataRow" select="."/>
						<xsl:with-param name="displayNode" select="/MERGEDOC/DOCUMENT/DISPLAY"/>
					</xsl:call-template>
				</DATAROW>
			</xsl:for-each>
		</TABLE_ELEMENT>
	</xsl:template>
	<xsl:template name="Create_Data_Row">
		<xsl:param name="dataRow"/>
		<xsl:param name="displayNode"/>
		<xsl:for-each select="$displayNode/*">
			<xsl:call-template name="Element_Builder">
				<xsl:with-param name="dataRow" select="$dataRow"/>
				<xsl:with-param name="displayNode" select="."/>
			</xsl:call-template>
		</xsl:for-each>
	</xsl:template>
	<xsl:template name="Element_Builder">
		<xsl:param name="dataRow"/>
		<xsl:param name="displayNode"/>
		<xsl:param name="bDoReplace"/>
		<xsl:variable name="elementName" select="local-name($displayNode)"/>
		<xsl:choose>
			<xsl:when test="$elementName = 'TABLE_ELEMENT'">
				<xsl:call-template name="Create_Table_Element">
					<xsl:with-param name="dataRow" select="$dataRow"/>
					<xsl:with-param name="displayNode" select="$displayNode"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$elementName = 'FIELD'">
				<xsl:call-template name="Create_Field_Element">
					<xsl:with-param name="dataRow" select="$dataRow"/>
					<xsl:with-param name="displayNode" select="$displayNode"/>
					<xsl:with-param name="bDoReplace" select="$bDoReplace"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$elementName = 'SUB_RS'">
				<xsl:call-template name="Process_Sub_RS">
					<xsl:with-param name="dataRow" select="$dataRow"/>
					<xsl:with-param name="displayNode" select="$displayNode"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$elementName = 'CAPTION_ELEMENT'">
		</xsl:when>
			<xsl:otherwise>
				<xsl:variable name="column" select="$displayNode/@COLUMN"/>
				<xsl:element name="INVALID_ELEMENT">
					<xsl:value-of select="$displayNode/@COLUMN"/>
					<xsl:value-of select="local-name($displayNode)"/>
				</xsl:element>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="Create_Table_Element">
		<xsl:param name="dataRow"/>
		<xsl:param name="displayNode"/>
		<xsl:if test="$displayNode/@SHOW = 1">
			<TABLE_ELEMENT>
				<xsl:attribute name="NAME"><xsl:value-of select="$displayNode/@NAME"/></xsl:attribute>
				<xsl:attribute name="COLUMNS"><xsl:value-of select="$displayNode/@COLUMNS"/></xsl:attribute>
				<xsl:attribute name="HEADER_NAME"><xsl:value-of select="$displayNode/@HEADER_NAME"/></xsl:attribute>
				<xsl:attribute name="HEADER_CLASS"><xsl:value-of select="$displayNode/@HEADER_CLASS"/></xsl:attribute>
				<xsl:attribute name="HEADER_TIP"><xsl:value-of select="$displayNode/@HEADER_TIP"/></xsl:attribute>
				<xsl:attribute name="BORDER"><xsl:value-of select="$displayNode/@BORDER"/></xsl:attribute>
				<xsl:attribute name="CLASS"><xsl:value-of select="$displayNode/@CLASS"/></xsl:attribute>
				<xsl:attribute name="WIDTH"><xsl:value-of select="$displayNode/@WIDTH"/></xsl:attribute>
				<xsl:attribute name="REPEAT_HEADER"><xsl:value-of select="$displayNode/@REPEAT_HEADER"/></xsl:attribute>
				<xsl:attribute name="COLSPAN"><xsl:value-of select="$displayNode/@COLSPAN"/></xsl:attribute>
				<xsl:if test="count($displayNode/CAPTION_ELEMENT) = 1">
					<xsl:if test="$displayNode/CAPTION_ELEMENT/@SHOW = 1">
						<CAPTION_ELEMENT>
							<xsl:attribute name="COLSPAN"><xsl:value-of select="$displayNode/CAPTION_ELEMENT/@COLSPAN"/></xsl:attribute>
							<xsl:value-of select="$displayNode/CAPTION_ELEMENT"/>
						</CAPTION_ELEMENT>
					</xsl:if>
				</xsl:if>
				<xsl:for-each select="$displayNode/*">
					<xsl:call-template name="Element_Builder">
						<xsl:with-param name="dataRow" select="$dataRow"/>
						<xsl:with-param name="displayNode" select="."/>
					</xsl:call-template>
				</xsl:for-each>
			</TABLE_ELEMENT>
		</xsl:if>
	</xsl:template>
	<xsl:template name="Create_Field_Element">
		<xsl:param name="dataRow"/>
		<xsl:param name="displayNode"/>
		<xsl:param name="bDoReplace"/>
		<xsl:if test="$displayNode/@SHOW = 1">
			<xsl:variable name="column" select="$displayNode/@COLUMN"/>
			<xsl:copy>
				<xsl:call-template name="Copy_Attributes">
					<xsl:with-param name="dataNode" select="$displayNode"/>
				</xsl:call-template>
				<xsl:if test="$displayNode/@IS_FIRST='1'">
					<xsl:attribute name="RSNAME"><xsl:choose><xsl:when test="count($displayNode/ancestor::SUB_RS) = 0">main</xsl:when><xsl:otherwise><xsl:value-of select="$displayNode/ancestor::SUB_RS[1]/@NAME"/></xsl:otherwise></xsl:choose></xsl:attribute>
				</xsl:if>
				<xsl:if test="count(../../*[local-name() = 'SUB_RS']) > 0">
					<xsl:attribute name="SORT_COLUMN"><xsl:value-of select="$displayNode/@VALUE_COLUMNS"/></xsl:attribute>
				</xsl:if>
				<xsl:attribute name="NUM"><xsl:value-of select="count($dataRow/following-sibling::*)"/></xsl:attribute>
				<xsl:choose>
					<xsl:when test="$displayNode/@IS_STRUCTURE = 1">
						<xsl:attribute name="PAGE_ROWNUM">
							<xsl:call-template name="SEARCH-AND-REPLACE">
								<xsl:with-param name="string" select="$dataRow/@PAGEROWNUM"/>
								<xsl:with-param name="search-for" select="'-'"/>
								<xsl:with-param name="replace-with"/>
							</xsl:call-template>
						</xsl:attribute>					
						<!--<xsl:choose>
							<xsl:when test="$displayNode/@DATA_TYPE = 'base64'">-->
								<xsl:call-template name="Replace_Values">
									<xsl:with-param name="dataRow" select="$dataRow"/>
									<xsl:with-param name="displayNode" select="$displayNode"/>
									<xsl:with-param name="valueNames" select="$displayNode/@VALUE_COLUMNS"/>
									<xsl:with-param name="theString">
										<xsl:value-of select="$displayNode" disable-output-escaping="yes"/>
									</xsl:with-param>
								</xsl:call-template>
								<!--<xsl:value-of select="$dataRow/@BASE64_CDX"/>
							</xsl:when>
						</xsl:choose>-->
					</xsl:when>
					<xsl:when test="string-length($displayNode) &gt; 0 and $bDoReplace != 'false'">
						<xsl:call-template name="Replace_Values">
							<xsl:with-param name="dataRow" select="$dataRow"/>
							<xsl:with-param name="displayNode" select="$displayNode"/>
							<xsl:with-param name="valueNames" select="$displayNode/@VALUE_COLUMNS"/>
							<xsl:with-param name="theString">
								<xsl:value-of select="$displayNode" disable-output-escaping="yes"/>
							</xsl:with-param>
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text disable-output-escaping="yes"> </xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:copy>
		</xsl:if>
	</xsl:template>
	<xsl:template name="Process_Sub_RS">
		<xsl:param name="dataRow"/>
		<xsl:param name="displayNode"/>
		<xsl:choose>
			<xsl:when test="$displayNode[string-length(@SORT_COLUMN) >0]">
				<!--<xsl:for-each select="$displayNode/FIELD">
					<xsl:attribute name="SORT_COLUMN"><xsl:value-of select="$displayNode/@SORT_COLUMN"/></xsl:attribute>
				</xsl:for-each>-->
				<xsl:variable name="sortField" select="$displayNode/*[@SORT_COLUMN=$displayNode/@SORT_COLUMN]"/>
				<xsl:variable name="dataTypeTemp" select="/MERGEDOC/xml/s:Schema/descendant::s:AttributeType[@name=$displayNode/@SORT_COLUMN][1]/s:datatype/@dt:type"/>
				<xsl:variable name="dataType">
					<xsl:choose>
						<xsl:when test="$sortField/@DATA_TYPE='date'">date</xsl:when>
						<xsl:when test="$dataTypeTemp='number'">number</xsl:when>
						<xsl:when test="$dataTypeTemp='string'">text</xsl:when>
						<xsl:otherwise>text</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<xsl:choose>
					<xsl:when test="$displayNode/@SORT_DIRECTION = 'ascending' and $dataType='date'">
						<xsl:choose>
							<xsl:when test="$sortField/@DATE_FORMAT='8'">
								<xsl:for-each select="$dataRow/*[local-name() = $displayNode/@NAME]">
  									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 7)" data-type="number" order="ascending"/>
									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 1, 2)" data-type="number" order="ascending"/>
									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 4, 2)" data-type="number" order="ascending"/>
									<xsl:call-template name="CREATE_SUB_RS_DATAROW">
										<xsl:with-param name="theRow" select="."/>
										<xsl:with-param name="displayNode" select="$displayNode"/>
									</xsl:call-template>
								</xsl:for-each>
  							</xsl:when>
							<xsl:when test="$sortField/@DATE_FORMAT='9'">
								<xsl:for-each select="$dataRow/*[local-name() = $displayNode/@NAME]">
  									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 7 )" data-type="number" order="ascending"/>
									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 4, 2)" data-type="number" order="ascending"/>
									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 1, 2)" data-type="number" order="ascending"/>
									<xsl:call-template name="CREATE_SUB_RS_DATAROW">
										<xsl:with-param name="theRow" select="."/>
										<xsl:with-param name="displayNode" select="$displayNode"/>
									</xsl:call-template>
								</xsl:for-each>
							</xsl:when>
							<xsl:when test="$sortField/@DATE_FORMAT='10'">
								<xsl:for-each select="$dataRow/*[local-name() = $displayNode/@NAME]">
  									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 1,4)" data-type="number" order="ascending"/>
									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 6, 2)" data-type="number" order="ascending"/>
									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 9, 2)" data-type="number" order="ascending"/>
									<xsl:call-template name="CREATE_SUB_RS_DATAROW">
										<xsl:with-param name="theRow" select="."/>
										<xsl:with-param name="displayNode" select="$displayNode"/>
									</xsl:call-template>
								</xsl:for-each>
							</xsl:when>
						</xsl:choose>
					</xsl:when>
					<xsl:when test="$displayNode/@SORT_DIRECTION = 'ascending' and $dataType='number'">
						<xsl:for-each select="$dataRow/*[local-name() = $displayNode/@NAME]">
							<xsl:sort select="./@*[local-name() = $displayNode/@SORT_COLUMN]" data-type="number" order="ascending"/>
							<xsl:call-template name="CREATE_SUB_RS_DATAROW">
								<xsl:with-param name="theRow" select="."/>
								<xsl:with-param name="displayNode" select="$displayNode"/>
							</xsl:call-template>
						</xsl:for-each>
						<xsl:if test="count($dataRow/*[local-name() = $displayNode/@NAME]) = 0">
							<xsl:call-template name="CREATE_SUB_RS_DATAROW">
								<xsl:with-param name="theRow" select="."/>
								<xsl:with-param name="displayNode" select="$displayNode"/>
							</xsl:call-template>
						</xsl:if>
					</xsl:when>
					<xsl:when test="$displayNode/@SORT_DIRECTION = 'ascending' and $dataType='text'">
						<xsl:for-each select="$dataRow/*[local-name() = $displayNode/@NAME]">
							<xsl:sort select="./@*[local-name() = $displayNode/@SORT_COLUMN]" data-type="text" order="ascending"/>
							<xsl:call-template name="CREATE_SUB_RS_DATAROW">
								<xsl:with-param name="theRow" select="."/>
								<xsl:with-param name="displayNode" select="$displayNode"/>
							</xsl:call-template>
						</xsl:for-each>
						<xsl:if test="count($dataRow/*[local-name() = $displayNode/@NAME]) = 0">
							<xsl:call-template name="CREATE_SUB_RS_DATAROW">
								<xsl:with-param name="theRow" select="."/>
								<xsl:with-param name="displayNode" select="$displayNode"/>
							</xsl:call-template>
						</xsl:if>
					</xsl:when>
					<xsl:when test="$displayNode/@SORT_DIRECTION = 'descending' and $dataType='date'">
						<xsl:choose>
							<xsl:when test="$sortField/@DATE_FORMAT='8'">
								<xsl:for-each select="$dataRow/*[local-name() = $displayNode/@NAME]">
  									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 7)" data-type="number" order="descending"/>
									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 1, 2)" data-type="number" order="descending"/>
									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 4, 2)" data-type="number" order="descending"/>
									<xsl:call-template name="CREATE_SUB_RS_DATAROW">
										<xsl:with-param name="theRow" select="."/>
										<xsl:with-param name="displayNode" select="$displayNode"/>
									</xsl:call-template>
								</xsl:for-each>
  							</xsl:when>
							<xsl:when test="$sortField/@DATE_FORMAT='9'">
								<xsl:for-each select="$dataRow/*[local-name() = $displayNode/@NAME]">
  									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 7 )" data-type="number" order="descending"/>
									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 4, 2)" data-type="number" order="descending"/>
									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 1, 2)" data-type="number" order="descending"/>
									<xsl:call-template name="CREATE_SUB_RS_DATAROW">
										<xsl:with-param name="theRow" select="."/>
										<xsl:with-param name="displayNode" select="$displayNode"/>
									</xsl:call-template>
								</xsl:for-each>
							</xsl:when>
							<xsl:when test="$sortField/@DATE_FORMAT='10'">
								<xsl:for-each select="$dataRow/*[local-name() = $displayNode/@NAME]">
  									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 1, 4)" data-type="number" order="descending"/>
									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 6, 2)" data-type="number" order="descending"/>
									<xsl:sort select="substring(./@*[local-name() = $displayNode/@SORT_COLUMN], 9, 2)" data-type="number" order="descending"/>
									<xsl:call-template name="CREATE_SUB_RS_DATAROW">
										<xsl:with-param name="theRow" select="."/>
										<xsl:with-param name="displayNode" select="$displayNode"/>
									</xsl:call-template>
								</xsl:for-each>
							</xsl:when>
						</xsl:choose>
					</xsl:when>
					<xsl:when test="$displayNode/@SORT_DIRECTION = 'descending' and $dataType='number'">
						<xsl:for-each select="$dataRow/*[local-name() = $displayNode/@NAME]">
							<xsl:sort select="./@*[local-name() = $displayNode/@SORT_COLUMN]" data-type="number" order="descending"/>
							<xsl:call-template name="CREATE_SUB_RS_DATAROW">
								<xsl:with-param name="theRow" select="."/>
								<xsl:with-param name="displayNode" select="$displayNode"/>
							</xsl:call-template>
						</xsl:for-each>
						<xsl:if test="count($dataRow/*[local-name() = $displayNode/@NAME]) = 0">
							<xsl:call-template name="CREATE_SUB_RS_DATAROW">
								<xsl:with-param name="theRow" select="."/>
								<xsl:with-param name="displayNode" select="$displayNode"/>
							</xsl:call-template>
						</xsl:if>
					</xsl:when>
					<xsl:when test="$displayNode/@SORT_DIRECTION = 'descending' and $dataType='text'">
						<xsl:for-each select="$dataRow/*[local-name() = $displayNode/@NAME]">
							<xsl:sort select="./@*[local-name() = $displayNode/@SORT_COLUMN]" data-type="text" order="descending"/>
							<xsl:call-template name="CREATE_SUB_RS_DATAROW">
								<xsl:with-param name="theRow" select="."/>
								<xsl:with-param name="displayNode" select="$displayNode"/>
							</xsl:call-template>
						</xsl:for-each>
						<xsl:if test="count($dataRow/*[local-name() = $displayNode/@NAME]) = 0">
							<xsl:call-template name="CREATE_SUB_RS_DATAROW">
								<xsl:with-param name="theRow" select="."/>
								<xsl:with-param name="displayNode" select="$displayNode"/>
							</xsl:call-template>
						</xsl:if>
					</xsl:when>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="count($dataRow/*[local-name() = $displayNode/@NAME]) > 0">
						<xsl:for-each select="$dataRow/*[local-name() = $displayNode/@NAME]">
							<xsl:call-template name="CREATE_SUB_RS_DATAROW">
								<xsl:with-param name="theRow" select="."/>
								<xsl:with-param name="displayNode" select="$displayNode"/>
							</xsl:call-template>
						</xsl:for-each>
					</xsl:when>
					<xsl:otherwise>
					<DATAROW>

						<xsl:for-each select="$displayNode/*">
							<xsl:call-template name="Element_Builder">
								<xsl:with-param name="dataRow" select="$dataRow"/>
								<xsl:with-param name="displayNode" select="."/>
								<xsl:with-param name="bDoReplace">false</xsl:with-param>
							</xsl:call-template>
						</xsl:for-each>					
					</DATAROW>

					</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="CREATE_SUB_RS_DATAROW">
		<xsl:param name="theRow"/>
		<xsl:param name="displayNode"/>
		<DATAROW>
			<xsl:for-each select="$displayNode/*">
				<xsl:call-template name="Element_Builder">
					<xsl:with-param name="dataRow" select="$theRow"/>
					<xsl:with-param name="displayNode" select="."/>
				</xsl:call-template>
			</xsl:for-each>
		</DATAROW>
	</xsl:template>
	<xsl:template name="Copy_Elements">
		<xsl:param name="dataSet"/>
		<xsl:for-each select="$dataSet">
			<xsl:copy>
				<xsl:call-template name="Copy_Attributes">
					<xsl:with-param name="dataNode" select="."/>
				</xsl:call-template>
			</xsl:copy>
		</xsl:for-each>
	</xsl:template>
	<xsl:template name="Copy_Attributes">
		<xsl:param name="dataNode"/>
		<xsl:for-each select="$dataNode/@*">
			<xsl:attribute name="{name(.)}"><xsl:value-of select="."/></xsl:attribute>
		</xsl:for-each>
	</xsl:template>
	<xsl:template name="Replace_Values">
		<xsl:param name="dataRow"/>
		<xsl:param name="displayNode"/>
		<xsl:param name="valueNames"/>
		<xsl:param name="theString"/>
		<xsl:choose>
			<!-- more than 1 value name -->
			<xsl:when test="contains($valueNames,',')">
				<xsl:variable name="currName" select="substring-before($valueNames,',')"/>
				<xsl:variable name="nextName" select="substring-after($valueNames,',')"/>
				<xsl:variable name="currValue">
					<xsl:choose>
						<!-- ASP vars -->
						<xsl:when test="substring($currName,1,4) = 'ASP:'">
							<xsl:value-of select="/MERGEDOC/DOCUMENT/ASP_VARIABLES/VARIABLE[@NAME = substring-after($currName,'ASP:')]"/>
						</xsl:when>
						<!-- sub RS FIELDs -->
						<xsl:when test="string-length($displayNode/@SUB_RS_NAME) &gt; 0">
							<xsl:choose>
								<!-- format dates -->
								<xsl:when test="/MERGEDOC/xml/s:Schema/descendant::s:ElementType[@name = $displayNode/@SUB_RS_NAME]/s:AttributeType[@name = $currName]/s:datatype[@dt:type = 'dateTime']">
									<xsl:value-of select="substring($dataRow/*[local-name() = $displayNode/@SUB_RS_NAME]/@*[local-name() = $currName],1,10)"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="$dataRow/@*[local-name() = $currName]"/>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:when>
						<!-- top level FIELDS -->
						<!-- format the dates -->
						<xsl:when test="/MERGEDOC/xml/s:Schema/s:ElementType[@name = 'row']/s:AttributeType[@name=$currName]/s:datatype[@dt:type = 'dateTime']">
							<xsl:value-of select="substring($dataRow/@*[local-name() = $currName],1,10)"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="$dataRow/@*[local-name() = $currName]"/>
						</xsl:otherwise>
						<!--
						<xsl:when test="/MERGEDOC/xml/s:Schema/s:ElementType[@name = 'row']/s:AttributeType[@name=$currName]/s:datatype[@dt:type = 'dateTime']">
							<xsl:value-of select="substring($dataRow/@*[local-name() = $currName],1,10)"/>
						</xsl:when>
						<xsl:when test="string-length($displayNode/@SUB_RS_NAME) &gt; 0">
							<xsl:value-of select="$dataRow/*[local-name() = $displayNode/@SUB_RS_NAME]/@*[@name = $currName]"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="$dataRow/@*[local-name() = $currName]"/>
						</xsl:otherwise>
						-->
					</xsl:choose>
				</xsl:variable>
				<xsl:variable name="currString">
					<xsl:choose>
						<xsl:when test="string-length($currValue) &gt; 0">
							<xsl:call-template name="SEARCH-AND-REPLACE">
								<xsl:with-param name="string" select="concat(' ',$theString)"/>
								<xsl:with-param name="search-for" select="concat('#',$currName,'#')"/>
								<xsl:with-param name="replace-with" select="$currValue"/>
							</xsl:call-template>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="' '"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<xsl:call-template name="Replace_Values">
					<xsl:with-param name="dataRow" select="$dataRow"/>
					<xsl:with-param name="displayNode" select="$displayNode"/>
					<xsl:with-param name="valueNames" select="$nextName"/>
					<xsl:with-param name="theString" select="substring-after($currString,' ')"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="string-length($valueNames &gt; 0)">
				<xsl:variable name="currName" select="$valueNames"/>
				<xsl:variable name="currValue">
					<xsl:choose>
						<!-- ASP vars -->
						<xsl:when test="substring($currName,1,4) = 'ASP:'">
							<xsl:value-of select="/MERGEDOC/DOCUMENT/ASP_VARIABLES/VARIABLE[@NAME = substring-after($currName,'ASP:')]"/>
						</xsl:when>
						<!-- sub RS FIELDs -->
						<xsl:when test="string-length($displayNode/@SUB_RS_NAME) &gt; 0">
							<xsl:choose>
								<!-- format dates -->
								<xsl:when test="/MERGEDOC/xml/s:Schema/descendant::s:ElementType[@name = $displayNode/@SUB_RS_NAME]/s:AttributeType[@name = $currName]/s:datatype[@dt:type = 'dateTime']">
									<xsl:value-of select="substring($dataRow/*[local-name() = $displayNode/@SUB_RS_NAME]/@*[local-name() = $currName],1,10)"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="$dataRow/@*[local-name() = $currName]"/>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:when>
						<!-- top level FIELDS -->
						<!-- format the dates -->
						<xsl:when test="/MERGEDOC/xml/s:Schema/s:ElementType[@name = 'row']/s:AttributeType[@name=$currName]/s:datatype[@dt:type = 'dateTime']">
							<xsl:value-of select="substring($dataRow/@*[local-name() = $currName],1,10)"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="$dataRow/@*[local-name() = $currName]"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<xsl:variable name="currString">
					<xsl:choose>
						<xsl:when test="string-length($currValue) &gt; 0">
							<xsl:call-template name="SEARCH-AND-REPLACE">
								<xsl:with-param name="string" select="concat(' ',$theString)"/>
								<xsl:with-param name="search-for" select="concat('#',$currName,'#')"/>
								<xsl:with-param name="replace-with" select="$currValue"/>
							</xsl:call-template>
						</xsl:when>
						<xsl:otherwise>
							<xsl:variable name="blank"/>
							<xsl:call-template name="SEARCH-AND-REPLACE">
								<xsl:with-param name="string" select="concat(' ',$theString)"/>
								<xsl:with-param name="search-for" select="concat('#',$currName,'#')"/>
								<xsl:with-param name="replace-with" select="$blank"/>
							</xsl:call-template>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<xsl:value-of select="substring-after($currString,' ')"/>
			</xsl:when>
			<xsl:otherwise>
		
		</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="SEARCH-AND-REPLACE">
		<xsl:param name="string"/>
		<xsl:param name="search-for"/>
		<xsl:param name="replace-with"/>
		<xsl:choose>
			<xsl:when test="contains($string,$search-for)">
				<xsl:value-of select="substring-before($string,$search-for)"/>
				<xsl:value-of select="$replace-with"/>
				<xsl:call-template name="SEARCH-AND-REPLACE">
					<xsl:with-param name="string" select="substring-after($string,$search-for)"/>
					<xsl:with-param name="search-for" select="$search-for"/>
					<xsl:with-param name="replace-with" select="$replace-with"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$string"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>
