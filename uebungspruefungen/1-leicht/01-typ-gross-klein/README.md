# Aufgabe 1 – Schiffstyp unabhängig von Groß-/Kleinschreibung

**Stufe:** leicht · **Zielzeit:** 10 Minuten · **Stoff:** Strings, `switch` (UE03)

## Ausgangslage

Die Reederei schickt ihre `ships.csv` neuerdings aus einem anderen Programm. Dort steht der
Schiffstyp mal als `cargo`, mal als `PASSENGER`, mal wie bisher als `Cargo`. Das Basisprojekt kennt
nur die exakte Schreibweise `Cargo`/`Passenger` und überspringt alle anderen Zeilen.

## Anforderungen

1. Der Schiffstyp in `ships.csv` wird **unabhängig von Groß- und Kleinschreibung** erkannt:
   `cargo`, `Cargo`, `CARGO`, `cArGo` sind alle ein Frachtschiff, dasselbe gilt für `Passenger`.
2. Ein wirklich unbekannter Typ (z. B. `Tanker`) wird weiterhin übersprungen, mit derselben
   Meldung wie bisher. In der Meldung steht der Typ so, wie er in der Datei steht.
3. Sonst ändert sich nichts an der Ausgabe.

## Testdaten

`test/gemischte-schreibweise/ships.csv`:

```
Name,Type,Length,Beam,Draft,Val1
Baltic Venture,cargo,92,16.2,5.4,120
Coastal Breeze,PASSENGER,108,18.2,4.8,160
Silver Mariner,Cargo,146,22.8,7.1,340
Seaside Spirit,passenger,156,23.5,5.9,420
Black Pearl,Tanker,120,20.0,6.0,500
```

## Beispielausgabe (Anfang)

```
Zeile 6 in ships.csv übersprungen: unbekannter Schiffstyp "Tanker"
HarborSim – Hafensimulation
4 Schiffe und 20 Mitarbeiter eingelesen.
```

Im Basisprojekt stehen hier stattdessen drei Meldungen (`"cargo"`, `"PASSENGER"`, `"passenger"`)
und nur 1 eingelesenes Schiff. Die vollständige Soll-Ausgabe liegt in
`test/gemischte-schreibweise/expected.txt`.

## Hinweise

- Betroffen ist nur **eine Methode** in `Program.cs`: die, die aus einer CSV-Zeile ein Schiff macht.
- Python-Gegenstück: `text.lower()`.

## Testen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 1-leicht/01-typ-gross-klein -Eigene
```
