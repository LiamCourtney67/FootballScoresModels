namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            League l = new League("Premier League");
            Team douglas = new Team("Douglas", l);
            Team fintry = new Team("Fintry", l);
            Team ferry = new Team("Ferry", l);
            Team lochee = new Team("Lochee", l);
            Match douglasVsFerry = new Match(douglas, ferry, 2, 0);
            Match fintryVsLochee = new Match(fintry, lochee, 1, 1);
            Match douglasVsFintry = new Match(douglas, fintry, 3, 1);
            Match ferryVsLochee = new Match(ferry, lochee, 1, 2);

            foreach (Team team in l.Teams)
            {
                Console.WriteLine($"Team: {team.Name}, Points: {team.Points} GD: {team.GoalDifference}");
            }
        }
    }
}