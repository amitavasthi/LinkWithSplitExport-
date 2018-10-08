USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[TaxonomyCategoryLinks]    Script Date: 17/02/2016 10:38:23 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[TaxonomyCategoryLinks](
	[Id] [uniqueidentifier] NOT NULL,
	[IdScoreGroup] [uniqueidentifier] NOT NULL,
	[IdTaxonomyCategory] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TaxonomyCategoryLinks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



ALTER TABLE [dbo].[TaxonomyCategoryLinks]  WITH CHECK ADD  CONSTRAINT [FK_TaxonomyCategoryLinks_TaxonomyCategories1] FOREIGN KEY([IdTaxonomyCategory])
REFERENCES [dbo].[TaxonomyCategories] ([Id])
ON DELETE CASCADE


ALTER TABLE [dbo].[TaxonomyCategoryLinks] CHECK CONSTRAINT [FK_TaxonomyCategoryLinks_TaxonomyCategories1]



