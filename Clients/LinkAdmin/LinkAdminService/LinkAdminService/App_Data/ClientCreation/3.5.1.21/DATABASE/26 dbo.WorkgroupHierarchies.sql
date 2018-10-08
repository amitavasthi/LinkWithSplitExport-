USE [###CLIENTNAME###]


/****** Object:  Table [dbo].[WorkgroupHierarchies]    Script Date: 17/02/2016 10:42:11 ******/
SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[WorkgroupHierarchies](
	[Id] [uniqueidentifier] NOT NULL,
	[IdHierarchy] [uniqueidentifier] NOT NULL,
	[IdWorkgroup] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_WorkgroupHierarchies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



ALTER TABLE [dbo].[WorkgroupHierarchies]  WITH CHECK ADD  CONSTRAINT [FK_WorkgroupHierarchies_Hierarchies] FOREIGN KEY([IdHierarchy])
REFERENCES [dbo].[Hierarchies] ([Id])


ALTER TABLE [dbo].[WorkgroupHierarchies] CHECK CONSTRAINT [FK_WorkgroupHierarchies_Hierarchies]


ALTER TABLE [dbo].[WorkgroupHierarchies]  WITH CHECK ADD  CONSTRAINT [FK_WorkgroupHierarchies_Workgroups] FOREIGN KEY([IdWorkgroup])
REFERENCES [dbo].[Workgroups] ([Id])


ALTER TABLE [dbo].[WorkgroupHierarchies] CHECK CONSTRAINT [FK_WorkgroupHierarchies_Workgroups]



