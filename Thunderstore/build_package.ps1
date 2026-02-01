# CaptainAudio Thunderstore Package Builder
# Usage: .\build_package.ps1

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDir = Split-Path -Parent $scriptDir
$outputDir = Join-Path $scriptDir "package"
$zipName = "CaptainAudio-1.2.2.zip"

Write-Host "=== CaptainAudio Thunderstore Package Builder ===" -ForegroundColor Cyan

# Clean and create output directory
if (Test-Path $outputDir) {
    Remove-Item -Recurse -Force $outputDir
}
New-Item -ItemType Directory -Path $outputDir | Out-Null

# Copy required files
Write-Host "Copying files..." -ForegroundColor Yellow

# manifest.json
Copy-Item (Join-Path $scriptDir "manifest.json") $outputDir

# README.md
Copy-Item (Join-Path $scriptDir "README.md") $outputDir

# CHANGELOG.md
Copy-Item (Join-Path $scriptDir "CHANGELOG.md") $outputDir

# icon.png (required by Thunderstore)
$iconPath = Join-Path $scriptDir "icon.png"
if (Test-Path $iconPath) {
    Copy-Item $iconPath $outputDir
} else {
    Write-Host "WARNING: icon.png not found! Thunderstore requires a 256x256 PNG icon." -ForegroundColor Red
    Write-Host "Please add icon.png to: $scriptDir" -ForegroundColor Red
}

# DLL file
$dllPath = "C:\temp\CaptainAudio\bin\netstandard2.1\CaptainAudio.dll"
if (Test-Path $dllPath) {
    Copy-Item $dllPath $outputDir
    Write-Host "DLL copied from: $dllPath" -ForegroundColor Green
} else {
    Write-Host "ERROR: DLL not found at $dllPath" -ForegroundColor Red
    Write-Host "Please build the project first: dotnet build CaptainAudio.csproj -c Release" -ForegroundColor Red
    exit 1
}

# Create ZIP
Write-Host "Creating ZIP package..." -ForegroundColor Yellow
$zipPath = Join-Path $scriptDir $zipName

if (Test-Path $zipPath) {
    Remove-Item $zipPath
}

Compress-Archive -Path (Join-Path $outputDir "*") -DestinationPath $zipPath

Write-Host ""
Write-Host "=== Package Created ===" -ForegroundColor Green
Write-Host "Location: $zipPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "Package contents:" -ForegroundColor Yellow
Get-ChildItem $outputDir | ForEach-Object { Write-Host "  - $($_.Name)" }

Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Ensure icon.png (256x256) is included" -ForegroundColor White
Write-Host "2. Upload to https://valheim.thunderstore.io/" -ForegroundColor White
