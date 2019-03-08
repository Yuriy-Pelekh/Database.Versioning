--##1
CREATE TABLE [TestTable](
    [Id] [int] IDENTITY(1, 1) PRIMARY KEY,
    [Name] [nvarchar](250) NOT NULL,
    [UpdatedDate] [nchar](10) NOT NULL)
GO

--##2
INSERT INTO [TestTable] ([Name], [UpdatedDate])
     VALUES ('Test Item', GETUTCDATE())
GO

--##3
DELETE FROM [TestTable]
GO

--##4
DROP TABLE [TestTable]
GO
