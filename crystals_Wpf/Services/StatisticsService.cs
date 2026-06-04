using System;
using System.Data.SqlClient;

namespace crystals_Wpf.Services
{
    public class StatisticsService
    {
        private string connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;
            Initial Catalog=AuthorizationDB;
            Integrated Security=True";

        public void UpdateStatistics(
            int userId,
            int score,
            int matches,
            int maxCombo)
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string checkQuery =
                    @"SELECT UserId
                      FROM UserStatistics
                      WHERE UserId = @UserId";

                SqlCommand checkCommand =
                    new SqlCommand(checkQuery, connection);

                checkCommand.Parameters.AddWithValue(
                    "@UserId",
                    userId);

                object result =
                    checkCommand.ExecuteScalar();

                if (result == null)
                {
                    string insertQuery =
                        @"INSERT INTO UserStatistics
                        (
                            UserId,
                            GamesPlayed,
                            TotalScore,
                            TotalMatches,
                            MaxComboLength
                        )
                        VALUES
                        (
                            @UserId,
                            1,
                            @Score,
                            @Matches,
                            @MaxCombo
                        )";

                    SqlCommand insertCommand =
                        new SqlCommand(
                            insertQuery,
                            connection);

                    insertCommand.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    insertCommand.Parameters.AddWithValue(
                        "@Score",
                        score);

                    insertCommand.Parameters.AddWithValue(
                        "@Matches",
                        matches);

                    insertCommand.Parameters.AddWithValue(
                        "@MaxCombo",
                        maxCombo);

                    insertCommand.ExecuteNonQuery();
                }
                else
                {
                    string updateQuery =
                        @"UPDATE UserStatistics
                        SET
                            GamesPlayed = GamesPlayed + 1,
                            TotalScore = TotalScore + @Score,
                            TotalMatches = TotalMatches + @Matches,
                            MaxComboLength =
                            CASE
                                WHEN MaxComboLength < @MaxCombo
                                THEN @MaxCombo
                                ELSE MaxComboLength
                            END
                        WHERE UserId = @UserId";

                    SqlCommand updateCommand =
                        new SqlCommand(
                            updateQuery,
                            connection);

                    updateCommand.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    updateCommand.Parameters.AddWithValue(
                        "@Score",
                        score);

                    updateCommand.Parameters.AddWithValue(
                        "@Matches",
                        matches);

                    updateCommand.Parameters.AddWithValue(
                        "@MaxCombo",
                        maxCombo);

                    updateCommand.ExecuteNonQuery();
                }
            }
        }
    }
}