Attribute VB_Name = "ObjectTagUtils"
Option Explicit

' ## Utility code for implementing ChemDraw stoichiometry via Object Tags

Const mMaxDataSgroups As Long = 5
Const otNameRoot As String = "StoredDataSgroup"
Const StoiFieldName As String = "STOICHIOMETRY"

Function GetStoiCoeff(ByVal g As ChemDrawControl9Ctl.Group) As String
    Dim ot As ChemDrawControl9Ctl.ObjectTag
    Dim tString As String
    Dim tArray() As String
    Dim i As Long
    Dim otName As String
    
    
    ' Find Data Sgroup data in ObjectTags.
    GetStoiCoeff = "" ' Default (for not found case)
    For i = 1 To mMaxDataSgroups
        otName = otNameRoot & CStr(i)
        Set ot = g.GetObjectTag(otName)
        If Not ot Is Nothing Then
            tString = ot.StringValue
            tArray = Split(tString, "|", -1, vbTextCompare)
            ' StringValue format: String data delimited by vertical bars:
            '   <data for each data sgroup> = <Field>|<Data>|<Left>|<Top>|<TagAlignment>
            If (StrComp(tArray(0), StoiFieldName, vbTextCompare) = 0) Then
                GetStoiCoeff = tArray(1) ' This <Data> is the stiochiometric coefficient.
                Exit For
            End If
        End If
    Next i
End Function

Sub GetBoxFromAtomsOT(ByVal g As ChemDrawControl9Ctl.Group, ByRef Left As Double, _
              ByRef Top As Double, ByRef Right As Double, ByRef Bottom As Double)
    Dim TotalAtoms As Long
    Dim atomID As Long
    Dim i As Long
    Dim x As Double
    Dim y As Double
    Dim z As Double
              
    Left = 0# ' Initialize output parameters.
    Top = 0#
    Right = 0#
    Bottom = 0#
    
    For i = 1 To g.Atoms.Count
        x = g.Atoms(i).Position.x
        y = g.Atoms(i).Position.y
        If i = 1 Then
            Left = x
            Right = x
            Top = y
            Bottom = y
        Else
            If x < Left Then Left = x
            If x > Right Then Right = x
            If y < Top Then Top = y
            If y > Bottom Then Bottom = y
        End If
    Next i
End Sub


Function PutStoiCoeff(ByVal g As ChemDrawControl9Ctl.Group, ByVal Coefficient As String, _
                        ByVal cdc As ChemDrawControl9Ctl.ChemDrawCtl)
    Dim ot As ChemDrawControl9Ctl.ObjectTag
    Dim otVis As ChemDrawControl9Ctl.ObjectTag
    Dim tString As String
    Dim tArray() As String
    Dim i As Long
    Dim j As Long
    Dim otName As String
    Dim TagExists As Boolean
    Dim val As Variant
    Dim a As ChemDrawControl9Ctl.Atom
    Dim LeftMostAtom As Long
    Dim LeftX As Double
    Dim pt As ChemDrawControl9Ctl.Point
    Dim MySettings As ChemDrawControl9Ctl.Settings
    
    If CDbl(Coefficient) > 1# Then
        ' Look for an existing Stoichiometry Object Tag.
        TagExists = False
        For i = 1 To mMaxDataSgroups
            otName = otNameRoot & CStr(i)
            Set ot = g.GetObjectTag(otName)
            If Not ot Is Nothing Then
                tString = ot.StringValue
                tArray = Split(tString, "|", -1, vbTextCompare)
                ' StringValue format: String data delimited by vertical bars:
                '   <data for each data sgroup> = <Field>|<Data>|<Left>|<Top>|<TagAlignment>
                If (StrComp(tArray(0), StoiFieldName, vbTextCompare) = 0) Then
                    ' We found an existing Stoichiometry Object Tag.
                    TagExists = True
                    If tArray(1) <> Coefficient Then            ' changed?
                        ' Store the new coefficient.
                        ot.StringValue = tArray(0) & "|" & Coefficient & "|" & tArray(2) & "|" & tArray(3) & "|" & tArray(4)
                        ot.Caption.Text = Coefficient
                    End If
                End If
                Exit For
            End If
        Next i
        
        If Not TagExists Then
            ' Make a new tag and set it's position.
            For i = 1 To mMaxDataSgroups
                otName = otNameRoot & CStr(i)
                Call g.MakeObjectTag(otName, ot, True) ' Create the Object Tag.
                If Not ot Is Nothing Then
    
                    ' Make another one to work around CSBR-56268.  Since relative positioning of
                    ' Object tags on atoms *does* work, we pick an atom, and position our object tag on it.
                    'Pick the leftmost atom.
                    For j = 1 To g.Atoms.Count
                        If j = 1 Then
                            LeftMostAtom = j
                            LeftX = g.Atoms(j).Position.x
                        Else
                            If g.Atoms(j).Position.x < LeftX Then
                                LeftMostAtom = j
                                LeftX = g.Atoms(j).Position.x
                            End If
                        End If
                    Next j
                    Set a = g.Atoms(LeftMostAtom)
                    Call a.MakeObjectTag(otName & "Visible", otVis, True)
                    otVis.PositioningType = kCDPositioningTypeOffset
                    otVis.Caption.Color = 1
                    otVis.Caption.Text = Coefficient
                    otVis.Caption.Settings.CaptionJustification = kCDJustificationRight
                    
                    Dim DX As Double
                    Dim DY As Double
                    Dim NewX As Double
                    Dim NewY As Double
                    Dim OldX As Double
                    Dim OldY As Double
                    Dim GroupWidth As Double
                    Dim otWidth As Double
                    Dim otHeight As Double
                    ' 20 is a scale factor to take into account different ChemDraw units used.
                    ' (suggested by Coh)
                    ' (Per JBrecher, the different units are bugs.)
                    ' The ot.Bounds.Right is to right-justify the text.
                    otWidth = (otVis.Bounds.Right - otVis.Bounds.Left) / 20#
                    otHeight = (otVis.Bounds.Bottom - otVis.Bounds.Top) / 20#
                    
                    Set pt = otVis.PositioningOffset
                    pt.x = (-0.75 * otWidth) * 2 '+ (g.Bounds.Left / 20# - LeftX) / 2
'                    pt.x = (-0.75 * otWidth) * 4
                    pt.y = -(otHeight / 2) * 1.5
'                    pt.y = -(otHeight / 2) * 3
                    pt.z = 0
                    otVis.PositioningOffset = pt
                    
    '                 Note: ChemDraw needs the code above.  This does not work:
    '                   otVis.PositioningOffset.X = 1.5 * otWidth
    '                   otVis.PositioningOffset.Y = otHeight / 2
    '                   otVis.PositioningOffset.Z = 0#
                    
                    ' Load Object Tag data into Object Tag.
                    
                    ' Object Tag coordinates are in a molecule-relative coordinate system.
                    '   The origin is the upper left of the molecule, and
                    '   the point (1,1) is the lower right of the molecule.
                    
                    Dim Left As Double
                    Dim Top As Double
                    Dim Right As Double
                    Dim Bottom As Double
                    Dim Width As Double
                    Dim Height As Double
                    Dim LeftPos As Double
                    Dim TopPos As Double
                    
                    If g.Atoms.Count > 1 Then
                        Call GetBoxFromAtomsOT(g, Left, Top, Right, Bottom)
                        Width = Right - Left
                        If Width > 0# Then
                            LeftPos = (pt.x / Width) * 2
                        Else
                            LeftPos = 0#
                        End If
                        Height = Bottom - Top
                        If Height > 0# Then
                            TopPos = pt.y / Height + 0.35 ' Center vertically.
                            'TopPos = pt.y / Height + 0.5 ' Center vertically.
                        Else
                            TopPos = 0#
                        End If
                    Else
                        ' We can't use GetBoxFromAtomsOT because atoms are just points.
                        Left = g.Bounds.Left / 20#
                        Top = g.Bounds.Top / 20#
                        Right = g.Bounds.Right / 20#
                        Bottom = g.Bounds.Bottom / 20#
                        Width = Right - Left
                        If Width > 0# Then
                            LeftPos = (pt.x / Width)  'LeftPos = (pt.x / Width) / 2# ' The 2 is a tweak to move it closer
                        Else
                            LeftPos = 0#
                        End If
                        Height = Bottom - Top
                        If Height > 0# Then
                            TopPos = pt.y / Height
                        Else
                            TopPos = 0#
                        End If
                    End If
                    
                    tString = StoiFieldName & "|"
                    tString = tString & Coefficient & "|"
                    tString = tString & CStr(Round(LeftPos, 3)) & "|"
                    tString = tString & CStr(Round(TopPos, 3)) & "|"
                    tString = tString & "1"
                    ot.StringValue = tString
                    
                    Exit For
                End If
            Next i
        End If
    End If
End Function
