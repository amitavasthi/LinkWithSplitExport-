USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[Hierarchies]    Script Date: 17/02/2016 10:32:08 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[Hierarchies](
	[Id] [uniqueidentifier] NOT NULL,
	[IdHierarchy] [uniqueidentifier] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Hierarchies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]




