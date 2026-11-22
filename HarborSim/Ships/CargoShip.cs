// HarborSim – CargoShip.cs – Leo Bischof (12320283)
using System;

namespace HarborSim
{
    // Frachtschiff.
    // ": Ship" = erbt von Ship (Python: class CargoShip(Ship))
    public class CargoShip : Ship
    {
        // Anzahl der Container, kommt aus Val1 in ships.csv
        private int _containerNumber;

        // ": base(...)" ruft zuerst den Konstruktor von Ship auf (Python: super().__init__(...))
        public CargoShip(string name, double length, double beam, double draft, int containerNumber)
            : base(name, length, beam, draft)
        {
            _containerNumber = containerNumber;
        }

        // Basisbedarf an Hafenarbeitern + Kranführer:
        // je angefangene 400 Container einer, über 500 einer qualifiziert, über 1000 einer Experte.
        public override SkillMatrix LoadingStaff
        {
            get
            {
                // base.LoadingStaff = Bedarf aus Ship (Python: super().loading_staff)
                SkillMatrix staff = base.LoadingStaff;
                // 400.0 statt 400, sonst rechnet C# Ganzzahl-Division (120 / 400 = 0)
                int count = (int)Math.Ceiling(_containerNumber / 400.0);

                SkillLevel leaderLevel = SkillLevel.Beginner;
                if (_containerNumber > 1000)
                {
                    leaderLevel = SkillLevel.Expert;
                }
                else if (_containerNumber > 500)
                {
                    leaderLevel = SkillLevel.Qualified;
                }

                AddStaffWithLeader(staff, SkillType.CraneOperator, count, leaderLevel);
                return staff;
            }
        }

        // Ladezeit: 30 Minuten + Container / 10, auf ganze Minuten aufgerundet.
        // Docking und Undocking übernimmt die Basisklasse.
        public override TimeSpan GetDuration(TaskType taskType)
        {
            if (taskType == TaskType.Loading)
            {
                return TimeSpan.FromMinutes(Math.Ceiling(30 + _containerNumber / 10.0));
            }
            return base.GetDuration(taskType);
        }

        public override string GetDescription()
        {
            return $"{base.GetDescription()}, Frachtschiff mit {_containerNumber} Containern";
        }

        // Beladen wird nur durch eine Konsolenausgabe dargestellt.
        public override void Load()
        {
            if (!IsDocked)
            {
                throw new InvalidOperationException($"{Name} ist nicht angedockt und kann nicht beladen werden.");
            }
            Console.WriteLine($"       -> {Name}: {_containerNumber} Container verladen");
        }
    }
}
