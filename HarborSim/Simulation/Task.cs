// HarborSim – Task.cs – Leo Bischof (12320283)
using System;
using System.Collections.Generic;

namespace HarborSim
{
    // Ein Arbeitsauftrag: ein TaskType für ein bestimmtes Schiff.
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

        public void StartTask(List<Worker> workers, DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public void EndTask()
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetDuration()
        {
            throw new NotImplementedException();
        }

        public string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}
