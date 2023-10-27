namespace ConsoleApp1
{
    internal class League
    {
        private string _name;
        private List<Team> _teams = new List<Team>();
        private Team _champions;
        private Team _relegated;

        public string Name { get => _name; set => _name = value; }
        public List<Team> Teams { get => _teams; set => _teams = value; }
        public Team Champions { get => _champions; set => _champions = value; }
        public Team Relegated { get => _relegated; set => _relegated = value; }

        public League(string name)
        {
            Name = name;
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
