# Übungsprüfungen zum Erweiterungsteil

Aufgaben im Stil des Erweiterungsteils der Prüfung: Das Basisprojekt bekommt eine neue Anforderung,
die du in 10 bis 25 Minuten einbaust. Verwendet wird nur Stoff aus UE03 bis UE07 (Klassen,
Vererbung, Interfaces, Listen, Arrays, Dateien, Exceptions), kein LINQ, keine Lambdas.

| Nr. | Aufgabe | Stufe | Zielzeit | Betroffen |
|---|---|---|---|---|
| 01 | [Schiffstyp unabhängig von Groß-/Kleinschreibung](1-leicht/01-typ-gross-klein/README.md) | leicht | 10 min | `Program` |
| 02 | [Sicherheitsregel für große Passagierschiffe](1-leicht/02-lotse-grosse-passagierschiffe/README.md) | leicht | 10 min | `PassengerShip` |
| 03 | [Container und Passagiere gesamt in der Übersicht](1-leicht/03-summen-in-uebersicht/README.md) | leicht | 10 min | `Harbor`, `CargoShip`, `PassengerShip` |
| 04 | [CSV aus dem österreichischen Excel](2-mittel/04-excel-csv/README.md) | mittel | 15 min | `Program` |
| 05 | [Schiffe nach Name sortieren](2-mittel/05-sortieren-nach-name/README.md) | mittel | 15 min | `Program`, neue Klasse `ShipNameComparer` |
| 06 | [Fehlerprotokoll in eine Datei](2-mittel/06-fehlerprotokoll/README.md) | mittel | 15 min | `Program` |
| 07 | [Neuer Schiffstyp Tanker](3-schwer/07-tanker/README.md) | schwer | 20 min | `Program`, neue Klasse `TankerShip` |
| 08 | [Docks mit Wassertiefe](3-schwer/08-docks-wassertiefe/README.md) | schwer | 20 min | `Harbor` |
| 09 | [Passagierschiffe haben Vorrang](3-schwer/09-passagiere-zuerst/README.md) | schwer | 20 min | `Ship`, `PassengerShip`, `Harbor`, neue Klasse `DockingOrderComparer` |

## So übst du

1. Angabe lesen (`README.md` oder `Angabe.pdf` im Aufgabenordner), Stoppuhr starten.
2. Direkt im normalen Projekt `HarborSim/` arbeiten. Wo du etwas ändern musst, verraten die
   **Hinweise** in der Angabe.
3. Eigenen Versuch testen (PowerShell, im Repo-Ordner):
   ```
   powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 1-leicht/01-typ-gross-klein -Eigene
   ```
   Das Skript spielt die Szenarien aus dem Ordner `test/` der Aufgabe durch und vergleicht die
   Ausgabe mit `expected.txt`. Bei einem Fehler zeigt es die erste abweichende Zeile (Soll/Ist).
4. Danach die Musterlösung in `loesung/` ansehen und dein Projekt zurücksetzen:
   `git checkout -- HarborSim` (oder die ZIP neu entpacken).

## Aufbau einer Aufgabe

```
1-leicht/01-typ-gross-klein/
├── README.md            ← Angabe
├── Angabe.pdf           ← dieselbe Angabe als PDF (python tools/make_pdf.py)
├── test/<szenario>/     ← input.txt, expected.txt, eventuell eigene ships.csv/workers.csv
│                           und erwartete-dateien/ (Dateien, die das Programm schreiben soll)
└── loesung/
    ├── README.md        ← Erklärung der Lösung
    └── aenderungen.patch← nur die geänderten Zeilen gegenüber dem Basisprojekt
```

Die Musterlösung ist bewusst **kein** kompletter Projektordner, sondern ein Patch (Unified Diff):
`-` = Zeile fällt weg, `+` = Zeile kommt dazu, der Rest ist Kontext. So steht der unveränderte Code
nicht in jeder Lösung noch einmal.

Musterlösung automatisch prüfen (kopiert das Basisprojekt in einen Temp-Ordner, spielt den Patch
ein und testet; dein Projekt bleibt unverändert):

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 1-leicht/01-typ-gross-klein
```

Mit `-OhneLoesung` läuft dieselbe Prüfung auf dem unveränderten Basisprojekt. Dort müssen die
Szenarien scheitern, sonst würden sie die Aufgabe gar nicht prüfen. Ausnahme: Szenarien, die nur
sicherstellen, dass das Bisherige weiter funktioniert (z. B. `normales-format` in Aufgabe 4).
