CREATE TABLE [dbo].[Endpoint]
(
    [Id] uniqueidentifier NOT NULL CONSTRAINT [DF_Endpoint_Id] DEFAULT (NEWSEQUENTIALID()),
    [Address] nvarchar(256) NOT NULL,
    [ApplicationId] uniqueidentifier NULL,
    CONSTRAINT [PK_Endpoint] PRIMARY KEY ( [Id] ),
    CONSTRAINT [FK_Endpoint_Application] FOREIGN KEY ( [ApplicationId] ) REFERENCES [Application] ( [Id] ),
)
GO
