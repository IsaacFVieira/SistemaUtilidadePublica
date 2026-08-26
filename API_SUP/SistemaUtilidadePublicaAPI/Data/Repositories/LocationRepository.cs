using Microsoft.Data.SqlClient;
using SistemaUtilidadePublicaAPI.Data;
using SistemaUtilidadePublicaAPI.Models;

namespace SistemaUtilidadePublicaAPI.Data.Repositories
{
    public class LocationRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public LocationRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(Location location)
        {
            const string sql = """
        INSERT INTO Localizacao
        (
            Latitude,
            Longitude,
            Endereco,
            Bairro,
            Municipio,
            Provincia
        )
        OUTPUT INSERTED.Id_Localizacao
        VALUES
        (
            @Latitude,
            @Longitude,
            @Endereco,
            @Bairro,
            @Municipio,
            @Provincia
        );
        """;

            await using var connection = _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Latitude", location.Latitude);
            command.Parameters.AddWithValue("@Longitude", location.Longitude);
            command.Parameters.AddWithValue("@Endereco",
                (object?)location.Endereco ?? DBNull.Value);
            command.Parameters.AddWithValue("@Bairro",
                (object?)location.Bairro ?? DBNull.Value);
            command.Parameters.AddWithValue("@Municipio",
                (object?)location.Municipio ?? DBNull.Value);
            command.Parameters.AddWithValue("@Provincia",
                (object?)location.Provincia ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }
        public async Task<bool> ExistsAsync(decimal latitude, decimal longitude)
        {
            const string sql = """
        SELECT COUNT(1)
        FROM Localizacao
        WHERE Latitude = @Latitude
          AND Longitude = @Longitude;
        """;

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Latitude", latitude);
            command.Parameters.AddWithValue("@Longitude", longitude);

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }
    }
}
