using Microsoft.Data.SqlClient;
using SistemaUtilidadePublicaAPI.Common.Exceptions;
using SistemaUtilidadePublicaAPI.DTOs;
using SistemaUtilidadePublicaAPI.DTOs.Content;
using SistemaUtilidadePublicaAPI.Models;
using System.Data;

namespace SistemaUtilidadePublicaAPI.Data.Repositories
{
    public class ContentRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public ContentRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }


        public async Task<ContentResponseDto> CreateAsync(CreateContentDto dto)
        {
            const string sql = """
        DECLARE @IdTipoConteudo INT;
        DECLARE @IdCategoria INT;

        -- 
        SELECT @IdTipoConteudo = Id_TipoConteudo
        FROM TipoConteudo
        WHERE Nome = @ContentType
          AND Ativo = 1;

        -- Verificar se o tipo existe
        IF @IdTipoConteudo IS NULL
        BEGIN
            THROW 50001, 'O tipo de conteúdo informado não existe ou está inativo.', 1;
        END;

        -- Procurar a categoria, caso tenha sido informada
        IF @Category IS NOT NULL
        BEGIN
            SELECT @IdCategoria = Id_Categoria
            FROM CategoriaConteudo
            WHERE LTRIM(RTRIM(Nome)) = LTRIM(RTRIM(@Category))
              AND Ativo = 1;

            -- Verificar se a categoria existe
            IF @IdCategoria IS NULL
            BEGIN
                THROW 50002, 'A categoria informada não existe ou está inativa.', 1;
            END;
        END;

        -- Inserir o conteúdo
        INSERT INTO Conteudo
        (
            Titulo,
            Descricao,
            ConteudoTexto,
            Id_TipoConteudo,
            Id_Categoria,
            ImagemUrl,
            VideoUrl,
            AudioUrl,
            Id_Criador
        )
        OUTPUT INSERTED.Id_Conteudo
        VALUES
        (
            @Title,
            @Description,
            @ContentText,
            @IdTipoConteudo,
            @IdCategoria,
            @ImageUrl,
            @VideoUrl,
            @AudioUrl,
            @CreatorId
        );
        """;

            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await using var command =
                new SqlCommand(sql, connection);

            command.Parameters.Add("@Title", SqlDbType.VarChar, 250)
                .Value = dto.Title;

            command.Parameters.Add("@Description", SqlDbType.VarChar, 1000)
                .Value = (object?)dto.Description ?? DBNull.Value;

            command.Parameters.Add("@ContentText", SqlDbType.VarChar, -1)
                .Value = (object?)dto.ContentText ?? DBNull.Value;

            command.Parameters.Add("@ContentType", SqlDbType.VarChar, 100)
                .Value = dto.ContentType;

            command.Parameters.Add("@Category", SqlDbType.VarChar, 100)
                .Value = (object?)dto.Category ?? DBNull.Value;

            command.Parameters.Add("@ImageUrl", SqlDbType.VarChar, 1000)
                .Value = (object?)dto.ImageUrl ?? DBNull.Value;

            command.Parameters.Add("@VideoUrl", SqlDbType.VarChar, 1000)
                .Value = (object?)dto.VideoUrl ?? DBNull.Value;

            command.Parameters.Add("@AudioUrl", SqlDbType.VarChar, 1000)
                .Value = (object?)dto.AudioUrl ?? DBNull.Value;

            command.Parameters.Add("@CreatorId", SqlDbType.Int)
                .Value = (object?)dto.CreatorId ?? DBNull.Value;

            var result = await command.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException(
                    "Não foi possível criar o conteúdo.");
            }

            var idConteudo = Convert.ToInt32(result);

            return new ContentResponseDto
            {
                Id_Content = idConteudo,
                Title = dto.Title,
                Description = dto.Description,
                ContentText = dto.ContentText,

                // Estes dados ainda serão preenchidos com os valores reais
                // posteriormente através do SELECT/JOIN.
                ContentType = dto.ContentType,
                Category = dto.Category,

                ImageUrl = dto.ImageUrl,
                VideoUrl = dto.VideoUrl,
                AudioUrl = dto.AudioUrl,

                Id_Creator = dto.CreatorId,

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                Active = true
            };
        }

        public async Task<List<ContentResponseDto>> GetAllContentAsync()
        {
            const string sql = """
SELECT 
    C.Id_Conteudo AS Id_Content,
    C.Titulo AS Title,
    C.Descricao AS Description,
    C.ConteudoTexto AS ContentText,
    T.Nome AS ContentType,
    CC.Nome AS Category,
    C.ImagemUrl AS ImageUrl,
    C.VideoUrl AS VideoUrl,
    C.AudioUrl AS AudioUrl,
    C.Id_Criador AS Id_Criador,
    C.DataCriacao AS CreatedAt,
    C.[DataAtualizacao] AS UpdatedAt,
    C.[Ativo] AS Active
FROM 
    Conteudo C
LEFT JOIN 
    TipoConteudo T ON C.Id_TipoConteudo = T.Id_TipoConteudo
LEFT JOIN 
    CategoriaConteudo CC ON C.Id_Categoria = CC.Id_Categoria
""";

            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await using var command =
                new SqlCommand(sql, connection);

            var results = await command.ExecuteReaderAsync();

            var contentResponses = new List<ContentResponseDto>();

            while (results.Read())
            {
                var contentResponse = new ContentResponseDto
                {
                    Id_Content = results.GetInt32("Id_Content"),
                    Title = results.GetString("Title"),
                    Description = results.IsDBNull("Description") ? (string?)null : results.GetString("Description"),
                    ContentText = results.IsDBNull("ContentText") ? (string?)null : results.GetString("ContentText"),
                    ContentType = results.GetString("ContentType"),
                    Category = results.IsDBNull("Category") ? (string?)null : results.GetString("Category"),
                    ImageUrl = results.IsDBNull("ImageUrl") ? (string?)null : results.GetString("ImageUrl"),
                    VideoUrl = results.IsDBNull("VideoUrl") ? (string?)null : results.GetString("VideoUrl"),
                    AudioUrl = results.IsDBNull("AudioUrl") ? (string?)null : results.GetString("AudioUrl"),
                    Id_Creator = results.GetInt32("Id_Criador"),
                    CreatedAt = results.GetDateTime("CreatedAt"),
                    UpdatedAt = results.IsDBNull("UpdatedAt") ? (DateTime?)null : results.GetDateTime("UpdatedAt"),
                    Active = results.GetBoolean("Active")
                };

                contentResponses.Add(contentResponse);
            }

            return contentResponses;
        }
        public async Task<ContentResponseDto> GetContentAsync(GetIdDto dto)
        {
            const string sql = """
SELECT 
    C.Id_Conteudo AS Id_Content,
    C.Titulo AS Title,
    C.Descricao AS Description,
    C.ConteudoTexto AS ContentText,
    T.Nome AS ContentType,
    CC.Nome AS Category,
    C.ImagemUrl AS ImageUrl,
    C.VideoUrl AS VideoUrl,
    C.AudioUrl AS AudioUrl,
    C.Id_Criador AS Id_Criador,
    C.DataCriacao AS CreatedAt,
    C.[DataAtualizacao] AS UpdatedAt,
    C.[Ativo] AS Active
FROM 
    Conteudo C
LEFT JOIN 
    TipoConteudo T ON C.Id_TipoConteudo = T.Id_TipoConteudo
LEFT JOIN 
    CategoriaConteudo CC ON C.Id_Categoria = CC.Id_Categoria
    WHERE 
   C.Id_Conteudo =@Id_content
""";

            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await using var command =
                new SqlCommand(sql, connection);
            command.Parameters.Add("@Id_content", SqlDbType.Int)
               .Value = (object?)dto.Id ?? DBNull.Value;


            var results = await command.ExecuteReaderAsync();

            var contentResponses = new List<ContentResponseDto>();

           
                var contentResponse = new ContentResponseDto
                {
                    Id_Content = results.GetInt32("Id_Content"),
                    Title = results.GetString("Title"),
                    Description = results.IsDBNull("Description") ? (string?)null : results.GetString("Description"),
                    ContentText = results.IsDBNull("ContentText") ? (string?)null : results.GetString("ContentText"),
                    ContentType = results.GetString("ContentType"),
                    Category = results.IsDBNull("Category") ? (string?)null : results.GetString("Category"),
                    ImageUrl = results.IsDBNull("ImageUrl") ? (string?)null : results.GetString("ImageUrl"),
                    VideoUrl = results.IsDBNull("VideoUrl") ? (string?)null : results.GetString("VideoUrl"),
                    AudioUrl = results.IsDBNull("AudioUrl") ? (string?)null : results.GetString("AudioUrl"),
                    Id_Creator = results.GetInt32("Id_Criador"),
                    CreatedAt = results.GetDateTime("CreatedAt"),
                    UpdatedAt = results.IsDBNull("UpdatedAt") ? (DateTime?)null : results.GetDateTime("UpdatedAt"),
                    Active = results.GetBoolean("Active")
                };

            return contentResponse;
            

           
        }


        public async Task<List<ContentResponseDto>> SearchGetContentAsync(searchDto dto)
        {
            const string sql = """
        SELECT 
            C.Id_Conteudo AS Id_Content,
            C.Titulo AS Title,
            C.Descricao AS Description,
            C.ConteudoTexto AS ContentText,
            T.Nome AS ContentType,
            CC.Nome AS Category,
            C.ImagemUrl AS ImageUrl,
            C.VideoUrl AS VideoUrl,
            C.AudioUrl AS AudioUrl,
            C.Id_Criador AS Id_Criador,
            C.DataCriacao AS CreatedAt,
            C.DataAtualizacao AS UpdatedAt,
            C.Ativo AS Active
        FROM Conteudo C
        LEFT JOIN TipoConteudo T
            ON C.Id_TipoConteudo = T.Id_TipoConteudo
        LEFT JOIN CategoriaConteudo CC
            ON C.Id_Categoria = CC.Id_Categoria
        WHERE
            C.Titulo LIKE @Item
            OR C.Descricao LIKE @Item
            OR C.ConteudoTexto LIKE @Item
            OR T.Nome LIKE @Item
            OR CC.Nome LIKE @Item
            OR (
                TRY_CONVERT(datetime2, @Item) IS NOT NULL
                AND C.DataCriacao >= TRY_CONVERT(datetime2, @Item)
                AND C.DataCriacao < DATEADD(DAY, 1, TRY_CONVERT(datetime2, @Item))
            )
        """;

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Item", SqlDbType.VarChar, 250)
     .Value = $"%{dto.Item?.Trim()}%";

            await using var results = await command.ExecuteReaderAsync();

            var contentResponses = new List<ContentResponseDto>();

            while (await results.ReadAsync())
            {
                var contentResponse = new ContentResponseDto
                {
                    Id_Content = results.GetInt32(results.GetOrdinal("Id_Content")),
                    Title = results.GetString(results.GetOrdinal("Title")),

                    Description = results.IsDBNull(results.GetOrdinal("Description"))
                        ? null
                        : results.GetString(results.GetOrdinal("Description")),

                    ContentText = results.IsDBNull(results.GetOrdinal("ContentText"))
                        ? null
                        : results.GetString(results.GetOrdinal("ContentText")),

                    ContentType = results.IsDBNull(results.GetOrdinal("ContentType"))
                        ? string.Empty
                        : results.GetString(results.GetOrdinal("ContentType")),

                    Category = results.IsDBNull(results.GetOrdinal("Category"))
                        ? null
                        : results.GetString(results.GetOrdinal("Category")),

                    ImageUrl = results.IsDBNull(results.GetOrdinal("ImageUrl"))
                        ? null
                        : results.GetString(results.GetOrdinal("ImageUrl")),

                    VideoUrl = results.IsDBNull(results.GetOrdinal("VideoUrl"))
                        ? null
                        : results.GetString(results.GetOrdinal("VideoUrl")),

                    AudioUrl = results.IsDBNull(results.GetOrdinal("AudioUrl"))
                        ? null
                        : results.GetString(results.GetOrdinal("AudioUrl")),

                    Id_Creator = results.IsDBNull(results.GetOrdinal("Id_Criador"))
                        ? null
                        : results.GetInt32(results.GetOrdinal("Id_Criador")),

                    CreatedAt = results.GetDateTime(results.GetOrdinal("CreatedAt")),

                    UpdatedAt = results.IsDBNull(results.GetOrdinal("UpdatedAt"))
                        ? null
                        : results.GetDateTime(results.GetOrdinal("UpdatedAt")),

                    Active = results.GetBoolean(results.GetOrdinal("Active"))
                };

                contentResponses.Add(contentResponse);
            }

            return contentResponses;
        }

        public async Task<bool> DeleteAsync(GetIdDto dto)
        {
            const string sql = """
        DELETE FROM Conteudo
        WHERE Id_Conteudo = @Id;
        """;

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Id", SqlDbType.Int)
                .Value = dto.Id;

            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }
        public async Task<bool> ActivateAsync(GetIdDto dto)
        {
            const string sql = """
        UPDATE Conteudo
        SET Ativo = 1,
            DataAtualizacao = SYSDATETIME()
        WHERE Id_Conteudo = @Id;
        """;

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Id", SqlDbType.Int)
                .Value = dto.Id;

            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeactivateAsync(GetIdDto dto)
        {
            const string sql = """
        UPDATE Conteudo
        SET Ativo = 0,
            DataAtualizacao = SYSDATETIME()
        WHERE Id_Conteudo = @Id;
        """;

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Id", SqlDbType.Int)
                .Value = dto.Id;

            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<ContentResponseDto> UpdateAsync(
      CreateContentDto dto,
      GetIdDto idDto)
        {
            const string sql = """
        SET NOCOUNT ON;

        DECLARE @IdTipoConteudo INT;
        DECLARE @IdCategoria INT;

        -- Verificar se o conteúdo existe
        IF NOT EXISTS (
            SELECT 1
            FROM Conteudo
            WHERE Id_Conteudo = @Id
        )
        BEGIN
            THROW 50000, 'O conteúdo informado não existe.', 1;
        END;

        -- Procurar o tipo de conteúdo
        SELECT @IdTipoConteudo = Id_TipoConteudo
        FROM TipoConteudo
        WHERE Nome = @ContentType
          AND Ativo = 1;

        -- Verificar se o tipo existe
        IF @IdTipoConteudo IS NULL
        BEGIN
            THROW 50001,
                'O tipo de conteúdo informado não existe ou está inativo.',
                1;
        END;

        -- Procurar a categoria, caso tenha sido informada
        IF @Category IS NOT NULL
        BEGIN
            SELECT @IdCategoria = Id_Categoria
            FROM CategoriaConteudo
            WHERE LTRIM(RTRIM(Nome)) = LTRIM(RTRIM(@Category))
              AND Ativo = 1;

            -- Verificar se a categoria existe
            IF @IdCategoria IS NULL
            BEGIN
                THROW 50002,
                    'A categoria informada não existe ou está inativa.',
                    1;
            END;
        END;

        -- Atualizar o conteúdo
        UPDATE Conteudo
        SET
            Titulo = @Title,
            Descricao = @Description,
            ConteudoTexto = @ContentText,
            Id_TipoConteudo = @IdTipoConteudo,
            Id_Categoria = @IdCategoria,
            ImagemUrl = @ImageUrl,
            VideoUrl = @VideoUrl,
            AudioUrl = @AudioUrl,
            DataAtualizacao = SYSDATETIME()
        WHERE Id_Conteudo = @Id;

        -- Retornar os dados atualizados
        SELECT
            C.Id_Conteudo AS Id_Content,
            C.Titulo AS Title,
            C.Descricao AS Description,
            C.ConteudoTexto AS ContentText,
            T.Nome AS ContentType,
            CC.Nome AS Category,
            C.ImagemUrl AS ImageUrl,
            C.VideoUrl AS VideoUrl,
            C.AudioUrl AS AudioUrl,
            C.Id_Criador AS Id_Creator,
            C.DataCriacao AS CreatedAt,
            C.DataAtualizacao AS UpdatedAt,
            C.Ativo AS Active
        FROM Conteudo C
        INNER JOIN TipoConteudo T
            ON C.Id_TipoConteudo = T.Id_TipoConteudo
        LEFT JOIN CategoriaConteudo CC
            ON C.Id_Categoria = CC.Id_Categoria
        WHERE C.Id_Conteudo = @Id;
        """;

            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await using var command =
                new SqlCommand(sql, connection);

            command.Parameters.Add("@Id", SqlDbType.Int)
                .Value = idDto.Id;

            command.Parameters.Add("@Title", SqlDbType.VarChar, 250)
                .Value = dto.Title;

            command.Parameters.Add("@Description", SqlDbType.VarChar, 1000)
                .Value = (object?)dto.Description ?? DBNull.Value;

            command.Parameters.Add("@ContentText", SqlDbType.VarChar, -1)
                .Value = (object?)dto.ContentText ?? DBNull.Value;

            command.Parameters.Add("@ContentType", SqlDbType.VarChar, 100)
                .Value = dto.ContentType;

            command.Parameters.Add("@Category", SqlDbType.VarChar, 100)
                .Value = (object?)dto.Category ?? DBNull.Value;

            command.Parameters.Add("@ImageUrl", SqlDbType.VarChar, 1000)
                .Value = (object?)dto.ImageUrl ?? DBNull.Value;

            command.Parameters.Add("@VideoUrl", SqlDbType.VarChar, 1000)
                .Value = (object?)dto.VideoUrl ?? DBNull.Value;

            command.Parameters.Add("@AudioUrl", SqlDbType.VarChar, 1000)
                .Value = (object?)dto.AudioUrl ?? DBNull.Value;

            try
            {
                await using var reader =
                    await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    throw new ExceptionCommon(
                        "Não foi possível obter o conteúdo atualizado.");
                }

                return new ContentResponseDto
                {
                    Id_Content = reader.GetInt32(
                        reader.GetOrdinal("Id_Content")),

                    Title = reader.GetString(
                        reader.GetOrdinal("Title")),

                    Description = reader.IsDBNull(
                        reader.GetOrdinal("Description"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("Description")),

                    ContentText = reader.IsDBNull(
                        reader.GetOrdinal("ContentText"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("ContentText")),

                    ContentType = reader.GetString(
                        reader.GetOrdinal("ContentType")),

                    Category = reader.IsDBNull(
                        reader.GetOrdinal("Category"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("Category")),

                    ImageUrl = reader.IsDBNull(
                        reader.GetOrdinal("ImageUrl"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("ImageUrl")),

                    VideoUrl = reader.IsDBNull(
                        reader.GetOrdinal("VideoUrl"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("VideoUrl")),

                    AudioUrl = reader.IsDBNull(
                        reader.GetOrdinal("AudioUrl"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("AudioUrl")),

                    Id_Creator = reader.IsDBNull(
                        reader.GetOrdinal("Id_Creator"))
                        ? null
                        : reader.GetInt32(
                            reader.GetOrdinal("Id_Creator")),

                    CreatedAt = reader.GetDateTime(
                        reader.GetOrdinal("CreatedAt")),

                    UpdatedAt = reader.IsDBNull(
                        reader.GetOrdinal("UpdatedAt"))
                        ? null
                        : reader.GetDateTime(
                            reader.GetOrdinal("UpdatedAt")),

                    Active = reader.GetBoolean(
                        reader.GetOrdinal("Active"))
                };
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public async Task<List<ContentResponseDto>> GetByQuantityAsync(
    GetIdDto dto)
        {
            const string sql = """
        SELECT TOP (@Quantity)
            C.Id_Conteudo AS Id_Content,
            C.Titulo AS Title,
            C.Descricao AS Description,
            C.ConteudoTexto AS ContentText,
            T.Nome AS ContentType,
            CC.Nome AS Category,
            C.ImagemUrl AS ImageUrl,
            C.VideoUrl AS VideoUrl,
            C.AudioUrl AS AudioUrl,
            C.Id_Criador AS Id_Creator,
            C.DataCriacao AS CreatedAt,
            C.DataAtualizacao AS UpdatedAt,
            C.Ativo AS Active
        FROM Conteudo C
        INNER JOIN TipoConteudo T
            ON C.Id_TipoConteudo = T.Id_TipoConteudo
        LEFT JOIN CategoriaConteudo CC
            ON C.Id_Categoria = CC.Id_Categoria
        WHERE C.Ativo = 1
        ORDER BY C.DataCriacao DESC;
        """;

            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await using var command =
                new SqlCommand(sql, connection);

            command.Parameters.Add("@Quantity", SqlDbType.Int)
                .Value = dto.Id;

            var contents = new List<ContentResponseDto>();

            await using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                contents.Add(new ContentResponseDto
                {
                    Id_Content = reader.GetInt32(
                        reader.GetOrdinal("Id_Content")),

                    Title = reader.GetString(
                        reader.GetOrdinal("Title")),

                    Description = reader.IsDBNull(
                        reader.GetOrdinal("Description"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("Description")),

                    ContentText = reader.IsDBNull(
                        reader.GetOrdinal("ContentText"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("ContentText")),

                    ContentType = reader.GetString(
                        reader.GetOrdinal("ContentType")),

                    Category = reader.IsDBNull(
                        reader.GetOrdinal("Category"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("Category")),

                    ImageUrl = reader.IsDBNull(
                        reader.GetOrdinal("ImageUrl"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("ImageUrl")),

                    VideoUrl = reader.IsDBNull(
                        reader.GetOrdinal("VideoUrl"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("VideoUrl")),

                    AudioUrl = reader.IsDBNull(
                        reader.GetOrdinal("AudioUrl"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("AudioUrl")),

                    Id_Creator = reader.IsDBNull(
                        reader.GetOrdinal("Id_Creator"))
                        ? null
                        : reader.GetInt32(
                            reader.GetOrdinal("Id_Creator")),

                    CreatedAt = reader.GetDateTime(
                        reader.GetOrdinal("CreatedAt")),

                    UpdatedAt = reader.IsDBNull(
                        reader.GetOrdinal("UpdatedAt"))
                        ? null
                        : reader.GetDateTime(
                            reader.GetOrdinal("UpdatedAt")),

                    Active = reader.GetBoolean(
                        reader.GetOrdinal("Active"))
                });
            }

            return contents;
        }

        public async Task<bool> RegisterViewAsync(CreateContentViewDto dto)
        {
            const string sql = """
        IF NOT EXISTS (
            SELECT 1
            FROM Conteudo
            WHERE Id_Conteudo = @IdContent
              AND Ativo = 1
        )
        BEGIN
            THROW 50003, 'O conteúdo informado não existe ou está inativo.', 1;
        END;

        INSERT INTO ConteudoVisualizacao
        (
            Id_Conteudo,
            Id_User
        )
        VALUES
        (
            @IdContent,
            @IdUser
        );
        """;

            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await using var command =
                new SqlCommand(sql, connection);

            command.Parameters.Add("@IdContent", SqlDbType.Int)
                .Value = dto.Id_Content;

            command.Parameters.Add("@IdUser", SqlDbType.Int)
                .Value = (object?)dto.Id_User ?? DBNull.Value;

            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> CreateCommentAsync(CreateContentCommentDto dto)
        {
            const string sql = """
                IF NOT EXISTS (
                    SELECT 1
                    FROM Conteudo
                    WHERE Id_Conteudo = @IdContent
                      AND Ativo = 1
                )
                BEGIN
                    THROW 50004, 'O conteúdo informado não existe ou está inativo.', 1;
                END;

                INSERT INTO ConteudoComentario
                (
                    Id_Conteudo,
                    Id_User,
                    Comentario
                )
                VALUES
                (
                    @IdContent,
                    @IdUser,
                    @Comentario
                );
                """;

            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await using var command =
                new SqlCommand(sql, connection);

            command.Parameters.Add("@IdContent", SqlDbType.Int)
                .Value = dto.Id_Content;

            command.Parameters.Add("@IdUser", SqlDbType.Int)
                .Value = (object?)dto.Id_User ?? DBNull.Value;

            command.Parameters.Add("@Comentario", SqlDbType.VarChar, 1000)
                .Value = dto.Comment;

            var rowsAffected =
                await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateCommentAsync(UpdateContentCommentDto dto)
        {
            const string sql = """
        IF NOT EXISTS (
            SELECT 1
            FROM ConteudoComentario
            WHERE Id_Comentario = @IdComment
              AND Ativo = 1
        )
        BEGIN
            THROW 50005, 'O comentário informado não existe ou está inativo.', 1;
        END;

        UPDATE ConteudoComentario
        SET
            Comentario = @Comentario,
            DataAtualizacao = SYSDATETIME()
        WHERE Id_Comentario = @IdComment
          AND Ativo = 1;
        """;

            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await using var command =
                new SqlCommand(sql, connection);

            command.Parameters.Add("@IdComment", SqlDbType.BigInt)
                .Value = dto.Id_Comentario;

            command.Parameters.Add("@Comentario", SqlDbType.VarChar, 1000)
                .Value = dto.Comment;

            var rowsAffected =
                await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteCommentAsync(GetIdDto dto)
        {
            const string sql = """
        DELETE FROM ConteudoComentario
        WHERE Id_Comentario = @IdComment;
        """;

            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await using var command =
                new SqlCommand(sql, connection);

            command.Parameters.Add("@IdComment", SqlDbType.BigInt)
                .Value = dto.Id;

            var rowsAffected =
                await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> CreateReportAsync(CreateContentReportDto dto)
        {
            const string sql = """
        IF NOT EXISTS (
            SELECT 1
            FROM Conteudo
            WHERE Id_Conteudo = @IdContent
              AND Ativo = 1
        )
        BEGIN
            THROW 50006, 'O conteúdo informado não existe ou está inativo.', 1;
        END;

        IF NOT EXISTS (
            SELECT 1
            FROM Users
            WHERE Id_User = @IdUser
        )
        BEGIN
            THROW 50007, 'O usuário informado não existe.', 1;
        END;

        INSERT INTO ConteudoDenuncia
        (
            Id_Conteudo,
            Id_User,
            Motivo,
            Descricao
        )
        VALUES
        (
            @IdContent,
            @IdUser,
            @Motivo,
            @Descricao
        );
        """;

            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await using var command =
                new SqlCommand(sql, connection);

            command.Parameters.Add("@IdContent", SqlDbType.Int)
                .Value = dto.Id_Content;

            command.Parameters.Add("@IdUser", SqlDbType.Int)
                .Value = dto.Id_User;

            command.Parameters.Add("@Motivo", SqlDbType.VarChar, 150)
                .Value = dto.Reason;

            command.Parameters.Add("@Descricao", SqlDbType.VarChar, 1000)
                .Value = (object?)dto.Description ?? DBNull.Value;

            var rowsAffected =
                await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }
    }

}
