param (
    [string]$backendUrl = "http://localhost:5234/api"
)

$ErrorActionPreference = "Stop"

if (Test-Path ".\src\client\.env") {
    Remove-Item ".\src\client\.env" -Verbose
}

New-Item -ItemType File -Path ".\src\client\.env" -Force
Set-Content -Path ".\src\client\.env" -Value "VITE_BASE_URL = '$backendUrl'"