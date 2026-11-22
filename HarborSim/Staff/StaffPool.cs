using System;
using System.Collections.Generic;

namespace HarborSim
{
    public class StaffPool
    {
        private List<Worker> _workers;

        public StaffPool(List<Worker> workers)
        {
            _workers = new List<Worker>(workers);
        }

        public bool TrySelect(SkillMatrix requirement, out List<Worker> selected)
        {
            _workers.Sort();
            selected = new List<Worker>();

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

                    if (needed > 0)
                    {
                        selected.Clear();
                        return false;
                    }
                }
            }
            return true;
        }

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
