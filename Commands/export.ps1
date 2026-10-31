# Writes each command to dist/ with {{KEY}} placeholders replaced by values from secrets.json,
# so every file in dist/ can be pasted into Streamer.bot as is.
$ErrorActionPreference = 'Stop'

$root = Split-Path $PSScriptRoot -Parent
$secretsPath = Join-Path $root 'secrets.json'
$outDir = Join-Path $root 'dist'

$secrets = @{}
if (Test-Path $secretsPath) {
    (Get-Content $secretsPath -Raw | ConvertFrom-Json).PSObject.Properties | ForEach-Object { $secrets[$_.Name] = $_.Value }
}

New-Item -ItemType Directory -Force $outDir | Out-Null

foreach ($file in Get-ChildItem $PSScriptRoot -Filter *.cs) {
    $text = [IO.File]::ReadAllText($file.FullName)
    foreach ($key in $secrets.Keys) {
        $text = $text.Replace("{{$key}}", $secrets[$key])
    }

    $missing = [regex]::Matches($text, '\{\{([A-Z0-9_]+)\}\}') | ForEach-Object { $_.Groups[1].Value } | Select-Object -Unique
    if ($missing) {
        throw "$($file.Name): secrets.json has no value for $($missing -join ', ')"
    }

    [IO.File]::WriteAllText((Join-Path $outDir $file.Name), $text)
}

Write-Host "Exported commands to $outDir"
