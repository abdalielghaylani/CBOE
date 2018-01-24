Set CURRENTDIR=%1

CD %windir%\microsoft.net\framework\v4.0.30319

caspol.exe -polchgprompt off -u -remgroup "ChemBioVizExcel"

caspol.exe -polchgprompt on
