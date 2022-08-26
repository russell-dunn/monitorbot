CREATE TABLE [dbo].[Names]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[Name] [varchar] (255) COLLATE Latin1_General_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Names] ADD CONSTRAINT [NamePrimaryKey] PRIMARY KEY CLUSTERED  ([ID]) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [NameIndex] ON [dbo].[Names] ([Name]) ON [PRIMARY]
GO