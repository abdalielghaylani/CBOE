cscript.exe appl1.vbs
cd Application Files\ChemBioViz.NET_11_0_2_0028
If exist ChemBioViz.NET.exe.deploy for %%F in (*.*) do ren "%%F" "%%~nF"
cd help
If exist ChemBioVizHelp.chm.deploy for %%F in (*.*) do ren "%%F" "%%~nF"
cd..
cd resources
If exist cbv_logo.ico.deploy for %%F in (*.*) do ren "%%F" "%%~nF"
cd..
cd..
cd..
mage -u "Application Files\ChemBioViz.NET_11_0_2_0028\ChemBioViz.NET.exe.manifest" -fd "Application Files\ChemBioViz.NET_11_0_2_0028" -v 11.0.2.0028

cd Application Files\ChemBioViz.NET_11_0_2_0030
ren "*.*" "*.*.deploy"
ren "Resources\*.*" "*.*.deploy"
ren "help\*.*" "*.*.deploy"
ren "ChemBioViz.NET.exe.manifest.deploy" "ChemBioViz.NET.exe.manifest"
cd..
cd..
cscript.exe appl.vbs
mage -s "Application Files\ChemBioViz.NET_11_0_2_0028\ChemBioViz.NET.exe.manifest" -cf MyPFX.pfx -pwd mykey 

mage -u ChemBioViz.NET.application -appm "Application Files\ChemBioViz.NET_11_0_2_0028\ChemBioViz.NET.exe.manifest" -v 11.0.2.0028 -mv 11.0.2.0028 -pu http://Coebuild/CBVN1102ClickOnce/ChemBioViz.NET.application
mage -s ChemBioViz.NET.application -cf MyPFX.pfx -pwd mykey

