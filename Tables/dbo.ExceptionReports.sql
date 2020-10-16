CREATE TABLE [dbo].[ExceptionReports]
(
[ID] [uniqueidentifier] NOT NULL,
[ProjectID] [uniqueidentifier] NOT NULL,
[AssemblyID] [uniqueidentifier] NULL,
[UserID] [uniqueidentifier] NULL,
[CreationDate] [datetime] NULL,
[InsertionDate] [datetime] NULL,
[ExceptionType] [varchar] (255) COLLATE Latin1_General_CI_AS NULL,
[ExceptionMessage] [varchar] (255) COLLATE Latin1_General_CI_AS NULL,
[TypeName] [varchar] (255) COLLATE Latin1_General_CI_AS NULL,
[MethodName] [varchar] (255) COLLATE Latin1_General_CI_AS NULL,
[Unread] [bit] NULL,
[Fixed] [bit] NULL,
[Flag] [tinyint] NULL,
[Data] [varbinary] (max) NULL,
[HasAttachment] [bit] NULL,
[Completeness] [tinyint] NOT NULL CONSTRAINT [DF__Exception__Compl__29572725] DEFAULT ((1))
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ExceptionReports] ADD CONSTRAINT [PK_ExceptionReports] PRIMARY KEY NONCLUSTERED  ([ID]) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [ExceptionReportsAssemblyIDIndex] ON [dbo].[ExceptionReports] ([AssemblyID], [CreationDate] DESC) INCLUDE ([ExceptionMessage], [ExceptionType], [Fixed], [Flag], [HasAttachment], [ID], [InsertionDate], [MethodName], [ProjectID], [TypeName], [Unread], [UserID]) ON [PRIMARY]
GO
CREATE UNIQUE CLUSTERED INDEX [ExceptionReportsIDIndex] ON [dbo].[ExceptionReports] ([ID]) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [ExceptionReportsProjectIDIndex] ON [dbo].[ExceptionReports] ([ProjectID], [CreationDate] DESC) INCLUDE ([AssemblyID], [ExceptionMessage], [ExceptionType], [Fixed], [Flag], [HasAttachment], [ID], [InsertionDate], [MethodName], [TypeName], [Unread], [UserID]) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [ExceptionReportsReadnessCreationDateIndex] ON [dbo].[ExceptionReports] ([Unread], [CreationDate] DESC) INCLUDE ([AssemblyID], [ExceptionMessage], [ExceptionType], [Fixed], [Flag], [HasAttachment], [ID], [InsertionDate], [MethodName], [ProjectID], [TypeName], [UserID]) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [ExceptionReportsUserIDIndex] ON [dbo].[ExceptionReports] ([UserID]) ON [PRIMARY]
GO
