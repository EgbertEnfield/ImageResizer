@echo off

set PATH=%PATH%;%~dp0
set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework64\v4.0.30319
set TARGET_0=imgresize

title Compiling...

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe ^
    /r:mscorlib.dll                                     ^
    /r:netstandard.dll                                  ^
    /r:System.dll                                       ^
    %~dp0\%TARGET%.cs

if errorlevel 1 goto ERROR

echo  ---  Compile Succeeded  ---
title Succeeded
color 2F
echo.
pause
start %~dp0\%TARGET_1%.exe
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
