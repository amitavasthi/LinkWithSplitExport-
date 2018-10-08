CREATE TABLE [resp].[{0}](
	[Id] [uniqueidentifier] NOT NULL,
	[IdRespondent] [uniqueidentifier] NOT NULL,
	[IdStudy] [uniqueidentifier] NOT NULL,
	[IdCategory] [uniqueidentifier] NULL,
	[NumericAnswer] [float] NULL,
	[TextAnswer] [nvarchar](4000) NULL,
 CONSTRAINT [PK_resp.Var_{0}] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]