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
        public int _goalsScored;
        private int _assists;
        private int _cleanSheets;
        private int _yellowCards;
        private int _redCards;

        public int PlayerID { get => _playerID; set => _playerID = value; }
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
        public int GoalsScored { get => _goalsScored; set => _goalsScored = value; }
        public int Assists { get => _assists; set => _assists = value; }
        public int CleanSheets { get => _cleanSheets; set => _cleanSheets = value; }
        public int YellowCards { get => _yellowCards; set => _yellowCards = value; }
        public int RedCards { get => _redCards; set => _redCards = value; }

        public Player(string firstName, string lastName, Team team)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Team = team;
            team.AddPlayer(this);
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

            team.AddPlayer(this);
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
        }
        public void AssistGoal(int amount)
        {
            Assists += amount;
        }
        public void CleanSheet()
        {
            CleanSheets++;
        }
        public void YellowCard(int amount)
        {
            YellowCards += amount;
        }
        public void RedCard()
        {
            RedCards++;
        }
    }
}