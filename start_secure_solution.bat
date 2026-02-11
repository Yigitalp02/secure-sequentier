@echo off
echo Starting Secure Sequentier...
echo.

REM Change to the project directory
cd /d "%~dp0"

REM Start the backend API service in a new window
echo Starting Backend API Service...
start "Secure Sequentier Backend" cmd /k "cd /d %~dp0SecureSolution2 && dotnet run"

REM Wait a moment for the backend to start
timeout /t 3 /nobreak >nul

REM Start the frontend web service in a new window
echo Starting Frontend Web Service...
start "Secure Sequentier Frontend" cmd /k "cd /d %~dp0Web && dotnet run"

REM Wait a moment for the frontend to start
timeout /t 5 /nobreak >nul

REM Open the web interface in the default browser
echo Opening web interface...
start http://localhost:5188

echo.
echo Secure Sequentier is now running!
echo - Backend API: http://localhost:9999
echo - Frontend Web: http://localhost:5188
echo.
echo To stop the applications, close this window or press Ctrl+C
echo.

REM Keep this window open and wait for user input
pause
