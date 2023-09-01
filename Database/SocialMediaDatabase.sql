Create database SocialMediaApplication
use SocialMediaApplication

SET IDENTITY_INSERT Users ON;
create table Users(
userId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
username varchar(30) not null ,
name varchar(30) not null,
surname varchar(30) not null,
email varchar(30) null,
password varchar(200) not null,
refreshToken varchar(500) not null,
tokenCreated datetime2 not null,
tokenExpires datetime2 not null,
roleName varchar(30) not null foreign key references Role(roleName)
);

create table Role(
roleId int not null identity(1,1),
roleName varchar(30) not null primary key
);

insert into Role values ('String')
delete from Users where username='arberejupi'
select * from Users

create TABLE Post (
    Id INT identity(1,1) primary key,
    UserId INT NOT NULL,
    Content VARCHAR(MAX),
    CreatedAt DATETIME,
    FOREIGN KEY (UserId) REFERENCES [Users](userId)
);

-- Create Like Table
create TABLE [Like] (
    Id INT identity(1,1) primary key,
    UserId INT NOT NULL,
    PostId INT NOT NULL,
    CreatedAt DATETIME,
    FOREIGN KEY (UserId) REFERENCES [Users](userId),
    FOREIGN KEY (PostId) REFERENCES Post(Id)
);
SELECT name
FROM sys.foreign_keys
WHERE parent_object_id = OBJECT_ID('Comment') AND referenced_object_id = OBJECT_ID('Post');

ALTER TABLE Comment
DROP CONSTRAINT FK__Comment__PostId__693CA210; -- Drop the existing foreign key constraint

ALTER TABLE Comment
ADD CONSTRAINT FK__Comment__PostId__693CA210
FOREIGN KEY (PostId)
REFERENCES Post(Id)
ON DELETE CASCADE;
-- Create Comment Table
create TABLE Comment (
    Id INT identity(1,1) primary key,
    UserId INT NOT NULL,
    PostId INT NOT NULL,
    Content VARCHAR(MAX),
    CreatedAt DATETIME,
    FOREIGN KEY (UserId) REFERENCES [Users](userId),
    FOREIGN KEY (PostId) REFERENCES Post(Id)
);

CREATE TABLE Connection
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] INT NOT NULL,
    [FriendId] INT NOT NULL,
    [Status] NVARCHAR(20) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2,
    
    CONSTRAINT [FK_Connection_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([userId]),
    CONSTRAINT [FK_Connection_Users_FriendId] FOREIGN KEY ([FriendId]) REFERENCES [dbo].[Users]([userId])
);

CREATE TABLE FriendRequest
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] INT NOT NULL,
    [RequesterId] INT NOT NULL,
    [Status] NVARCHAR(20) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2,
    
    CONSTRAINT [FK_FriendRequest_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([userId]),
    CONSTRAINT [FK_FriendRequest_Users_RequesterId] FOREIGN KEY ([RequesterId]) REFERENCES [dbo].[Users]([userId])
);

create TABLE SavedPost (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId int NOT NULL,
    PostId INT NOT NULL,
    CONSTRAINT FK_SavedPost_User FOREIGN KEY (UserId) REFERENCES [dbo].[Users]([userId]),
    CONSTRAINT FK_SavedPost_Post FOREIGN KEY (PostId) REFERENCES Post(Id)
);

select* from Post
select * from SavedPost
select * from Users