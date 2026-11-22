using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace HarborSim
{
    internal class Program
    {
        private static readonly string ShipsPath = Path.Combine(AppContext.BaseDirectory, "Data", "ships.csv");
        private static readonly string WorkersPath = Path.Combine(AppContext.BaseDirectory, "Data", "workers.csv");

        private const int DefaultDockCount = 3;
        private const int MaxDockCount = 10;
        private const int EndOfInput = -1;

        private static List<Ship> _menuShips;
        private static List<Worker> _menuWorkers;
        private static Dictionary<string, bool> _shipAvailable = new Dictionary<string, bool>();
        private static Dictionary<string, bool> _workerAvailable = new Dictionary<string, bool>();
        private static int _dockCount = DefaultDockCount;
        private static DateTime _startTime;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = new CultureInfo("de-AT");

            _startTime = GetStartTime(args);
            _menuShips = LoadShips(ShipsPath);
            _menuWorkers = LoadWorkers(WorkersPath);
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

            DateTime start = DateTime.Today.AddDays(1).AddHours(6);
            while (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday)
            {
                start = start.AddDays(1);
            }
            return start;
        }

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
                if (Console.IsInputRedirected)
                {
                    Console.WriteLine(line);
                }

                int number;
                if (int.TryParse(line.Trim(), out number) && number >= min && number <= max)
                {
                    return number;
                }
                Console.WriteLine($"Ungültige Eingabe, bitte eine Zahl von {min} bis {max} eingeben.");
            }
        }

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

        private static void ToggleAvailability(string title, List<string> names, List<string> labels,
            Dictionary<string, bool> available)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine($"=== {title} ===");
                for (int i = 0; i < names.Count; i++)
                {
                    string status = available[names[i]] ? "verfügbar" : "nicht verfügbar";
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

        private static bool IsAvailable(Dictionary<string, bool> available, string name)
        {
            return available.ContainsKey(name) && available[name];
        }

        private static List<Ship> LoadShips(string path)
        {
            List<Ship> ships = new List<Ship>();
            List<string> names = new List<string>();
            string[] lines = ReadLines(path);

            for (int i = FirstDataLine(lines, path); i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }
                try
                {
                    CheckUniqueName(lines[i], names);
                    Ship ship = ParseShip(lines[i]);
                    ships.Add(ship);
                    names.Add(ship.Name);
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

        private static List<Worker> LoadWorkers(string path)
        {
            List<Worker> workers = new List<Worker>();
            List<string> names = new List<string>();
            string[] lines = ReadLines(path);

            for (int i = FirstDataLine(lines, path); i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }
                try
                {
                    CheckUniqueName(lines[i], names);
                    Worker worker = ParseWorker(lines[i]);
                    workers.Add(worker);
                    names.Add(worker.Name);
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

        private static string[] ReadLines(string path)
        {
            try
            {
                return File.ReadAllLines(path);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Fehler: {Path.GetFileName(path)} wurde nicht gefunden.");
            }
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

        private static int FirstDataLine(string[] lines, string path)
        {
            if (lines.Length == 0)
            {
                return 0;
            }
            if (lines[0].Split(',')[0].Trim() == "Name")
            {
                return 1;
            }
            Console.WriteLine($"Hinweis: {Path.GetFileName(path)} hat keine Kopfzeile, Zeile 1 wird als Daten gelesen.");
            return 0;
        }

        private static void CheckUniqueName(string line, List<string> names)
        {
            string name = line.Split(',')[0].Trim();
            if (names.Contains(name))
            {
                throw new FormatException($"Name \"{name}\" kommt doppelt vor");
            }
        }

        private static void PrintSkippedLine(string path, int index, string reason)
        {
            Console.WriteLine($"Zeile {index + 1} in {Path.GetFileName(path)} übersprungen: {reason}");
        }

        private static Ship ParseShip(string line)
        {
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

        private static Worker ParseWorker(string line)
        {
            string[] parts = line.Split(',');
            if (parts.Length < 3)
            {
                throw new FormatException("zu wenige Spalten (erwartet: Name,SkillType,SkillLevel)");
            }

            string name = parts[0].Trim();
            string typeText = parts[1].Trim();
            if (!Enum.IsDefined(typeof(SkillType), typeText))
            {
                throw new FormatException($"unbekannter Skilltyp \"{typeText}\"");
            }
            SkillType skillType = (SkillType)Enum.Parse(typeof(SkillType), typeText);

            int level = ParseInt(parts[2], "SkillLevel");
            if (!Enum.IsDefined(typeof(SkillLevel), level))
            {
                throw new FormatException($"unbekanntes Level {level} (erlaubt: 0, 1, 2)");
            }
            SkillLevel skillLevel = (SkillLevel)level;

            return new Worker(name, skillType, skillLevel);
        }

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
