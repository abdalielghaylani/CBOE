<% 
Sub GetCreateContainer_parameters()
    Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
        Cmd.Parameters("RETURN_VALUE").Precision = 9
    Cmd.Parameters.Append Cmd.CreateParameter("PBARCODE",200, 1, 50, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PBARCODEDESCID",131, 1, 50, NULL)
        Cmd.Parameters("PBARCODEDESCID").NumericScale = 9
        Cmd.Parameters("PBARCODEDESCID").Precision = 4
    Cmd.Parameters.Append Cmd.CreateParameter("PLOCATIONID",200, 1, 4000, NULL)
        'Cmd.Parameters.Append Cmd.CreateParameter("PLOCATIONID",131, 1, 0, NULL)
        'Cmd.Parameters("PLOCATIONID").Precision = 9
    Cmd.Parameters.Append Cmd.CreateParameter("PUOMID",131, 1, 0, NULL)
        Cmd.Parameters("PUOMID").Precision = 4
    Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERTYPEID",131, 1, 0, NULL)
        Cmd.Parameters("PCONTAINERTYPEID").Precision = 9
    Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERSTATUSID",131, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PMAXQTY",5, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PREGID",131, 1, 0, NULL)
        Cmd.Parameters("PREGID").Precision = 9
    Cmd.Parameters.Append Cmd.CreateParameter("PBATCHNUMBER",131, 1, 0, NULL)
        Cmd.Parameters("PBATCHNUMBER").Precision = 9
    Cmd.Parameters.Append Cmd.CreateParameter("PINITIALQTY",5, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PQTYREMAINING",5, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PMINSTOCKQTY",5, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PMAXSTOCKQTY",5, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PEXPDATE",135, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PCOMPOUNDID",131, 1, 0, NULL)
        Cmd.Parameters("PCOMPOUNDID").Precision = 9
    Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERNAME",200, 1, 255, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERDESC",200, 1, 255, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PTAREWEIGHT",5, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PNETWEIGHT",5, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PFINALWEIGHT",5, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PUOWID",131, 1, 0, NULL)
        Cmd.Parameters("PUOWID").Precision = 4
    Cmd.Parameters.Append Cmd.CreateParameter("PPURITY",5, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PUOPID",131, 1, 0, NULL)
        Cmd.Parameters("PUOPID").Precision = 4
    Cmd.Parameters.Append Cmd.CreateParameter("PCONCENTRATION",5, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PDENSITY",5, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PUOCID",131, 1, 0, NULL)
        Cmd.Parameters("PUOCID").Precision = 4
    Cmd.Parameters.Append Cmd.CreateParameter("PUODID",131, 1, 0, NULL)
        Cmd.Parameters("PUODID").Precision = 4
    Cmd.Parameters.Append Cmd.CreateParameter("PSOLVENTIDFK",200, 1, 255, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PGRADE",200, 1, 255, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PCOMMENTS",200, 1, 4000, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PSTORAGECONDITIONS",200, 1, 4000, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PHANDLINGPROCEDURES",200, 1, 4000, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PSUPPLIERID",131, 1, 0, NULL)
        Cmd.Parameters("PSUPPLIERID").Precision = 4
    Cmd.Parameters.Append Cmd.CreateParameter("PSUPPLIERCATNUM",200, 1, 50, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PLOTNUM",200, 1, 50, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PDATEPRODUCED",135, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PDATEORDERED",135, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PDATERECEIVED",135, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERCOST",131, 1, 0, NULL)
        Cmd.Parameters("PCONTAINERCOST").NumericScale = 2
        Cmd.Parameters("PCONTAINERCOST").Precision = 9
    Cmd.Parameters.Append Cmd.CreateParameter("PUOCOSTID", 131, 1, 0, NULL)
        Cmd.Parameters("PUOCOSTID").Precision = 4
    Cmd.Parameters.Append Cmd.CreateParameter("PPONUMBER",200, 1, 50, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PPOLINENUMBER",200, 1, 50, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PREQNUMBER",200, 1, 50, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("POWNERID",200, 1, 50, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PCURRENTUSERID",200, 1, 50, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PNUMCOPIES", 5, 1, 0, 1)
    Cmd.Parameters.Append Cmd.CreateParameter("PNEWIDS",200, 2, 4000, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_1",200, 1, 2000, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_2",200, 1, 2000, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_3",200, 1, 2000, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_4",200, 1, 2000, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_5",200, 1, 2000, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_6",200, 1, 2000, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_7",200, 1, 2000, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_8",200, 1, 2000, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_9",200, 1, 2000, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PFIELD_10",200, 1, 2000, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PDATE_1",135, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PDATE_2",135, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PDATE_3",135, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PDATE_4",135, 1, 0, NULL)
    Cmd.Parameters.Append Cmd.CreateParameter("PDATE_5",135, 1, 0, NULL)
    if Application("ENABLE_OWNERSHIP")="TRUE" then
       Cmd.Parameters.Append Cmd.CreateParameter("PPRINCIPALID", 5, 1, 0, NULL)	
       Cmd.Parameters.Append Cmd.CreateParameter("PLOCATIONTYPE", 5, 1, 0, NULL)
    end if
End Sub

%>
