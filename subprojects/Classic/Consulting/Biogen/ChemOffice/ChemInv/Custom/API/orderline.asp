<% Sub CreateOrderLineXML
iProcQty=pkgnum
iProcCatNum = Cmd.Parameters("PSUPPLIERCATNUM")
iProcProdDesc = Cmd.parameters("PCONTAINERNAME") & ". " & Cmd.parameters("PCONTAINERDESC")
iProcCatCode = category
iProcPrice = Cmd.Parameters("PCONTAINERCOST")
iProcDuns = DUNS
iProcUOMType = "EA"
iProcUnit=Unit
iProcContainerIdList=cmd.Parameters("PNEWIDS")
iProcReqNum=Cmd.Parameters("PREQNUMBER")
iProcPONum=Cmd.Parameters("PPONUMBER")
iProcPackageSize=Cmd.Parameters("PMaxQty")

OrderLine =  " <orderLine>" & vbCRLF & _
  "  <contract contractNumberIdentifier=""KNOWN"">" & vbCRLF & _
  "   <supplierContract>" & vbCRLF & vbCRLF & _
  "    <contractNumber>Unknown</contractNumber>" & vbCRLF & vbCRLF & _
  "   </supplierContract>" & vbCRLF & _
  "   <catalogType>CATALOG</catalogType>" & vbCRLF & _
  "  </contract>" & vbCRLF & _
  "  <item lineType=""GOODS"" quantity=""" & iProcQty & """>" & vbCRLF & _
  "   <itemNumber>" & vbCRLF & _
  "    <supplierItemNumber>" & vbCRLF & _
  "       <itemID>" & iProcCatNum & "</itemID>" & vbCRLF & _
  "      </supplierItemNumber>" & vbCRLF & _
  "      <manufacturerItemNumber>" & vbCRLF & _
  "       <itemID />" & vbCRLF & _
  "       <manufacturerName />" & vbCRLF & _
  "      </manufacturerItemNumber>" & vbCRLF & _
  "      <buyerItemNumber>" & vbCRLF & _
  "       <itemID />" & vbCRLF & _
  "      </buyerItemNumber>" & vbCRLF & _
  "     </itemNumber>" & vbCRLF & _
  "     <itemDescription>" & iProcProdDesc & "</itemDescription>" & vbCRLF & _
  "     <unitOfMeasure>" & vbCRLF & _
  "      <supplierUnitOfMeasure>" & vbCRLF & _
  "       <supplierUOMType>" & iProcUOMType & "</supplierUOMType>" & vbCRLF & _
  "       <supplierUOMQuantity>1</supplierUOMQuantity>" & vbCRLF & _
  "       </supplierUnitOfMeasure>" & vbCRLF & _
  "     </unitOfMeasure>" & vbCRLF & _
  "    </item>" & vbCRLF & _
  "    <category categoryCodeIdentifier=""SUPPLIER"">" & vbCRLF & _
  "     <categoryCode>" & iProcCatCode & "</categoryCode>" & vbCRLF & _
  "    </category>" & vbCRLF & _
  "    <price>" & vbCRLF & _
  "     <currency>USD</currency>" & vbCRLF & _
  "     <unitPrice>" & iProcPrice & "</unitPrice>" & vbCRLF & _
  "    </price>" & vbCRLF & _
  "    <supplier>" & vbCRLF & _
  "     <supplierDUNS>" & iProcDuns & "</supplierDUNS>" & vbCRLF & _
  "    </supplier>" & vbCRLF & _
  "    <additionalAttributes>" & vbCRLF & _
  "     <attribute15>" & iProcReqNum & "</attribute15>" & vbCRLF & _
  "     <attribute11>CambridgeSoft</attribute11>" & vbCRLF & _
  "     </additionalAttributes> " & vbCRLF & _
  "   </orderLine>"




End Sub
%>