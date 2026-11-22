// HarborSim – CargoShip.cs – Leo Bischof (12320283)
using System;

namespace HarborSim
{
    // ": Ship" = erbt von Ship (Python: class CargoShip(Ship))
    public class CargoShip : Ship
    {
        private int _containerNumber;

        public CargoShip(string name, double length, double beam, double draft, int containerNumber)
            : base(name, length, beam, draft)
        {
            _containerNumber = containerNumber;
        }

        public override TimeSpan GetDuration(TaskType taskType)
        {
            throw new NotImplementedException();
        }

        public override string GetDescription()
        {
            throw new NotImplementedException();
        }

        public override void Load()
        {
            throw new NotImplementedException();
        }
    }
}
