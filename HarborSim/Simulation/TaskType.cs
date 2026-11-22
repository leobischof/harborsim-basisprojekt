// HarborSim – TaskType.cs – Leo Bischof (12320283)
namespace HarborSim
{
    // Die drei Arbeitsschritte, die jedes Schiff der Reihe nach durchläuft:
    // Anlegen -> Beladen -> Ablegen
    public enum TaskType
    {
        Docking,
        Loading,
        Undocking
    }
}
