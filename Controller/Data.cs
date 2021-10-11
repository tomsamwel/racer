using Model;

namespace Controller
{
    public static class Data
    {
        public static Competition Competition { get; set; }
        public static Race CurrentRace { get; set; }


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
            Competition.Participants.Add(new Driver("Tom", TeamColors.Red));
            Competition.Participants.Add(new Driver("Bob", TeamColors.Yellow));
            Competition.Participants.Add(new Driver("Jake", TeamColors.Green));
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
                new[] {SectionTypes.StartGrid, SectionTypes.Straight, SectionTypes.Finish}));
            Competition.Queue.Enqueue(new Track("track 3",
                new[] {SectionTypes.StartGrid, SectionTypes.Straight, SectionTypes.Finish}));
        }

        public static void NextRace()
        {
            Track nextTrack = Competition.NextTrack();
            if (nextTrack != null) CurrentRace = new Race(nextTrack, Competition.Participants);
        }
    }
}