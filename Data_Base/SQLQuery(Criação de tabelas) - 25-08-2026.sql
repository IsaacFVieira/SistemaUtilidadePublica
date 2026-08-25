USE [SistemaUtilidadePublica];
GO

/* ============================================================
   1. TIPOS DE CONTA
   ============================================================ */

CREATE TABLE TipoConta
(
    Id_TipoConta INT IDENTITY(1,1) PRIMARY KEY,
    Nome VARCHAR(50) NOT NULL UNIQUE,
    Descricao VARCHAR(255) NULL,
    Ativo BIT NOT NULL DEFAULT 1
);
GO


/* ============================================================
   2. LOCALIZAÇÃO
   Utilizada pelos contactos de emergência para encontrar
   automaticamente o contacto mais próximo.
   ============================================================ */

CREATE TABLE Localizacao
(
    Id_Localizacao INT IDENTITY(1,1) PRIMARY KEY,

    Latitude DECIMAL(10,8) NOT NULL,
    Longitude DECIMAL(11,8) NOT NULL,

    Endereco VARCHAR(255) NULL,
    Bairro VARCHAR(100) NULL,
    Municipio VARCHAR(100) NULL,
    Provincia VARCHAR(100) NULL
);
GO


/* ============================================================
   3. TIPOS DE CONTACTOS DE EMERGÊNCIA
   ============================================================ */

CREATE TABLE TipoContactoEmergencia
(
    Id_TipoContacto INT IDENTITY(1,1) PRIMARY KEY,

    Nome VARCHAR(100) NOT NULL UNIQUE,

    Descricao VARCHAR(255) NULL,

    Ativo BIT NOT NULL DEFAULT 1
);
GO


/* ============================================================
   4. CONTACTOS DE EMERGÊNCIA
   ============================================================ */

CREATE TABLE ContactoEmergencia
(
    Id_Contacto INT IDENTITY(1,1) PRIMARY KEY,

    Nome VARCHAR(150) NOT NULL,

    Descricao VARCHAR(500) NULL,

    Telefone VARCHAR(30) NOT NULL,

    TelefoneAlternativo VARCHAR(30) NULL,

    Endereco VARCHAR(255) NULL,

    Id_TipoContacto INT NOT NULL,

    Id_Localizacao INT NOT NULL,

    Ativo BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_ContactoEmergencia_Tipo
        FOREIGN KEY (Id_TipoContacto)
        REFERENCES TipoContactoEmergencia(Id_TipoContacto),

    CONSTRAINT FK_ContactoEmergencia_Localizacao
        FOREIGN KEY (Id_Localizacao)
        REFERENCES Localizacao(Id_Localizacao)
);
GO


/* ============================================================
   5. TIPOS DE CONTEÚDO
   ============================================================ */

CREATE TABLE TipoConteudo
(
    Id_TipoConteudo INT IDENTITY(1,1) PRIMARY KEY,

    Nome VARCHAR(100) NOT NULL UNIQUE,

    Descricao VARCHAR(255) NULL,

    Ativo BIT NOT NULL DEFAULT 1
);
GO


/* ============================================================
   6. CATEGORIAS DE CONTEÚDO
   ============================================================ */

CREATE TABLE CategoriaConteudo
(
    Id_Categoria INT IDENTITY(1,1) PRIMARY KEY,

    Nome VARCHAR(100) NOT NULL,

    Descricao VARCHAR(255) NULL,

    Id_TipoConteudo INT NOT NULL,

    Ativo BIT NOT NULL DEFAULT 1,

    CONSTRAINT UQ_Categoria_Tipo
        UNIQUE (Nome, Id_TipoConteudo),

    CONSTRAINT FK_CategoriaConteudo_Tipo
        FOREIGN KEY (Id_TipoConteudo)
        REFERENCES TipoConteudo(Id_TipoConteudo)
);
GO


/* ============================================================
   7. CONTEÚDO
   ============================================================
   
   IMPORTANTE:
   Esta tabela só receberá conteúdos APROVADOS pela IA.

   Fluxo:
   
   Conteúdo enviado
          ↓
        IA
          ↓
      APROVADO
          ↓
   INSERT nesta tabela
          ↓
   INSERT em AvaliacaoIA

   Se REJEITADO:
   
   Conteúdo enviado
          ↓
        IA
          ↓
      REJEITADO
          ↓
   NÃO É ARMAZENADO
   ============================================================ */

CREATE TABLE Conteudo
(
    Id_Conteudo INT IDENTITY(1,1) PRIMARY KEY,

    Titulo VARCHAR(250) NOT NULL,

    Descricao VARCHAR(1000) NULL,

    ConteudoTexto VARCHAR(MAX) NULL,

    Id_TipoConteudo INT NOT NULL,

    Id_Categoria INT NULL,

    ImagemUrl VARCHAR(1000) NULL,

    VideoUrl VARCHAR(1000) NULL,

    AudioUrl VARCHAR(1000) NULL,

    Id_Criador INT NULL,

    DataCriacao DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    DataAtualizacao DATETIME2 NULL,

    Ativo BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Conteudo_Tipo
        FOREIGN KEY (Id_TipoConteudo)
        REFERENCES TipoConteudo(Id_TipoConteudo),

    CONSTRAINT FK_Conteudo_Categoria
        FOREIGN KEY (Id_Categoria)
        REFERENCES CategoriaConteudo(Id_Categoria),

    CONSTRAINT FK_Conteudo_Criador
        FOREIGN KEY (Id_Criador)
        REFERENCES Users(Id_User)
);
GO


/* ============================================================
   8. AVALIAÇÃO DA IA
   ============================================================
   
   Guarda SOMENTE a avaliação referente a conteúdos aprovados.

   Não existe registro de IA para conteúdos rejeitados.
   ============================================================ */

CREATE TABLE AvaliacaoIA
(
    Id_AvaliacaoIA BIGINT IDENTITY(1,1) PRIMARY KEY,

    Id_Conteudo INT NOT NULL,

    Modelo VARCHAR(100) NOT NULL,

    Classificacao VARCHAR(50) NOT NULL,

    Seriedade VARCHAR(30) NOT NULL,

    Confianca DECIMAL(5,4) NULL,

    Resultado VARCHAR(MAX) NULL,

    SugestaoCategoria INT NULL,

    DataAvaliacao DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_AvaliacaoIA_Conteudo
        FOREIGN KEY (Id_Conteudo)
        REFERENCES Conteudo(Id_Conteudo),

    CONSTRAINT FK_AvaliacaoIA_Categoria
        FOREIGN KEY (SugestaoCategoria)
        REFERENCES CategoriaConteudo(Id_Categoria),

    CONSTRAINT CK_AvaliacaoIA_Confianca
        CHECK
        (
            Confianca IS NULL
            OR Confianca BETWEEN 0 AND 1
        ),

    CONSTRAINT CK_AvaliacaoIA_Seriedade
        CHECK
        (
            Seriedade IN
            (
                'Baixa',
                'Media',
                'Alta',
                'Critica'
            )
        )
);
GO


/* ============================================================
   9. VISUALIZAÇÕES DOS CONTEÚDOS
   ============================================================ */

CREATE TABLE ConteudoVisualizacao
(
    Id_Visualizacao BIGINT IDENTITY(1,1) PRIMARY KEY,

    Id_Conteudo INT NOT NULL,

    Id_User INT NULL,

    DataHora DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Visualizacao_Conteudo
        FOREIGN KEY (Id_Conteudo)
        REFERENCES Conteudo(Id_Conteudo),

    CONSTRAINT FK_Visualizacao_Usuario
        FOREIGN KEY (Id_User)
        REFERENCES Users(Id_User)
);
GO


/* ============================================================
   10. DENÚNCIAS DOS CONTEÚDOS
   ============================================================ */

CREATE TABLE ConteudoDenuncia
(
    Id_Denuncia BIGINT IDENTITY(1,1) PRIMARY KEY,

    Id_Conteudo INT NOT NULL,

    Id_User INT NOT NULL,

    Motivo VARCHAR(150) NOT NULL,

    Descricao VARCHAR(1000) NULL,

    DataDenuncia DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    Estado VARCHAR(30) NOT NULL DEFAULT 'Pendente',

    CONSTRAINT FK_Denuncia_Conteudo
        FOREIGN KEY (Id_Conteudo)
        REFERENCES Conteudo(Id_Conteudo),

    CONSTRAINT FK_Denuncia_Usuario
        FOREIGN KEY (Id_User)
        REFERENCES Users(Id_User),

    CONSTRAINT CK_Denuncia_Estado
        CHECK
        (
            Estado IN
            (
                'Pendente',
                'EmAnalise',
                'Aceite',
                'Rejeitada'
            )
        )
);
GO


/* ============================================================
   11. INSERIR TIPOS DE CONTA
   ============================================================ */

INSERT INTO TipoConta
(
    Nome,
    Descricao
)
VALUES
(
    'Utilizador',
    'Utilizador normal do sistema'
),
(
    'Administrador',
    'Administrador do sistema'
),
(
    'Moderador',
    'Responsável pela moderação de conteúdos'
);
GO


/* ============================================================
   12. INSERIR TIPOS DE CONTACTOS DE EMERGÊNCIA
   ============================================================ */

INSERT INTO TipoContactoEmergencia
(
    Nome,
    Descricao
)
VALUES
(
    'Hospital',
    'Hospitais e unidades de saúde'
),
(
    'Policia',
    'Serviços de polícia'
),
(
    'Bombeiros',
    'Serviços de bombeiros'
),
(
    'Ambulancia',
    'Serviços de ambulância'
),
(
    'Protecao Civil',
    'Serviços de proteção civil'
);
GO


/* ============================================================
   13. INSERIR TIPOS DE CONTEÚDO
   ============================================================ */

INSERT INTO TipoConteudo
(
    Nome,
    Descricao
)
VALUES
(
    'Indicacao de Seguranca',
    'Informações e orientações de segurança'
),
(
    'Alerta de Seguranca',
    'Alertas relacionados com situações de perigo'
),
(
    'Primeiros Socorros',
    'Instruções de primeiros socorros'
);
GO



USE [SistemaUtilidadePublica];
GO

/* ============================================================
   ADICIONAR CAMPOS NECESSÁRIOS À TABELA USERS
   ============================================================ */

ALTER TABLE Users
ADD
    GoogleId VARCHAR(255) NULL,
    Id_TipoConta INT NULL,
    UltimoAcesso DATETIME2 NULL,
    Ativo BIT NOT NULL
        CONSTRAINT DF_Users_Ativo DEFAULT 1;
GO


/* ============================================================
   GOOGLE ID ÚNICO
   Um utilizador pode não ter GoogleId (NULL), mas quando tiver,
   não poderá existir outro utilizador com o mesmo GoogleId.
   ============================================================ */

CREATE UNIQUE INDEX IX_Users_GoogleId
ON Users(GoogleId)
WHERE GoogleId IS NOT NULL;
GO


/* ============================================================
   RELAÇÃO USERS → TIPOCONTA
   ============================================================ */

ALTER TABLE Users
ADD CONSTRAINT FK_Users_TipoConta
    FOREIGN KEY (Id_TipoConta)
    REFERENCES TipoConta(Id_TipoConta);
GO