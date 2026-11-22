// HarborSim – SkillLevel.cs – Leo Bischof (12320283)
namespace HarborSim
{
    // Qualifikation eines Mitarbeiters, aufsteigend sortiert.
    // Die Zahlen passen zur Spalte SkillLevel in workers.csv (0, 1, 2)
    // und zur Spaltennummer in der SkillMatrix.
    // Weil enums intern Zahlen sind, geht auch: level >= SkillLevel.Qualified
    public enum SkillLevel
    {
        Beginner = 0,
        Qualified = 1,
        Expert = 2
    }
}
