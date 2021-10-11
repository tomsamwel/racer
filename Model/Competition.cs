using System.Collections.Generic;

namespace Model
{
    public class Competition
    {
        public Competition(List<IParticipant> participants, Queue<Track> queue)
        {
            Participants = participants;
            Queue = queue;
        }

        public Competition() : this(new List<IParticipant>(), new Queue<Track>())
        {
        }

        public List<IParticipant> Participants { get; set; }
        public Queue<Track> Queue { get; set; }


        public Track NextTrack()
        {
            return Queue.Count == 0 ? null : Queue.Dequeue();
        }
    }
}