@echo off

REM Clean existing dev certificate
dotnet dev-certs https --clean

REM Set the password for the development certificate
dotnet user-secrets -p .\VantageView.Api\VantageView.Api.csproj set "Kestrel:Certificates:Development:Password" "crypticpassword"
dotnet user-secrets -p .\VantageView.Gateway\VantageView.Gateway.csproj set "Kestrel:Certificates:Development:Password" "crypticpassword"
dotnet user-secrets -p .\VantageView.Administration\VantageView.Administration.csproj set "Kestrel:Certificates:Development:Password" "crypticpassword"
REM Create a new development certificate
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\VantageView.Api.pfx -p crypticpassword --trust
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\VantageView.Gateway.pfx -p crypticpassword --trust
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\VantageView.Administration.pfx -p crypticpassword --trust

REM Pause to keep the command window open
pause
