param (
    [switch] $debug = $true
)
$ErrorActionPreference = "Stop"

docker compose up --build

if($debug -eq $true)
{
    docker compose -f docker-compose.debug.yml up --build
}