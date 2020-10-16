CREATE TABLE [dbo].[Builds]
(
[AssemblyID] [uniqueidentifier] NOT NULL,
[ProjectID] [uniqueidentifier] NOT NULL,
[LastAccessDate] [datetime] NULL,
[Released] [bit] NULL,
[BuildDate] [datetime] NULL,
[BuildVersion] [varchar] (23) COLLATE Latin1_General_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Builds] ADD CONSTRAINT [PK_Builds] PRIMARY KEY NONCLUSTERED  ([AssemblyID]) ON [PRIMARY]
GO
CREATE UNIQUE CLUSTERED INDEX [BuildAssemblyIDIndex] ON [dbo].[Builds] ([AssemblyID]) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [BuildProjectIDIndex] ON [dbo].[Builds] ([ProjectID]) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [index_BuildsAll] ON [dbo].[Builds] ([ProjectID], [AssemblyID], [BuildDate]) INCLUDE ([BuildVersion]) ON [PRIMARY]
GO
