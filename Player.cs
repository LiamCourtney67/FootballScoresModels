using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
    internal class Player
    {
        private int _playerID;
        private string _firstName;
        private string _lastName;
        private int _age;
        private int _kitNumber;
        private Dictionary<int, string> _positionsContainer = new Dictionary<int, string>
        {
            { 1, "Goalkeeper" },
            { 2, "Defender" },
            { 3, "Midfielder" },
            { 4, "Attacker" }
        };
        private string _position;
        private Team _team;
        private int _goalsScored;
        private int _assists;
        private int _cleanSheets;
        private int _yellowCards;
        private int _redCards;

        public int PlayerID { get => _playerID; private set => _playerID = value; }
        public string FirstName
        {
            get => _firstName;
            set
            {
                // If the name is the same as the value, then do nothing.
                if (!IsValidName(value))
                {
                    throw new Exception("Name is not valid: cannot be null, contain a digit, or punctuation except - or '");
                }
                else
                {
                    _firstName = value.Trim();
                }
            }
        }
        public string LastName
        {
            get => _lastName;
            set
            {
                // If the name is the same as the value, then do nothing.
                if (!IsValidName(value))
                {
                    throw new Exception("Name is not valid: cannot be null, contain a digit, or punctuation except - or '");
                }
                else
                {
                    _lastName = value.Trim();
                }
            }
        }
        public int Age
        {
            get => _age;
            set
            {
                try
                {
                    if (value > 0 || value < 123)
                    {
                        _age = value;
                    }
                    else
                    {
                        throw new Exception("age must be an integer between 1-122.");
                    }
                }
                catch (Exception)
                {
                    throw new Exception("age must be an integer between 1-122.");
                }
            }
        }
        public int KitNumber
        {
            get => _kitNumber;
            set
            {
                try
                {
                    if (value > 0 || value < 100)
                    {
                        _kitNumber = value;
                    }
                    else
                    {
                        throw new Exception("kitNumber must be an integer between 1-99.");
                    }
                }
                catch (Exception)
                {
                    throw new Exception("kitNumber must be an integer between 1-99.");
                }
            }
        }
        public string Position
        {
            get => _position;
            set
            {
                _position = value;
            }
        }
        public Team Team
        {
            get => _team;
            set
            {
                _team = value;
            }
        }
        public int GoalsScored { get => _goalsScored; private set => _goalsScored = value; }
        public int Assists { get => _assists; private set => _assists = value; }
        public int CleanSheets { get => _cleanSheets; private set => _cleanSheets = value; }
        public int YellowCards { get => _yellowCards; private set => _yellowCards = value; }
        public int RedCards { get => _redCards; private set => _redCards = value; }

        public Player(string firstName, string lastName, Team team)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Team = team;
            if (AddToDatabase(new DatabaseConnection()))
            {
                team.AddPlayer(this);
            }
            else { throw new Exception("Failed to add player to the database."); }
        }

        public Player(string firstName, string lastName, int age, int kitNumber, int positionKey, Team team)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
            this.KitNumber = kitNumber;
            this.Team = team;

            if (_positionsContainer.TryGetValue(positionKey, out string position))
            {
                this.Position = position;
            }
            else
            {
                throw new ArgumentException("Invalid positionKey, must be 1-4");
            }
            if (AddToDatabase(new DatabaseConnection()))
            {
                team.AddPlayer(this);
            }
            else { throw new Exception("Failed to add player to the database."); }
        }

        public Player(int playerID, string firstName, string lastName, int age, int kitNumber, string position, Team team, int goalsScored, int assists, int cleanSheets, int yellowCards, int redCards)
        {
            this.PlayerID = playerID;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
            this.KitNumber = kitNumber;
            this.Position = position;
            this.Team = team;
            this.GoalsScored = goalsScored;
            this.Assists = assists;
            this.CleanSheets = cleanSheets;
            this.YellowCards = yellowCards;
            this.RedCards = redCards;
        }

        private bool DoesKitNumberExistsInTeam(DatabaseConnection dbConnection)
        {
            if (dbConnection.OpenConnection())
            {
                try
                {
                    using (MySqlConnection connection = dbConnection.GetConnection())
                    {
                        string checkQuery = "SELECT COUNT(*) FROM Players WHERE PlayerKitNumber = @KitNumber AND TeamID = @TeamID;";
                        using (MySqlCommand command = new MySqlCommand(checkQuery, connection))
                        {
                            command.Parameters.AddWithValue("@KitNumber", KitNumber);
                            command.Parameters.AddWithValue("@TeamID", Team.TeamID);

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
            if (!DoesKitNumberExistsInTeam(dbConnection))
            {
                if (dbConnection.OpenConnection())
                {
                    try
                    {
                        using (MySqlConnection connection = dbConnection.GetConnection())
                        {
                            string insertQuery = "INSERT INTO Players (PlayerFirstName, PlayerLastName, PlayerAge, PlayerKitNumber, Position, TeamID, GoalsScored, Assists, CleanSheets, YellowCards, RedCards) " +
                                "VALUES (@PlayerFirstName, @PlayerSurname, @PlayerAge, @PlayerKitNumber, @Position, @TeamID, @GoalsScored, @Assists, @CleanSheets, @YellowCards, @RedCards); SELECT LAST_INSERT_ID();";
                            using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                            {
                                command.Parameters.AddWithValue("@PlayerFirstName", FirstName);
                                command.Parameters.AddWithValue("@PlayerSurname", LastName);
                                command.Parameters.AddWithValue("@PlayerAge", Age);
                                command.Parameters.AddWithValue("@PlayerKitNumber", KitNumber);
                                command.Parameters.AddWithValue("@Position", Position);
                                command.Parameters.AddWithValue("@TeamID", Team.TeamID);
                                command.Parameters.AddWithValue("@GoalsScored", GoalsScored);
                                command.Parameters.AddWithValue("@Assists", Assists);
                                command.Parameters.AddWithValue("@CleanSheets", CleanSheets);
                                command.Parameters.AddWithValue("@YellowCards", YellowCards);
                                command.Parameters.AddWithValue("@RedCards", RedCards);

                                PlayerID = Convert.ToInt32(command.ExecuteScalar());

                                if (PlayerID > 0)
                                {
                                    Console.WriteLine("New player added to the database with ID: " + PlayerID);
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
                Console.WriteLine("A player with the same kit number already exists in the same team.");
                return false;
            }
        }

        public static List<Player> GetAllPlayersForTeamFromDatabase(Team team, DatabaseConnection dbConnection)
        {
            List<Player> players = new List<Player>();
            if (dbConnection.OpenConnection())
            {
                try
                {
                    using (MySqlConnection connection = dbConnection.GetConnection())
                    {
                        string selectQuery = $"SELECT * FROM Players WHERE TeamID = {team.TeamID};";
                        using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Player player = new Player(
                                        Convert.ToInt32(reader["PlayerID"]),
                                        reader["PlayerFirstName"].ToString(),
                                        reader["PlayerLastName"].ToString(),
                                        Convert.ToInt32(reader["PlayerAge"]),
                                        Convert.ToInt32(reader["PlayerKitNumber"]),
                                        reader["Position"].ToString(),
                                        team,
                                        Convert.ToInt32(reader["GoalsScored"]),
                                        Convert.ToInt32(reader["Assists"]),
                                        Convert.ToInt32(reader["CleanSheets"]),
                                        Convert.ToInt32(reader["YellowCards"]),
                                        Convert.ToInt32(reader["RedCards"])
                                    );
                                    players.Add(player);

                                }
                                reader.Close();
                                return players;
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
            throw new Exception("Failed to get player from the database.");
        }

        private bool IsValidName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length == 0)
            {
                return false;
            }
            else if (name.Any(char.IsDigit))
            {
                return false;
            }
            else if (name.Any(c => (char.IsPunctuation(c) || char.IsSymbol(c)) && c != '-' && c != '\''))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void ScoreGoal(int amount)
        {
            GoalsScored += amount;
            AddStatistic(0, amount);
        }
        public void AssistGoal(int amount)
        {
            Assists += amount;
            AddStatistic(1, amount);
        }
        public void CleanSheet(int amount)
        {
            CleanSheets += amount;
            AddStatistic(2, amount);
        }
        public void YellowCard(int amount)
        {
            YellowCards += amount;
            AddStatistic(3, amount);
        }
        public void RedCard(int amount)
        {
            RedCards += amount;
            AddStatistic(4, amount);
        }

        public void AddStatistic(int index, int amount)
        {
            string[] stats = { "GoalsScored", "Assists", "CleanSheets", "YellowCards", "RedCards" };

            DatabaseConnection dbConnection = new DatabaseConnection();

            if (dbConnection.OpenConnection())
            {
                try
                {
                    using (MySqlConnection connection = dbConnection.GetConnection())
                    {
                        string columnName = stats[index];
                        string updateQuery = $"UPDATE Players SET {columnName} = {columnName} + {amount} WHERE PlayerID = @PlayerID;";
                        using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@PlayerID", PlayerID);
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