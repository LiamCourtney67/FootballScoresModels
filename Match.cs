namespace ConsoleApp1
{
    internal class Match
    {
        private int _matchID;
        private Team _homeTeam;
        private Team _awayTeam;
        private League _league;
        private DateTime _datePlayed;
        private int _homeGoals;
        private int _awayGoals;
        private List<Player> _scorers = new List<Player>();
        private List<Player> _assisters = new List<Player>();
        private List<Player> _yellowCards = new List<Player>();
        private List<Player> _redCards = new List<Player>();
        private List<Player> _cleanSheets = new List<Player>();
        private string _result;

        private PlayerService _playerService = new PlayerService(new DatabaseConnection());
        private TeamService _teamService = new TeamService(new DatabaseConnection());

        public int MatchID { get => _matchID; set => _matchID = value; }
        public Team HomeTeam { get => _homeTeam; set => _homeTeam = value; }
        public Team AwayTeam { get => _awayTeam; set => _awayTeam = value; }
        public League League { get => _league; set => _league = value; }
        public DateTime DatePlayed { get => _datePlayed; set => _datePlayed = value; }
        public int HomeGoals { get => _homeGoals; set => _homeGoals = value; }
        public int AwayGoals { get => _awayGoals; set => _awayGoals = value; }
        public List<Player> Scorers { get => _scorers; set => _scorers = value; }
        public List<Player> Assisters { get => _assisters; set => _assisters = value; }
        public List<Player> YellowCards { get => _yellowCards; set => _yellowCards = value; }
        public List<Player> RedCards { get => _redCards; set => _redCards = value; }
        public List<Player> CleanSheets { get => _cleanSheets; set => _cleanSheets = value; }
        public string Result { get => _result; set => _result = value; }

        public Match(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals)
        {
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this.DatePlayed = datePlayed;
            this.HomeGoals = homeGoals;
            this.AwayGoals = awayGoals;
            this.Result = CalculateResult();
            HomeTeam.League.SortTeams();
            HomeTeam.League.AddMatch(this);

            if (HomeTeam.League.LeagueID == AwayTeam.League.LeagueID)
            {
                this.League = HomeTeam.League;
            }
            else { throw new Exception("The teams are not in the same league."); }

            AssignTeamStats();
            CheckCleenSheets();
        }

        public Match(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals, List<Player> scorers)
        {
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this.DatePlayed = datePlayed;
            this.HomeGoals = homeGoals;
            this.AwayGoals = awayGoals;
            this.Result = CalculateResult();
            HomeTeam.League.SortTeams();
            HomeTeam.League.AddMatch(this);
            HomeTeam.League.SortMatches();

            if (HomeTeam.League.LeagueID == AwayTeam.League.LeagueID)
            {
                this.League = HomeTeam.League;
            }
            else { throw new Exception("The teams are not in the same league."); }

            AssignTeamStats();
            CheckCleenSheets();

            foreach (Player player in scorers)
            {
                Scorers.Add(player);
                player.ScoreGoal(1);
            }
        }

        public Match(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals, List<Player> scorers, List<Player> assisters)
        {
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this.DatePlayed = datePlayed;
            this.HomeGoals = homeGoals;
            this.AwayGoals = awayGoals;
            this.Result = CalculateResult();
            HomeTeam.League.SortTeams();
            HomeTeam.League.AddMatch(this);

            if (HomeTeam.League.LeagueID == AwayTeam.League.LeagueID)
            {
                this.League = HomeTeam.League;
            }
            else { throw new Exception("The teams are not in the same league."); }

            AssignTeamStats();
            CheckCleenSheets();

            foreach (Player player in scorers)
            {
                Scorers.Add(player);
                player.ScoreGoal(1);
            }
            foreach (Player player in assisters)
            {
                Assisters.Add(player);
                player.AssistGoal(1);
            }
        }

        public Match(int matchID, Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals, string result)
        {
            this.MatchID = matchID;
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this.DatePlayed = datePlayed;
            this.HomeGoals = homeGoals;
            this.AwayGoals = awayGoals;
            this.Result = result;

            if (HomeTeam.League.LeagueID == AwayTeam.League.LeagueID)
            {
                this.League = HomeTeam.League;
            }
            else { throw new Exception("The teams are not in the same league."); }
        }

        private string CalculateResult()
        {
            if (HomeGoals > AwayGoals)
            {
                return "Home";
            }
            else if (AwayGoals > HomeGoals)
            {
                return "Away";
            }
            else
            {
                return "Draw";
            }
        }

        public void AssignTeamStats()
        {
            HomeTeam.GamesPlayed++;
            HomeTeam.GoalsFor += HomeGoals;
            HomeTeam.GoalsAgainst += AwayGoals;
            AwayTeam.GamesPlayed++;
            AwayTeam.GoalsFor += AwayGoals;
            AwayTeam.GoalsAgainst += HomeGoals;

            if (Result == "Home")
            {
                HomeTeam.GamesWon++;
                _teamService.AddStatisticToDatabase(HomeTeam, "GamesWon");
                AwayTeam.GamesLost++;
                _teamService.AddStatisticToDatabase(AwayTeam, "GamesLost");
            }
            else if (Result == "Draw")
            {
                HomeTeam.GamesDrawn++;
                _teamService.AddStatisticToDatabase(HomeTeam, "GamesDrawn");
                AwayTeam.GamesDrawn++;
                _teamService.AddStatisticToDatabase(AwayTeam, "GamesDrawn");
            }
            else if (Result == "Away")
            {
                HomeTeam.GamesLost++;
                _teamService.AddStatisticToDatabase(HomeTeam, "GamesLost");
                AwayTeam.GamesWon++;
                _teamService.AddStatisticToDatabase(AwayTeam, "GamesWon");
            }

            HomeTeam.Points = (HomeTeam.GamesWon * 3) + HomeTeam.GamesDrawn;
            AwayTeam.Points = (AwayTeam.GamesWon * 3) + AwayTeam.GamesDrawn;
            HomeTeam.GoalDifference = HomeTeam.GoalsFor - HomeTeam.GoalsAgainst;
            AwayTeam.GoalDifference = AwayTeam.GoalsFor - AwayTeam.GoalsAgainst;
        }

        private void CheckCleenSheets()
        {
            if (HomeGoals == 0)
            {
                foreach (Player player in AwayTeam.Players)
                {
                    if (player.Position == "Goalkeeper" || player.Position == "Defender")
                    {
                        CleanSheets.Add(player);
                        player.CleanSheet();
                    }
                }
            }
            if (AwayGoals == 0)
            {
                foreach (Player player in HomeTeam.Players)
                {
                    if (player.Position == "Goalkeeper" || player.Position == "Defender")
                    {
                        CleanSheets.Add(player);
                        player.CleanSheet();
                    }
                }
            }
        }
    }
}
