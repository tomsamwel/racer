using System.Threading;
using Controller;

namespace View
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Data.Initialize();
            Data.NextRace();
            Data.CurrentRace.RandomizeEquipment();

            Visual.Initialize(Data.CurrentRace.Track);

            Data.CurrentRace.DriversChanged += Visual.OnDriversChanged;
            Data.CurrentRace.Start();


            for (;;) Thread.Sleep(100);
        }
    }
}