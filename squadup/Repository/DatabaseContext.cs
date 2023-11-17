using Npgsql;
using System.Data;

namespace squadup.Repository
{
    public class DatabaseContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DatabaseContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["Sensitive:DefaultConnection"];
        }

        public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
    }
}

