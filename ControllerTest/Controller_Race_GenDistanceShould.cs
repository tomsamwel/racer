using Controller;
using Model;
using NUnit.Framework;

namespace ControllerTest
{
    [TestFixture]
    public class Controller_Race_GenDistanceShould
    {
        [SetUp]
        public void SetUp()
        {
            Data.Initialize();
            Data.NextRace();
            _race = Data.CurrentRace;
        }

        private Race _race;

        [Test]
        public void GenDistance_AverageCar_ReturnGreaterThan0()
        {
            Car car = new Car
            {
                Quality = 50,
                Speed = 50,
                Performance = 50,
                IsBroken = false
            };

            int result = _race.GenerateDistance(car);

            Assert.Greater(result, 0);
        }

        [Test]
        public void GenDistance_AverageCar_ReturnLessThan101()
        {
            Car car = new Car
            {
                Quality = 50,
                Speed = 50,
                Performance = 50,
                IsBroken = false
            };

            int result = _race.GenerateDistance(car);

            Assert.Less(result, 101);
        }

        [Test]
        public void GenDistance_MaxCar_ReturnLessThan101()
        {
            Car car = new Car
            {
                Quality = 100,
                Speed = 100,
                Performance = 100,
                IsBroken = false
            };

            int result = _race.GenerateDistance(car);

            Assert.Less(result, 101);
        }

        [Test]
        public void GenDistance_MinCar_ReturnGreaterThan101()
        {
            Car car = new Car
            {
                Quality = 1,
                Speed = 1,
                Performance = 1,
                IsBroken = true
            };

            int result = _race.GenerateDistance(car);

            Assert.Greater(result, 0);
        }
    }
}