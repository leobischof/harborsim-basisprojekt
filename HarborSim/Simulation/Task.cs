using System;
using System.Collections.Generic;

namespace HarborSim
{
    public class Task
    {
        public TaskType Type { get; private set; }
        public Ship Ship { get; private set; }
        public List<Worker> Workers { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public Task(TaskType type, Ship ship)
        {
            Type = type;
            Ship = ship;
            Workers = new List<Worker>();
        }

        public TimeSpan GetDuration()
        {
            return Ship.GetDuration(Type);
        }

        public void StartTask(List<Worker> workers, DateTime startTime)
        {
            Workers = workers;
            StartTime = startTime;
            EndTime = startTime + GetDuration();

            Ship.StartTask(Type);
            foreach (Worker worker in Workers)
            {
                worker.StartTask();
            }
        }

        public void EndTask()
        {
            if (Type == TaskType.Loading)
            {
                Ship.Load();
            }
            else if (Type == TaskType.Undocking)
            {
                Ship.Undock();
            }

            foreach (Worker worker in Workers)
            {
                worker.EndTask();
            }
            Ship.EndTask();
        }

        public string GetDescription()
        {
            string description = $"{Type,-9} | {Ship.Name}";

            if (Workers.Count == 0)
            {
                return description;
            }

            string names = "";
            foreach (Worker worker in Workers)
            {
                if (names != "")
                {
                    names += ", ";
                }
                names += worker.Name;
            }
            return $"{description} | {StartTime:HH:mm}-{EndTime:HH:mm} | {names}";
        }
    }
}
