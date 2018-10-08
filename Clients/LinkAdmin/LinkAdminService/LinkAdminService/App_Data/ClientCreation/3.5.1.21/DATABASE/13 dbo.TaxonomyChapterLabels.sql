USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[TaxonomyChapterLabels]    Script Date: 17/02/2016 10:39:06 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[TaxonomyChapterLabels](
	[Id] [uniqueidentifier] NOT NULL,
	[IdTaxonomyChapter] [uniqueidentifier] NOT NULL,
	[IdLanguage] [int] NOT NULL,
	[Label] [nvarchar](1000) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TaxonomyChapterLabel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



ALTER TABLE [dbo].[TaxonomyChapterLabels]  WITH CHECK ADD  CONSTRAINT [FK_TaxonomyChapterLabels_TaxonomyChapters] FOREIGN KEY([IdTaxonomyChapter])
REFERENCES [dbo].[TaxonomyChapters] ([Id])


ALTER TABLE [dbo].[TaxonomyChapterLabels] CHECK CONSTRAINT [FK_TaxonomyChapterLabels_TaxonomyChapters]



