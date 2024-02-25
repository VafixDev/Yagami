@echo off
title Unhook Method

color 0c
cls

setlocal enabledelayedexpansion

for %%i in (*.exe) do (
    taskkill /IM "%%~ni.exe" /F 2>nul
    set "exeFile=%%i"
    goto :found
)
:found

copy "!exeFile!" C:\

del "!exeFile!"

copy "C:\!exeFile!" .

set "n1=notepad"
set "n2=mspaint"
set "n3=CalculatorApp"
set "n4=WINWORD"
set "n5=EXCEL"
set "n6=EXCEL"

set /a index=!random! %% 6
if !index! equ 0 set "name=!n1!"
if !index! equ 1 set "name=!n2!"
if !index! equ 2 set "name=!n3!"
if !index! equ 3 set "name=!n4!"
if !index! equ 4 set "name=!n5!"
if !index! equ 5 set "name=!n6!"

ren "!exeFile!" "!name!.exe"

del %Temp%\*.* /S /F /Q
del c:\windows\temp\*.* /S /F /Q

cls

endlocal
