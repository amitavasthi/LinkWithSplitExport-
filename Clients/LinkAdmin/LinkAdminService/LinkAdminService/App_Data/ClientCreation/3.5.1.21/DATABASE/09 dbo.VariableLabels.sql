USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[VariableLabels]    Script Date: 17/02/2016 10:34:11 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[VariableLabels](
	[Id] [uniqueidentifier] NOT NULL,
	[IdVariable] [uniqueidentifier] NOT NULL,
	[IdLanguage] [int] NOT NULL,
	[Label] [nvarchar](max) NULL,
	[ReportLabel] [nvarchar](300) NULL,
 CONSTRAINT [PK_VariableLabels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]



ALTER TABLE [dbo].[VariableLabels]  WITH CHECK ADD  CONSTRAINT [FK_VariableLabels_Variables] FOREIGN KEY([IdVariable])
REFERENCES [dbo].[Variables] ([Id])


ALTER TABLE [dbo].[VariableLabels] CHECK CONSTRAINT [FK_VariableLabels_Variables]



