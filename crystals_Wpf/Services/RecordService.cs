using crystals_Wpf.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace crystals_Wpf.Services
{
    public class RecordService
    {
        private string connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;
            Initial Catalog=AuthorizationDB;
            Integrated Security=True";

        public void SaveRecord(
            int userId,
            int score)
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string checkQuery =
                    @"SELECT BestScore
                      FROM Records
                      WHERE UserId = @UserId";

                SqlCommand checkCommand =
                    new SqlCommand(checkQuery, connection);

                checkCommand.Parameters.AddWithValue(
                    "@UserId", userId);

                object result =
                    checkCommand.ExecuteScalar();

                if (result == null)
                {
                    string insertQuery =
                        @"INSERT INTO Records
                        (UserId, BestScore, RecordDate)
                        VALUES
                        (@UserId, @BestScore, GETDATE())";

                    SqlCommand insertCommand =
                        new SqlCommand(
                            insertQuery,
                            connection);

                    insertCommand.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    insertCommand.Parameters.AddWithValue(
                        "@BestScore",
                        score);

                    insertCommand.ExecuteNonQuery();
                }
                else
                {
                    int bestScore =
                        Convert.ToInt32(result);

                    if (score > bestScore)
                    {
                        string updateQuery =
                            @"UPDATE Records
                            SET BestScore = @BestScore,
                                RecordDate = GETDATE()
                            WHERE UserId = @UserId";

                        SqlCommand updateCommand =
                            new SqlCommand(
                                updateQuery,
                                connection);

                        updateCommand.Parameters.AddWithValue(
                            "@BestScore",
                            score);

                        updateCommand.Parameters.AddWithValue(
                            "@UserId",
                            userId);

                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<Record> GetRecords()
        {
            List<Record> records =
                new List<Record>();

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"SELECT
                Users.Login,
                Records.BestScore,
                Records.RecordDate
              FROM Records
              INNER JOIN Users
              ON Records.UserId = Users.Id
              ORDER BY Records.BestScore DESC";

                SqlCommand command =
                    new SqlCommand(query, connection);

                SqlDataReader reader =
                    command.ExecuteReader();

                int place = 1;

                while (reader.Read())
                {
                    records.Add(
                        new Record
                        {
                            Place = place++,
                            Login = reader["Login"].ToString(),
                            BestScore =
                                Convert.ToInt32(
                                    reader["BestScore"]),
                            RecordDate =
                                Convert.ToDateTime(
                                    reader["RecordDate"])
                        });
                }
            }

            return records;
        }
    }
}