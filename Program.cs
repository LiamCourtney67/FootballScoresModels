namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            League l = new League("Dundee League");

            Team douglas = new Team("Douglas", l);
            Team fintry = new Team("Fintry", l);
            Team ferry = new Team("Ferry", l);
            Team lochee = new Team("Lochee", l);

            Player johnSmith = new Player("John", "Smith", 25, 1, 1, douglas);
            Player daveSmith = new Player("Dave", "Smith", 25, 2, 2, douglas);
            Player bobSmith = new Player("Bob", "Smith", 25, 7, 3, douglas);
            Player eoinSmith = new Player("Eoin", "Smith", 25, 9, 4, douglas);

            List<Player> douglasVsFerryScorers = new List<Player> { bobSmith, eoinSmith };
            List<Player> douglasVsFerryAssisters = new List<Player> { daveSmith, bobSmith };

            List<Player> douglasVsFintryScorers = new List<Player> { bobSmith, eoinSmith, eoinSmith };
            List<Player> douglasVsFintryAssisters = new List<Player> { bobSmith, bobSmith, eoinSmith };

            Match douglasVsFerry = new Match(douglas, ferry, 2, 0, douglasVsFerryScorers, douglasVsFerryAssisters);
            Match fintryVsLochee = new Match(fintry, lochee, 1, 1);
            Match douglasVsFintry = new Match(douglas, fintry, 3, 1, douglasVsFintryScorers, douglasVsFintryAssisters);
            Match ferryVsLochee = new Match(ferry, lochee, 1, 2);

            foreach (Team team in l.Teams)
            {
                Console.WriteLine($"Team: {team.Name}, Points: {team.Points} GD: {team.GoalDifference}");
            }

            foreach (Player player in douglas.Players)
            {
                if (player.Position == "Goalkeeper" || player.Position == "Defender")
                    Console.WriteLine($"Player: {player.FirstName} {player.LastName}, Goals: {player.GoalsScored}, Assists: {player.Assists}, Clean Sheets: {player.CleanSheets}");

                else
                    Console.WriteLine($"Player: {player.FirstName} {player.LastName}, Goals: {player.GoalsScored}, Assists: {player.Assists}");

            }
        }
    }
}