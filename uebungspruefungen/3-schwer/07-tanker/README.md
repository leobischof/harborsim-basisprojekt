# Aufgabe 7 – Neuer Schiffstyp Tanker

**Stufe:** schwer · **Zielzeit:** 20 Minuten · **Stoff:** Vererbung, abstrakte Klassen, Exceptions, Dateien (UE04–UE07)

## Ausgangslage

Der Hafen fertigt ab sofort auch Tanker ab. In `ships.csv` stehen sie mit dem Typ `Tanker`;
`Val1` ist bei ihnen die Ladung in **m³** (ganze Zahl). Bisher überspringt das Programm solche
Zeilen mit `unbekannter Schiffstyp "Tanker"`.

## Anforderungen

1. Eine neue Klasse `TankerShip` erbt von `Ship`. `ParseShip` legt für den Typ `Tanker` ein
   `TankerShip` an.
2. Eine negative Ladung ist ein Fehler: Die Zeile wird übersprungen mit dem Grund
   `<Name>: Die Ladung darf nicht negativ sein.`
3. **Personal beim Beladen:** der normale Bedarf aller Schiffe (Hafenarbeiter nach Länge) und
   zusätzlich Hafenarbeiter (`DockWorker`) für die Schläuche: je angefangene 5000 m³ einer,
   bei mehr als 10000 m³ ist einer davon qualifiziert, bei mehr als 20000 m³ ist einer davon Experte.
4. **Dauer des Beladens:** 20 Minuten + m³ / 200, auf ganze Minuten **aufgerundet**.
   Andocken und Ablegen dauern wie bei allen Schiffen.
5. **Beschreibung:** wie bei allen Schiffen, dahinter `, Tanker mit <m³> m³ Ladung`.
6. **Beladen:** gibt `       -> <Name>: <m³> m³ gepumpt` aus (gleiche Einrückung wie bei den
   anderen Schiffen).

## Beispielausgabe

Aus der Simulation mit zwei Tankern (Szenario `mit-tankern`):

```
Schiffe, sortiert nach Länge:
  #5 Baltic Venture, 92 m lang, 16,2 m breit, 5,4 m Tiefgang, Frachtschiff mit 120 Containern
  #8 Polarstern, 96 m lang, 17,4 m breit, 6,3 m Tiefgang, Tanker mit 3500 m³ Ladung
  ...
06:30  START  Loading   | Nordlicht | 06:30-07:50 | Freya, Gabriel, Helena, Isaac, Caleb, Diana, Adrian
...
07:50  ENDE   Loading   | Nordlicht | 06:30-07:50 | Freya, Gabriel, Helena, Isaac, Caleb, Diana, Adrian
       -> Nordlicht: 12000 m³ gepumpt
```

Nordlicht (182 m, 12000 m³) braucht beim Beladen 4 Hafenarbeiter nach Länge (einer Experte) und
3 für die Schläuche (einer qualifiziert), zusammen also 7. Das Beladen dauert 20 + 60 = 80 Minuten.

## Testszenarien

- `test/mit-tankern/`: eigene `ships.csv` mit zwei Tankern, Eingaben `2`, `0`. Ganze Simulation.
- `test/negative-ladung/`: eigene `ships.csv` mit einem Tanker mit `-100` m³, Eingabe `0`.

## Hinweise

- Vorbild ist `CargoShip` im Ordner `Ships/`: Konstruktor mit Prüfung, `LoadingStaff`,
  `GetDuration`, `GetDescription` und `Load`.
- `Load` ist in `Ship` `abstract`: Ohne eigenes `Load` lässt sich `TankerShip` nicht kompilieren.
- Teilen mit `200` statt `200.0` rechnet mit ganzen Zahlen (Python: `//` statt `/`).
- Das Basis-Szenario `tests/scenarios/kaputte-csv` erwartet noch `unbekannter Schiffstyp "Tanker"`.
  Nach deiner Änderung scheitert es dort, das ist bei dieser Aufgabe richtig.

## Testen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 3-schwer/07-tanker -Eigene
```
