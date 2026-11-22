// HarborSim – Task.cs – Leo Bischof (12320283)
using System;
using System.Collections.Generic;

namespace HarborSim
{
    // Ein Arbeitsauftrag: verbindet einen TaskType mit einem bestimmten Schiff
    // und merkt sich Personal, Start- und Endzeit.
    // Hinweis: Diese Klasse heißt wie System.Threading.Tasks.Task. Weil wir dieses
    // Namespace nicht mit "using" einbinden, ist hier immer unser HarborSim.Task gemeint.
    public class Task
    {
        public TaskType Type { get; private set; }
        public Ship Ship { get; private set; }
        public List<Worker> Workers { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public Task(TaskType type, Ship ship)
        {
            Type = type;
            Ship = ship;
            Workers = new List<Worker>();
        }

        // Die Dauer kennt das Schiff, nicht der Task.
        public TimeSpan GetDuration()
        {
            return Ship.GetDuration(Type);
        }

        // Start laut Flowchart "Task-Lebenszyklus".
        public void StartTask(List<Worker> workers, DateTime startTime)
        {
            Workers = workers;
            StartTime = startTime;
            // DateTime + TimeSpan = neuer Zeitpunkt (Python: datetime + timedelta)
            EndTime = startTime + GetDuration();

            // Schiff wird beschäftigt (und beim Docking angedockt)
            Ship.StartTask(Type);
            // Erst jetzt werden die Mitarbeiter beschäftigt
            foreach (Worker worker in Workers)
            {
                worker.StartTask();
            }
        }

        // Abschluss laut Flowchart "Task-Lebenszyklus".
        // Das Freigeben des Docks erledigt vorher der Hafen.
        public void EndTask()
        {
            if (Type == TaskType.Loading)
            {
                Ship.Load();
            }
            else if (Type == TaskType.Undocking)
            {
                Ship.Undock();
            }

            foreach (Worker worker in Workers)
            {
                worker.EndTask();
            }
            Ship.EndTask();
        }

        // Beispiel: "Loading   | Baltic Venture | 06:30-07:12 | Elias, Freya"
        // {Type,-9} = linksbündig auf 9 Zeichen auffüllen (Python: f"{type:<9}")
        public string GetDescription()
        {
            string description = $"{Type,-9} | {Ship.Name}";

            // Noch nicht gestartet -> keine Zeiten und kein Personal
            if (Workers.Count == 0)
            {
                return description;
            }

            string names = "";
            foreach (Worker worker in Workers)
            {
                if (names != "")
                {
                    names += ", ";
                }
                names += worker.Name;
            }
            return $"{description} | {StartTime:HH:mm}-{EndTime:HH:mm} | {names}";
        }
    }
}
