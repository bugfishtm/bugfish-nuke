@echo off
setlocal enabledelayedexpansion

rem Cool Output Messages
echo ==============================
echo Welcome to the Bugfish Git Repository Reset Script!
echo ==============================
echo WARNING: This script will:
echo 1. Delete all commit history
echo 2. Create a new initial commit with current content
echo 3. Force push to the specified branch
echo ==============================
echo CAUTION: This action is irreversible!
echo ==============================

rem Confirm the user wants to proceed
set /p "confirm=Are you sure you want to proceed? (y/n): "
if /i not "!confirm!"=="y" (
    echo Operation cancelled.
    pause
    exit /b 1
)

rem Asking for the branch name
set /p "branch=Enter the branch you want to reset (default is 'main'): "
set /p "commit=Enter reset commit message (default is 'Initial'): "

rem Set default branch to 'main' if no input is given
if "!branch!"=="" set "branch=main"
if "!commit!"=="" set "commit=Initial"

rem Cool message before starting the Git commands
echo ==============================
echo Resetting repository...
echo ==============================

rem Check if Git is installed
where git >nul 2>&1
if %errorlevel% neq 0 (
    echo Git is not installed or not in the system PATH.
    pause
    exit /b 1
)

rem Remove Git Folder
if exist ".git" (
    echo Removing existing .git directory...
    rmdir /s /q .git
)

rem Initialize a new Git repository
git init

rem Stage all files
git add .

rem Create a new initial commit
git commit -m !commit!

rem Add the remote origin
set /p REPO_URL="Enter your GitHub repository URL: "
git remote add origin %REPO_URL%
git checkout -b !branch!
git push -f origin !branch!

rem Completion message
echo ==============================
echo All done! Your repository has been reset and force pushed.
echo All previous history has been erased.
echo ==============================
pause

endlocal
