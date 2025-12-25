# PowerShell script to run Docker-based integration tests
# Usage: .\run-docker-tests.ps1 [-SkipBuild] [-KeepContainers] [-Filter <pattern>]

param(
    [switch]$SkipBuild,
    [switch]$KeepContainers,
    [string]$Filter = ""
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "DevBase.Net Docker Integration Tests" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Check Docker is running
Write-Host "Checking Docker..." -ForegroundColor Yellow
try {
    docker version | Out-Null
    Write-Host "  Docker is available" -ForegroundColor Green
} catch {
    Write-Host "  ERROR: Docker is not running or not installed" -ForegroundColor Red
    Write-Host "  Please start Docker Desktop and try again" -ForegroundColor Red
    exit 1
}

# Start Docker containers
Write-Host ""
Write-Host "Starting Docker containers..." -ForegroundColor Yellow
Push-Location $ScriptDir

try {
    if ($SkipBuild) {
        docker compose up -d
    } else {
        docker compose up -d --build
    }
    
    Write-Host "  Containers started" -ForegroundColor Green
} catch {
    Write-Host "  ERROR: Failed to start containers: $_" -ForegroundColor Red
    Pop-Location
    exit 1
}

# Wait for services to be healthy
Write-Host ""
Write-Host "Waiting for services to be ready..." -ForegroundColor Yellow

$maxAttempts = 30
$attempt = 0
$ready = $false

while ($attempt -lt $maxAttempts -and -not $ready) {
    $attempt++
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5080/health" -TimeoutSec 2 -ErrorAction SilentlyContinue
        if ($response.StatusCode -eq 200) {
            $ready = $true
            Write-Host "  Mock API is ready" -ForegroundColor Green
        }
    } catch {
        Write-Host "  Waiting... ($attempt/$maxAttempts)" -ForegroundColor Gray
        Start-Sleep -Seconds 2
    }
}

if (-not $ready) {
    Write-Host "  WARNING: Mock API may not be ready, proceeding anyway" -ForegroundColor Yellow
}

# Check proxy ports
$proxyPorts = @(
    @{ Name = "HTTP Proxy (auth)"; Port = 8888 },
    @{ Name = "HTTP Proxy (no auth)"; Port = 8889 },
    @{ Name = "SOCKS5 Proxy (auth)"; Port = 1080 },
    @{ Name = "SOCKS5 Proxy (no auth)"; Port = 1081 }
)

foreach ($proxy in $proxyPorts) {
    try {
        $tcpClient = New-Object System.Net.Sockets.TcpClient
        $tcpClient.Connect("localhost", $proxy.Port)
        $tcpClient.Close()
        Write-Host "  $($proxy.Name) is ready (port $($proxy.Port))" -ForegroundColor Green
    } catch {
        Write-Host "  WARNING: $($proxy.Name) may not be ready (port $($proxy.Port))" -ForegroundColor Yellow
    }
}

Pop-Location

# Run tests
Write-Host ""
Write-Host "Running integration tests..." -ForegroundColor Yellow
Write-Host ""

$testProjectPath = Join-Path (Split-Path -Parent $ScriptDir) ".." ".." ".." "DevBase.Test.csproj"
$testProjectPath = Resolve-Path $testProjectPath

$testArgs = @(
    "test",
    $testProjectPath,
    "--filter", "Category=Docker",
    "--verbosity", "normal",
    "--logger", "console;verbosity=detailed"
)

if ($Filter) {
    $testArgs += "--filter"
    $testArgs += "Category=Docker&FullyQualifiedName~$Filter"
}

try {
    & dotnet @testArgs
    $testExitCode = $LASTEXITCODE
} catch {
    Write-Host "ERROR: Test execution failed: $_" -ForegroundColor Red
    $testExitCode = 1
}

# Cleanup
if (-not $KeepContainers) {
    Write-Host ""
    Write-Host "Stopping Docker containers..." -ForegroundColor Yellow
    Push-Location $ScriptDir
    docker compose down -v --remove-orphans
    Pop-Location
    Write-Host "  Containers stopped" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "Containers left running (use -KeepContainers:$false to stop)" -ForegroundColor Yellow
    Write-Host "  To stop manually: docker compose down -v" -ForegroundColor Gray
}

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
if ($testExitCode -eq 0) {
    Write-Host "Tests completed successfully!" -ForegroundColor Green
} else {
    Write-Host "Tests completed with failures" -ForegroundColor Red
}
Write-Host "============================================" -ForegroundColor Cyan

exit $testExitCode
