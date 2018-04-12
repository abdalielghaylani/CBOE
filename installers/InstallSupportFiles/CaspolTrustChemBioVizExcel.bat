
Set CURRENTDIR=%CD%

CD %windir%\microsoft.net\framework\v4.0.30319

caspol.exe -polchgprompt off -u -ag All_Code -url %CURRENTDIR%\* FullTrust -n "ChemBioVizExcel" -d "Full trust to entire direcotry"

caspol.exe -polchgprompt on
Pause