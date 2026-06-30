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
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [AdminUsers] (
        [Id] bigint NOT NULL,
        [FullName] nvarchar(100) NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        [Role] int NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreationTime] datetimeoffset NOT NULL,
        [ModificationTime] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_AdminUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [BnplContracts] (
        [Id] bigint NOT NULL,
        [OrderId] bigint NOT NULL,
        [UserId] bigint NOT NULL,
        [TotalAmount] decimal(18,4) NOT NULL,
        [InstallmentCount] int NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreationTime] datetimeoffset NOT NULL,
        [ModificationTime] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_BnplContracts] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [Categories] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [ParentId] bigint NULL,
        [CreationTime] datetimeoffset NOT NULL,
        [ModificationTime] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [CreditLimits] (
        [Id] bigint NOT NULL,
        [UserId] bigint NOT NULL,
        [TotalLimit] decimal(18,2) NOT NULL,
        [UsedLimit] decimal(18,2) NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [CreationTime] datetimeoffset NOT NULL,
        [ModificationTime] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_CreditLimits] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [Inventorys] (
        [Id] bigint NOT NULL IDENTITY,
        [ProductId] bigint NOT NULL,
        [Quantity] int NOT NULL,
        [ReservedQuantity] int NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [CreationTime] datetimeoffset NOT NULL,
        [ModificationTime] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Inventorys] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [Products] (
        [Id] bigint NOT NULL,
        [SellerId] bigint NOT NULL,
        [CategoryId] bigint NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(100) NULL,
        [Price] decimal(18,4) NOT NULL,
        [Status] int NOT NULL,
        [CreationTime] datetimeoffset NOT NULL,
        [ModificationTime] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(100) NULL,
        [Family] nvarchar(100) NULL,
        [DisplayName] nvarchar(100) NULL,
        [NationalCode] nvarchar(100) NULL,
        [IsDeleted] bit NOT NULL,
        [IsEnabled] bit NOT NULL,
        [UserName] nvarchar(100) NULL,
        [NormalizedUserName] nvarchar(100) NULL,
        [Email] nvarchar(100) NULL,
        [NormalizedEmail] nvarchar(100) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(100) NULL,
        [SecurityStamp] nvarchar(100) NULL,
        [ConcurrencyStamp] nvarchar(100) NULL,
        [PhoneNumber] nvarchar(100) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] bigint NOT NULL IDENTITY,
        [AdminUserId] bigint NULL,
        [Action] nvarchar(100) NOT NULL,
        [EntityType] nvarchar(100) NOT NULL,
        [EntityId] nvarchar(100) NOT NULL,
        [OldValue] nvarchar(100) NULL,
        [NewValue] nvarchar(100) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreationTime] datetimeoffset NOT NULL,
        [ModificationTime] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AuditLogs_AdminUsers_AdminUserId] FOREIGN KEY ([AdminUserId]) REFERENCES [AdminUsers] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [Reports] (
        [Id] bigint NOT NULL IDENTITY,
        [AdminUserId] bigint NULL,
        [Type] int NOT NULL,
        [Status] int NOT NULL,
        [FilePath] nvarchar(100) NULL,
        [GeneratedAt] datetime2 NULL,
        [FailReason] nvarchar(100) NULL,
        [CreationTime] datetimeoffset NOT NULL,
        [ModificationTime] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Reports] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reports_AdminUsers_AdminUserId] FOREIGN KEY ([AdminUserId]) REFERENCES [AdminUsers] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [Installments] (
        [Id] bigint NOT NULL IDENTITY,
        [ContractId] bigint NOT NULL,
        [InstallmentNo] int NOT NULL,
        [Amount] decimal(18,4) NOT NULL,
        [DueDate] datetime2 NOT NULL,
        [PaidAt] datetime2 NULL,
        [Status] int NOT NULL,
        [BnplContractId] bigint NULL,
        [CreationTime] datetimeoffset NOT NULL,
        [ModificationTime] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Installments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Installments_BnplContracts_BnplContractId] FOREIGN KEY ([BnplContractId]) REFERENCES [BnplContracts] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [ProductImages] (
        [Id] uniqueidentifier NOT NULL,
        [ProductId] bigint NOT NULL,
        [Url] nvarchar(max) NOT NULL,
        [BucketKey] nvarchar(max) NOT NULL,
        [Type] int NOT NULL,
        [SortOrder] int NOT NULL,
        [UploadedAt] datetime2 NOT NULL,
        [CreationTime] datetimeoffset NOT NULL,
        [ModificationTime] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ProductImages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductImages_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] bigint NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] bigint NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] bigint NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE TABLE [Sessions] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] bigint NOT NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [IsActive] bit NOT NULL,
        [CreationTime] datetimeoffset NOT NULL,
        [ModificationTime] datetimeoffset NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Sessions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_AdminUserId] ON [AuditLogs] ([AdminUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CreditLimits_UserId] ON [CreditLimits] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Installments_BnplContractId] ON [Installments] ([BnplContractId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ProductImages_ProductId] ON [ProductImages] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Reports_AdminUserId] ON [Reports] ([AdminUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Sessions_UserId] ON [Sessions] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [Users] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260628220550_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260628220550_InitialCreate', N'9.0.0');
END;

COMMIT;
GO

