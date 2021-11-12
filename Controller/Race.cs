using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Model;

namespace Controller
{
    public class Race
    {
        public delegate void OnDriversChanged(DriversChangedEventArgs e);

        public delegate void OnRaceEvent(RaceEventArgs e);


        private readonly Dictionary<Section, SectionData> _positions;
        private readonly Random _random;
        private readonly Timer _timer;
        
        public Dictionary<IParticipant, ParticipantData> _laps;

        public Race(Track track, List<IParticipant> participants)
        {
            Track = track;
            Participants = participants;

            StartTime = new DateTime();

            _random = new Random(DateTime.Now.Millisecond);

            _timer = new Timer(100);
            _timer.Elapsed += OnTimedEvent;

            _positions = new Dictionary<Section, SectionData>();
            _laps = new Dictionary<IParticipant, ParticipantData>();
            
            Laps = 3;

            PlaceParticipants();
        }

        private int Laps { get; }

        public Track Track { get; set; }
        public List<IParticipant> Participants { get; set; }
        public DateTime StartTime { get; set; }
        public event OnDriversChanged DriversChanged;
        public event OnRaceEvent RaceEnd;

        
        public void Start()
        {
            StartTime = DateTime.Now;
            _timer.Start();
        }

        private void EndRace()
        {
            _timer.Stop();
            _timer.Elapsed -= OnTimedEvent;
            DriversChanged = null;

            RaceEnd?.Invoke(new RaceEventArgs());
            RaceEnd = null;
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            if (NextStep())
            {
                DriversChangedEventArgs dce = new DriversChangedEventArgs
                {
                    Track = Track
                };
                DriversChanged?.Invoke(dce);
            }
        }

        private bool NextStep()
        {
            bool participantMoved = false;
            LinkedListNode<Section> section = Track.Sections.Last;

            while (section != null)
            {
                SectionData sectionData = GetSectionData(section.Value);

                if (sectionData.Left != null)
                {
                    IParticipant participant = sectionData.Left;
                    LinkedListNode<Section> toSection = GetNextSection(section);
                    SectionData toSectionData = GetSectionData(toSection.Value);
                    sectionData.DistanceLeft += GenerateDistance(participant.Equipment);
                    ChanceToBreakOrRepair(participant.Equipment);
                    
                    if (sectionData.DistanceLeft > 100)
                    {
                        if (toSectionData.Left == null)
                        {
                            toSectionData.Left = participant;
                            toSectionData.DistanceLeft = sectionData.DistanceLeft - 100;
                            participantMoved = true;
                            sectionData.Left = null;
                            sectionData.DistanceLeft = 0;
                            if (section.Value.SectionType == SectionTypes.Finish)
                            {
                                LapParticipant(participant, toSection);
                            }
                        }
                        else if (toSectionData.Right == null)
                        {
                            toSectionData.Right = participant;
                            toSectionData.DistanceRight = sectionData.DistanceLeft - 100;
                            participantMoved = true;
                            sectionData.Left = null;
                            sectionData.DistanceLeft = 0;
                            if (section.Value.SectionType == SectionTypes.Finish)
                            {
                                LapParticipant(participant, toSection);
                            }
                        }
                    }
                }

                if (sectionData.Right != null)
                {
                    IParticipant participant = sectionData.Right;
                    LinkedListNode<Section> toSection = GetNextSection(section);
                    SectionData toSectionData = GetSectionData(toSection.Value);
                    sectionData.DistanceRight += GenerateDistance(participant.Equipment);
                    ChanceToBreakOrRepair(participant.Equipment);

                    if (sectionData.DistanceRight > 100)
                    {
                        if (toSectionData.Left == null)
                        {
                            toSectionData.Left = participant;
                            toSectionData.DistanceLeft = sectionData.DistanceRight - 100;
                            participantMoved = true;
                            sectionData.Right = null;
                            sectionData.DistanceRight = 0;
                            if (section.Value.SectionType == SectionTypes.Finish)
                            {
                                LapParticipant(participant, toSection);
                            }
                        }
                        else if (toSectionData.Right == null)
                        {
                            toSectionData.Right = participant;
                            toSectionData.DistanceRight = sectionData.DistanceRight - 100;
                            participantMoved = true;
                            sectionData.Right = null;
                            sectionData.DistanceRight = 0;
                            if (section.Value.SectionType == SectionTypes.Finish)
                            {
                                LapParticipant(participant, toSection);
                            }
                        }
                    }
                }

                section = section.Previous;
            }

            return participantMoved;
        }

        private void ChanceToBreakOrRepair(IEquipment equipment)
        {
            if (equipment.IsBroken)
            {
                equipment.IsBroken = _random.Next(0, 10) != 1;
            }
            else if (_random.Next(0,50) == 1)
            {
                equipment.IsBroken = true;
                equipment.Speed /= 2;
            }
        }

        private void LapParticipant(IParticipant participant, LinkedListNode<Section> currentSection)
        {
            ParticipantData participantData = GetParticipantData(participant);
            participantData.AddLap();

            if (participantData.CurrentLap > Laps) FinishParticipant(participant, currentSection);
        }

        private void FinishParticipant(IParticipant participant, LinkedListNode<Section> currentSection)
        {
            SectionData sectionData = GetSectionData(currentSection.Value);
            if (sectionData.Left == participant)
            {
                sectionData.Left = null;
                sectionData.DistanceLeft = 0;
            }

            if (sectionData.Right == participant)
            {
                sectionData.Right = null;
                sectionData.DistanceRight = 0;
            }

            if (AllParticipantsFinished())
            {
                EndRace();
            }
        }
        
        private bool AllParticipantsFinished()
        {
            foreach (ParticipantData participantData in Participants.Select(GetParticipantData))
            {
                if (participantData.CurrentLap <= Laps) return false;
            }

            return true;
        }
        
        private LinkedListNode<Section> GetNextSection(LinkedListNode<Section> section) => section.Next ?? Track.Sections.First;

        public int GenerateDistance(IEquipment equipment)
        {
            int distance = 0;
            if (equipment.IsBroken)
            {
                return distance;
            }

            // TODO: create a better distance generator
            distance = (equipment.Quality + equipment.Speed + equipment.Performance + _random.Next(1, 100)) / 4;

            // boundary 1..100
            distance = Math.Min(distance, 100);
            distance = Math.Max(distance, 1);
            return distance;
        }

        public SectionData GetSectionData(Section section)
        {
            if (_positions.ContainsKey(section) == false) _positions.Add(section, new SectionData());

            return _positions[section];
        }

        public ParticipantData GetParticipantData(IParticipant participant)
        {
            if (_laps.ContainsKey(participant) == false) _laps.Add(participant, new ParticipantData());
            
            return _laps[participant];
        }


        public void RandomizeEquipment()
        {
            foreach (IParticipant participant in Participants)
            {
                Car newCar = new Car
                {
                    Performance = _random.Next(0, 101),
                    Quality = _random.Next(0, 101),
                    Speed = _random.Next(0, 101),
                    IsBroken = false
                };
                participant.Equipment = newCar;
            }
        }

        private void PlaceParticipants()
        {
            Queue<IParticipant> participants = new Queue<IParticipant>(Participants);

            foreach (Section section in Track.Sections.Where(section => section.SectionType == SectionTypes.StartGrid))
            {
                SectionData sectionData = GetSectionData(section);

                sectionData.Left ??= participants.Dequeue();
                if (participants.Count == 0) return;

                sectionData.Right ??= participants.Dequeue();
                if (participants.Count == 0) return;
            }
        }
    }
}