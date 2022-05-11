@ECHO OFF
::--------------------------------------------------------------------
:: Usage: "build [clean | compile | test | analyze | package | build | rebuild | release] [/log] [/NoPause] [/?]"
::
::                 /NoPause  - Does not pause after completion
::                 /log      - Creates an extensive log file
::                 /?        - Gets the usage for this script
::--------------------------------------------------------------------



:: Reset ERRORLEVEL
VERIFY OTHER 2>nul
SETLOCAL ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION
IF ERRORLEVEL 1 GOTO ERROR_EXT

SET _NO_PAUSE=0
SET _LOGGERS=
SET _PROJECT=Ziusudra.proj
SET _TARGET=Build
SET _VERBOSITY=detailed
GOTO ARGS

:SHOW_USAGE
ECHO.
ECHO Usage: "build [clean | compile | test | analyze | package | build | rebuild | release] [/log] [/NoPause] [/?]"
ECHO.
ECHO.                /NoPause  - Does not pause after completion
ECHO.                /log      - Creates an extensive log file
ECHO.                /?        - Gets the usage for this script
IF "%_ERROR%"=="1" GOTO END_ERROR
GOTO END



:: -------------------------------------------------------------------
:: Builds the project
:: -------------------------------------------------------------------
:BUILD
dotnet.exe tool restore
IF ERRORLEVEL 1 GOTO END_ERROR

dotnet.exe msbuild %_PROJECT% /nologo /t:%_TARGET% /m /r /fl /flp:logfile=build.log;verbosity=%_VERBOSITY%;encoding=UTF-8 %_LOGGERS% /nr:False /v:normal
IF ERRORLEVEL 1 GOTO END_ERROR
GOTO END



:: -------------------------------------------------------------------
:: Parse command line argument values
:: Note: Currently, last one on the command line wins (ex: rebuild clean == clean)
:: -------------------------------------------------------------------
:ARGS
IF "%PROCESSOR_ARCHITECTURE%"=="x86" (
    "C:\Windows\Sysnative\cmd.exe" /C "%0 %*"

    IF ERRORLEVEL 1 EXIT /B 1
    EXIT /B 0
)
::IF NOT "x%~5"=="x" GOTO ERROR_USAGE

:ARGS_PARSE
IF /I "%~1"=="clean"                SET _TARGET=Clean& RMDIR /S /Q .tmp 2>nul& RMDIR /S /Q tmp 2>nul& SHIFT & GOTO ARGS_PARSE
IF /I "%~1"=="analyze"              SET _TARGET=Analyze& SHIFT & GOTO ARGS_PARSE
IF /I "%~1"=="compile"              SET _TARGET=Compile& SHIFT & GOTO ARGS_PARSE
IF /I "%~1"=="test"                 SET _TARGET=Test& SHIFT & GOTO ARGS_PARSE
IF /I "%~1"=="package"              SET _TARGET=Package& SHIFT & GOTO ARGS_PARSE
IF /I "%~1"=="rebuild"              SET _TARGET=Rebuild& SHIFT & GOTO ARGS_PARSE
IF /I "%~1"=="release"              SET _TARGET=Release& SHIFT & GOTO ARGS_PARSE
IF /I "%~1"=="build"                SET _TARGET=Build& SHIFT & GOTO ARGS_PARSE
IF /I "%~1"=="/log"                 SET _VERBOSITY=diagnostic& SET _LOGGERS=/bl:build.binlog& SHIFT & GOTO ARGS_PARSE
IF /I "%~1"=="/NoPause"             SET _NO_PAUSE=1& SHIFT & GOTO ARGS_PARSE
IF /I "%~1"=="/?"   GOTO SHOW_USAGE
IF    "%~1" EQU ""  GOTO ARGS_DONE
ECHO [31mUnknown command-line switch[0m %~1
GOTO ERROR_USAGE

:ARGS_DONE



:: -------------------------------------------------------------------
:: Set environment variables
:: -------------------------------------------------------------------
:SETENV
CALL .build\SetEnv.bat
IF ERRORLEVEL 1 GOTO END_ERROR
ECHO.
GOTO BUILD



:: -------------------------------------------------------------------
:: Errors
:: -------------------------------------------------------------------
:ERROR_EXT
ECHO [31mCould not activate command extensions[0m
GOTO END_ERROR

:ERROR_USAGE
SET _ERROR=1
GOTO SHOW_USAGE



:: -------------------------------------------------------------------
:: End
:: -------------------------------------------------------------------
:END_ERROR
ECHO.
ECHO [41m                                                                                [0m
ECHO [41;1mThe build failed                                                                [0m
ECHO [41m                                                                                [0m

:END
@IF NOT "%_NO_PAUSE%"=="1" PAUSE
ENDLOCAL
