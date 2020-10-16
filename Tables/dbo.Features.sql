CREATE TABLE [dbo].[Features]
(
[ID] [int] NOT NULL IDENTITY(1, 1),
[Name] [varchar] (255) COLLATE Latin1_General_CI_AS NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Features] ADD CONSTRAINT [FeaturePrimaryKey] PRIMARY KEY CLUSTERED  ([ID]) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [FeatureIndex] ON [dbo].[Features] ([Name]) ON [PRIMARY]
GO
