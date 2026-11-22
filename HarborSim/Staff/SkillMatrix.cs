// HarborSim – SkillMatrix.cs – Leo Bischof (12320283)
using System;

namespace HarborSim
{
    // Personalbedarf eines Tasks: wie viele Mitarbeiter je SkillType und Mindest-SkillLevel.
    public class SkillMatrix
    {
        // Zeile = SkillType, Spalte = SkillLevel
        private int[,] _matrix;

        public SkillMatrix()
        {
            throw new NotImplementedException();
        }

        public void Add(SkillType skillType, SkillLevel skillLevel, int count)
        {
            throw new NotImplementedException();
        }

        public int GetCount(SkillType skillType, SkillLevel skillLevel)
        {
            throw new NotImplementedException();
        }
    }
}
