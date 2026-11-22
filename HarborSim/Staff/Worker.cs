using System;

namespace HarborSim
{
    public class Worker : IComparable<Worker>
    {
        private static int _nextId = 1;

        public int Id { get; private set; }
        public string Name { get; private set; }
        public SkillType SkillType { get; private set; }
        public SkillLevel SkillLevel { get; private set; }
        public bool IsBusy { get; private set; }

        public Worker(string name, SkillType skillType, SkillLevel skillLevel)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Der Name eines Mitarbeiters darf nicht leer sein.");
            }
            Id = _nextId;
            _nextId++;
            Name = name;
            SkillType = skillType;
            SkillLevel = skillLevel;
            IsBusy = false;
        }

        public void StartTask()
        {
            if (IsBusy)
            {
                throw new InvalidOperationException($"{Name} ist bereits beschäftigt.");
            }
            IsBusy = true;
        }

        public void EndTask()
        {
            IsBusy = false;
        }

        public int CompareTo(Worker other)
        {
            int result = SkillLevel.CompareTo(other.SkillLevel);
            if (result == 0)
            {
                result = Id.CompareTo(other.Id);
            }
            return result;
        }

        public string GetDescription()
        {
            return $"#{Id} {Name} ({SkillType}, {SkillLevel})";
        }
    }
}
