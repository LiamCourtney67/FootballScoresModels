using MySql.Data.MySqlClient;
using System.Data;

namespace ConsoleApp1;

public class DatabaseConnection
{
    private MySqlConnection connection;
    private string connectionString = "Server=127.0.0.1;Port=3306;Database=Leagues;Uid=Application;Pwd=Jimi;";

    public DatabaseConnection()
    {
        connection = new MySqlConnection(connectionString);
    }

    public bool OpenConnection()
    {
        try
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            return true;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    public bool CloseConnection()
    {
        try
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            return true;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    public MySqlConnection GetConnection()
    {
        return connection;
    }
}
