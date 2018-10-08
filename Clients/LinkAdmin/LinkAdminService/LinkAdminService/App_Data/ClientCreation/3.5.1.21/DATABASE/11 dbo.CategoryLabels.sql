USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[CategoryLabels]    Script Date: 17/02/2016 10:34:47 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[CategoryLabels](
	[Id] [uniqueidentifier] NOT NULL,
	[IdCategory] [uniqueidentifier] NOT NULL,
	[IdLanguage] [int] NOT NULL,
	[Label] [nvarchar](max) NULL,
 CONSTRAINT [PK_CategoryLabels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]



ALTER TABLE [dbo].[CategoryLabels]  WITH CHECK ADD  CONSTRAINT [FK_CategoryLabels_Categories] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[Categories] ([Id])


ALTER TABLE [dbo].[CategoryLabels] CHECK CONSTRAINT [FK_CategoryLabels_Categories]



