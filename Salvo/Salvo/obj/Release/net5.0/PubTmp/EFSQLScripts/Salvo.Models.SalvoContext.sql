IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211029211022_InitialCreate')
BEGIN
    CREATE TABLE [Players] (
        [Id] bigint NOT NULL IDENTITY,
        [Nombre] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [Password] nvarchar(max) NULL,
        CONSTRAINT [PK_Players] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211029211022_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211029211022_InitialCreate', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211029214422_updatePlayerEntity')
BEGIN
    EXEC sp_rename N'[Players].[Nombre]', N'Name', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211029214422_updatePlayerEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211029214422_updatePlayerEntity', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211101205939_addGameEntity')
BEGIN
    CREATE TABLE [Games] (
        [Id] bigint NOT NULL IDENTITY,
        [CreationDate] datetime2 NULL,
        CONSTRAINT [PK_Games] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211101205939_addGameEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211101205939_addGameEntity', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102225101_addGamePlayerEntity')
BEGIN
    CREATE TABLE [GamePlayers] (
        [Id] bigint NOT NULL IDENTITY,
        [GameId] bigint NOT NULL,
        [PlayerId] bigint NOT NULL,
        [JoinDate] datetime2 NOT NULL,
        CONSTRAINT [PK_GamePlayers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GamePlayers_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [Games] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_GamePlayers_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102225101_addGamePlayerEntity')
BEGIN
    CREATE INDEX [IX_GamePlayers_GameId] ON [GamePlayers] ([GameId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102225101_addGamePlayerEntity')
BEGIN
    CREATE INDEX [IX_GamePlayers_PlayerId] ON [GamePlayers] ([PlayerId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211102225101_addGamePlayerEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211102225101_addGamePlayerEntity', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211105204145_addShipEntity')
BEGIN
    CREATE TABLE [Ships] (
        [Id] bigint NOT NULL IDENTITY,
        [GamePlayerId] bigint NOT NULL,
        CONSTRAINT [PK_Ships] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Ships_GamePlayers_GamePlayerId] FOREIGN KEY ([GamePlayerId]) REFERENCES [GamePlayers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211105204145_addShipEntity')
BEGIN
    CREATE TABLE [ShipLocations] (
        [Id] bigint NOT NULL IDENTITY,
        [Location] nvarchar(max) NULL,
        [ShipId] bigint NOT NULL,
        CONSTRAINT [PK_ShipLocations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ShipLocations_Ships_ShipId] FOREIGN KEY ([ShipId]) REFERENCES [Ships] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211105204145_addShipEntity')
BEGIN
    CREATE INDEX [IX_ShipLocations_ShipId] ON [ShipLocations] ([ShipId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211105204145_addShipEntity')
BEGIN
    CREATE INDEX [IX_Ships_GamePlayerId] ON [Ships] ([GamePlayerId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211105204145_addShipEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211105204145_addShipEntity', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211105213547_FIXaddShipEntity')
BEGIN
    ALTER TABLE [Ships] ADD [Type] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211105213547_FIXaddShipEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211105213547_FIXaddShipEntity', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211110204721_addSalvoEntity')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GamePlayers]') AND [c].[name] = N'JoinDate');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [GamePlayers] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [GamePlayers] ALTER COLUMN [JoinDate] datetime2 NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211110204721_addSalvoEntity')
BEGIN
    CREATE TABLE [Salvos] (
        [Id] bigint NOT NULL IDENTITY,
        [Turn] int NOT NULL,
        [GamePlayerId] bigint NOT NULL,
        CONSTRAINT [PK_Salvos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Salvos_GamePlayers_GamePlayerId] FOREIGN KEY ([GamePlayerId]) REFERENCES [GamePlayers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211110204721_addSalvoEntity')
BEGIN
    CREATE TABLE [SalvoLocations] (
        [Id] bigint NOT NULL IDENTITY,
        [Location] nvarchar(max) NULL,
        [SalvoId] bigint NOT NULL,
        CONSTRAINT [PK_SalvoLocations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SalvoLocations_Salvos_SalvoId] FOREIGN KEY ([SalvoId]) REFERENCES [Salvos] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211110204721_addSalvoEntity')
BEGIN
    CREATE INDEX [IX_SalvoLocations_SalvoId] ON [SalvoLocations] ([SalvoId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211110204721_addSalvoEntity')
BEGIN
    CREATE INDEX [IX_Salvos_GamePlayerId] ON [Salvos] ([GamePlayerId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211110204721_addSalvoEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211110204721_addSalvoEntity', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211112204155_addScoreEntity')
BEGIN
    CREATE TABLE [Scores] (
        [Id] bigint NOT NULL IDENTITY,
        [GameId] bigint NOT NULL,
        [PlayerId] bigint NOT NULL,
        [Point] float NOT NULL,
        [FinishDate] datetime2 NULL,
        CONSTRAINT [PK_Scores] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Scores_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [Games] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Scores_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211112204155_addScoreEntity')
BEGIN
    CREATE INDEX [IX_Scores_GameId] ON [Scores] ([GameId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211112204155_addScoreEntity')
BEGIN
    CREATE INDEX [IX_Scores_PlayerId] ON [Scores] ([PlayerId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211112204155_addScoreEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211112204155_addScoreEntity', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211210221434_updatePlayer')
BEGIN
    ALTER TABLE [Players] ADD [Avatar] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211210221434_updatePlayer')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211210221434_updatePlayer', N'5.0.11');
END;
GO

COMMIT;
GO

