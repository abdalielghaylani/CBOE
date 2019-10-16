@echo off
SET WORKSPACE=%1
SET MSBUILD=%2
MD "%WORKSPACE%\CBOE17.1"
MD "%WORKSPACE%\subprojects\Registration\PresentationTier\Test" 
MD "%WORKSPACE%\Install\install9" 
MD "%WORKSPACE%\Install\install9\UserInterface" 
xcopy /Y/S "C:\builds\workspace\CBOE18.1_Pipeline\subprojects\Registration\PresentationTier\Test\*.*" "%WORKSPACE%\subprojects\Registration\PresentationTier\Test\"
xcopy /Y/S "C:\builds\workspace\CBOE18.1_Pipeline\Install\install9\UserInterface\*.*" "%WORKSPACE%\Install\install9\UserInterface\"
xcopy /Y/S "C:\builds\workspace\CBOE18.1_Pipeline\ChemOffice\webserver_source\cfserveradmin\AdminSource\mswcrun.dll" "%WORKSPACE%\ChemOffice\webserver_source\cfserveradmin\AdminSource\"
xcopy /Y "%WORKSPACE%\subprojects\ChemBioViz\ServiceTier\ChemBioViz.Services\bin\Release\CambridgeSoft.COE.ChemBioViz.Services.dll" "%WORKSPACE%\CBOE17.1"
xcopy /Y "%WORKSPACE%\subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\CambridgeSoft.COE.Framework.dll" "%WORKSPACE%\CBOE17.1"
xcopy /Y "%WORKSPACE%\subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\CambridgeSoft.COE.Framework.XmlSerializers.dll" "%WORKSPACE%\CBOE17.1"
xcopy /Y "%WORKSPACE%\subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\CambridgeSoft.COE.Registration.RegistrationAddins.dll" "%WORKSPACE%\CBOE17.1"
xcopy /Y "%WORKSPACE%\subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\CambridgeSoft.COE.Registration.Services.dll" "%WORKSPACE%\CBOE17.1"
xcopy /Y "%WORKSPACE%\subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\CambridgeSoft.COE.RegistrationAdmin.Services.dll" "%WORKSPACE%\CBOE17.1"
xcopy /Y "%WORKSPACE%\subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\CambridgeSoft.COE.RegistrationWebApp.dll" "%WORKSPACE%\CBOE17.1"
xcopy /Y "%WORKSPACE%\subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\Registration.Core.dll" "%WORKSPACE%\CBOE17.1" 
nuget.exe update -self
nuget.exe restore "%WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server.2013.Build.sln"
%MSBUILD% %WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server.2013.Build.sln /p:CreatePackageOnPublish=true /p:DeployOnBuild=true /p:Platform=x64
xcopy /Y/S "%WORKSPACE%\subprojects\Framework\CambridgeSoft\CommonRuntimeLibraries\*.*" "%WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server\bin\"
xcopy /Y/S "%WORKSPACE%\CBOE17.1\*.*" "%WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server\bin\"
xcopy /Y/S "%WORKSPACE%\subprojects\ChemBioViz\ServiceTier\ChemBioViz.Services\bin\Release\Interop.ChemDrawControl18.dll" "%WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server\bin"