namespace ConsoleApp1
{
    internal class MatchService
    {
        private readonly MatchDataAccess _matchDataAccess;
        private readonly TeamService _teamService;
        private readonly PlayerService _playerService;

        public TeamService TeamService { get => _teamService; }
        public PlayerService PlayerService { get => _playerService; }

        public MatchService(DatabaseConnection dbConnection)
        {
            _matchDataAccess = new MatchDataAccess(dbConnection);

            _teamService = new TeamService(dbConnection);
            _playerService = new PlayerService(dbConnection);
        }

        public Match CreateMatch(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals)
        {
            Match newMatch = new Match(homeTeam, awayTeam, datePlayed, homeGoals, awayGoals);
            AssignTeamStatsToDatabase(newMatch);
            AssignPlayerStatsToDatabase(newMatch);

            if (_matchDataAccess.AddToDatabase(newMatch))
            {
                return newMatch;
            }
            else
            {
                throw new Exception("Failed to add match to the database.");
            }
        }

        public Match CreateMatch(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals, List<Player> scorers)
        {
            Match newMatch = new Match(homeTeam, awayTeam, datePlayed, homeGoals, awayGoals, scorers);
            AssignTeamStatsToDatabase(newMatch);
            AssignPlayerStatsToDatabase(newMatch);

            if (_matchDataAccess.AddToDatabase(newMatch))
            {
                return newMatch;
            }
            else
            {
                throw new Exception("Failed to add match to the database.");
            }
        }

        public Match CreateMatch(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals, List<Player> scorers, List<Player> assisters)
        {
            Match newMatch = new Match(homeTeam, awayTeam, datePlayed, homeGoals, awayGoals, scorers, assisters);
            AssignTeamStatsToDatabase(newMatch);
            AssignPlayerStatsToDatabase(newMatch);

            if (_matchDataAccess.AddToDatabase(newMatch))
            {
                return newMatch;
            }
            else
            {
                throw new Exception("Failed to add match to the database.");
            }
        }

        public List<Match> GetAllMatchesForLeague(League league)
        {
            List<Match> matches = _matchDataAccess.GetAllMatchesForLeagueFromDatabase(league);
            return matches;
        }

        public void AssignTeamStatsToDatabase(Match match)
        {
            TeamService.AddStatisticToDatabase(match.HomeTeam, "GamesPlayed");
            TeamService.AddStatisticToDatabase(match.HomeTeam, "GoalsFor");
            TeamService.AddStatisticToDatabase(match.HomeTeam, "GoalsAgainst");
            TeamService.AddStatisticToDatabase(match.AwayTeam, "GamesPlayed");
            TeamService.AddStatisticToDatabase(match.AwayTeam, "GoalsFor");
            TeamService.AddStatisticToDatabase(match.AwayTeam, "GoalsAgainst");

            if (match.Result == "Home")
            {
                TeamService.AddStatisticToDatabase(match.HomeTeam, "GamesWon");
                TeamService.AddStatisticToDatabase(match.AwayTeam, "GamesLost");
            }
            else if (match.Result == "Draw")
            {
                TeamService.AddStatisticToDatabase(match.HomeTeam, "GamesDrawn");
                TeamService.AddStatisticToDatabase(match.AwayTeam, "GamesDrawn");
            }
            else if (match.Result == "Away")
            {
                TeamService.AddStatisticToDatabase(match.HomeTeam, "GamesLost");
                TeamService.AddStatisticToDatabase(match.AwayTeam, "GamesWon");
            }

            TeamService.AddStatisticToDatabase(match.HomeTeam, "Points");
            TeamService.AddStatisticToDatabase(match.AwayTeam, "Points");
            TeamService.AddStatisticToDatabase(match.HomeTeam, "GoalDifference");
            TeamService.AddStatisticToDatabase(match.AwayTeam, "GoalDifference");
        }

        public void AssignPlayerStatsToDatabase(Match match)
        {
            foreach (Player player in match.Scorers)
            {
                PlayerService.AddStatisticToDatabase(player, "GoalsScored");
            }
            foreach (Player player in match.Assisters)
            {
                PlayerService.AddStatisticToDatabase(player, "Assists");
            }
            foreach (Player player in match.CleanSheets)
            {
                PlayerService.AddStatisticToDatabase(player, "CleanSheets");
            }
            foreach (Player player in match.YellowCards)
            {
                PlayerService.AddStatisticToDatabase(player, "YellowCards");
            }
            foreach (Player player in match.RedCards)
            {
                PlayerService.AddStatisticToDatabase(player, "RedCards");
            }
        }
    }
}
