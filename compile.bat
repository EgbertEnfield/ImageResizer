@echo off

set PATH=%PATH%;%~dp0
set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework64\v4.0.30319
set TARGET_0=imgresize

title Compiling...

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe ^
    /r:mscorlib.dll                                     ^
    /r:netstandard.dll                                  ^
    /r:System.dll                                       ^
    /r:System.Drawing.dll                               ^
    /r:System.Reflection.dll                            ^
    /r:System.Runtime.Serialization.dll                 ^
    /r:System.Runtime.Serialization.Json.dll;           ^
    /r:System.Text.RegularExpressions.dll               ^
    /r:System.Windows.Forms.dll                         ^
    /r:System.Threading.dll                             ^
    /r:%~dp0\bin\Debug\CommandLine.dll                  ^
    %~dp0\%TARGET_0%.cs %~dp0\Properties\AssemblyInfo.cs

if errorlevel 1 goto ERROR

echo  ---  Compile Succeeded  ---
title Succeeded
color 2F
echo.
pause
move %~dp0\%TARGET_0%.exe %USERPROFILE%\desktop\%TARGET_0%.exe
copy %~dp0\bin\Debug\CommandLine.dll %USERPROFILE%\desktop\CommandLine.dll
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
