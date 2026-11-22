# Lösung zu Aufgabe 6 – Fehlerprotokoll in eine Datei

Die Änderung steht als Patch in [aenderungen.patch](aenderungen.patch). Betroffen ist nur
`Program.cs`: zwei neue Felder, `PrintSkippedLine` merkt sich jede Meldung, und `Main` schreibt
die Datei.

## Idee

`PrintSkippedLine` ist die eine Stelle, an der jede Meldung vorbeikommt. Dort wird die Meldung
zusätzlich in einer statischen Liste gesammelt:

```csharp
private static List<string> _skippedLines = new List<string>();   // Python: skipped_lines = []

private static void PrintSkippedLine(string path, int index, string reason)
{
    string message = $"Zeile {index + 1} in {Path.GetFileName(path)} übersprungen: {reason}";
    Console.WriteLine(message);
    _skippedLines.Add(message);
}
```

In `Main`, gleich nach der Zeile `… eingelesen.`, wird die Liste **einmal** geschrieben:

```csharp
File.WriteAllLines(ErrorLogPath, _skippedLines);
if (_skippedLines.Count > 0)
{
    Console.WriteLine($"{_skippedLines.Count} fehlerhafte Zeilen in fehler.log protokolliert.");
}
```

Der Pfad steht als Feld neben den anderen: `Path.Combine(AppContext.BaseDirectory, "fehler.log")`.

## Warum so

- **Eine Meldung, zwei Ziele:** Konsole und Datei bekommen denselben String. So können sie nicht
  auseinanderlaufen (Anforderung 2).
- **Zeitpunkt:** Die Simulation liest die CSV-Dateien erneut ein und ruft dabei wieder
  `PrintSkippedLine` auf. Die Liste wächst also weiter. Geschrieben wird aber nur einmal, gleich
  nach dem ersten Einlesen. Wer die Datei erst am Programmende schreibt oder mit
  `File.AppendAllText` anhängt, hat nach einem Simulationslauf jede Meldung doppelt. Das prüft
  `kaputte-daten-mit-lauf`.
- `WriteAllLines` überschreibt immer und schreibt auch eine leere Liste. Damit sind Anforderung 3
  und das Szenario `normale-daten` ohne eigenen Sonderfall erfüllt.

## Prüfen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 2-mittel/06-fehlerprotokoll
```
