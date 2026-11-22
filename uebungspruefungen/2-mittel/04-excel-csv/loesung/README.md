# Lösung zu Aufgabe 4 – CSV aus dem österreichischen Excel

Die Änderung steht als Patch in [aenderungen.patch](aenderungen.patch). Betroffen ist nur
`Program.cs`: eine neue Methode `ConvertExcelLines` und ihr Aufruf in `ReadLines`.

## Idee

Die Excel-Zeilen werden **gleich nach dem Einlesen** ins normale Format umgebaut. Der ganze Rest
(Kopfzeile, doppelte Namen, Parsen, Fehlermeldungen) bleibt unverändert und funktioniert
automatisch auch für Excel-Dateien.

```csharp
private static string[] ConvertExcelLines(string[] lines)
{
    if (lines.Length == 0 || !lines[0].Contains(";"))
    {
        return lines;                      // normales Format: nichts zu tun
    }
    for (int i = 0; i < lines.Length; i++)
    {
        lines[i] = lines[i].Replace(',', '.').Replace(';', ',');
    }
    return lines;
}
```

Aufruf in `ReadLines`: `return ConvertExcelLines(File.ReadAllLines(path));`

## Warum so

- **Reihenfolge:** Zuerst `,` → `.` (aus `16,2` wird `16.2`), erst dann `;` → `,`. Andersherum
  würden die neuen Trennzeichen gleich wieder zu Punkten.
- In `ReadLines` deckt eine Stelle beide Dateien ab (Anforderung 1). Ein Durchreichen des
  Trennzeichens durch `FirstDataLine`, `CheckUniqueName`, `ParseShip` und `ParseWorker` würde
  vier Methoden ändern.
- Grenze der Lösung: Ein Komma *im Namen* würde zum Punkt. In den Angabedaten kommt das nicht vor.

## Prüfen

```
powershell -ExecutionPolicy Bypass -File uebungspruefungen/pruefe-loesung.ps1 2-mittel/04-excel-csv
```
