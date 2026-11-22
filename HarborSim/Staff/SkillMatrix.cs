// HarborSim – SkillMatrix.cs – Leo Bischof (12320283)
using System;

namespace HarborSim
{
    // Personalbedarf eines Tasks: wie viele Mitarbeiter je SkillType und Mindest-SkillLevel.
    // Enthält KEINE konkreten Mitarbeiter, nur Zahlen.
    //
    // Beispiel Anlegen eines 220 m Passagierschiffs:
    //                        Beginner  Qualified  Expert
    //   Pilot                    0         0        1
    //   DockWorker               3         0        0
    //   CraneOperator            0         0        0
    //   PassengerCoordinator     0         0        0
    public class SkillMatrix
    {
        // int[,] = zweidimensionales Array (Python: Liste von Listen bzw. numpy-Matrix).
        // Zeile = SkillType, Spalte = SkillLevel. Zugriff mit _matrix[zeile, spalte].
        private int[,] _matrix;

        public SkillMatrix()
        {
            // Enum.GetValues liefert alle Werte eines enums -> Anzahl Zeilen bzw. Spalten.
            // So passt die Matrix automatisch, wenn ein neuer SkillType dazukommt.
            int typeCount = Enum.GetValues(typeof(SkillType)).Length;
            int levelCount = Enum.GetValues(typeof(SkillLevel)).Length;
            _matrix = new int[typeCount, levelCount];
        }

        // Erhöht den Bedarf für eine Kombination aus SkillType und SkillLevel.
        public void Add(SkillType skillType, SkillLevel skillLevel, int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("Die Anzahl darf nicht negativ sein.");
            }
            // (int) wandelt den enum-Wert in seine Zahl um (Pilot -> 0, DockWorker -> 1, ...)
            _matrix[(int)skillType, (int)skillLevel] += count;
        }

        // Überladung 1: Bedarf für genau einen SkillType und ein SkillLevel.
        // Überladung = gleicher Methodenname, andere Parameter (gibt es in Python so nicht).
        public int GetCount(SkillType skillType, SkillLevel skillLevel)
        {
            return _matrix[(int)skillType, (int)skillLevel];
        }

        // Überladung 2: Bedarf eines SkillTypes über alle Levels (eine Zeile aufsummiert).
        public int GetCount(SkillType skillType)
        {
            int sum = 0;
            // GetLength(1) = Anzahl Spalten, GetLength(0) = Anzahl Zeilen
            for (int level = 0; level < _matrix.GetLength(1); level++)
            {
                sum += _matrix[(int)skillType, level];
            }
            return sum;
        }

        // Überladung 3: Bedarf eines SkillLevels über alle SkillTypes (eine Spalte aufsummiert).
        public int GetCount(SkillLevel skillLevel)
        {
            int sum = 0;
            for (int type = 0; type < _matrix.GetLength(0); type++)
            {
                sum += _matrix[type, (int)skillLevel];
            }
            return sum;
        }
    }
}
