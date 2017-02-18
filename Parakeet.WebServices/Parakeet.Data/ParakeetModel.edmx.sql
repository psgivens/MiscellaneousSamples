
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 03/27/2013 05:33:11
-- Generated from EDMX file: E:\HgSC\psgivens\sandbox\Parakeet.WebServices\Parakeet.Data\ParakeetModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Parakeet];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UserTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tasks] DROP CONSTRAINT [FK_UserTask];
GO
IF OBJECT_ID(N'[dbo].[FK_CommentTag_Comment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CommentTag] DROP CONSTRAINT [FK_CommentTag_Comment];
GO
IF OBJECT_ID(N'[dbo].[FK_CommentTag_Tag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CommentTag] DROP CONSTRAINT [FK_CommentTag_Tag];
GO
IF OBJECT_ID(N'[dbo].[FK_TagTask_Tag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TagTask] DROP CONSTRAINT [FK_TagTask_Tag];
GO
IF OBJECT_ID(N'[dbo].[FK_TagTask_Task]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TagTask] DROP CONSTRAINT [FK_TagTask_Task];
GO
IF OBJECT_ID(N'[dbo].[FK_RoleUser_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoleUser] DROP CONSTRAINT [FK_RoleUser_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_RoleUser_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoleUser] DROP CONSTRAINT [FK_RoleUser_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UserQuery_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserQuery] DROP CONSTRAINT [FK_UserQuery_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UserQuery_Query]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserQuery] DROP CONSTRAINT [FK_UserQuery_Query];
GO
IF OBJECT_ID(N'[dbo].[FK_UserComment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Comments] DROP CONSTRAINT [FK_UserComment];
GO
IF OBJECT_ID(N'[dbo].[FK_UserComment1_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserComment1] DROP CONSTRAINT [FK_UserComment1_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UserComment1_Comment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserComment1] DROP CONSTRAINT [FK_UserComment1_Comment];
GO
IF OBJECT_ID(N'[dbo].[FK_RoleQuery]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Queries] DROP CONSTRAINT [FK_RoleQuery];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tasks] DROP CONSTRAINT [FK_TaskTask];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskComment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Comments] DROP CONSTRAINT [FK_TaskComment];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskComment1_Task]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskComment1] DROP CONSTRAINT [FK_TaskComment1_Task];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskComment1_Comment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskComment1] DROP CONSTRAINT [FK_TaskComment1_Comment];
GO
IF OBJECT_ID(N'[dbo].[FK_QueryTag_Query]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[QueryTag] DROP CONSTRAINT [FK_QueryTag_Query];
GO
IF OBJECT_ID(N'[dbo].[FK_QueryTag_Tag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[QueryTag] DROP CONSTRAINT [FK_QueryTag_Tag];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskProgressEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProgressEntries] DROP CONSTRAINT [FK_TaskProgressEntry];
GO
IF OBJECT_ID(N'[dbo].[FK_UserProgressEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProgressEntries] DROP CONSTRAINT [FK_UserProgressEntry];
GO
IF OBJECT_ID(N'[dbo].[FK_UserProgressEntry1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProgressEntries] DROP CONSTRAINT [FK_UserProgressEntry1];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[Tasks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tasks];
GO
IF OBJECT_ID(N'[dbo].[Comments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Comments];
GO
IF OBJECT_ID(N'[dbo].[Tags]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tags];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[Queries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Queries];
GO
IF OBJECT_ID(N'[dbo].[ProgressEntries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProgressEntries];
GO
IF OBJECT_ID(N'[dbo].[CommentTag]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommentTag];
GO
IF OBJECT_ID(N'[dbo].[TagTask]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TagTask];
GO
IF OBJECT_ID(N'[dbo].[RoleUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RoleUser];
GO
IF OBJECT_ID(N'[dbo].[UserQuery]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserQuery];
GO
IF OBJECT_ID(N'[dbo].[UserComment1]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserComment1];
GO
IF OBJECT_ID(N'[dbo].[TaskComment1]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TaskComment1];
GO
IF OBJECT_ID(N'[dbo].[QueryTag]', 'U') IS NOT NULL
    DROP TABLE [dbo].[QueryTag];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Tasks'
CREATE TABLE [dbo].[Tasks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [Parent_Id] int  NULL
);
GO

-- Creating table 'Comments'
CREATE TABLE [dbo].[Comments] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TimeStamp] datetime  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [UserId] int  NOT NULL,
    [TaskId] int  NULL
);
GO

-- Creating table 'Tags'
CREATE TABLE [dbo].[Tags] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Queries'
CREATE TABLE [dbo].[Queries] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Role_Id] int  NULL
);
GO

-- Creating table 'ProgressEntries'
CREATE TABLE [dbo].[ProgressEntries] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [TaskId] int  NOT NULL,
    [AssociatedUserId] int  NOT NULL,
    [ReportingUserId] int  NOT NULL
);
GO

-- Creating table 'CommentTag'
CREATE TABLE [dbo].[CommentTag] (
    [Comments_Id] int  NOT NULL,
    [Tags_Id] int  NOT NULL
);
GO

-- Creating table 'TagTask'
CREATE TABLE [dbo].[TagTask] (
    [Tags_Id] int  NOT NULL,
    [Tasks_Id] int  NOT NULL
);
GO

-- Creating table 'RoleUser'
CREATE TABLE [dbo].[RoleUser] (
    [Roles_Id] int  NOT NULL,
    [Users_Id] int  NOT NULL
);
GO

-- Creating table 'UserQuery'
CREATE TABLE [dbo].[UserQuery] (
    [Users_Id] int  NOT NULL,
    [Queries_Id] int  NOT NULL
);
GO

-- Creating table 'UserComment1'
CREATE TABLE [dbo].[UserComment1] (
    [MentionedUsers_Id] int  NOT NULL,
    [MentionedIn_Id] int  NOT NULL
);
GO

-- Creating table 'TaskComment1'
CREATE TABLE [dbo].[TaskComment1] (
    [MentionedTasks_Id] int  NOT NULL,
    [MentionedIn_Id] int  NOT NULL
);
GO

-- Creating table 'QueryTag'
CREATE TABLE [dbo].[QueryTag] (
    [QueryTag_Tag_Id] int  NOT NULL,
    [Tags_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tasks'
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT [PK_Tasks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Comments'
ALTER TABLE [dbo].[Comments]
ADD CONSTRAINT [PK_Comments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [PK_Tags]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Queries'
ALTER TABLE [dbo].[Queries]
ADD CONSTRAINT [PK_Queries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProgressEntries'
ALTER TABLE [dbo].[ProgressEntries]
ADD CONSTRAINT [PK_ProgressEntries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Comments_Id], [Tags_Id] in table 'CommentTag'
ALTER TABLE [dbo].[CommentTag]
ADD CONSTRAINT [PK_CommentTag]
    PRIMARY KEY NONCLUSTERED ([Comments_Id], [Tags_Id] ASC);
GO

-- Creating primary key on [Tags_Id], [Tasks_Id] in table 'TagTask'
ALTER TABLE [dbo].[TagTask]
ADD CONSTRAINT [PK_TagTask]
    PRIMARY KEY NONCLUSTERED ([Tags_Id], [Tasks_Id] ASC);
GO

-- Creating primary key on [Roles_Id], [Users_Id] in table 'RoleUser'
ALTER TABLE [dbo].[RoleUser]
ADD CONSTRAINT [PK_RoleUser]
    PRIMARY KEY NONCLUSTERED ([Roles_Id], [Users_Id] ASC);
GO

-- Creating primary key on [Users_Id], [Queries_Id] in table 'UserQuery'
ALTER TABLE [dbo].[UserQuery]
ADD CONSTRAINT [PK_UserQuery]
    PRIMARY KEY NONCLUSTERED ([Users_Id], [Queries_Id] ASC);
GO

-- Creating primary key on [MentionedUsers_Id], [MentionedIn_Id] in table 'UserComment1'
ALTER TABLE [dbo].[UserComment1]
ADD CONSTRAINT [PK_UserComment1]
    PRIMARY KEY NONCLUSTERED ([MentionedUsers_Id], [MentionedIn_Id] ASC);
GO

-- Creating primary key on [MentionedTasks_Id], [MentionedIn_Id] in table 'TaskComment1'
ALTER TABLE [dbo].[TaskComment1]
ADD CONSTRAINT [PK_TaskComment1]
    PRIMARY KEY NONCLUSTERED ([MentionedTasks_Id], [MentionedIn_Id] ASC);
GO

-- Creating primary key on [QueryTag_Tag_Id], [Tags_Id] in table 'QueryTag'
ALTER TABLE [dbo].[QueryTag]
ADD CONSTRAINT [PK_QueryTag]
    PRIMARY KEY NONCLUSTERED ([QueryTag_Tag_Id], [Tags_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [UserId] in table 'Tasks'
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT [FK_UserTask]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTask'
CREATE INDEX [IX_FK_UserTask]
ON [dbo].[Tasks]
    ([UserId]);
GO

-- Creating foreign key on [Comments_Id] in table 'CommentTag'
ALTER TABLE [dbo].[CommentTag]
ADD CONSTRAINT [FK_CommentTag_Comment]
    FOREIGN KEY ([Comments_Id])
    REFERENCES [dbo].[Comments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Tags_Id] in table 'CommentTag'
ALTER TABLE [dbo].[CommentTag]
ADD CONSTRAINT [FK_CommentTag_Tag]
    FOREIGN KEY ([Tags_Id])
    REFERENCES [dbo].[Tags]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CommentTag_Tag'
CREATE INDEX [IX_FK_CommentTag_Tag]
ON [dbo].[CommentTag]
    ([Tags_Id]);
GO

-- Creating foreign key on [Tags_Id] in table 'TagTask'
ALTER TABLE [dbo].[TagTask]
ADD CONSTRAINT [FK_TagTask_Tag]
    FOREIGN KEY ([Tags_Id])
    REFERENCES [dbo].[Tags]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Tasks_Id] in table 'TagTask'
ALTER TABLE [dbo].[TagTask]
ADD CONSTRAINT [FK_TagTask_Task]
    FOREIGN KEY ([Tasks_Id])
    REFERENCES [dbo].[Tasks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TagTask_Task'
CREATE INDEX [IX_FK_TagTask_Task]
ON [dbo].[TagTask]
    ([Tasks_Id]);
GO

-- Creating foreign key on [Roles_Id] in table 'RoleUser'
ALTER TABLE [dbo].[RoleUser]
ADD CONSTRAINT [FK_RoleUser_Role]
    FOREIGN KEY ([Roles_Id])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Users_Id] in table 'RoleUser'
ALTER TABLE [dbo].[RoleUser]
ADD CONSTRAINT [FK_RoleUser_User]
    FOREIGN KEY ([Users_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RoleUser_User'
CREATE INDEX [IX_FK_RoleUser_User]
ON [dbo].[RoleUser]
    ([Users_Id]);
GO

-- Creating foreign key on [Users_Id] in table 'UserQuery'
ALTER TABLE [dbo].[UserQuery]
ADD CONSTRAINT [FK_UserQuery_User]
    FOREIGN KEY ([Users_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Queries_Id] in table 'UserQuery'
ALTER TABLE [dbo].[UserQuery]
ADD CONSTRAINT [FK_UserQuery_Query]
    FOREIGN KEY ([Queries_Id])
    REFERENCES [dbo].[Queries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserQuery_Query'
CREATE INDEX [IX_FK_UserQuery_Query]
ON [dbo].[UserQuery]
    ([Queries_Id]);
GO

-- Creating foreign key on [UserId] in table 'Comments'
ALTER TABLE [dbo].[Comments]
ADD CONSTRAINT [FK_UserComment]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserComment'
CREATE INDEX [IX_FK_UserComment]
ON [dbo].[Comments]
    ([UserId]);
GO

-- Creating foreign key on [MentionedUsers_Id] in table 'UserComment1'
ALTER TABLE [dbo].[UserComment1]
ADD CONSTRAINT [FK_UserComment1_User]
    FOREIGN KEY ([MentionedUsers_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [MentionedIn_Id] in table 'UserComment1'
ALTER TABLE [dbo].[UserComment1]
ADD CONSTRAINT [FK_UserComment1_Comment]
    FOREIGN KEY ([MentionedIn_Id])
    REFERENCES [dbo].[Comments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserComment1_Comment'
CREATE INDEX [IX_FK_UserComment1_Comment]
ON [dbo].[UserComment1]
    ([MentionedIn_Id]);
GO

-- Creating foreign key on [Role_Id] in table 'Queries'
ALTER TABLE [dbo].[Queries]
ADD CONSTRAINT [FK_RoleQuery]
    FOREIGN KEY ([Role_Id])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RoleQuery'
CREATE INDEX [IX_FK_RoleQuery]
ON [dbo].[Queries]
    ([Role_Id]);
GO

-- Creating foreign key on [Parent_Id] in table 'Tasks'
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT [FK_TaskTask]
    FOREIGN KEY ([Parent_Id])
    REFERENCES [dbo].[Tasks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskTask'
CREATE INDEX [IX_FK_TaskTask]
ON [dbo].[Tasks]
    ([Parent_Id]);
GO

-- Creating foreign key on [TaskId] in table 'Comments'
ALTER TABLE [dbo].[Comments]
ADD CONSTRAINT [FK_TaskComment]
    FOREIGN KEY ([TaskId])
    REFERENCES [dbo].[Tasks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskComment'
CREATE INDEX [IX_FK_TaskComment]
ON [dbo].[Comments]
    ([TaskId]);
GO

-- Creating foreign key on [MentionedTasks_Id] in table 'TaskComment1'
ALTER TABLE [dbo].[TaskComment1]
ADD CONSTRAINT [FK_TaskComment1_Task]
    FOREIGN KEY ([MentionedTasks_Id])
    REFERENCES [dbo].[Tasks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [MentionedIn_Id] in table 'TaskComment1'
ALTER TABLE [dbo].[TaskComment1]
ADD CONSTRAINT [FK_TaskComment1_Comment]
    FOREIGN KEY ([MentionedIn_Id])
    REFERENCES [dbo].[Comments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskComment1_Comment'
CREATE INDEX [IX_FK_TaskComment1_Comment]
ON [dbo].[TaskComment1]
    ([MentionedIn_Id]);
GO

-- Creating foreign key on [QueryTag_Tag_Id] in table 'QueryTag'
ALTER TABLE [dbo].[QueryTag]
ADD CONSTRAINT [FK_QueryTag_Query]
    FOREIGN KEY ([QueryTag_Tag_Id])
    REFERENCES [dbo].[Queries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Tags_Id] in table 'QueryTag'
ALTER TABLE [dbo].[QueryTag]
ADD CONSTRAINT [FK_QueryTag_Tag]
    FOREIGN KEY ([Tags_Id])
    REFERENCES [dbo].[Tags]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_QueryTag_Tag'
CREATE INDEX [IX_FK_QueryTag_Tag]
ON [dbo].[QueryTag]
    ([Tags_Id]);
GO

-- Creating foreign key on [TaskId] in table 'ProgressEntries'
ALTER TABLE [dbo].[ProgressEntries]
ADD CONSTRAINT [FK_TaskProgressEntry]
    FOREIGN KEY ([TaskId])
    REFERENCES [dbo].[Tasks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskProgressEntry'
CREATE INDEX [IX_FK_TaskProgressEntry]
ON [dbo].[ProgressEntries]
    ([TaskId]);
GO

-- Creating foreign key on [AssociatedUserId] in table 'ProgressEntries'
ALTER TABLE [dbo].[ProgressEntries]
ADD CONSTRAINT [FK_UserProgressEntry]
    FOREIGN KEY ([AssociatedUserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserProgressEntry'
CREATE INDEX [IX_FK_UserProgressEntry]
ON [dbo].[ProgressEntries]
    ([AssociatedUserId]);
GO

-- Creating foreign key on [ReportingUserId] in table 'ProgressEntries'
ALTER TABLE [dbo].[ProgressEntries]
ADD CONSTRAINT [FK_UserProgressEntry1]
    FOREIGN KEY ([ReportingUserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserProgressEntry1'
CREATE INDEX [IX_FK_UserProgressEntry1]
ON [dbo].[ProgressEntries]
    ([ReportingUserId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------