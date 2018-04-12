cscript.exe appl1.vbs
cd DataLoader_11_0_1_0
If exist DataLoader.exe.deploy for %%F in (*.*) do ren "%%F" "%%~nF"
cd resources
If exist DL.ico.deploy for %%F in (*.*) do ren "%%F" "%%~nF"
cd..
cd..
cd..
mage -u "Application Files\DataLoader_11_0_1_0\DataLoader.exe.manifest" -fd "Application Files\DataLoader_11_0_1_0" -v 11.0.1.0
cd DataLoader_11_0_1_10
ren "*.*" "*.*.deploy"
ren "Resources\*.*" "*.*.deploy"
ren "help\*.*" "*.*.deploy"

ren "DataLoader.exe.manifest.deploy" "DataLoader.exe.manifest"
cd..
cd..
cscript.exe appl.vbs

mage -s "Application Files\DataLoader_11_0_1_0\DataLoader.exe.manifest" -cf MyPFX.pfx -pwd mykey 

mage -u DataLoader.application -appm "Application Files\DataLoader_11_0_1_0\DataLoader.exe.manifest" -v 11.0.1.0 -mv 11.0.1.0 -pu http://Coebuild/DataLoader110X/DataLoader.application
mage -s DataLoader.application -cf MyPFX.pfx -pwd mykey



