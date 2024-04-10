# FS2 (fishing suite 2)

## Setup

1. In the repo run:
   ```
   dotnet dev-certs https -ep .aspnet/https/fs.pfx -p password -t -v
   dotnet dev-certs https --trust
   ```
2. Enter the client directory in the cmd using command:
   ```
   cd src/client
   ```
3. Install the frontend packages using:
   ```
   npm i
   ```
4. Make sure Docker Desktop is open. Open a fresh powershell terminal and run (if you want to run the backend manually add '-debug' after the command):
   ```
   .\startup.ps1
   ```
