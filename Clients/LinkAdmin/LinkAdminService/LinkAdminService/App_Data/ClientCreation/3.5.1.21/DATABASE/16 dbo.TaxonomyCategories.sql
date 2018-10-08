USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[TaxonomyCategories]    Script Date: 17/02/2016 10:37:31 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[TaxonomyCategories](
	[Id] [uniqueidentifier] NOT NULL,
	[IdTaxonomyVariable] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Order] [int] NOT NULL CONSTRAINT [DF_TaxonomyCategories_Order]  DEFAULT ((0)),
	[CreationDate] [datetime] NOT NULL CONSTRAINT [DF_TaxonomyCategories_CreationDate]  DEFAULT ('2014/10/09'),
	[Value] [int] NOT NULL CONSTRAINT [DF_TaxonomyCategories_Value]  DEFAULT ((0)),
	[Color] [nvarchar](6) NULL,
	[IsScoreGroup] [bit] NOT NULL CONSTRAINT [DF_TaxonomyCategories_IsScoreGroup]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_TaxonomyCategories_Enabled_1]  DEFAULT ((1)),
	[Equation] [nvarchar](max) NULL,
 CONSTRAINT [PK_TaxonomyCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]



ALTER TABLE [dbo].[TaxonomyCategories]  WITH CHECK ADD  CONSTRAINT [FK_TaxonomyCategories_TaxonomyVariables] FOREIGN KEY([IdTaxonomyVariable])
REFERENCES [dbo].[TaxonomyVariables] ([Id])


ALTER TABLE [dbo].[TaxonomyCategories] CHECK CONSTRAINT [FK_TaxonomyCategories_TaxonomyVariables]



