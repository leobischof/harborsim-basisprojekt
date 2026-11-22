// HarborSim – StaffPool.cs – Leo Bischof (12320283)
using System;
using System.Collections.Generic;

namespace HarborSim
{
    // Verwaltet alle Mitarbeiter der Simulation und wählt passendes Personal aus.
    public class StaffPool
    {
        private List<Worker> _workers;

        public StaffPool(List<Worker> workers)
        {
            _workers = workers;
        }

        public bool TrySelect(SkillMatrix requirement, out List<Worker> selected)
        {
            throw new NotImplementedException();
        }

        public string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}
