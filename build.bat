@ECHO OFF
SET NCSC=csc
REM SET NCSC=csc /platform:x86

%NCSC% setversion.cs

ECHO BUILDING TEMPLATE RESOURCE
CD starter\resources

%NCSC% setdotnetver.cs
%NCSC% makeres.cs

REM setdotnetver.exe -v 1.0.3705
REM setdotnetver.exe -v 1.1.4322
REM setdotnetver.exe -v 2.0.50727
REM without -v the default will be taken
setdotnetver.exe

CALL makesr.bat
CD ..
CD ..

ECHO BUILDING APPLICATION

setversion.exe

SET NETZ_FILES=AssemblyInfo.cs ColorConsole.cs CompressProvider.cs GenData.cs Help.cs InputParser.cs InputFile.cs Logger.cs Netz.cs OutDirMan.cs ResMan.cs Utils.cs Zipper.cs
SET NETZ_FILES=%NETZ_FILES% compress\ICompress.cs starter\AssemblyInfoGen.cs starter\IconEx.cs starter\IconExtractor.cs starter\StarterGen.cs subsys\Win32PESubSystem.cs

COPY starter\starter.resources starter\netz.starter.starter.resources
SET NETZ_RES=starter\netz.starter.starter.resources

%NCSC% /target:exe /out:netz.exe /win32icon:App.ico %NETZ_FILES% /res:%NETZ_RES%

DEL /Q starter\netz.starter.starter.resources

setversion.exe -unset
del setversion.exe

ECHO BUILDING COMPRESSION PROVIDERS

COPY /Y netz.exe bin\Release
CD compress
CALL makedefcomp.bat
CALL makenet20comp.bat
CD ..

CD starter\resources
del makeres.exe
del setdotnetver.exe
CD ..
CD ..

ECHO PACKING FILES

MKDIR netz-bin
COPY /Y netz.exe netz-bin\*.*
COPY /Y zip.dll netz-bin\*.*
COPY /Y subsys\subsys.dll netz-bin\*.*
COPY /Y compress\defcomp.dll netz-bin\*.*
COPY /Y compress\net20comp.dll netz-bin\*.*
COPY /Y readme.txt netz-bin\*.*
COPY /Y license.txt netz-bin\*.*
COPY /Y COPYING.txt netz-bin\*.*
DEL /Q netz.exe

ECHO DONE


