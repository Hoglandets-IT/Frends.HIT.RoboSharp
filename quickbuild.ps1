param(
    [string]$APIKEY
)
$ErrorActionPreference = "Stop"

$VER=$(Get-Content -Path .\Version).Split(".")
$VER[2] = [int]$VER[2] + 1
$VER = $VER -join "."
Write-Host "Building version: $VER"
dotnet pack --configuration Release --include-source --output . /p:Version=$VER .\Frends.HIT.RoboSharp\Frends.HIT.RoboSharp.csproj
dotnet nuget push .\Frends.HIT.RoboSharp.$VER.nupkg --source https://proget.hoglan.dev/nuget/Frends/ --api-key "$APIKEY"
Set-Content -PATH .\VERSION -Value $VER
Remove-Item -Path ./Frends.HIT.RoboSharp.*.nupkg