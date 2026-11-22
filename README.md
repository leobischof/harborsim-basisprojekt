# HarborSim – Basisprojekt

Hafensimulation in C# für die Lehrveranstaltung Ingenieurinformatik II (TU Graz, WS 2026/27).
Das ist das Basisprojekt für die Prüfung: fertig, lauffähig und für einen Python-Leser kommentiert.

- **Kommentarfreie Fassung** (für die Prüfung): Branch [`ohne-kommentare`](../../tree/ohne-kommentare),
  Download über *Code → Download ZIP*. Der Branch wird bei jedem Push auf `main` automatisch neu erzeugt.
- **Übungen** zu UE03 bis UE07: [ingenieurinformatik-2-uebungen](https://github.com/leobischof/ingenieurinformatik-2-uebungen)
- **Fachbegriffe** Englisch ↔ Deutsch ↔ Python: [GLOSSARY.md](GLOSSARY.md)
- **Übungsprüfungen** zum Erweiterungsteil (10–25 min, mit Tests und Musterlösung):
  [uebungspruefungen/](uebungspruefungen/README.md)

> Die Angabe (PDF, Klassendiagramm, Flowcharts) ist urheberrechtlich geschützt (© TU Graz) und
> deshalb nicht im Repo. Unten steht sie in eigenen Worten.

## Starten

```
dotnet run --project HarborSim
dotnet run --project HarborSim -- 2026-11-23      # mit festem Startdatum (yyyy-MM-dd)
```

In Zed: Terminal öffnen (`` Ctrl+` ``) und den Befehl im Repo-Ordner eingeben.

Tests (PowerShell): `./tests/run-tests.ps1`. Das Skript baut das Programm, spielt jedes Szenario in
`tests/scenarios/` über `input.txt` durch und vergleicht die Ausgabe mit `expected.txt`.
Liegen im Szenario-Ordner eigene `ships.csv`/`workers.csv`, ersetzen sie für diesen Lauf die normalen
Daten; `fehlende-dateien.txt` listet Daten-Dateien, die vorher gelöscht werden (siehe `kaputte-csv`,
`fehlende-datei`).
Mit `-Projekt <Ordner>` und `-Szenarien <Ordner>` testet das Skript eine andere Projektkopie bzw. andere
Szenarien; das nutzt `uebungspruefungen/pruefe-loesung.ps1`.

Kommentarfreie Fassung lokal erzeugen: `python tools/strip_comments.py ../harborsim-ohne-kommentare`

Syntax-Index prüfen (stimmen alle Datei:Zeile-Angaben unten noch?): `python tools/check_syntax_index.py`
(mit `--fix` werden verrutschte Zeilennummern automatisch korrigiert)

## Aufbau

```
harborsim-basisprojekt/
├── HarborSim.sln
├── HarborSim/
│   ├── Program.cs            ← Menü, Verfügbarkeit, Start der Simulation
│   ├── Data/                 ← ships.csv, workers.csv (werden neben die .exe kopiert)
│   ├── Ships/                ← Ship (abstrakt), CargoShip, PassengerShip, ShipComparer
│   ├── Staff/                ← SkillType, SkillLevel, Worker, SkillMatrix, StaffPool
│   └── Simulation/           ← TaskType, Task, Harbor
├── tests/                    ← run-tests.ps1 + ein Ordner pro Szenario
├── uebungspruefungen/        ← Übungsaufgaben zum Erweiterungsteil, je Angabe, Test, Lösungs-Patch
└── tools/                    ← strip_comments.py, check_syntax_index.py, make_pdf.py
```

Ein Namespace `HarborSim`, eine Klasse bzw. ein Enum pro Datei, Ordner nach Fachbereich.

## Die Angabe in eigenen Worten

Ein Hafen hat mehrere **Docks** (Standard 3, einstellbar 1 bis 10). Angekündigte **Schiffe**
(`ships.csv`) sollen an einem Tag abgefertigt werden, dafür steht **Personal** (`workers.csv`) bereit.

**Ablauf je Schiff:** Docking → Loading → Undocking, genau in dieser Reihenfolge.

- Docking startet nur, wenn das Schiff nicht beschäftigt und nicht angedockt ist und ein Dock frei ist.
- Loading und Undocking starten nur, wenn das Schiff angedockt und nicht beschäftigt ist.
- Undocking startet erst, wenn für das Schiff kein Loading mehr offen ist.
- Ein Task startet nur, wenn das gesamte Personal verfügbar ist. Sonst wird niemand reserviert.
- Ein Mitarbeiter arbeitet immer nur an einem Task.

**Personalbedarf:**

| Task | Bedarf |
| --- | --- |
| Docking / Undocking | 3 Hafenarbeiter (beliebiges Level); über 100 m zusätzlich 1 qualifizierter Lotse, über 200 m stattdessen 1 Experten-Lotse |
| Loading (alle Schiffe) | 1 Hafenarbeiter je angefangene 50 m; über 75 m einer davon qualifiziert, über 150 m einer davon Experte |
| Loading Frachtschiff | zusätzlich 1 Kranführer je angefangene 400 Container; über 500 einer qualifiziert, über 1000 einer Experte |
| Loading Passagierschiff | zusätzlich 1 Passagierkoordinator je angefangene 750 Passagiere; über 1000 einer qualifiziert, über 2500 einer Experte |

„Einer davon Experte“ heißt: einer bekommt das höhere Level, alle anderen sind Beginner.
Ein höheres Level darf ein niedrigeres ersetzen. Ausgewählt werden immer die Mitarbeiter mit dem
niedrigsten noch ausreichenden Level, damit Experten für schwere Aufgaben frei bleiben.

**Dauer:** Docking 30 min, Loading 60 min, Undocking 20 min. Loading Frachtschiff: 30 min +
Container / 10, Passagierschiff: 15 min + Passagiere / 50, jeweils auf ganze Minuten aufgerundet.

**Simulation:** Start am folgenden Werktag (Mo bis Fr, nie heute) um 06:00. Es werden alle möglichen Tasks
gestartet, dann springt die Zeit zum nächsten Task-Ende. Alle Tasks, die zu diesem Zeitpunkt enden,
werden gemeinsam abgeschlossen (Personal und Schiff wieder frei, nach Undocking auch das Dock),
danach werden wieder neue Tasks gestartet. Läuft nichts mehr, obwohl noch Tasks offen sind, ist die
Simulation **blockiert**: Das Programm listet die nicht durchführbaren Tasks auf und hört auf. Ein
Schiff, das angelegt hat, aber nicht beladen werden kann, bleibt angedockt und belegt sein Dock.

**Menü:** Mitarbeiter an- und abwesend schalten, Schiffe ein- und ausplanen, Anzahl der Docks
ändern, Simulation starten (beliebig oft), Beenden. Ungültige Eingaben werden abgewiesen.

**Fehler:** Fehlt eine CSV-Datei oder ist eine Zeile kaputt (keine Zahl, unbekannter Schiffstyp,
unbekannter Skilltyp oder Level, Name doppelt), gibt es eine Meldung und die Zeile wird übersprungen.
Fehlt die Kopfzeile, wird schon Zeile 1 als Datensatz gelesen und ein Hinweis ausgegeben.

### Ergebnis mit den Original-Daten

Mit den Original-CSVs und 3 Docks endet die Simulation **blockiert**: Die *Grand Meridian* hat
1800 Container und braucht deshalb 5 Kranführer (1800 / 400 = 4,5, aufgerundet 5), im Personal
gibt es aber nur 4. Sie legt an, kann nicht beladen werden und bleibt an ihrem Dock liegen. Das ist
richtig so: Die Regeln lassen keinen Start mit zu wenig Personal zu. Alle anderen Schiffe werden
fertig (28 von 30 Tasks, Ende Montag 15:47).

### Warum zählen die IDs bei einem zweiten Lauf weiter?

Vor jeder Simulation werden die CSVs neu eingelesen, damit kein Schiff mehr angedockt und kein
Mitarbeiter mehr beschäftigt ist. Die IDs vergibt der statische Zähler `_nextId` in `Worker` und
`Ship`. Er gehört zur Klasse und läuft deshalb einfach weiter: beim Programmstart Schiffe #1 bis
#10, im ersten Lauf #11 bis #20, im zweiten #21 bis #30. Das ist bewusst so, weil das
Klassendiagramm keine Methode zum Zurücksetzen des Zählers vorsieht. Jede ID ist damit über das
ganze Programm eindeutig. Am Ergebnis der Simulation ändert das nichts.

## Syntax-Index

Wo welcher Stoff aus UE03 bis UE07 im Code vorkommt. Der Link springt auf GitHub direkt zur Zeile,
der Commit zeigt, in welchem Schritt die Stelle entstanden ist.

### UE03 · Datentypen, Kontrollstrukturen, Klassen

| Thema | Python | Fundstelle | Code | Commit |
| --- | --- | --- | --- | --- |
| Klasse | `class Task:` | [Task.cs:11](HarborSim/Simulation/Task.cs#L11) | `public class Task` | 3ddf69c |
| Property, außen nur lesbar | `@property` | [Worker.cs:17](HarborSim/Staff/Worker.cs#L17) | `public int Id { get; private set; }` | 3ddf69c |
| berechnete Property | `@property` | [Ship.cs:25](HarborSim/Ships/Ship.cs#L25) | `get { return CalculateDockingStaff(); }` | 2476bc9 |
| Klassenvariable (static) | Klassenattribut | [Worker.cs:13](HarborSim/Staff/Worker.cs#L13) | `private static int _nextId = 1;` | 3ddf69c |
| Konstante | `MAX = 10` | [Program.cs:20](HarborSim/Program.cs#L20) | `private const int MaxDockCount = 10;` | a104009 |
| while-Schleife | `while ...:` | [Harbor.cs:80](HarborSim/Simulation/Harbor.cs#L80) | `while (_pendingTasks.Count > 0` | 2ba16c1 |
| foreach-Schleife | `for x in liste:` | [StaffPool.cs:42](HarborSim/Staff/StaffPool.cs#L42) | `foreach (Worker worker in _workers)` | a492597 |
| switch | `match ...:` | [Ship.cs:167](HarborSim/Ships/Ship.cs#L167) | `switch (taskType)` | 2476bc9 |
| Bedingung ? a : b | `a if bed else b` | [Program.cs:206](HarborSim/Program.cs#L206) | `? "verfügbar" : "nicht verfügbar"` | a104009 |
| aufrunden | `math.ceil` | [Ship.cs:76](HarborSim/Ships/Ship.cs#L76) | `(int)Math.Ceiling(Length / 50)` | 2476bc9 |
| String-Interpolation, rechtsbündig | `f"{x:>2}"` | [Program.cs:208](HarborSim/Program.cs#L208) | `{i + 1,2}` | a104009 |
| Datum formatieren | `strftime` | [Harbor.cs:61](HarborSim/Simulation/Harbor.cs#L61) | `{_startTime:dddd, dd.MM.yyyy HH:mm}` | a104009 |
| DateTime + TimeSpan | `datetime + timedelta` | [Task.cs:38](HarborSim/Simulation/Task.cs#L38) | `EndTime = startTime + GetDuration();` | 107b210 |
| DateTime − DateTime | `datetime - datetime` | [Harbor.cs:212](HarborSim/Simulation/Harbor.cs#L212) | `TimeSpan duration = _currentTime - _startTime;` | 2ba16c1 |
| Konsole lesen | `input()` | [Program.cs:116](HarborSim/Program.cs#L116) | `string line = Console.ReadLine();` | a104009 |

### UE04 · Verzweigungen, Arrays, Klassen

| Thema | Python | Fundstelle | Code | Commit |
| --- | --- | --- | --- | --- |
| Array anlegen | `[None] * n` | [Harbor.cs:34](HarborSim/Simulation/Harbor.cs#L34) | `_dockedShips = new Ship[dockCount];` | 3ddf69c |
| Array-Element prüfen (null = frei) | `is None` | [Harbor.cs:269](HarborSim/Simulation/Harbor.cs#L269) | `if (_dockedShips[i] == null)` | f81001b |
| 2D-Array | Liste von Listen | [SkillMatrix.cs:19](HarborSim/Staff/SkillMatrix.cs#L19) | `private int[,] _matrix;` | 3ddf69c |
| 2D-Array Zugriff | `m[i][j]` | [SkillMatrix.cs:38](HarborSim/Staff/SkillMatrix.cs#L38) | `_matrix[(int)skillType, (int)skillLevel] += count;` | e9f3933 |
| enum | `class SkillType(Enum)` | [SkillType.cs:7](HarborSim/Staff/SkillType.cs#L7) | `public enum SkillType` | 2b62f87 |
| alle Werte eines enums | `list(SkillType)` | [StaffPool.cs:32](HarborSim/Staff/StaffPool.cs#L32) | `foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))` | a492597 |
| Zahl → enum | `SkillLevel(2)` | [Program.cs:444](HarborSim/Program.cs#L444) | `(SkillLevel)level` | 0872463 |
| Text zerlegen | `line.split(",")` | [Program.cs:394](HarborSim/Program.cs#L394) | `line.Split(',')` | 0872463 |

### UE05 · Methoden, Vererbung

| Thema | Python | Fundstelle | Code | Commit |
| --- | --- | --- | --- | --- |
| abstrakte Klasse | `class Ship(ABC)` | [Ship.cs:9](HarborSim/Ships/Ship.cs#L9) | `public abstract class Ship` | 3ddf69c |
| abstrakte Methode | `@abstractmethod` | [Ship.cs:186](HarborSim/Ships/Ship.cs#L186) | `public abstract void Load();` | 3ddf69c |
| Vererbung | `class CargoShip(Ship)` | [CargoShip.cs:8](HarborSim/Ships/CargoShip.cs#L8) | `public class CargoShip : Ship` | 3ddf69c |
| Basiskonstruktor aufrufen | `super().__init__(...)` | [CargoShip.cs:15](HarborSim/Ships/CargoShip.cs#L15) | `: base(name, length, beam, draft)` | 3ddf69c |
| protected Konstruktor | `_` als Konvention | [Ship.cs:36](HarborSim/Ships/Ship.cs#L36) | `protected Ship(string name` | 3ddf69c |
| virtual | überschreibbar | [Ship.cs:164](HarborSim/Ships/Ship.cs#L164) | `public virtual TimeSpan GetDuration(TaskType taskType)` | 3ddf69c |
| override | Methode überschreiben | [CargoShip.cs:52](HarborSim/Ships/CargoShip.cs#L52) | `public override TimeSpan GetDuration(TaskType taskType)` | 3ddf69c |
| Basisversion aufrufen | `super().loading_staff` | [CargoShip.cs:31](HarborSim/Ships/CargoShip.cs#L31) | `SkillMatrix staff = base.LoadingStaff;` | b52a893 |
| statische Hilfsmethode | `@staticmethod` | [Ship.cs:60](HarborSim/Ships/Ship.cs#L60) | `protected static void AddStaffWithLeader(` | 2476bc9 |
| Methodenüberladung | – (gibt es nicht) | [SkillMatrix.cs:49](HarborSim/Staff/SkillMatrix.cs#L49) | `public int GetCount(SkillType skillType)` | e9f3933 |
| out-Parameter | Tupel zurückgeben | [StaffPool.cs:24](HarborSim/Staff/StaffPool.cs#L24) | `public bool TrySelect(SkillMatrix requirement, out List<Worker> selected)` | 3ddf69c |
| out beim Aufruf | `ok, sel = ...` | [Harbor.cs:114](HarborSim/Simulation/Harbor.cs#L114) | `CanStartTask(task, out selected)` | 2ba16c1 |
| statische Methode | Funktion im Modul | [Program.cs:267](HarborSim/Program.cs#L267) | `private static List<Ship> LoadShips(` | 3ddf69c |

### UE06 · Listen, Dictionaries, Schnittstellen

| Thema | Python | Fundstelle | Code | Commit |
| --- | --- | --- | --- | --- |
| Liste | `list[Task]` | [Harbor.cs:38](HarborSim/Simulation/Harbor.cs#L38) | `_pendingTasks = new List<Task>();` | 3ddf69c |
| Element entfernen | `del liste[i]` | [Harbor.cs:121](HarborSim/Simulation/Harbor.cs#L121) | `_pendingTasks.RemoveAt(index);` | 2ba16c1 |
| Liste kopieren | `list(alt)` | [Harbor.cs:55](HarborSim/Simulation/Harbor.cs#L55) | `new List<Ship>(_ships)` | a104009 |
| Dictionary | `dict[str, bool]` | [Program.cs:29](HarborSim/Program.cs#L29) | `Dictionary<string, bool> _shipAvailable` | a104009 |
| Schlüssel vorhanden? | `name in d` | [Program.cs:263](HarborSim/Program.cs#L263) | `available.ContainsKey(name)` | b00b27d |
| IComparable (Klasse sortiert sich selbst) | `__lt__` | [Worker.cs:9](HarborSim/Staff/Worker.cs#L9) | `public class Worker : IComparable<Worker>` | 3ddf69c |
| CompareTo | `__lt__` | [Worker.cs:58](HarborSim/Staff/Worker.cs#L58) | `public int CompareTo(Worker other)` | 3ddf69c |
| Sort() mit IComparable | `liste.sort()` | [StaffPool.cs:28](HarborSim/Staff/StaffPool.cs#L28) | `_workers.Sort();` | a492597 |
| IComparer (eigene Sortierklasse) | `key=...` | [ShipComparer.cs:14](HarborSim/Ships/ShipComparer.cs#L14) | `public class ShipComparer : IComparer<Ship>` | a104009 |
| Sort() mit IComparer | `sorted(l, key=...)` | [Harbor.cs:56](HarborSim/Simulation/Harbor.cs#L56) | `sortedShips.Sort(new ShipComparer());` | a104009 |

### UE07 · Fehlerbehandlung

| Thema | Python | Fundstelle | Code | Commit |
| --- | --- | --- | --- | --- |
| try / catch | `try: ... except` | [Program.cs:281](HarborSim/Program.cs#L281) | `try` | b00b27d |
| bestimmte Exception fangen | `except ValueError` | [Program.cs:288](HarborSim/Program.cs#L288) | `catch (FormatException e)` | b00b27d |
| Datei-Fehler fangen | `except OSError` | [Program.cs:346](HarborSim/Program.cs#L346) | `catch (IOException e)` | b00b27d |
| Datei lesen | `open(p).read().splitlines()` | [Program.cs:338](HarborSim/Program.cs#L338) | `File.ReadAllLines(path)` | b00b27d |
| Exception werfen | `raise ValueError(...)` | [Ship.cs:41](HarborSim/Ships/Ship.cs#L41) | `throw new ArgumentException("Der Name` | b00b27d |
| ungültiger Zustand | `raise RuntimeError` | [Ship.cs:129](HarborSim/Ships/Ship.cs#L129) | `throw new InvalidOperationException(` | 2476bc9 |
| TryParse statt Absturz | `try: int(s)` | [Program.cs:130](HarborSim/Program.cs#L130) | `int.TryParse(line.Trim(), out number)` | b00b27d |
| Zahl kulturunabhängig lesen | `float("16.2")` | [Program.cs:454](HarborSim/Program.cs#L454) | `CultureInfo.InvariantCulture, out value` | b00b27d |
| Datum mit Format lesen | `strptime` | [Program.cs:92](HarborSim/Program.cs#L92) | `DateTime.TryParseExact(args[0], "yyyy-MM-dd"` | b00b27d |
| gibt es den enum-Namen? | `in SkillType.__members__` | [Program.cs:431](HarborSim/Program.cs#L431) | `Enum.IsDefined(typeof(SkillType), typeText)` | b00b27d |

## Entstehung

Jeder Schritt ist ein eigener Branch, per Pull Request mit Merge-Commit nach `main` gebracht:

| Branch | Inhalt |
| --- | --- |
| `01-grundstruktur` | Projekt, Enums, Klassengerüst laut Klassendiagramm |
| `02-personal` | Worker, SkillMatrix, StaffPool |
| `03-schiffe` | Ship, CargoShip, PassengerShip |
| `04-simulation` | Task, Harbor mit Docks und Simulationsschleife |
| `05-programm-menue` | CSV einlesen, Hauptmenü, Verfügbarkeit, Übersicht |
| `06-fehlerbehandlung` | ungültige Eingaben, kaputte CSV-Zeilen, Validierung |
| `07-dokumentation` | README, Glossar, kommentarfreie Fassung |
