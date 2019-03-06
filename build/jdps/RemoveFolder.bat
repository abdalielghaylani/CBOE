@echo off
SET WORKSPACE=%1
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\subprojects\CBOEHelp\CBOE Admin - Project Files"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\subprojects\CBOEHelp\CBOE User - Project Files"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\ChemOffice\BioSAR_Browser"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\ChemOffice\DocManager"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\ChemOffice\VB Projects"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\ChemOffice\Consulting"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\ChemOffice\ChemInv_9SR"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\ChemOffice\Chem_Reg"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\ChemOffice\ChemACXOra"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\ChemOffice\D3"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\ChemOffice\DrugDeg"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\ChemOffice\Installers"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\ChemOffice\Excel SAR Viewer"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\ChemOffice\Inventory"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\ChemOffice\Project_Doc"
IF ERRORLEVEL 1 GOTO skip1
RD /S /Q "%WORKSPACE%\ChemOffice\wwwroot"
IF ERRORLEVEL 1 GOTO skip1

:skip1