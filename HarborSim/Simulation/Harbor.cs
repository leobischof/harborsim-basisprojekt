// HarborSim – Harbor.cs – Leo Bischof (12320283)
using System;
using System.Collections.Generic;

namespace HarborSim
{
    // Der Hafen: zentrale Ablaufsteuerung der Simulation.
    public class Harbor
    {
        private DateTime _currentTime;
        private Ship[] _dockedShips;
        private List<Task> _pendingTasks;
        private List<Task> _runningTasks;
        private List<Task> _finishedTasks;
        private List<Ship> _ships;

        public StaffPool StaffPool { get; private set; }

        public Harbor(List<Ship> ships, List<Worker> workers, int dockCount, DateTime startTime)
        {
            _ships = ships;
            _dockedShips = new Ship[dockCount];
            StaffPool = new StaffPool(workers);
            _currentTime = startTime;
            _pendingTasks = new List<Task>();
            _runningTasks = new List<Task>();
            _finishedTasks = new List<Task>();
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

        private bool HasFreeDock()
        {
            throw new NotImplementedException();
        }

        private bool HasPendingLoadingTask(Ship ship)
        {
            throw new NotImplementedException();
        }

        private void AddShipToDock(Ship ship)
        {
            throw new NotImplementedException();
        }

        private void RemoveShipFromDock(Ship ship)
        {
            throw new NotImplementedException();
        }
    }
}
