// HarborSim – ShipComparer.cs – Leo Bischof (12320283)
using System;
using System.Collections.Generic;

namespace HarborSim
{
    // Sortiert Schiffe nach Länge, bei gleicher Länge nach Name.
    // Wird nur für die Übersicht verwendet, nicht für die Reihenfolge der Simulation.
    //
    // IComparer = eigene Sortierregel als eigene Klasse, die man an Sort übergibt
    // (Python: ships.sort(key=lambda s: (s.length, s.name))).
    // Unterschied zu IComparable (siehe Worker): Dort sortiert sich die Klasse selbst,
    // hier entscheidet eine fremde Klasse. So kann es mehrere Sortierregeln geben.
    public class ShipComparer : IComparer<Ship>
    {
        // Rückgabe: < 0 -> x zuerst, 0 -> gleich, > 0 -> y zuerst
        public int Compare(Ship x, Ship y)
        {
            int result = x.Length.CompareTo(y.Length);
            if (result != 0)
            {
                return result;
            }
            // Ordinal = Zeichen für Zeichen vergleichen, unabhängig von der Spracheinstellung
            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }
    }
}
