namespace ConsoleApp1
{
    internal class Team
    {
        private string _name;
        private League _league;
        private List<Player> _players = new List<Player>();
        private int _gamesPlayed;
        private int _gamesWon;
        private int _gamesDrawn;
        private int _gamesLost;
        private int _goalsFor;
        private int _goalsAgainst;
        private int _goalDifference;
        private int _points;


        public string Name { get => _name; set => _name = value; }
        public League League { get => _league; set => _league = value; }
        public List<Player> Players { get => _players; }
        public int GamesPlayed { get => _gamesPlayed; set => _gamesPlayed = value; }
        public int GamesWon { get => _gamesWon; set => _gamesWon = value; }
        public int GamesDrawn { get => _gamesDrawn; set => _gamesDrawn = value; }
        public int GamesLost { get => _gamesLost; set => _gamesLost = value; }
        public int GoalsFor { get => _goalsFor; set => _goalsFor = value; }
        public int GoalsAgainst { get => _goalsAgainst; set => _goalsAgainst = value; }
        public int GoalDifference { get => _goalDifference; set => _goalDifference = value; }
        public int Points { get => _points; set => _points = value; }

        public Team(string name, League league)
        {
            Name = name;
            League = league;
            league.AddTeam(this);
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
            player.PlayerTeam = this;
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
            player.PlayerTeam = null;
        }

        public void CalculateStats()
        {
            GamesPlayed = GamesWon + GamesDrawn + GamesLost;
            Points = (GamesWon * 3) + GamesDrawn;
            GoalDifference = GoalsFor - GoalsAgainst;
        }
    }
}
