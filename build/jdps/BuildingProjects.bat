@echo off
SET MSBUILD_PATH=%1
SET WORKSPACE=%2
%MSBUILD_PATH% %WORKSPACE%\subprojects\Security\SingleSignOn\SingleSignOn.sln /t:Build /p:Configuration=Release
%MSBUILD_PATH% %WORKSPACE%\subprojects\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework.sln /t:Build /p:Configuration=Release
%MSBUILD_PATH% %WORKSPACE%\subprojects\Registration\PresentationTier\RegistrationWebApp.sln  /t:Build /p:Configuration=Release
%MSBUILD_PATH% %WORKSPACE%\subprojects\DataLoader\CambridgeSoft.COE.DataLoader.sln /t:Build /p:Configuration=Release
%MSBUILD_PATH% %WORKSPACE%\subprojects\DataLoader\CambridgeSoft.COE.DataLoader.sln /t:Publish /p:Configuration=Release
%MSBUILD_PATH% %WORKSPACE%\subprojects\ConfigLoader\CambridgeSoft.COE.ConfigLoader.sln /t:Publish /p:Configuration=Release
%MSBUILD_PATH% %WORKSPACE%\subprojects\ChemBioViz\ServiceTier\CambridgeSoft.COE.ChemBioViz.sln /t:Build /p:Configuration=Release
%MSBUILD_PATH% %WORKSPACE%\subprojects\Manager\Manager.sln /t:Build /p:Configuration=Release
%MSBUILD_PATH% %WORKSPACE%\subprojects\DataUploader\DataUploader.sln /t:Build /p:Configuration=Release
%MSBUILD_PATH% %WORKSPACE%\subprojects\Registration\ServiceTier\RLSConfigurationTool\RLSConfigurationTool.sln /t:Build /p:Configuration=Release
%MSBUILD_PATH% %WORKSPACE%\subprojects\Framework\CambridgeSoft\ServiceTier\Patcher\Patcher.sln  /t:Publish /p:Configuration=Release
%MSBUILD_PATH% %WORKSPACE%\subprojects\Framework\CambridgeSoft\ServiceTier\COELogViewer\COELogViewer.sln /t:Publish /p:Configuration=Release	