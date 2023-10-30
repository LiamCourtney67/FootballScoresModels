using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TestPush();
            TestPull();
            Console.WriteLine();

            //TestLeaguePull();
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

        public static void TestLeaguePull()
        {
            LeagueService leagueService = new LeagueService(new DatabaseConnection());

            League league = leagueService.GetLeague(1);

            Console.WriteLine($"League: {league.Name}");

            foreach (Team team in league.Teams)
            {
                Console.WriteLine($"Name: {team.Name}, GamesPlayed: {team.GamesPlayed}, GamesWon: {team.GamesWon}, GamesDrawn: {team.GamesDrawn}, GamesLost: {team.GamesLost}, GoalsFor: {team.GoalsFor}, GoalsAgainst: {team.GoalsAgainst}, GoalDifference: {team.GoalDifference}, Points: {team.Points}");

                foreach (Player player in team.Players)
                {
                    Console.WriteLine($"FirstName: {player.FirstName}, LastName: {player.LastName}, Age: {player.Age}, KitNumber: {player.KitNumber}, Position: {player.Position}, GoalsScored: {player.GoalsScored}, Assists: {player.Assists}, CleanSheets: {player.CleanSheets}, YellowCards: {player.YellowCards}, RedCards: {player.RedCards}");
                }
                foreach (Match match in team.Matches)
                {
                    Console.WriteLine($"Match: {match.HomeTeam.Name} {match.HomeGoals} - {match.AwayGoals} {match.AwayTeam.Name} Result: {match.Result}");
                }
            }
        }

        public static void TestPull()
        {
            LeagueService leagueService = new LeagueService(new DatabaseConnection());

            List<League> leagues = leagueService.GetAllLeagues();
            foreach (League league in leagues)
            {
                Console.WriteLine($"League: {league.Name}");

                foreach (Team team in league.Teams)
                {
                    Console.WriteLine($"Name: {team.Name}, GamesPlayed: {team.GamesPlayed}, GamesWon: {team.GamesWon}, GamesDrawn: {team.GamesDrawn}, GamesLost: {team.GamesLost}, GoalsFor: {team.GoalsFor}, GoalsAgainst: {team.GoalsAgainst}, GoalDifference: {team.GoalDifference}, Points: {team.Points}");

                    foreach (Player player in team.Players)
                    {
                        Console.WriteLine($"FirstName: {player.FirstName}, LastName: {player.LastName}, Age: {player.Age}, KitNumber: {player.KitNumber}, Position: {player.Position}, GoalsScored: {player.GoalsScored}, Assists: {player.Assists}, CleanSheets: {player.CleanSheets}, YellowCards: {player.YellowCards}, RedCards: {player.RedCards}");
                    }

                    foreach (Match match in team.Matches)
                    {
                        Console.WriteLine($"Match: {match.HomeTeam.Name} {match.HomeGoals} - {match.AwayGoals} {match.AwayTeam.Name} Result: {match.Result}");
                    }
                }

                foreach (Match match in league.Matches)
                {
                    Console.WriteLine($"Match: {match.HomeTeam.Name} {match.HomeGoals} - {match.AwayGoals} {match.AwayTeam.Name} Result: {match.Result}");
                }
            }
        }

        public static void TestPush()
        {
            ClearDatabase();
            Console.WriteLine("Database Cleared");
            LeagueService leagueService = new LeagueService(new DatabaseConnection());

            League dundeeLeague = leagueService.CreateLeague("Dundee League");

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

            League angusLeague = leagueService.CreateLeague("Angus League");

            Team arbroath = new Team("Arbroath", angusLeague);
            Team brechin = new Team("Brechin", angusLeague);
            Team carnoustie = new Team("Carnoustie", angusLeague);
            Team forfar = new Team("Forfar", angusLeague);

            Player johnDoe = new Player("John", "Doe", 25, 1, 1, arbroath);
            Player daveDoe = new Player("Dave", "Doe", 25, 2, 2, arbroath);
            Player bobDoe = new Player("Bob", "Doe", 25, 7, 3, arbroath);
            Player eoinDoe = new Player("Eoin", "Doe", 25, 9, 4, arbroath);

            List<Player> arbroathVsBrechinScorers = new List<Player> { bobDoe, eoinDoe };
            List<Player> arbroathVsBrechinAssisters = new List<Player> { daveDoe, bobDoe };

            List<Player> arbroathVsCarnoustieScorers = new List<Player> { bobDoe, eoinDoe, eoinDoe };
            List<Player> arbroathVsCarnoustieAssisters = new List<Player> { bobDoe, bobDoe, eoinDoe };

            Match arbroathVsBrechin = new Match(arbroath, brechin, date, 2, 0, arbroathVsBrechinScorers, arbroathVsBrechinAssisters);
            Match carnoustieVsForfar = new Match(carnoustie, forfar, date, 1, 1);
            Match arbroathVsCarnoustie = new Match(arbroath, carnoustie, date, 3, 1, arbroathVsCarnoustieScorers, arbroathVsCarnoustieAssisters);
            Match brechinVsForfar = new Match(brechin, forfar, date, 1, 2);

            Console.WriteLine("Database Populated");

            //foreach (Team team in dundeeLeague.Teams)
            //{
            //    Console.WriteLine($"Name: {team.Name}, GamesPlayed: {team.GamesPlayed}, GamesWon: {team.GamesWon}, GamesDrawn: {team.GamesDrawn}, GamesLost: {team.GamesLost}, GoalsFor: {team.GoalsFor}, GoalsAgainst: {team.GoalsAgainst}, GoalDifference: {team.GoalDifference}, Points: {team.Points}");
            //}

            //foreach (Player player in douglas.Players)
            //{
            //    if (player.Position == "Goalkeeper" || player.Position == "Defender")
            //        Console.WriteLine($"FirstName: {player.FirstName}, LastName: {player.LastName}, Age: {player.Age}, KitNumber: {player.KitNumber}, Position: {player.Position}, GoalsScored: {player.GoalsScored}, Assists: {player.Assists}, CleanSheets: {player.CleanSheets}, YellowCards: {player.YellowCards}, RedCards: {player.RedCards}");

            //    else
            //        Console.WriteLine($"FirstName: {player.FirstName}, LastName: {player.LastName}, Age: {player.Age}, KitNumber: {player.KitNumber}, Position: {player.Position}, GoalsScored: {player.GoalsScored}, Assists: {player.Assists}, YellowCards: {player.YellowCards}, RedCards: {player.RedCards}");

            //}
        }
    }
}