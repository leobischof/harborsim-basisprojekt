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

        static void Main(string[] args)
        {
            // Umlaute richtig ausgeben und Datum/Zahlen immer im österreichischen Format
            Console.OutputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = new CultureInfo("de-AT");

            List<Ship> ships = LoadShips();
            List<Worker> workers = LoadWorkers();

            Console.WriteLine("HarborSim – Hafensimulation");
            Console.WriteLine($"{ships.Count} Schiffe und {workers.Count} Mitarbeiter eingelesen.");
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
