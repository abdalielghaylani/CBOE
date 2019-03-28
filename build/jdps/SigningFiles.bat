@echo off
SET SIGNTOOL=%1
SET WORKSPACE=%2
SET FOLDER_VERSION_NAME=%3
call %SIGNTOOL% "%WORKSPACE%\subprojects\DataLoader\bin\Release\CambridgeSoft.COE.DataLoader.Calculation.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\CambridgeSoft.COE.DataLoader.Common.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataUploader\DataUploader\bin\Release\CambridgeSoft.COE.DataLoader.Core.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework\bin\Release\CambridgeSoft.COE.Framework.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework.Web\bin\Release\CambridgeSoft.COE.Framework.Web.COEFrameworkServices.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework\bin\Release\CambridgeSoft.COE.Framework.XmlSerializers.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Manager\Manager\bin\CambridgeSoft.COE.Manager.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\CambridgeSoft.COE.Registration.RegistrationAddins.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\CambridgeSoft.COE.Registration.Services.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\CambridgeSoft.COE.RegistrationAdmin.Services.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\CambridgeSoft.COE.RegistrationWebApp.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\Registration.Core.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Security\SingleSignOn\SingleSignOn\bin\SingleSignOn.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\COELogViewer\bin\Release\COELogViewer.exe"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\ServiceTier\RLSConfigurationTool\RLSConfigurationTool\bin\Release\CambridgeSoft.COE.RLSConfigurationTool.exe"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\ConfigLoader\bin\Release\ConfigLoader.exe"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataUploader\DataUploader\bin\Release\COEDataLoader.exe"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\DataLoader.exe"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Classic\Common\dlls\CSDO17.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Classic\Common\dlls\REPORTQ.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Classic\Common\dlls\COEHeaderServer.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Classic\Common\dlls\CSSecurityLDAP.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Classic\Common\dlls\CSDO14.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Classic\Common\dlls\CSDO15.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Classic\Common\dlls\ENLDAPProxy.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\COELogViewer\bin\Release\CambridgeSoft.COE.Framework.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\Patcher\Patcher\bin\Release\COEPatcher.exe"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\ConfigLoader\bin\Release\CambridgeSoft.COE.Framework.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\PyEngine3.dll"
call %SIGNTOOL% "%WORKSPACE%\Registration\PresentationTier\RegistrationWebApp\Bin\PyEngine3.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\CommonRuntimeLibraries\PyEngine3.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\CommonRuntimeLibraries\CambridgeSoft.COE.Framework.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\CommonRuntimeLibraries\AssemblySettings.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework.Web\bin\Release\CambridgeSoft.COE.Framework.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework.Web\bin\Release\CambridgeSoft.COE.Framework.XmlSerializers.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework.Web\bin\Release\CambridgeSoft.COE.Framework.Web.COEFrameworkServices.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Manager\Manager\bin\CambridgeSoft.COE.Framework.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Manager\Manager\bin\CambridgeSoft.COE.Manager.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\ServiceTier\RLSConfigurationTool\RLSConfigurationTool\bin\Release\CambridgeSoft.COE.Framework.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\CambridgeSoft.COE.Framework.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework.WebServiceHost\Bin\CambridgeSoft.COE.Framework.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework.WebServiceHost\Bin\CambridgeSoft.COE.Framework.XmlSerializers.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\CambridgeSoft.COE.Registration.RegistrationAddins.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Manager\COEConfigurationPublishTool\COEConfigurationPublishTool\bin\Release\COEConfigurationPublishTool.exe"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Manager\Manager\bin\CambridgeSoft.COE.Framework.XmlSerializers.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataUploader\DataUploader\bin\Release\CambridgeSoft.COE.DataLoader.Common.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataUploader\DataUploader\bin\Release\CambridgeSoft.COE.Framework.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataUploader\DataUploader\bin\Release\CambridgeSoft.COE.Framework.XmlSerializers.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataUploader\DataUploader\bin\Release\CambridgeSoft.COE.Registration.RegistrationAddins.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataUploader\DataUploader\bin\Release\CambridgeSoft.COE.Registration.Services.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataUploader\DataUploader\bin\Release\DataLoaderGUI.exe"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataUploader\DataUploader\bin\Release\Registration.Core.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\CambridgeSoft.COE.Framework.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\CambridgeSoft.COE.Framework.XmlSerializers.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\CambridgeSoft.COE.Registration.RegistrationAddins.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\CambridgeSoft.COE.Registration.Services.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\Registration.Core.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\ChemBioViz\PresentationTier\ChemBioVizWebApp\bin\CambridgeSoft.COE.ChemBioViz.Services.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\ChemBioViz\PresentationTier\ChemBioVizWebApp\bin\CambridgeSoft.COE.ChemBioVizWebApp.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\ChemBioViz\PresentationTier\ChemBioVizWebApp\bin\CambridgeSoft.COE.Framework.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\ChemBioViz\PresentationTier\ChemBioVizWebApp\bin\CambridgeSoft.COE.Framework.XmlSerializers.dll"
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\ChemInv\Installation\InvLoader\InvLoaderSetup.exe"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataUploader\DataUploader\bin\Release\ChemControls.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataUploader\DataUploader\bin\Release\CambridgeSoft.COE.DataLoader.Calculation.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataUploader\DataUploader\bin\Release\CambridgeSoft.COE.Framework.XmlSerializers.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\COELogViewer\bin\Release\CambridgeSoft.COE.Framework.XmlSerializers.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\ConfigLoader\bin\Release\CambridgeSoft.COE.Framework.XmlSerializers.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework.WebServiceHost\Bin\CambridgeSoft.COE.Framework.XmlSerializers.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\ServiceTier\RLSConfigurationTool\RLSConfigurationTool\bin\Release\CambridgeSoft.COE.Framework.XmlSerializers.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\CambridgeSoft.COE.Framework.XmlSerializers.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\RegistrationWebApp\Bin\App_Licenses.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Manager\Manager\Forms\Master\UserControls\Custom\MyCustomControlsLib.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\CommonResources\infragistics\20111CLR20\Forms\WebSchedule\Bin\App_Licenses.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Classic\Common\dlls\SingleSignOnCom.dll"
call %SIGNTOOL% "%WORKSPACE%\Informatics\Classic\DocManager\8.0\Components\FileBatchLoad.exe"
call %SIGNTOOL% "%WORKSPACE%\Informatics\Classic\DocManager\8.0\Components\dcomperm.exe"
call %SIGNTOOL% "%WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server\bin\PerkinElmer.COE.Registration.Server.dll"
call %SIGNTOOL% "%WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server\bin\CambridgeSoft.COE.ChemBioViz.Services.dll"
call %SIGNTOOL% "%WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server\bin\CambridgeSoft.COE.ChemBioViz.Services.dll"	
call %SIGNTOOL% "%WORKSPACE%\Subprojects\CSDO\CSDO18\Goods\CSDO18.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataUploader\DataUploader\bin\Release\PyEngine3.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\RegistrationWebApp\bin\PyEngine3.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\Registration.Server\bin\PyEngine3.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\Registration.Server\bin\CambridgeSoft.COE.Framework.XmlSerializers.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\Registration.Server\bin\CambridgeSoft.COE.Framework.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\Registration.Server\bin\CambridgeSoft.COE.Registration.RegistrationAddins.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\Registration.Server\bin\CambridgeSoft.COE.Registration.Services.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\Registration.Server\bin\CambridgeSoft.COE.RegistrationAdmin.Services.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\Registration.Server\bin\CambridgeSoft.COE.RegistrationWebApp.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Registration\PresentationTier\Registration.Server\bin\Registration.Core.dll"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\app.publish\Application Files\DataLoader_%FOLDER_VERSION_NAME%\CambridgeSoft.COE.Framework.dll.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\app.publish\Application Files\DataLoader_%FOLDER_VERSION_NAME%\CambridgeSoft.COE.Framework.XmlSerializers.dll.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\app.publish\Application Files\DataLoader_%FOLDER_VERSION_NAME%\CambridgeSoft.COE.Registration.RegistrationAddins.dll.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\app.publish\Application Files\DataLoader_%FOLDER_VERSION_NAME%\CambridgeSoft.COE.Registration.Services.dll.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\app.publish\Application Files\DataLoader_%FOLDER_VERSION_NAME%\Registration.Core.dll.deploy"
call %SIGNTOOL% "%WORKSPACE%\subprojects\DataLoader\bin\Release\app.publish\Application Files\DataLoader_%FOLDER_VERSION_NAME%\CambridgeSoft.COE.DataLoader.Calculation.dll.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\app.publish\Application Files\DataLoader_%FOLDER_VERSION_NAME%\CambridgeSoft.COE.DataLoader.Common.dll.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\DataLoader\bin\Release\app.publish\Application Files\DataLoader_%FOLDER_VERSION_NAME%\DataLoader.exe.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\COELogViewer\bin\Release\app.publish\Application Files\COELogViewer_1_0_0_0\CambridgeSoft.COE.Framework.dll.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\COELogViewer\bin\Release\app.publish\Application Files\COELogViewer_1_0_0_0\CambridgeSoft.COE.Framework.XmlSerializers.dll.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\CambridgeSoft\ServiceTier\COELogViewer\bin\Release\app.publish\Application Files\COELogViewer_1_0_0_0\COELogViewer.exe.deploy"
call %SIGNTOOL% "%WORKSPACE%\subprojects\Framework\CambridgeSoft\ServiceTier\Patcher\Patcher\bin\Release\app.publish\Application Files\COEPatcher_1_0_0_0\COEPatcher.exe.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\ConfigLoader\bin\Release\app.publish\Application Files\ConfigLoader_1_0_0_0\CambridgeSoft.COE.Framework.dll.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\ConfigLoader\bin\Release\app.publish\Application Files\ConfigLoader_1_0_0_0\ConfigLoader.exe.deploy"
call %SIGNTOOL% "%WORKSPACE%\subprojects\Framework\CambridgeSoft\CommonRuntimeLibraries\MolServer12.dll
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\COELogViewer\app.publish\Application Files\COELogViewer_1_0_0_0\CambridgeSoft.COE.Framework.dll.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\COELogViewer\app.publish\Application Files\COELogViewer_1_0_0_0\CambridgeSoft.COE.Framework.XmlSerializers.dll.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\COELogViewer\app.publish\Application Files\COELogViewer_1_0_0_0\COELogViewer.exe.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\COEPatchingTool\app.publish\Application Files\COEPatcher_1_0_0_0\COEPatcher.exe.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\Framework\ConfigLoader\app.publish\Application Files\ConfigLoader_1_0_0_0\CambridgeSoft.COE.Framework.dll.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\Framework\ConfigLoader\app.publish\Application Files\ConfigLoader_1_0_0_0\ConfigLoader.exe.deploy"
call %SIGNTOOL% "%WORKSPACE%\Subprojects\Framework\RuntimeLibraries\MolServer12.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\Common\dlls\Asc67Cvt.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\Common\dlls\Base64.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\Common\dlls\Base64Decode.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\Common\dlls\ChemImp.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\Common\dlls\ChemImp.exe
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\Common\dlls\COEHeaderServer.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\Common\dlls\CSDO12.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\Common\dlls\CSSecurityLDAP.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\Common\dlls\ENLDAPProxy.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\Common\dlls\HillOrder.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\Common\dlls\LDAPTest.exe
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\Common\dlls\ReadBinFile.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\Common\dlls\SingleSignOnCom.XmlSerializers.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\ChemInv\config\oracle_install_scripts\Create_test_ChemInv_DB\OracleSetup.exe
call %SIGNTOOL% "%WORKSPACE%\ChemInv\config\oracle_install_scripts\Export_ChemInv_DB\OracleSetup.exe
call %SIGNTOOL% "%WORKSPACE%\ChemInv\config\oracle_install_scripts\Create_blank_ChemInv_DB\OracleSetup.exe
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\WebServer_Source\cfserver_scripts\LockBroker.exe
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\WebServer_Source\cfserver_scripts\WAITFOR.DLL
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\WebServer_Source\cfserver_scripts\Lockit.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\WebServer_Source\cfserver_scripts\CDXLibCOM.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\WebServer_Source\cfserver_scripts\Base64Decode.dll
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\WebServer_Source\cfserver_scripts\configiis.exe
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\WebServer_Source\cfserverasp\Clients\SendToEnotebookV1\SendToE-Notebook 18.1.exe
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\WebServer_Source\cfserverasp\Clients\BioAssayClient10.1.6\setup.exe
call %SIGNTOOL% "%WORKSPACE%\ChemOffice\WebServer_Source\cfserver_scripts\LockBroker.exe				