# HarborSim – Testskript
# Startet das Programm einmal pro Szenario in tests/scenarios/<name>/:
#   input.txt    = Menüeingaben (eine Eingabe pro Zeile)
#   expected.txt = Soll-Ausgabe
#   actual.txt   = Ist-Ausgabe (wird erzeugt, nicht eingecheckt)
# Das Startdatum ist fest (Montag, 23.11.2026), damit die Ausgabe immer gleich ist.
# Aufruf: powershell -ExecutionPolicy Bypass -File tests/run-tests.ps1

$ErrorActionPreference = 'Stop'
$StartDate = '2026-11-23'

$root = Split-Path -Parent $PSScriptRoot
$binDir = Join-Path $PSScriptRoot '.bin'
$scenarioDir = Join-Path $PSScriptRoot 'scenarios'

dotnet build (Join-Path $root 'HarborSim/HarborSim.csproj') -o $binDir --nologo -v q | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Host 'Build fehlgeschlagen.' -ForegroundColor Red
    exit 1
}
$dll = Join-Path $binDir 'HarborSim.dll'

# Zeilen ohne Leerzeichen am Ende und ohne Leerzeilen am Schluss.
# Die Eingabeaufforderung "Auswahl: " endet sonst mit einem Leerzeichen.
function Read-Lines($path) {
    $lines = @(Get-Content -LiteralPath $path -Encoding UTF8 | ForEach-Object { $_.TrimEnd() })
    $count = $lines.Count
    while ($count -gt 0 -and $lines[$count - 1] -eq '') {
        $count--
    }
    if ($count -eq 0) {
        return @()
    }
    return $lines[0..($count - 1)]
}

$failed = 0
$scenarios = Get-ChildItem -LiteralPath $scenarioDir -Directory | Sort-Object Name
foreach ($scenario in $scenarios) {
    $inputFile = Join-Path $scenario.FullName 'input.txt'
    $expectedFile = Join-Path $scenario.FullName 'expected.txt'
    $actualFile = Join-Path $scenario.FullName 'actual.txt'

    $process = Start-Process -FilePath 'dotnet' -ArgumentList "`"$dll`"", $StartDate `
        -RedirectStandardInput $inputFile -RedirectStandardOutput $actualFile `
        -NoNewWindow -Wait -PassThru

    $expected = Read-Lines $expectedFile
    $actual = Read-Lines $actualFile

    # Erste abweichende Zeile suchen
    $firstDiff = -1
    $max = [Math]::Max($expected.Count, $actual.Count)
    for ($i = 0; $i -lt $max; $i++) {
        if ($i -ge $expected.Count -or $i -ge $actual.Count -or $expected[$i] -cne $actual[$i]) {
            $firstDiff = $i
            break
        }
    }

    if ($process.ExitCode -eq 0 -and $firstDiff -eq -1) {
        Write-Host "OK      $($scenario.Name)" -ForegroundColor Green
        continue
    }

    $failed++
    Write-Host "FEHLER  $($scenario.Name) (Exit-Code $($process.ExitCode))" -ForegroundColor Red
    if ($firstDiff -ge 0) {
        $soll = '<Ende>'
        $ist = '<Ende>'
        if ($firstDiff -lt $expected.Count) { $soll = $expected[$firstDiff] }
        if ($firstDiff -lt $actual.Count) { $ist = $actual[$firstDiff] }
        Write-Host "        Zeile $($firstDiff + 1)"
        Write-Host "        Soll: $soll"
        Write-Host "        Ist:  $ist"
    }
}

Write-Host ''
Write-Host "$($scenarios.Count - $failed) von $($scenarios.Count) Szenarien OK"
if ($failed -gt 0) {
    exit 1
}
exit 0
