using MySql.Data.MySqlClient;

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

        public int TeamID { get => _teamID; private set => _teamID = value; }
        public string Name { get => _name; set => _name = value; }
        public League League { get => _league; set => _league = value; }
        public List<Player> Players { get => _players; private set => _players = value; }
        public List<Match> Matches { get => matches; set => matches = value; }
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
            this.Name = name;
            this.League = league;
            if (AddToDatabase(new DatabaseConnection()))
            {
                league.AddTeam(this);
            }
            else { throw new Exception("Failed to add team to the database."); }
        }

        public Team(int teamID, string name, int leagueID, int gamesPlayed, int gamesWon, int gamesDrawn, int gamesLost, int goalsFor, int goalsAgainst, int goalDifference, int points)
        {
            this.TeamID = teamID;
            LeagueService leagueService = new LeagueService(new DatabaseConnection());
            this.League = leagueService.GetLeague(leagueID);
            this.Name = name;
            this.Players = Player.GetAllPlayersForTeamFromDatabase(this, new DatabaseConnection());
            this.GamesPlayed = gamesPlayed;
            this.GamesWon = gamesWon;
            this.GamesDrawn = gamesDrawn;
            this.GamesLost = gamesLost;
            this.GoalsFor = goalsFor;
            this.GoalsAgainst = goalsAgainst;
            this.GoalDifference = goalDifference;
            this.Points = points;
        }

        public Team(int teamID, string name, League league, int gamesPlayed, int gamesWon, int gamesDrawn, int gamesLost, int goalsFor, int goalsAgainst, int goalDifference, int points)
        {
            this.TeamID = teamID;
            this.League = league;
            this.Name = name;
            this.Players = Player.GetAllPlayersForTeamFromDatabase(this, new DatabaseConnection());
            this.GamesPlayed = gamesPlayed;
            this.GamesWon = gamesWon;
            this.GamesDrawn = gamesDrawn;
            this.GamesLost = gamesLost;
            this.GoalsFor = goalsFor;
            this.GoalsAgainst = goalsAgainst;
            this.GoalDifference = goalDifference;
            this.Points = points;
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

        public static List<Team> GetAllTeamsForLeagueFromDatabase(League league, DatabaseConnection dbConnection)
        {
            List<Team> teams = new List<Team>();

            if (dbConnection.OpenConnection())
            {
                try
                {
                    using (MySqlConnection connection = dbConnection.GetConnection())
                    {
                        string selectQuery = $"SELECT * FROM Teams WHERE LeagueID = {league.LeagueID};";
                        using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Team team = new Team(
                                    Convert.ToInt32(reader["TeamID"]),
                                    reader["TeamName"].ToString(),
                                    league,
                                    Convert.ToInt32(reader["GamesPlayed"]),
                                    Convert.ToInt32(reader["GamesWon"]),
                                    Convert.ToInt32(reader["GamesDrawn"]),
                                    Convert.ToInt32(reader["GamesLost"]),
                                    Convert.ToInt32(reader["GoalsFor"]),
                                    Convert.ToInt32(reader["GoalsAgainst"]),
                                    Convert.ToInt32(reader["GoalDifference"]),
                                    Convert.ToInt32(reader["Points"])
                                    );
                                    teams.Add(team);

                                }
                                reader.Close();
                                return teams;
                            }
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
            }
            throw new Exception("Failed to get team from the database.");
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
            AddStatistic(TeamID, 0, GamesPlayed);

            Points = (GamesWon * 3) + GamesDrawn;
            AddStatistic(TeamID, 7, Points);

            GoalDifference = GoalsFor - GoalsAgainst;
            AddStatistic(TeamID, 6, GoalDifference);
        }


        public void AddStatistic(int teamID, int index, int amount)
        {
            string[] stats = { "GamesPlayed", "GamesWon", "GamesDrawn", "GamesLost", "GoalsFor", "GoalsAgainst", "GoalDifference", "Points" };

            DatabaseConnection dbConnection = new DatabaseConnection();

            if (dbConnection.OpenConnection())
            {
                try
                {
                    using (MySqlConnection connection = dbConnection.GetConnection())
                    {
                        string columnName = stats[index];
                        string updateQuery = $"UPDATE Teams SET {columnName} = {amount} WHERE TeamID = {teamID};";
                        using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                        {
                            command.ExecuteNonQuery();
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
            }
        }
    }
}

