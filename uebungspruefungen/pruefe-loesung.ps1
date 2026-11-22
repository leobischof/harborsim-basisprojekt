# HarborSim – Übungsprüfungen: Lösung prüfen
# Testet eine Aufgabe mit ihren eigenen Szenarien (Ordner test/ der Aufgabe).
#
#   Musterlösung prüfen (Standard):
#     powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 1-leicht/01-typ-gross-klein
#     -> kopiert das Basisprojekt in einen Temp-Ordner, spielt loesung/aenderungen.patch ein
#        und lässt die Szenarien laufen. Das Projekt selbst bleibt unverändert.
#
#   Eigenen Versuch prüfen:
#     ... pruefe-loesung.ps1 1-leicht/01-typ-gross-klein -Eigene
#     -> testet das Projekt so, wie es gerade im Ordner HarborSim/ liegt (deine Änderungen).
#
#   Gegenprobe:
#     ... pruefe-loesung.ps1 1-leicht/01-typ-gross-klein -OhneLoesung
#     -> unverändertes Basisprojekt; die Szenarien MÜSSEN hier scheitern,
#        sonst prüfen sie die Aufgabe gar nicht (außer reinen Kontroll-Szenarien,
#        die nur zeigen, dass das Bisherige weiter funktioniert).

param(
    [Parameter(Mandatory = $true)][string]$Aufgabe,
    [switch]$Eigene,
    [switch]$OhneLoesung
)

$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
$taskDir = Join-Path $PSScriptRoot $Aufgabe
$testDir = Join-Path $taskDir 'test'
$runTests = Join-Path $repo 'tests/run-tests.ps1'

if (-not (Test-Path -LiteralPath $testDir)) {
    Write-Host "Keine Szenarien gefunden: $testDir" -ForegroundColor Red
    exit 1
}

# Eigener Versuch: direkt das Projekt im Repo testen
if ($Eigene) {
    & powershell -ExecutionPolicy Bypass -File $runTests -Projekt $repo -Szenarien $testDir
    exit $LASTEXITCODE
}

# Sonst: frische Kopie des Basisprojekts (ohne bin/obj) in einen Temp-Ordner
$work = Join-Path ([System.IO.Path]::GetTempPath()) ("harborsim-pruefung-" + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $work | Out-Null
try {
    Copy-Item -LiteralPath (Join-Path $repo 'HarborSim') -Destination $work -Recurse
    foreach ($junk in @('bin', 'obj')) {
        $path = Join-Path $work "HarborSim/$junk"
        if (Test-Path -LiteralPath $path) {
            Remove-Item -LiteralPath $path -Recurse -Force
        }
    }

    if (-not $OhneLoesung) {
        $patch = Join-Path $taskDir 'loesung/aenderungen.patch'
        # git apply funktioniert auch außerhalb eines Git-Repos (wie das Programm "patch")
        Push-Location $work
        git apply --whitespace=nowarn $patch
        $applied = $LASTEXITCODE
        Pop-Location
    }
    else {
        $applied = 0
    }

    if ($applied -ne 0) {
        Write-Host 'Patch lässt sich nicht einspielen.' -ForegroundColor Red
        $result = 1
    }
    else {
        & powershell -ExecutionPolicy Bypass -File $runTests -Projekt $work -Szenarien $testDir
        $result = $LASTEXITCODE
    }
}
finally {
    Remove-Item -LiteralPath $work -Recurse -Force -ErrorAction SilentlyContinue
}
exit $result
