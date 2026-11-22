# Lösung zu Aufgabe 1 – Schiffstyp unabhängig von Groß-/Kleinschreibung

Die Änderung steht als Patch in [aenderungen.patch](aenderungen.patch): nur die geänderten Zeilen,
`-` = Zeile fällt weg, `+` = Zeile kommt dazu. Der restliche Code ist das Basisprojekt.

## Idee

Vor dem Vergleich wird der Typ in Kleinbuchstaben umgewandelt, und die `case`-Zweige vergleichen
mit kleingeschriebenen Texten. Damit landen alle Schreibweisen im selben Zweig.

```csharp
switch (type.ToLower())      // Python: match type.lower():
{
    case "cargo":
        return new CargoShip(name, length, beam, draft, val1);
    case "passenger":
        return new PassengerShip(name, length, beam, draft, val1);
    default:
        throw new FormatException($"unbekannter Schiffstyp \"{type}\"");
}
```

## Warum so

- `type` selbst bleibt unverändert, deshalb zeigt die Fehlermeldung im `default`-Zweig den Typ so,
  wie er in der Datei steht (Anforderung 2).
- `ToLower()` liefert einen **neuen** String, Strings ändern sich in C# (wie in Python) nie selbst.

## Prüfen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 1-leicht/01-typ-gross-klein
```
