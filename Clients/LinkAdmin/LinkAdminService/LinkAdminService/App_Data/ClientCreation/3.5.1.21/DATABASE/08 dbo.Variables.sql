USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[Variables]    Script Date: 17/02/2016 10:33:53 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[Variables](
	[Id] [uniqueidentifier] NOT NULL,
	[IdStudy] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[IdChapter] [uniqueidentifier] NULL,
	[IdVariable] [uniqueidentifier] NULL,
	[Type] [int] NOT NULL CONSTRAINT [DF_Variables_Type]  DEFAULT ((0)),
	[RangeExpression] [nvarchar](50) NULL,
	[Formula] [text] NULL,
	[ClearText] [nvarchar](250) NULL,
	[AdditionalInfo] [nvarchar](50) NULL,
	[Option1] [nvarchar](50) NULL,
	[Option2] [nvarchar](50) NULL,
	[Option3] [nvarchar](50) NULL,
	[Option4] [nvarchar](50) NULL,
	[Option5] [nvarchar](50) NULL,
	[ReportFilter] [bit] NOT NULL CONSTRAINT [DF_Variables_ReportFilter]  DEFAULT ((0)),
	[ReportVariable] [bit] NOT NULL CONSTRAINT [DF_Variables_ReportVariable]  DEFAULT ((0)),
	[ScaleType] [tinyint] NOT NULL CONSTRAINT [DF_Variables_ScaleType]  DEFAULT ((0)),
	[ChapterOrder] [int] NULL,
	[VariableOrderInChapter] [int] NULL,
	[IdMeasure] [uniqueidentifier] NULL,
	[Order] [int] NOT NULL CONSTRAINT [DF_Variables_Order]  DEFAULT ((0)),
 CONSTRAINT [PK_Variables] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]



ALTER TABLE [dbo].[Variables]  WITH CHECK ADD  CONSTRAINT [FK_Variables_Studies] FOREIGN KEY([IdStudy])
REFERENCES [dbo].[Studies] ([Id])


ALTER TABLE [dbo].[Variables] CHECK CONSTRAINT [FK_Variables_Studies]



