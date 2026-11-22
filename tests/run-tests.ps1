# HarborSim – Testskript
# Startet das Programm einmal pro Szenario in tests/scenarios/<name>/:
#   input.txt    = Menüeingaben (eine Eingabe pro Zeile)
#   expected.txt = Soll-Ausgabe
#   actual.txt   = Ist-Ausgabe (wird erzeugt, nicht eingecheckt)
#   ships.csv / workers.csv  = optional, ersetzen für dieses Szenario die normalen Daten
#   fehlende-dateien.txt     = optional, diese Daten-Dateien werden vorher gelöscht
#   erwartete-dateien/       = optional, jede Datei darin muss nach dem Lauf genauso
#                              neben dem Programm liegen (z. B. ein Protokoll, das das Programm schreibt)
# Das Startdatum ist fest (Montag, 23.11.2026), damit die Ausgabe immer gleich ist.
# Aufruf: powershell -ExecutionPolicy Bypass -File tests/run-tests.ps1
# Optional: -Projekt <Ordner> testet eine andere Kopie des Projekts,
#           -Szenarien <Ordner> nimmt andere Szenarien (z. B. die einer Übungsaufgabe).

param(
    [string]$Projekt = (Split-Path -Parent $PSScriptRoot),
    [string]$Szenarien = (Join-Path $PSScriptRoot 'scenarios')
)

$ErrorActionPreference = 'Stop'
$StartDate = '2026-11-23'

$root = (Resolve-Path -LiteralPath $Projekt).Path
$binDir = Join-Path $root 'tests/.bin'
$scenarioDir = (Resolve-Path -LiteralPath $Szenarien).Path

dotnet build (Join-Path $root 'HarborSim/HarborSim.csproj') -o $binDir --nologo -v q | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Host 'Build fehlgeschlagen.' -ForegroundColor Red
    exit 1
}
$dll = Join-Path $binDir 'HarborSim.dll'
$dataSource = Join-Path $root 'HarborSim/Data'
$dataTarget = Join-Path $binDir 'Data'

# Vor jedem Szenario die normalen CSV-Dateien zurückkopieren,
# dann die Dateien des Szenarios darüberlegen bzw. löschen.
function Set-ScenarioData($scenarioPath) {
    Copy-Item -Path (Join-Path $dataSource '*.csv') -Destination $dataTarget -Force
    foreach ($name in @('ships.csv', 'workers.csv')) {
        $own = Join-Path $scenarioPath $name
        if (Test-Path -LiteralPath $own) {
            Copy-Item -LiteralPath $own -Destination $dataTarget -Force
        }
    }
    # Dateien, die das Programm selbst schreiben soll, vorher wegräumen
    $expectedFiles = Join-Path $scenarioPath 'erwartete-dateien'
    if (Test-Path -LiteralPath $expectedFiles) {
        foreach ($file in (Get-ChildItem -LiteralPath $expectedFiles -File)) {
            Remove-Item -LiteralPath (Join-Path $binDir $file.Name) -Force -ErrorAction SilentlyContinue
        }
    }
    $missingList = Join-Path $scenarioPath 'fehlende-dateien.txt'
    if (Test-Path -LiteralPath $missingList) {
        foreach ($name in (Get-Content -LiteralPath $missingList)) {
            Remove-Item -LiteralPath (Join-Path $dataTarget $name.Trim()) -Force
        }
    }
}

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
    Set-ScenarioData $scenario.FullName

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

    # Vom Programm geschriebene Dateien zeilenweise vergleichen
    $fileErrors = @()
    $expectedFiles = Join-Path $scenario.FullName 'erwartete-dateien'
    if (Test-Path -LiteralPath $expectedFiles) {
        foreach ($file in (Get-ChildItem -LiteralPath $expectedFiles -File)) {
            $written = Join-Path $binDir $file.Name
            if (-not (Test-Path -LiteralPath $written)) {
                $fileErrors += "$($file.Name) wurde nicht geschrieben"
                continue
            }
            $soll = @(Read-Lines $file.FullName)
            $ist = @(Read-Lines $written)
            if (($soll -join "`n") -cne ($ist -join "`n")) {
                $fileErrors += "$($file.Name) hat einen anderen Inhalt"
            }
        }
    }

    if ($process.ExitCode -eq 0 -and $firstDiff -eq -1 -and $fileErrors.Count -eq 0) {
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
    foreach ($message in $fileErrors) {
        Write-Host "        $message"
    }
}

# Normale Daten wiederherstellen, damit ein späterer Start aus .bin nichts Kaputtes liest
Set-ScenarioData $scenarioDir

Write-Host ''
Write-Host "$($scenarios.Count - $failed) von $($scenarios.Count) Szenarien OK"
if ($failed -gt 0) {
    exit 1
}
exit 0
