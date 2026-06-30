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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220604_InitialIdentity'
)
BEGIN
    CREATE TABLE [IdentityRoles] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_IdentityRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220604_InitialIdentity'
)
BEGIN
    CREATE TABLE [IdentityUsers] (
        [Id] bigint NOT NULL IDENTITY,
        [PhoneNumber] nvarchar(20) NOT NULL,
        [FullName] nvarchar(200) NULL,
        [UserType] int NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreationTime] datetimeoffset NOT NULL,
        [ModificationTime] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_IdentityUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220604_InitialIdentity'
)
BEGIN
    CREATE TABLE [OtpRequests] (
        [Id] bigint NOT NULL IDENTITY,
        [PhoneNumber] nvarchar(20) NOT NULL,
        [Code] nvarchar(10) NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [IsUsed] bit NOT NULL,
        CONSTRAINT [PK_OtpRequests] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220604_InitialIdentity'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] bigint NOT NULL,
        [Token] nvarchar(500) NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [IsRevoked] bit NOT NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220604_InitialIdentity'
)
BEGIN
    CREATE UNIQUE INDEX [IX_IdentityRoles_Name] ON [IdentityRoles] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220604_InitialIdentity'
)
BEGIN
    CREATE UNIQUE INDEX [IX_IdentityUsers_PhoneNumber] ON [IdentityUsers] ([PhoneNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220604_InitialIdentity'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220604_InitialIdentity'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260628220604_InitialIdentity', N'9.0.0');
END;

COMMIT;
GO

