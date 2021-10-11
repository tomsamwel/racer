using Model;
using NUnit.Framework;

namespace ControllerTest
{
    [TestFixture]
    public class Model_Competition_NextTrackShould
    {
        [SetUp]
        public void SetUp()
        {
            _competition = new Competition();
        }

        private Competition _competition;

        [Test]
        public void NextTrack_EmptyQueue_ReturnNull()
        {
            Track result = _competition.NextTrack();
            Assert.IsNull(result);
        }

        [Test]
        public void NextTrack_OneInQueue_ReturnTrack()
        {
            Track track = new Track("track 1",
                new[] {SectionTypes.StartGrid, SectionTypes.Straight, SectionTypes.Finish});
            _competition.Queue.Enqueue(track);
            Track result = _competition.NextTrack();
            Assert.AreEqual(track, result);
        }

        [Test]
        public void NextTrack_OneInQueue_RemoveTrackFromQueue()
        {
            Track track = new Track("track 1",
                new[] {SectionTypes.StartGrid, SectionTypes.Straight, SectionTypes.Finish});
            _competition.Queue.Enqueue(track);
            _competition.NextTrack();
            Track result = _competition.NextTrack();
            Assert.Null(result);
        }

        [Test]
        public void NextTrack_TwoInQueue_ReturnNextTrack()
        {
            Track track1 = new Track("track 1",
                new[] {SectionTypes.StartGrid, SectionTypes.Straight, SectionTypes.Finish});
            Track track2 = new Track("track 2",
                new[] {SectionTypes.StartGrid, SectionTypes.Straight, SectionTypes.Finish});

            _competition.Queue.Enqueue(track1);
            _competition.Queue.Enqueue(track2);

            Track result1 = _competition.NextTrack();
            Track result2 = _competition.NextTrack();
            Assert.AreEqual(result2, track2);
        }
    }
}