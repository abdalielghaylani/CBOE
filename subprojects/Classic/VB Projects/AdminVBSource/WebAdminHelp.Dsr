VERSION 5.00
Begin {17016CEE-E118-11D0-94B8-00A0C91110ED} WebAdminUtil 
   ClientHeight    =   9255
   ClientLeft      =   0
   ClientTop       =   0
   ClientWidth     =   9135
   _ExtentX        =   16113
   _ExtentY        =   16325
   MajorVersion    =   0
   MinorVersion    =   8
   StateManagementType=   1
   ASPFileName     =   ""
   DIID_WebClass   =   "{D71DFF92-3383-11D3-8788-0080C765739F}"
   DIID_WebClassEvents=   "{D71DFF91-3383-11D3-8788-0080C765739F}"
   TypeInfoCookie  =   445
   BeginProperty WebItems {193556CD-4486-11D1-9C70-00C04FB987DF} 
      WebItemCount    =   2
      BeginProperty WebItem1 {FA6A55FE-458A-11D1-9C71-00C04FB987DF} 
         MajorVersion    =   0
         MinorVersion    =   8
         Name            =   "WebAdminAddDialog"
         DISPID          =   1280
         Template        =   "WebAdminAddDialog.htm"
         Token           =   "WC@"
         DIID_WebItemEvents=   "{D71E034F-3383-11D3-8788-0080C765739F}"
         ParseReplacements=   0   'False
         AppendedParams  =   ""
         HasTempTemplate =   0   'False
         UsesRelativePath=   -1  'True
         OriginalTemplate=   "C:\Inetpub\wwwroot\ChemOffice\webserver_source\cfserveradmin\AdminSource\editor\WebAdminAddDialog.htm"
         TagPrefixInfo   =   2
         BeginProperty Events {193556D1-4486-11D1-9C70-00C04FB987DF} 
            EventCount      =   0
         EndProperty
         BeginProperty BoundTags {FA6A55FA-458A-11D1-9C71-00C04FB987DF} 
            AttribCount     =   0
         EndProperty
      EndProperty
      BeginProperty WebItem2 {FA6A55FE-458A-11D1-9C71-00C04FB987DF} 
         MajorVersion    =   0
         MinorVersion    =   8
         Name            =   "WebAdminHelp"
         DISPID          =   1281
         Template        =   "WebAdminHelp.htm"
         Token           =   "WC@"
         DIID_WebItemEvents=   "{D71DFF97-3383-11D3-8788-0080C765739F}"
         ParseReplacements=   0   'False
         AppendedParams  =   ""
         HasTempTemplate =   0   'False
         UsesRelativePath=   -1  'True
         OriginalTemplate=   "C:\Inetpub\wwwroot\ChemOffice\webserver_source\cfserveradmin\AdminSource\editor\WebAdminHelp.htm"
         TagPrefixInfo   =   2
         BeginProperty Events {193556D1-4486-11D1-9C70-00C04FB987DF} 
            EventCount      =   0
         EndProperty
         BeginProperty BoundTags {FA6A55FA-458A-11D1-9C71-00C04FB987DF} 
            AttribCount     =   0
         EndProperty
      EndProperty
   EndProperty
   NameInURL       =   "WebAdminUtil"
End
Attribute VB_Name = "WebAdminUtil"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True




Private Sub WebAdminHelp_ProcessTag(ByVal TagName As String, TagContents As String, SendTags As Boolean)
 topic = UCase(Request.QueryString("topic"))

Select Case TagName
    Case "WC@HELP_TEXT"
        Select Case topic
            Case "WEB_APP_NAME"
                TagContents = "Enter a short name for the application.  This name entered becomes a virtual directory where all the web application files and converted chemfinder form files reside."
            Case "CFW_FORM"
                TagContents = "Enter the full file path to the ChemFinder form you are converting. Also, specify whether the ChemFinder form being converted is linked to a reaction database or a structure database."
            Case "NUM_LIST_VIEW"
                TagContents = "Specify the number of records displayed in list view when results are returned.  Values greater then 5 tend to be slower, if structure, mw or formula are selected for display in list view. "
            Case "RUN_CONVERT"
                TagContents = "Complete the conversion of the current ChemFinder form and add the web enabled forms to the web application."
            Case "CONVERT_ANOTHER"
                TagContents = "Allows you specify another ChemFinder form to convert and add to the same web application."
            Case "ADMIN_OPTIONS"
                help_text = "To convert a ChemFinder form and add it to an existing ChemOffice Web Application, choice the Web Application from the drop down list and click ""Add Data View to Web Application."""
                help_text = help_text & "Global searching across the multiple views is enable automaticlaly when an applicaiton has multiple data views."
                help_text = help_text & "<br><br>" & "To delete a web application click ""Delete Web Application."""
                help_text = help_text & "<br><br>" & "To create a new ChemOffice Web Application and convert a ChemFinder form click ""Create New Web Application."""
                TagContents = help_text
        End Select
    End Select

End Sub


Private Sub WebClass_Start()
UtilType = Request.QueryString("type")

    Select Case UtilType
        Case "help"
            WebAdminHelp.WriteTemplate
    End Select


End Sub

