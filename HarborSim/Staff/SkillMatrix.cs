using System;

namespace HarborSim
{
    public class SkillMatrix
    {
        private int[,] _matrix;

        public SkillMatrix()
        {
            int typeCount = Enum.GetValues(typeof(SkillType)).Length;
            int levelCount = Enum.GetValues(typeof(SkillLevel)).Length;
            _matrix = new int[typeCount, levelCount];
        }

        public void Add(SkillType skillType, SkillLevel skillLevel, int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("Die Anzahl darf nicht negativ sein.");
            }
            _matrix[(int)skillType, (int)skillLevel] += count;
        }

        public int GetCount(SkillType skillType, SkillLevel skillLevel)
        {
            return _matrix[(int)skillType, (int)skillLevel];
        }

        public int GetCount(SkillType skillType)
        {
            int sum = 0;
            for (int level = 0; level < _matrix.GetLength(1); level++)
            {
                sum += _matrix[(int)skillType, level];
            }
            return sum;
        }

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
