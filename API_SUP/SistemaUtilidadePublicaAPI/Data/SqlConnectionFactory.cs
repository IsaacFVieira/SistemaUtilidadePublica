using Microsoft.Data.SqlClient;

namespace SistemaUtilidadePublicaAPI.Data
{
    public class SqlConnectionFactory
    {
        private readonly IConfiguration _configuration;
        public SqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public SqlConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if(string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "A connection string 'DefaultConnetion' não foi configurada.");

            }

            return new SqlConnection(connectionString);
        }
    }
}
