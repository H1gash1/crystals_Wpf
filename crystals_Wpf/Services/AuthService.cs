using crystals_Wpf.Services;
using crystals_Wpf.Models;
using System.Data.SqlClient;

namespace crystals_Wpf.Services
{
    public class AuthService
    {
        public User Login(string login, string password)
        {
            string passwordHash =
                EncryptionService.HashPassword(password);

            using (SqlConnection connection =
                DatabaseService.GetConnection())
            {
                connection.Open();

                string query =
                    @"SELECT *
            FROM Users
            WHERE Login = @Login
            AND PasswordHash = @PasswordHash";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                SqlDataReader reader =
                    command.ExecuteReader();

                if (reader.Read())
                {
                    return new User
                    {
                        Id = (int)reader["Id"],
                        Login = reader["Login"].ToString(),
                        PasswordHash = reader["PasswordHash"].ToString(),
                        Role = reader["Role"].ToString(),
                        IsBlocked = (bool)reader["IsBlocked"]  
                    };
                }

                return null;
            }
        }
        public bool UserExists(string login)
        {
            using (SqlConnection connection =
                DatabaseService.GetConnection())
            {
                connection.Open();

                string query =
                    "SELECT COUNT(*) FROM Users WHERE Login = @Login";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Login", login);

                int count =
                    (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        public bool Register(string login, string password)
        {
            if (UserExists(login))
                return false;

            string passwordHash =
                EncryptionService.HashPassword(password);

            using (SqlConnection connection =
                DatabaseService.GetConnection())
            {
                connection.Open();

                string query =
                @"INSERT INTO Users
                (Login, PasswordHash, Role)
                VALUES
                (@Login, @PasswordHash, 'User')";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                command.ExecuteNonQuery();
            }

            return true;
        }
    }
}