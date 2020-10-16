CREATE TABLE [dbo].[Categories]
(
[ExceptionType] [varchar] (255) COLLATE Latin1_General_CI_AS NOT NULL,
[TypeName] [varchar] (255) COLLATE Latin1_General_CI_AS NOT NULL,
[MethodName] [varchar] (255) COLLATE Latin1_General_CI_AS NOT NULL,
[Completeness] [int] NOT NULL,
[CompletionDate] [datetime] NULL,
[Username] [varchar] (50) COLLATE Latin1_General_CI_AS NULL,
[CategoryID] [int] NOT NULL,
[ProjectID] [uniqueidentifier] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Categories] ADD CONSTRAINT [category_def] UNIQUE NONCLUSTERED  ([ProjectID], [ExceptionType], [TypeName], [MethodName]) ON [PRIMARY]
GO
