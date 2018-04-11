C:
cd %~dp0
REM  In order to create a package that will properly validate in the Spotfire server
REM  while exlcuding some of the assemblies that we know we will not call from the
REM  extension, we use a trick.  Register those dlls in the GAC before the package is
REM  created and then unregister them when done
set var=%1
set var
set var=%var:"=%
"Package Builder\gacutil.exe" /il AssemblyPathlist.txt

"Package Builder\Spotfire.Dxp.PackageBuilder-Console.exe" /pkdesc:ChemBioVizSpotFireAddin.pkdesc /packageversion:%var% /target:Datalytix_%var%.spk /basefolder:%CD%\bin\Release /refpath:%CD%\bin\Release

"Package Builder\gacutil.exe" /ul AssemblyNamelist.txt
