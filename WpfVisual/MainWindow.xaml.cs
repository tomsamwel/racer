using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Controller;
using Model;

namespace WpfVisual
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ParticipantInfo _participantInfo;
        private RaceInfo _raceInfo;

        public MainWindow()
        {
            InitializeComponent();
            _participantInfo = new ParticipantInfo();
            _raceInfo = new RaceInfo();
            ImageCache.Initialize();
            Data.Initialize();
            Data.NewRace += OnNewRace;
            Data.NextRace();
            Data.CurrentRace.RandomizeEquipment();

            Data.CurrentRace.Start();


            




            //for (; ; ) Thread.Sleep(100);
        }

        private void OnNewRace(RaceEventArgs e)
        {
            Visual.Initialize(e.Track);

            Data.CurrentRace.DriversChanged += OnDriversChanged;

            Stage.Dispatcher.BeginInvoke(
                DispatcherPriority.Render,
                new Action(() =>
                {
                    this.Stage.Source = null;
                    this.Stage.Source = Visual.DrawTrack(e.Track);
                }));
        }

        private void OnDriversChanged(DriversChangedEventArgs e)
        {
            Stage.Dispatcher.BeginInvoke(
                DispatcherPriority.Render,
                new Action(() =>
                {
                    this.Stage.Source = null;
                    this.Stage.Source = Visual.DrawTrack(e.Track);
                }));
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }



        private void MenuItem_ParticipantInfo_Click(object sender, RoutedEventArgs e)
        {
            //_participantInfo?.Close();
            
            _participantInfo.Show();
        }

        private void MenuItem_RaceInfo_Click(object sender, RoutedEventArgs e)
        {
            //_raceInfo?.Close();
            _raceInfo.Show();
        }
    }
}
