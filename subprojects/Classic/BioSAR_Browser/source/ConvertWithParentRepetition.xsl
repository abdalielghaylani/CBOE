<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:rs="urn:schemas-microsoft-com:rowset" xmlns:z="#RowsetSchema" xmlns:s="uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882" xmlns:dt="uuid:C2F41010-65B3-11d1-A29F-00AA00C14882" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet" xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:html="http://www.w3.org/TR/REC-html40">
	<xsl:output method="xml"/>
	<xsl:template match="/">
		<Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet" xmlns:html="http://www.w3.org/TR/REC-html40">
			<DocumentProperties xmlns="urn:schemas-microsoft-com:office:office">
				<Author>CambridgeSoft</Author>
				<LastAuthor/>
				<Created/>
				<LastSaved/>
				<Company>CambridgeSoft</Company>
				<Version/>
			</DocumentProperties>
			<Styles>
				<Style ss:ID="Default" ss:Name="Normal">
					<Alignment ss:Horizontal="Left" ss:Vertical="Top"/>
					<Borders/>
					<Font/>
					<Interior/>
					<NumberFormat/>
					<Protection/>
				</Style>
				<Style ss:ID="s22">
					<Alignment ss:Horizontal="Left" ss:Vertical="Top"/>
				</Style>
				<Style ss:ID="s23">
					<Alignment ss:Horizontal="Left" ss:Vertical="Top" ss:WrapText="1"/>
				</Style>
				<Style ss:ID="s115">
					<Alignment ss:Horizontal="Left" ss:Vertical="Top" ss:WrapText="1"/>
					<Borders>
						<Border ss:Position="Bottom" ss:LineStyle="Continuous"/>
						<Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>
						<Border ss:Position="Right" ss:LineStyle="Continuous"/>
						<Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>
					</Borders>
					<Font ss:Bold="1"/>
					<Interior ss:Color="#C0C0C0" ss:Pattern="Solid"/>
				</Style>
				<Style ss:ID="s120">
					<Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>
					<Borders>
						<Border ss:Position="Bottom" ss:LineStyle="Continuous"/>
						<Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>
						<Border ss:Position="Right" ss:LineStyle="Continuous"/>
						<Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>
					</Borders>
					<Font ss:Size="12" ss:Bold="1"/>
					<Interior ss:Color="#C0C0C0" ss:Pattern="Solid"/>
				</Style>
				<Style ss:ID="s121">
					<Alignment ss:Vertical="Center" ss:WrapText="1"/>
					<Borders>
						<Border ss:Position="Bottom" ss:LineStyle="Continuous"/>
						<Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>
						<Border ss:Position="Right" ss:LineStyle="Continuous"/>
						<Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>
					</Borders>
					<Font ss:Size="10" ss:Bold="1"/>
					<Interior ss:Color="#C0C0C0" ss:Pattern="Solid"/>
				</Style>
				<Style ss:ID="s125">
					<Alignment ss:Vertical="Bottom" ss:WrapText="1"/>
					<Borders>
						<Border ss:Position="Bottom" ss:LineStyle="Continuous"/>
						<Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>
						<Border ss:Position="Right" ss:LineStyle="Continuous"/>
						<Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>
					</Borders>
					<Font ss:Size="10" ss:Bold="1"/>
				</Style>
			</Styles>
			<Worksheet ss:Name="ChemDraw1">
				<Names>
					<NamedRange ss:Name="CFxlNextC" ss:RefersTo="=5" ss:Hidden="1"/>
				</Names>
				<Table x:FullColumns="1" x:FullRows="1">
					<xsl:attribute name="ss:ExpandedColumnCount"><xsl:value-of select="count(//s:ElementType/s:AttributeType)"/></xsl:attribute>
					<Column ss:AutoFitWidth="0" ss:Width="206.25"/>
					<xsl:apply-templates select="//s:ElementType" mode="columns"/>
					<!-- output column header grouping data here -->
					<Row>
						<xsl:apply-templates select="//s:ElementType" mode="groupings"/>
					</Row>
					<!-- now output column headers here -->
					<Row>
						<xsl:apply-templates select="//s:ElementType" mode="headers"/>
					</Row>
					<!-- output all the Row data here -->
					<xsl:apply-templates select="//rs:data/z:row"/>
				</Table>
			</Worksheet>
			<Worksheet ss:Name="CSXL_Settings">
				<Table ss:ExpandedColumnCount="2" ss:ExpandedRowCount="4" x:FullColumns="1" x:FullRows="1">
					<Row>
						<Cell>
							<Data ss:Type="String">MajorVersion_ChemDraw</Data>
						</Cell>
						<Cell>
							<Data ss:Type="Number">9</Data>
						</Cell>
					</Row>
					<Row>
						<Cell>
							<Data ss:Type="String">MinorVersion_ChemDraw</Data>
						</Cell>
						<Cell>
							<Data ss:Type="Number">0</Data>
						</Cell>
					</Row>
					<Row>
						<Cell>
							<Data ss:Type="String">BugfixVersion_ChemDraw</Data>
						</Cell>
						<Cell>
							<Data ss:Type="Number">0</Data>
						</Cell>
					</Row>
					<Row>
						<Cell>
							<Data ss:Type="String">StyleSheetPath</Data>
						</Cell>
					</Row>
				</Table>
				<WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel">
					<Visible>SheetVeryHidden</Visible>
					<ProtectObjects>False</ProtectObjects>
					<ProtectScenarios>False</ProtectScenarios>
				</WorksheetOptions>
			</Worksheet>
		</Workbook>
	</xsl:template>
	<xsl:template match="s:ElementType" mode="columns">
		<xsl:choose>
			<xsl:when test="@name='row'">
				<xsl:for-each select="s:AttributeType">
					<!--filter unwanted columns-->
					<xsl:if test="(contains('MOL_ID,INDEXCOUNTER,PAGEROWNUM,ROWID',@name) = false) and (count(@rs:name[contains('ROWID',.)])=0)">
						<xsl:if test="(string-length(@rs:name)>0)">
							<Column ssAutoFitWidth="1" ss:Width="80"/>
						</xsl:if>
					</xsl:if>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<xsl:for-each select="s:AttributeType">
					<!--filter unwanted columns-->
					<xsl:if test="(string-length(@rs:name)>0)">
						<Column ssAutoFitWidth="1" ss:Width="80"/>
					</xsl:if>
				</xsl:for-each>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- these are the column header groupings; they span the number -->
	<!-- of columns equal to the number of AttributeType children -->
	<xsl:template match="s:ElementType" mode="groupings">
		<xsl:choose>
			<xsl:when test="@name='row'">
				<Cell ss:StyleID="s120">
					<xsl:attribute name="ss:MergeAcross"><xsl:value-of select="count(child::s:AttributeType[ (string-length(normalize-space(@rs:name))>0) and not (boolean(@rs:hidden))])-1"/></xsl:attribute>
					<Data ss:Type="String">
						<xsl:value-of select="@rs:name"/>
					</Data>
					<NamedCell ss:Name="Print_Titles"/>
					<NamedCell ss:Name="Print_Area"/>
				</Cell>
			</xsl:when>
			<xsl:otherwise>
				<Cell ss:StyleID="s120">
					<xsl:attribute name="ss:MergeAcross"><xsl:value-of select="count(child::s:AttributeType[string-length(@rs:name)>0])-1"/></xsl:attribute>
					<Data ss:Type="String">
						<xsl:value-of select="@rs:name"/>
					</Data>
					<NamedCell ss:Name="Print_Titles"/>
					<NamedCell ss:Name="Print_Area"/>
				</Cell>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- Get the names of the AttributeType data as the column headings -->
	<xsl:template match="s:ElementType" mode="headers">
		<!-- loop through the rest of the attributes -->
		<xsl:choose>
			<xsl:when test="@name='row'">
				<xsl:for-each select="s:AttributeType">
					<xsl:if test="(contains('MOL_ID,INDEXCOUNTER,PAGEROWNUM,ROWID',@name) = false) and (count(@rs:name[contains('ROWID',.)])=0)">
						<xsl:if test="(string-length(@rs:name)>0)">
							<Cell ss:StyleID="s121">
								<xsl:if test="(contains('STRUCTURE',@rs:name)=false)">
									<Data ss:Type="String">
										<xsl:value-of select="@rs:name"/>
									</Data>
								</xsl:if>
								<NamedCell ss:Name="Print_Titles"/>
								<NamedCell ss:Name="Print_Area"/>
							</Cell>
						</xsl:if>
					</xsl:if>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<xsl:for-each select="s:AttributeType">
					<xsl:if test="(string-length(@rs:name)>0)">
						<Cell ss:StyleID="s121">
							<xsl:if test="(contains('STRUCTURE',@rs:name)=false)">
								<Data ss:Type="String">
									<xsl:value-of select="@rs:name"/>
								</Data>
							</xsl:if>
							<NamedCell ss:Name="Print_Titles"/>
							<NamedCell ss:Name="Print_Area"/>
						</Cell>
					</xsl:if>
				</xsl:for-each>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<!-- now output the values of each attribute per Row element -->
	<xsl:template match="rs:data/z:row">
		<!-- get the data type of the cell from the Metadata at the top -->
		<!--filter unwanted attributes-->
		<xsl:call-template name="for-loop">
			<xsl:with-param name="dataRow" select="."/>
			<xsl:with-param name="currRow" select="."/>
			<xsl:with-param name="Index" select="1"/>
		</xsl:call-template>
	</xsl:template>
	<xsl:template name="for-loop">
		<xsl:param name="dataRow"/>
		<xsl:param name="currRow"/>
		<xsl:param name="Index"/>
		<Row ss:Height="127">
			<xsl:for-each select="//s:ElementType[@name='row']/s:AttributeType[(contains('MOL_ID,INDEXCOUNTER,PAGEROWNUM,ROWID',@name) = false) and (count(@rs:name[contains('ROWID',.)])=0)]">
				<xsl:if test="(string-length(@rs:name)>0)">
					<xsl:variable name="currAttrib" select="."/>
					<xsl:variable name="currAttribType">
						<xsl:value-of select="$currAttrib/s:datatype/@dt:type"/>
					</xsl:variable>
					<xsl:variable name="position" select="position()"/>
					<Cell>
						<xsl:attribute name="ss:StyleID"><xsl:choose><xsl:when test="@rs:name='STRUCTURE'">s125</xsl:when><xsl:otherwise>s23</xsl:otherwise></xsl:choose></xsl:attribute>
						<Data>
							<xsl:attribute name="ss:Type"><xsl:choose><xsl:when test="$currAttribType='number'">Number</xsl:when><xsl:otherwise>String</xsl:otherwise></xsl:choose></xsl:attribute>
							<xsl:value-of select="$dataRow/@*[(name()=$currAttrib/@name) or (name()=$currAttrib/@rs:name)]"/>
						</Data>
						<NamedCell ss:Name="Print_Area"/>
					</Cell>
				</xsl:if>
			</xsl:for-each>
			<!-- now, if there's any child rows of this row, process them -->
			<!--filter unwanted attributes-->
			<xsl:for-each select="//s:Schema/s:ElementType/s:ElementType">
				<xsl:variable name="currSubtable">
					<xsl:value-of select="@name"/>
				</xsl:variable>
				<xsl:for-each select="child::s:AttributeType">
					<xsl:variable name="currAttribute1">
						<xsl:value-of select="@name"/>
					</xsl:variable>
					<xsl:variable name="position" select="position()"/>
					<xsl:if test="count(//s:ElementType[@name=$currSubtable]/s:AttributeType[(position()=$position) ])=1">
						<xsl:if test="(string-length(@rs:name)>0)">
							<Cell ss:StyleID="s23">
								<Data ss:Type="String">
									<xsl:for-each select="$currRow/*[name()=$currSubtable][$Index]">
										<xsl:value-of select="./@*[name()=$currAttribute1]"/>
									</xsl:for-each>
								</Data>
							</Cell>
						</xsl:if>
					</xsl:if>
				</xsl:for-each>
			</xsl:for-each>
		</Row>
		<xsl:variable name="IsThereOthersData">
			<xsl:for-each select="//s:Schema/s:ElementType/s:ElementType">
				<xsl:variable name="currSubtable">
					<xsl:value-of select="@name"/>
				</xsl:variable>
				<xsl:for-each select="child::s:AttributeType">
					<xsl:variable name="currAttribute1">
						<xsl:value-of select="@name"/>
					</xsl:variable>
					<xsl:variable name="position" select="position()"/>
					<xsl:if test="count(//s:ElementType[@name=$currSubtable]/s:AttributeType[(position()=$position) ])=1">
						<xsl:if test="(string-length(@rs:name)>0)">
									<xsl:for-each select="$currRow/*[name()=$currSubtable][$Index+1]">
										<xsl:value-of select="./@*[name()=$currAttribute1]"/>
									</xsl:for-each>
						</xsl:if>
					</xsl:if>
				</xsl:for-each>
			</xsl:for-each>
		</xsl:variable>
		<xsl:if test="(string-length($IsThereOthersData)>0)">
			<xsl:call-template name="for-loop">
				<xsl:with-param name="dataRow" select="$dataRow"/>
				<xsl:with-param name="currRow" select="$currRow"/>
				<xsl:with-param name="Index" select="$Index + 1"/>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	
	<!-- try processing subtables with call-template -->
	<xsl:template name="ProcessSubrows">
		<xsl:param name="currRow"/>
	</xsl:template>
	<xsl:template name="addEmptyCells">
		<xsl:param name="i"/>
		<xsl:param name="count"/>
		<xsl:if test="$i &lt;= $count">
			<Cell>
				<Data ss:Type="String"/>
			</Cell>
		</xsl:if>
		<xsl:if test="$i &lt;= $count">
			<xsl:call-template name="addEmptyCells">
				<xsl:with-param name="i">
					<!-- Increment index-->
					<xsl:value-of select="$i + 1"/>
				</xsl:with-param>
				<xsl:with-param name="count">
					<xsl:value-of select="$count"/>
				</xsl:with-param>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
	<xsl:template name="GetSubtableData">
		<xsl:param name="childTable"/>
		<xsl:for-each select="//s:Schema/s:ElementType/s:ElementType">
			<Cell ss:StyleID="s22">
				<xsl:attribute name="ss:MergeAcross"><xsl:value-of select="count(child::s:AttributeType)-1"/></xsl:attribute>
				<Data ss:Type="String">
					<!--<xsl:value-of select="@name"/>-->
					<xsl:value-of select="$childTable"/>
				</Data>
			</Cell>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>
