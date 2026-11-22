// HarborSim – SkillType.cs – Leo Bischof (12320283)
namespace HarborSim
{
    // enum = feste Liste benannter Werte (Python: class SkillType(Enum))
    // Die Reihenfolge ist zugleich die Zeilennummer in der SkillMatrix:
    // Pilot = 0, DockWorker = 1, CraneOperator = 2, PassengerCoordinator = 3
    public enum SkillType
    {
        Pilot,
        DockWorker,
        CraneOperator,
        PassengerCoordinator
    }
}
