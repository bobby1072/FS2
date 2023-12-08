param (
    [switch] $debug = $false,
    [switch] $buildFront = $false
)
$ErrorActionPreference = "Stop"

if($buildFront){
    npm install --prefix src/client
    npm run build --prefix src/client
}

if($debug -eq $false)
{
    docker compose -f docker-compose.yml -f docker-compose.debug.yml up -d --build
} else {
    docker compose up -d --build
}

npm start --prefix src/client