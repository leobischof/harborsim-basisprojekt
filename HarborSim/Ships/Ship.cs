// HarborSim – Ship.cs – Leo Bischof (12320283)
using System;

namespace HarborSim
{
    // abstract = von Ship selbst gibt es keine Objekte, nur von CargoShip und PassengerShip
    public abstract class Ship
    {
        private static int _nextId = 1;

        public int Id { get; private set; }
        public string Name { get; private set; }
        public double Length { get; private set; }
        public double Beam { get; private set; }
        public double Draft { get; private set; }
        public bool IsBusy { get; private set; }
        public bool IsDocked { get; private set; }
        public SkillMatrix DockingStaff { get; private set; }
        public SkillMatrix LoadingStaff { get; private set; }

        protected Ship(string name, double length, double beam, double draft)
        {
            Name = name;
            Length = length;
            Beam = beam;
            Draft = draft;
        }

        private SkillMatrix CalculateBaseLoadingStaff()
        {
            throw new NotImplementedException();
        }

        private SkillMatrix CalculateDockingStaff()
        {
            throw new NotImplementedException();
        }

        public bool CanDoTask(TaskType taskType)
        {
            throw new NotImplementedException();
        }

        public void Dock()
        {
            throw new NotImplementedException();
        }

        public void Undock()
        {
            throw new NotImplementedException();
        }

        public void StartTask(TaskType taskType)
        {
            throw new NotImplementedException();
        }

        public void EndTask()
        {
            throw new NotImplementedException();
        }

        public virtual TimeSpan GetDuration(TaskType taskType)
        {
            throw new NotImplementedException();
        }

        public virtual string GetDescription()
        {
            throw new NotImplementedException();
        }

        public abstract void Load();
    }
}
