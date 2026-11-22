// HarborSim – Program.cs – Leo Bischof (12320283)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace HarborSim
{
    // Einstiegspunkt der Anwendung (Python: if __name__ == "__main__":)
    // Program liest die CSV-Dateien ein, zeigt das Menü, verwaltet die Verfügbarkeit
    // und startet die Simulation (so steht es im Klassendiagramm).
    internal class Program
    {
        // Die CSV-Dateien werden beim Build neben die .exe kopiert (siehe HarborSim.csproj)
        private static readonly string ShipsPath = Path.Combine(AppContext.BaseDirectory, "Data", "ships.csv");
        private static readonly string WorkersPath = Path.Combine(AppContext.BaseDirectory, "Data", "workers.csv");

        private const int DefaultDockCount = 3;
        private const int MaxDockCount = 10;
        // Rückgabewert von ReadNumber, wenn keine Eingabe mehr kommt (Datei zu Ende, Strg+Z)
        private const int EndOfInput = -1;

        // Einstellungen aus dem Menü. "static", weil Program nie als Objekt erzeugt wird.
        // Die Listen dienen nur der Anzeige im Menü (Nummer -> Name).
        private static List<Ship> _menuShips;
        private static List<Worker> _menuWorkers;
        // Verfügbarkeit: Name -> verfügbar (Python: dict[str, bool])
        private static Dictionary<string, bool> _shipAvailable = new Dictionary<string, bool>();
        private static Dictionary<string, bool> _workerAvailable = new Dictionary<string, bool>();
        private static int _dockCount = DefaultDockCount;
        private static DateTime _startTime;

        static void Main(string[] args)
        {
            // Umlaute richtig ausgeben und Datum/Zahlen immer im österreichischen Format
            Console.OutputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = new CultureInfo("de-AT");

            _startTime = GetStartTime(args);
            _menuShips = LoadShips(ShipsPath);
            _menuWorkers = LoadWorkers(WorkersPath);
            // Am Anfang sind alle verfügbar
            foreach (Ship ship in _menuShips)
            {
                _shipAvailable[ship.Name] = true;
            }
            foreach (Worker worker in _menuWorkers)
            {
                _workerAvailable[worker.Name] = true;
            }

            Console.WriteLine("HarborSim – Hafensimulation");
            Console.WriteLine($"{_menuShips.Count} Schiffe und {_menuWorkers.Count} Mitarbeiter eingelesen.");

            bool running = true;
            while (running)
            {
                Console.WriteLine();
                Console.WriteLine("=== Hauptmenü ===");
                Console.WriteLine("1  Verfügbarkeit ändern");
                Console.WriteLine("2  Simulation starten");
                Console.WriteLine("0  Beenden");

                // ReadNumber liefert nur 0, 1, 2 oder EndOfInput -> kein default nötig
                switch (ReadNumber("Auswahl: ", 0, 2))
                {
                    case 1:
                        ChangeAvailability();
                        break;
                    case 2:
                        RunSimulation();
                        break;
                    case 0:
                    case EndOfInput:
                        running = false;
                        break;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Programm beendet.");
        }

        // Startzeit: Datum aus dem Argument (yyyy-MM-dd) oder folgender Werktag, jeweils 06:00.
        // Das Argument macht die Tests reproduzierbar.
        private static DateTime GetStartTime(string[] args)
        {
            DateTime date;
            if (args.Length > 0)
            {
                if (DateTime.TryParseExact(args[0], "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out date))
                {
                    return date.AddHours(6);
                }
                Console.WriteLine($"Ungültiges Startdatum \"{args[0]}\" (Format yyyy-MM-dd), nehme den folgenden Werktag.");
            }

            // Laut Angabe immer ab dem folgenden Tag; fällt der auf ein Wochenende, weiter bis Montag
            DateTime start = DateTime.Today.AddDays(1).AddHours(6);
            while (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday)
            {
                start = start.AddDays(1);
            }
            return start;
        }

        // Fragt so lange nach, bis eine ganze Zahl von min bis max eingegeben wird.
        // Keine Eingabe mehr (null) -> EndOfInput, damit das Programm sauber endet.
        private static int ReadNumber(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string line = Console.ReadLine();
                if (line == null)
                {
                    Console.WriteLine();
                    return EndOfInput;
                }
                // Kommt die Eingabe aus einer Datei (Tests), sieht man sie sonst nicht in der Ausgabe
                if (Console.IsInputRedirected)
                {
                    Console.WriteLine(line);
                }

                // TryParse statt Parse: bei "abc" kein Absturz, sondern false
                int number;
                if (int.TryParse(line.Trim(), out number) && number >= min && number <= max)
                {
                    return number;
                }
                Console.WriteLine($"Ungültige Eingabe, bitte eine Zahl von {min} bis {max} eingeben.");
            }
        }

        // Untermenü: Mitarbeiter, Schiffe oder Docks ändern.
        private static void ChangeAvailability()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("=== Verfügbarkeit ändern ===");
                Console.WriteLine("1  Mitarbeiter");
                Console.WriteLine("2  Schiffe");
                Console.WriteLine($"3  Docks (aktuell {_dockCount})");
                Console.WriteLine("0  Zurück");

                switch (ReadNumber("Auswahl: ", 0, 3))
                {
                    case 1:
                        ToggleWorkers();
                        break;
                    case 2:
                        ToggleShips();
                        break;
                    case 3:
                        ChangeDockCount();
                        break;
                    case 0:
                    case EndOfInput:
                        return;
                }
            }
        }

        // Mitarbeiter an- und abwesend schalten.
        private static void ToggleWorkers()
        {
            List<string> names = new List<string>();
            List<string> labels = new List<string>();
            foreach (Worker worker in _menuWorkers)
            {
                names.Add(worker.Name);
                labels.Add($"{worker.Name} ({worker.SkillType}, {worker.SkillLevel})");
            }
            ToggleAvailability("Mitarbeiter", names, labels, _workerAvailable);
        }

        // Schiffe ein- und ausplanen.
        private static void ToggleShips()
        {
            List<string> names = new List<string>();
            List<string> labels = new List<string>();
            foreach (Ship ship in _menuShips)
            {
                names.Add(ship.Name);
                labels.Add($"{ship.Name} ({ship.Length} m)");
            }
            ToggleAvailability("Schiffe", names, labels, _shipAvailable);
        }

        // Gemeinsame Liste für Mitarbeiter und Schiffe: nummeriert anzeigen,
        // über die Nummer umschalten, 0 = zurück.
        private static void ToggleAvailability(string title, List<string> names, List<string> labels,
            Dictionary<string, bool> available)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine($"=== {title} ===");
                for (int i = 0; i < names.Count; i++)
                {
                    // Bedingung ? wennJa : wennNein (Python: "a" if bedingung else "b")
                    string status = available[names[i]] ? "verfügbar" : "nicht verfügbar";
                    // {i + 1,2} = rechtsbündig auf 2 Zeichen (Python: f"{i + 1:>2}")
                    Console.WriteLine($"{i + 1,2}  {labels[i]} – {status}");
                }

                int number = ReadNumber("Nummer zum Umschalten (0 = zurück): ", 0, names.Count);
                if (number == 0 || number == EndOfInput)
                {
                    return;
                }
                string name = names[number - 1];
                available[name] = !available[name];
            }
        }

        private static void ChangeDockCount()
        {
            int count = ReadNumber($"Anzahl der Docks (1-{MaxDockCount}): ", 1, MaxDockCount);
            if (count == EndOfInput)
            {
                return;
            }
            _dockCount = count;
        }

        // Ein Simulationslauf mit frisch eingelesenen Daten.
        // Frisch einlesen, weil Schiffe und Mitarbeiter nach einem Lauf noch Zustände
        // (angedockt, beschäftigt) haben könnten. Die IDs zählen dabei weiter.
        private static void RunSimulation()
        {
            List<Ship> ships = new List<Ship>();
            foreach (Ship ship in LoadShips(ShipsPath))
            {
                if (IsAvailable(_shipAvailable, ship.Name))
                {
                    ships.Add(ship);
                }
            }

            List<Worker> workers = new List<Worker>();
            foreach (Worker worker in LoadWorkers(WorkersPath))
            {
                if (IsAvailable(_workerAvailable, worker.Name))
                {
                    workers.Add(worker);
                }
            }

            Harbor harbor = new Harbor(ships, workers, _dockCount, _startTime);
            harbor.PrintOverview();
            harbor.Run();
        }

        // Wer erst nach dem Programmstart in die CSV geschrieben wurde, ist im Dictionary
        // nicht enthalten. ContainsKey verhindert dann eine KeyNotFoundException.
        private static bool IsAvailable(Dictionary<string, bool> available, string name)
        {
            return available.ContainsKey(name) && available[name];
        }

        // Liest ships.csv: Name,Type,Length,Beam,Draft,Val1[,Val2,...]
        private static List<Ship> LoadShips(string path)
        {
            List<Ship> ships = new List<Ship>();
            string[] lines = ReadLines(path);

            // Bei 1 starten = Kopfzeile überspringen
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }
                // try/catch (Python: try/except): eine kaputte Zeile soll nicht alles abbrechen
                try
                {
                    ships.Add(ParseShip(lines[i]));
                }
                catch (FormatException e)
                {
                    PrintSkippedLine(path, i, e.Message);
                }
                catch (ArgumentException e)
                {
                    PrintSkippedLine(path, i, e.Message);
                }
            }
            return ships;
        }

        // Liest workers.csv: Name,SkillType,SkillLevel
        private static List<Worker> LoadWorkers(string path)
        {
            List<Worker> workers = new List<Worker>();
            string[] lines = ReadLines(path);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }
                try
                {
                    workers.Add(ParseWorker(lines[i]));
                }
                catch (FormatException e)
                {
                    PrintSkippedLine(path, i, e.Message);
                }
                catch (ArgumentException e)
                {
                    PrintSkippedLine(path, i, e.Message);
                }
            }
            return workers;
        }

        // Alle Zeilen auf einmal lesen (Python: open(path).read().splitlines()).
        // Fehlt die Datei oder ist sie nicht lesbar -> Meldung und leeres Array.
        private static string[] ReadLines(string path)
        {
            try
            {
                return File.ReadAllLines(path);
            }
            // IOException deckt auch FileNotFoundException und DirectoryNotFoundException ab
            catch (IOException e)
            {
                Console.WriteLine($"Fehler: {Path.GetFileName(path)} konnte nicht gelesen werden ({e.Message})");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"Fehler: {Path.GetFileName(path)} konnte nicht gelesen werden ({e.Message})");
            }
            return new string[0];
        }

        // Zeilennummer wie im Editor: Index 0 ist Zeile 1 (die Kopfzeile)
        private static void PrintSkippedLine(string path, int index, string reason)
        {
            Console.WriteLine($"Zeile {index + 1} in {Path.GetFileName(path)} übersprungen: {reason}");
        }

        // Macht aus einer CSV-Zeile das passende Schiffsobjekt.
        private static Ship ParseShip(string line)
        {
            // Split teilt am Komma (Python: line.split(","))
            string[] parts = line.Split(',');
            if (parts.Length < 6)
            {
                throw new FormatException("zu wenige Spalten (erwartet: Name,Type,Length,Beam,Draft,Val1)");
            }

            string name = parts[0].Trim();
            string type = parts[1].Trim();
            double length = ParseDouble(parts[2], "Length");
            double beam = ParseDouble(parts[3], "Beam");
            double draft = ParseDouble(parts[4], "Draft");
            // Val1: Container bzw. Passagiere. Weitere Spalten (Val2, ...) werden ignoriert.
            int val1 = ParseInt(parts[5], "Val1");

            switch (type)
            {
                case "Cargo":
                    return new CargoShip(name, length, beam, draft, val1);
                case "Passenger":
                    return new PassengerShip(name, length, beam, draft, val1);
                default:
                    throw new FormatException($"unbekannter Schiffstyp \"{type}\"");
            }
        }

        // Macht aus einer CSV-Zeile einen Worker.
        private static Worker ParseWorker(string line)
        {
            string[] parts = line.Split(',');
            if (parts.Length < 3)
            {
                throw new FormatException("zu wenige Spalten (erwartet: Name,SkillType,SkillLevel)");
            }

            string name = parts[0].Trim();
            string typeText = parts[1].Trim();
            // IsDefined prüft, ob es diesen Namen im enum gibt (Python: name in SkillType.__members__)
            if (!Enum.IsDefined(typeof(SkillType), typeText))
            {
                throw new FormatException($"unbekannter Skilltyp \"{typeText}\"");
            }
            // Text -> enum-Wert, z. B. "DockWorker" -> SkillType.DockWorker
            SkillType skillType = (SkillType)Enum.Parse(typeof(SkillType), typeText);

            int level = ParseInt(parts[2], "SkillLevel");
            if (!Enum.IsDefined(typeof(SkillLevel), level))
            {
                throw new FormatException($"unbekanntes Level {level} (erlaubt: 0, 1, 2)");
            }
            // Zahl -> enum-Wert, z. B. 2 -> SkillLevel.Expert
            SkillLevel skillLevel = (SkillLevel)level;

            return new Worker(name, skillType, skillLevel);
        }

        // Kommazahl mit Punkt als Dezimaltrennzeichen, egal welche Spracheinstellung (InvariantCulture).
        // TryParse wirft keine Exception, sondern liefert false (Python: try: float(text) except ValueError).
        private static double ParseDouble(string text, string column)
        {
            double value;
            if (!double.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                throw new FormatException($"{column} \"{text.Trim()}\" ist keine Zahl");
            }
            return value;
        }

        private static int ParseInt(string text, string column)
        {
            int value;
            if (!int.TryParse(text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                throw new FormatException($"{column} \"{text.Trim()}\" ist keine ganze Zahl");
            }
            return value;
        }
    }
}
