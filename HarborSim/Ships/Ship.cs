// HarborSim – Ship.cs – Leo Bischof (12320283)
using System;

namespace HarborSim
{
    // Basisklasse aller Schiffe.
    // abstract = von Ship selbst gibt es keine Objekte, nur von CargoShip und PassengerShip
    // (Python: class Ship(ABC)).
    public abstract class Ship
    {
        // Zähler für eindeutige IDs, gemeinsam für alle Schiffe (Klassenvariable)
        private static int _nextId = 1;

        public int Id { get; private set; }
        public string Name { get; private set; }
        public double Length { get; private set; }
        public double Beam { get; private set; }
        public double Draft { get; private set; }
        public bool IsBusy { get; private set; }
        public bool IsDocked { get; private set; }

        // Property nur mit get = wird bei jedem Zugriff neu berechnet (Python: @property).
        public SkillMatrix DockingStaff
        {
            get { return CalculateDockingStaff(); }
        }

        // virtual = abgeleitete Klassen dürfen das mit "override" ersetzen.
        // CargoShip und PassengerShip holen sich hier den Basisbedarf und ergänzen eigenes Personal.
        public virtual SkillMatrix LoadingStaff
        {
            get { return CalculateBaseLoadingStaff(); }
        }

        // protected = nur diese Klasse und abgeleitete Klassen dürfen den Konstruktor aufrufen
        protected Ship(string name, double length, double beam, double draft)
        {
            // Erst prüfen, dann zuweisen. throw beendet den Konstruktor sofort, das Objekt entsteht nicht.
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

        // Hilfsmethode für alle Regeln der Form "je X einer, ab Y einer qualifiziert,
        // ab Z einer Experte": einer bekommt leaderLevel, alle anderen Beginner.
        // static = braucht kein Objekt, arbeitet nur mit den Parametern.
        protected static void AddStaffWithLeader(SkillMatrix staff, SkillType skillType, int count, SkillLevel leaderLevel)
        {
            if (count <= 0)
            {
                return;
            }
            staff.Add(skillType, leaderLevel, 1);
            staff.Add(skillType, SkillLevel.Beginner, count - 1);
        }

        // Beladen: je angefangene 50 m ein Hafenarbeiter,
        // über 75 m einer davon qualifiziert, über 150 m einer davon Experte.
        private SkillMatrix CalculateBaseLoadingStaff()
        {
            SkillMatrix staff = new SkillMatrix();
            // Math.Ceiling rundet auf (Python: math.ceil), (int) macht daraus eine ganze Zahl
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

        // An- und Ablegen: immer drei Hafenarbeiter beliebigen Levels,
        // über 100 m ein qualifizierter Lotse, über 200 m stattdessen ein Experten-Lotse.
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

        // Prüft nur, ob der Task jetzt möglich wäre. Ändert NICHTS am Zustand.
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
            // Loading und Undocking gehen nur im angedockten Zustand
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

        // Start eines Tasks: beim Docking wird gleich zu Beginn angedockt.
        public void StartTask(TaskType taskType)
        {
            // Verhindert eine Doppelbelegung des Schiffs
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

        // Standardzeiten der Basisklasse. Abgeleitete Klassen überschreiben Loading.
        public virtual TimeSpan GetDuration(TaskType taskType)
        {
            // klassisches switch (Python: match taskType:)
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

        // abstract = keine Umsetzung hier, jede abgeleitete Klasse MUSS Load schreiben
        public abstract void Load();
    }
}
