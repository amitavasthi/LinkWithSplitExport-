USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[Respondents]    Script Date: 17/02/2016 10:41:38 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[Respondents](
	[Id] [uniqueidentifier] NOT NULL,
	[IdStudy] [uniqueidentifier] NOT NULL,
	[OriginalRespondentID] [nvarchar](50) NULL,
 CONSTRAINT [PK_Responses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



ALTER TABLE [dbo].[Respondents]  WITH CHECK ADD  CONSTRAINT [FK_Respondents_Studies] FOREIGN KEY([IdStudy])
REFERENCES [dbo].[Studies] ([Id])


ALTER TABLE [dbo].[Respondents] CHECK CONSTRAINT [FK_Respondents_Studies]



