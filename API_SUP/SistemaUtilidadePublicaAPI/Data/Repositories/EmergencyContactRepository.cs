using Microsoft.Data.SqlClient;
using SistemaUtilidadePublicaAPI.DTOs.EmergencyContact;
using SistemaUtilidadePublicaAPI.Models;


namespace SistemaUtilidadePublicaAPI.Data.Repositories
{
    public class EmergencyContactRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public EmergencyContactRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(EmergencyContact emergencyContact)
        {
            const string sql = """
                Insert Into ContactoEmergencia
                (
                  Nome,
                  Descricao,
                  Telefone,
                  TelefoneAlternativo,
                  Endereco,
                  Id_TipoContacto,
                  Id_Localizacao,
                  Ativo
                )
                output inserted.Id_Contacto
                values
                (
                    @Nome,
                    @Descricao,
                    @Telefone,
                    @TelefoneAlternativo,
                    @Endereco,
                    @Id_TipoContacto,
                    @Id_Localizacao,
                    @Ativo
                );
                """;

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Nome", emergencyContact.Name);
            command.Parameters.AddWithValue("@Descricao", (object)emergencyContact.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Telefone", (object)emergencyContact.PhoneNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@TelefoneAlternativo", (object)emergencyContact.PhoneNumber2 ?? DBNull.Value);
            command.Parameters.AddWithValue("@Endereco", (object)emergencyContact.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@Id_TipoContacto", emergencyContact.Id_EmergencyContactType);
            command.Parameters.AddWithValue("@Id_Localizacao", emergencyContact.Id_Location);
            command.Parameters.AddWithValue("@Ativo", emergencyContact.IsActive);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<List<EmergencyContactResponseDto>> GetAllAsync()
        {
            const string sql = """
                                SELECT
                    ce.Id_Contacto,
                    ce.Nome,
                    ce.Descricao,
                    ce.Telefone,
                    ce.TelefoneAlternativo,
                    ce.Endereco,
                    ce.Id_TipoContacto,
                    ce.Id_Localizacao,
                    ce.Ativo,

                    l.Id_Localizacao,
                    l.Latitude,
                    l.Longitude,
                    l.Endereco AS Localizacao_Endereco,
                    l.Bairro,
                    l.Municipio,
                    l.Provincia

                FROM ContactoEmergencia AS ce
                INNER JOIN Localizacao AS l
                    ON ce.Id_Localizacao = l.Id_Localizacao

                WHERE ce.Ativo = 1;
              
                """;
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            
            await using var reader = await command.ExecuteReaderAsync();
            var contacts = new List<EmergencyContactResponseDto>();

            while (await reader.ReadAsync())
            {
                var emergencyContact = new EmergencyContactResponseDto
                {
                    Id_EmergencyContact = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    PhoneNumber = reader.GetString(3),
                    PhoneNumber2 = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Address = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Id_EmergencyContactType = reader.GetInt32(6),

                    Location = new LocationResponseDto
                    {
                        Id_Localizacao = reader.GetInt32(9),
                        Latitude = reader.GetDecimal(10),
                        Longitude = reader.GetDecimal(11),
                        Endereco = reader.IsDBNull(12) ? null : reader.GetString(12),
                        Bairro = reader.IsDBNull(13) ? null : reader.GetString(13),
                        Municipio = reader.IsDBNull(14) ? null : reader.GetString(14),
                        Provincia = reader.IsDBNull(15) ? null : reader.GetString(15)
                    },

                    IsActive = reader.GetBoolean(8)
                };

                contacts.Add(emergencyContact);
            }

            return contacts;

        }

        public async Task<List<EmergencyContactResponseDto>> GetNearestByCategoryAsync(NearestEmergencyContactRequestDto dto)
        {
            const string sql = """
                  DECLARE @LatitudeUsuario DECIMAL(10,8) = @Latitude;
                DECLARE @LongitudeUsuario DECIMAL(11,8) = @Longitude;

                WITH ContactosComDistancia AS
                (
                    SELECT
                        ce.Id_Contacto,
                        ce.Nome,
                        ce.Descricao,
                        ce.Telefone,
                        ce.TelefoneAlternativo,
                        ce.Endereco,
                        ce.Id_TipoContacto,
                        ce.Id_Localizacao,
                        ce.Ativo,

                        l.Latitude,
                        l.Longitude,
                        l.Endereco AS Localizacao_Endereco,
                        l.Bairro,
                        l.Municipio,
                        l.Provincia,

                        6371 * ACOS(
                            COS(RADIANS(@LatitudeUsuario))
                            * COS(RADIANS(l.Latitude))
                            * COS(RADIANS(@LongitudeUsuario - l.Longitude))
                            + SIN(RADIANS(@LatitudeUsuario))
                            * SIN(RADIANS(l.Latitude))
                        ) AS DistanciaKm

                    FROM ContactoEmergencia AS ce

                    INNER JOIN Localizacao AS l
                        ON ce.Id_Localizacao = l.Id_Localizacao

                    WHERE ce.Ativo = 1
                ),

                ContactosNumerados AS
                (
                    SELECT
                        *,
                        ROW_NUMBER() OVER
                        (
                            PARTITION BY Id_TipoContacto
                            ORDER BY DistanciaKm ASC
                        ) AS Numero
                    FROM ContactosComDistancia
                )

                SELECT
                    Id_Contacto,
                    Nome,
                    Descricao,
                    Telefone,
                    TelefoneAlternativo,
                    Endereco,
                    Id_TipoContacto,
                    Id_Localizacao,
                    Ativo,
                    Latitude,
                    Longitude,
                    Localizacao_Endereco,
                    Bairro,
                    Municipio,
                    Provincia,
                    DistanciaKm
                FROM ContactosNumerados
                WHERE Numero <= 5
                ORDER BY Id_TipoContacto, DistanciaKm ASC;              
                  
                """;
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Latitude", dto.Latitude);
            command.Parameters.AddWithValue("@Longitude", dto.Longitude);
            await using var reader = await command.ExecuteReaderAsync();
            var contacts = new List<EmergencyContactResponseDto>();

            while (await reader.ReadAsync())
            {
                var emergencyContact = new EmergencyContactResponseDto
                {
                    Id_EmergencyContact = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    PhoneNumber = reader.GetString(3),
                    PhoneNumber2 = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Address = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Id_EmergencyContactType = reader.GetInt32(6),

                    Location = new LocationResponseDto
                    {
                        Id_Localizacao = reader.GetInt32(7),
                        Latitude = reader.GetDecimal(9),
                        Longitude = reader.GetDecimal(10),
                        Endereco = reader.IsDBNull(11) ? null : reader.GetString(11),
                        Bairro = reader.IsDBNull(12) ? null : reader.GetString(12),
                        Municipio = reader.IsDBNull(13) ? null : reader.GetString(13),
                        Provincia = reader.IsDBNull(14) ? null : reader.GetString(14)
                    },

                    IsActive = reader.GetBoolean(8),

                    DistanceKm = reader.GetDouble(15)
                };

                contacts.Add(emergencyContact);
            }

            return contacts;

        }

    }
}
        