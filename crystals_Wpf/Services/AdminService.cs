using crystals_Wpf.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace crystals_Wpf.Services
{
    public class AdminService
    {
        private string connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;
              Initial Catalog=AuthorizationDB;
              Integrated Security=True";

        public List<UserInfo> GetUsers()
        {
            List<UserInfo> users =
                new List<UserInfo>();

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"SELECT *
                      FROM Users
                      ORDER BY Id";

                SqlCommand command =
                    new SqlCommand(query, connection);

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(
                        new UserInfo
                        {
                            Id =
                                Convert.ToInt32(
                                    reader["Id"]),

                            Login =
                                reader["Login"].ToString(),

                            Role =
                                reader["Role"].ToString(),

                            RegistrationDate =
                                Convert.ToDateTime(
                                    reader["RegistrationDate"]),

                            IsBlocked =
                                Convert.ToBoolean(
                                    reader["IsBlocked"])
                        });
                }
            }

            return users;
        }

        public void BlockUser(int userId)
        {
            UpdateBlockedStatus(
                userId,
                true);
        }

        public void UnblockUser(int userId)
        {
            UpdateBlockedStatus(
                userId,
                false);
        }

        private void UpdateBlockedStatus(
            int userId,
            bool blocked)
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"UPDATE Users
                      SET IsBlocked = @Blocked
                      WHERE Id = @Id";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@Blocked",
                    blocked);

                command.Parameters.AddWithValue(
                    "@Id",
                    userId);

                command.ExecuteNonQuery();
            }
        }

        public void MakeAdmin(
            int userId)
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"UPDATE Users
                      SET Role = 'Admin'
                      WHERE Id = @Id";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@Id",
                    userId);

                command.ExecuteNonQuery();
            }
        }
    }
}