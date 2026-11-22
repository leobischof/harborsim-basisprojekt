using System;
using System.Collections.Generic;

namespace HarborSim
{
    public class Harbor
    {
        private DateTime _currentTime;
        private DateTime _startTime;

        private Ship[] _dockedShips;

        private List<Task> _pendingTasks;
        private List<Task> _runningTasks;
        private List<Task> _finishedTasks;
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

            foreach (Ship ship in _ships)
            {
                _pendingTasks.Add(new Task(TaskType.Docking, ship));
                _pendingTasks.Add(new Task(TaskType.Loading, ship));
                _pendingTasks.Add(new Task(TaskType.Undocking, ship));
            }
        }

        public void PrintOverview()
        {
            List<Ship> sortedShips = new List<Ship>(_ships);
            sortedShips.Sort(new ShipComparer());

            Console.WriteLine();
            Console.WriteLine("=== Übersicht ===");
            Console.WriteLine($"Start: {_startTime:dddd, dd.MM.yyyy HH:mm}");
            Console.WriteLine($"Docks: {_dockedShips.Length}");
            Console.WriteLine("Schiffe, sortiert nach Länge:");
            foreach (Ship ship in sortedShips)
            {
                Console.WriteLine("  " + ship.GetDescription());
            }
            Console.WriteLine("Mitarbeiter:");
            Console.Write(StaffPool.GetDescription());
        }

        public void Run()
        {
            Console.WriteLine();
            Console.WriteLine("=== Simulation ===");

            while (_pendingTasks.Count > 0 || _runningTasks.Count > 0)
            {
                StartPossibleTasks();

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

        private void StartPossibleTasks()
        {
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

        private bool CanStartTask(Task task, out List<Worker> selected)
        {
            selected = null;
            Ship ship = task.Ship;

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

            SkillMatrix requirement = ship.DockingStaff;
            if (task.Type == TaskType.Loading)
            {
                requirement = ship.LoadingStaff;
            }
            return StaffPool.TrySelect(requirement, out selected);
        }

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
                if (task.Type == TaskType.Undocking)
                {
                    RemoveShipFromDock(task.Ship);
                }
                task.EndTask();
                _runningTasks.RemoveAt(index);
                _finishedTasks.Add(task);
            }
        }

        private void PrintTaskEvent(string action, Task task)
        {
            Console.WriteLine($"{_currentTime:HH:mm}  {action,-5}  {task.GetDescription()}");
        }

        private void PrintSummary()
        {
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

            Console.WriteLine($"Nicht durchgeführte Tasks: {_pendingTasks.Count} (Liste siehe oben)");
            foreach (Ship dockedShip in _dockedShips)
            {
                if (dockedShip != null)
                {
                    Console.WriteLine($"Noch angedockt: {dockedShip.Name}");
                }
            }
        }

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
