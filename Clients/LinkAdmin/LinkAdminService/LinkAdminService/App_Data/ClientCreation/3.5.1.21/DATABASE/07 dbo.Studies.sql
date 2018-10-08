USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[Studies]    Script Date: 17/02/2016 10:33:31 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[Studies](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [text] NULL,
	[CreationDate] [datetime] NOT NULL,
	[IdUser] [uniqueidentifier] NOT NULL,
	[Status] [int] NOT NULL CONSTRAINT [DF_Studies_Status]  DEFAULT ((0)),
	[IdHierarchy] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Studies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]



ALTER TABLE [dbo].[Studies]  WITH CHECK ADD  CONSTRAINT [FK_Studies_Hierarchies] FOREIGN KEY([IdHierarchy])
REFERENCES [dbo].[Hierarchies] ([Id])


ALTER TABLE [dbo].[Studies] CHECK CONSTRAINT [FK_Studies_Hierarchies]



