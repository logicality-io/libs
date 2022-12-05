$ErrorActionPreference = "Stop"

$GitHubToken=$Env:GITHUB_TOKEN
$GitHubRunNumber=$Env:GITHUB_RUN_NUMBER
$NugetOrgApiKey=$Env:LOGICALITY_NUGET_ORG

if ($GitHubToken -eq $null -or $GitHubToken -eq "") {
	Write-Warning "GITHUB_TOKEN environment variable empty or missing."
}

if ($GitHubRunNumber -eq $null -or $GitHubRunNumber -eq "") {
	Write-Warning "GITHUB_RUN_NUMBER environment variable empty or missing."
}

if ($NugetOrgApiKey -eq $null -or $NugetOrgApiKey -eq "") {
	Write-Warning "LOGICALITY_NUGET_ORG environment variable empty or missing."
}

dotnet run --project build/Build.csproj -c Release -- $args