USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[TaxonomyVariableLabels]    Script Date: 17/02/2016 10:37:16 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[TaxonomyVariableLabels](
	[Id] [uniqueidentifier] NOT NULL,
	[IdTaxonomyVariable] [uniqueidentifier] NOT NULL,
	[IdLanguage] [int] NOT NULL,
	[Label] [nvarchar](4000) NOT NULL,
	[CreationDate] [datetime] NOT NULL CONSTRAINT [DF_TaxonomyVariableLabels_CreationDate]  DEFAULT ('2014/10/09'),
 CONSTRAINT [PK_TaxonomyVariableLabels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



ALTER TABLE [dbo].[TaxonomyVariableLabels]  WITH CHECK ADD  CONSTRAINT [FK_TaxonomyVariableLabels_TaxonomyVariables] FOREIGN KEY([IdTaxonomyVariable])
REFERENCES [dbo].[TaxonomyVariables] ([Id])


ALTER TABLE [dbo].[TaxonomyVariableLabels] CHECK CONSTRAINT [FK_TaxonomyVariableLabels_TaxonomyVariables]



