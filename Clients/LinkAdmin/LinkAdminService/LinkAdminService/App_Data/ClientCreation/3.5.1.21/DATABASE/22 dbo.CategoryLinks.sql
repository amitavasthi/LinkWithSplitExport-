USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[CategoryLinks]    Script Date: 17/02/2016 10:40:53 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[CategoryLinks](
	[Id] [uniqueidentifier] NOT NULL,
	[IdVariable] [uniqueidentifier] NOT NULL,
	[IdCategory] [uniqueidentifier] NOT NULL,
	[IdTaxonomyCategory] [uniqueidentifier] NOT NULL,
	[IdTaxonomyVariable] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[QA] [bit] NOT NULL CONSTRAINT [DF_CategoryLinks_QA]  DEFAULT ((0)),
	[Notes] [nvarchar](500) NULL,
 CONSTRAINT [PK_CategoryLinks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



ALTER TABLE [dbo].[CategoryLinks]  WITH CHECK ADD  CONSTRAINT [FK_CategoryLinks_Categories] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[Categories] ([Id])


ALTER TABLE [dbo].[CategoryLinks] CHECK CONSTRAINT [FK_CategoryLinks_Categories]


ALTER TABLE [dbo].[CategoryLinks]  WITH CHECK ADD  CONSTRAINT [FK_CategoryLinks_TaxonomyCategories] FOREIGN KEY([IdTaxonomyCategory])
REFERENCES [dbo].[TaxonomyCategories] ([Id])


ALTER TABLE [dbo].[CategoryLinks] CHECK CONSTRAINT [FK_CategoryLinks_TaxonomyCategories]


ALTER TABLE [dbo].[CategoryLinks]  WITH CHECK ADD  CONSTRAINT [FK_CategoryLinks_TaxonomyVariables] FOREIGN KEY([IdTaxonomyVariable])
REFERENCES [dbo].[TaxonomyVariables] ([Id])


ALTER TABLE [dbo].[CategoryLinks] CHECK CONSTRAINT [FK_CategoryLinks_TaxonomyVariables]


ALTER TABLE [dbo].[CategoryLinks]  WITH CHECK ADD  CONSTRAINT [FK_CategoryLinks_Variables] FOREIGN KEY([IdVariable])
REFERENCES [dbo].[Variables] ([Id])


ALTER TABLE [dbo].[CategoryLinks] CHECK CONSTRAINT [FK_CategoryLinks_Variables]



