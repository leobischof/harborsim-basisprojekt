using System;

namespace HarborSim
{
    public class CargoShip : Ship
    {
        private int _containerNumber;

        public CargoShip(string name, double length, double beam, double draft, int containerNumber)
            : base(name, length, beam, draft)
        {
            if (containerNumber < 0)
            {
                throw new ArgumentException($"{name}: Die Anzahl der Container darf nicht negativ sein.");
            }
            _containerNumber = containerNumber;
        }

        public override SkillMatrix LoadingStaff
        {
            get
            {
                SkillMatrix staff = base.LoadingStaff;
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
