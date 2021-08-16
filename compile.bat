@echo off

set PATH=%PATH%;%~dp0
set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework64\v4.0.30319

title Compiling...

csc.exe ^
    /r:mscorlib.dll ^
    /r:netstandard.dll ^
    /r:System.dll ^
    /r:System.IO.dll ^
    /r:System.Linq.dll ^
    /r:System.Collections.dll ^
    /r:System.Diagnostics.Process.dll ^
    /r:System.XML.dll ^
    /r:System.Xml.Linq.dll ^
    /r:System.Text.Encoding.dll ^
    /r:System.Runtime.dll ^
    %1

if errorlevel 1 goto ERROR

echo  ---  Compile Succeeded  ---
title Succeeded
color 2F
echo.
pause
start %~dp0\
color
exit

:ERROR
echo.
echo  ---  Compile Failed  ---
title Failed
color 4F
echo.
pause
color
exit
