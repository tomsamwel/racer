using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Controller;
using Model;

namespace WpfVisual
{
    class StatisticsDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string TrackName { get; set; }
        public List<IParticipant> Participants { get; set; }

        public IParticipant Participant1 { get; set; }
        public IParticipant Participant2 { get; set; }
        public IParticipant Participant3 { get; set; }

        public ParticipantData Participant1Data { get; set; }
        public ParticipantData Participant2Data { get; set; }
        public ParticipantData Participant3Data { get; set; }

        public List<DateTime> Participant1Laps { get; set; }
        public List<DateTime> Participant2Laps { get; set; }
        public List<DateTime> Participant3Laps { get; set; }



        public StatisticsDataContext()
        {
            Data.NewRace += OnNextRace;
        }

        public void OnNextRace(RaceEventArgs e)
        {
            TrackName = e.Track.Name;
            Participants = Data.Competition.Participants;

            Data.CurrentRace.DriversChanged += OnDriversChanged;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));

        }

        public void OnDriversChanged(DriversChangedEventArgs e)
        {

            Participants = Data.Competition.Participants.ToList();
            Participant1 = Data.Competition.Participants[0];
            Participant2 = Data.Competition.Participants[1];
            Participant3 = Data.Competition.Participants[2];

            Participant1Data = Data.CurrentRace.GetParticipantData(Participant1);
            Participant2Data = Data.CurrentRace.GetParticipantData(Participant2);
            Participant3Data = Data.CurrentRace.GetParticipantData(Participant3);

            Participant1Laps = Data.CurrentRace.GetParticipantData(Participant1).LapTimes.Values.ToList();
            Participant2Laps = Data.CurrentRace.GetParticipantData(Participant2).LapTimes.Values.ToList();
            Participant3Laps = Data.CurrentRace.GetParticipantData(Participant3).LapTimes.Values.ToList();



            //LapTimes = _lapTimeStorage.GetList().ToList();
            //SectionTimes = _sectionTimeStorage.GetList().ToList();
            //BestLapTime = _lapTimeStorage.BestParticipant();
            //BestSectionTime = _sectionTimeStorage.BestParticipant();
            //Participants = CurrentRace.Participants.ToList();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }
}
