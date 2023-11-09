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

        private PlayerService _playerService = new PlayerService(new DatabaseConnection());
        private TeamService _teamService = new TeamService(new DatabaseConnection());

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
            _teamService.AddStatisticToDatabase(HomeTeam, "GamesPlayed");

            HomeTeam.GoalsFor += HomeGoals;
            _teamService.AddStatisticToDatabase(HomeTeam, "GoalsFor");

            HomeTeam.GoalsAgainst += AwayGoals;
            _teamService.AddStatisticToDatabase(HomeTeam, "GoalsAgainst");

            AwayTeam.GamesPlayed++;
            _teamService.AddStatisticToDatabase(AwayTeam, "GamesPlayed");

            AwayTeam.GoalsFor += AwayGoals;
            _teamService.AddStatisticToDatabase(AwayTeam, "GoalsFor");

            AwayTeam.GoalsAgainst += HomeGoals;
            _teamService.AddStatisticToDatabase(AwayTeam, "GoalsAgainst");

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
            _teamService.AddStatisticToDatabase(HomeTeam, "Points");

            AwayTeam.Points = (AwayTeam.GamesWon * 3) + AwayTeam.GamesDrawn;
            _teamService.AddStatisticToDatabase(AwayTeam, "Points");

            HomeTeam.GoalDifference = HomeTeam.GoalsFor - HomeTeam.GoalsAgainst;
            _teamService.AddStatisticToDatabase(HomeTeam, "GoalDifference");

            AwayTeam.GoalDifference = AwayTeam.GoalsFor - AwayTeam.GoalsAgainst;
            _teamService.AddStatisticToDatabase(AwayTeam, "GoalDifference");
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

        public void AddScorer(Player player, int amount) { player.ScoreGoal(amount); _playerService.AddStatisticToDatabase(player, "GoalsScored"); }
        public void AddAssit(Player player, int amount) { player.AssistGoal(amount); _playerService.AddStatisticToDatabase(player, "Assists"); }
        public void AddCleanSheet(Player player, int amount) { player.CleanSheet(amount); _playerService.AddStatisticToDatabase(player, "CleanSheets"); }
        public void AddYellowCard(Player player, int amount) { player.YellowCard(amount); _playerService.AddStatisticToDatabase(player, "YellowCards"); }
        public void AddRedCard(Player player, int amount) { player.RedCard(amount); _playerService.AddStatisticToDatabase(player, "RedCards"); }
    }
}
