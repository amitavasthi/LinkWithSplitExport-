USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[Categories]    Script Date: 17/02/2016 10:34:33 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[Categories](
	[Id] [uniqueidentifier] NOT NULL,
	[IdVariable] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Formula] [text] NULL,
	[Factor] [smallint] NULL,
	[ClearText] [nvarchar](300) NULL,
	[Value] [int] NOT NULL CONSTRAINT [DF_Categories_Value]  DEFAULT ((0)),
	[Order] [int] NOT NULL CONSTRAINT [DF_Categories_Order]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_Categories_Enabled]  DEFAULT ((1)),
	[ExcludeBase] [bit] NOT NULL CONSTRAINT [DF_Categories_ExcludeBase_1]  DEFAULT ((0)),
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]



ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Variables] FOREIGN KEY([IdVariable])
REFERENCES [dbo].[Variables] ([Id])


ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_Variables]



