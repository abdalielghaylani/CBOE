<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" encoding="iso-8859-1"  indent="no"/>
	<xsl:template match="/">
		<xsl:for-each select="/TABLE_ELEMENT">
			<xsl:call-template name="Build_Table">
				<xsl:with-param name="dataNode" select="."/>
			</xsl:call-template>
		</xsl:for-each>
	</xsl:template>
	<xsl:template name="Build_Table">
		<xsl:param name="dataNode"/>
		<xsl:if test="count($dataNode/*) &gt; 0">
			<div>
				<xsl:attribute name="ID"><xsl:value-of select="$dataNode/@NAME"/></xsl:attribute>
				<xsl:attribute name="CLASS"><xsl:value-of select="concat($dataNode/@CLASS,'_div')"/></xsl:attribute>
				<table cellpadding="1" cellspacing="2">
					<xsl:attribute name="BORDER"><xsl:value-of select="$dataNode/@BORDER"/></xsl:attribute>
					<xsl:attribute name="WIDTH"><xsl:value-of select="$dataNode/@WIDTH"/></xsl:attribute>
					<xsl:attribute name="HEIGHT"><xsl:value-of select="$dataNode/@HEIGHT"/></xsl:attribute>
					<xsl:attribute name="CLASS"><xsl:value-of select="$dataNode/@CLASS"/></xsl:attribute>
					<xsl:variable name="nlTotalHeaders" select="$dataNode/descendant::*[string-length(@HEADER_NAME) != 0]"/>
					<xsl:variable name="hasParent" select="count($dataNode/parent::*)"/>
					<xsl:variable name="subHeadersCount" select="count($nlTotalHeaders[string-length(@SUB_RS_NAME) > 0])"/>
					<xsl:if test="($hasParent = 0 and count($nlTotalHeaders) > $subHeadersCount) or ($hasParent = 1 and count($nlTotalHeaders) > 0 and string-length($dataNode/@COLUMNS) = 0)">
						<xsl:if test="count($dataNode/CAPTION_ELEMENT) = 1">
							<tr>
								<td>
									<xsl:attribute name="COLSPAN"><xsl:value-of select="$dataNode/@COLSPAN * 2"/></xsl:attribute>
									<xsl:attribute name="CLASS"><xsl:value-of select="$dataNode/@CLASS"/></xsl:attribute>
									<xsl:value-of select="$dataNode/CAPTION_ELEMENT"/>
								</td>
							</tr>
						</xsl:if>
						<tr>
							<xsl:call-template name="Build_Table_Headers">
								<xsl:with-param name="dataNode" select="$dataNode/descendant::*[string-length(@HEADER_NAME)!=0][1]"/>
							</xsl:call-template>
						</tr>
					</xsl:if>
					<xsl:choose>
						<xsl:when test="count($dataNode/DATAROW) &gt; 0">
							<xsl:for-each select="$dataNode/DATAROW">
								<xsl:call-template name="Build_Rows">
									<xsl:with-param name="dataNode" select="./*[1]"/>
									<xsl:with-param name="tableNode" select="$dataNode"/>
								</xsl:call-template>
							</xsl:for-each>
						</xsl:when>
						<xsl:otherwise>
							<xsl:if test="count($dataNode/FIELD) &gt; 0 or count($dataNode/TABLE_ELEMENT) &gt; 0">
								<xsl:call-template name="Build_Rows">
									<xsl:with-param name="dataNode" select="$dataNode/*[1]"/>
									<xsl:with-param name="tableNode" select="$dataNode"/>
								</xsl:call-template>
							</xsl:if>
						</xsl:otherwise>
					</xsl:choose>
				</table>
			</div>
		</xsl:if>
	</xsl:template>
	<xsl:template name="Build_Rows">
		<xsl:param name="dataNode"/>
		<xsl:param name="tableNode"/>
		<xsl:variable name="numElements">
			<xsl:call-template name="Get_Number_Of_Elements_Left_In_Row">
				<xsl:with-param name="dataNode" select="$dataNode"/>
				<xsl:with-param name="numElements" select="$tableNode/@COLUMNS"/>
				<xsl:with-param name="numColumns" select="$tableNode/@COLUMNS"/>
			</xsl:call-template>
		</xsl:variable>
		<tr>
			<xsl:attribute name="ID"><xsl:value-of select="$dataNode/parent::*/@PAGEROWNUM"/></xsl:attribute>
			<xsl:call-template name="Build_Column">
				<xsl:with-param name="dataNode" select="$dataNode"/>
				<xsl:with-param name="tableNode" select="$tableNode"/>
			</xsl:call-template>
			<xsl:if test="$numElements &gt;= 1">
				<xsl:for-each select="$dataNode/following-sibling::*[position() &lt;= $numElements]">
					<xsl:call-template name="Build_Column">
						<xsl:with-param name="dataNode" select="."/>
						<xsl:with-param name="tableNode" select="$tableNode"/>
					</xsl:call-template>
				</xsl:for-each>
			</xsl:if>
		</tr>
		<!-- check for downstream elements, then build next row -->
		<xsl:if test="count($dataNode/following-sibling::*[position() &gt;= ($numElements +1)]) &gt; 0">
			<xsl:call-template name="Build_Rows">
				<xsl:with-param name="dataNode" select="$dataNode/following-sibling::*[position() = $numElements + 1]"/>
				<xsl:with-param name="tableNode" select="$tableNode"/>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<xsl:template name="Build_Column">
		<xsl:param name="dataNode"/>
		<xsl:param name="tableNode"/>
		<xsl:variable name="isFirst" select="$dataNode/@IS_FIRST"/>
		<xsl:choose>
			<xsl:when test="name($dataNode) = 'TABLE_ELEMENT'">
				<td>
					<xsl:attribute name="CLASS"><xsl:value-of select="$dataNode/@CLASS"/></xsl:attribute>
					<xsl:choose>
						<xsl:when test="$isFirst=1">
							<div>
								<xsl:attribute name="ID"><xsl:value-of select="concat('firstColumn',$dataNode/@RSNAME)"/></xsl:attribute>
								<xsl:attribute name="CLASS">firstColumnClass</xsl:attribute>
								<xsl:call-template name="Build_Table">
									<xsl:with-param name="dataNode" select="$dataNode"/>
								</xsl:call-template>
							</div>
						</xsl:when>
						<xsl:otherwise>
							<xsl:call-template name="Build_Table">
								<xsl:with-param name="dataNode" select="$dataNode"/>
							</xsl:call-template>
						</xsl:otherwise>
					</xsl:choose>
				</td>
			</xsl:when>
			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="$dataNode/@IS_STRUCTURE = 1">
						<td>
							<xsl:attribute name="COLSPAN"><xsl:value-of select="$dataNode/@COLSPAN * 2"/></xsl:attribute>
							<xsl:attribute name="WIDTH"><xsl:value-of select="$dataNode/@WIDTH"/></xsl:attribute>
							<xsl:attribute name="HEIGHT"><xsl:value-of select="$dataNode/@HEIGHT"/></xsl:attribute>
							<xsl:choose>
								<xsl:when test="$isFirst=1">
									<div>
										<xsl:attribute name="ID"><xsl:value-of select="concat('firstColumn',$dataNode/@RSNAME)"/></xsl:attribute>
										<xsl:attribute name="Class">firstColumnClass</xsl:attribute>
										<xsl:call-template name="SHOW_STRUCTURE">
											<xsl:with-param name="dataNode" select="$dataNode"/>
										</xsl:call-template>
									</div>
								</xsl:when>
								<xsl:otherwise>
									<xsl:call-template name="SHOW_STRUCTURE">
										<xsl:with-param name="dataNode" select="$dataNode"/>
									</xsl:call-template>
								</xsl:otherwise>
							</xsl:choose>
						</td>
					</xsl:when>
					<xsl:when test="string-length($dataNode/@DISPLAY_NAME) = 0">
						<td>
							<xsl:attribute name="COLSPAN"><xsl:value-of select="$dataNode/@COLSPAN * 2"/></xsl:attribute>
							<xsl:attribute name="WIDTH"><xsl:value-of select="$dataNode/@WIDTH"/></xsl:attribute>
							<xsl:attribute name="HEIGHT"><xsl:value-of select="$dataNode/@HEIGHT"/></xsl:attribute>
							<xsl:attribute name="CLASS"><xsl:value-of select="$dataNode/@VALUE_CLASS"/></xsl:attribute>
							<xsl:choose>
								<xsl:when test="$isFirst=1">
									<div>
										<xsl:attribute name="ID"><xsl:value-of select="concat('firstColumn',$dataNode/@RSNAME)"/></xsl:attribute>
										<xsl:attribute name="CLASS">firstColumnClass</xsl:attribute>
										<xsl:choose>
											<xsl:when test="$dataNode/@MAX_LENGTH > 0">
												<xsl:call-template name="TRUNCATE_IN_SPAN">
													<xsl:with-param name="text" select="$dataNode"/>
													<xsl:with-param name="length" select="$dataNode/@MAX_LENGTH"/>
													<xsl:with-param name="id"/>
												</xsl:call-template>
											</xsl:when>
											<xsl:otherwise>
												<xsl:value-of select="$dataNode" disable-output-escaping="yes"/>
											</xsl:otherwise>
										</xsl:choose>
									</div>
								</xsl:when>
								<xsl:otherwise>
									<xsl:choose>
										<xsl:when test="$dataNode/@MAX_LENGTH > 0">
											<xsl:call-template name="TRUNCATE_IN_SPAN">
												<xsl:with-param name="text" select="$dataNode"/>
												<xsl:with-param name="length" select="$dataNode/@MAX_LENGTH"/>
												<xsl:with-param name="id"/>
											</xsl:call-template>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="$dataNode" disable-output-escaping="yes"/>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:otherwise>
							</xsl:choose>
						</td>
					</xsl:when>
					<xsl:otherwise>
						<td>
							<xsl:attribute name="CLASS"><xsl:value-of select="$dataNode/@NAME_CLASS"/></xsl:attribute>
							<xsl:attribute name="COLSPAN"><xsl:value-of select="$dataNode/@COLSPAN "/></xsl:attribute>
							<xsl:attribute name="WIDTH"><xsl:value-of select="$dataNode/@WIDTH"/></xsl:attribute>
							<xsl:attribute name="HEIGHT"><xsl:value-of select="$dataNode/@HEIGHT"/></xsl:attribute>
							<xsl:choose>
								<xsl:when test="$isFirst=1">
									<div>
										<xsl:attribute name="ID"><xsl:value-of select="concat('firstColumn',$dataNode/@RSNAME)"/></xsl:attribute>
										<xsl:attribute name="CLASS">firstColumnClass</xsl:attribute>
										
								<xsl:choose>
									<xsl:when test="string-length($dataNode/@SORT_COLUMN) != 0 and string-length($dataNode/@SUB_RS_NAME) = 0 and string-length($dataNode/@HEADER_NAME) = 0 and $dataNode/ancestor::DATAROW/@IS_FIRST='true'">
										<a> 
											<xsl:attribute name="href"><xsl:variable name="url" select="/TABLE_ELEMENT/@SORT_URL"/><xsl:choose><xsl:when test="contains($url,'?')"><xsl:value-of select="concat($url,'&amp;','sortFields=',$dataNode/@SORT_COLUMN) "/></xsl:when><xsl:otherwise><xsl:value-of select="concat($url,'?sortFields=',$dataNode/@SORT_COLUMN) "/></xsl:otherwise></xsl:choose></xsl:attribute>
											<xsl:value-of select="$dataNode/@DISPLAY_NAME"/>
										</a>
									</xsl:when>
									<xsl:when test="string-length($dataNode/@SORT_COLUMN) != 0 and string-length($dataNode/@SUB_RS_NAME) > 0 and string-length($dataNode/@HEADER_NAME) = 0 and $dataNode/ancestor::DATAROW/@IS_FIRST='true'">
										<a>
											<xsl:attribute name="href"><xsl:variable name="url" select="/TABLE_ELEMENT/@SORT_URL"/><xsl:choose><xsl:when test="contains($url,'?')"><xsl:value-of select="concat($url,'&amp;','sortFields=|',$dataNode/@SUB_RS_NAME,':',$dataNode/@SORT_COLUMN) "/></xsl:when><xsl:otherwise><xsl:value-of select="concat($url,'?sortFields=|',$dataNode/@SUB_RS_NAME,':',$dataNode/@SORT_COLUMN) "/></xsl:otherwise></xsl:choose></xsl:attribute>
											<xsl:value-of select="$dataNode/@DISPLAY_NAME"/>
										</a>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="$dataNode/@DISPLAY_NAME"/>
									</xsl:otherwise>
								</xsl:choose>
										
									</div>
								</xsl:when>
								<xsl:otherwise>
										
								<xsl:choose>
									<xsl:when test="string-length($dataNode/@SORT_COLUMN) != 0 and string-length($dataNode/@SUB_RS_NAME) = 0 and string-length($dataNode/@HEADER_NAME) = 0 and $dataNode/ancestor::DATAROW/@IS_FIRST='true'">
										<a> 
											<xsl:attribute name="href"><xsl:variable name="url" select="/TABLE_ELEMENT/@SORT_URL"/><xsl:choose><xsl:when test="contains($url,'?')"><xsl:value-of select="concat($url,'&amp;','sortFields=',$dataNode/@SORT_COLUMN) "/></xsl:when><xsl:otherwise><xsl:value-of select="concat($url,'?sortFields=',$dataNode/@SORT_COLUMN) "/></xsl:otherwise></xsl:choose></xsl:attribute>
											<xsl:value-of select="$dataNode/@DISPLAY_NAME"/>
										</a>
									</xsl:when>
									<xsl:when test="string-length($dataNode/@SORT_COLUMN) != 0 and string-length($dataNode/@SUB_RS_NAME) > 0 and string-length($dataNode/@HEADER_NAME) = 0 and $dataNode/ancestor::DATAROW/@IS_FIRST='true'">
										<a>
											<xsl:attribute name="href"><xsl:variable name="url" select="/TABLE_ELEMENT/@SORT_URL"/><xsl:choose><xsl:when test="contains($url,'?')"><xsl:value-of select="concat($url,'&amp;','sortFields=|',$dataNode/@SUB_RS_NAME,':',$dataNode/@SORT_COLUMN) "/></xsl:when><xsl:otherwise><xsl:value-of select="concat($url,'?sortFields=|',$dataNode/@SUB_RS_NAME,':',$dataNode/@SORT_COLUMN) "/></xsl:otherwise></xsl:choose></xsl:attribute>
											<xsl:value-of select="$dataNode/@DISPLAY_NAME"/>
										</a>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="$dataNode/@DISPLAY_NAME"/>
									</xsl:otherwise>
								</xsl:choose>

<!--									<xsl:value-of select="$dataNode/@DISPLAY_NAME"/> -->
								</xsl:otherwise>
							</xsl:choose>
						</td>
						<td>
							<xsl:attribute name="CLASS"><xsl:value-of select="$dataNode/@VALUE_CLASS"/></xsl:attribute>
							<xsl:attribute name="COLSPAN"><xsl:value-of select="$dataNode/@COLSPAN"/></xsl:attribute>
							<xsl:choose>
								<xsl:when test="$isFirst=1">
									<div>
										<xsl:attribute name="ID"><xsl:value-of select="concat('firstColumn',$dataNode/@RSNAME)"/></xsl:attribute>
										<xsl:attribute name="CLASS">firstColumnClass</xsl:attribute>
										<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
										<xsl:choose>
											<xsl:when test="$dataNode/@MAX_LENGTH > 0">
												<xsl:call-template name="TRUNCATE_IN_SPAN">
													<xsl:with-param name="text" select="$dataNode"/>
													<xsl:with-param name="length" select="$dataNode/@MAX_LENGTH"/>
													<xsl:with-param name="id"/>
												</xsl:call-template>
											</xsl:when>
											<xsl:otherwise>
												<xsl:value-of select="$dataNode" disable-output-escaping="yes"/>
											</xsl:otherwise>
										</xsl:choose>
									</div>
								</xsl:when>
								<xsl:otherwise>
									<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
									<xsl:choose>
										<xsl:when test="$dataNode/@MAX_LENGTH > 0">
											<xsl:call-template name="TRUNCATE_IN_SPAN">
												<xsl:with-param name="text" select="$dataNode"/>
												<xsl:with-param name="length" select="$dataNode/@MAX_LENGTH"/>
												<xsl:with-param name="id"/>
											</xsl:call-template>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="$dataNode" disable-output-escaping="yes"/>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:otherwise>
							</xsl:choose>
						</td>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="Get_Number_Of_Elements_Left_In_Row">
		<xsl:param name="dataNode"/>
		<xsl:param name="numElements"/>
		<xsl:param name="numColumns"/>
		<xsl:choose>
			<!-- if this is a table where #columns are specified -->
			<xsl:when test="$numColumns &gt; 0">
				<xsl:choose>
					<xsl:when test="(sum($dataNode/following-sibling::*[position() &lt; $numElements]/@COLSPAN) + $dataNode/@COLSPAN) &lt;= $numColumns or count($dataNode/following-sibling::*) = 0">
						<xsl:value-of select="$numElements -1"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="Get_Number_Of_Elements_Left_In_Row">
							<xsl:with-param name="dataNode" select="$dataNode"/>
							<xsl:with-param name="numElements" select="number($numElements) - 1"/>
							<xsl:with-param name="numColumns" select="$numColumns"/>
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<!-- otherwise all of the siblings are in the same row -->
			<xsl:otherwise>
				<xsl:value-of select="count($dataNode/following-sibling::*)"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="Build_Table_Headers">
		<xsl:param name="dataNode"/>
		<xsl:call-template name="Build_Header_Column">
			<xsl:with-param name="dataNode" select="$dataNode"/>
		</xsl:call-template>
		<xsl:if test="count($dataNode/following-sibling::*) &gt; 0">
			<xsl:call-template name="Build_Table_Headers">
				<xsl:with-param name="dataNode" select="$dataNode/following-sibling::*[1]"/>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<xsl:template name="Build_Header_Column">
		<xsl:param name="dataNode"/>
		<xsl:choose>
			<xsl:when test="$dataNode/ancestor::TABLE_ELEMENT[1]/@REPEAT_HEADER = 'true' or $dataNode/ancestor::DATAROW/@IS_FIRST = 'true'">
				<xsl:choose>
					<xsl:when test="$dataNode/ancestor::TABLE_ELEMENT[1]/@REPEAT_HEADER != 'true'">
						<td>
							<xsl:attribute name="COLSPAN"><xsl:value-of select="$dataNode/@COLSPAN * 2"/></xsl:attribute>
							<xsl:attribute name="WIDTH"><xsl:value-of select="$dataNode/@WIDTH"/></xsl:attribute>
							<xsl:attribute name="CLASS"><xsl:value-of select="$dataNode/@HEADER_CLASS"/></xsl:attribute>
					<img src="/cfserverasp/source/graphics/clearpix.gif" alt="" height="1" border="0">
						<xsl:attribute name="WIDTH"><xsl:value-of select="$dataNode/@WIDTH"/></xsl:attribute>
					</img>
					<br/>
							<div>
								<xsl:attribute name="ID">columnName</xsl:attribute>
								<xsl:attribute name="CLASS">columnNameClass</xsl:attribute>
								<xsl:attribute name="title">
									<xsl:value-of select="$dataNode/@HEADER_TIP"/>
								</xsl:attribute>
								<xsl:choose>
									<xsl:when test="string-length($dataNode/@SORT_COLUMN) != 0 and string-length($dataNode/@SUB_RS_NAME) = 0">
										<a>
											<xsl:attribute name="href"><xsl:variable name="url" select="/TABLE_ELEMENT/@SORT_URL"/><xsl:choose><xsl:when test="contains($url,'?')"><xsl:value-of select="concat($url,'&amp;','sortFields=',$dataNode/@SORT_COLUMN) "/></xsl:when><xsl:otherwise><xsl:value-of select="concat($url,'?sortFields=',$dataNode/@SORT_COLUMN) "/></xsl:otherwise></xsl:choose></xsl:attribute>
											<xsl:value-of select="$dataNode/@HEADER_NAME"/>
										</a>
									</xsl:when>
									<xsl:when test="string-length($dataNode/@SORT_COLUMN) != 0 and string-length($dataNode/@SUB_RS_NAME) > 0">
										<a>
											<xsl:attribute name="href"><xsl:variable name="url" select="/TABLE_ELEMENT/@SORT_URL"/><xsl:choose><xsl:when test="contains($url,'?')"><xsl:value-of select="concat($url,'&amp;','sortFields=|',$dataNode/@SUB_RS_NAME,':',$dataNode/@SORT_COLUMN) "/></xsl:when><xsl:otherwise><xsl:value-of select="concat($url,'?sortFields=|',$dataNode/@SUB_RS_NAME,':',$dataNode/@SORT_COLUMN) "/></xsl:otherwise></xsl:choose></xsl:attribute>
											<xsl:value-of select="$dataNode/@HEADER_NAME"/>
										</a>
									</xsl:when>
									<xsl:otherwise>
										<xsl:value-of select="$dataNode/@HEADER_NAME"/>
									</xsl:otherwise>
								</xsl:choose>
							</div>
						</td>
					</xsl:when>
					<xsl:otherwise>
						<td>
							<xsl:attribute name="COLSPAN"><xsl:value-of select="$dataNode/@COLSPAN * 2"/></xsl:attribute>
							<xsl:attribute name="WIDTH"><xsl:value-of select="$dataNode/@WIDTH"/></xsl:attribute>
							<xsl:attribute name="CLASS"><xsl:value-of select="$dataNode/@HEADER_CLASS"/></xsl:attribute>
							<img src="/cfserverasp/source/graphics/clearpix.gif" alt="" height="1" border="0">
								<xsl:attribute name="WIDTH"><xsl:value-of select="$dataNode/@WIDTH"/></xsl:attribute>
							</img>
							<br/>
							<div>
								<xsl:attribute name="ID">tableHeader</xsl:attribute>
								<xsl:attribute name="CLASS">tableHeaderClass</xsl:attribute>
								<xsl:attribute name="title"><xsl:value-of select="$dataNode/@HEADER_TIP"/></xsl:attribute>
					
							<xsl:choose>
								<xsl:when test="string-length($dataNode/@SORT_COLUMN) != 0 and string-length($dataNode/@SUB_RS_NAME) = 0">
									<a>
										<xsl:attribute name="href"><xsl:variable name="url" select="/TABLE_ELEMENT/@SORT_URL"/><xsl:choose><xsl:when test="contains($url,'?')"><xsl:value-of select="concat($url,'&amp;','sortFields=',$dataNode/@SORT_COLUMN) "/></xsl:when><xsl:otherwise><xsl:value-of select="concat($url,'?sortFields=',$dataNode/@SORT_COLUMN) "/></xsl:otherwise></xsl:choose></xsl:attribute>
										<xsl:value-of select="$dataNode/@HEADER_NAME"/>
									</a>
								</xsl:when>
								<xsl:when test="string-length($dataNode/@SORT_COLUMN) != 0 and string-length($dataNode/@SUB_RS_NAME) > 0">
									<a>
										<xsl:attribute name="href"><xsl:variable name="url" select="/TABLE_ELEMENT/@SORT_URL"/><xsl:choose><xsl:when test="contains($url,'?')"><xsl:value-of select="concat($url,'&amp;','sortFields=|',$dataNode/@SUB_RS_NAME,':',$dataNode/@SORT_COLUMN) "/></xsl:when><xsl:otherwise><xsl:value-of select="concat($url,'?sortFields=|',$dataNode/@SUB_RS_NAME,':',$dataNode/@SORT_COLUMN) "/></xsl:otherwise></xsl:choose></xsl:attribute>
										<xsl:value-of select="$dataNode/@HEADER_NAME"/>
									</a>
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="$dataNode/@HEADER_NAME"/>
								</xsl:otherwise>
							</xsl:choose>
							</div>

						</td>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<td>
					<xsl:attribute name="COLSPAN"><xsl:value-of select="$dataNode/@COLSPAN * 2"/></xsl:attribute>
					<xsl:attribute name="WIDTH"><xsl:value-of select="$dataNode/@WIDTH"/></xsl:attribute>
					<img src="/cfserverasp/source/graphics/clearpix.gif" alt="" height="1" border="0">
						<xsl:attribute name="WIDTH"><xsl:value-of select="$dataNode/@WIDTH"/></xsl:attribute>
					</img>
				</td>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="TRUNCATE_IN_SPAN">
		<xsl:param name="text"/>
		<xsl:param name="length"/>
		<xsl:param name="id"/>
		<span>
			<xsl:attribute name="id"><xsl:value-of select="$id"/></xsl:attribute>
			<xsl:choose>
				<xsl:when test="string-length($text)>$length">
					<xsl:attribute name="title"><xsl:value-of select="$text"/></xsl:attribute>
					<xsl:value-of select="concat(substring($text,1,(number($length)-3)),'...')" disable-output-escaping="yes"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$text" disable-output-escaping="yes"/>
				</xsl:otherwise>
			</xsl:choose>
		</span>
	</xsl:template>
	<xsl:template name="SHOW_STRUCTURE">
		<xsl:param name="dataNode"/>

		<xsl:variable name="MIMETYPE">
			<xsl:choose>
				<xsl:when test="string-length($dataNode/@MIMETYPE) > 0"><xsl:value-of select="$dataNode/@MIMETYPE"/></xsl:when>
				<xsl:otherwise>chemical/x-cdx</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<table cellpadding="1" cellspacing="2">
			<xsl:attribute name="BORDER"><xsl:value-of select="$dataNode/@BORDER"/></xsl:attribute>
			<tr>
				<td>
					<xsl:attribute name="WIDTH"><xsl:value-of select="$dataNode/@WIDTH"/></xsl:attribute>
					<xsl:attribute name="HEIGHT"><xsl:value-of select="$dataNode/@HEIGHT"/></xsl:attribute>
					<xsl:choose>
						<!-- if the structure is in a form -->
						<!--<xsl:when test="$dataNode/@DATA_TYPE = 'base64' and string-length($dataNode/@FORM_NAME) &gt; 0">-->
						<xsl:when test="string-length($dataNode/@FORM_NAME) &gt; 0">
							<xsl:variable name="inputName" select="concat('structure',count($dataNode/preceding::*))"/>
							<input type="hidden">
								<xsl:attribute name="name"><xsl:value-of select="$inputName"/></xsl:attribute>
								<xsl:choose>
									<xsl:when test="string-length($dataNode/@FILE_EXT)>0">
										<xsl:attribute name="value"><xsl:value-of select="concat($dataNode/@FILE_LOCATION,'struct',$dataNode/@PAGE_ROWNUM,$dataNode/@FILE_EXT)"/></xsl:attribute>
									</xsl:when>
									<xsl:when test="$dataNode/@DATA_LOCATION='table'">
										<xsl:attribute name="value"><xsl:value-of select="concat($dataNode/@dataURL,'&amp;unique_id=',$dataNode)"/></xsl:attribute>
									</xsl:when>
									<xsl:otherwise>
										<xsl:choose>
											<xsl:when test="string-length($dataNode)>0">
												<xsl:attribute name="value"><xsl:value-of select="concat('data:', $MIMETYPE,',',$dataNode)"/></xsl:attribute>
											</xsl:when>
											<xsl:otherwise>
												<xsl:attribute name="value"></xsl:attribute>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:otherwise>
								</xsl:choose>
							</input>
							<xsl:choose>
								<xsl:when test="$dataNode/@DATA_LOCATION='recordset'">
									<script language="javascript">
										<xsl:value-of select="concat('cd_insertObject(&quot;chemical/x-cdx&quot;,',$dataNode/@WIDTH,',',$dataNode/@HEIGHT,',&quot;mycdx&quot;,&quot;',$dataNode/@MT_LOCATION,'mt.cdx&quot;, true, true, dataURL=escape(document.', $dataNode/@FORM_NAME,'.',$inputName,'.value));')"/>
									</script>
								</xsl:when>
								<xsl:otherwise>
									<script language="javascript">
										<xsl:value-of select="concat('cd_insertObject(&quot;chemical/x-cdx&quot;,',$dataNode/@WIDTH,',',$dataNode/@HEIGHT,',&quot;mycdx&quot;,&quot;',$dataNode/@MT_LOCATION,'mt.cdx&quot;, true, true, dataURL=document.', $dataNode/@FORM_NAME,'.',$inputName,'.value);')"/>
									</script>
								</xsl:otherwise>						
							</xsl:choose>
						</xsl:when>
						<xsl:otherwise>
							<xsl:variable name="formName" select="concat('form',$dataNode/@NUM)"/>
							<form>
								<xsl:attribute name="name"><xsl:value-of select="$formName"/></xsl:attribute>
								<input type="hidden" name="structure">
									<xsl:choose>
										<xsl:when test="string-length($dataNode/@FILE_EXT)>0">
											<xsl:attribute name="value"><xsl:value-of select="concat($dataNode/@FILE_LOCATION,'struct',$dataNode/@PAGE_ROWNUM,$dataNode/@FILE_EXT)"/></xsl:attribute>
										</xsl:when>
										<xsl:when test="$dataNode/@DATA_LOCATION='table'">
											<xsl:attribute name="value"><xsl:value-of select="concat($dataNode/@dataURL,'&amp;unique_id=',$dataNode)"/></xsl:attribute>
										</xsl:when>
										<xsl:otherwise>
											<xsl:choose>
												<xsl:when test="string-length($dataNode)>0">
													<xsl:attribute name="value"><xsl:value-of select="concat('data:', $MIMETYPE,',',$dataNode)"/></xsl:attribute>
												</xsl:when>
												<xsl:otherwise>
													<xsl:attribute name="value"></xsl:attribute>
												</xsl:otherwise>
											</xsl:choose>

										</xsl:otherwise>
									</xsl:choose>
								</input>
							</form>
							<xsl:choose>
								<xsl:when test="$dataNode/@DATA_LOCATION='recordset'">
									<script language="javascript">
										<xsl:value-of select="concat('cd_insertObject(&quot;chemical/x-cdx&quot;,185,130,&quot;mycdx&quot;,&quot;/CFWTEMP/cheminv/cheminvTemp/mt.cdx&quot;, true, true, dataURL=escape(document.', $formName,'.structure.value));')"/>
									</script>
								</xsl:when>
								<xsl:otherwise>
									<script language="javascript">
										<xsl:value-of select="concat('cd_insertObject(&quot;chemical/x-cdx&quot;,185,130,&quot;mycdx&quot;,&quot;/CFWTEMP/cheminv/cheminvTemp/mt.cdx&quot;, true, true, dataURL=document.', $formName,'.structure.value);')"/>
									</script>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:otherwise>
					</xsl:choose>
				</td>
			</tr>
		</table>
	</xsl:template>

	<xsl:template name="SEARCH-AND-REPLACE">
		<xsl:param name="string"/>
		<xsl:param name="search-for"/>
		<xsl:param name="replace-with"/>
		<xsl:choose>
			<xsl:when test="contains($string,$search-for)">
				<xsl:value-of disable-output-escaping="yes" select="substring-before($string,$search-for)"/>
				<xsl:value-of disable-output-escaping="yes" select="$replace-with"/>
				<xsl:call-template name="SEARCH-AND-REPLACE">
					<xsl:with-param name="string" select="substring-after($string,$search-for)"/>
					<xsl:with-param name="search-for" select="$search-for"/>
					<xsl:with-param name="replace-with" select="$replace-with"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of disable-output-escaping="yes" select="$string"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="replace-string">
		<xsl:param name="text"/>
		<xsl:param name="from"/>
		<xsl:param name="to"/>

		<xsl:choose>
			<xsl:when test="contains($text, $from)">

				<xsl:variable name="before" select="substring-before($text, $from)"/>
				<xsl:variable name="after" select="substring-after($text, $from)"/>
				<xsl:variable name="prefix" select="concat($before, $to)"/>

				<xsl:value-of select="$before"/>
				<xsl:value-of select="$to"/>
				<xsl:call-template name="replace-string">
					<xsl:with-param name="text" select="$after"/>
					<xsl:with-param name="from" select="$from"/>
					<xsl:with-param name="to" select="$to"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$text"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>
