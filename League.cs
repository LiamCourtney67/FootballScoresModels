using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
    internal class League
    {
        private int _leagueID;
        private string _name;
        private List<Team> _teams = new List<Team>();
        private Team _champions;
        private Team _relegated;

        public int LeagueID { get => _leagueID; private set => _leagueID = value; }
        public string Name { get => _name; set => _name = value; }
        public List<Team> Teams { get => _teams; set => _teams = value; }
        public Team Champions { get => _champions; set => _champions = value; }
        public Team Relegated { get => _relegated; set => _relegated = value; }

        public League(string name)
        {
            Name = name;
            if (!AddToDatabase(new DatabaseConnection())) { throw new Exception("Failed to add league to the database."); }
        }

        private bool DoesLeagueNameExists(DatabaseConnection dbConnection)
        {
            if (dbConnection.OpenConnection())
            {
                try
                {
                    using (MySqlConnection connection = dbConnection.GetConnection())
                    {
                        string checkQuery = "SELECT COUNT(*) FROM Leagues WHERE LeagueName = @LeagueName;";
                        using (MySqlCommand cmd = new MySqlCommand(checkQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@LeagueName", Name);

                            // Execute the query to check if the league name exists
                            int count = Convert.ToInt32(cmd.ExecuteScalar());

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

                                LeagueID = Convert.ToInt32(command.ExecuteScalar());

                                if (LeagueID > 0)
                                {
                                    Console.WriteLine("New league added to the database with ID: " + LeagueID);
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
                Console.WriteLine("A league with the same name already exists.");
                return false;
            }
        }

        public void AddTeam(Team team)
        {
            Teams.Add(team);
            team.League = this;
        }

        public void RemoveTeam(Team team)
        {
            Teams.Remove(team);
            team.League = null;
        }

        public void SortTeams()
        {
            Teams = Teams
                .OrderByDescending(team => team.Points)
                .ThenByDescending(team => team.GoalDifference)
                .ThenByDescending(team => team.GoalsFor)
                .ToList();
        }
    }
}
