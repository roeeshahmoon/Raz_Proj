using Raz_Project_App.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.IO;
using Raz_Project_App.Components.Pages;


namespace Raz_Project_App.Services
{
    public class UserService
    {
        private readonly string _connectionString;

        public UserService(string connectionString)
        {
            _connectionString = connectionString;
            EnsureDatabase();
        }

        private void EnsureDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            string createTableSQL = @"CREATE TABLE IF NOT EXISTS Users (
										Id INTEGER PRIMARY KEY AUTOINCREMENT,
										UserName TEXT NOT NULL,
										Email TEXT NOT NULL,
										Password TEXT NOT NULL,
                                        IsAdminUser INTEGER NOT NULL DEFAULT 0
										);";
            using var command = new SqliteCommand(createTableSQL, connection);
            command.ExecuteNonQuery();
        }

        public List<User> GetUsers()
        {
            List<User> users = new List<User>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string selectSQL = "SELECT Id, UserName, Email, Password, IsAdminUser FROM Users";
            using var command = new SqliteCommand(selectSQL, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string username = reader.GetString(1);
                string email = reader.GetString(2);
                string password = reader.GetString(3);
                bool isadminuser = reader.GetInt32(4) == 1;
                ;

                users.Add(new User { Id = id, UserName = username, Email = email, Password = password, IsAdminUser = isadminuser });
            }
            return users;

        }
        public void UpdateUser(User updatedUser)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string updateSQL = "UPDATE Users SET UserName = @username, Email = @email, Password = @password, IsAdminUser = @isadminuser WHERE Id = @id;";
            using var command = new SqliteCommand(updateSQL, connection);

            command.Parameters.AddWithValue("@username", updatedUser.UserName);
            command.Parameters.AddWithValue("@email", updatedUser.Email);
            command.Parameters.AddWithValue("@password", updatedUser.Password);
            command.Parameters.AddWithValue("@IsAdminUser", updatedUser.IsAdminUser ? 1 : 0);

            command.ExecuteNonQuery();
        }

        public void AddUser(User newUser)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string insertSQL = "INSERT INTO Users (UserName, Email, Password, IsAdminUser) Values (@username, @email, @password, @isadminuser );";

            using var command = new SqliteCommand(insertSQL, connection);

            command.Parameters.AddWithValue("@username", newUser.UserName);
            command.Parameters.AddWithValue("@email", newUser.Email);
            command.Parameters.AddWithValue("@password", newUser.Password);
            command.Parameters.AddWithValue("@isadminuser", newUser.IsAdminUser ? 1 : 0);

            command.ExecuteNonQuery();
        }

        public void DeleteUser(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string deleteSQL = "DELETE FROM Users WHERE Id = @id;";
            using var command = new SqliteCommand(deleteSQL, connection);
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }

        public User? GetUserByEmailAndPassword(string email, string password)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string selectSQL = "SELECT Id, UserName, Email, Password, IsAdminUser FROM Users WHERE Email = @email AND Password = @password;";
            using var command = new SqliteCommand(selectSQL, connection);
            command.Parameters.AddWithValue("@username", email);

            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@password", password);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    UserName = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    IsAdminUser = reader.GetInt32(4) == 1
                };
            }
            return null;
        }



    }
}