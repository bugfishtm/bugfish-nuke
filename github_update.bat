@echo off
setlocal enabledelayedexpansion

::
:: Configurable variables
::
set "INITIAL_COMMIT_MSG=Initial"

::
:: Cool Output Messages
::
echo ==============================
echo Github Repository Update
echo ==============================

::
:: Confirm before proceeding
::
set /p "confirm=Are you sure you want to proceed? (y/n): "
if /i not "!confirm!"=="y" (
	echo ==============================
    echo Operation cancelled.
	echo ==============================
    pause
    exit /b 1
)

::
:: Find the last version file in _changelogs/ folder and use its name (without extension) as default commit msg
::
set "defaultCommitMsg="
for /f "delims=" %%I in ('dir /a-d /b /o-d /tw "_changelogs" 2^>nul ^| findstr /r "^[0-9]*\."') do (
    set "defaultCommitMsg=%%~nI"
    goto :foundDefault
)
:foundDefault

::
:: Ask for commit message for this update (cannot be empty), default to changelog filename or initial
::
set "commitMsg=%defaultCommitMsg%"
if "!commitMsg!"=="" set "commitMsg=%INITIAL_COMMIT_MSG%"
echo Default commit message: "!commitMsg!"
set /p "commitMsg=Enter your commit message [!commitMsg!]: "
if "!commitMsg!"=="" set "commitMsg=%INITIAL_COMMIT_MSG%"

::
:: Cool message before starting the Git commands
::
echo ==============================
echo Staging all files...
echo ==============================

::
:: Stage all files except batch script itself (optional: modify if you want to exclude)
::
git add .

::
:: Commit with user input message
::
echo ==============================
echo Committing with message: "!commitMsg!"
echo ==============================
git commit -m "!commitMsg!"

::
:: Push to specified branch
::
echo ==============================
echo Pushing to branch: main
echo ==============================
git push -u origin main

::
:: Completion message
::
echo ==============================
echo Operation Complete
echo ==============================
pause

endlocal
