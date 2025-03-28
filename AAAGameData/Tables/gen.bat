set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%/Luban/Luban.dll
set CONF_ROOT=.
set PROJECT_ROOT=../..

dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-bin ^
    -d bin  ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=%PROJECT_ROOT%/Assets/AAAGame/Scripts/Gen/DataTable ^
    -x bin.outputDataDir=%PROJECT_ROOT%/Assets/AAAGame/GenerateData/bytes ^
    -x json.outputDataDir=%PROJECT_ROOT%/Assets/AAAGame/GenerateData/json ^
    -x pathValidator.rootDir=%PROJECT_ROOT%

pause