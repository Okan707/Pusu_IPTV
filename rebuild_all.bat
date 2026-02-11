@echo off
echo Cleaning and Publishing IPTVApp...
cd IPTVApp
dotnet publish -c Release -r win-x64 --self-contained true

if %errorlevel% neq 0 (
    echo Publish failed!
    pause
    exit /b %errorlevel%
)

cd ..
echo Building Installer...
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" "setup_final.iss" > build_final.log 2>&1

if %errorlevel% neq 0 (
    echo Installer build failed! Check build_final.log
    pause
    exit /b %errorlevel%
)

echo Success! Installer created in Installer_Output folder.
pause
