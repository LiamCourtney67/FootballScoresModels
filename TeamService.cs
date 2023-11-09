using System.Reflection;

namespace ConsoleApp1
{
    internal class TeamService
    {
        private readonly TeamDataAccess _teamDataAccess;
        private readonly PlayerService _playerService;

        public PlayerService PlayerService { get => _playerService; }

        public TeamService(DatabaseConnection dbConnection)
        {
            _teamDataAccess = new TeamDataAccess(dbConnection);

            _playerService = new PlayerService(dbConnection);
        }

        public Team CreateTeam(string name, League league)
        {
            Team newTeam = new Team(name, league);

            if (_teamDataAccess.AddToDatabase(newTeam))
            {
                return newTeam;
            }
            else
            {
                throw new Exception("Failed to add team to the database.");
            }
        }

        public List<Team> GetAllTeamsForLeague(League league)
        {
            List<Team> teams = _teamDataAccess.GetAllTeamsForLeagueFromDatabase(league);
            foreach (Team team in teams)
            {
                team.Players = PlayerService.GetAllPlayersForTeam(team);
            }
            return teams;
        }

        public void AddStatisticToDatabase(Team team, String stat)
        {
            PropertyInfo statPropertyInfo = typeof(Team).GetProperty(stat);
            _teamDataAccess.AddStatisticToDatabase(team, statPropertyInfo);
        }
    }
}
