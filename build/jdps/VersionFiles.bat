@echo off
SET VERSION_STRING=%1
SET WORKSPACE=%2
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Security\SingleSignOn\SingleSignOn\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%		  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%			      
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Framework\CambridgeSoft\ServiceTier\SQLGenerator.NUnitTest\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%			  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework.NUnitTests\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework.UnitTests\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Framework\CambridgeSoft\ServiceTier\CambridgeSoft.COE.Framework.Web\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Manager\Manager\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\ChemBioViz\ServiceTier\ChemBioViz.Services\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%                  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\ChemBioViz\PresentationTier\ChemBioVizWebApp\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%                  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Registration\ServiceTier\RegistrationAddins\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%                  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Registration\ServiceTier\CambridgeSoft.COE.Registration.Services\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%                  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Registration\ServiceTier\CambridgeSoft.COE.RegistrationAdmin.Services\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%                  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Core\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%			      
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Registration\PresentationTier\RegistrationWebServices\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%			     
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Registration\PresentationTier\RegistrationWebApp\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%			      
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Manager\COEConfigurationPublishTool\COEConfigurationPublishTool\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%			      
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\AssemblyInfo.cs" -fileversion=%VERSION_STRING%			      
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\CBVN\ChemBioViz.Net\ChemControls\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%			      
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\DataLoaderGUI\DataLoaderGUI\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\DataUploader\DataPreviewer\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\DataUploader\DataUploader.Core\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\DataUploader\DataUploader\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\DataUploader\DataUploader.UnitTests\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\DataLoader\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\DataLoader\Calculation\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\DataLoader\Calculator\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\DataLoader\Common\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\ConfigLoader\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Framework\CambridgeSoft\ServiceTier\Patcher\Patcher\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				  
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Registration\ServiceTier\RLSConfigurationTool\RLSConfigurationTool\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING%				 
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Framework\CambridgeSoft\ServiceTier\COELogViewer\Properties\AssemblyInfo.cs" -fileversion=%VERSION_STRING% 
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server\Properties\AssemblyInfo.cs"  -fileversion=%VERSION_STRING% 
%WORKSPACE%\installers\InstallSupportFiles\setversionstring.pl -file="%WORKSPACE%\subprojects\Registration\PresentationTier\Registration.Server\Properties\AssemblyInfo.cs"  -fileversion=%VERSION_STRING%