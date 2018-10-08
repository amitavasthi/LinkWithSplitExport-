USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[TaxonomyCategoryLabels]    Script Date: 17/02/2016 10:37:49 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[TaxonomyCategoryLabels](
	[Id] [uniqueidentifier] NOT NULL,
	[IdTaxonomyCategory] [uniqueidentifier] NOT NULL,
	[IdLanguage] [int] NOT NULL,
	[Label] [nvarchar](4000) NOT NULL,
	[CreationDate] [datetime] NOT NULL CONSTRAINT [DF_TaxonomyCategoryLabels_CreationDate]  DEFAULT ('2014/10/09'),
 CONSTRAINT [PK_TaxonomyCategoryLabels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



ALTER TABLE [dbo].[TaxonomyCategoryLabels]  WITH CHECK ADD  CONSTRAINT [FK_TaxonomyCategoryLabels_TaxonomyCategories] FOREIGN KEY([IdTaxonomyCategory])
REFERENCES [dbo].[TaxonomyCategories] ([Id])
ON DELETE CASCADE


ALTER TABLE [dbo].[TaxonomyCategoryLabels] CHECK CONSTRAINT [FK_TaxonomyCategoryLabels_TaxonomyCategories]



