using Microsoft.Data.SqlClient;
using SistemaUtilidadePublicaAPI.Models;
using SistemaUtilidadePublicaAPI.DTOs.Authentication;

namespace SistemaUtilidadePublicaAPI.Data.Repositories
{
    public class UserRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public UserRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> EamilExistsAsync(string email)
        {
            const string sql = """
                select count(1)
                from Users
                where Email = @Email;
                """;

            await using var connection = _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Email", email);
            var result = await command.ExecuteScalarAsync();
             return Convert.ToInt32(result) > 0;
        }

        public async Task<User?> LoginAsync(LoginUserDto dto)
        {
            const string sql = """
                select Id_User, Name, Telefone, Email, PasswordHash, Salt, ImagemPerfil, CreatedAt
                from Users
                where Email = @Email;
                """;
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", dto.Email);
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null; // User not found
            }
            var user = new User
            {
                Id_User = reader.GetInt32(0),
                Name = reader.GetString(1),
                Telefone = reader.IsDBNull(2) ? null : reader.GetString(2),
                Email = reader.GetString(3),
                PasswordHash = reader.GetString(4),
                Salt = reader.GetString(5),
                ImagemPerfil = reader.IsDBNull(6) ? null : reader.GetString(6),
                Create_Date = reader.GetDateTime(7)
            };

            return user;
           
        }
        public async Task<int> CreateAsync(User user)
        {
            const string sql = """
                Insert Into Users
                (
                  Name,
                  Telefone,
                  Email,
                  PasswordHash,
                  Salt,
                  ImagemPerfil
                )
                output inserted.Id_User
                values
                (
                    @Name,
                    @Telefone,
                    @Email,
                    @PasswordHash,
                    @Salt,
                    @ImagemPerfil
                
                );
                """;
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Telefone", (object)user.Telefone ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@Salt", user.Salt);
            command.Parameters.AddWithValue("@ImagemPerfil", (object)user.ImagemPerfil ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

    }
}
