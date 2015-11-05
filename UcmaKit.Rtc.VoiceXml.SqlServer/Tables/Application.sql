CREATE TABLE [dbo].[Application]
(
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_Application_Id] DEFAULT (NEWSEQUENTIALID()),
    [Uri] nvarchar(256) NOT NULL,
    CONSTRAINT [PK_Application] PRIMARY KEY ( [Id] ),
)
GO
