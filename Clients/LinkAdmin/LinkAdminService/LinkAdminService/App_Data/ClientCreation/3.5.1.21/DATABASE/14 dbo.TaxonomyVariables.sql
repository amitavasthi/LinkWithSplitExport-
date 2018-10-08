USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[TaxonomyVariables]    Script Date: 17/02/2016 10:37:01 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[TaxonomyVariables](
	[Id] [uniqueidentifier] NOT NULL,
	[Type] [int] NOT NULL CONSTRAINT [DF_TaxonomyVariables_Type]  DEFAULT ((0)),
	[Name] [nvarchar](255) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Weight] [bit] NOT NULL CONSTRAINT [DF_TaxonomyVariables_Weight]  DEFAULT ((0)),
	[Order] [int] NOT NULL CONSTRAINT [DF_TaxonomyVariables_Order_1]  DEFAULT ((0)),
	[Scale] [bit] NOT NULL CONSTRAINT [DF_TaxonomyVariables_Scale]  DEFAULT ((0)),
	[IdTaxonomyChapter] [uniqueidentifier] NULL,
 CONSTRAINT [PK_TaxonomyVariables] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]




