namespace ConsoleApp1
{
    internal class Match
    {
        private Team _homeTeam;
        private Team _awayTeam;
        private int _homeGoals;
        private int _awayGoals;
        private string _result;

        public Team HomeTeam { get => _homeTeam; private set => _homeTeam = value; }
        public Team AwayTeam { get => _awayTeam; private set => _awayTeam = value; }
        public int HomeGoals { get => _homeGoals; private set => _homeGoals = value; }
        public int AwayGoals { get => _awayGoals; private set => _awayGoals = value; }
        public string Result { get => _result; private set => _result = value; }

        public Match(Team homeTeam, Team awayTeam, int homeGoals, int awayGoals)
        {
            HomeTeam = homeTeam;
            AwayTeam = awayTeam;
            HomeGoals = homeGoals;
            AwayGoals = awayGoals;

            CalculateResult();
            AssignPoints();
            homeTeam.CalculateStats();
            awayTeam.CalculateStats();
            homeTeam.League.SortTeams();

            CheckCleenSheets();
        }

        public Match(Team homeTeam, Team awayTeam, int homeGoals, int awayGoals, List<Player> scorers)
        {
            HomeTeam = homeTeam;
            AwayTeam = awayTeam;
            HomeGoals = homeGoals;
            AwayGoals = awayGoals;

            CalculateResult();
            AssignPoints();
            homeTeam.CalculateStats();
            awayTeam.CalculateStats();
            homeTeam.League.SortTeams();

            CheckCleenSheets();
            foreach (Player player in scorers)
            {
                AddScorer(player);
            }

        }

        public Match(Team homeTeam, Team awayTeam, int homeGoals, int awayGoals, List<Player> scorers, List<Player> assisters)
        {
            HomeTeam = homeTeam;
            AwayTeam = awayTeam;
            HomeGoals = homeGoals;
            AwayGoals = awayGoals;

            CalculateResult();
            AssignPoints();
            homeTeam.CalculateStats();
            awayTeam.CalculateStats();
            homeTeam.League.SortTeams();

            CheckCleenSheets();
            foreach (Player player in scorers)
            {
                AddScorer(player);
            }
            foreach (Player player in assisters)
            {
                AddAssit(player);
            }
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
                        AddCleanSheet(player);
                    }
                }
            }
            if (AwayGoals == 0)
            {
                foreach (Player player in HomeTeam.Players)
                {
                    if (player.Position == "Goalkeeper" || player.Position == "Defender")
                    {
                        AddCleanSheet(player);
                    }
                }
            }
        }

        public void AddScorer(Player player) => player.ScoreGoal();
        public void AddAssit(Player player) => player.AssistGoal();
        public void AddCleanSheet(Player player) => player.CleanSheet();
        public void AddYellowCard(Player player) => player.YellowCard();
        public void AddRedCard(Player player) => player.RedCard();
    }
}
