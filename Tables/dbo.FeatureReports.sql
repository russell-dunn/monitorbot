CREATE TABLE [dbo].[FeatureReports]
(
[SessionID] [uniqueidentifier] NOT NULL,
[FeatureID] [int] NOT NULL,
[UsageCount] [bigint] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FeatureReports] ADD CONSTRAINT [FeatureReportPrimaryKey] PRIMARY KEY CLUSTERED  ([SessionID], [FeatureID]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FeatureReports] ADD CONSTRAINT [FK__FeatureRe__Featu__32E0915F] FOREIGN KEY ([FeatureID]) REFERENCES [dbo].[Features] ([ID])
GO
ALTER TABLE [dbo].[FeatureReports] ADD CONSTRAINT [FK__FeatureRe__Sessi__31EC6D26] FOREIGN KEY ([SessionID]) REFERENCES [dbo].[Sessions] ([ID])
GO
