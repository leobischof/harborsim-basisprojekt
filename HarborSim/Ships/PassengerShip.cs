using System;

namespace HarborSim
{
    public class PassengerShip : Ship
    {
        private int _passengerNumber;

        public PassengerShip(string name, double length, double beam, double draft, int passengerNumber)
            : base(name, length, beam, draft)
        {
            if (passengerNumber < 0)
            {
                throw new ArgumentException($"{name}: Die Anzahl der Passagiere darf nicht negativ sein.");
            }
            _passengerNumber = passengerNumber;
        }

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
