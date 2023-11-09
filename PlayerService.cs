using System.Reflection;

namespace ConsoleApp1
{
    internal class PlayerService
    {
        private readonly PlayerDataAccess _playerDataAccess;

        public PlayerService(DatabaseConnection dbConnection)
        {
            _playerDataAccess = new PlayerDataAccess(dbConnection);
        }

        public Player CreatePlayer(string firstName, string lastName, Team team)
        {
            Player newPlayer = new Player(firstName, lastName, team);

            if (_playerDataAccess.AddToDatabase(newPlayer))
            {
                return newPlayer;
            }
            else
            {
                throw new Exception("Failed to add player to the database.");
            }
        }

        public Player CreatePlayer(string firstName, string lastName, int age, int kitNumber, int positionKey, Team team)
        {
            Player newPlayer = new Player(firstName, lastName, age, kitNumber, positionKey, team);

            if (_playerDataAccess.AddToDatabase(newPlayer))
            {
                return newPlayer;
            }
            else
            {
                throw new Exception("Failed to add player to the database.");
            }
        }

        public List<Player> GetAllPlayersForTeam(Team team)
        {
            return _playerDataAccess.GetAllPlayersForTeamFromDatabase(team);
        }

        public void AddStatisticToDatabase(Player player, String stat)
        {
            PropertyInfo statPropertyInfo = typeof(Player).GetProperty(stat);
            _playerDataAccess.AddStatisticToDatabase(player, statPropertyInfo);
        }
    }
}
