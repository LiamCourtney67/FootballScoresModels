namespace ConsoleApp1
{
    internal class LeagueService
    {
        private readonly LeagueDataAccess _leagueDataAccess;
        private readonly TeamService _teamService;
        private readonly MatchService _matchService;

        public TeamService TeamService { get => _teamService; }
        public MatchService MatchService { get => _matchService; }

        public LeagueService(DatabaseConnection dbConnection)
        {
            _leagueDataAccess = new LeagueDataAccess(dbConnection);
            _teamService = new TeamService(dbConnection);
            _matchService = new MatchService(dbConnection);
        }

        public League CreateLeague(string name)
        {
            League newLeague = new League(name);

            if (_leagueDataAccess.AddToDatabase(newLeague))
            {
                return newLeague;
            }
            else
            {
                throw new Exception("Failed to add league to the database.");
            }
        }

        public League GetLeague(int leagueID)
        {
            League league = _leagueDataAccess.GetLeagueFromDatabase(leagueID);
            league.Teams = TeamService.GetAllTeamsForLeague(league);
            league.Matches = MatchService.GetAllMatchesForLeague(league);
            return league;
        }

        public List<League> GetAllLeagues()
        {
            List<League> leagues = _leagueDataAccess.GetAllLeaguesFromDatabase();
            foreach (League league in leagues)
            {
                league.Teams = _teamService.GetAllTeamsForLeague(league);
                league.Matches = MatchService.GetAllMatchesForLeague(league);
            }
            return leagues;
        }
    }

}
