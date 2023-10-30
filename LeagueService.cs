namespace ConsoleApp1
{
    internal class LeagueService
    {
        private readonly LeagueDataAccess _leagueDataAccess;

        public LeagueService(DatabaseConnection dbConnection)
        {
            _leagueDataAccess = new LeagueDataAccess(dbConnection);
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
            league.Teams = Team.GetAllTeamsForLeagueFromDatabase(league, new DatabaseConnection());
            league.Matches = Match.GetAllMatchesForLeagueFromDatabase(league, new DatabaseConnection());
            return league;
        }

        public List<League> GetAllLeagues()
        {
            List<League> leagues = _leagueDataAccess.GetAllLeaguesFromDatabase();
            foreach (League league in leagues)
            {
                league.Teams = Team.GetAllTeamsForLeagueFromDatabase(league, new DatabaseConnection());
                league.Matches = Match.GetAllMatchesForLeagueFromDatabase(league, new DatabaseConnection());
            }
            return leagues;
        }
    }

}
