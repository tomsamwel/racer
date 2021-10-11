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

        private readonly Dictionary<Section, SectionData> _positions;
        private readonly Random _random;
        private readonly Timer _timer;

        public Race(Track track, List<IParticipant> participants)
        {
            Track = track;
            Participants = participants;

            StartTime = new DateTime();

            _random = new Random(DateTime.Now.Millisecond);

            _timer = new Timer(500);
            _timer.Elapsed += OnTimedEvent;

            _positions = new Dictionary<Section, SectionData>();

            PlaceParticipants();
        }

        public Track Track { get; set; }
        public List<IParticipant> Participants { get; set; }
        public DateTime StartTime { get; set; }
        public event OnDriversChanged DriversChanged;

        public void Start()
        {
            _timer.Start();
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
                    sectionData.DistanceLeft += GenDistance(sectionData.Left.Equipment);
                    if (sectionData.DistanceLeft > 100)
                    {
                        bool hasMoved = MoveParticipant(sectionData.Left, section, sectionData.DistanceLeft - 100);
                        if (hasMoved)
                        {
                            sectionData.Left = null;
                            sectionData.DistanceLeft = 0;
                            participantMoved = true;
                        }
                    }
                }

                if (sectionData.Right != null)
                {
                    sectionData.DistanceRight += GenDistance(sectionData.Right.Equipment);
                    if (sectionData.DistanceRight > 100)
                    {
                        bool hasMoved = MoveParticipant(sectionData.Right, section, sectionData.DistanceRight - 100);
                        if (hasMoved)
                        {
                            sectionData.Right = null;
                            sectionData.DistanceRight = 0;
                            participantMoved = true;
                        }
                    }
                }

                section = section.Previous;
            }

            return participantMoved;
        }

        private bool MoveParticipant(IParticipant participant, LinkedListNode<Section> currentSection, int distance)
        {
            bool hasMoved;
            hasMoved = false;
            SectionData nextSectionData = GetNextSectionData(currentSection);

            if (nextSectionData.Left == null)
            {
                nextSectionData.Left = participant;
                nextSectionData.DistanceLeft = distance;
                hasMoved = true;
            }
            else if (nextSectionData.Right == null)
            {
                nextSectionData.Right = participant;
                nextSectionData.DistanceRight = distance;
                hasMoved = true;
            }

            return hasMoved;
        }

        private SectionData GetNextSectionData(LinkedListNode<Section> section)
        {
            LinkedListNode<Section> nextSection = section.Next ?? Track.Sections.First;
            SectionData nextSectionData = nextSection != null ? GetSectionData(nextSection.Value) : null;
            return nextSectionData;
        }

        public int GenDistance(IParticipant participant)
        {
            return GenDistance(participant.Equipment);
        }

        public int GenDistance(IEquipment equipment)
        {
            int distance;

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
            Queue<IParticipant> participantsToPlace = new Queue<IParticipant>(Participants);
            
            foreach (Section section in Track.Sections.Where(section => section.SectionType == SectionTypes.StartGrid))
            {
                SectionData sectionData = GetSectionData(section);

                sectionData.Left ??= participantsToPlace.Dequeue();
                if (participantsToPlace.Count == 0) return;

                sectionData.Right ??= participantsToPlace.Dequeue();
                if (participantsToPlace.Count == 0) return;
            }
        }
    }
}