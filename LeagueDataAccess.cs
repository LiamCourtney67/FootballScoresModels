﻿using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
    internal class LeagueDataAccess
    {
        private readonly DatabaseConnection _dbConnection;

        public DatabaseConnection DBConnection { get { return _dbConnection; } }

        public LeagueDataAccess(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Checks if a league with the same name already exists in the database
        /// </summary>
        /// <param name="leagueName">The league name to be checked</param>
        /// <returns>True if a team already exists with that name in the database or false if it doesn't</returns>
        /// <exception cref="Exception"></exception>
        private bool DoesLeagueNameExist(string leagueName)
        {
            try
            {
                // Attempt to open the database connection
                DBConnection.OpenConnection();

                using (MySqlConnection connection = DBConnection.GetConnection())
                {
                    // SQL query to check if a league with the same name exists
                    string checkQuery = "SELECT COUNT(*) FROM Leagues WHERE LeagueName = @LeagueName;";

                    using (MySqlCommand command = new MySqlCommand(checkQuery, connection))
                    {
                        // Add the league name as a parameter to the SQL query
                        command.Parameters.AddWithValue("@LeagueName", leagueName);

                        try
                        {
                            // Execute the SQL query and get the count
                            int count = Convert.ToInt32(command.ExecuteScalar());

                            return count > 0;
                        }
                        catch (MySqlException e)
                        {
                            throw new Exception("Failed to check if the league name already exists in the database: " + e.Message + " " + e.InnerException);
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
        /// Adding a new league to the database
        /// </summary>
        /// <param name="league">The league object to be added</param>
        /// <returns>True if the league has been added to the database or an exception if it couldn't be added</returns>
        /// <exception cref="Exception"></exception>
        public bool AddToDatabase(League league)
        {
            // Check if a league with the same name already exists
            if (DoesLeagueNameExist(league.Name))
            {
                throw new Exception("A league with the same name already exists.");
            }

            try
            {
                // Attempt to open the database connection
                DBConnection.OpenConnection();

                using (MySqlConnection connection = DBConnection.GetConnection())
                {
                    // SQL query to insert a new league and get its ID
                    string insertQuery = "INSERT INTO Leagues (LeagueName) VALUES (@LeagueName); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        // Add the league name as a parameter to the SQL query
                        command.Parameters.AddWithValue("@LeagueName", league.Name);

                        // Execute the SQL query and get the new league's ID
                        league.LeagueID = Convert.ToInt32(command.ExecuteScalar());

                        // Return true if the league has been added to the database or throw an exception if it couldn't be added
                        return league.LeagueID > 0 ? true : throw new Exception("Failed to add the league to the database.");
                    }
                }
            }
            catch (MySqlException e)
            {
                throw new Exception("Failed to execute the insert query or retrieve the LeagueID: " + e.Message + " " + e.InnerException);
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
        /// Get an individual league from the database.
        /// </summary>
        /// <param name="leagueID">LeagueID.</param>
        /// <returns>An instance of the League class based on the database, or null if not found.</returns>
        public League GetLeagueFromDatabase(int leagueID)
        {
            // Attempt to open the database connection.
            if (!DBConnection.OpenConnection())
            {
                throw new Exception("Failed to open the database connection.");
            }

            try
            {
                using (MySqlConnection connection = DBConnection.GetConnection())
                {
                    // SQL query to get the league with the same leagueID
                    string selectQuery = "SELECT * FROM Leagues WHERE LeagueID = @LeagueID";

                    using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                    {
                        // Add the leagueID as a parameter to the SQL query
                        command.Parameters.AddWithValue("@LeagueID", leagueID);

                        // Execute the query and retrieve the result set.
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Create a League instance based on the database results.
                                League league = new League(
                                    Convert.ToInt32(reader["LeagueID"]),
                                    reader["LeagueName"].ToString()
                                );

                                // Validate and return the league object.
                                return league.LeagueID > 0 ? league : throw new Exception("Failed to get the league from the database.");
                            }
                            return null; // No league found with the given ID.
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                throw new Exception("An error occurred while retrieving data from the database: " + e.Message + " " + e.InnerException);
            }
            finally
            {
                // Close the database connection when done
                DBConnection.CloseConnection();
            }
        }


        /// <summary>
        /// Retrieves all league records from the database.
        /// </summary>
        /// <returns>A list of League instances populated with data from the database.</returns>
        public List<League> GetAllLeaguesFromDatabase()
        {
            // Initialize a list to store and return the League objects.
            List<League> leagues = new List<League>();

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
                    // Define a SQL query to retrieve all league records.
                    string selectQuery = "SELECT * FROM Leagues;";

                    using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            // Iterate over the result set and populate the League objects.
                            while (reader.Read())
                            {
                                League league = new League(
                                    Convert.ToInt32(reader["LeagueID"]),
                                    reader["LeagueName"].ToString()
                                );


                                // Validate the league object (i.e., ensure it has a valid ID) and add it to the list.
                                if (league.LeagueID > 0)
                                {
                                    leagues.Add(league);
                                }
                                else
                                {
                                    throw new Exception("A league retrieved from the database has an invalid ID.");
                                }
                            }
                            reader.Close();
                            return leagues;
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                // Handle any MySQL-specific exceptions that might arise during database operations.
                throw new Exception("An error occurred while retrieving leagues from the database: " + e.Message + " " + e.InnerException);
            }
            finally
            {
                // Ensure the database connection is closed after the operation.
                DBConnection.CloseConnection();
            }
        }
    }
}
