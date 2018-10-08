USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[TaxonomyVariableHierarchies]    Script Date: 17/02/2016 10:39:28 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[TaxonomyVariableHierarchies](
	[Id] [uniqueidentifier] NOT NULL,
	[IdTaxonomyVariable] [uniqueidentifier] NOT NULL,
	[IdHierarchy] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_TaxonomyVariableHierarchies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



ALTER TABLE [dbo].[TaxonomyVariableHierarchies]  WITH CHECK ADD  CONSTRAINT [FK_TaxonomyVariableHierarchies_Hierarchies] FOREIGN KEY([IdHierarchy])
REFERENCES [dbo].[Hierarchies] ([Id])


ALTER TABLE [dbo].[TaxonomyVariableHierarchies] CHECK CONSTRAINT [FK_TaxonomyVariableHierarchies_Hierarchies]


ALTER TABLE [dbo].[TaxonomyVariableHierarchies]  WITH CHECK ADD  CONSTRAINT [FK_TaxonomyVariableHierarchies_TaxonomyVariables] FOREIGN KEY([IdTaxonomyVariable])
REFERENCES [dbo].[TaxonomyVariables] ([Id])


ALTER TABLE [dbo].[TaxonomyVariableHierarchies] CHECK CONSTRAINT [FK_TaxonomyVariableHierarchies_TaxonomyVariables]



