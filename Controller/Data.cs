using Model;

namespace Controller
{
    public static class Data
    {
        public static Competition Competition { get; set; }
        public static Race CurrentRace { get; set; }
        
        public delegate void OnNewRace(RaceEventArgs e);
        public static event OnNewRace NewRace;




        public static void Initialize(Competition competition)
        {
            Competition = competition;
            AddParticipants();
            AddTracks();
        }

        public static void Initialize()
        {
            Initialize(new Competition());
        }

        private static void AddParticipants()
        {
            Competition.Participants.Add(new Driver("Tom", 0, null, TeamColors.Red));
            Competition.Participants.Add(new Driver("Bob", 0, null, TeamColors.Yellow));
            Competition.Participants.Add(new Driver("Jake", 0, null, TeamColors.Green));
        }

        private static void AddTracks()
        {
            Competition.Queue.Enqueue(new Track("track 1",
                new[]
                {
                    SectionTypes.StartGrid, SectionTypes.StartGrid, SectionTypes.RightCorner, SectionTypes.LeftCorner,
                    SectionTypes.LeftCorner,
                    SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.LeftCorner,
                    SectionTypes.LeftCorner, SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight,
                    SectionTypes.Straight,
                    SectionTypes.LeftCorner, SectionTypes.Straight, SectionTypes.LeftCorner, SectionTypes.Finish
                }));
            Competition.Queue.Enqueue(new Track("track 2",
                new[] {SectionTypes.StartGrid,SectionTypes.StartGrid, SectionTypes.Straight, SectionTypes.LeftCorner,SectionTypes.LeftCorner,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.Straight,SectionTypes.LeftCorner,SectionTypes.LeftCorner,SectionTypes.Finish}));
            Competition.Queue.Enqueue(new Track("track 3",
                new[] {SectionTypes.StartGrid,SectionTypes.StartGrid, SectionTypes.Straight,SectionTypes.RightCorner,SectionTypes.RightCorner, SectionTypes.Finish}));
        }

        private static void OnRaceEnd(RaceEventArgs e)
        {
            NextRace();
            CurrentRace.Start();
        }

        public static void NextRace()
        {
            Track nextTrack = Competition.NextTrack();
            if (nextTrack != null)
            {
                CurrentRace = new Race(nextTrack, Competition.Participants);
                CurrentRace.RaceEnd += OnRaceEnd;
                NewRace?.Invoke(new RaceEventArgs(){Track = nextTrack});
            }

        }
    }
}