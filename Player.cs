namespace ConsoleApp1
{
    internal class Player
    {
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
        public Team PlayerTeam
        {
            get => _team;
            set
            {
                _team = value;
            }
        }
        public int GoalsScored { get => _goalsScored; }
        public int Assists { get => _assists; }
        public int CleanSheets { get => _cleanSheets; }
        public int YellowCards { get => _yellowCards; }
        public int RedCards { get => _redCards; }

        public Player(string firstName, string lastName, int age, int kitNumber, int positionKey, Team team)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            KitNumber = kitNumber;
            if (_positionsContainer.TryGetValue(positionKey, out string position))
            {
                Position = position;
            }
            else
            {
                throw new ArgumentException("Invalid positionKey, must be 1-4");
            }
            PlayerTeam = team;
            team.AddPlayer(this);
        }

        public Player(string firstName, string lastName, Team team)
        {
            FirstName = firstName;
            LastName = lastName;
            PlayerTeam = team;
            team.AddPlayer(this);
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

        public void ScoreGoal() => _goalsScored++;
        public void AssistGoal() => _assists++;
        public void CleanSheet() => _cleanSheets++;
        public void YellowCard() => _yellowCards++;
        public void RedCard() => _redCards++;
    }
}
