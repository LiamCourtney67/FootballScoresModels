namespace ConsoleApp1
{
    internal class Team
    {
        private int _teamID = 1;
        private string _name;
        private League _league;
        private List<Player> _players = new List<Player>();
        private List<Match> matches = new List<Match>();
        private int _gamesPlayed;
        private int _gamesWon;
        private int _gamesDrawn;
        private int _gamesLost;
        private int _goalsFor;
        private int _goalsAgainst;
        private int _goalDifference;
        private int _points;

        private TeamService _teamService = new TeamService(new DatabaseConnection());

        public int TeamID { get => _teamID; set => _teamID = value; }
        public string Name { get => _name; set => _name = value; }
        public League League { get => _league; set => _league = value; }
        public List<Player> Players { get => _players; set => _players = value; }
        public List<Match> Matches { get => matches; set => matches = value; }
        public int GamesPlayed { get => _gamesPlayed; set => _gamesPlayed = value; }
        public int GamesWon { get => _gamesWon; set => _gamesWon = value; }
        public int GamesDrawn { get => _gamesDrawn; set => _gamesDrawn = value; }
        public int GamesLost { get => _gamesLost; set => _gamesLost = value; }
        public int GoalsFor { get => _goalsFor; set => _goalsFor = value; }
        public int GoalsAgainst { get => _goalsAgainst; set => _goalsAgainst = value; }
        public int GoalDifference { get => _goalDifference; set => _goalDifference = value; }
        public int Points { get => _points; set => _points = value; }

        public TeamService TeamService { get => _teamService; }

        public Team(string name, League league)
        {
            this.Name = name;
            this.League = league;
        }

        public Team(int teamID, string name, League league, int gamesPlayed, int gamesWon, int gamesDrawn, int gamesLost, int goalsFor, int goalsAgainst, int goalDifference, int points)
        {
            this.TeamID = teamID;
            this.League = league;
            this.Name = name;
            this.Players = TeamService.PlayerService.GetAllPlayersForTeam(this);
            this.GamesPlayed = gamesPlayed;
            this.GamesWon = gamesWon;
            this.GamesDrawn = gamesDrawn;
            this.GamesLost = gamesLost;
            this.GoalsFor = goalsFor;
            this.GoalsAgainst = goalsAgainst;
            this.GoalDifference = goalDifference;
            this.Points = points;
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
            player.Team = this;
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
            player.Team = null;
        }

        public void AddMatch(Match match)
        {
            Matches.Add(match);
        }

        public void RemoveMatch(Match match)
        {
            Matches.Remove(match);
        }

        public void CalculateStats()
        {
            GamesPlayed = GamesWon + GamesDrawn + GamesLost;
            Points = (GamesWon * 3) + GamesDrawn;
            GoalDifference = GoalsFor - GoalsAgainst;
        }
    }
}

