param (
    [switch] $debug = $false,
    [switch] $production = $false
)
$ErrorActionPreference = "Stop"

if($production -eq $true){
    docker compose -f docker-compose.yml -f docker-compose.production.debug.yml up -d --build
}
elseif($debug -eq $false)
{
    docker compose -f docker-compose.yml -f docker-compose.debug.yml up -d --build
} else {
    docker compose up -d --build
}

npm start --prefix src/client