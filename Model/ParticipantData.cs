using System;
using System.Collections.Generic;

namespace Model
{
    public class ParticipantData
    {
        public ParticipantData()
        {
            CurrentLap = 1;
            LapTimes = new Dictionary<int, DateTime>();
        }

        public int CurrentLap { get; set; }
        public Dictionary<int, DateTime> LapTimes { get; set; }

        public void AddLap()
        {
            LapTimes.Add(CurrentLap, DateTime.Now);
            CurrentLap++;
        }
    }
}