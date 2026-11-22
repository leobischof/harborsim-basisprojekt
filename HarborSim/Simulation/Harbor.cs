// HarborSim – Harbor.cs – Leo Bischof (12320283)
using System;
using System.Collections.Generic;

namespace HarborSim
{
    // Der Hafen: zentrale Ablaufsteuerung der Simulation.
    // Verwaltet Docks, Personal, Schiffe, Tasklisten und die Simulationszeit.
    public class Harbor
    {
        private DateTime _currentTime;
        // Startzeit merken, um am Ende die Gesamtdauer auszurechnen
        private DateTime _startTime;

        // Docks als Array: ein Platz pro Dock, null = Dock frei, sonst das angedockte Schiff.
        // Array statt Liste, weil die Anzahl der Docks fix ist (Python: [None] * dock_count)
        private Ship[] _dockedShips;

        private List<Task> _pendingTasks;   // offen, noch nicht gestartet
        private List<Task> _runningTasks;   // laufen gerade
        private List<Task> _finishedTasks;  // abgeschlossen
        private List<Ship> _ships;

        public StaffPool StaffPool { get; private set; }

        public Harbor(List<Ship> ships, List<Worker> workers, int dockCount, DateTime startTime)
        {
            if (dockCount < 1)
            {
                throw new ArgumentException("Ein Hafen braucht mindestens ein Dock.");
            }

            _ships = ships;
            _dockedShips = new Ship[dockCount];
            StaffPool = new StaffPool(workers);
            _currentTime = startTime;
            _startTime = startTime;
            _pendingTasks = new List<Task>();
            _runningTasks = new List<Task>();
            _finishedTasks = new List<Task>();

            // Für jedes Schiff drei Tasks in fester Reihenfolge
            foreach (Ship ship in _ships)
            {
                _pendingTasks.Add(new Task(TaskType.Docking, ship));
                _pendingTasks.Add(new Task(TaskType.Loading, ship));
                _pendingTasks.Add(new Task(TaskType.Undocking, ship));
            }
        }

        // Übersicht vor dem Start: Startzeit, Docks, Schiffe und Mitarbeiter.
        public void PrintOverview()
        {
            // Kopie sortieren, damit die Reihenfolge der Tasks (CSV-Reihenfolge) gleich bleibt
            List<Ship> sortedShips = new List<Ship>(_ships);
            sortedShips.Sort(new ShipComparer());

            Console.WriteLine();
            Console.WriteLine("=== Übersicht ===");
            // dddd = Wochentag ausgeschrieben, z. B. "Montag"
            Console.WriteLine($"Start: {_startTime:dddd, dd.MM.yyyy HH:mm}");
            Console.WriteLine($"Docks: {_dockedShips.Length}");
            Console.WriteLine("Schiffe, sortiert nach Länge:");
            foreach (Ship ship in sortedShips)
            {
                Console.WriteLine("  " + ship.GetDescription());
            }
            Console.WriteLine("Mitarbeiter:");
            // GetDescription endet schon mit einem Zeilenumbruch -> Write statt WriteLine
            Console.Write(StaffPool.GetDescription());
        }

        // Simulationsschleife laut Flowchart "Discrete Event Simulation".
        // Die Zeit läuft nicht Minute für Minute, sondern springt von Endzeit zu Endzeit.
        public void Run()
        {
            Console.WriteLine();
            Console.WriteLine("=== Simulation ===");

            while (_pendingTasks.Count > 0 || _runningTasks.Count > 0)
            {
                StartPossibleTasks();

                // Blockiert: offene Tasks, aber nichts läuft und nichts konnte starten
                if (_runningTasks.Count == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Simulation blockiert: Keiner der offenen Tasks kann gestartet werden.");
                    Console.WriteLine("Nicht durchführbare Tasks:");
                    foreach (Task task in _pendingTasks)
                    {
                        Console.WriteLine("  " + task.GetDescription());
                    }
                    break;
                }

                MakeTimeJump();
                CompleteFinishedTasks();
            }

            PrintSummary();
        }

        // Prüft alle offenen Tasks der Reihe nach und startet jeden, der jetzt möglich ist.
        private void StartPossibleTasks()
        {
            // while statt foreach, weil wir während des Durchlaufs aus der Liste löschen.
            // Nach RemoveAt rutscht der nächste Task auf denselben Index nach -> index bleibt.
            int index = 0;
            while (index < _pendingTasks.Count)
            {
                Task task = _pendingTasks[index];
                List<Worker> selected;
                if (!CanStartTask(task, out selected))
                {
                    index++;
                    continue;
                }

                task.StartTask(selected, _currentTime);
                _pendingTasks.RemoveAt(index);
                _runningTasks.Add(task);
                PrintTaskEvent("START", task);

                if (task.Type == TaskType.Docking)
                {
                    AddShipToDock(task.Ship);
                }
            }
        }

        // Alle Bedingungen für den Start eines Tasks. Bei true steht das Personal in "selected".
        // Guard Clauses: jede Bedingung, die nicht passt, beendet die Methode sofort.
        private bool CanStartTask(Task task, out List<Worker> selected)
        {
            // out-Parameter muss vor jedem return einen Wert haben
            selected = null;
            Ship ship = task.Ship;

            // Schiff frei und Dockzustand passt?
            if (!ship.CanDoTask(task.Type))
            {
                return false;
            }
            if (task.Type == TaskType.Docking && !HasFreeDock())
            {
                return false;
            }
            if (task.Type == TaskType.Undocking && HasPendingLoadingTask(ship))
            {
                return false;
            }

            // Loading braucht LoadingStaff, Docking und Undocking brauchen DockingStaff
            SkillMatrix requirement = ship.DockingStaff;
            if (task.Type == TaskType.Loading)
            {
                requirement = ship.LoadingStaff;
            }
            return StaffPool.TrySelect(requirement, out selected);
        }

        // Springt zur kleinsten Endzeit aller laufenden Tasks.
        private void MakeTimeJump()
        {
            DateTime nextTime = _runningTasks[0].EndTime;
            foreach (Task task in _runningTasks)
            {
                if (task.EndTime < nextTime)
                {
                    nextTime = task.EndTime;
                }
            }
            _currentTime = nextTime;
        }

        // Schließt alle Tasks ab, deren Endzeit genau jetzt ist (Flowchart "Task-Lebenszyklus").
        private void CompleteFinishedTasks()
        {
            int index = 0;
            while (index < _runningTasks.Count)
            {
                Task task = _runningTasks[index];
                if (task.EndTime != _currentTime)
                {
                    index++;
                    continue;
                }

                PrintTaskEvent("ENDE", task);
                // Beim Undocking gibt zuerst der Hafen das Dock frei
                if (task.Type == TaskType.Undocking)
                {
                    RemoveShipFromDock(task.Ship);
                }
                task.EndTask();
                _runningTasks.RemoveAt(index);
                _finishedTasks.Add(task);
            }
        }

        // Eine Zeile pro Start oder Ende, z. B. "06:30  START  Loading   | Baltic Venture | ..."
        private void PrintTaskEvent(string action, Task task)
        {
            Console.WriteLine($"{_currentTime:HH:mm}  {action,-5}  {task.GetDescription()}");
        }

        // Ergebnis nach dem Ende der Simulation.
        private void PrintSummary()
        {
            // DateTime - DateTime = TimeSpan (Python: datetime - datetime = timedelta)
            TimeSpan duration = _currentTime - _startTime;
            int taskCount = _finishedTasks.Count + _pendingTasks.Count;

            Console.WriteLine();
            Console.WriteLine("=== Zusammenfassung ===");
            Console.WriteLine($"Ende der Simulation: {_currentTime:dddd, dd.MM.yyyy HH:mm}");
            Console.WriteLine($"Dauer: {(int)duration.TotalHours} h {duration.Minutes} min");
            Console.WriteLine($"Abgeschlossene Tasks: {_finishedTasks.Count} von {taskCount}");

            if (_pendingTasks.Count == 0)
            {
                Console.WriteLine("Alle Tasks wurden durchgeführt.");
                return;
            }

            // Die Liste selbst wurde schon bei der Blockierung ausgegeben (siehe Run)
            Console.WriteLine($"Nicht durchgeführte Tasks: {_pendingTasks.Count} (Liste siehe oben)");
            foreach (Ship dockedShip in _dockedShips)
            {
                if (dockedShip != null)
                {
                    Console.WriteLine($"Noch angedockt: {dockedShip.Name}");
                }
            }
        }

        // Ist mindestens ein Dock frei (also null)?
        private bool HasFreeDock()
        {
            foreach (Ship dockedShip in _dockedShips)
            {
                if (dockedShip == null)
                {
                    return true;
                }
            }
            return false;
        }

        // Muss dieses Schiff noch beladen werden? Dann darf es nicht ablegen.
        private bool HasPendingLoadingTask(Ship ship)
        {
            foreach (Task task in _pendingTasks)
            {
                if (task.Ship == ship && task.Type == TaskType.Loading)
                {
                    return true;
                }
            }
            return false;
        }

        // Setzt das Schiff in das erste freie Dock.
        private void AddShipToDock(Ship ship)
        {
            for (int i = 0; i < _dockedShips.Length; i++)
            {
                if (_dockedShips[i] == null)
                {
                    _dockedShips[i] = ship;
                    Console.WriteLine($"       -> {ship.Name} liegt an Dock {i + 1}");
                    return;
                }
            }
            throw new InvalidOperationException("Kein freies Dock vorhanden.");
        }

        // Gibt das Dock frei, an dem das Schiff liegt.
        private void RemoveShipFromDock(Ship ship)
        {
            for (int i = 0; i < _dockedShips.Length; i++)
            {
                if (_dockedShips[i] == ship)
                {
                    _dockedShips[i] = null;
                    Console.WriteLine($"       -> Dock {i + 1} ist wieder frei");
                    return;
                }
            }
            throw new InvalidOperationException($"{ship.Name} liegt an keinem Dock.");
        }
    }
}
