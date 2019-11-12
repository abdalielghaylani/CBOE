@echo off
SET WORKSPACE=%1
SET MSBUILD=%2
MD "%WORKSPACE%\subprojects\Registration\PresentationTier\Test" 
MD "%WORKSPACE%\Install\install9" 
MD "%WORKSPACE%\Install\install9\UserInterface" 
xcopy /Y/S "C:\builds\workspace\Install\install9\UserInterface\*.*" "%WORKSPACE%\Install\install9\UserInterface\"
xcopy /Y/S "C:\builds\workspace\ChemOffice\webserver_source\cfserveradmin\AdminSource\mswcrun.dll" "%WORKSPACE%\ChemOffice\webserver_source\cfserveradmin\AdminSource\"
nuget.exe update -self
nuget.exe restore "%WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server.2013.sln"
%MSBUILD% %WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server.2013.sln /p:CreatePackageOnPublish=true /p:DeployOnBuild=true
xcopy /Y/S "%WORKSPACE%\subprojects\Framework\CambridgeSoft\CommonRuntimeLibraries\*.*" "%WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server\bin\"
xcopy /Y/S "%WORKSPACE%\subprojects\ChemBioViz\ServiceTier\ChemBioViz.Services\bin\Release\Interop.ChemDrawControl19.dll" "%WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server\bin"
xcopy /Y/S "%WORKSPACE%\subprojects\ChemBioViz\ServiceTier\ChemBioViz.Services\bin\Release\CambridgeSoft.COE.Framework.XmlSerializers.dll" "%WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server\bin"
