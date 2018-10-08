USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[TaxonomyCategoryHierarchies]    Script Date: 17/02/2016 10:39:49 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[TaxonomyCategoryHierarchies](
	[Id] [uniqueidentifier] NOT NULL,
	[IdTaxonomyCategory] [uniqueidentifier] NOT NULL,
	[IdHierarchy] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TaxonomyCategoryHierarchies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]




