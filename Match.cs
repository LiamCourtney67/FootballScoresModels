using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
    internal class Match
    {
        private int _matchID;
        private Team _homeTeam;
        private Team _awayTeam;
        private DateTime _datePlayed;
        private int _homeGoals;
        private int _awayGoals;
        private string _result;

        public int MatchID { get => _matchID; private set => _matchID = value; }
        public Team HomeTeam { get => _homeTeam; private set => _homeTeam = value; }
        public Team AwayTeam { get => _awayTeam; private set => _awayTeam = value; }
        public DateTime DatePlayed { get => _datePlayed; private set => _datePlayed = value; }
        public int HomeGoals { get => _homeGoals; private set => _homeGoals = value; }
        public int AwayGoals { get => _awayGoals; private set => _awayGoals = value; }
        public string Result { get => _result; private set => _result = value; }

        public Match(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals)
        {
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this.DatePlayed = datePlayed;
            this.HomeGoals = homeGoals;
            this.AwayGoals = awayGoals;

            if (AddToDatabase(new DatabaseConnection()))
            {
                CalculateResult();
                AssignPoints();
                homeTeam.CalculateStats();
                awayTeam.CalculateStats();
                homeTeam.League.SortTeams();

                CheckCleenSheets();
            }
            else { throw new Exception("Failed to add match to the database."); }
        }

        public Match(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals, List<Player> scorers)
        {
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this.DatePlayed = datePlayed;
            this.HomeGoals = homeGoals;
            this.AwayGoals = awayGoals;

            if (AddToDatabase(new DatabaseConnection()))
            {
                CalculateResult();
                AssignPoints();
                homeTeam.CalculateStats();
                awayTeam.CalculateStats();
                homeTeam.League.SortTeams();

                CheckCleenSheets();
                foreach (Player player in scorers)
                {
                    AddScorer(player, 1);
                }
            }
            else { throw new Exception("Failed to add match to the database."); }
        }

        public Match(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals, List<Player> scorers, List<Player> assisters)
        {
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this.DatePlayed = datePlayed;
            this.HomeGoals = homeGoals;
            this.AwayGoals = awayGoals;
            if (AddToDatabase(new DatabaseConnection()))
            {
                CalculateResult();
                AssignPoints();
                homeTeam.CalculateStats();
                awayTeam.CalculateStats();
                homeTeam.League.SortTeams();

                CheckCleenSheets();
                foreach (Player player in scorers)
                {
                    AddScorer(player, 1);
                }
                foreach (Player player in assisters)
                {
                    AddAssit(player, 1);
                }
            }
            else { throw new Exception("Failed to add match to the database."); }
        }

        public Match(int matchID, int homeTeamID, int awayTeamID, DateTime datePlayed, int homeGoals, int awayGoals, string result)
        {
            this.MatchID = matchID;
            this.HomeTeam = Team.GetTeamFromDatabase(homeTeamID, new DatabaseConnection());
            this.AwayTeam = Team.GetTeamFromDatabase(awayTeamID, new DatabaseConnection());
            this.DatePlayed = datePlayed;
            this.HomeGoals = homeGoals;
            this.AwayGoals = awayGoals;
            this.Result = result;
        }

        public bool DoesMatchExist(DatabaseConnection dbConnection)
        {
            if (dbConnection.OpenConnection())
            {
                try
                {
                    using (MySqlConnection connection = dbConnection.GetConnection())
                    {
                        string checkQuery = "SELECT COUNT(*) FROM Matches WHERE HomeTeamID = @HomeTeamID AND AwayTeamID = @AwayTeamID AND DatePlayed = @DatePlayed;";
                        using (MySqlCommand command = new MySqlCommand(checkQuery, connection))
                        {
                            command.Parameters.AddWithValue("@HomeTeamID", HomeTeam.TeamID);
                            command.Parameters.AddWithValue("@AwayTeamID", AwayTeam.TeamID);
                            command.Parameters.AddWithValue("@DatePlayed", DatePlayed.ToString("yyyy-MM-dd HH:mm:ss"));

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

        public bool AddToDatabase(DatabaseConnection dbConnection)
        {
            if (!DoesMatchExist(dbConnection))
            {
                if (dbConnection.OpenConnection())
                {
                    try
                    {
                        using (MySqlConnection connection = dbConnection.GetConnection())
                        {
                            string insertQuery = "INSERT INTO Matches (HomeTeamID, AwayTeamID, DatePlayed, HomeGoals, AwayGoals, Result) " +
                                ("VALUES (@HomeTeamID, @AwayTeamID, @DatePlayed, @HomeGoals, @AwayGoals, @Result); SELECT LAST_INSERT_ID();");
                            using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                            {
                                command.Parameters.AddWithValue("@HomeTeamID", HomeTeam.TeamID);
                                command.Parameters.AddWithValue("@AwayTeamID", AwayTeam.TeamID);
                                command.Parameters.AddWithValue("@DatePlayed", DatePlayed);
                                command.Parameters.AddWithValue("@HomeGoals", HomeGoals);
                                command.Parameters.AddWithValue("@AwayGoals", AwayGoals);
                                command.Parameters.AddWithValue("@Result", Result);

                                int matchID = Convert.ToInt32(command.ExecuteScalar());

                                if (matchID > 0)
                                {
                                    Console.WriteLine("New match added to the database with ID: " + matchID);
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
                Console.WriteLine("A match with the same teams and sate already exists.");
                return false;
            }
        }

        public static List<Match> GetAllMatchesFromDatabase(DatabaseConnection dbConnection)
        {
            List<Match> matches = new List<Match>();
            if (dbConnection.OpenConnection())
            {
                try
                {
                    using (MySqlConnection connection = dbConnection.GetConnection())
                    {
                        string selectQuery = "SELECT * FROM Matches;";
                        using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Match match = new Match(
                                        Convert.ToInt32(reader["MatchID"]),
                                        Convert.ToInt32(reader["HomeTeamID"]),
                                        Convert.ToInt32(reader["AwayTeamID"]),
                                        Convert.ToDateTime(reader["DatePlayed"]),
                                        Convert.ToInt32(reader["HomeGoals"]),
                                        Convert.ToInt32(reader["AwayGoals"]),
                                        reader["Result"].ToString()
                                    );
                                    matches.Add(match);

                                }
                                reader.Close();
                                return matches;
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
            throw new Exception("Failed to get match from the database.");
        }

        public static Match GetMatchFromDatabase(int matchID, DatabaseConnection dbConnection)
        {
            // TODO
        }


        private void CalculateResult()
        {
            if (HomeGoals > AwayGoals)
            {
                Result = "Home";
            }
            else if (AwayGoals > HomeGoals)
            {
                Result = "Away";
            }
            else
            {
                Result = "Draw";
            }
        }

        private void AssignPoints()
        {
            HomeTeam.GoalsFor += HomeGoals;
            HomeTeam.GoalsAgainst += AwayGoals;
            AwayTeam.GoalsFor += AwayGoals;
            AwayTeam.GoalsAgainst += HomeGoals;
            if (Result == "Home")
            {
                HomeTeam.GamesWon++;
                AwayTeam.GamesLost++;
            }
            else if (Result == "Draw")
            {
                HomeTeam.GamesDrawn++;
                AwayTeam.GamesDrawn++;
            }
            else if (Result == "Away")
            {
                HomeTeam.GamesLost++;
                AwayTeam.GamesWon++;
            }
        }

        private void CheckCleenSheets()
        {
            if (HomeGoals == 0)
            {
                foreach (Player player in AwayTeam.Players)
                {
                    if (player.Position == "Goalkeeper" || player.Position == "Defender")
                    {
                        AddCleanSheet(player, 1);
                    }
                }
            }
            if (AwayGoals == 0)
            {
                foreach (Player player in HomeTeam.Players)
                {
                    if (player.Position == "Goalkeeper" || player.Position == "Defender")
                    {
                        AddCleanSheet(player, 1);
                    }
                }
            }
        }

        public void AddScorer(Player player, int amount) => player.ScoreGoal(amount);
        public void AddAssit(Player player, int amount) => player.AssistGoal(amount);
        public void AddCleanSheet(Player player, int amount) => player.CleanSheet(amount);
        public void AddYellowCard(Player player, int amount) => player.YellowCard(amount);
        public void AddRedCard(Player player, int amount) => player.RedCard(amount);
    }
}
