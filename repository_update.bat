@echo off
:: Cool Output Messages
echo ==============================
echo Welcome to the Bugfish Git Update Script!
echo ==============================
echo This script will help you:
echo 1. Stage all changes
echo 2. Commit with a message of your choice
echo 3. Push the commit to the branch you specify
echo ==============================

:: Asking for the branch name
set /p branch=Enter the branch you want to push to (default is 'main'): 

:: Set default branch to 'main' if no input is given
if "%branch%"=="" set branch=main

:: Asking for the commit message
set /p commitMsg=Enter your commit message: 

:: Cool message before starting the Git commands
echo ==============================
echo Staging files (excluding the .bat file)...
echo ==============================

:: Staging all files except this .bat file
git add . 

:: Cool message before committing
echo ==============================
echo Committing with message: "%commitMsg%"
echo ==============================

:: Committing the changes
git commit -m "%commitMsg%"

:: Cool message before pushing
echo ==============================
echo Pushing to branch: %branch%
echo ==============================

:: Pushing to the specified branch
git push -u origin %branch%

:: Completion message
echo ==============================
echo All done! Your changes have been pushed to the repository.
echo ==============================
pause
