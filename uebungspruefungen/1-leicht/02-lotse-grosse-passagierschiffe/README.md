# Aufgabe 2 – Sicherheitsregel für große Passagierschiffe

**Stufe:** leicht · **Zielzeit:** 10 Minuten · **Stoff:** Vererbung, `override`, `if` (UE03–UE05)

## Ausgangslage

Die Hafenbehörde hat eine neue Sicherheitsregel erlassen: Wenn sehr viele Passagiere an Bord gehen,
muss beim Beladen zusätzlich ein erfahrener Lotse auf dem Schiff sein.

## Anforderungen

1. Ein **Passagierschiff mit mehr als 1500 Passagieren** braucht beim **Beladen** (Loading)
   zusätzlich **einen Lotsen (Pilot) mit Level Expert**.
2. Bei genau 1500 Passagieren oder weniger bleibt alles wie bisher.
3. Frachtschiffe und die Tasks Docking/Undocking ändern sich nicht.
4. Steht kein freier Experten-Lotse zur Verfügung, wartet der Task wie bei jedem anderen fehlenden
   Personal (das erledigt der bestehende Code schon).

## Testdaten

`test/grenzfall-1500/ships.csv`:

```
Name,Type,Length,Beam,Draft,Val1
Seaside Spirit,Passenger,156,23.5,5.9,420
Ocean Dream,Passenger,180,25.0,6.5,1500
Grand Horizon,Passenger,316,44.6,8.9,3200
```

## Beispielausgabe (Ausschnitt)

```
07:00  START  Loading   | Ocean Dream | 07:00-07:45 | Helena, Isaac, Bianca, Adrian, Ulrich, Tina
...
08:05  START  Loading   | Grand Horizon | 08:05-09:24 | Martin, Elias, Freya, Gabriel, Helena, Isaac, Bianca, Adrian, Ulrich, Thomas, Vera, Jana, Tina
```

Ocean Dream (genau 1500) bekommt keinen Lotsen, Grand Horizon (3200) bekommt Martin, den einzigen
Experten-Lotsen. Die vollständige Soll-Ausgabe liegt in `test/grenzfall-1500/expected.txt`.

## Hinweise

- Den Personalbedarf beim Beladen berechnet jede Schiffsklasse selbst in der Property
  `LoadingStaff`. Gesucht ist die Version in `Ships/PassengerShip.cs`.
- Personal fügt man einer `SkillMatrix` mit `Add(skillType, level, anzahl)` hinzu.

## Testen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 1-leicht/02-lotse-grosse-passagierschiffe -Eigene
```
