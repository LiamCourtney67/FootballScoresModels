using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
    internal class League
    {
        private int _leagueID;
        private string _name = "";
        private List<Team> _teams = new List<Team>();
        private List<Match> matches = new List<Match>();
        private Team? _champions = null;
        private Team? _relegated = null;

        public int LeagueID { get => _leagueID; private set => _leagueID = value; }
        public string Name { get => _name; set => _name = value; }
        public List<Team> Teams { get => _teams; set => _teams = value; }
        public List<Match> Matches { get => matches; set => matches = value; }
        public Team? Champions { get => _champions; set => _champions = value; }
        public Team? Relegated { get => _relegated; set => _relegated = value; }

        /// <summary>
        /// Creating an instance of the League class with a new league to be added to the database
        /// </summary>
        /// <param name="name">League name</param>
        /// <exception cref="Exception">Failed to add league to the database</exception> -------------- Ask what to put here
        public League(string name)
        {
            this.Name = name;
            try { AddToDatabase(new DatabaseConnection()); }
            catch (Exception e) { throw new Exception("Failed to add league to the database: ", e); }
        }

        /// <summary>
        /// Creating an instance of the League class with an existing league in the database
        /// </summary>
        /// <param name="leagueID">LeagueID</param>
        /// <param name="name">League name</param>
        public League(int leagueID, string name)
        {
            this.LeagueID = leagueID;
            this.Name = name;
            this.Teams = Team.GetAllTeamsForLeagueFromDatabase(this, new DatabaseConnection());
            this.Matches = Match.GetAllMatchesForLeagueFromDatabase(this, new DatabaseConnection());
        }

        /// <summary>
        /// Checks if a league with the same name already exists in the database
        /// </summary>
        /// <param name="dbConnection">Instance of DatabaseConnection</param>
        /// <returns>True if a league with the same name exists, otherwise false</returns>
        private bool DoesLeagueNameExists(DatabaseConnection dbConnection)
        {
            if (dbConnection.OpenConnection())
            {
                try
                {
                    using (MySqlConnection connection = dbConnection.GetConnection())
                    {
                        string checkQuery = "SELECT COUNT(*) FROM Leagues WHERE LeagueName = @LeagueName;";
                        using (MySqlCommand command = new MySqlCommand(checkQuery, connection))
                        {
                            command.Parameters.AddWithValue("@LeagueName", Name);

                            int count = Convert.ToInt32(command.ExecuteScalar());

                            return count > 0;
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    throw new Exception("Failed to check if the league name already exists in the database.", ex);
                }
                finally
                {
                    dbConnection.CloseConnection();
                }
            }
            else
            {
                throw new Exception("Failed to open the database connection.");
            }
        }

        /// <summary>
        /// Adding an instance of the League class to the database
        /// </summary>
        /// <param name="dbConnection">Instance of DatabaseConnection</param>
        /// <returns>True if the League could be added, otherwise false</returns>
        private bool AddToDatabase(DatabaseConnection dbConnection)
        {
            if (!DoesLeagueNameExists(dbConnection))
            {
                if (dbConnection.OpenConnection())
                {
                    try
                    {
                        using (MySqlConnection connection = dbConnection.GetConnection())
                        {
                            string insertQuery = "INSERT INTO Leagues (LeagueName) VALUES (@LeagueName); SELECT LAST_INSERT_ID();";
                            using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                            {
                                command.Parameters.AddWithValue("@LeagueName", Name);

                                try
                                {
                                    LeagueID = Convert.ToInt32(command.ExecuteScalar());

                                    if (LeagueID > 0)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        throw new Exception("Failed to add the league to the database.");
                                    }
                                }
                                catch (MySqlException ex)
                                {
                                    throw new Exception("Failed to execute the insert query or retrieve the LeagueID.", ex);
                                }
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception("Failed to interact with the database.", ex);
                    }
                    finally
                    {
                        dbConnection.CloseConnection();
                    }
                }
                else
                {
                    throw new Exception("Failed to open the database connection.");
                }
            }
            else
            {
                throw new Exception("A league with the same name already exists.");
            }
        }



        /// <summary>
        /// Get an individual league from the database
        /// </summary>
        /// <param name="leagueID">LeagueID</param>
        /// <param name="dbConnection">Instance of DatabaseConnection</param>
        /// <returns>An instance of the League class based on the database, or null if not found</returns>
        public static League GetLeagueFromDatabase(int leagueID, DatabaseConnection dbConnection)
        {
            // Check if the database connection can be opened
            if (dbConnection.OpenConnection())
            {
                try
                {
                    // Use the existing database connection
                    using (MySqlConnection connection = dbConnection.GetConnection())
                    {
                        // SQL query to select a league by ID
                        string selectQuery = $"SELECT * FROM Leagues WHERE LeagueID = {leagueID};";

                        // Create a MySQL command
                        using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                        {
                            // Execute the query and retrieve the result set
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Create a League instance based on the database
                                    League league = new League(
                                        Convert.ToInt32(reader["LeagueID"]),
                                        reader["LeagueName"].ToString()
                                    );
                                    reader.Close();

                                    if (league.LeagueID > 0)
                                    {
                                        // Return the league if it has a valid ID
                                        return league;
                                    }
                                    else
                                    {
                                        // Throw an exception if the league ID is not valid
                                        throw new Exception("Failed to get the league from the database.");
                                    }
                                }
                                reader.Close();
                                return null;
                            }
                        }
                    }
                }
                finally
                {
                    // Close the database connection when done
                    dbConnection.CloseConnection();
                }
            }
            else
            {
                // Throw an exception if opening the database connection failed
                throw new Exception("Failed to open the database connection.");
            }
        }

        /// <summary>
        /// Gets all leagues from the database
        /// </summary>
        /// <param name="dbConnection">Instance of DatabaseConnection</param>
        /// <returns>A list of instances of the League class based on the database</returns>
        public static List<League> GetAllLeaguesFromDatabase(DatabaseConnection dbConnection)
        {
            // Create a list to hold the League instances
            List<League> leagues = new List<League>();

            // Check if the database connection can be opened
            if (dbConnection.OpenConnection())
            {
                try
                {
                    // Use the existing database connection
                    using (MySqlConnection connection = dbConnection.GetConnection())
                    {
                        // SQL query to select all leagues
                        string selectQuery = "SELECT * FROM Leagues;";

                        // Create a MySQL command
                        using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                        {
                            // Execute the query and retrieve the result set
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Create a League instance based on each database record
                                    League league = new League(
                                        Convert.ToInt32(reader["LeagueID"]),
                                        reader["LeagueName"].ToString()
                                    );
                                    reader.Close();

                                    if (league.LeagueID > 0)
                                    {
                                        // Add the league to the list if it has a valid ID
                                        leagues.Add(league);
                                    }
                                    else
                                    {
                                        // Throw an exception if the league ID is not valid
                                        throw new Exception("Failed to get the league from the database.");
                                    }
                                }
                                reader.Close();
                                return leagues;
                            }
                        }
                    }
                }
                finally
                {
                    // Close the database connection when done
                    dbConnection.CloseConnection();
                }
            }
            else
            {
                // Throw an exception if opening the database connection failed
                throw new Exception("Failed to open the database connection.");
            }
        }

        // Adding, removing, and sorting teams, matches, champions, and relegated teams

        /// <summary>
        /// Adds a team to the league list of teams
        /// </summary>
        /// <param name="team">Team to be added</param>
        public void AddTeam(Team team) { Teams.Add(team); team.League = this; }

        /// <summary>
        /// Removes a team from the league list of teams
        /// </summary>
        /// <param name="team">Team to be removed</param>
        public void RemoveTeam(Team team) { Teams.Remove(team); team.League = null; }

        /// <summary>
        /// Adds a match to the league list of matches
        /// </summary>
        /// <param name="team">Match to be added</param>
        public void AddMatch(Match match) { Matches.Add(match); match.League = this; }

        /// <summary>
        /// Removes a match from the league list of matches
        /// </summary>
        /// <param name="team">Match to be removed</param>
        public void RemoveMatch(Match match) { Matches.Remove(match); match.League = null; }

        /// <summary>
        /// Sorts the list of teams in the league by points, goal difference and goals for
        /// </summary>
        public void SortTeams()
        {
            Teams = Teams
                .OrderByDescending(team => team.Points)
                .ThenByDescending(team => team.GoalDifference)
                .ThenByDescending(team => team.GoalsFor)
                .ToList();
        }

        /// <summary>
        /// Sorts the list of matches in the league by date played
        /// </summary>
        public void SortMatches()
        {
            Matches = Matches
                .OrderByDescending(match => match.DatePlayed)
                .ToList();
        }

        /// <summary>
        /// Sets the champions to the first team in the sorted list of teams
        /// </summary>
        public void SetChampions() { Champions = Teams[0]; }

        /// <summary>
        /// Sets the relegated team to the last team in the sorted list of teams
        /// </summary>
        public void SetRelegated() { Relegated = Teams[Teams.Count - 1]; }
    }
}
