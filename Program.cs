using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ClearDatabase();
            Console.WriteLine("Database Cleared");

            League dundeeLeague = new League("Dundee League");

            Team douglas = new Team("Douglas", dundeeLeague);
            Team fintry = new Team("Fintry", dundeeLeague);
            Team ferry = new Team("Ferry", dundeeLeague);
            Team lochee = new Team("Lochee", dundeeLeague);

            Player johnSmith = new Player("John", "Smith", 25, 1, 1, douglas);
            Player daveSmith = new Player("Dave", "Smith", 25, 2, 2, douglas);
            Player bobSmith = new Player("Bob", "Smith", 25, 7, 3, douglas);
            Player eoinSmith = new Player("Eoin", "Smith", 25, 9, 4, douglas);

            List<Player> douglasVsFerryScorers = new List<Player> { bobSmith, eoinSmith };
            List<Player> douglasVsFerryAssisters = new List<Player> { daveSmith, bobSmith };

            List<Player> douglasVsFintryScorers = new List<Player> { bobSmith, eoinSmith, eoinSmith };
            List<Player> douglasVsFintryAssisters = new List<Player> { bobSmith, bobSmith, eoinSmith };
            DateTime date = DateTime.Now;

            Match douglasVsFerry = new Match(douglas, ferry, date, 2, 0, douglasVsFerryScorers, douglasVsFerryAssisters);
            Match fintryVsLochee = new Match(fintry, lochee, date, 1, 1);
            Match douglasVsFintry = new Match(douglas, fintry, date, 3, 1, douglasVsFintryScorers, douglasVsFintryAssisters);
            Match ferryVsLochee = new Match(ferry, lochee, date, 1, 2);

            foreach (Team team in dundeeLeague.Teams)
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

        public static void ClearDatabase()
        {
            DatabaseConnection dbConnection = new DatabaseConnection();

            if (dbConnection.OpenConnection())
            {
                try
                {
                    using (MySqlConnection connection = dbConnection.GetConnection())
                    {
                        string deleteQuery = "DELETE FROM Players; DELETE FROM Matches; DELETE FROM Teams; DELETE FROM Leagues;";
                        using (MySqlCommand command = new MySqlCommand(deleteQuery, connection))
                        {
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