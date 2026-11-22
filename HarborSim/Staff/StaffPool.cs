// HarborSim – StaffPool.cs – Leo Bischof (12320283)
using System;
using System.Collections.Generic;

namespace HarborSim
{
    // Verwaltet alle Mitarbeiter der Simulation und wählt passendes Personal für Tasks aus.
    public class StaffPool
    {
        private List<Worker> _workers;

        public StaffPool(List<Worker> workers)
        {
            // Eigene Kopie der Liste, damit das Sortieren die Liste des Aufrufers nicht verändert
            // (Python: list(workers))
            _workers = new List<Worker>(workers);
        }

        // Versucht, den gesamten Personalbedarf zu erfüllen (Flowchart "Personalauswahl").
        // "out" = zweiter Rückgabewert: der Aufrufer bekommt die Auswahl über diesen Parameter
        // (Python: return erfolg, auswahl).
        // Wichtig: Hier wird niemand beschäftigt (IsBusy bleibt false). Das passiert erst
        // beim Start des Tasks in Task.StartTask.
        public bool TrySelect(SkillMatrix requirement, out List<Worker> selected)
        {
            // Sortieren mit Worker.CompareTo -> Beginner zuerst.
            // So wird immer die niedrigste noch ausreichende Qualifikation gewählt.
            _workers.Sort();
            selected = new List<Worker>();

            // Jede Kombination aus SkillType und SkillLevel durchgehen
            foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
            {
                foreach (SkillLevel skillLevel in Enum.GetValues(typeof(SkillLevel)))
                {
                    int needed = requirement.GetCount(skillType, skillLevel);
                    if (needed == 0)
                    {
                        continue;
                    }

                    foreach (Worker worker in _workers)
                    {
                        bool fits = !worker.IsBusy
                            && worker.SkillType == skillType
                            && worker.SkillLevel >= skillLevel
                            && !selected.Contains(worker);
                        if (fits)
                        {
                            selected.Add(worker);
                            needed--;
                        }
                        if (needed == 0)
                        {
                            break;
                        }
                    }

                    // Keine Mitarbeiter mehr übrig, aber Bedarf noch offen -> alles oder nichts
                    if (needed > 0)
                    {
                        selected.Clear();
                        return false;
                    }
                }
            }
            return true;
        }

        // Liste aller Mitarbeiter, eine Zeile pro Mitarbeiter.
        public string GetDescription()
        {
            string description = "";
            foreach (Worker worker in _workers)
            {
                description += "  " + worker.GetDescription() + Environment.NewLine;
            }
            return description;
        }
    }
}
