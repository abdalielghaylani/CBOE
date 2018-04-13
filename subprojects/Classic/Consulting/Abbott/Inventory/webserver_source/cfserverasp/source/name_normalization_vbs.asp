<%' Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
'DO NOT EDIT THIS FILE
'This file contains name normalization functions%>
<%
'***************************************************************************
'Clean name query to match the format of cleanfield in database
'Input: thesyns
'Output: cleaned thesyns
'***************************************************************************
Function CleanTheSyns(ByRef thesyns)
	Dim offset
	Dim allnewsyns
	Dim doloop
	
	'if Left(thesyns, 1) = "=" then
		'thesyns = Right(thesyns, Len(thesyns) - 1) ' get rid of "=" at the beginning
	'end if
	
	thesyns = "(" & LCase(thesyns)

	offset = 1
	'Debug.Print thesyns
	Do While offset <= Len(thesyns)
	    'Debug.Print offset
	    Select Case (Mid(thesyns, offset, 1))
	    Case ".", ")", "[", "]", "{", "}", "/", "_", ":", """"
	        offset = offset + 1
	    Case " "
	        If Mid(thesyns, offset, 10) = " potassium" Then
	            newsyns = "potassium" & newsyns
	            offset = offset + 10
	        ElseIf Mid(thesyns, offset, 7) = " sodium" Then
	            newsyns = "sobium" & newsyns
	            offset = offset + 7
	        ElseIf Mid(thesyns, offset, 5) = " and " Then
	            newsyns = newsyns & "+"
	            offset = offset + 5
	        ElseIf Mid(thesyns, offset, 4) = " na+" Then
	            newsyns = "sobium" & newsyns
	            offset = offset + 4
	        ElseIf Mid(thesyns, offset, 3) = " k+" Then
	            newsyns = "potassium" & newsyns
	            offset = offset + 3
	        Else
	            offset = offset + 1
	        End If
	    Case "`"
	        newsyns = newsyns & "'"
	        offset = offset + 1
	    Case "&"
	        newsyns = newsyns & "+"
	        offset = offset + 1
	    'Case "*"
	    '    newsyns = newsyns & "%"
	    '    offset = offset + 1
	    Case ","
	        If Mid(thesyns, offset, 3) = ",0'" Then
	            newsyns = newsyns & ",O'"
	            offset = offset + 3
	        Else
	            offset = offset + 1
	        End If
	    Case "-"
	        If Mid(thesyns, offset, 3) = "-0-" Then
	            newsyns = newsyns & "-O-"
	            offset = offset + 3
	        Else
	            offset = offset + 1
	        End If
	    Case ";"
	        If Mid(thesyns, offset, 2) = "; " Then
	            If allnewsyns <> "" Then allnewsyns = allnewsyns & "; "
	            allnewsyns = allnewsyns & newsyns
	            newsyns = ""
	            offset = offset + 2
	        Else
	            newsyns = newsyns & ";"
	            offset = offset + 1
	        End If
	    Case "0"
	        If Mid(thesyns, offset, 3) = "0,0" Then
	            newsyns = newsyns & "22"
	            offset = offset + 3
	        Else
	            newsyns = newsyns & "0"
	            offset = offset + 1
	        End If
	    Case "1"
	        If Mid(thesyns, offset, 2) = "1+" Then
	            newsyns = newsyns & "i"
	            offset = offset + 2
	        Else
	            newsyns = newsyns & "1"
	            offset = offset + 1
	        End If
	    Case "2"
	        If Mid(thesyns, offset, 4) = "2hcl" Then
	            newsyns = newsyns & "bihydrochlorid"
	            offset = offset + 4
	        ElseIf Mid(thesyns, offset, 2) = "2+" Then
	            newsyns = newsyns & "ii"
	            offset = offset + 2
	        Else
	            newsyns = newsyns & "2"
	            offset = offset + 1
	        End If
	    Case "3"
	        If Mid(thesyns, offset, 2) = "3+" Then
	            newsyns = newsyns & "iii"
	            offset = offset + 2
	        Else
	            newsyns = newsyns & "3"
	            offset = offset + 1
	        End If
	    Case "4"
	        If Mid(thesyns, offset, 2) = "4+" Then
	            newsyns = newsyns & "iv"
	            offset = offset + 2
	        Else
	            newsyns = newsyns & "4"
	            offset = offset + 1
	        End If
	    Case "5"
	        If Mid(thesyns, offset, 2) = "5+" Then
	            newsyns = newsyns & "v"
	            offset = offset + 2
	        Else
	            newsyns = newsyns & "5"
	            offset = offset + 1
	        End If
	    Case "6"
	        If Mid(thesyns, offset, 2) = "6+" Then
	            newsyns = newsyns & "vi"
	            offset = offset + 2
	        Else
	            newsyns = newsyns & "6"
	            offset = offset + 1
	        End If
	    Case "("
	        If Mid(thesyns, offset, 3) = "(-)" Then
	            newsyns = newsyns & "-"
	            offset = offset + 3
	        Else
	            offset = offset + 1
	        End If
	    Case "#"
	        newsyns = newsyns & "no"
	        offset = offset + 1
	    Case "+"
	        If Mid(thesyns, offset, 3) = "+/-" Or Mid(thesyns, offset, 3) = "+,-" Then
	            newsyns = newsyns & "+-"
	            offset = offset + 3
	        ElseIf Mid(thesyns, offset, 2) = "+-" Then
	            newsyns = newsyns & "+-"
	            offset = offset + 2
	        Else
	            newsyns = newsyns & "+"
	            offset = offset + 1
	        End If
	    Case "a"
	        If Mid(thesyns, offset, 12) = "antimony tri" Then
	            newsyns = newsyns & "antimonyiii"
	            offset = offset + 12
	        ElseIf Mid(thesyns, offset, 11) = "antimonious" Then
	            newsyns = newsyns & "antimonyiii"
	            offset = offset + 11
	        ElseIf Mid(thesyns, offset, 10) = "antimonous" Or Mid(thesyns, offset, 10) = "antimonius" Then
	            newsyns = newsyns & "antimonyiii"
	            offset = offset + 10
	        ElseIf Mid(thesyns, offset, 9) = "aluminium" Then
	            newsyns = newsyns & "aluminum"
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 9) = "anhydrous" Then
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 9) = "argentous" Then
	            newsyns = newsyns & "silveri"
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 9) = "arsenious" Then
	            newsyns = newsyns & "arseniciii"
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 9) = "anisidine" Then
	            newsyns = newsyns & "methoxybenzenamin"
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 8) = "arsenous" Then
	            newsyns = newsyns & "arseniciii"
	            offset = offset + 8
	        ElseIf Mid(thesyns, offset, 7) = "aniline" Then
	            newsyns = newsyns & "benzenamin"
	            offset = offset + 7
	        ElseIf Mid(thesyns, offset, 6) = "ammine" Then
	            newsyns = newsyns & "amin"
	            offset = offset + 6
	        ElseIf Mid(thesyns, offset, 5) = "alpha" Then
	            newsyns = newsyns & "a"
	            offset = offset + 5
	        ElseIf Mid(thesyns, offset, 4) = "amyl" Then
	            newsyns = newsyns & "pent"
	            offset = offset + 2
	        ElseIf Mid(thesyns, offset, 3) = "ate" Then
	            newsyns = FixIcAte(newsyns) & "a"
	            offset = offset + 1
	        Else
	            newsyns = newsyns & "a"
	            offset = offset + 1
	        End If
	    Case "b"
	        If Mid(thesyns, offset, 12) = "benzeneamine" Then
	            newsyns = newsyns & "benzenamin"
	            offset = offset + 12
	        ElseIf Mid(thesyns, offset, 11) = "bismuth tri" Then
	            newsyns = newsyns & "bismuthiii"
	            offset = offset + 11
	        ElseIf Mid(thesyns, offset, 5) = "bromo" Then
	            newsyns = newsyns & "brom"
	            offset = offset + 5
	        ElseIf Mid(thesyns, offset, 4) = "beta" Then
	            newsyns = newsyns & "b"
	            offset = offset + 4
	        ElseIf Mid(thesyns, offset, 3) = "bis" Then
	            newsyns = newsyns & "bi"
	            offset = offset + 3
	        Else
	            newsyns = newsyns & "b"
	            offset = offset + 1
	        End If
	    Case "c"
	        If Mid(thesyns, offset, 21) = "carbamodithioato-S,S'" Then
	            newsyns = newsyns & "bithiocarbamato"
	            offset = offset + 21
	        ElseIf Mid(thesyns, offset, 9) = "columbium" Then
	            newsyns = newsyns & "niobium"
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 9) = "cobaltous" Then
	            newsyns = newsyns & "cobaltii"
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 9) = "carbamoyl" Then
	            newsyns = newsyns & "carbamyl"
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 7) = "complex" Then
	            offset = offset + 7
	        ElseIf Mid(thesyns, offset, 7) = "chromic" Then
	            newsyns = newsyns & "chromiumiii"
	            offset = offset + 7
	        ElseIf Mid(thesyns, offset, 7) = "caesium" Then
	            newsyns = newsyns & "cesium"
	            offset = offset + 7
	        ElseIf Mid(thesyns, offset, 7) = "cuprous" Then
	            newsyns = newsyns & "copperi"
	            offset = offset + 7
	        ElseIf Mid(thesyns, offset, 7) = "capryl " Then
	            newsyns = newsyns & "octyl"
	            offset = offset + 6
	        ElseIf Mid(thesyns, offset, 6) = "chloro" Then
	            newsyns = newsyns & "chlor"
	            offset = offset + 6
	        ElseIf Mid(thesyns, offset, 6) = "capryl" Then
	            newsyns = newsyns & "octano"
	            offset = offset + 6
	        ElseIf Mid(thesyns, offset, 6) = "cresol" Then
	            newsyns = newsyns & "methylphenol"
	            offset = offset + 6
	        ElseIf Mid(thesyns, offset, 6) = "cupric" Then
	            newsyns = newsyns & "copperii"
	            offset = offset + 6
	        ElseIf Mid(thesyns, offset, 6) = "cerous" Then
	            newsyns = newsyns & "ceriumiii"
	            offset = offset + 6
	        ElseIf Mid(thesyns, offset, 5) = "choro" Then
	            newsyns = newsyns & "chlor"
	            offset = offset + 5
	        ElseIf Mid(thesyns, offset, 5) = "ceric" Then
	            If Mid(thesyns, offset - 1, 1) = "y" Then
	                newsyns = newsyns & "cer"
	                offset = offset + 3
	            Else
	                newsyns = newsyns & "ceriumiv"
	                offset = offset + 5
	            End If
	        Else
	            newsyns = newsyns & "c"
	            offset = offset + 1
	        End If
	    Case "d"
	        If Mid(thesyns, offset, 13) = "dodecahydrate" Then
	            newsyns = newsyns & "12h2o"
	            offset = offset + 13
	        ElseIf Mid(thesyns, offset, 11) = "decahydrate" Then
	            newsyns = newsyns & "10h2o"
	            offset = offset + 11
	        ElseIf Mid(thesyns, offset, 10) = "derivative" Then
	            offset = offset + 10
	        ElseIf Mid(thesyns, offset, 9) = "dihydrate" Then
	            newsyns = newsyns & "2h2o"
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 6) = "dextro" Then
	            newsyns = newsyns & "d"
	            offset = offset + 6
	        ElseIf Mid(thesyns, offset, 5) = "delta" Then
	            newsyns = newsyns & "d"
	            offset = offset + 5
	        ElseIf Mid(thesyns, offset, 3) = "des" And Mid(thesyns, offset) <> "des" Then
	            newsyns = newsyns & "de"
	            offset = offset + 3
	        ElseIf Mid(thesyns, offset, 2) = "di" Then
	            newsyns = newsyns & "bi"
	            offset = offset + 2
	        Else
	            newsyns = newsyns & "d"
	            offset = offset + 1
	        End If
	    Case "e"
	        If Mid(thesyns, offset, 12) = "enneahydrate" Then
	            newsyns = newsyns & "9h2o"
	            offset = offset + 12
	        ElseIf Mid(thesyns, offset, 7) = "enathyl" Then
	            newsyns = newsyns & "heptyl"
	            offset = offset + 5
	        ElseIf Mid(thesyns, offset, 4) = "eico" Then
	            newsyns = newsyns & "ico"
	            offset = offset + 4
	        ElseIf Mid(thesyns, offset, 4) = "ehty" Then
	            newsyns = newsyns & "ethy"
	            offset = offset + 4
	        ElseIf Len(thesyns) > 6 Then
	            If Mid(thesyns, offset, 2) = "e" Then
	                If InStr("abcdefghijklmnopqrstuvwxyz", Mid(thesyns, offset - 1, 1)) > 0 Then
	                    offset = offset + 1
	                Else
	                    newsyns = newsyns & "e"
	                    offset = offset + 1
	                End If
	            Else
	                If InStr("1234567890-()+[]}{':,./ *%", Mid(thesyns, offset + 1, 1)) > 0 Then
	                    If offset = 1 Then
	                        newsyns = newsyns & "e"
	                        offset = offset + 1
	                    Else
	                        If InStr("abcdefghijklmnopqrstuvwxyz", Mid(thesyns, offset - 1, 1)) > 0 Then
	                            offset = offset + 1
	                        Else
	                            newsyns = newsyns & "e"
	                            offset = offset + 1
	                        End If
	                    End If
	                Else
	                    newsyns = newsyns & "e"
	                    offset = offset + 1
	                End If
	            End If
	        Else
	            newsyns = newsyns & "e"
	            offset = offset + 1
	        End If
	    Case "f"
	        If Mid(thesyns, offset, 7) = "ferrous" Then
	            newsyns = newsyns & "ironii"
	            offset = offset + 7
	        ElseIf Mid(thesyns, offset, 6) = "ferric" Then
	            newsyns = newsyns & "ironiii"
	            offset = offset + 6
	        ElseIf Mid(thesyns, offset, 6) = "fluoro" Then
	            newsyns = newsyns & "fluor"
	            offset = offset + 6
	        ElseIf Mid(thesyns, offset, 6) = "flouro" Then
	            newsyns = newsyns & "fluor"
	            offset = offset + 6
	        ElseIf Mid(thesyns, offset, 5) = "flour" Then
	            newsyns = newsyns & "fluor"
	            offset = offset + 5
	        Else
	            newsyns = newsyns & "f"
	            offset = offset + 1
	        End If
	    Case "g"
	        If Mid(thesyns, offset, 5) = "gamma" Then
	            newsyns = newsyns & "y"
	            offset = offset + 5
	        Else
	            newsyns = newsyns & "g"
	            offset = offset + 1
	        End If
	    Case "h"
	        If Mid(thesyns, offset, 16) = "hemipentahydrate" Then
	            newsyns = newsyns & "2.5h2o"
	            offset = offset + 16
	        ElseIf Mid(thesyns, offset, 12) = "heptahydrate" Then
	            newsyns = newsyns & "7h2o"
	            offset = offset + 12
	        ElseIf Mid(thesyns, offset, 11) = "hemihydrate" Then
	            newsyns = newsyns & "1/2h2o"
	            offset = offset + 11
	        ElseIf Mid(thesyns, offset, 11) = "hexahydrate" Then
	            newsyns = newsyns & "6h2o"
	            offset = offset + 11
	        ElseIf Mid(thesyns, offset, 9) = "hexadecyl" Then
	            newsyns = newsyns & "cetyl"
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 7) = "hydrate" Then
	            newsyns = newsyns & "h2o"
	            offset = offset + 7
	        ElseIf Mid(thesyns, offset, 4) = "hyro" Then
	            newsyns = newsyns & "hydro"
	            offset = offset + 4
	        ElseIf Mid(thesyns, offset, 3) = "hcl" Then
	            newsyns = newsyns & "hydrochlorid"
	            offset = offset + 3
	        ElseIf Mid(thesyns, offset, 3) = "hbr" Then
	            newsyns = newsyns & "hydrobromid"
	            offset = offset + 3
	        Else
	            newsyns = newsyns & "h"
	            offset = offset + 1
	        End If
	    Case "i"
	        If Mid(thesyns, offset, 7) = "ic acid" Then
	            If InStr(newsyns, "yl") Then
	                newsyns = newsyns & "icacid"
	                offset = offset + 7
	            Else
	                newsyns = FixIcAte(newsyns) & "at"
	                offset = offset + 7
	            End If
	        ElseIf Mid(thesyns, offset, 6) = "icacid" Then
	            newsyns = FixIcAte(newsyns) & "at"
	            offset = offset + 6
	        Else
	            newsyns = newsyns & "i"
	            offset = offset + 1
	        End If
	    Case "l"
	        If Mid(thesyns, offset, 4) = "levo" Then
	            newsyns = newsyns & "l"
	            offset = offset + 4
	        Else
	            newsyns = newsyns & "l"
	            offset = offset + 1
	        End If
	    Case "m"
	        If Mid(thesyns, offset, 9) = "mercaptan" Then
	            newsyns = newsyns & "thiol"
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 9) = "manganous" Then
	            newsyns = newsyns & "manganesii"
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 9) = "mercurous" Then
	            newsyns = newsyns & "mercuryi"
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 8) = "mercuric" Then
	            newsyns = newsyns & "mercuryii"
	            offset = offset + 8
	        ElseIf Mid(thesyns, offset, 8) = "methyoxy" Then
	            newsyns = newsyns & "methoxy"
	            offset = offset + 8
	        ElseIf Mid(thesyns, offset, 5) = "methy" And Not Mid(thesyns, offset, 6) = "methyl" Then
	            newsyns = newsyns & "methyl"
	            offset = offset + 5
	        ElseIf Mid(thesyns, offset, 4) = "meta" Then
	            newsyns = newsyns & "3"
	            offset = offset + 4
	        ElseIf Len(Mid(thesyns, offset + 1, 1)) * _
	            InStr("- ,'", Mid(thesyns, offset + 1, 1)) * _
	            InStr("- ,'([{", Mid(thesyns, offset - 1, 1)) > 0 Then
	            newsyns = newsyns & "3"
	            offset = offset + 1
	        Else
	            newsyns = newsyns & "m"
	            offset = offset + 1
	        End If
	    Case "n"
	        If Mid(thesyns, offset, 11) = "nonahydrate" Then
	            newsyns = newsyns & "9h2o"
	            offset = offset + 11
	        ElseIf Mid(thesyns, offset, 9) = "nickelous" Then
	            newsyns = newsyns & "nickelii"
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 5) = "napth" Then
	            newsyns = newsyns & "naphth"
	            offset = offset + 5
	        Else
	            newsyns = newsyns & "n"
	            offset = offset + 1
	        End If
	    Case "o"
	        If Mid(thesyns, offset, 15) = "octadecahydrate" Then
	            newsyns = newsyns & "18h2o"
	            offset = offset + 15
	        ElseIf Mid(thesyns, offset, 11) = "octahydrate" Then
	            newsyns = newsyns & "8h2o"
	            offset = offset + 11
	        ElseIf Mid(thesyns, offset, 8) = "ous acid" Then
	            If InStr(newsyns, "yl") Then
	                newsyns = newsyns & "ousacid"
	                offset = offset + 8
	            Else
	                newsyns = newsyns & "it"
	                offset = offset + 8
	            End If
	        ElseIf Mid(thesyns, offset, 5) = "ortho" Then
	            newsyns = newsyns & "2"
	            offset = offset + 5
	        ElseIf Mid(thesyns, offset, 5) = "omega" Then
	            newsyns = newsyns & "w"
	            offset = offset + 5
	        ElseIf Mid(thesyns, offset, 3) = "oyl" Then
	            newsyns = FixIcAte(newsyns) & "o"
	            If Right(newsyns, 1) <> "o" Then newsyns = newsyns & "o"
	            offset = offset + 1
	        ElseIf Len(Mid(thesyns, offset + 1, 1)) * InStr("- ,'", Mid(thesyns, offset + 1, 1)) * InStr("- ,'([{", Mid(thesyns, offset - 1, 1)) > 0 Then
	            newsyns = newsyns & "2"
	            offset = offset + 1
	        Else
	            newsyns = newsyns & "o"
	            offset = offset + 1
	        End If
	    Case "p"
	        If Mid(thesyns, offset, 16) = "phosphorothioate" Then
	            newsyns = newsyns & "thiophosphat"
	            offset = offset + 16
	        ElseIf Mid(thesyns, offset, 12) = "pentahydrate" Then
	            newsyns = newsyns & "5h2o"
	            offset = offset + 12
	        ElseIf Mid(thesyns, offset, 11) = "phosphorous" Then
	            newsyns = newsyns & "phosphorus"
	            offset = offset + 11
	        ElseIf Mid(thesyns, offset, 8) = "plumbous" Then
	            newsyns = newsyns & "leadi"
	            offset = offset + 8
	        ElseIf Mid(thesyns, offset, 6) = "pyrine" Then
	            newsyns = newsyns & "pyren"
	            offset = offset + 6
	        ElseIf Mid(thesyns, offset, 4) = "para" Then
	            newsyns = newsyns & "4"
	            offset = offset + 4
	        ElseIf (Mid(thesyns, offset, 3) = "pht") And Not (Mid(thesyns, offset, 4) = "phth") Then
	            newsyns = newsyns & "phth"
	            offset = offset + 3
	        ElseIf Len(Mid(thesyns, offset + 1, 1)) * (InStr("- ,'", Mid(thesyns, offset + 1, 1)) * InStr("- ,'([{", Mid(thesyns, offset - 1, 1))) <> 0 Then
	            newsyns = newsyns & "4"
	            offset = offset + 1
	        Else
	            newsyns = newsyns & "p"
	            offset = offset + 1
	        End If
	    Case "s"
	        If Mid(thesyns, offset, 13) = "sesquihydrate" Then
	            newsyns = newsyns & "3/2h2o"
	            offset = offset + 13
	        ElseIf Mid(thesyns, offset, 8) = "salicycl" Then
	            newsyns = newsyns & "salicyl"
	            offset = offset + 8
	        ElseIf Mid(thesyns, offset, 8) = "stannane" Then
	            newsyns = newsyns & "tin"
	            offset = offset + 8
	        ElseIf Mid(thesyns, offset, 8) = "stannous" Then
	            newsyns = newsyns & "tinii"
	            offset = offset + 8
	        ElseIf Mid(thesyns, offset, 7) = "stannic" Then
	            newsyns = newsyns & "tiniv"
	            offset = offset + 7
	        ElseIf Mid(thesyns, offset, 5) = "sulph" Then
	            newsyns = newsyns & "sulf"
	            offset = offset + 5
	        ElseIf Mid(thesyns, offset, 5) = "salts" Then
	            offset = offset + 5
	        ElseIf Mid(thesyns, offset, 4) = "sec-" Then
	            newsyns = newsyns & "s"
	            offset = offset + 4
	        ElseIf Mid(thesyns, offset, 4) = "salt" Then
	            offset = offset + 4
	        Else
	            newsyns = newsyns & "s"
	            offset = offset + 1
	        End If
	    Case "t"
	        If Mid(thesyns, offset, 12) = "tetrahydrate" Then
	            newsyns = newsyns & "4h2o"
	            offset = offset + 12
	        ElseIf Mid(thesyns, offset, 10) = "trihydrate" Then
	            newsyns = newsyns & "3h2o"
	            offset = offset + 10
	        ElseIf Mid(thesyns, offset, 9) = "toluidine" Then
	            newsyns = newsyns & "methylbenzenamin"
	            offset = offset + 9
	        ElseIf Mid(thesyns, offset, 8) = "thallous" Then
	            newsyns = newsyns & "thalliumi"
	            offset = offset + 8
	        ElseIf Mid(thesyns, offset, 8) = "terthiop" Then
	            newsyns = newsyns & "terthiop"
	            offset = offset + 8
	        ElseIf Mid(thesyns, offset, 7) = "thallic" Then
	            newsyns = newsyns & "thalliumiii"
	            offset = offset + 7
	        ElseIf Mid(thesyns, offset, 4) = "tert" Then
	            newsyns = newsyns & "t"
	            offset = offset + 4
	        ElseIf Mid(thesyns, offset, 4) = "tris" Then
	            newsyns = newsyns & "tri"
	            offset = offset + 4
	        Else
	            newsyns = newsyns & "t"
	            offset = offset + 1
	        End If
	    Case "w"
	        If Mid(thesyns, offset, 5) = "water" Then
	            newsyns = newsyns & "h2o"
	            offset = offset + 6
	        Else
	            newsyns = newsyns & "w"
	            offset = offset + 1
	        End If
	    Case "y"
	        If Mid(thesyns, offset, 12) = "yl mercaptan" Then
	            newsyns = newsyns & "anethiol"
	            offset = offset + 12
	        ElseIf Mid(thesyns, offset, 11) = "ylmercaptan" Then
	            newsyns = newsyns & "anethiol"
	            offset = offset + 11
	        ElseIf Mid(thesyns, offset, 10) = "yl alcohol" Then
	            newsyns = newsyns & "anol"
	            offset = offset + 10
	        ElseIf Mid(thesyns, offset, 9) = "ylalcohol" Then
	            newsyns = newsyns & "anol"
	            offset = offset + 9
	        Else
	            newsyns = newsyns & "y"
	            offset = offset + 1
	        End If
	    Case Else
	        newsyns = newsyns & Mid(thesyns, offset, 1)
	        offset = offset + 1
	    End Select
	Loop

	If allnewsyns <> "" Then allnewsyns = allnewsyns & "; "
	newsyns = allnewsyns & newsyns

	      'Debug.Print newsyns
	thesyns = Mid(thesyns, 2, Len(thesyns) - 1)
	If newsyns <> "" And newsyns <> "%" And newsyns <> "%%" Then
	    CleanTheSyns = CleanTheSynsPart2(newsyns)
	Else
	    Do While InStr(thesyns, "*")
	        Mid(thesyns, InStr(thesyns, "*"), 1) = "%"
	    Loop
	    CleanTheSyns = thesyns
	End If
	      'Debug.Print newsyns
End Function


'***************************************************************************
'Clean name query to match the format of cleanfield in database
'Called by CleanTheSyns
'Input: partially cleaned thesyns
'Output: more cleaned thesyns
'***************************************************************************

Function CleanTheSynsPart2(ByRef thesyns)
	Dim offset
	Dim newsyns
	Dim doloop

	thesyns = "(" & thesyns

	offset = 1
	'Debug.Print thesyns
	Do While offset <= Len(thesyns)
	    Select Case (Mid(thesyns, offset, 1))
	    Case "("
	        offset = offset + 1
	    Case "1"
	        If Mid(thesyns, offset, 15) = "11dimethylethyl" Then
	            newsyns = newsyns & "tbutyl"
	            offset = offset + 15
	        ElseIf Mid(thesyns, offset, 13) = "1methylpropyl" Then
	            newsyns = newsyns & "sbutyl"
	            offset = offset + 13
	        ElseIf Mid(thesyns, offset, 12) = "1methylethyl" Then
	            newsyns = newsyns & "isopropyl"
	            offset = offset + 12
	        ElseIf Mid(thesyns, offset, 11) = "135triazine" Then
	            newsyns = newsyns & "striazin"
	            offset = offset + 11
	        Else
	            newsyns = newsyns & "1"
	            offset = offset + 1
	        End If
	    Case "a"
	        If Mid(thesyns, offset, 7) = "arachid" Then
	            newsyns = newsyns & "eicosan"
	            offset = offset + 7
	        Else
	            newsyns = newsyns & "a"
	            offset = offset + 1
	        End If
	    Case "b"
	        If Mid(thesyns, offset, 5) = "butyr" Then
	            newsyns = newsyns & "butan"
	            offset = offset + 5
	        Else
	            newsyns = newsyns & "b"
	            offset = offset + 1
	        End If
	    Case "c"
	        If Mid(thesyns, offset, 5) = "capro" Then
	            newsyns = newsyns & "hexan"
	            offset = offset + 5
	        Else
	            newsyns = newsyns & "c"
	            offset = offset + 1
	        End If
	    Case "d"
	        If Mid(thesyns, offset, 2) = "d+" Then
	            newsyns = newsyns & "d"
	            offset = offset + 2
	        ElseIf Mid(thesyns, offset, 2) = "d-" Then
	            newsyns = newsyns & "d"
	            offset = offset + 2
	        Else
	            newsyns = newsyns & "d"
	            offset = offset + 1
	        End If
	    Case "e"
	        If Mid(thesyns, offset, 32) = "ethylenedinitrilotetraaceticacid" Then
	            newsyns = newsyns & "edta"
	            offset = offset + 32
	        ElseIf Mid(thesyns, offset, 30) = "ethylenediaminetetraaceticacid" Then
	            newsyns = newsyns & "edta"
	            offset = offset + 30
	        ElseIf Mid(thesyns, offset, 27) = "ethylenediaminetetraacetate" Then
	            newsyns = newsyns & "edta"
	            offset = offset + 27
	        ElseIf Mid(thesyns, offset, 7) = "edetate" Then
	            newsyns = newsyns & "edta"
	            offset = offset + 7
	        ElseIf Mid(thesyns, offset, 5) = "enath" Then
	            newsyns = newsyns & "heptan"
	            offset = offset + 5
	        Else
	            newsyns = newsyns & "e"
	            offset = offset + 1
	        End If
	    Case "l"
	        If Mid(thesyns, offset, 4) = "laur" Then
	            newsyns = newsyns & "dodecan"
	            offset = offset + 4
	        ElseIf Mid(thesyns, offset, 2) = "l+" Then
	            newsyns = newsyns & "l"
	            offset = offset + 2
	        ElseIf Mid(thesyns, offset, 2) = "l-" Then
	            newsyns = newsyns & "l"
	            offset = offset + 2
	        Else
	            newsyns = newsyns & "l"
	            offset = offset + 1
	        End If
	    Case "m"
	        If Mid(thesyns, offset, 6) = "myrist" Then
	            newsyns = newsyns & "tetradecan"
	            offset = offset + 6
	        Else
	            newsyns = newsyns & "m"
	            offset = offset + 1
	        End If
	    Case "n"
	        If Mid(thesyns, offset, 12) = "naphthalenyl" Then
	            newsyns = newsyns & "naphthyl"
	            offset = offset + 6
	        Else
	            newsyns = newsyns & "n"
	            offset = offset + 1
	        End If
	    Case "p"
	        If Mid(thesyns, offset, 8) = "pelargon" Then
	            newsyns = newsyns & "nonan"
	            offset = offset + 8
	        ElseIf Mid(thesyns, offset, 7) = "propion" Then
	            newsyns = newsyns & "propan"
	            offset = offset + 7
	        ElseIf Mid(thesyns, offset, 6) = "palmit" Then
	            newsyns = newsyns & "hexadecan"
	            offset = offset + 6
	        Else
	            newsyns = newsyns & "p"
	            offset = offset + 1
	        End If
	    Case "s"
	        If Mid(thesyns, offset, 7) = "silveri" Then
	            newsyns = newsyns & "silver"
	            offset = offset + 7
	        ElseIf Mid(thesyns, offset, 5) = "stear" Then
	            newsyns = newsyns & "octadecan"
	            offset = offset + 5
	        Else
	            newsyns = newsyns & "s"
	            offset = offset + 1
	        End If
	    Case "v"
	        If Mid(thesyns, offset, 5) = "valer" Then
	            newsyns = newsyns & "pentan"
	            offset = offset + 5
	        Else
	            newsyns = newsyns & "v"
	            offset = offset + 1
	        End If
	    Case Else
	        newsyns = newsyns & Mid(thesyns, offset, 1)
	        offset = offset + 1
	    End Select
	Loop
	thesyns = Mid(thesyns, 2, Len(thesyns) - 1)
	If newsyns <> "" Then
	    CleanTheSynsPart2 = newsyns
	Else
	    CleanTheSynsPart2 = thesyns
	End If
End Function

'***************************************************************************
'Part of the chemical name normalization routine
'Used primarily for acids, esters, and acid halides
'Called by CleanTheSyns
'Input: thesyns
'Output: more cleaned thesyns
'***************************************************************************
Function FixIcAte(ByVal thesyns)

	If Right(thesyns, 8) = "pelargon" Then
	    thesyns = Left(thesyns, Len(thesyns) - 8) & "nonano"
	ElseIf Right(thesyns, 7) = "propion" Then
	    thesyns = Left(thesyns, Len(thesyns) - 7) & "propano"
	ElseIf Right(thesyns, 7) = "arachid" Then
	    thesyns = Left(thesyns, Len(thesyns) - 7) & "eicosano"
	ElseIf Right(thesyns, 7) = "methano" Then
	    thesyns = Left(thesyns, Len(thesyns) - 7) & "form"
	ElseIf Right(thesyns, 6) = "myrist" Then
	    thesyns = Left(thesyns, Len(thesyns) - 6) & "tetradecano"
	ElseIf Right(thesyns, 6) = "ethano" Then
	    thesyns = Left(thesyns, Len(thesyns) - 6) & "acet"
	ElseIf Right(thesyns, 6) = "palmit" Then
	    thesyns = Left(thesyns, Len(thesyns) - 6) & "hexadecano"
	ElseIf Right(thesyns, 5) = "stear" Then
	    thesyns = Left(thesyns, Len(thesyns) - 5) & "octadecano"
	ElseIf Right(thesyns, 5) = "valer" Then
	    thesyns = Left(thesyns, Len(thesyns) - 5) & "pentano"
	ElseIf Right(thesyns, 5) = "butyr" Then
	    thesyns = Left(thesyns, Len(thesyns) - 5) & "butano"
	ElseIf Right(thesyns, 5) = "capro" Then
	    thesyns = Left(thesyns, Len(thesyns) - 5) & "hexano"
	ElseIf Right(thesyns, 5) = "enath" Then
	    thesyns = Left(thesyns, Len(thesyns) - 5) & "heptano"
	ElseIf Right(thesyns, 4) = "laur" Then
	    thesyns = Left(thesyns, Len(thesyns) - 4) & "dodecano"
	ElseIf Right(thesyns, 4) = "capr" Then
	    thesyns = Left(thesyns, Len(thesyns) - 4) & "decano"
	End If

	FixIcAte = thesyns

End Function
%>