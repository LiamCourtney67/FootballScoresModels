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

        private LeagueService _leagueService = new LeagueService(new DatabaseConnection());

        public int LeagueID { get => _leagueID; set => _leagueID = value; }
        public string Name { get => _name; set => _name = value; }
        public List<Team> Teams { get => _teams; set => _teams = value; }
        public List<Match> Matches { get => matches; set => matches = value; }
        public Team? Champions { get => _champions; set => _champions = value; }
        public Team? Relegated { get => _relegated; set => _relegated = value; }

        public LeagueService LeagueService { get => _leagueService; }

        /// <summary>
        /// Creating an instance of the League class
        /// </summary>
        /// <param name="name">League name</param>
        public League(string name)
        {
            this.Name = name;
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
