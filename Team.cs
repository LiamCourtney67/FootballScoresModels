using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
    internal class Team
    {
        private int _teamID;
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

        public int TeamID { get => _teamID; private set => _teamID = value; }
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
            if (AddToDatabase(new DatabaseConnection()))
            {
                league.AddTeam(this);
            }
            else { throw new Exception("Failed to add team to the database."); }
        }


        private bool DoesTeamNameExistsInLeague(DatabaseConnection dbConnection)
        {
            if (dbConnection.OpenConnection())
            {
                try
                {
                    using (MySqlConnection connection = dbConnection.GetConnection())
                    {
                        string checkQuery = "SELECT COUNT(*) FROM Teams WHERE TeamName = @TeamName AND LeagueID = @LeagueID;";
                        using (MySqlCommand command = new MySqlCommand(checkQuery, connection))
                        {
                            command.Parameters.AddWithValue("@TeamName", Name);
                            command.Parameters.AddWithValue("@LeagueID", League.LeagueID);

                            int count = Convert.ToInt32(command.ExecuteScalar());

                            return count > 0;
                        }
                    }
                }
                finally
                {
                    dbConnection.CloseConnection();
                }
            }
            else
            {
                Console.WriteLine("Failed to open the database connection.");
                return false;
            }
        }

        private bool AddToDatabase(DatabaseConnection dbConnection)
        {
            if (!DoesTeamNameExistsInLeague(dbConnection))
            {
                if (dbConnection.OpenConnection())
                {
                    try
                    {
                        using (MySqlConnection connection = dbConnection.GetConnection())
                        {
                            string insertQuery = "INSERT INTO Teams (TeamName, LeagueID, GamesPlayed, GamesWon, GamesDrawn, GamesLost, GoalsFor, GoalsAgainst, GoalDifference, Points) VALUES (@TeamName, @LeagueID, @GamesPlayed, @GamesWon, @GamesDrawn, @GamesLost, @GoalsFor, @GoalsAgainst, @GoalDifference, @Points); SELECT LAST_INSERT_ID();";
                            using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                            {
                                command.Parameters.AddWithValue("@TeamName", Name);
                                command.Parameters.AddWithValue("@LeagueID", League.LeagueID);
                                command.Parameters.AddWithValue("@GamesPlayed", GamesPlayed);
                                command.Parameters.AddWithValue("@GamesWon", GamesWon);
                                command.Parameters.AddWithValue("@GamesDrawn", GamesDrawn);
                                command.Parameters.AddWithValue("@GamesLost", GamesLost);
                                command.Parameters.AddWithValue("@GoalsFor", GoalsFor);
                                command.Parameters.AddWithValue("@GoalsAgainst", GoalsAgainst);
                                command.Parameters.AddWithValue("@GoalDifference", GoalDifference);
                                command.Parameters.AddWithValue("@Points", Points);

                                TeamID = Convert.ToInt32(command.ExecuteScalar());

                                if (TeamID > 0)
                                {
                                    Console.WriteLine("New team added to the database with ID: " + TeamID);
                                    return true;
                                }
                                return true;
                            }
                        }
                    }
                    finally
                    {
                        dbConnection.CloseConnection();
                    }
                }
                else
                {
                    Console.WriteLine("Failed to open the database connection.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("A team with the same name already exists in the same league.");
                return false;
            }
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

        public void CalculateStats()
        {
            GamesPlayed = GamesWon + GamesDrawn + GamesLost;
            Points = (GamesWon * 3) + GamesDrawn;
            GoalDifference = GoalsFor - GoalsAgainst;
        }
    }
}
