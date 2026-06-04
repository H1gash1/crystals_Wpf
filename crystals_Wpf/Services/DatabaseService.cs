using System.Data.SqlClient;

namespace crystals_Wpf.Services
{
    public static class DatabaseService
    {
        private static readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;
              Database=AuthorizationDB;
              Trusted_Connection=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}