using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
    internal class MatchDataAccess
    {
        private readonly DatabaseConnection _dbConnection;

        public DatabaseConnection DBConnection { get { return _dbConnection; } }

        public MatchDataAccess(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Checks if a match with the same teams and date played already exists within the database
        /// </summary>
        /// <param name="homeTeam">The home team to be checked</param>
        /// <param name="awayTeam">The away team to be checked</param>
        /// <param name="datePlayed">The date played to be checked</param>
        /// <returns>True if a match already exists within the database or false if it doesn't</returns>
        /// <exception cref="Exception"></exception>
        private bool DoesMatchExist(Team homeTeam, Team awayTeam, DateTime datePlayed)
        {
            try
            {
                // Attempt to open the database connection
                DBConnection.OpenConnection();

                using (MySqlConnection connection = DBConnection.GetConnection())
                {
                    // SQL query to check if a team with the same name exists in the league
                    string checkQuery = "SELECT COUNT(*) FROM Matches WHERE HomeTeamID = @HomeTeamID AND AwayTeamID = @AwayTeamID AND DatePlayed = @DatePlayed;";

                    using (MySqlCommand command = new MySqlCommand(checkQuery, connection))
                    {
                        // Add the teamIDs and date played as a parameter to the SQL query
                        command.Parameters.AddWithValue("@HomeTeamID", homeTeam.TeamID);
                        command.Parameters.AddWithValue("@AwayTeamID", awayTeam.TeamID);
                        command.Parameters.AddWithValue("@DatePlayed", datePlayed.ToString("yyyy-MM-dd"));

                        try
                        {
                            // Execute the SQL query and get the count
                            int count = Convert.ToInt32(command.ExecuteScalar());

                            return count > 0;
                        }
                        catch (MySqlException e)
                        {
                            throw new Exception("Failed to check if the match already exists within the database: " + e.Message + " " + e.InnerException);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to open the database connection: " + e.Message + " " + e.InnerException);
            }
        }

        /// <summary>
        /// Adding a new match to the database
        /// </summary>
        /// <param name="match">The match object to be added to the database</param>
        /// <returns>True if the match has been added to the database or an exception if it couldn't be added</returns>
        /// <exception cref="Exception"></exception>
        public bool AddToDatabase(Match match)
        {
            // Check if a team with the same name already exists in the league
            if (DoesMatchExist(match.HomeTeam, match.AwayTeam, match.DatePlayed))
            {
                throw new Exception("A match with the same teams and date played already within the database.");
            }

            try
            {
                // Attempt to open the database connection
                DBConnection.OpenConnection();

                using (MySqlConnection connection = DBConnection.GetConnection())
                {
                    // SQL query to insert a new match and get its ID
                    string insertQuery = "INSERT INTO Matches (HomeTeamID, AwayTeamID, LeagueID, DatePlayed, HomeGoals, AwayGoals, Result) VALUES (@HomeTeamID, @AwayTeamID, @LeagueID, @DatePlayed, @HomeGoals, @AwayGoals, @Result); SELECT LAST_INSERT_ID();";
                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        // Add the match parameters to the SQL query
                        command.Parameters.AddWithValue("@HomeTeamID", match.HomeTeam.TeamID);
                        command.Parameters.AddWithValue("@AwayTeamID", match.AwayTeam.TeamID);
                        command.Parameters.AddWithValue("@LeagueID", match.League.LeagueID);
                        command.Parameters.AddWithValue("@DatePlayed", match.DatePlayed);
                        command.Parameters.AddWithValue("@HomeGoals", match.HomeGoals);
                        command.Parameters.AddWithValue("@AwayGoals", match.AwayGoals);
                        command.Parameters.AddWithValue("@Result", match.Result);

                        // Execute the SQL query and get the new match's ID
                        match.MatchID = Convert.ToInt32(command.ExecuteScalar());

                        // Return true if the match has been added to the database or throw an exception if it couldn't be added
                        return match.MatchID > 0 ? true : throw new Exception("Failed to add the match to the database.");
                    }
                }
            }
            catch (MySqlException e)
            {
                throw new Exception("Failed to execute the insert query or retrieve the MatchID: " + e.Message + " " + e.InnerException);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to interact with the database: " + e.Message + " " + e.InnerException);
            }
            finally
            {
                // Close the database connection when done
                DBConnection.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves all match records for a league from the database.
        /// </summary>
        /// <param name="league">The league to retrieve the matches for.</param>
        /// <returns>A list of match instances from a league populated with data from the database.</returns>
        public List<Match> GetAllMatchesForLeagueFromDatabase(League league)
        {
            // Initialize a list to store and return the Match objects.
            List<Match> matches = new List<Match>();

            // Attempt to open a connection to the database.
            if (!DBConnection.OpenConnection())
            {
                // Throw an exception if the connection attempt fails.
                throw new Exception("Failed to open the database connection.");
            }

            try
            {
                using (MySqlConnection connection = DBConnection.GetConnection())
                {
                    // Define a SQL query to retrieve all match records for a league.
                    string selectQuery = "SELECT * FROM Matches WHERE LeagueID = @LeagueID;";

                    using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                    {
                        // Add the league id as a parameter to the SQL query.
                        command.Parameters.AddWithValue("@LeagueID", league.LeagueID);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {

                            // Iterate over the result set and populate the Match objects.
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


                                // Validate the match object (i.e., ensure it has a valid ID) and add it to the list.
                                if (match.MatchID > 0)
                                {
                                    match.HomeTeam.AddMatch(match);
                                    match.AwayTeam.AddMatch(match);
                                    matches.Add(match);
                                }
                                else
                                {
                                    throw new Exception("A match retrieved from the database has an invalid ID.");
                                }
                            }
                            reader.Close();
                            return matches;
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                // Handle any MySQL-specific exceptions that might arise during database operations.
                throw new Exception("An error occurred while retrieving matches from the database: " + e.Message + " " + e.InnerException);
            }
            finally
            {
                // Ensure the database connection is closed after the operation.
                DBConnection.CloseConnection();
            }
        }
    }
}
