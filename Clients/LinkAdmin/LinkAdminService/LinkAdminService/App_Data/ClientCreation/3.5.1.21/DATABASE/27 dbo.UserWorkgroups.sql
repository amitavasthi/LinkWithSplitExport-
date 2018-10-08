USE [###CLIENTNAME###]


SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON


CREATE TABLE [dbo].[UserWorkgroups](
	[Id] [uniqueidentifier] NOT NULL,
	[IdUser] [uniqueidentifier] NOT NULL,
	[IdWorkgroup] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_UserWorkgroups] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



ALTER TABLE [dbo].[UserWorkgroups]  WITH CHECK ADD  CONSTRAINT [FK_UserWorkgroups_Users] FOREIGN KEY([IdUser])
REFERENCES [dbo].[Users] ([Id])


ALTER TABLE [dbo].[UserWorkgroups] CHECK CONSTRAINT [FK_UserWorkgroups_Users]


ALTER TABLE [dbo].[UserWorkgroups]  WITH CHECK ADD  CONSTRAINT [FK_UserWorkgroups_Workgroups] FOREIGN KEY([IdWorkgroup])
REFERENCES [dbo].[Workgroups] ([Id])


ALTER TABLE [dbo].[UserWorkgroups] CHECK CONSTRAINT [FK_UserWorkgroups_Workgroups]



