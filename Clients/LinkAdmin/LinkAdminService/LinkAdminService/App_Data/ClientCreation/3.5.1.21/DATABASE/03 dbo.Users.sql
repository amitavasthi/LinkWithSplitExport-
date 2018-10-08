USE [###CLIENTNAME###]


SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[Mail] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[Language] [nvarchar](255) NOT NULL DEFAULT ('English'),
	[LastLogon] [datetime] NULL,
	[Browser] [nvarchar](255) NULL,
	[Phone] [nvarchar](50) NULL,
	[PasswordReset] [uniqueidentifier] NULL,
	[PwdCreated] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]




