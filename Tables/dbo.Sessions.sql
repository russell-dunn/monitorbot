CREATE TABLE [dbo].[Sessions]
(
[ID] [uniqueidentifier] NOT NULL,
[ProjectID] [uniqueidentifier] NOT NULL,
[AssemblyID] [uniqueidentifier] NOT NULL,
[UserID] [uniqueidentifier] NOT NULL,
[SessionDate] [datetime] NOT NULL,
[UserHostAddress] [varchar] (255) COLLATE Latin1_General_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Sessions] ADD CONSTRAINT [PK__Sessions__3214EC2756A5E169] PRIMARY KEY CLUSTERED  ([ID]) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [index_SessionsAssemblyID] ON [dbo].[Sessions] ([AssemblyID]) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [SessionsAssemblyIDIndex] ON [dbo].[Sessions] ([AssemblyID]) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [index_Sessions_AssemblyProjectUser] ON [dbo].[Sessions] ([AssemblyID], [ProjectID], [UserHostAddress], [UserID]) INCLUDE ([ID], [SessionDate]) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [SessionsProjectIDIndex] ON [dbo].[Sessions] ([ProjectID]) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [index_Sessions_All] ON [dbo].[Sessions] ([ProjectID], [UserID], [SessionDate], [ID], [AssemblyID], [UserHostAddress]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Sessions] ADD CONSTRAINT [FK__Sessions__Assemb__2F10007B] FOREIGN KEY ([AssemblyID]) REFERENCES [dbo].[Builds] ([AssemblyID])
GO
ALTER TABLE [dbo].[Sessions] ADD CONSTRAINT [FK__Sessions__Projec__2E1BDC42] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Projects] ([ID])
GO
