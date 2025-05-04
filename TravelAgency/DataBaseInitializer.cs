namespace TravelAgency;
using System;
using System.IO;
using Microsoft.Data.SqlClient;



public static class DatabaseInitializer
{
    public static void Run(string connectionString, string scriptPath)
    {
        if (!File.Exists(scriptPath))
        {
            Console.WriteLine("SQL script not found at: " + scriptPath);
            return;
        }

        string script = File.ReadAllText(scriptPath);

        using var connection = new SqlConnection(connectionString);
        connection.Open();

        foreach (var commandText in script.Split("GO", StringSplitOptions.RemoveEmptyEntries))
        {
            if (string.IsNullOrWhiteSpace(commandText)) continue;

            using var command = new SqlCommand(commandText, connection);
            command.ExecuteNonQuery();
        }

        Console.WriteLine("Database initialized successfully.");
    }
}
