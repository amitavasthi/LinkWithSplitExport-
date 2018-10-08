USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[QALogs]    Script Date: 17/02/2016 10:41:21 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[QALogs](
	[Id] [uniqueidentifier] NOT NULL,
	[IdUser] [uniqueidentifier] NOT NULL,
	[Source] [nvarchar](255) NOT NULL,
	[Identity] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[IdStudy] [uniqueidentifier] NOT NULL,
	[Status] [int] NOT NULL CONSTRAINT [DF_QALogs_Status]  DEFAULT ((0))
) ON [PRIMARY]



ALTER TABLE [dbo].[QALogs]  WITH CHECK ADD  CONSTRAINT [FK_QALogs_Studies] FOREIGN KEY([IdStudy])
REFERENCES [dbo].[Studies] ([Id])


ALTER TABLE [dbo].[QALogs] CHECK CONSTRAINT [FK_QALogs_Studies]


ALTER TABLE [dbo].[QALogs]  WITH CHECK ADD  CONSTRAINT [FK_QALogs_Users] FOREIGN KEY([IdUser])
REFERENCES [dbo].[Users] ([Id])


ALTER TABLE [dbo].[QALogs] CHECK CONSTRAINT [FK_QALogs_Users]



