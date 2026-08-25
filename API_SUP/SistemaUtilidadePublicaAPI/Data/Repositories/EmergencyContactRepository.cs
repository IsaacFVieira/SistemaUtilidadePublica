using Microsoft.Data.SqlClient;
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
    }
}
