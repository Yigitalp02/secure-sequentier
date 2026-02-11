# Secure Sequentier Startup Script
Write-Host "Starting Secure Sequentier..." -ForegroundColor Green
Write-Host ""

# Get the script directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

# Function to cleanup processes when script exits
function Cleanup {
    Write-Host "Stopping Secure Sequentier applications..." -ForegroundColor Yellow
    Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force
    Write-Host "Applications stopped." -ForegroundColor Red
}

# Register cleanup function to run when script exits
Register-EngineEvent -SourceIdentifier PowerShell.Exiting -Action { Cleanup }

try {
    # Start the backend API service
    Write-Host "Starting Backend API Service..." -ForegroundColor Cyan
    $backendJob = Start-Job -ScriptBlock {
        Set-Location $using:scriptPath\SecureSolution2
        dotnet run
    }
    
    # Wait for backend to start
    Start-Sleep -Seconds 3
    
    # Start the frontend web service
    Write-Host "Starting Frontend Web Service..." -ForegroundColor Cyan
    $frontendJob = Start-Job -ScriptBlock {
        Set-Location $using:scriptPath\Web
        dotnet run
    }
    
    # Wait for frontend to start
    Start-Sleep -Seconds 5
    
    # Open the web interface
    Write-Host "Opening web interface..." -ForegroundColor Green
    Start-Process "http://localhost:5188"
    
    Write-Host ""
    Write-Host "Secure Sequentier is now running!" -ForegroundColor Green
    Write-Host "- Backend API: http://localhost:9999" -ForegroundColor White
    Write-Host "- Frontend Web: http://localhost:5188" -ForegroundColor White
    Write-Host ""
    Write-Host "Press any key to stop the applications..." -ForegroundColor Yellow
    
    # Wait for user input
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    
} finally {
    # Cleanup when script exits
    Cleanup
    if ($backendJob) { Remove-Job $backendJob -Force }
    if ($frontendJob) { Remove-Job $frontendJob -Force }
}
