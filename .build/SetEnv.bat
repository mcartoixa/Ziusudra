@ECHO OFF

:: Reset ERRORLEVEL
VERIFY OTHER 2>nul


:: -------------------------------------------------------------------
:: Set environment variables
:: -------------------------------------------------------------------
CALL :SetVersionsEnvHelper 2>nul


:: -------------------------------------------------------------------
:: Install dependencies
:: -------------------------------------------------------------------
IF NOT EXIST .tmp MKDIR .tmp



IF NOT EXIST "%CD%\.tmp\cloc.exe" GOTO SETENV_CLOC
FOR /F %%i IN ('"%CD%\.tmp\cloc.exe" --version') DO (
    IF "%%i"=="%_CLOC_VERSION%" GOTO SETENV_LOCAL
)
:SETENV_CLOC
powershell.exe -NoLogo -NonInteractive -ExecutionPolicy ByPass -Command "& { [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; Invoke-WebRequest https://github.com/AlDanial/cloc/releases/download/v$Env:_CLOC_VERSION/cloc-$Env:_CLOC_VERSION.exe -OutFile .tmp\cloc.exe; }"
IF ERRORLEVEL 1 GOTO ERROR_CLOC




:: -------------------------------------------------------------------
:: Set environment variables
:: -------------------------------------------------------------------
:SETENV_LOCAL
CALL :SetLocalEnvHelper 2>nul

GOTO END



:: -------------------------------------------------------------------
:: Install dependencies
:: -------------------------------------------------------------------
:SetLocalEnvHelper
IF EXIST .env (
    FOR /F "eol=# tokens=1* delims==" %%i IN (.env) DO (
        SET "%%i=%%j"
        ECHO SET %%i=%%j
    )
    ECHO.
)
EXIT /B 0



:SetVersionsEnvHelper
IF EXIST .build\versions.env (
    FOR /F "eol=# tokens=1* delims==" %%i IN (.build\versions.env) DO (
        SET "%%i=%%j"
        ECHO SET %%i=%%j
    )
    ECHO.
)
EXIT /B 0



:: -------------------------------------------------------------------
:: Error messages
:: -------------------------------------------------------------------
:ERROR_CLOC
ECHO [31mCould not install CLOC %_CLOC_VERSION%[0m 1>&2
GOTO END_ERROR



:: -------------------------------------------------------------------
:: End
:: -------------------------------------------------------------------
:END_ERROR
EXIT /B 1

:END
