namespace Model
{
    public class Driver : IParticipant
    {
        public Driver(string name, int points, IEquipment equipment, TeamColors teamColor)
        {
            Name = name;
            Points = points;
            Equipment = equipment;
            TeamColor = teamColor;
        }

        //public Driver(): this("Dummy", 0, new Car(), TeamColors.Blue)
        //{}
        public Driver(string name, TeamColors teamColor) : this(name, 0, null, teamColor)
        {
        }


        public string Name { get; set; }
        public int Points { get; set; }
        public IEquipment Equipment { get; set; }
        public TeamColors TeamColor { get; set; }
    }
}