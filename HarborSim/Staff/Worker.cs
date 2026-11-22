// HarborSim – Worker.cs – Leo Bischof (12320283)
using System;

namespace HarborSim
{
    // Ein Mitarbeiter im Hafen.
    // ": IComparable<Worker>" = Worker verspricht eine Methode CompareTo (Python: __lt__).
    // Dadurch kann List<Worker>.Sort() die Mitarbeiter ohne weitere Angaben sortieren.
    public class Worker : IComparable<Worker>
    {
        // static = gehört zur Klasse, nicht zum einzelnen Objekt (Python: Klassenvariable).
        // Jeder neue Worker holt sich die nächste freie Nummer -> eindeutige ID.
        private static int _nextId = 1;

        // Property mit "private set": von außen nur lesbar, ändern nur innerhalb der Klasse
        // (Python: @property ohne Setter)
        public int Id { get; private set; }
        public string Name { get; private set; }
        public SkillType SkillType { get; private set; }
        public SkillLevel SkillLevel { get; private set; }
        public bool IsBusy { get; private set; }

        // Konstruktor (Python: __init__)
        public Worker(string name, SkillType skillType, SkillLevel skillLevel)
        {
            Id = _nextId;
            _nextId++;
            Name = name;
            SkillType = skillType;
            SkillLevel = skillLevel;
            IsBusy = false;
        }

        // Wird beim tatsächlichen Start eines Tasks aufgerufen.
        public void StartTask()
        {
            // Ein beschäftigter Mitarbeiter darf keinem zweiten Task zugewiesen werden.
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

        // Vergleich für Sort(): negativ = this kommt zuerst, 0 = gleich, positiv = other zuerst.
        // Sortiert wird nach SkillLevel (Beginner zuerst), bei Gleichstand nach Id,
        // damit die Reihenfolge immer gleich ist.
        public int CompareTo(Worker other)
        {
            int result = SkillLevel.CompareTo(other.SkillLevel);
            if (result == 0)
            {
                result = Id.CompareTo(other.Id);
            }
            return result;
        }

        // $"..." = interpolierter String (Python: f"...")
        public string GetDescription()
        {
            return $"#{Id} {Name} ({SkillType}, {SkillLevel})";
        }
    }
}
