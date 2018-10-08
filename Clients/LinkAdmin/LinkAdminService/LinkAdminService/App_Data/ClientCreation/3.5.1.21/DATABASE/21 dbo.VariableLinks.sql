USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[VariableLinks]    Script Date: 17/02/2016 10:40:40 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[VariableLinks](
	[Id] [uniqueidentifier] NOT NULL,
	[IdVariable] [uniqueidentifier] NOT NULL,
	[IdTaxonomyVariable] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[QA] [bit] NOT NULL CONSTRAINT [DF_VariableLinks_QA]  DEFAULT ((0)),
 CONSTRAINT [PK_VariableLinks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



ALTER TABLE [dbo].[VariableLinks]  WITH CHECK ADD  CONSTRAINT [FK_VariableLinks_TaxonomyVariables] FOREIGN KEY([IdTaxonomyVariable])
REFERENCES [dbo].[TaxonomyVariables] ([Id])


ALTER TABLE [dbo].[VariableLinks] CHECK CONSTRAINT [FK_VariableLinks_TaxonomyVariables]


ALTER TABLE [dbo].[VariableLinks]  WITH CHECK ADD  CONSTRAINT [FK_VariableLinks_Variables] FOREIGN KEY([IdVariable])
REFERENCES [dbo].[Variables] ([Id])


ALTER TABLE [dbo].[VariableLinks] CHECK CONSTRAINT [FK_VariableLinks_Variables]



