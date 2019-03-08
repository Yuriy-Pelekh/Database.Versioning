CREATE DATABASE [{database}]
GO

USE [{database}]
GO

CREATE TABLE [Version](
    [Id] [int] IDENTITY(1, 1) PRIMARY KEY,
    [Version] [int] NOT NULL,
    [UpdatedDate] [datetime] NOT NULL)
GO

INSERT INTO [Version] ([Version], [UpdatedDate])
     VALUES (0, GETUTCDATE())
GO
