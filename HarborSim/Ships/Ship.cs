using System;

namespace HarborSim
{
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

        public SkillMatrix DockingStaff
        {
            get { return CalculateDockingStaff(); }
        }

        public virtual SkillMatrix LoadingStaff
        {
            get { return CalculateBaseLoadingStaff(); }
        }

        protected Ship(string name, double length, double beam, double draft)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Der Name eines Schiffs darf nicht leer sein.");
            }
            if (length <= 0 || beam <= 0 || draft <= 0)
            {
                throw new ArgumentException($"{name}: Länge, Breite und Tiefgang müssen größer als 0 sein.");
            }
            Id = _nextId;
            _nextId++;
            Name = name;
            Length = length;
            Beam = beam;
            Draft = draft;
            IsBusy = false;
            IsDocked = false;
        }

        protected static void AddStaffWithLeader(SkillMatrix staff, SkillType skillType, int count, SkillLevel leaderLevel)
        {
            if (count <= 0)
            {
                return;
            }
            staff.Add(skillType, leaderLevel, 1);
            staff.Add(skillType, SkillLevel.Beginner, count - 1);
        }

        private SkillMatrix CalculateBaseLoadingStaff()
        {
            SkillMatrix staff = new SkillMatrix();
            int count = (int)Math.Ceiling(Length / 50);

            SkillLevel leaderLevel = SkillLevel.Beginner;
            if (Length > 150)
            {
                leaderLevel = SkillLevel.Expert;
            }
            else if (Length > 75)
            {
                leaderLevel = SkillLevel.Qualified;
            }

            AddStaffWithLeader(staff, SkillType.DockWorker, count, leaderLevel);
            return staff;
        }

        private SkillMatrix CalculateDockingStaff()
        {
            SkillMatrix staff = new SkillMatrix();
            staff.Add(SkillType.DockWorker, SkillLevel.Beginner, 3);

            if (Length > 200)
            {
                staff.Add(SkillType.Pilot, SkillLevel.Expert, 1);
            }
            else if (Length > 100)
            {
                staff.Add(SkillType.Pilot, SkillLevel.Qualified, 1);
            }
            return staff;
        }

        public bool CanDoTask(TaskType taskType)
        {
            if (IsBusy)
            {
                return false;
            }
            if (taskType == TaskType.Docking)
            {
                return !IsDocked;
            }
            return IsDocked;
        }

        public void Dock()
        {
            if (IsDocked)
            {
                throw new InvalidOperationException($"{Name} ist bereits angedockt.");
            }
            IsDocked = true;
        }

        public void Undock()
        {
            if (!IsDocked)
            {
                throw new InvalidOperationException($"{Name} ist nicht angedockt.");
            }
            IsDocked = false;
        }

        public void StartTask(TaskType taskType)
        {
            if (!CanDoTask(taskType))
            {
                throw new InvalidOperationException($"{Name} kann {taskType} jetzt nicht durchführen.");
            }
            if (taskType == TaskType.Docking)
            {
                Dock();
            }
            IsBusy = true;
        }

        public void EndTask()
        {
            IsBusy = false;
        }

        public virtual TimeSpan GetDuration(TaskType taskType)
        {
            switch (taskType)
            {
                case TaskType.Docking:
                    return TimeSpan.FromMinutes(30);
                case TaskType.Loading:
                    return TimeSpan.FromMinutes(60);
                case TaskType.Undocking:
                    return TimeSpan.FromMinutes(20);
                default:
                    throw new ArgumentException($"Unbekannter Tasktyp: {taskType}");
            }
        }

        public virtual string GetDescription()
        {
            return $"#{Id} {Name}, {Length} m lang, {Beam} m breit, {Draft} m Tiefgang";
        }

        public abstract void Load();
    }
}
