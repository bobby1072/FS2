param (
    [switch] $debug = $false
)
$ErrorActionPreference = "Stop"


if($debug -eq $false)
{
    docker compose -f docker-compose.yml -f docker-compose.debug.yml up --build
} else {
    docker compose up --build
}