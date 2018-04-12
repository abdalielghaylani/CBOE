<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
Dim Conn
Dim ShapeConn
Dim Cmd
Dim RS
Dim bNoWells

bDebugPrint = false
ContainerCount = 0
nameLength =10
on error resume next

Response.ExpiresAbsolute = Now()
WellList = Request("WellList")

bWells = false
if len(WellList)>0 then bWells = true

fieldArray = Session("WellReportFieldArray3")
if bWells then
Delimiter = chr(9)	
%>	
<!--#INCLUDE VIRTUAL = "/cheminv/gui/WellSQL.asp"-->
<%		
	SQL = SQL & "w.well_id in (" & WellList & ")"
	ParentWellSQL = ParentWellSQL & " AND w.well_id in (SELECT wp.parent_well_id_fk FROM inv_well_parent wp, inv_wells w WHERE w.well_id = wp.child_well_id_fk and w.well_id in (" & WellList &"))"
	GetInvShapeConnection()

	Shapestr = "SHAPE {" & SQL & "}" & _
               " APPEND ((" & _
               " SHAPE {SELECT wp.parent_well_id_fk, wp.child_well_id_fk FROM inv_well_parent wp, inv_wells w where child_well_id_fk = well_id and well_id in (Select Well_id from inv_wells where well_id in (" & WellList & "))} as rsWellParent" & _
               " APPEND(" & _
				" {" & ParentWellSQL & "} as rsParent" & _               
				" RELATE parent_well_id_fk to well_id))" & _
               " RELATE well_id TO child_well_id_fk)"
	Set ShapeCmd = Server.CreateObject("ADODB.Command")
	ShapeCmd.ActiveConnection = ShapeConn
	ShapeCmd.commandtext = Shapestr

	If bDebugPrint then
		Response.Write "<BR><BR><BR><BR><BR><BR><BR>" & SQL
		Response.Write "<BR><BR><BR><BR><BR><BR><BR>" & ParentWellSQL
		Response.end	
	End if


	Set RS= Server.CreateObject("ADODB.Recordset")
	RS.cursorlocation= aduseclient
	RS.cachesize=5
	'ShapeConn.Execute("Alter session set sql_trace = true")
	' The shape recordset is obtained by executing on the command object
	RS.open ShapeCmd
	'ShapeConn.Execute("Alter session set sql_trace = false")
	
	if NOT (RS.BOF AND RS.EOF) then 
		TotalWells = RS.RecordCount
	End if
	
	If RS.EOF AND RS.BOF then bNoWells = True

	Set oStream = server.CreateObject("ADODB.Stream")
	oStream.Open 
	oStream.Type = adTypeText
	oStream.Charset = "ascii"
	
	Do While Not RS.EOF
		'get parent well information
		Set rsWellParent = RS("rsWellParent").Value
		if rsWellParent.BOF or rsWellParent.EOF then
			For i=0 to Ubound(fieldArray)
				fn = Cstr(fieldArray(i,0))
				if inStr(1,fn,"inv_compounds.") > 0 then
					fn = Replace(fn, "inv_compounds.", "")
				end if
				'Response.Write fn & ":" & RS(fn)
				oStream.WriteText(RS(fn) & Delimiter)
			Next
			'get rid of trailing tab
			'add CR
			oStream.Position = oStream.Size-1
			oStream.WriteText vbcrlf
			counter = counter + 1
		else
			while not rsWellParent.EOF
				Set rsParent = rsWellParent("rsParent").value
				while not (rsParent.BOF or rsParent.EOF)
					For i=0 to Ubound(fieldArray)
						fn = Cstr(fieldArray(i,0))
						if inStr(1,fn,"inv_compounds.") > 0 then
							fn = Replace(fn, "inv_compounds.", "")
						end if
						'Response.Write fn & ":" & RS(fn)
						select case lcase(fn)
							case "compound_id_fk"
								oStream.WriteText(rsParent(fn) & Delimiter)
							case "reg_batch_id"
								oStream.WriteText(rsParent(fn) & Delimiter)
							case "parent_well_names"
								oStream.WriteText(rsParent("grid_position_name") & Delimiter)
							case "parent_plate_barcodes"
								oStream.WriteText(rsParent("plate_barcode") & Delimiter)
							case else
								oStream.WriteText(RS(fn) & Delimiter)
						end select
					Next
					rsParent.MoveNext
					'get rid of trailing tab
					'add CR
					oStream.Position = oStream.Size-1
					oStream.WriteText vbcrlf
					counter = counter + 1
				wend
				rsWellParent.MoveNext
			wend
		end if
		RS.MoveNext
	Loop
	Response.Buffer = true
	Response.AddHeader "Content-Disposition", "attachment;filename=ExportWells.txt"
	Response.ContentType = "text/plain"
	'Response.ContentType = "application/download"
	'Response.Write FileText
	oStream.Position = 0
	Response.Write oStream.ReadText
		
	RS.Close
	Set RS = Nothing
	ShapeConn.Close
	Set ShapeConn = Nothing
	Set oStream = Nothing
	Response.End

'if no well list sent 
else
	'Response.Write "<BR><BR><BR><BR><BR><BR><TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff><TR><TD HEIGHT=50 VALIGN=MIDDLE NOWRAP><span class=""GuiFeedback"">No wells selected..</span></TD></TR></TABLE>"
End if	
%>
