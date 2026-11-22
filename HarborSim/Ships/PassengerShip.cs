// HarborSim – PassengerShip.cs – Leo Bischof (12320283)
using System;

namespace HarborSim
{
    // ": Ship" = erbt von Ship (Python: class PassengerShip(Ship))
    public class PassengerShip : Ship
    {
        private int _passengerNumber;

        public PassengerShip(string name, double length, double beam, double draft, int passengerNumber)
            : base(name, length, beam, draft)
        {
            _passengerNumber = passengerNumber;
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
