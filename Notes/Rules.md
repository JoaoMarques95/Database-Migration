# Objectivo:

Migrar os dados todos para uma stack nova.

# Passos:

1. Perceber esquema.
2. Construir novo stack.
   1. Construir tabela Faculdades
   2. Adicionar FK Faculdade -- universidade
   3. Adicionar FK Education Experience -- Faculdade
   4. Adicionar flag à Education Experience {NotListedInstitution}
3. Construir sript dinamico para migrar os dados dos utilizadores
   1. Exportar excel.
   2. Fazer o programa c# usando esse dataSet para testar.
   3. Correr Sript de SQL da BD de review (falar com o Marcelo para ver como isso se processa).
4. Eliminar antigo stack

# Transformação de dados Script

- Dados inconsistentes nas universidades:
  - Flag "Deleted":"true" --> Ver se o nome da universidade existe no "false" flag (fica apenas uma entrada para a universidade)
  - Flag "Deleted":"false" --> Nome da universidade é válida
- Faculdade construção de Dados:
  - Mesma coisa, todos os nomes diferentes associados à mesma universidade. Têm que ser adicionados em colunas diferentes na tabela com a FK da universidade.
- Modelar dados na User Education Experience:
  - Se a universidade associada à falculdade tem uma flag "true" (inserida pelo utilizador), então colocar a "NotListedInstitution" flag equals to true.

# Ordem de transformação de dados

1.  Modificar dados das universidades guardando os dados das faculdades.
2.  Inserir todas as faculdades de todas as universidades na tabela.
3.  Actualizar a "NotListedInstitution" na UEE.

# Estratégia

1.  Ter um programa em C# que leia ficheiros excel que foram exportados pela BD. (Ver rever curso link plugralsight). Receber modelo e retornar script SQL.
2.  Ter lógica para executar a "# Ordem de transformação de dados", no Sript SQL retornado.
3.  Ter algumas configurações onde vou colocar regras nas quais constam:
    1. O que é um data set de universidades inválido? (Criar delete Statement). -- Solução otima n perfeita
    2. Quais os critérios para dar match de uma universidade customizada? -- Solução otima n perfeita
4.  A criação da nova estrutura e destruição da antiga fazemos à mão (só preciso fazer 1 vez).
5.  A performance do programa não é relevante.

# Notas:

- Trabalhar no branch dev a unica coisa que vou mudar no código são mesmo os scripts de SQL depois quando for para correr.
- UEE ligar diretamente às faculdades e não às universidades ( "UP" 1---inf "UEE" 1---1 "F" 1---inf "U").
  - Questão: Então as faculdades vão ficar no Front-End como Free-Style? Teoricamente as faculdades são todas "NotListedInstitutuion" para não o serem eles tinham mesmo que ter uma serie de campos nas faculdades pré defenidos por eles a "false" que mostravam sempre no drop down das faculdades.
  - O objectivo não é ter dados mais consistentes no que toca às universidades, porque para isso tinhamos que ter infinitas entradas by default na BD que tivessem todas as universidades do mundo, impedindo que o utilizador colocase um nome customizado da Universidade dele (havendo sempre a uni dele). As universidades vão continuar iguais, com a flexibilidade de o user adicionar mais opções.
- Se apenas temos nome da universidade, nome da faculdade é igual. Id Universidade tem que ficar iguais, faculdade vai ter uma FK com esse valor.

# Programa C#

- Data Set: UEE e U.
- 1º Programa:

  - SQL para mudar Tabelas Universidades e faculdades.
  - Se necessário escrever novo dataset das Universiades ou Faculdades em excel para a utilização em outro programa.


  CREATE TABLE [dbo].[Faculty] (
    [Id]                 UNIQUEIDENTIFIER CONSTRAINT [DF_faculty_id] DEFAULT (newid()) NOT NULL,
    [Name]               NVARCHAR (1000)   NULL,
    [Domain]             NVARCHAR (1000)   NULL,
    [WebPage]            NVARCHAR (1000)   NULL,
    [Localization]       NVARCHAR (1000)   NULL,
    [Abbreviations]      NVARCHAR (1000)   NULL,
    [UniversityId]       UNIQUEIDENTIFIER  NOT NULL, 
    [CreationDate]       DATETIME2 (7)    CONSTRAINT [DF_faculty_creation_date] DEFAULT (getdate()) NOT NULL,
    [UpdateDate]         DATETIME2 (7)    NULL,
    [CreatedByUserId]           UNIQUEIDENTIFIER   NULL,
    [UpdatedByUserId]           UNIQUEIDENTIFIER   NULL,
    [Deleted]            BIT              NOT NULL DEFAULT(0),
    [OriginalUniversityId]       NVARCHAR (1000)   NULL,
    CONSTRAINT [PK_faculty] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_faculty_university] FOREIGN KEY ([UniversityId]) REFERENCES [dbo].[University] ([Id]),
);