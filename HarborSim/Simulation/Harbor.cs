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

        public void PrintOverview()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            throw new NotImplementedException();
        }

        private void StartPossibleTasks()
        {
            throw new NotImplementedException();
        }

        private void MakeTimeJump()
        {
            throw new NotImplementedException();
        }

        private void CompleteFinishedTasks()
        {
            throw new NotImplementedException();
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
