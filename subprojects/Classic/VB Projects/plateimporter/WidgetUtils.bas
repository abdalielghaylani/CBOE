Attribute VB_Name = "UtilsWidget"
Option Explicit

' contains helper functions for dealing with user-interface widgets

Private Declare Function SetWindowPos Lib "user32" _
          (ByVal hWnd As Long, _
           ByVal hWndInsertAfter As Long, _
           ByVal X As Long, ByVal Y As Long, _
           ByVal cx As Long, _
           ByVal cy As Long, _
           ByVal wFlags As Long) As Long

Const HWND_TOP As Long = &H0
Const SWP_NOZORDER As Long = &H4
Const SWP_NOSIZE As Long = &H1
Const SWP_NOMOVE As Long = &H2


Public Enum CheckBoxVals
    chkFalse = 0
    chkTrue
    chkDimmed
End Enum

Public Const gcintCmbIndexNull As Integer = -1

Public Sub BringFormToFront(ByRef oFrm As Form)
    Dim bResult As Long
    oFrm.WindowState = vbNormal
    bResult = SetWindowPos(oFrm.hWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOZORDER Or SWP_NOSIZE Or SWP_NOMOVE)
End Sub

Public Function CheckBoxDimmed(intCheckValue As Integer) As Boolean
    CheckBoxDimmed = (intCheckValue = CheckBoxVals.chkDimmed)
End Function

Public Function CheckBoxBoolean(intCheckValue As Integer) As Boolean
    Dim bRet As Boolean: bRet = False
    Select Case intCheckValue
        Case chkTrue
            bRet = True
        Case chkFalse
            bRet = False
        Case Else
            bRet = False
    End Select
    CheckBoxBoolean = bRet
End Function

Public Sub ComboboxKeypress(ByRef cmb As ComboBox, KeyAscii As Integer)
    Dim intX As Integer
    With cmb
        .ListIndex = gcintCmbIndexNull
        For intX = 0 To .ListCount - 1
            If Len(.list(intX)) = 0 Then
                If KeyAscii = Asc(" ") Then
                    .ListIndex = intX
                    Exit For
                End If
            ElseIf Asc(UCase(Left(.list(intX), 1))) = KeyAscii Then
                .ListIndex = intX
                Exit For
            ElseIf Asc(LCase(Left(.list(intX), 1))) = KeyAscii Then
                .ListIndex = intX
                Exit For
            End If
        Next intX
    End With
End Sub

Public Sub ComboRemoveItemData(ByRef Combo As ComboBox, ByVal ID As Long)
    Dim i As Integer, lRem As Long
    lRem = NULL_AS_LONG
    For i = 0 To Combo.ListCount - 1
        If Combo.ItemData(i) = ID Then
            lRem = i
            Exit For
        End If
    Next
    If lRem <> NULL_AS_LONG Then
        Combo.RemoveItem lRem
    End If
End Sub

Public Sub ComboFill(ByRef Combo As ComboBox, ByRef dict As Dictionary, Optional ByVal SelectFirst As Boolean = True)
    ' fill combo box - keys get put in ItemData, values as combo values
    Dim aAr As Variant
    If Not dict Is Nothing Then
        aAr = dict.Keys
        Combo.Clear
        Dim i As Integer
        For i = LBound(aAr) To UBound(aAr)
            Combo.AddItem CnvString(dict.Item(aAr(i)), eDBtoVB), Combo.ListCount    ' add at last position
            Combo.ItemData(Combo.ListCount - 1) = aAr(i)
        Next
        If SelectFirst And Combo.ListCount > 0 Then
            Combo.ListIndex = 0
        End If
    End If
End Sub

Public Sub ListFill(ByRef list As ListBox, ByRef dict As Dictionary, Optional ByVal SelectFirst As Boolean = True)
    Dim aAr As Variant
    list.Clear
    If Not dict Is Nothing Then
        aAr = dict.Keys
        Dim i As Integer
        For i = LBound(aAr) To UBound(aAr)
            If aAr(i) <> "" Then
                list.AddItem dict.Item(aAr(i)), list.ListCount    ' add at last position
                list.ItemData(list.ListCount - 1) = aAr(i)
            End If
        Next
        If SelectFirst And list.ListCount > 0 Then
            list.ListIndex = 0
        End If
    End If
End Sub

Public Function ComboSelect(ByRef cmb As ComboBox, _
        strItem As String, Optional blnReset As Boolean = True) As Boolean
    ' looks for strItem among list-items in cmb, _
            sets active item to any match, and _
            returns true if found
    Dim intX As Integer
    
    ComboSelect = False
    With cmb
        If blnReset Then
            .ListIndex = gcintCmbIndexNull
        End If
        If Len(strItem) > 0 Then
            For intX = 0 To .ListCount - 1
                If StrMatch(.list(intX), strItem) Then
                    ComboSelect = True
                    .ListIndex = intX
                    Exit For
                End If
            Next intX
        End If
    End With
End Function

Public Function ComboSelectItemData(ByRef cmb As ComboBox, ByVal lItemData As Long) As Boolean
    ComboSelectItemData = False
    Dim i As Integer
    With cmb
        For i = 0 To .ListCount - 1
            If .ItemData(i) = lItemData Then
                ComboSelectItemData = True
                .ListIndex = i
                Exit For
            End If
        Next
    End With
End Function

Public Function ListSelectItemData(ByRef list As ListBox, ByVal lItemData As Long) As Boolean
    ' select (potentially multiple) items in a listbox
    ListSelectItemData = False
    Dim i As Integer
    With list
        For i = 0 To .ListCount - 1
            If .ItemData(i) = lItemData Then
                .Selected(i) = True
            End If
        Next
    End With
End Function

Public Function ListSelectItem(ByRef list As ListBox, ByVal sItem As String) As Boolean
    ' select (potentially multiple) items in a listbox
    ListSelectItem = False
    Dim i As Integer
    With list
        For i = 0 To .ListCount - 1
            If .list(i) = sItem Then
                .Selected(i) = True
                ListSelectItem = True
            End If
        Next
    End With
End Function


Public Function ComboItemData(ByRef cmb As ComboBox) As String
    ' return item data of currently selected combo item
    If cmb.ListIndex = NULL_AS_LONG Then
        ComboItemData = NULL_AS_LONG
    Else
        ComboItemData = cmb.ItemData(cmb.ListIndex)
    End If
End Function

Public Function ListItemData(ByRef lst As ListBox) As String
    ' return item data of currently selected list item
    If lst.ListIndex = NULL_AS_LONG Then
        ListItemData = NULL_AS_LONG
    Else
        ListItemData = lst.ItemData(lst.ListIndex)
    End If
End Function

Public Function ListItemString(ByRef lst As ListBox) As String
    ' return display string of currently selected list item
    If lst.ListIndex = NULL_AS_LONG Then
        ListItemString = ""
    Else
        ListItemString = lst.list(lst.ListIndex)
    End If
End Function

Public Function ComboItemString(ByRef cmb As ComboBox) As String
    ' return display string of currently selected combo item
    ComboItemString = cmb.list(cmb.ListIndex)
End Function

Public Function ListGetSelectedItemData(ByRef list As ListBox) As String
    Dim i As Integer
    Dim sRet As String: sRet = ""
    Dim bFirst As Boolean: bFirst = True
    For i = 0 To list.ListCount - 1
        If list.Selected(i) Then
            If Not bFirst Then
                sRet = sRet & ","
            Else
                bFirst = False
            End If
            sRet = sRet & list.ItemData(i)
        End If
    Next
    ListGetSelectedItemData = sRet
End Function

Public Function Bool2Checkbox(bln As Boolean) As CheckBoxVals
    If bln Then
        Bool2Checkbox = CheckBoxVals.chkTrue
    Else
        Bool2Checkbox = CheckBoxVals.chkFalse
    End If
End Function

Public Sub WizStopNext(ByRef oWiz As ActiveWizard, ByVal OldPane As Integer, ByVal ErrMsg As String)
    oWiz.ViewPane OldPane
    DoEvents
    MsgBox ErrMsg, vbExclamation
End Sub

Public Function GridBuildComboList(ByRef dict As Dictionary) As String
    ' return string for colcombolist from dictionary
    Dim vkey As Variant
    Dim sRet As String
    Dim bFirst As Boolean: bFirst = True
    If Not dict Is Nothing Then
        For Each vkey In dict.Keys
            If Not bFirst Then
                sRet = sRet & "|"
            End If
            sRet = sRet & "#" & vkey & ";" & dict(vkey)
            bFirst = False
        Next
        GridBuildComboList = sRet
    End If
End Function

Public Function GridBuildComboList2(ByVal indexList As String, ByVal valueList As String) As String
    ' return string for colcombolist from 2 ";" delimited string
    Dim vkey As Variant
    Dim sRet As String
    Dim arrIndex, arrValue As Variant
    Dim i As Integer
    Dim bFirst As Boolean: bFirst = True
    If InStr(indexList, ";") > 0 Then
        arrIndex = Split(indexList, ";")
        arrValue = Split(valueList, ";")
        For i = 0 To UBound(arrIndex)
            If bFirst Then
                bFirst = False
            Else
                sRet = sRet & "|"
            End If
            sRet = sRet & "#" & arrIndex(i) & ";" & arrValue(i)
        Next
    Else
        sRet = "#" & indexList & ";" & valueList
    End If
    GridBuildComboList2 = sRet
End Function

Public Function PicklistFromXML(ByVal sXML As String, ByRef sList As String, Optional ByVal bCombo As Boolean = False) As String
    Dim oDoc As DOMDocument40
    Dim oNodeList As IXMLDOMNodeList
    Dim oDocNode As IXMLDOMElement
    Dim oNode As IXMLDOMElement
    Dim sEntry As String
    Dim sID As String
    Dim bFirst As Boolean

    On Error GoTo Error
    If bCombo Then
        sList = "|"
    Else
        sList = ""
    End If
    Set oDoc = New DOMDocument40
    oDoc.loadXML (sXML)
    If oDoc.parseError.errorCode <> 0 Then
        PicklistFromXML = oDoc.parseError.reason
        Exit Function
    End If
    'CSBR-117485:Invloader crashes if the Notebook dropdown is selected.
    'This bug occurs because the code was not written to handle empty
    'list entries
    'JB, 01-FEB-2010
    If oDoc.documentElement.Text <> "" Then
        Set oDocNode = oDoc.documentElement
        If oDocNode Is Nothing Then
            PicklistFromXML = "No parent node found"
            Exit Function
        End If
        
        Set oNodeList = oDocNode.getElementsByTagName("PicklistItem")
        bFirst = True
        For Each oNode In oNodeList
            sEntry = oNode.Text
            sID = oNode.getAttribute("ID")
            If bFirst Then
                bFirst = False
            Else
                sList = sList & "|"
            End If
            sList = sList & "#" & sID & ";" & sEntry
        Next oNode
    Else
        sList = "#;<No Data>"
    End If
    
    PicklistFromXML = ""
    
    Exit Function
Error:
    sList = ""
    PicklistFromXML = Err.Description
End Function

Public Sub GridFormatGeneric(ByRef aGrd As VSFlexGrid)
    ' generic formatting
    With aGrd
        .Clear
        .DataMode = flexDMFree
        .Editable = flexEDKbdMouse
        .TabBehavior = flexTabCells
        .ExtendLastCol = True
        .FixedRows = 1
        .FixedCols = 1
        .FrozenCols = 0
        .FrozenRows = 0
        .FocusRect = flexFocusSolid
        .ExtendLastCol = False
        .AllowUserResizing = flexResizeColumns
        .GridLines = flexGridFlat
        ' set font
        Dim oFont As New StdFont
        With oFont
            .Name = "Tahoma"
            .Size = 8
        End With
        Set .Font = oFont
    End With
End Sub

