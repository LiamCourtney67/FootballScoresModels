using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DatabaseConnection dbConnection = new DatabaseConnection();

            List<League> leagues = League.GetAllLeaguesFromDatabase(dbConnection);
            foreach (League league in leagues)
            {
                Console.WriteLine($"League: {league.Name}");
            }
            foreach (League league in leagues)
            {
                foreach (Team team in league.Teams)
                {
                    Console.WriteLine($"Team: {team.Name}, Points: {team.Points} GD: {team.GoalDifference}");
                }
            }
            foreach (League league in leagues)
            {
                foreach (Team team in league.Teams)
                {
                    foreach (Player player in team.Players)
                    {
                        Console.WriteLine($"Player: {player.FirstName} {player.LastName}, Goals: {player.GoalsScored}, Assists: {player.Assists}");
                    }
                }
            }

            List<Match> matches = Match.GetAllMatchesFromDatabase(dbConnection);
            foreach (Match match in matches)
            {
                Console.WriteLine($"Match: {match.HomeTeam.Name} {match.HomeGoals} - {match.AwayGoals} {match.AwayTeam.Name}");
            }

            Team douglas = Team.GetTeamFromDatabase(5, dbConnection);
            Team fintry = Team.GetTeamFromDatabase(6, dbConnection);
            Team ferry = Team.GetTeamFromDatabase(7, dbConnection);
            Team lochee = Team.GetTeamFromDatabase(8, dbConnection);

            Console.WriteLine($"Team: {douglas.Name}, Points: {douglas.Points} GD: {douglas.GoalDifference}");
            Console.WriteLine($"Team: {fintry.Name}, Points: {fintry.Points} GD: {fintry.GoalDifference}");
            Console.WriteLine($"Team: {ferry.Name}, Points: {ferry.Points} GD: {ferry.GoalDifference}");
            Console.WriteLine($"Team: {lochee.Name}, Points: {lochee.Points} GD: {lochee.GoalDifference}");

            //League dundeeLeague = League.GetLeagueFromDatabase(15, dbConnection);
            //Team douglas = Team.GetTeamFromDatabase(49, dbConnection);
            //Team fintry = Team.GetTeamFromDatabase(50, dbConnection);
            //Team ferry = Team.GetTeamFromDatabase(51, dbConnection);
            //Team lochee = Team.GetTeamFromDatabase(52, dbConnection);

            //foreach (Team team in dundeeLeague.Teams)
            //{
            //    Console.WriteLine($"Team: {team.Name}, Points: {team.Points} GD: {team.GoalDifference}");
            //}

            //Player johnSmith = Player.GetPlayerFromDatabase(41, dbConnection);
            //Player daveSmith = Player.GetPlayerFromDatabase(42, dbConnection);
            //Player bobSmith = Player.GetPlayerFromDatabase(43, dbConnection);
            //Player eoinSmith = Player.GetPlayerFromDatabase(44, dbConnection);

            //Match douglasVsFerry = Match.GetMatchFromDatabase(32, dbConnection);
            //Match fintryVsLochee = Match.GetMatchFromDatabase(33, dbConnection);
            //Match douglasVsFintry = Match.GetMatchFromDatabase(34, dbConnection);
            //Match ferryVsLochee = Match.GetMatchFromDatabase(35, dbConnection);




            //ClearDatabase();
            //Console.WriteLine("Database Cleared");

            //League dundeeLeague = new League("Dundee League");

            //Team douglas = new Team("Douglas", dundeeLeague);
            //Team fintry = new Team("Fintry", dundeeLeague);
            //Team ferry = new Team("Ferry", dundeeLeague);
            //Team lochee = new Team("Lochee", dundeeLeague);

            //Player johnSmith = new Player("John", "Smith", 25, 1, 1, douglas);
            //Player daveSmith = new Player("Dave", "Smith", 25, 2, 2, douglas);
            //Player bobSmith = new Player("Bob", "Smith", 25, 7, 3, douglas);
            //Player eoinSmith = new Player("Eoin", "Smith", 25, 9, 4, douglas);

            //List<Player> douglasVsFerryScorers = new List<Player> { bobSmith, eoinSmith };
            //List<Player> douglasVsFerryAssisters = new List<Player> { daveSmith, bobSmith };

            //List<Player> douglasVsFintryScorers = new List<Player> { bobSmith, eoinSmith, eoinSmith };
            //List<Player> douglasVsFintryAssisters = new List<Player> { bobSmith, bobSmith, eoinSmith };
            //DateTime date = DateTime.Now;

            //Match douglasVsFerry = new Match(douglas, ferry, date, 2, 0, douglasVsFerryScorers, douglasVsFerryAssisters);
            //Match fintryVsLochee = new Match(fintry, lochee, date, 1, 1);
            //Match douglasVsFintry = new Match(douglas, fintry, date, 3, 1, douglasVsFintryScorers, douglasVsFintryAssisters);
            //Match ferryVsLochee = new Match(ferry, lochee, date, 1, 2);

            //foreach (Team team in dundeeLeague.Teams)
            //{
            //    Console.WriteLine($"Team: {team.Name}, Points: {team.Points} GD: {team.GoalDifference}");
            //}

            //foreach (Player player in douglas.Players)
            //{
            //    if (player.Position == "Goalkeeper" || player.Position == "Defender")
            //        Console.WriteLine($"Player: {player.FirstName} {player.LastName}, Goals: {player.GoalsScored}, Assists: {player.Assists}, Clean Sheets: {player.CleanSheets}");

            //    else
            //        Console.WriteLine($"Player: {player.FirstName} {player.LastName}, Goals: {player.GoalsScored}, Assists: {player.Assists}");

            //}
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