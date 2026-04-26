@echo off
setlocal enabledelayedexpansion

rem 
rem Configurable variables
rem 
set "BRANCH=main"
set "INITIAL_COMMIT_MSG=Initial"

rem 
rem Cool Output Messages
rem 
echo ==============================
echo Github Repository Reset
echo ==============================
echo WARNING: This script will:
echo 1. Delete all commit history
echo 2. Create a new initial commit with current content
echo 3. Force push to the specified branch
echo CAUTION: This action is irreversible!
echo ==============================

rem 
rem Confirm the user wants to proceed
rem 
set /p "confirm=Are you sure you want to proceed? (y/n): "
if /i not "!confirm!"=="y" (
	echo ==============================
    echo Operation cancelled.
	echo ==============================
    pause
    exit /b 1
)

rem 
rem Ask for remote origin URL
rem 
set /p "REPO_URL=Enter the remote repository URL (e.g., https://github.com/user/repo): "

rem 
rem Find the last version file in _changelogs/ folder and use its name (without extension) as default commit msg
rem 
set "defaultCommitMsg="
for /f "delims=" %%I in ('dir /a-d /b /o-d /tw "_changelogs" 2^>nul ^| findstr /r "^[0-9]*\."') do (
    set "defaultCommitMsg=%%~nI"
    goto :foundDefault
)

:foundDefault

rem 
rem Ask for commit message for this update (cannot be empty), default to changelog filename or initial
rem 
set "commitMsg=%defaultCommitMsg%"
if "!commitMsg!"=="" set "commitMsg=%INITIAL_COMMIT_MSG%"
echo Default commit message: "!commitMsg!"
set /p "commitMsg=Enter your commit message [!commitMsg!]: "
if "!commitMsg!"=="" set "commitMsg=%INITIAL_COMMIT_MSG%"

rem 
rem Cool message before starting the Git commands
rem 
echo ==============================
echo Resetting repository to !REPO_URL!...
echo ==============================

rem 
rem Check if Git is installed [web:17]
rem 
where git >nul 2>&1
if %errorlevel% neq 0 (
	echo ==============================
    echo Error updating the repository!
	echo ==============================
    echo Git is not installed or not in the system PATH.
    pause
    exit /b 1
)

rem 
rem Remove Git Folder
rem 
if exist ".git" (
	echo ==============================
    echo Removing existing .git directory...
	echo ==============================
    rmdir /s /q .git
)

rem 
rem Initialize a new Git repository
rem 
git init

rem 
rem Stage all files
rem 
git add .

rem 
rem Create a new initial commit
rem 
git commit -m "!commitMsg!"

rem 
rem Add the remote origin and push [web:6]
rem 
git remote add origin !REPO_URL!
git checkout -b %BRANCH%
git push -f origin %BRANCH%

rem 
rem Completion message
rem 
echo ==============================
echo Operation Complete
echo ==============================
pause

endlocal
