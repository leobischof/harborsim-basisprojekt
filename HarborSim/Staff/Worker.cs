// HarborSim – Worker.cs – Leo Bischof (12320283)
using System;

namespace HarborSim
{
    // Ein Mitarbeiter im Hafen.
    // ": IComparable<Worker>" = Worker verspricht eine Methode CompareTo (Python: __lt__)
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
            Name = name;
            SkillType = skillType;
            SkillLevel = skillLevel;
        }

        public void StartTask()
        {
            throw new NotImplementedException();
        }

        public void EndTask()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(Worker other)
        {
            throw new NotImplementedException();
        }

        public string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}
