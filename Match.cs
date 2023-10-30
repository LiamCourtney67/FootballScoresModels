using MySql.Data.MySqlClient;

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
        private string _result;

        public int MatchID { get => _matchID; private set => _matchID = value; }
        public Team HomeTeam { get => _homeTeam; private set => _homeTeam = value; }
        public Team AwayTeam { get => _awayTeam; private set => _awayTeam = value; }
        public League League { get => _league; set => _league = value; }
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
            this.Result = CalculateResult();
            HomeTeam.League.SortTeams();
            HomeTeam.League.AddMatch(this);

            if (HomeTeam.League.LeagueID == AwayTeam.League.LeagueID)
            {
                this.League = HomeTeam.League;
            }
            else { throw new Exception("The teams are not in the same league."); }


            if (AddToDatabase(new DatabaseConnection()))
            {
                AssignPoints();
                HomeTeam.CalculateStats();
                AwayTeam.CalculateStats();
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
            this.Result = CalculateResult();
            HomeTeam.League.SortTeams();
            HomeTeam.League.AddMatch(this);
            HomeTeam.League.SortMatches();

            if (HomeTeam.League.LeagueID == AwayTeam.League.LeagueID)
            {
                this.League = HomeTeam.League;
            }
            else { throw new Exception("The teams are not in the same league."); }


            if (AddToDatabase(new DatabaseConnection()))
            {
                AssignPoints();
                HomeTeam.CalculateStats();
                AwayTeam.CalculateStats();
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
            this.Result = CalculateResult();
            HomeTeam.League.SortTeams();
            HomeTeam.League.AddMatch(this);

            if (HomeTeam.League.LeagueID == AwayTeam.League.LeagueID)
            {
                this.League = HomeTeam.League;
            }
            else { throw new Exception("The teams are not in the same league."); }

            if (AddToDatabase(new DatabaseConnection()))
            {
                AssignPoints();
                HomeTeam.CalculateStats();
                AwayTeam.CalculateStats();
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
                            string insertQuery = "INSERT INTO Matches (HomeTeamID, AwayTeamID, LeagueID, DatePlayed, HomeGoals, AwayGoals, Result) " +
                                ("VALUES (@HomeTeamID, @AwayTeamID, @LeagueID, @DatePlayed, @HomeGoals, @AwayGoals, @Result); SELECT LAST_INSERT_ID();");
                            using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                            {
                                command.Parameters.AddWithValue("@HomeTeamID", HomeTeam.TeamID);
                                command.Parameters.AddWithValue("@AwayTeamID", AwayTeam.TeamID);
                                command.Parameters.AddWithValue("@LeagueID", League.LeagueID);
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

        public static List<Match> GetAllMatchesForLeagueFromDatabase(League league, DatabaseConnection dbConnection)
        {
            List<Match> matches = new List<Match>();
            if (dbConnection.OpenConnection())
            {
                try
                {
                    using (MySqlConnection connection = dbConnection.GetConnection())
                    {
                        string selectQuery = $"SELECT * FROM Matches WHERE LeagueID = {league.LeagueID};";
                        using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Match match = new Match(
                                        Convert.ToInt32(reader["MatchID"]),
                                        league.Teams.Find(team => team.TeamID == Convert.ToInt32(reader["HomeTeamID"])),
                                        league.Teams.Find(team => team.TeamID == Convert.ToInt32(reader["AwayTeamID"])),
                                        Convert.ToDateTime(reader["DatePlayed"]),
                                        Convert.ToInt32(reader["HomeGoals"]),
                                        Convert.ToInt32(reader["AwayGoals"]),
                                        reader["Result"].ToString()
                                    );
                                    match.HomeTeam.AddMatch(match);
                                    match.AwayTeam.AddMatch(match);
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

        private void AssignPoints()
        {
            HomeTeam.GamesPlayed++;
            AddStatistic(HomeTeam.TeamID, 0, 1);

            HomeTeam.GoalsFor += HomeGoals;
            AddStatistic(HomeTeam.TeamID, 4, HomeGoals);

            HomeTeam.GoalsAgainst += AwayGoals;
            AddStatistic(HomeTeam.TeamID, 5, AwayGoals);

            AwayTeam.GamesPlayed++;
            AddStatistic(AwayTeam.TeamID, 0, 1);

            AwayTeam.GoalsFor += AwayGoals;
            AddStatistic(AwayTeam.TeamID, 4, AwayGoals);

            AwayTeam.GoalsAgainst += HomeGoals;
            AddStatistic(AwayTeam.TeamID, 5, HomeGoals);

            if (Result == "Home")
            {
                HomeTeam.GamesWon++;
                AddStatistic(HomeTeam.TeamID, 1, 1);
                AwayTeam.GamesLost++;
                AddStatistic(AwayTeam.TeamID, 3, 1);
            }
            else if (Result == "Draw")
            {
                HomeTeam.GamesDrawn++;
                AddStatistic(HomeTeam.TeamID, 2, 1);
                AwayTeam.GamesDrawn++;
                AddStatistic(AwayTeam.TeamID, 2, 1);
            }
            else if (Result == "Away")
            {
                HomeTeam.GamesLost++;
                AddStatistic(HomeTeam.TeamID, 3, 1);
                AwayTeam.GamesWon++;
                AddStatistic(AwayTeam.TeamID, 1, 1);
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
                        string updateQuery = $"UPDATE Teams SET {columnName} = {columnName} + {amount} WHERE TeamID = {teamID};";
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
