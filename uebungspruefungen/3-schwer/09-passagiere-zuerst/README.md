# Aufgabe 9 – Passagierschiffe haben Vorrang

**Stufe:** schwer · **Zielzeit:** 20 Minuten · **Stoff:** Vererbung, Properties, `IComparer`, Listen (UE05–UE07)

## Ausgangslage

Die Simulation versucht die offenen Tasks in der Reihenfolge der CSV-Datei. Deshalb legen morgens
zuerst Frachtschiffe an, und die Passagiere der Coastal Breeze warten bis nach 8 Uhr. Der Hafen will
das ändern: Passagierschiffe sollen zuerst drankommen.

## Anforderungen

1. Jedes Schiff bekommt eine Property `HasPriority` (nur lesen). Für normale Schiffe ist sie `false`,
   für Passagierschiffe `true`. Die Entscheidung gehört in die Schiffsklassen, nicht in eine
   Typ-Abfrage in `Harbor`.
2. Schreibe eine neue Klasse `DockingOrderComparer`, die `IComparer<Ship>` umsetzt:
   - Schiffe mit Vorrang kommen vor Schiffen ohne Vorrang.
   - Haben zwei Schiffe denselben Vorrang, entscheidet die `Id` (kleinere zuerst). So bleibt
     innerhalb jeder Gruppe die Reihenfolge der CSV erhalten.
3. `Harbor` legt die Tasks (Docking, Loading, Undocking je Schiff) in dieser sortierten Reihenfolge
   an: zuerst alle Tasks der Passagierschiffe, dann die der übrigen Schiffe.
4. Die Übersicht bleibt unverändert (Schiffe weiterhin nach Länge sortiert).
5. Vorrang heißt nur "wird zuerst versucht". Fehlt einem Passagierschiff gerade das Personal oder
   ein freies Dock, startet wie bisher der nächste Task, der möglich ist. Das macht der vorhandene
   Code von selbst.

## Beispielausgabe

Mit den normalen Daten und 3 Docks (bisher legten um 06:00 Baltic Venture, Silver Mariner und
Emerald Trader an):

```
=== Simulation ===
06:00  START  Docking   | Coastal Breeze | 06:00-06:30 | Linda, Elias, Freya, Gabriel
       -> Coastal Breeze liegt an Dock 1
06:00  START  Docking   | Seaside Spirit | 06:00-06:30 | Martin, Helena, Isaac, Bianca
       -> Seaside Spirit liegt an Dock 2
06:00  START  Docking   | Baltic Venture | 06:00-06:30 | Caleb, Diana, Adrian
       -> Baltic Venture liegt an Dock 3
...
07:09  START  Docking   | Silver Mariner | 07:09-07:39 | Linda, Elias, Freya, Bianca
       -> Silver Mariner liegt an Dock 1
...
07:14  START  Docking   | Royal Voyager | 07:14-07:44 | Martin, Gabriel, Helena, Isaac
...
Dauer: 9 h 48 min
Abgeschlossene Tasks: 28 von 30
```

Baltic Venture (Fracht) darf schon um 06:00 andocken: Royal Voyager und die anderen großen
Passagierschiffe brauchen einen Experten-Lotsen, und den hat gerade Seaside Spirit. Genauso kommt
Silver Mariner um 07:09 vor Royal Voyager dran.

## Testszenarien

- `test/normale-daten/`: Eingaben `2`, `0`. Ganze Simulation mit den normalen Daten.
- `test/viele-schiffe/`: Eingaben `2`, `0`, eigene `ships.csv` mit 18 kleinen Schiffen,
  jedes dritte ist ein Passagierschiff. Hier zeigt sich, ob die `Id` bei Gleichstand wirklich
  entscheidet (siehe Hinweise).

## Hinweise

- Betroffen: `Ship` und `PassengerShip` (Ordner `Ships/`), `Harbor` (Ordner `Simulation/`) und die
  neue Klasse `DockingOrderComparer`. Vorlage für den Comparer ist `ShipComparer.cs`.
- `List<T>.Sort` ist **nicht stabil**: Elemente, die der Comparer als gleich meldet (Rückgabe `0`),
  können dabei die Plätze tauschen. Bei wenigen Elementen fällt das nicht auf, bei 18 Schiffen
  schon. Deshalb der Vergleich der `Id` am Schluss.
- Sortiere eine Kopie der Schiffsliste (`new List<Ship>(_ships)`), nicht `_ships` selbst.

## Testen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 3-schwer/09-passagiere-zuerst -Eigene
```
