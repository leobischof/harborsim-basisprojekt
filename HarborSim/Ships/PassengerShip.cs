// HarborSim – PassengerShip.cs – Leo Bischof (12320283)
using System;

namespace HarborSim
{
    // Passagierschiff.
    // ": Ship" = erbt von Ship (Python: class PassengerShip(Ship))
    public class PassengerShip : Ship
    {
        // Anzahl der Passagiere, kommt aus Val1 in ships.csv
        private int _passengerNumber;

        public PassengerShip(string name, double length, double beam, double draft, int passengerNumber)
            : base(name, length, beam, draft)
        {
            _passengerNumber = passengerNumber;
        }

        // Basisbedarf an Hafenarbeitern + Passagierkoordinatoren:
        // je angefangene 750 Passagiere einer, über 1000 einer qualifiziert, über 2500 einer Experte.
        public override SkillMatrix LoadingStaff
        {
            get
            {
                SkillMatrix staff = base.LoadingStaff;
                int count = (int)Math.Ceiling(_passengerNumber / 750.0);

                SkillLevel leaderLevel = SkillLevel.Beginner;
                if (_passengerNumber > 2500)
                {
                    leaderLevel = SkillLevel.Expert;
                }
                else if (_passengerNumber > 1000)
                {
                    leaderLevel = SkillLevel.Qualified;
                }

                AddStaffWithLeader(staff, SkillType.PassengerCoordinator, count, leaderLevel);
                return staff;
            }
        }

        // Ladezeit: 15 Minuten + Passagiere / 50, auf ganze Minuten aufgerundet.
        public override TimeSpan GetDuration(TaskType taskType)
        {
            if (taskType == TaskType.Loading)
            {
                return TimeSpan.FromMinutes(Math.Ceiling(15 + _passengerNumber / 50.0));
            }
            return base.GetDuration(taskType);
        }

        public override string GetDescription()
        {
            return $"{base.GetDescription()}, Passagierschiff mit {_passengerNumber} Passagieren";
        }

        public override void Load()
        {
            if (!IsDocked)
            {
                throw new InvalidOperationException($"{Name} ist nicht angedockt und kann nicht beladen werden.");
            }
            Console.WriteLine($"       -> {Name}: {_passengerNumber} Passagiere an Bord");
        }
    }
}
