CREATE TABLE [dbo].[Projects]
(
[ID] [uniqueidentifier] NOT NULL,
[Name] [varchar] (255) COLLATE Latin1_General_CI_AS NULL,
[CryptoKey] [varchar] (max) COLLATE Latin1_General_CI_AS NULL,
[ProjectFileName] [varchar] (255) COLLATE Latin1_General_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Projects] ADD CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED  ([ID]) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [ProjectsIDIndex] ON [dbo].[Projects] ([ID]) ON [PRIMARY]
GO
