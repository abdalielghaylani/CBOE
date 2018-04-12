<?xml version='1.0'?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:template match="/">
    
		 <xsl:element name="CHEMINVACTIONBATCH">
			<xsl:for-each select="//package">
				<xsl:element name="CREATECONTAINER">
					<xsl:attribute name="ID">
						<xsl:value-of select="@ACXpackID" />
					</xsl:attribute>
					<xsl:attribute name="UOMID">
						<xsl:value-of select="@ContainerUOMID" />
					</xsl:attribute>
					<xsl:attribute name="MaxQty">
						<xsl:value-of select="@ContainerSize" />
					</xsl:attribute>
					<xsl:attribute name="InitialQty">
						<xsl:value-of select="@initialQty" />
					</xsl:attribute>
					<xsl:attribute name="QtyRemaining">
						<xsl:value-of select="@QtyRemaining" />
					</xsl:attribute>
					<xsl:attribute name="LocationID">
						<xsl:value-of select="@locationID" />
					</xsl:attribute>
					<xsl:attribute name="PrincipalID">
						<xsl:value-of select="@PrincipalID" />
					</xsl:attribute>
					<xsl:attribute name="ContainerTypeID">
						<xsl:value-of select="@containerTypeID" />
					</xsl:attribute>
					<xsl:attribute name="ContainerStatusID">
						<xsl:value-of select="@containerStatusID" />
					</xsl:attribute>
					<xsl:attribute name="DeliveryLocationID">
						<xsl:value-of select="@DeliveryLocationID" />
					</xsl:attribute>
					<xsl:attribute name="ProjectNo">
						<xsl:value-of select="@ProjectNo" />
					</xsl:attribute>
					<xsl:attribute name="JobNo">
						<xsl:value-of select="@JobNo" />
					</xsl:attribute>
					<xsl:attribute name="DueDate">
						<xsl:value-of select="@DueDate" />
					</xsl:attribute>
					<xsl:attribute name="IsRushOrder">
						<xsl:value-of select="@IsRushOrder" />
					</xsl:attribute>
					
					<xsl:attribute name="OwnerID">
						<xsl:value-of select="@OwnerID" />
					</xsl:attribute>
					<xsl:attribute name="OrderReasonID">
						<xsl:value-of select="@OrderReasonID" />
					</xsl:attribute>
					<xsl:attribute name="OrderReasonIfOtherText">
						<xsl:value-of select="@OrderReasonIfOtherText" />
					</xsl:attribute>

					<xsl:element name="OPTIONALPARAMS">
						<xsl:element name="CONTAINERNAME">
							<xsl:value-of select="ancestor::product/prodName" />				
						</xsl:element>
						<xsl:element name="CONTAINERDESC">
							<xsl:value-of select="packSize" />				
						</xsl:element>
						<xsl:element name="SUPPLIERID">
							<xsl:value-of select="ancestor::product/@supplierID" />				
						</xsl:element>
						<xsl:element name="SUPPLIERCATNUM">
							<xsl:value-of select="ancestor::product/@catNum" />				
						</xsl:element>
						<xsl:element name="CONTAINERCOST">
							<xsl:value-of select="packPrice" />				
						</xsl:element>
						<xsl:element name="UOPID">
							<xsl:value-of select="@UOPID" />				
						</xsl:element>
						<xsl:element name="NUMCOPIES">
							<xsl:value-of select="@NumCopies" />				
						</xsl:element>
						<xsl:element name="FIELD_1">
							<xsl:value-of select="@Field_1" />				
						</xsl:element>
						<xsl:element name="FIELD_2">
							<xsl:value-of select="@Field_2" />				
						</xsl:element>
						<xsl:element name="FIELD_3">
							<xsl:value-of select="@Field_3" />				
						</xsl:element>
						<xsl:element name="FIELD_4">
							<xsl:value-of select="@Field_4" />				
						</xsl:element>
						<xsl:element name="FIELD_5">
							<xsl:value-of select="@Field_5" />				
						</xsl:element>
						<xsl:element name="FIELD_5">
							<xsl:value-of select="@Field_6" />				
						</xsl:element>
						<xsl:element name="FIELD_5">
							<xsl:value-of select="@Field_7" />				
						</xsl:element>
						<xsl:element name="FIELD_5">
							<xsl:value-of select="@Field_8" />				
						</xsl:element>
						<xsl:element name="FIELD_5">
							<xsl:value-of select="@Field_9" />				
						</xsl:element>
						<xsl:element name="FIELD_5">
							<xsl:value-of select="@Field_10" />				
						</xsl:element>
					</xsl:element>
				</xsl:element>
				<xsl:element name="CREATESUBSTANCE">
					<xsl:attribute name="packageID">
						<xsl:value-of select="@ACXpackID" />
					</xsl:attribute>
					<xsl:attribute name="ACX_ID">
						<xsl:value-of select="ancestor::substance/@acxNum" />
					</xsl:attribute>
					<xsl:attribute name="CAS">
						<xsl:value-of select="ancestor::substance/@casNum" />
					</xsl:attribute>
					<xsl:copy-of select="ancestor::substance/structure"/>
					<xsl:element name="substanceName">
						<xsl:value-of select="ancestor::substance/substanceName"/>
					</xsl:element>
					<xsl:element name="OPTIONALPARAMS">
						<xsl:element name="ID">
							<xsl:value-of select="@ACXpackID" />				
						</xsl:element>
					</xsl:element>
				</xsl:element> 	
			</xsl:for-each>	
		</xsl:element>
  </xsl:template>
</xsl:stylesheet>


