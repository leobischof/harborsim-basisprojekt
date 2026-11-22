// HarborSim – Program.cs – Leo Bischof (12320283)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace HarborSim
{
    // Einstiegspunkt der Anwendung (Python: if __name__ == "__main__":)
    // Program liest die CSV-Dateien, zeigt das Menü, erstellt den Hafen und startet die Simulation.
    internal class Program
    {
        // Die CSV-Dateien werden beim Build neben die .exe kopiert (siehe HarborSim.csproj)
        private static readonly string ShipsPath = Path.Combine(AppContext.BaseDirectory, "Data", "ships.csv");
        private static readonly string WorkersPath = Path.Combine(AppContext.BaseDirectory, "Data", "workers.csv");

        private const int DefaultDockCount = 3;
        private const int MaxDockCount = 10;

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
            _menuShips = LoadShips();
            _menuWorkers = LoadWorkers();
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

                switch (ReadNumber("Auswahl: "))
                {
                    case 1:
                        ChangeAvailability();
                        break;
                    case 2:
                        RunSimulation();
                        break;
                    case 0:
                    case -1:
                        running = false;
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("Ungültige Auswahl.");
                        break;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Programm beendet.");
        }

        // Startzeit: Datum aus dem Argument (yyyy-MM-dd) oder nächster Werktag, jeweils 06:00.
        // Das Argument macht die Tests reproduzierbar.
        private static DateTime GetStartTime(string[] args)
        {
            if (args.Length > 0)
            {
                return DateTime.ParseExact(args[0], "yyyy-MM-dd", CultureInfo.InvariantCulture).AddHours(6);
            }

            // Heute 06:00, falls das noch bevorsteht, sonst ab morgen weitersuchen
            DateTime start = DateTime.Today.AddHours(6);
            if (start <= DateTime.Now)
            {
                start = start.AddDays(1);
            }
            while (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday)
            {
                start = start.AddDays(1);
            }
            return start;
        }

        // Liest eine Zahl ein. Ende der Eingabe (null, z. B. Datei zu Ende) -> -1.
        private static int ReadNumber(string prompt)
        {
            Console.Write(prompt);
            string line = Console.ReadLine();
            if (line == null)
            {
                Console.WriteLine();
                return -1;
            }
            // Kommt die Eingabe aus einer Datei (Tests), sieht man sie sonst nicht in der Ausgabe
            if (Console.IsInputRedirected)
            {
                Console.WriteLine(line);
            }
            return int.Parse(line.Trim());
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

                switch (ReadNumber("Auswahl: "))
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
                    case -1:
                        return;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("Ungültige Auswahl.");
                        break;
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

                int number = ReadNumber("Nummer zum Umschalten (0 = zurück): ");
                if (number <= 0)
                {
                    return;
                }
                if (number > names.Count)
                {
                    Console.WriteLine();
                    Console.WriteLine("Ungültige Nummer.");
                    continue;
                }
                string name = names[number - 1];
                available[name] = !available[name];
            }
        }

        private static void ChangeDockCount()
        {
            int count = ReadNumber($"Anzahl der Docks (1-{MaxDockCount}): ");
            if (count < 1 || count > MaxDockCount)
            {
                Console.WriteLine();
                Console.WriteLine("Ungültige Anzahl, die Docks bleiben unverändert.");
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
            foreach (Ship ship in LoadShips())
            {
                if (_shipAvailable[ship.Name])
                {
                    ships.Add(ship);
                }
            }

            List<Worker> workers = new List<Worker>();
            foreach (Worker worker in LoadWorkers())
            {
                if (_workerAvailable[worker.Name])
                {
                    workers.Add(worker);
                }
            }

            Harbor harbor = new Harbor(ships, workers, _dockCount, _startTime);
            harbor.PrintOverview();
            harbor.Run();
        }

        // Liest ships.csv: Name,Type,Length,Beam,Draft,Val1[,Val2,...]
        private static List<Ship> LoadShips()
        {
            List<Ship> ships = new List<Ship>();
            // Alle Zeilen auf einmal lesen (Python: open(path).read().splitlines())
            string[] lines = File.ReadAllLines(ShipsPath);

            // Bei 1 starten = Kopfzeile überspringen
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }
                ships.Add(ParseShip(lines[i]));
            }
            return ships;
        }

        // Macht aus einer CSV-Zeile das passende Schiffsobjekt.
        private static Ship ParseShip(string line)
        {
            // Split teilt am Komma (Python: line.split(","))
            string[] parts = line.Split(',');

            string name = parts[0].Trim();
            string type = parts[1].Trim();
            // InvariantCulture = Punkt ist immer Dezimaltrennzeichen, egal welche Spracheinstellung
            double length = double.Parse(parts[2], CultureInfo.InvariantCulture);
            double beam = double.Parse(parts[3], CultureInfo.InvariantCulture);
            double draft = double.Parse(parts[4], CultureInfo.InvariantCulture);
            // Val1: Container bzw. Passagiere. Weitere Spalten (Val2, ...) werden ignoriert.
            int val1 = int.Parse(parts[5], CultureInfo.InvariantCulture);

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

        // Liest workers.csv: Name,SkillType,SkillLevel
        private static List<Worker> LoadWorkers()
        {
            List<Worker> workers = new List<Worker>();
            string[] lines = File.ReadAllLines(WorkersPath);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }
                workers.Add(ParseWorker(lines[i]));
            }
            return workers;
        }

        // Macht aus einer CSV-Zeile einen Worker.
        private static Worker ParseWorker(string line)
        {
            string[] parts = line.Split(',');

            string name = parts[0].Trim();
            // Text -> enum-Wert, z. B. "DockWorker" -> SkillType.DockWorker
            SkillType skillType = (SkillType)Enum.Parse(typeof(SkillType), parts[1].Trim());
            // Zahl -> enum-Wert, z. B. 2 -> SkillLevel.Expert
            int level = int.Parse(parts[2], CultureInfo.InvariantCulture);
            SkillLevel skillLevel = (SkillLevel)level;

            return new Worker(name, skillType, skillLevel);
        }
    }
}
