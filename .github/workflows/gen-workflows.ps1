# Generates a workflow yml file for each library using the template

$libs = @("aspnet-core", "bullseye", "configuration", "hosting", "lambda", "pulumi", "system-extensions", "testing")

$libs |ForEach-Object {
    $workflow = "$_-ci.yml"
    Copy-Item -Path lib.yml.template -Destination $workflow -Force
    ((Get-Content -Path $workflow -Raw) -replace "<lib>", "$_" ) | Set-Content -Path $workflow
}